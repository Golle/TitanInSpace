using Titan;
using Titan.Assets;
using Titan.Core.Logging;
using Titan.Graphics;
using Titan.Input;
using Titan.Runners;
using Titan.Setup.Configs;

#if DEBUG
using var _ = Logger.Start<ConsoleLogger>(10_000);

var devAssetFolder = Path.GetFullPath($"{AppContext.BaseDirectory}../../../assets");
var devPakFolder = Path.Combine(devAssetFolder, "bin");
var devEngineFolder = Path.GetFullPath($"{AppContext.BaseDirectory}../../../../../../TitanEngine/");
var devConfig = new AssetsDevConfiguration(devAssetFolder, devPakFolder, devEngineFolder);

#else
using var _ = Logger.Start(new FileLogger(Path.GetFullPath("logs", AppContext.BaseDirectory)), 10_000);
#endif

App.Create(new AppCreationArgs())
#if DEBUG
    .AddConfig(devConfig)
#endif
    .AddModule<GraphicsModule>()
    .AddModule<InputModule>()
    .AddConfig(GraphicsConfig.Default with
    {
        Vsync = true,
        AllowTearing = false
    })
    .AddConfig(WindowConfig.Default with
    {
        Title = "Space Invaders on Titan",
        Windowed = true
    })
    .UseRunner<WindowedRunner>()
    .Build()
    .Run()
    ;



