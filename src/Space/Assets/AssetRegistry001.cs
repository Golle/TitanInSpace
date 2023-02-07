// This is a generated file from Titan.Tools.Packager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
namespace Space.Assets;

internal static partial class AssetRegistry
{
    public readonly struct Manifest : Titan.Assets.IManifestDescriptor
    {
        public static uint Id => 1;
        public static string ManifestFile => "manifest.tmanifest";
        public static string TitanPackageFile => "manifest.titanpak";
        public static uint AssetCount => 1;
        public static Titan.Assets.AssetDescriptor[] AssetDescriptors { get; } =
        {
            new() { Id = 0, ManifestId = 1, Reference = { Offset = 0, Size = 65536}, Type = Titan.Assets.AssetDescriptorType.Texture, Image = new() { Format = 87, Height = 128, Width = 128, Stride = 512 } },
        };
        public static class Textures
        {
            public static ref readonly Titan.Assets.AssetDescriptor GameAtlas => ref AssetDescriptors[0];
        }
    }
}
