using Space.Assets;
using Space.Bullets;
using Space.Game;
using Space.Hud;
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
    private const uint InvaderRows = 5;
    private const uint InvaderColumns = 11;

    private const float InvaderMinShootingCooldown = 2f;
    private const float InvaderMaxShootingCooldown = 3f;
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddComponent<PlayerComponent>(5, ComponentPoolType.Packed)
            .AddComponent<ScoreComponent>(5, ComponentPoolType.Packed)
            .AddComponent<ShieldComponent>(10, ComponentPoolType.Packed)
            .AddComponent<InvaderComponent>(1000, ComponentPoolType.Packed)
            .AddComponent<BulletComponent>(ComponentPoolType.Sparse) // bullet component has smaller size than the entity id.
            .AddSystem<SplashSystem>()
            .AddSystem<ShieldSpawnSystem>()
            .AddSystem<ShieldDamageSystem>()
            .AddSystem<PlayerMovementSystem>()
            .AddSystem<CameraShakeSystem>()
            .AddSystem<InvaderSpawnSystem>()
            .AddSystem<InvaderMovementSystem>()
            .AddSystem<InvaderDamageSystem>()
            .AddSystem<InvaderShootingSystem>()
            .AddSystem<GameCameraSystem>()
            .AddSystem<GameStartupSystem>()
            .AddSystem<InvaderAnimationSystem>()
            .AddSystem<PlayerShootingSystem>()
            .AddSystem<BulletSystem>()
            

            .AddSystem<HudSystem>()
            .AddSystem<ScoreDispaySystem>()

            .AddResource(new GameState(OriginalBoardSize, InvaderRows, InvaderColumns, InvaderMinShootingCooldown, InvaderMaxShootingCooldown) { CurrentState = GameStateTypes.Splash })

            .AddAssetsManifest<AssetRegistry.Manifest>()
            ;

        return true;
    }
    public static bool Init(IApp app) => true;
    public static bool Shutdown(IApp app) => true;
}
