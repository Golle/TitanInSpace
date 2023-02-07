using Titan.Core.Maths;
using Titan.ECS.Components;

namespace Space.Invaders;

internal struct InvaderComponent : IComponent
{
    public Rectangle Sprite1, Sprite2;
    public int SpriteIndex;
    public float TimeElapsed;
}
