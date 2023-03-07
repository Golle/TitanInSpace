using Titan.Core.Maths;

namespace Space;

internal static class GameConstants
{

    public const uint InvaderWidth = 12;
    public const uint InvaderHalfWidth = InvaderWidth / 2;
    public const uint InvaderBlockWidth = 18;
    public const uint InvaderBlockHeight = 18;
}

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
    public static readonly Rectangle PressE = new(0, 80, 42, 8);
    public static readonly Rectangle GameOver = new(48, 80, 56, 8);

    public static readonly Rectangle Player = new(0, 48, 11, 7);

    public static readonly Rectangle[][] Invaders =
    {
        new Rectangle[]
        {
            new(0, 0, 12, 8),
            new(16, 0, 12, 8)
        },
        new Rectangle[]
        {
            new(32, 0, 8, 8),
            new(48, 0, 8, 8)
        },
        new Rectangle[]
        {
            new(64, 0, 14, 12),
            new(80, 0, 14, 12)
        }
    };

    public static readonly Rectangle Bullet1_0 = new(0, 64, 2, 9);

    public static readonly Rectangle[] Shields = {
        new(0, 96, 11, 11),
        new(16, 96, 11, 11),
        new(32, 96, 11, 11),
        new(48, 96, 11, 11),
        new(64, 96, 11, 11),
        new(80, 96, 11, 11),
        new(96, 96, 11, 11),
        new(112, 96, 11, 11),
    };

    public static readonly Rectangle[] Numbers =
    {
        new(0, 16, 8, 8),
        new(9, 16, 8, 8),
        new(18, 16, 8, 8),
        new(27, 16, 8, 8),
        new(36, 16, 8, 8),
        new(45, 16, 8, 8),
        new(54, 16, 8, 8),
        new(63, 16, 8, 8),
        new(72, 16, 8, 8),
        new(81, 16, 8, 8)
    };

    public static readonly Rectangle[] Explosion =
    {
        new(0, 32, 9, 9),
        //new(9, 32, 9, 9),
        new(18, 32, 9, 9),
        //new(27, 32, 9, 9),
        new(36, 32, 9, 9)
    };
}
