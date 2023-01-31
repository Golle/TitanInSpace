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

namespace Space.Game;

internal struct GameStartupSystem : ISystem
{
    private EntityManager _entityManager;
    private MutableResource<GameState> _gameState;
    private ComponentManager _componentsManager;
    private AssetsManager _assetsManager;

    public void Init(in SystemInitializer init)
    {
        _entityManager = init.GetEntityManager();
        _gameState = init.GetMutableResource<GameState>();
        _componentsManager = init.GetComponentManager();
        _assetsManager = init.GetAssetsManager();
    }

    public void Update()
    {
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

        // Spawn enemies
        SpawnEnemies(3, 11, spriteSheet, _gameState.Get().BoardSize);


        // Debug background
#if DEBUG
        {
            var entity = _entityManager.Create();

            _componentsManager.AddComponent(entity, Transform2D.Default with { Position = new Vector2(0, 0), Scale = new Vector2(_gameState.Get().BoardSize.Width, _gameState.Get().BoardSize.Height) });
            _componentsManager.AddComponent(entity, new Sprite
            {
                Asset = spriteSheet,
                Color = Color.Red with { A = 0.05f },
                SourceRect = new Rectangle(4, 4, 1, 1),
                Pivot = new Vector2(0),
                Layer = short.MinValue
            });
        }
#endif

        // change the game state
        _gameState.Get().CurrentState = GameStateTypes.Playing;
    }

    private void SpawnEnemies(int rows, int columns, Handle<Asset> asset, in Size boardSize)
    {
        const int topOffset = 10;
        const int rowHeight = 20;

        const int columnWidth = 20;

        var totalWidth = columnWidth * columns+1;
        var halfWidth = totalWidth / 2f;
        var startOffset = boardSize.Width / 2f - halfWidth  + columnWidth/2f;

        for (var row = 0; row < rows; row++)
        {
            for (var column = 0; column < columns; column++)
            {
                var entity = _entityManager.Create();
                var enemy = new EnemyComponent
                {
                    Sprite1 = column % 2 == 0 ? SpriteRectangles.Monster1_0 : SpriteRectangles.Monster2_0,
                    Sprite2 = column % 2 == 0 ? SpriteRectangles.Monster1_1 : SpriteRectangles.Monster2_1,
                };
                _componentsManager.AddComponent(entity, enemy);
                _componentsManager.AddComponent(entity, Transform2D.Default with
                {
                    Position = new Vector2(columnWidth * column + startOffset, boardSize.Height - rowHeight * row - topOffset)
                });
                _componentsManager.AddComponent(entity, new Sprite
                {
                    Asset = asset,
                    Color = Random.Shared.Next(0, 2) switch
                    {
                        0 => ColorPalette.Lighter,
                        1 => ColorPalette.Darker,
                        _ => ColorPalette.Middle,
                    },
                    SourceRect = enemy.Sprite1,
                    Pivot = new Vector2(0.5f, 1)
                });
            }
        }
    }
    public bool ShouldRun() => _gameState.Get().CurrentState is GameStateTypes.Startup;
}
