using System.Numerics;
using Space.Assets;
using Titan.Assets;
using Titan.BuiltIn.Components;
using Titan.Core.Maths;
using Titan.ECS;
using Titan.ECS.Entities;
using Titan.Systems;

namespace Space.Game;

internal struct GameStartupSystem : ISystem
{
    private EntityManager _entityManager;
    private MutableResource<GameState> _gameState;
    private ComponentManager _componentsManager;
    private AssetsManager _assetsManager;

#if DEBUG
    private Entity _debugBackground;
#endif

    public void Init(in SystemInitializer init)
    {
        _entityManager = init.GetEntityManager();
        _gameState = init.GetMutableResource<GameState>();
        _componentsManager = init.GetComponentManager();
        _assetsManager = init.GetAssetsManager();
    }



    public void Update()
    {
        ref var gameState = ref _gameState.Get();

        // Reset the lives counter
        gameState.Lives = (int)gameState.MaxLives;
        // Reset score
        gameState.Score = 0;
        // change the game state
        gameState.CurrentState = GameStateTypes.Playing;

        // Debug background
#if DEBUG
        {
            if (_debugBackground.IsValid)
            {
                return;
            }
            // Load the shared sprite sheet
            var spriteSheet = _assetsManager.Load(AssetRegistry.Manifest.Textures.GameAtlas);
            _debugBackground = _entityManager.Create();

            _componentsManager.AddComponent(_debugBackground, Transform2D.Default with { Position = new Vector2(0, 0), Scale = new Vector2(gameState.BoardSize.Width, gameState.BoardSize.Height) });
            _componentsManager.AddComponent(_debugBackground, new Sprite
            {
                Asset = spriteSheet,
                Color = Color.Red with { A = 0.05f },
                SourceRect = new Rectangle(4, 4, 1, 1),
                Pivot = new Vector2(0),
                Layer = short.MinValue
            });
        }
#endif
    }
    public bool ShouldRun() => _gameState.Get().CurrentState is GameStateTypes.Startup;
}
