using Titan.ECS.Components;

namespace Space.Invaders;

internal struct InvaderComponent : IComponent
{
    public InvaderType Type;
    public int SpriteIndex;
    public float InvaderWidth;
    public float TimeElapsed;
    public float ShootingCooldown;
}
