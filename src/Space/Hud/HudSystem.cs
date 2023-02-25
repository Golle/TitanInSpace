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
internal struct HudSystem : ISystem
{
    private AssetsManager _assetManager;
    private ReadOnlyResource<GameState> _gameState;
    private EventsReader<GameStartEvent> _gameStart;
    private EventsReader<GameEndedEvent> _gameEnded;


    private Entity _hudEntity;
    private EntityManager _entityManager;
    private ComponentManager _componentManager;

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
            SpawnScoreBoard();
        }

        if (_gameEnded.HasEvents() && _hudEntity.IsValid)
        {
            _entityManager.Destroy(ref _hudEntity);
        }
    }

    private void SpawnScoreBoard()
    {
        var boardSize = _gameState.Get().BoardSize;
        var heightOffset = boardSize.Height - 10;
        var widthOffset = boardSize.Width / 2;
        Debug.Assert(_hudEntity.IsInvalid);
        _hudEntity = _entityManager.Create();
        _componentManager.AddComponent(_hudEntity, Transform2D.Default with { Position = new Vector2(widthOffset, heightOffset) });
        _componentManager.AddComponent<ScoreComponent>(_hudEntity);
        var asset = _assetManager.Load(AssetRegistry.Manifest.Textures.GameAtlas);
        for (var i = -2; i < 3; ++i)
        {
            var entity = _entityManager.CreateChild(_hudEntity);
            _componentManager.AddComponent(entity, Transform2D.Default with { Position = new Vector2(i * 10, 0) });
            _componentManager.AddComponent(entity, Sprite.Default with
            {
                Asset = asset,
                Color = ColorPalette.Middle,
                SourceRect = SpriteRectangles.Numbers[Random.Shared.Next(0, 9)]
            });
        }
    }

    public bool ShouldRun()
        => _gameEnded.HasEvents() || _gameStart.HasEvents();
}
