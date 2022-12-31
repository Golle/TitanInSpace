using System.Runtime.InteropServices;
using Titan.ECS.Components;

namespace Space.Player;

[StructLayout(LayoutKind.Sequential, Size = 1)]
internal struct BulletComponent : IComponent
{
    public static ComponentId ID => ComponentId<BulletComponent>.Id;
}
