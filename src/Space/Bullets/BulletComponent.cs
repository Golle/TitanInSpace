using System.Runtime.InteropServices;
using Titan.ECS.Components;

namespace Space.Bullets;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct BulletComponent : IComponent
{
    public int Direction;
}
