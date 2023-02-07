namespace Space;

internal class CollisionCategories
{
    private const ushort Base = 0b1;

    public const ushort Player = Base;
    public const ushort Invader = Player << 1;
    public const ushort Bullet = Invader << 1;
    public const ushort Shield = Bullet << 1;
}
