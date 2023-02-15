using Space;
using Titan;
using Titan.Assets;
using Titan.Core.Logging;
using Titan.Graphics;
using Titan.Input;
using Titan.Runners;
using Titan.Setup.Configs;
using Titan.Sound;

#if DEBUG || CONSOLE_LOGGING
using var _ = Logger.Start<ConsoleLogger>(10_000);

//NOTE(Jens): Change this to false to run from the titanpak files.
const bool UseRawAssets = true;

var devAssetFolder = Path.GetFullPath($"{AppContext.BaseDirectory}../../../../../assets");
var devPakFolder = Path.Combine(devAssetFolder, "bin");
var devEngineFolder = Path.GetFullPath($"{AppContext.BaseDirectory}../../../../../../TitanEngine/");
var devConfig = new AssetsDevConfiguration(devAssetFolder, devPakFolder, devEngineFolder, UseRawAssets);

#else
using var _ = Logger.Start(new FileLogger(Path.GetFullPath("logs", AppContext.BaseDirectory)), 10_000);
#endif

App.Create(new AppCreationArgs())
#if DEBUG
    .AddConfig(devConfig)

#endif
    .AddModule<GraphicsModule>()
    .AddModule<InputModule>()
    .AddModule<SoundModule>()
    .AddModule<GameModule>()
    .AddConfig(GraphicsConfig.Default with
    {
        //Debug = false,
        //Vsync = false,
        //AllowTearing = true,
        Debug = true,
        Vsync = true,
        AllowTearing = false,
        ClearColor = ColorPalette.Darkest
    })
    .AddConfig(WindowConfig.Default with
    {
        Title = "Space Invaders on Titan",
        Windowed = true,
        //AlwaysOnTop = true,
        Width = 1024,
        Height = 768,
        Resizable = true
    })

    .UseRunner<WindowedRunner>()
    .Build()
    .Run()
    ;
