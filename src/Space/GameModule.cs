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
            .AddSystem<PlayerSystem>()
            .AddSystem<GameStartupSystem>()
            .AddResource(new GameState { CurrentState = GameStateTypes.Startup })
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
