using Space.Events;
using Space.Invaders;
using Titan.Events;
using Titan.Systems;

namespace Space.Game;
internal struct ScoreSystem : ISystem
{
    private EventsReader<InvaderDestroyedEvent> _invaderDestroyed;
    private MutableResource<GameState> _gameState;

    public void Init(in SystemInitializer init)
    {
        _invaderDestroyed = init.GetEventsReader<InvaderDestroyedEvent>();
        _gameState = init.GetMutableResource<GameState>();
    }

    public void Update()
    {
        foreach (ref readonly var @event in _invaderDestroyed)
        {
            var score = @event.Type switch
            {
                InvaderType.Basic => 10,
                InvaderType.Advanced => 20,
                InvaderType.Super => 30,
                InvaderType.Spaceship => 500,
                _ => 0
            };
            _gameState.Get().Score += score;
        }
    }

    public bool ShouldRun() => _invaderDestroyed.HasEvents();
}
