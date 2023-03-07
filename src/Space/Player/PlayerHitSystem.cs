using Space.Events;
using Titan.BuiltIn.Components;
using Titan.BuiltIn.Events;
using Titan.ECS;
using Titan.Events;
using Titan.Systems;

namespace Space.Player;

internal struct PlayerHitSystem : ISystem
{
    private EntityManager _entityManager;
    private EventsWriter<PlayerHitEvent> _playerHit;
    private EventsReader<Collision2DEnter> _collisionEnter;
    private ReadOnlyStorage<Transform2D> _transform;

    public void Init(in SystemInitializer init)
    {
        _playerHit = init.GetEventsWriter<PlayerHitEvent>();
        _entityManager = init.GetEntityManager();
        _collisionEnter = init.GetEventsReader<Collision2DEnter>();
        _transform = init.GetReadOnlyStorage<Transform2D>();
    }

    public void Update()
    {
        foreach (ref readonly var @event in _collisionEnter)
        {
            if (@event.Target.Category != CollisionCategories.Player)
            {
                continue;
            }

            if (@event.Source.Category == CollisionCategories.Bullet)
            {
                _entityManager.Destroy(@event.Source.Entity);
                _playerHit.Send(new PlayerHitEvent
                {
                    Position = _transform[@event.Target.Entity].Position
                });
                break;
            }

            if (@event.Source.Category == CollisionCategories.Invader)
            {
                _playerHit.Send(new PlayerHitEvent
                {
                    Position = _transform[@event.Target.Entity].Position,
                    CollisionWithInvader = true
                });
                break;
            }
        }
    }

    public bool ShouldRun() 
        => _collisionEnter.HasEvents();
}
