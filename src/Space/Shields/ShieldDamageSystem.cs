using Titan.BuiltIn.Components;
using Titan.BuiltIn.Events;
using Titan.ECS;
using Titan.Events;
using Titan.Systems;

namespace Space.Shields;

internal struct ShieldDamageSystem : ISystem
{
    private static readonly int MaxDamage = SpriteRectangles.Shields.Length - 1;
    private MutableStorage<Sprite> _sprites;
    private MutableStorage<ShieldComponent> _shields;
    private EntityManager _entityManager;
    private EventsReader<Collision2DEnter> _collisionEnter;

    public void Init(in SystemInitializer init)
    {
        _entityManager = init.GetEntityManager();
        _sprites = init.GetMutableStorage<Sprite>();
        _shields = init.GetMutableStorage<ShieldComponent>();
        _collisionEnter = init.GetEventsReader<Collision2DEnter>();
    }

    public void Update()
    {
        foreach (ref readonly var @event in _collisionEnter)
        {
            if (@event.Target.Category != CollisionCategories.Shield)
            {
                continue;
            }

            if (@event.Source.Category == CollisionCategories.Bullet)
            {
                var shieldEntity = @event.Target.Entity;
                ref var shield = ref _shields[shieldEntity];
                if (shield.Damage++ >= MaxDamage)
                {
                    _entityManager.Destroy(shieldEntity);
                }
                else
                {
                    ref var sprite = ref _sprites[shieldEntity];
                    sprite.SourceRect = SpriteRectangles.Shields[shield.Damage];
                }
                _entityManager.Destroy(@event.Source.Entity);
            }
            else if (@event.Source.Category == CollisionCategories.Invader)
            {
                _entityManager.Destroy(@event.Target.Entity);
            }
        }
    }

    public bool ShouldRun() => _collisionEnter.HasEvents();
}
