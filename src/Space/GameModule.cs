using Space.Assets;
using Space.Bullets;
using Space.Game;
using Space.Invaders;
using Space.Player;
using Space.Shields;
using Space.Splash;
using Titan.Core.Maths;
using Titan.ECS.Components;
using Titan.Modules;
using Titan.Setup;

namespace Space;
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
            //.AddSystem<TestSystem>()
            .AddSystem<SplashSystem>()
            .AddSystem<ShieldSpawnSystem>()
            .AddSystem<ShieldDamageSystem>()
            .AddSystem<PlayerMovementSystem>()
            .AddSystem<InvaderSpawnSystem>()
            .AddSystem<InvaderMovementSystem>()
            .AddSystem<InvaderDamageSystem>()
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
