using Space.Events;
using Space.Game;
using System.Numerics;
using Space.Assets;
using Titan.Assets;
using Titan.BuiltIn.Components;
using Titan.BuiltIn.Resources;
using Titan.ECS;
using Titan.Events;
using Titan.Systems;

namespace Space.Player;

internal struct PlayerSpawnSystem : ISystem
{
    private ReadOnlyResource<GameState> _gameState;
    private ReadOnlyResource<TimeStep> _time;
    private EventsReader<PlayerRespawnEvent> _playerRespawn;
    private EventsReader<GameStartEvent> _gameStart;
    private EntityManager _entityManager;
    private ComponentManager _componentManager;
    private AssetsManager _assetsManager;

    private float _respawnTimer;

    public void Init(in SystemInitializer init)
    {
        _gameState = init.GetReadOnlyResource<GameState>();
        _playerRespawn = init.GetEventsReader<PlayerRespawnEvent>();
        _gameStart = init.GetEventsReader<GameStartEvent>();
        _entityManager = init.GetEntityManager();
        _componentManager = init.GetComponentManager();
        _assetsManager = init.GetAssetsManager();
        _time = init.GetReadOnlyResource<TimeStep>();
    }

    public void Update()
    {
        if (_gameStart.HasEvents())
        {
            SpawnPlayer();
            return;
        }

        if (_playerRespawn.HasEvents())
        {
            _respawnTimer = 2f;
            return;
        }

        //NOTE(Jens): Do the calculation first because if it goes below 0 this frame the system will be disabled in the next.
        _respawnTimer -= _time.Get().DeltaTimeSecondsF;
        if (_respawnTimer < 0.0f)
        {
            SpawnPlayer();
        }
    }

    private void SpawnPlayer()
    {
        var player = _entityManager.Create();

        _componentManager.AddComponent(player, Transform2D.Default with { Position = new Vector2(_gameState.Get().BoardSize.Width / 2f, 40) });
        _componentManager.AddComponent(player, new PlayerComponent { StartSpeed = 50f, MaxSpeed = 130f, Width = SpriteRectangles.Player.Width });
        var pivot = new Vector2(0.5f, 0);
        _componentManager.AddComponent(player, new Sprite
        {
            Asset = _assetsManager.Load(AssetRegistry.Manifest.Textures.GameAtlas),
            Color = ColorPalette.Lighter,
            SourceRect = SpriteRectangles.Player,
            Pivot = pivot
        });
        _componentManager.AddComponent(player, BoxCollider2D.Default with
        {
            Category = CollisionCategories.Player,
            Pivot = pivot,
            Size = new(SpriteRectangles.Player.Width, SpriteRectangles.Player.Height)
        });
    }
    public bool ShouldRun() => _respawnTimer > 0.0f || _gameStart.HasEvents() || _playerRespawn.HasEvents();
}
