using Titan.ECS.Components;

namespace Space.Player;

internal struct PlayerComponent : IComponent
{
    public static ComponentId ID => ComponentId<PlayerComponent>.Id;

    public float CurrentSpeed;
}
