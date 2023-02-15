using Space.Bullets;
using System.Numerics;
using Space.Assets;
using Space.Game;
using Titan.Assets;
using Titan.BuiltIn.Components;
using Titan.BuiltIn.Resources;
using Titan.Core.Maths;
using Titan.ECS;
using Titan.ECS.Queries;
using Titan.Systems;

namespace Space.Invaders;

internal struct InvaderShootingSystem : ISystem
{
    private EntityManager _entityManager;
    private ComponentManager _componentManager;
    private AssetsManager _assetManager;
    private ReadOnlyStorage<Transform2D> _transform;
    private EntityQuery _query;
    private ReadOnlyResource<GameState> _gameState;
    private ReadOnlyResource<TimeStep> _timestep;
    private MutableStorage<InvaderComponent> _invader;

    public void Init(in SystemInitializer init)
    {
        _entityManager = init.GetEntityManager();
        _componentManager = init.GetComponentManager();
        _assetManager = init.GetAssetsManager();
        _transform = init.GetReadOnlyStorage<Transform2D>();
        _invader = init.GetMutableStorage<InvaderComponent>();
        _timestep = init.GetReadOnlyResource<TimeStep>();

        _gameState = init.GetReadOnlyResource<GameState>(false);
        _query = init.CreateQuery(new EntityQueryArgs().With<InvaderComponent>().With<Transform2D>());
    }

    public void Update()
    {
        // calculate the part of the max that is left and use that to determine the change of shooting
        ref readonly var gameState = ref _gameState.Get();
        var totalInvaders = gameState.InvaderColumns * gameState.InvaderRows;
        //NOTE(Jens): 0 chance of invaders shooting if the player hasn't killed one :) not indended.
        var shotProbability = (totalInvaders - _query.Count) / (float)totalInvaders; 

        var time = _timestep.Get().DeltaTimeSecondsF;
        foreach (ref readonly var entity in _query)
        {
            ref var invader = ref _invader[entity];
            invader.ShootingCooldown -= time;
            if (invader.ShootingCooldown <= 0.0f)
            {
                //NOTE(Jens): reset the Cooldown when it's below 0 and run the probability check
                invader.ShootingCooldown = Random.Shared.NextSingle() * gameState.InvaderMaxShootingCooldown + gameState.InvaderMinShootingCooldown;
                if (Random.Shared.NextSingle() <= shotProbability)
                {
                    ref readonly var transform = ref _transform[entity];
                    SpawnBullet(transform.Position, invader.InvaderWidth);
                }
            }
        }
    }

    public bool ShouldRun() => _query.HasEntities();

    private void SpawnBullet(Vector2 invaderPosition, float invaderWidth)
    {
        var bullet = _entityManager.Create();
        _componentManager.AddComponent(bullet, Transform2D.Default with
        {
            Position = invaderPosition + Vector2.UnitX * (invaderWidth / 2f),
            Scale = Vector2.One * 0.5f
        });
        _componentManager.AddComponent(bullet, new Sprite
        {
            Asset = _assetManager.Load(AssetRegistry.Manifest.Textures.GameAtlas),
            Color = ColorPalette.Lighest,
            Pivot = new Vector2(0.5f, 0.5f),
            SourceRect = SpriteRectangles.Bullet1_0
        });

        _componentManager.AddComponent(bullet, new BoxCollider2D
        {
            Size = new SizeF(4, 10),
            Category = CollisionCategories.Bullet,
            CollidesWith = CollisionCategories.Shield | CollisionCategories.Player
        });
        _componentManager.AddComponent(bullet, new BulletComponent { Down = true });
    }
}
