using Titan.ECS.Components;

namespace Space.Game;

internal struct ExplosionComponent : IComponent
{
    public float Timer;
    public int Index;
}
