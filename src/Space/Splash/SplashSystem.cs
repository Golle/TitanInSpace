using System.Numerics;
using Space.Assets;
using Space.Game;
using Titan.Assets;
using Titan.BuiltIn.Components;
using Titan.Core.Maths;
using Titan.ECS;
using Titan.ECS.Entities;
using Titan.Input;
using Titan.Systems;

namespace Space.Splash;

internal struct SplashSystem : ISystem
{
    private EntityManager _entityManager;
    private ComponentManager _componentManager;
    private MutableResource<GameState> _gameState;
    private MutableStorage<Transform2D> _transform;
    private InputManager _input;

    private Entity _splashEntity;
    private AssetsManager _assetsManager;

    public void Init(in SystemInitializer init)
    {
        _entityManager = init.GetEntityManager();
        _componentManager = init.GetComponentManager();
        _assetsManager = init.GetAssetsManager();
        _input = init.GetInputManager();

        _gameState = init.GetMutableResource<GameState>(true);
        _transform = init.GetMutableStorage<Transform2D>();
    }

    public void Update()
    {
        if (_splashEntity.IsInvalid)
        {
            _splashEntity = _entityManager.Create();
            _componentManager.AddComponent(_splashEntity, Transform2D.Default);
            _componentManager.AddComponent(_splashEntity, new Sprite
            {
                Layer = 1,
                Asset = _assetsManager.Load(AssetRegistry.Manifest.Textures.GameAtlas),
                Color = Color.Red,
                Pivot = Vector2.One * 0.5f,
                SourceRect = SpriteRectangles.PressE
            });
        }

        ref var transform = ref _transform[_splashEntity];
        transform.Position = new Vector2(_gameState.Get().BoardSize.Width / 2f, 100);

        if (_input.IsKeyPressed(KeyCode.E))
        {
            _gameState.Get().CurrentState = GameStateTypes.Startup;
            _entityManager.Destroy(_splashEntity);
            _splashEntity = Entity.Invalid;
        }
    }

    public bool ShouldRun() => _gameState.Get().CurrentState is GameStateTypes.Splash;
}
