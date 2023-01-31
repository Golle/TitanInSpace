using Titan.ECS.Components;

namespace Space.Player;

internal struct PlayerComponent : IComponent
{
    //public float CurrentSpeed;
    public float Width;
    public float StartSpeed;
    public float MaxSpeed;

    public float ElapsedTimeMoving;
    public bool IsMoving;
}
