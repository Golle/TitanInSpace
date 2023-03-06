using System.Diagnostics;
using System.Numerics;
using Space.Assets;
using Space.Events;
using Space.Game;
using Titan.Assets;
using Titan.BuiltIn.Components;
using Titan.ECS;
using Titan.ECS.Components;
using Titan.ECS.Entities;
using Titan.Events;
using Titan.Systems;

namespace Space.Shields;

internal struct ShieldComponent : IComponent
{
    public uint Damage;
}

internal struct ShieldSpawnSystem : ISystem
{
    private AssetsManager _assetsManager;
    private EntityManager _entityManager;
    private ComponentManager _componentsManager;
    private ReadOnlyResource<GameState> _gameState;
    private EventsReader<GameStartEvent> _gameStart;
    private EventsReader<GameEndedEvent> _gameEnded;

    private Entity _container;

    public void Init(in SystemInitializer init)
    {
        _assetsManager = init.GetAssetsManager();
        _entityManager = init.GetEntityManager();
        _componentsManager = init.GetComponentManager();

        _gameState = init.GetReadOnlyResource<GameState>();
        _gameStart = init.GetEventsReader<GameStartEvent>();
        _gameEnded = init.GetEventsReader<GameEndedEvent>();
    }

    public void Update()
    {
        const uint ShieldOffsetY = 70;
        const uint ShieldCount = 4;

        if (_gameStart.HasEvents())
        {
            Debug.Assert(_container.IsInvalid);
            _container = _entityManager.Create();
            _componentsManager.AddComponent(_container, Transform2D.Default);
            var boardWidth = _gameState.Get().BoardSize.Width;
            var distance = boardWidth / (ShieldCount + 1);
            for (var i = 0; i < ShieldCount; i++)
            {
                SpawnShield(_container, distance * (i + 1), ShieldOffsetY);
            }
        }

        if (_gameEnded.HasEvents())
        {
            _entityManager.Destroy(ref _container);
        }
    }

    private void SpawnShield(in Entity parent, float x, float y)
    {
        var entity = _entityManager.CreateChild(parent);
        _componentsManager.AddComponent(entity, Transform2D.Default with
        {
            Position = new(x, y)
        });
        var pivot = Vector2.One * 0.5f;
        _componentsManager.AddComponent(entity, Sprite.Default with
        {
            Asset = _assetsManager.Load(AssetRegistry.Manifest.Textures.GameAtlas),
            Pivot = pivot,
            Layer = 1,
            Color = ColorPalette.Lighest,
            SourceRect = SpriteRectangles.Shields[0],
        });
        _componentsManager.AddComponent(entity, new BoxCollider2D
        {
            Size = new(11),
            Category = CollisionCategories.Shield,
            Pivot = pivot
        });
        _componentsManager.AddComponent(entity, new ShieldComponent());
    }

    public bool ShouldRun() => _gameStart.HasEvents() || _gameEnded.HasEvents();
}
