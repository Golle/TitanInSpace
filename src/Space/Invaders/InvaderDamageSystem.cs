using Space.Events;
using Space.Game;
using Titan.BuiltIn.Components;
using Titan.BuiltIn.Events;
using Titan.Core.Logging;
using Titan.ECS;
using Titan.Events;
using Titan.Systems;

namespace Space.Invaders;

internal struct InvaderDamageSystem : ISystem
{
    private EntityManager _entityManager;
    private EventsReader<Collision2DEnter> _collisionEnter;
    private MutableResource<GameState> _gameState;
    private EventsWriter<InvaderDestroyedEvent> _invaderDestroyed;
    private ReadOnlyStorage<Transform2D> _transform;
    private ReadOnlyStorage<InvaderComponent> _invader;

    public void Init(in SystemInitializer init)
    {
        _collisionEnter = init.GetEventsReader<Collision2DEnter>();
        _entityManager = init.GetEntityManager();
        _gameState = init.GetMutableResource<GameState>();
        _invaderDestroyed = init.GetEventsWriter<InvaderDestroyedEvent>();
        _transform = init.GetReadOnlyStorage<Transform2D>();
        _invader = init.GetReadOnlyStorage<InvaderComponent>();
    }

    public void Update()
    {
        foreach (ref readonly var @event in _collisionEnter)
        {
            if (@event.Source.Category == CollisionCategories.Bullet && @event.Target.Category == CollisionCategories.Invader)
            {
                _entityManager.Destroy(@event.Source.Entity);
                _entityManager.Destroy(@event.Target.Entity);
                _invaderDestroyed.Send(new InvaderDestroyedEvent
                {
                    Type = _invader[@event.Target.Entity].Type,
                    Position = _transform[@event.Target.Entity].GetWorldPosition()
                });
            }
        }
    }
    public bool ShouldRun() => _collisionEnter.HasEvents();
}
