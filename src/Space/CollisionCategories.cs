namespace Space;

internal static class CollisionCategories
{
    public const int Player = 0b1;
    public const int Invader = Player << 1;
    public const int Bullet = Invader << 1;
    public const int Shield = Bullet << 1;
}
