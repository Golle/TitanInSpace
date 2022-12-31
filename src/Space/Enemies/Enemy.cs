using Titan.Core.Maths;
using Titan.ECS.Components;

namespace Space;

internal struct EnemyComponent : IComponent
{
    public static ComponentId ID => ComponentId<EnemyComponent>.Id;

    public Rectangle Sprite1;
    public Rectangle Sprite2;
    public int SpriteIndex;
    public float TimeElapsed;
}
