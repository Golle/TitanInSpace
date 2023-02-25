using Space.Game;
using Titan.BuiltIn.Events;
using Titan.ECS;
using Titan.Events;
using Titan.Systems;

namespace Space.Invaders;

internal struct InvaderDamageSystem : ISystem
{
    private EntityManager _entityManager;
    private EventsReader<Collision2DEnter> _collisionEnter;
    private MutableResource<GameState> _gameState;

    public void Init(in SystemInitializer init)
    {
        _collisionEnter = init.GetEventsReader<Collision2DEnter>();
        _entityManager = init.GetEntityManager();
        _gameState = init.GetMutableResource<GameState>();
    }

    public void Update()
    {
        foreach (ref readonly var @event in _collisionEnter)
        {
            if (@event.Source.Category == CollisionCategories.Bullet && @event.Target.Category == CollisionCategories.Invader)
            {
                _entityManager.Destroy(@event.Source.Entity);
                _entityManager.Destroy(@event.Target.Entity);
                _gameState.Get().Score += 10;
            }
        }
    }
    public bool ShouldRun() => _collisionEnter.HasEvents();
}
