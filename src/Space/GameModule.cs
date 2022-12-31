using Space.Assets;
using Space.Enemies;
using Space.Game;
using Space.Player;
using Titan.ECS.Components;
using Titan.Modules;
using Titan.Setup;

namespace Space;

internal struct GameModule : IModule
{
    public static bool Build(IAppBuilder builder)
    {
        builder
            .AddComponent<PlayerComponent>(5, ComponentPoolType.Packed)
            .AddComponent<EnemyComponent>(100, ComponentPoolType.Packed)
            .AddComponent<BulletComponent>(ComponentPoolType.Sparse) // bullet component has smaller size the the entity id.
            .AddSystem<PlayerSystem>()
            .AddSystem<GameStartupSystem>()
            .AddSystem<EnemyAnimationSystem>()
            .AddSystem<PlayerShootingSystem>()
            .AddSystem<BulletSystem>()

            .AddResource(new GameState { CurrentState = GameStateTypes.Startup })

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
