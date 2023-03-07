using Space.Assets;
using Space.Events;
using Space.Invaders;
using Titan.Assets;
using Titan.BuiltIn.Components;
using Titan.BuiltIn.Resources;
using Titan.Core.Maths;
using Titan.ECS;
using Titan.ECS.Entities;
using Titan.ECS.Queries;
using Titan.Events;
using Titan.Systems;

namespace Space.Game;
internal struct LevelCompletedSystem : ISystem
{
    private EntityManager _entityManager;
    private ComponentManager _componentManager;
    private AssetsManager _assetsManager;
    private EntityQuery _query;
    private MutableResource<GameState> _gameState;
    private ReadOnlyResource<TimeStep> _timeStep;
    private EventsWriter<LevelCompletedEvent> _levelCompleted;
    private MutableStorage<Sprite> _sprite;

    private Entity _countDown;
    private float _gameReset;
    private EventsReader<InvaderDestroyedEvent> _invaderDestroyed;

    public void Init(in SystemInitializer init)
    {
        _entityManager = init.GetEntityManager();
        _assetsManager = init.GetAssetsManager();
        _componentManager = init.GetComponentManager();
        _query = init.CreateQuery(new EntityQueryArgs().With<InvaderComponent>());
        _gameState = init.GetMutableResource<GameState>();
        _levelCompleted = init.GetEventsWriter<LevelCompletedEvent>();
        _timeStep = init.GetReadOnlyResource<TimeStep>();
        _sprite = init.GetMutableStorage<Sprite>();
        _invaderDestroyed = init.GetEventsReader<InvaderDestroyedEvent>();
    }

    public void Update()
    {
        ref var gameState = ref _gameState.Get();
        if (gameState.CurrentState == GameStateTypes.Playing && !_query.HasEntities())
        {
            _gameState.Get().CurrentState = GameStateTypes.LevelCompleted;
            _levelCompleted.Send(default);
            _gameReset = 3f;
            CreateCountDown(gameState.BoardSize);
        }
        else if (gameState.CurrentState == GameStateTypes.LevelCompleted)
        {
            _gameReset -= _timeStep.Get().DeltaTimeSecondsF;
            var seconds = (int)(_gameReset + 0.5f);
            if (_gameReset < 0.0f)
            {
                gameState.CurrentState = GameStateTypes.Playing;
                _entityManager.Destroy(_countDown);
            }
            else
            {
                _sprite[_countDown].SourceRect = SpriteRectangles.Numbers[seconds];
            }
        }
    }

    private void CreateCountDown(in Size boardSize)
    {
        _countDown = _entityManager.Create();
        _componentManager.AddComponent(_countDown, Transform2D.Default with { Position = new(boardSize.Width / 2f, boardSize.Height / 2f) });
        _componentManager.AddComponent(_countDown, Sprite.Default with
        {
            Layer = 100,
            Asset = _assetsManager.Load(AssetRegistry.Manifest.Textures.GameAtlas),
            Color = Color.Red,
            SourceRect = SpriteRectangles.Numbers[3]
        });
    }

    public bool ShouldRun() 
        => _invaderDestroyed.HasEvents() || _gameState.Get().CurrentState == GameStateTypes.LevelCompleted;
}
