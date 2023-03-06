using System.Numerics;
using Space.Assets;
using Space.Bullets;
using Space.Game;
using Titan.Assets;
using Titan.Audio;
using Titan.BuiltIn.Components;
using Titan.BuiltIn.Resources;
using Titan.Core;
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
    private AudioManager _audioManager;
    private ReadOnlyResource<TimeStep> _timestep;
    private ReadOnlyResource<GameState> _gameState;

    private const float TimeBetweenShots = 0.2f;
    private float _timeSinceLastShot;

    private Handle<Asset> _laser;
    

    public void Init(in SystemInitializer init)
    {
        _transform = init.GetReadOnlyStorage<Transform2D>();

        _query = init.CreateQuery(new EntityQueryArgs().With<PlayerComponent>().With<Transform2D>());
        _input = init.GetInputManager();
        _assetManager = init.GetAssetsManager();
        _entityManager = init.GetEntityManager();
        _componentManager = init.GetComponentManager();
        _audioManager = init.GetAudioManager();
        _timestep = init.GetReadOnlyResource<TimeStep>();
        _gameState = init.GetReadOnlyResource<GameState>();
    }

    public void Update()
    {
        if (_laser.IsInvalid)
        {
            _laser = _assetManager.Load(AssetRegistry.Manifest.Textures.Laser);
        }
        ref readonly var timestep = ref _timestep.Get();

        _timeSinceLastShot -= timestep.DeltaTimeSecondsF;
        if (_input.IsKeyPressed(KeyCode.Space) && _timeSinceLastShot <= 0)
        {
            var playerEntity = _query[0];
            var position = _transform.Get(playerEntity).Position;
            SpawnBullet(position);
            _timeSinceLastShot = TimeBetweenShots;
        }
    }
    private void SpawnBullet(Vector2 playerPosition)
    {
        var bullet = _entityManager.Create();
        _componentManager.AddComponent(bullet, Transform2D.Default with
        {
            Position = playerPosition + Vector2.UnitY * 10,
            Scale = Vector2.One * 0.5f
        });
        _componentManager.AddComponent(bullet, new Sprite
        {
            Asset = _assetManager.Load(AssetRegistry.Manifest.Textures.GameAtlas),
            Color = Color.White,
            Pivot = new Vector2(0.5f, 0.5f),
            SourceRect = SpriteRectangles.Bullet1_0
        });
        _componentManager.AddComponent(bullet, new BoxCollider2D
        {
            Size = new SizeF(4, 10),
            Category = CollisionCategories.Bullet,
            CollidesWith = CollisionCategories.Shield | CollisionCategories.Invader
        });
        _componentManager.AddComponent<BulletComponent>(bullet, default);
        _audioManager.PlayOnce(_laser, PlaybackSettings.Default with{Frequency = 2f, Volume = 1f});
    }

    public bool ShouldRun() => _gameState.Get().CurrentState is GameStateTypes.Playing && _query.HasEntities();
}
