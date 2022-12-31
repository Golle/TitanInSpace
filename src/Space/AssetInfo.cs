using Titan.Core.Maths;

namespace Space;

public static class ColorPalette
{
    public static readonly Color Darkest = Color.FromRGB(0x22223b);
    public static readonly Color Darker = Color.FromRGB(0x4a4e69);
    public static readonly Color Middle = Color.FromRGB(0x9a8c98);
    public static readonly Color Lighter = Color.FromRGB(0xc9ada7);
    public static readonly Color Lighest = Color.FromRGB(0xf2e9e4);
}

internal static class SpriteRectangles
{
    public static readonly Rectangle Player = new(0, 48, 11, 7);
    
    public static readonly Rectangle Monster1_0 = new(0, 0, 12, 8);
    public static readonly Rectangle Monster1_1 = new(16, 0, 12, 8);
    public static readonly Rectangle Monster2_0 = new(32, 0, 8, 8);
    public static readonly Rectangle Monster2_1 = new(48, 0, 8, 8);

    public static readonly Rectangle Bullet1_0 = new(0, 64, 2, 9);
}
