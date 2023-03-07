using System.Diagnostics;
using System.Numerics;
using Space.Assets;
using Space.Events;
using Space.Game;
using Titan.Assets;
using Titan.BuiltIn.Components;
using Titan.ECS;
using Titan.ECS.Entities;
using Titan.Events;
using Titan.Systems;

namespace Space.Invaders;

internal struct InvaderSpawnSystem : ISystem
{
    private EntityManager _entityManager;
    private AssetsManager _assetsManager;
    private ComponentManager _componentsManager;
    private EventsReader<GameStartEvent> _gameStart;
    private EventsReader<GameEndedEvent> _gameEnded;
    private EventsReader<LevelCompletedEvent> _levelCompleted;
    private ReadOnlyResource<GameState> _gameState;

    private Entity _enemyContainer;

    public void Init(in SystemInitializer init)
    {
        _entityManager = init.GetEntityManager();
        _assetsManager = init.GetAssetsManager();
        _componentsManager = init.GetComponentManager();
        _gameStart = init.GetEventsReader<GameStartEvent>();
        _gameEnded = init.GetEventsReader<GameEndedEvent>();
        _levelCompleted = init.GetEventsReader<LevelCompletedEvent>();
        _gameState = init.GetReadOnlyResource<GameState>();
    }

    public void Update()
    {
        //NOTE(Jens): This system will only run on GameStart, GameEnded. It's safe to destroy the entity container every "reset"
        if (_enemyContainer.IsValid)
        {
            _entityManager.Destroy(ref _enemyContainer);
        }

        if (_gameEnded.HasEvents())
        {
            return;
        }

        Debug.Assert(_enemyContainer.IsInvalid);
        ref readonly var gameState = ref _gameState.Get();
        _enemyContainer = _entityManager.Create();
        _componentsManager.AddComponent(_enemyContainer, Transform2D.Default); //NOTE(Jens): Workaround for entity tracking in Sparse Component pools.. 

        const uint invaderBlockSize = 18u;
        const uint invaderBlockHeight = 18u;
        const uint invaderWidth = 12;
        const uint invaderHalfSize = invaderWidth / 2;
        var boardWidth = _gameState.Get().BoardSize.Width;
        var offsetX = (boardWidth - gameState.InvaderColumns * invaderBlockSize + invaderHalfSize) / 2f;
        const uint offsetY = 220;

        for (var row = 0u; row < gameState.InvaderRows; ++row)
        {
            for (var col = 0u; col < gameState.InvaderColumns; ++col)
            {
                SpawnInvader(offsetX + col * invaderBlockSize, offsetY - row * invaderBlockHeight, gameState);
            }
        }
    }

    private void SpawnInvader(float x, float y, in GameState gameState)
    {
        //NOTE(Jens): we need to rework how this works to support different enemy types
        var entity = _entityManager.Create();
        _componentsManager.AddComponent(entity, Transform2D.Default with
        {
            Position = new(x, y)
        });
        _componentsManager.AddComponent(entity, Sprite.Default with
        {
            Asset = _assetsManager.Load(AssetRegistry.Manifest.Textures.GameAtlas),
            SourceRect = SpriteRectangles.Invaders[0][0],
            Pivot = new Vector2(0),
            Color = Random.Shared.Next(0, 3) switch
            {
                0 => ColorPalette.Lighter,
                1 => ColorPalette.Darker,
                2 or _ => ColorPalette.Middle
            }
        });
        _componentsManager.AddComponent(entity, new InvaderComponent
        {
            ShootingCooldown = gameState.InvaderMinShootingCooldown,
            InvaderWidth = SpriteRectangles.Invaders[0][0].Width
        });
        _componentsManager.AddComponent(entity, new BoxCollider2D
        {
            Size = new(SpriteRectangles.Invaders[0][0].Width, SpriteRectangles.Invaders[0][0].Height),
            Category = CollisionCategories.Invader,
            CollidesWith = CollisionCategories.Shield | CollisionCategories.Player
        });
        _entityManager.Attach(_enemyContainer, entity);
    }


    public bool ShouldRun() => _gameStart.HasEvents() || _gameEnded.HasEvents() || _levelCompleted.HasEvents();
}
