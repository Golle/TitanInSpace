using System.Numerics;
using Space.Assets;
using Space.Events;
using Titan.Assets;
using Titan.BuiltIn.Components;
using Titan.BuiltIn.Resources;
using Titan.ECS;
using Titan.ECS.Queries;
using Titan.Events;
using Titan.Systems;

namespace Space.Game;

internal struct ExplosionSystem : ISystem
{
    private EntityManager _entityManager;
    private ComponentManager _componentManager;
    private AssetsManager _assetsManager;
    private MutableStorage<Sprite> _sprite;
    private MutableStorage<ExplosionComponent> _explosion;
    private MutableResource<TimeStep> _time;
    private EventsReader<PlayerHitEvent> _playerHit;
    private EventsReader<InvaderDestroyedEvent> _invaderDestroyed;
    private EntityQuery _query;

    private const float KeyFrameTime = 0.05f;
    private static readonly int FrameCount = SpriteRectangles.Explosion.Length;


    public void Init(in SystemInitializer init)
    {
        _entityManager = init.GetEntityManager();
        _componentManager = init.GetComponentManager();
        _assetsManager = init.GetAssetsManager();
        _sprite = init.GetMutableStorage<Sprite>();
        _explosion = init.GetMutableStorage<ExplosionComponent>();
        _time = init.GetMutableResource<TimeStep>();
        _query = init.CreateQuery(new EntityQueryArgs().With<Sprite>().With<ExplosionComponent>());
        _invaderDestroyed = init.GetEventsReader<InvaderDestroyedEvent>();
        _playerHit = init.GetEventsReader<PlayerHitEvent>();
    }

    public void Update()
    {
        var deltaTimeS = _time.Get().DeltaTimeSecondsF;
        foreach (ref readonly var entity in _query)
        {
            ref var explosion = ref _explosion[entity];
            explosion.Timer -= deltaTimeS;
            if (explosion.Timer > 0)
            {
                continue;
            }
            explosion.Index++;
            if (explosion.Index >= FrameCount)
            {
                _entityManager.Destroy(entity);
            }
            else
            {
                _sprite[entity].SourceRect = SpriteRectangles.Explosion[explosion.Index];
                explosion.Timer = KeyFrameTime;
            }
        }

        foreach (ref readonly var @event in _invaderDestroyed)
        {
            ref readonly var invaderSpriteRectangle = ref SpriteRectangles.Invaders[(int)@event.Type][0];
            var offset = new Vector2(invaderSpriteRectangle.Width, invaderSpriteRectangle.Height) / 2f;
            SpawnExplosion(@event.Position + offset);
        }

        foreach (ref readonly var @event in _playerHit)
        {
            var width = SpriteRectangles.Player.Width;
            var offset = new Vector2(width, 0) / 2f;
            SpawnExplosion(@event.Position + offset);
        }
    }


    private void SpawnExplosion(in Vector2 position)
    {
        var entity = _entityManager.Create();
        _componentManager.AddComponent(entity, Transform2D.Default with { Position = position });
        _componentManager.AddComponent(entity, new ExplosionComponent { Index = 0, Timer = KeyFrameTime });
        _componentManager.AddComponent(entity, Sprite.Default with
        {
            Asset = _assetsManager.Load(AssetRegistry.Manifest.Textures.GameAtlas),
            Color = ColorPalette.Lighest,
            SourceRect = SpriteRectangles.Explosion[0],
            Layer = 10
        });

    }
    public bool ShouldRun()
        => _query.HasEntities() || _invaderDestroyed.HasEvents();
}

