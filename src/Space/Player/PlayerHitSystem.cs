using Space.Events;
using Titan.BuiltIn.Events;
using Titan.ECS;
using Titan.Events;
using Titan.Systems;

namespace Space.Player;

internal struct PlayerHitSystem : ISystem
{
    private EventsWriter<PlayerHitEvent> _playerHit;
    private EventsReader<Collision2DEnter> _collisionEnter;
    private EntityManager _entityManager;

    public void Init(in SystemInitializer init)
    {
        _playerHit = init.GetEventsWriter<PlayerHitEvent>();
        _entityManager = init.GetEntityManager();
        _collisionEnter = init.GetEventsReader<Collision2DEnter>();
    }

    public void Update()
    {
        var playerHit = false;
        foreach (ref readonly var @event in _collisionEnter)
        {
            if (@event.Target.Category == CollisionCategories.Player && @event.Source.Category == CollisionCategories.Bullet)
            {
                _entityManager.Destroy(@event.Source.Entity);
                playerHit = true;
                break;
            }
        }
        if (playerHit)
        {
            _playerHit.Send(new PlayerHitEvent());
        }
    }
}
