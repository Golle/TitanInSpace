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

namespace Space.Hud;

internal struct ScoreComponent : IComponent { }
internal struct LivesComponent : IComponent { }
internal struct HudSystem : ISystem
{
    private AssetsManager _assetManager;
    private ReadOnlyResource<GameState> _gameState;
    private EventsReader<GameStartEvent> _gameStart;
    private EventsReader<GameEndedEvent> _gameEnded;


    private Entity _hudEntity;
    private EntityManager _entityManager;
    private ComponentManager _componentManager;

    private const short HudLayer = 1_000;
    public void Init(in SystemInitializer init)
    {
        _assetManager = init.GetAssetsManager();
        _entityManager = init.GetEntityManager();
        _componentManager = init.GetComponentManager();
        _gameState = init.GetReadOnlyResource<GameState>();

        _gameStart = init.GetEventsReader<GameStartEvent>();
        _gameEnded = init.GetEventsReader<GameEndedEvent>();
    }

    public void Update()
    {
        if (_gameStart.HasEvents())
        {
            Debug.Assert(_hudEntity.IsInvalid);
            _hudEntity = _entityManager.Create();
            _componentManager.AddComponent(_hudEntity, Transform2D.Default);

            SpawnScoreBoard(_hudEntity);
            SpawnPlayerLives(_hudEntity);
        }

        if (_gameEnded.HasEvents() && _hudEntity.IsValid)
        {
            _entityManager.Destroy(ref _hudEntity);
        }
    }

    private void SpawnScoreBoard(in Entity parent)
    {
        var boardSize = _gameState.Get().BoardSize;
        var heightOffset = boardSize.Height - 10;
        var widthOffset = boardSize.Width / 2;

        var scoreBoard = _entityManager.CreateChild(parent);
        _componentManager.AddComponent(scoreBoard, Transform2D.Default with { Position = new Vector2(widthOffset, heightOffset) });
        _componentManager.AddComponent<ScoreComponent>(scoreBoard);
        var asset = _assetManager.Load(AssetRegistry.Manifest.Textures.GameAtlas);
        for (var i = -2; i < 3; ++i)
        {
            var entity = _entityManager.CreateChild(scoreBoard);
            _componentManager.AddComponent(entity, Transform2D.Default with { Position = new Vector2(i * 10, 0) });
            _componentManager.AddComponent(entity, Sprite.Default with
            {
                Asset = asset,
                Color = ColorPalette.Middle,
                SourceRect = SpriteRectangles.Numbers[Random.Shared.Next(0, 9)],
                Layer = HudLayer
            });
        }
    }

    private void SpawnPlayerLives(in Entity parent)
    {
        const uint heightOffset = 10;
        const uint widthOffset = 10;
        var lives = _gameState.Get().MaxLives;
        var asset = _assetManager.Load(AssetRegistry.Manifest.Textures.GameAtlas);

        
        var livesEntity = _entityManager.CreateChild(parent);
        _componentManager.AddComponent(livesEntity, Transform2D.Default with { Position = new(widthOffset, heightOffset) });
        //NOTE(Jens): This is such a bad solution :| but we need better support in the engine for this so we don't need empty components. Maybe Tags?
        _componentManager.AddComponent<LivesComponent>(livesEntity);
        for (var i = 0; i < lives; ++i)
        {
            var entity = _entityManager.CreateChild(livesEntity);
            _componentManager.AddComponent(entity, Transform2D.Default with { Position = new(widthOffset + i * 20, heightOffset) });
            _componentManager.AddComponent(entity, Sprite.Default with
            {
                Asset = asset,
                Color = ColorPalette.Lighter,
                SourceRect = SpriteRectangles.Player,
                Layer = HudLayer
            });
        }

    }

    public bool ShouldRun()
        => _gameEnded.HasEvents() || _gameStart.HasEvents();
}
