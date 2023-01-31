using Titan.Core.Maths;
using Titan.ECS.Components;

namespace Space.Enemies;

internal struct EnemyComponent : IComponent
{
    public Rectangle Sprite1;
    public Rectangle Sprite2;
    public int SpriteIndex;
    public float TimeElapsed;
}
