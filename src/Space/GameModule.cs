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

    private const int MaxLives = 3;
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddComponent<PlayerComponent>(5, ComponentPoolType.Packed)
            .AddComponent<ScoreComponent>(5, ComponentPoolType.Packed)
            .AddComponent<ShieldComponent>(10, ComponentPoolType.Packed)
            .AddComponent<InvaderComponent>(100, ComponentPoolType.Packed)
            .AddComponent<BulletComponent>(100, ComponentPoolType.Packed) // bullet component has smaller size than the entity id.
            .AddComponent<LivesComponent>(5, ComponentPoolType.Packed)
            .AddComponent<ExplosionComponent>(100, ComponentPoolType.Packed)

            .AddSystem<SplashSystem>()
            .AddSystem<GameOverSystem>()
            .AddSystem<GameStartupSystem>()
            .AddSystem<GameCameraSystem>()

            .AddSystem<ShieldSpawnSystem>()
            .AddSystem<ShieldDamageSystem>()


            .AddSystem<InvaderSpawnSystem>()
            .AddSystem<InvaderMovementSystem>()
            .AddSystem<InvaderDamageSystem>()
            .AddSystem<InvaderShootingSystem>()
            .AddSystem<InvaderAnimationSystem>()


            .AddSystem<PlayerShootingSystem>()
            .AddSystem<PlayerHitSystem>()
            .AddSystem<PlayerMovementSystem>()
            .AddSystem<PlayerDamageSystem>()
            .AddSystem<PlayerSpawnSystem>()

            .AddSystem<BulletSystem>()

            .AddSystem<CameraShakeSystem>()
            .AddSystem<ExplosionSystem>()

            .AddSystem<HudSystem>()
            .AddSystem<ScoreDispaySystem>()
            .AddSystem<LivesDisplaySystem>()

            .AddResource(new GameState(OriginalBoardSize, InvaderRows, InvaderColumns, InvaderMinShootingCooldown, InvaderMaxShootingCooldown, MaxLives) { CurrentState = GameStateTypes.Splash })

            .AddAssetsManifest<AssetRegistry.Manifest>()
            ;

        return true;
    }
    public static bool Init(IApp app) => true;
    public static bool Shutdown(IApp app) => true;
}
