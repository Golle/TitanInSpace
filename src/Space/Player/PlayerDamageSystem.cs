using System.Diagnostics;
using Space.Events;
using Space.Game;
using Titan.ECS;
using Titan.ECS.Queries;
using Titan.Events;
using Titan.Systems;

namespace Space.Player;

internal struct PlayerDamageSystem : ISystem
{
    private EntityQuery _entities;
    private EventsReader<PlayerHitEvent> _playerHit;
    private EntityManager _entityManager;
    private MutableResource<GameState> _gameState;
    private EventsWriter<GameEndedEvent> _gameEnded;
    private EventsWriter<PlayerRespawnEvent> _playerRespawn;

    public void Init(in SystemInitializer init)
    {
        _entities = init.CreateQuery(new EntityQueryArgs().With<PlayerComponent>());
        _entityManager = init.GetEntityManager();
        _playerHit = init.GetEventsReader<PlayerHitEvent>();
        _gameState = init.GetMutableResource<GameState>();
        _gameEnded = init.GetEventsWriter<GameEndedEvent>();
        _playerRespawn = init.GetEventsWriter<PlayerRespawnEvent>();
    }

    public void Update()
    {
        Debug.Assert(_entities.Count == 1, "More than a single player, this is not correct.");
        _entityManager.Destroy(_entities[0]);
        foreach (ref readonly var @event in _playerHit)
        {
            var livesLeft = --_gameState.Get().Lives;
            if (livesLeft <= 0 || @event.CollisionWithInvader)
            {
                _gameEnded.Send(default);
                _gameState.Get().CurrentState = GameStateTypes.EndGame;
            }
            else
            {
                _playerRespawn.Send(default);
            }
            // only look for a single event.
            break;
        }
    }

    public bool ShouldRun()
        => _playerHit.HasEvents() && _entities.HasEntities();
}
