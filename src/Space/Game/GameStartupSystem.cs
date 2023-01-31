using System.Numerics;
using Space.Assets;
using Space.Enemies;
using Space.Player;
using Titan.Assets;
using Titan.BuiltIn.Components;
using Titan.Core;
using Titan.Core.Maths;
using Titan.ECS;
using Titan.Systems;
using Titan.Windows;

namespace Space.Game;

internal struct GameStartupSystem : ISystem
{
    private EntityManager _entityManager;
    private MutableResource<GameState> _gameState;
    private ComponentManager _componentsManager;
    private AssetsManager _assetsManager;
    private ObjectHandle<IWindow> _window;

    public void Init(in SystemInitializer init)
    {
        _entityManager = init.GetEntityManager();
        _gameState = init.GetMutableResource<GameState>();
        _componentsManager = init.GetComponentManager();
        _assetsManager = init.GetAssetsManager();
        _window = init.GetManagedApi<IWindow>();
    }

    public void Update()
    {
        var camera = _entityManager.Create();
        _componentsManager.AddComponent(camera, Transform2D.Default);
        _componentsManager.AddComponent(camera, new Camera2D
        {
            ClearColor = ColorPalette.Darkest,
            Size = _window.Value.Size / 4f
        });

        // Load the shared sprite sheet
        var spriteSheet = _assetsManager.Load(AssetRegistry.Manifest.Textures.GameAtlas);

        // Add the player
        {
            var entity = _entityManager.Create();

            _componentsManager.AddComponent(entity, Transform2D.Default with { Position = new Vector2(_gameState.Get().BoardSize.Width / 2f, 0) });
            _componentsManager.AddComponent(entity, new PlayerComponent { StartSpeed = 50f, MaxSpeed = 130f, Width = SpriteRectangles.Player.Width });
            _componentsManager.AddComponent(entity, new Sprite
            {
                Asset = spriteSheet,
                Color = ColorPalette.Lighter,
                SourceRect = SpriteRectangles.Player,
                Pivot = new Vector2(0.5f, 0)
            });
        }
        // Debug background
#if DEBUG
        {
            var entity = _entityManager.Create();

            _componentsManager.AddComponent(entity, Transform2D.Default with { Position = new Vector2(0, 0), Scale = new Vector2(_gameState.Get().BoardSize.Width, _gameState.Get().BoardSize.Height) });
            _componentsManager.AddComponent(entity, new Sprite
            {
                Asset = spriteSheet,
                Color = Color.Green, // with { A = 0.05f },
                SourceRect = new Rectangle(4, 4, 1, 1),
                Pivot = new Vector2(0),
                Layer = short.MinValue
            });
        }
#endif
        // Spawn enemies
        //SpawnEnemies(3, 10, spriteSheet, _window.Value.Size);

        _gameState.Get().CurrentState = GameStateTypes.Playing;
    }

    private void SpawnEnemies(int rows, int columns, Handle<Asset> asset, in Size windowSize)
    {
        const uint enemyMultiplier = 1;
        const uint spaceBetweenRows = 10;
        const uint spaceBetweenColumns = 14;
        const uint enemyWidth = 12 * enemyMultiplier;
        const uint enemyHeight = 8 * enemyMultiplier;
        var center = windowSize.Width / 4;
        var totalSize = columns * enemyWidth + (columns - 1) * spaceBetweenColumns;
        var startOffset = center - totalSize / 2 + enemyWidth / 2;
        Span<Color> color = stackalloc Color[3];
        color[0] = ColorPalette.Lighter;
        color[1] = ColorPalette.Darker;
        color[2] = ColorPalette.Middle;

        //NOTE(Jens): must implement parent/child support to move all units at the same time.
        for (var y = 0; y < rows; y++)
        {
            for (var x = 0; x < columns; x++)
            {
                var entity = _entityManager.Create();
                var enemy = new EnemyComponent
                {
                    Sprite1 = y % 2 == 0 ? SpriteRectangles.Monster1_0 : SpriteRectangles.Monster2_0,
                    Sprite2 = y % 2 == 0 ? SpriteRectangles.Monster1_1 : SpriteRectangles.Monster2_1,
                };
                _componentsManager.AddComponent(entity, enemy);
                _componentsManager.AddComponent(entity, Transform2D.Default with
                {
                    Position = new Vector2(startOffset + x * (enemyWidth + spaceBetweenColumns), 700 - (y * (enemyHeight + spaceBetweenRows))),
                    Scale = Vector2.One * enemyMultiplier,
                });
                _componentsManager.AddComponent(entity, new Sprite
                {
                    Asset = asset,
                    Color = color[Random.Shared.Next(0, 2)],
                    SourceRect = enemy.Sprite1,
                    Pivot = new Vector2(0.5f, 0)
                });
            }
        }

    }

    public bool ShouldRun() => _gameState.Get().CurrentState is GameStateTypes.Startup;
}
