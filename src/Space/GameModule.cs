using System.Numerics;
using Space.Assets;
using Space.Game;
using Space.Invaders;
using Space.Player;
using Space.Shields;
using Space.Splash;
using Titan.Assets;
using Titan.BuiltIn.Components;
using Titan.Core;
using Titan.Core.Maths;
using Titan.ECS;
using Titan.ECS.Components;
using Titan.ECS.Queries;
using Titan.Input;
using Titan.Modules;
using Titan.Setup;
using Titan.Systems;
using Titan.Windows;

namespace Space;

internal struct TestSystem : ISystem
{
    //NOTE(Jens): these should be grouped into something that is always available. Maybe as an argument to Update? Could also be added with source generation ?
    private AssetsManager _assetsManager;
    private ComponentManager _componentManager;
    private EntityManager _entityManager;

    private MutableStorage<Transform2D> _transform;
    private EntityQuery _query;

    private ObjectHandle<IWindow> _window;

    private bool _created;

    public void Init(in SystemInitializer init)
    {
        _assetsManager = init.GetAssetsManager();
        _componentManager = init.GetComponentManager();
        _entityManager = init.GetEntityManager();
        _transform = init.GetMutableStorage<Transform2D>();

        _query = init.CreateQuery(new EntityQueryArgs().With<Transform2D>().With<MouseComponent>());
        _window = init.GetManagedApi<IWindow>();
    }

    public void Update()
    {
        if (!_created)
        {
            var entity = _entityManager.Create();
            _componentManager.AddComponent(entity, Transform2D.Default with
            {
                Position = Vector2.One * 200,
                Scale = Vector2.One * 8
            });
            _componentManager.AddComponent<MouseComponent>(entity);
            _componentManager.AddComponent(entity, new Sprite
            {
                Asset = _assetsManager.Load(AssetRegistry.Manifest.Textures.GameAtlas),
                SourceRect = SpriteRectangles.Monster1_1,
                Pivot = new Vector2(0.5f),
                Color = Color.Red
            });
            _created = true;
        }
        else
        {
            ref var transform = ref _transform[_query[0]];
            var pos = _window.Value.GetRelativeCursorPosition();
            transform.Position = new Vector2(pos.X - _window.Value.Width / 2f, pos.Y);
            transform.Position.Y = _window.Value.Height / 2f - transform.Position.Y;
        }
    }


    public bool ShouldRun() => !_created || _query.HasEntities();
}

internal struct MouseComponent : IComponent
{
}
internal struct GameModule : IModule
{
    private static readonly Size OriginalBoardSize = new(224, 256);
    private const uint MonsterRows = 5;
    private const uint MonsterColumns = 11;
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddComponent<PlayerComponent>(5, ComponentPoolType.Packed)
            .AddComponent<ShieldComponent>(10, ComponentPoolType.Packed)
            .AddComponent<InvaderComponent>(1000, ComponentPoolType.Packed)
            .AddComponent<BulletComponent>(ComponentPoolType.Sparse) // bullet component has smaller size the the entity id.
            .AddComponent<MouseComponent>(ComponentPoolType.Sparse)
            //.AddSystem<TestSystem>()
            .AddSystem<SplashSystem>()
            .AddSystem<ShieldSpawnSystem>()
            .AddSystem<ShieldDamageSystem>()
            .AddSystem<PlayerMovementSystem>()
            .AddSystem<InvaderSpawnSystem>()
            .AddSystem<InvaderMovementSystem>()
            .AddSystem<GameCameraSystem>()
            .AddSystem<GameStartupSystem>()
            .AddSystem<InvaderAnimationSystem>()
            .AddSystem<PlayerShootingSystem>()
            .AddSystem<BulletSystem>()

            .AddResource(new GameState(OriginalBoardSize, MonsterRows, MonsterColumns) { CurrentState = GameStateTypes.Splash })

            .AddAssetsManifest<AssetRegistry.Manifest>()
            ;

        return true;
    }

    public static bool Init(IApp app)
    {

        return true;
    }

    public static bool Shutdown(IApp app)
    {
        return true;
    }
}
