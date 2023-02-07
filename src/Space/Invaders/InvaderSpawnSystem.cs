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
    private ReadOnlyResource<GameState> _gameState;
    private EventsReader<GameEndedEvent> _gameEnded;

    private Entity _enemyContainer;

    public void Init(in SystemInitializer init)
    {
        _entityManager = init.GetEntityManager();
        _assetsManager = init.GetAssetsManager();
        _componentsManager = init.GetComponentManager();
        _gameStart = init.GetEventsReader<GameStartEvent>();
        _gameEnded = init.GetEventsReader<GameEndedEvent>();
        _gameState = init.GetReadOnlyResource<GameState>();
    }

    public void Update()
    {
        if (_gameEnded.HasEvents())
        {
            if (_enemyContainer.IsValid)
            {
                _entityManager.Destroy(ref _enemyContainer);
            }
            return;
        }

        Debug.Assert(_enemyContainer.IsInvalid);
        ref readonly var gameState = ref _gameState.Get();
        _enemyContainer = _entityManager.Create();
        _componentsManager.AddComponent(_enemyContainer, Transform2D.Default); //NOTE(Jens): Workaround for entity tracking in Sparse Component pools.. 

        const uint invaderBlockSize = 18u;
        const uint invaderBlockHeight = 18u;
        var invaderHalfSize = SpriteRectangles.Monster2_0.Width / 2f;
        var boardWidth = _gameState.Get().BoardSize.Width;
        var offsetX = (boardWidth - gameState.InvaderColumns * invaderBlockSize + invaderHalfSize) / 2f;
        const uint offsetY = 220;

        for (var row = 0u; row < gameState.InvaderRows; ++row)
        {
            for (var col = 0u; col < gameState.InvaderColumns; ++col)
            {
                SpawnEnemy(offsetX + col * invaderBlockSize, offsetY - row * invaderBlockHeight);
            }
        }
    }

    private void SpawnEnemy(float x, float y)
    {
        var entity = _entityManager.Create();
        _componentsManager.AddComponent(entity, Transform2D.Default with
        {
            Position = new(x, y)
        });
        _componentsManager.AddComponent(entity, Sprite.Default with
        {
            Asset = _assetsManager.Load(AssetRegistry.Manifest.Textures.GameAtlas),
            SourceRect = SpriteRectangles.Monster1_0,
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
            Sprite1 = SpriteRectangles.Monster1_0,
            Sprite2 = SpriteRectangles.Monster1_1
        });
        _componentsManager.AddComponent(entity, new BoxCollider2D
        {
            Size = new(SpriteRectangles.Monster1_0.Width, SpriteRectangles.Monster1_0.Height),
            Category = CollisionCategories.Invader
        });
        _entityManager.Attach(_enemyContainer, entity);
    }


    public bool ShouldRun() => _gameStart.HasEvents() || _gameEnded.HasEvents();
}
