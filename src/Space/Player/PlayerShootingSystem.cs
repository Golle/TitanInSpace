using System.Numerics;
using Space.Assets;
using Space.Game;
using Titan.Assets;
using Titan.BuiltIn.Components;
using Titan.BuiltIn.Resources;
using Titan.Core.Maths;
using Titan.ECS;
using Titan.ECS.Queries;
using Titan.Input;
using Titan.Systems;

namespace Space.Player;

internal struct PlayerShootingSystem : ISystem
{
    private ReadOnlyStorage<Transform2D> _transform;
    private EntityQuery _query;

    private InputManager _input;
    private AssetsManager _assetManager;
    private ComponentManager _componentManager;
    private EntityManager _entityManager;
    private ReadOnlyResource<TimeStep> _timestep;
    private ReadOnlyResource<GameState> _gameState;

    private const float TimeBetweenShots = 0.2f;
    private float _timeSinceLastShot;

    public void Init(in SystemInitializer init)
    {
        _transform = init.GetReadOnlyStorage<Transform2D>();

        _query = init.CreateQuery(new EntityQueryArgs().With<PlayerComponent>().With<Transform2D>());
        _input = init.GetInputManager();
        _assetManager = init.GetAssetsManager();
        _entityManager = init.GetEntityManager();
        _componentManager = init.GetComponentManager();
        _timestep = init.GetReadOnlyResource<TimeStep>();
        _gameState = init.GetReadOnlyResource<GameState>();
    }

    public void Update()
    {
        ref readonly var timestep = ref _timestep.Get();

        _timeSinceLastShot -= timestep.DeltaTimeSecondsF;
        if (_input.IsKeyPressed(KeyCode.Space) && _timeSinceLastShot <= 0)
        {
            var playerEntity = _query[0];
            var playerPositionX = _transform.Get(playerEntity).Position.X;
            SpawnBullet(playerPositionX);
            _timeSinceLastShot = TimeBetweenShots;
        }
    }
    private void SpawnBullet(float x)
    {
        var bullet = _entityManager.Create();
        _componentManager.AddComponent(bullet, Transform2D.Default with
        {
            Position = new Vector2(x, 50),
            Scale = Vector2.One * 4
        });
        _componentManager.AddComponent(bullet, new Sprite
        {
            Asset = _assetManager.Load(AssetRegistry.Manifest.Textures.GameArt),
            Color = Color.White,
            Pivot = new Vector2(0.5f, 0),
            SourceRect = SpriteRectangles.Bullet1_0
        });
        _componentManager.AddComponent<BulletComponent>(bullet);

    }

    public bool ShouldRun() => _gameState.Get().CurrentState == GameStateTypes.Playing;
}
