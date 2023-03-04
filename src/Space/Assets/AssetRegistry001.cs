// This is a generated file from Titan.Tools.Packager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
namespace Space.Assets;

internal static partial class AssetRegistry
{
    public readonly struct Manifest : Titan.Assets.IManifestDescriptor
    {
        public static uint Id => 1;
        public static string ManifestFile => "manifest.tmanifest";
        public static string TitanPackageFile => "manifest.titanpak";
        public static uint AssetCount => 4;
        public static Titan.Assets.AssetDescriptor[] AssetDescriptors { get; } =
        {
            new() { Id = 0, ManifestId = 1, Reference = { Offset = 0, Size = 65536}, Type = Titan.Assets.AssetDescriptorType.Texture, Image = new() { Format = 28, Height = 128, Width = 128, Stride = 512 } },
            new() { Id = 1, ManifestId = 1, Reference = { Offset = 65536, Size = 5644800}, Type = Titan.Assets.AssetDescriptorType.Audio, Audio = new() { Channels = 2, BitsPerSample = 16, SamplesPerSecond = 44100 } },
            new() { Id = 2, ManifestId = 1, Reference = { Offset = 5710336, Size = 2822400}, Type = Titan.Assets.AssetDescriptorType.Audio, Audio = new() { Channels = 2, BitsPerSample = 16, SamplesPerSecond = 44100 } },
            new() { Id = 3, ManifestId = 1, Reference = { Offset = 8532736, Size = 27654}, Type = Titan.Assets.AssetDescriptorType.Audio, Audio = new() { Channels = 1, BitsPerSample = 16, SamplesPerSecond = 44100 } },
        };
#if DEBUG
        public static object[] RawAssets { get; } =
        {
            new Titan.Tools.Core.Manifests.TextureItem{ Name = "GameAtlas", Path = @"textures\game_art.aseprite", Type = Titan.Tools.Core.Manifests.TextureType.Aseprite },
            new Titan.Tools.Core.Manifests.AudioItem{ Name = "SplashScreenMusic", Path = @"audio\loading.wav" },
            new Titan.Tools.Core.Manifests.AudioItem{ Name = "CombatMusic", Path = @"audio\battle.wav" },
            new Titan.Tools.Core.Manifests.AudioItem{ Name = "Laser", Path = @"audio\laser gun.wav" },
        };
#else
        public static object[] RawAssets { get; } = System.Array.Empty<object>();
#endif
        public static class Textures
        {
            public static ref readonly Titan.Assets.AssetDescriptor GameAtlas => ref AssetDescriptors[0];
            public static ref readonly Titan.Assets.AssetDescriptor SplashScreenMusic => ref AssetDescriptors[1];
            public static ref readonly Titan.Assets.AssetDescriptor CombatMusic => ref AssetDescriptors[2];
            public static ref readonly Titan.Assets.AssetDescriptor Laser => ref AssetDescriptors[3];
        }
    }
}
