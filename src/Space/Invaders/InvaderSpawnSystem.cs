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

        var boardWidth = _gameState.Get().BoardSize.Width;
        var offsetX = (boardWidth - gameState.InvaderColumns * GameConstants.InvaderBlockWidth + GameConstants.InvaderHalfWidth) / 2f;
        const uint offsetY = 220;



        for (var row = 0u; row < gameState.InvaderRows; ++row)
        {
            var type = row switch
            {
                0 => InvaderType.Super,
                1 or 2 => InvaderType.Advanced,
                _ => InvaderType.Basic
            };
            for (var col = 0u; col < gameState.InvaderColumns; ++col)
            {
                SpawnInvader(type, offsetX + col * GameConstants.InvaderBlockWidth, offsetY - row * GameConstants.InvaderBlockHeight, gameState);
            }
        }
    }

    private void SpawnInvader(InvaderType type, float x, float y, in GameState gameState)
    {
        //NOTE(Jens): we need to rework how this works to support different enemy types
        var entity = _entityManager.Create();
        ref readonly var spriteRectangle = ref SpriteRectangles.Invaders[(int)type][0];

        _componentsManager.AddComponent(entity, Transform2D.Default with
        {
            Position = new(x, y)
        });

        _componentsManager.AddComponent(entity, Sprite.Default with
        {
            Asset = _assetsManager.Load(AssetRegistry.Manifest.Textures.GameAtlas),
            SourceRect = spriteRectangle,
            //Pivot = Vector2.Zero,
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
            InvaderWidth = spriteRectangle.Width,
            Type = type
        });
        _componentsManager.AddComponent(entity, new BoxCollider2D
        {
            Size = new(spriteRectangle.Width, spriteRectangle.Height),
            Category = CollisionCategories.Invader,
            CollidesWith = CollisionCategories.Shield | CollisionCategories.Player,
            Pivot = new Vector2(0.5f)
        });
        _entityManager.Attach(_enemyContainer, entity);
    }


    public bool ShouldRun() => _gameStart.HasEvents() || _gameEnded.HasEvents() || _levelCompleted.HasEvents();
}
