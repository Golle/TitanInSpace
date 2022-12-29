using System.Numerics;
using Space.Assets;
using Space.Player;
using Titan.Assets;
using Titan.BuiltIn.Components;
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
        var entity = _entityManager.Create();

        _componentsManager.AddComponent(entity, new Transform2D(Vector2.One * 200, Vector2.One*30));
        _componentsManager.AddComponent(entity, new PlayerComponent { StartSpeed = 0.2f, MaxSpeed = 3f });
        _componentsManager.AddComponent(entity, new Sprite
        {
            Asset = _assetsManager.Load(AssetRegistry.Manifest.Textures.GameAtlas),
            //TextureIndex = 1,
            
            Color = Color.White,
            SourceRect = new Rectangle(0, 0, 16, 16)
        });

        var entity2 = _entityManager.Create();
        _componentsManager.AddComponent(entity2, new Transform2D(Vector2.One * 100, Vector2.One * 5));
        _componentsManager.AddComponent(entity2, new Sprite
        {
             Asset = _assetsManager.Load(AssetRegistry.Manifest.Textures.GameAtlas),
             Color = Color.Red,
             SourceRect = new Rectangle(0,0, 128, 16)
        });

        _gameState.Get().CurrentState = GameStateTypes.Playing;
    }

    public bool ShouldRun() => _gameState.Get().CurrentState == GameStateTypes.Startup;
}
