using Titan.ECS.Components;

namespace Space.Player;

internal struct PlayerComponent : IComponent
{
    public static ComponentId ID => ComponentId<PlayerComponent>.Id;

    //public float CurrentSpeed;

    public float StartSpeed;
    public float MaxSpeed;

    public float ElapsedTimeMoving;
    public bool IsMoving;
}
