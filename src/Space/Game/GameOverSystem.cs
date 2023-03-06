using System.Numerics;
using Space.Assets;
using Space.Events;
using Titan.Assets;
using Titan.BuiltIn.Components;
using Titan.Core.Maths;
using Titan.ECS;
using Titan.ECS.Entities;
using Titan.Events;
using Titan.Input;
using Titan.Systems;

namespace Space.Game;
internal struct GameOverSystem : ISystem
{
    private EventsReader<GameEndedEvent> _gameEnded;
    private MutableResource<GameState> _gameState;
    private AssetsManager _assetManager;
    private ComponentManager _componentManager;
    private EntityManager _entityManager;
    private Entity _container;
    private InputManager _inputManager;

    public void Init(in SystemInitializer init)
    {
        _gameEnded = init.GetEventsReader<GameEndedEvent>();
        _gameState = init.GetMutableResource<GameState>();
        _entityManager = init.GetEntityManager();
        _componentManager = init.GetComponentManager();
        _assetManager = init.GetAssetsManager();
        _inputManager = init.GetInputManager();
    }

    public void Update()
    {
        ref var gameState = ref _gameState.Get();

        if (_container.IsInvalid)
        {
            _container = _entityManager.Create();
            _componentManager.AddComponent(_container, Transform2D.Default with { Position = new(gameState.BoardSize.Width / 2f, gameState.BoardSize.Height / 2f) });
            {
                var entity = _entityManager.CreateChild(_container);
                _componentManager.AddComponent(entity, Transform2D.Default);
                _componentManager.AddComponent(entity, Sprite.Default with
                {
                    Asset = _assetManager.Load(AssetRegistry.Manifest.Textures.GameAtlas),
                    Layer = 1000,
                    Color = Color.Red,
                    Pivot = Vector2.One * 0.5f,
                    SourceRect = SpriteRectangles.GameOver
                });
            }
            {
                var entity = _entityManager.CreateChild(_container);
                _componentManager.AddComponent(entity, Transform2D.Default with { Position = Vector2.UnitY * -20f, Scale = Vector2.One * 0.7f });
                _componentManager.AddComponent(entity, Sprite.Default with
                {
                    Asset = _assetManager.Load(AssetRegistry.Manifest.Textures.GameAtlas),
                    Layer = 1000,
                    Color = Color.Red,
                    Pivot = Vector2.One * 0.5f,
                    SourceRect = SpriteRectangles.PressE
                });
            }

            {
                var scoreBoard = _entityManager.CreateChild(_container);
                _componentManager.AddComponent(scoreBoard, Transform2D.Default with { Position = Vector2.UnitY * 40f, Scale = Vector2.One * 1f });
                var score = gameState.Score;
                for (var i = -2; i < 3; ++i)
                {
                    var digit = score % 10;
                    score /= 10;
                    var entity = _entityManager.CreateChild(scoreBoard);
                    _componentManager.AddComponent(entity, Transform2D.Default with { Position = new Vector2(-i * 10, 0) });
                    _componentManager.AddComponent(entity, Sprite.Default with
                    {
                        Asset = _assetManager.Load(AssetRegistry.Manifest.Textures.GameAtlas),
                        Layer = 1000,
                        Color = ColorPalette.Middle,
                        SourceRect = SpriteRectangles.Numbers[digit]
                    });
                }
            }
        }

        if (_inputManager.IsKeyPressed(KeyCode.E))
        {
            gameState.CurrentState = GameStateTypes.Splash;
            _entityManager.Destroy(ref _container);
        }
    }

    public bool ShouldRun()
        => _gameState.Get().CurrentState == GameStateTypes.EndGame;
}
