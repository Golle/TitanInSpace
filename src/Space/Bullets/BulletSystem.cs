using Space.Game;
using Titan.BuiltIn.Components;
using Titan.BuiltIn.Resources;
using Titan.ECS;
using Titan.ECS.Queries;
using Titan.Systems;

namespace Space.Bullets;

internal struct BulletSystem : ISystem
{
    private ReadOnlyResource<TimeStep> _timestep;
    private MutableStorage<Transform2D> _transform;
    private ReadOnlyStorage<BulletComponent> _bullet;
    private ReadOnlyResource<GameState> _gameState;
    private EntityQuery _query;
    private EntityManager _entityManager;

    private const float Speed = 1f;
    private const float SpeedMultiplier = 10;
    public void Init(in SystemInitializer init)
    {
        _transform = init.GetMutableStorage<Transform2D>();
        _bullet = init.GetReadOnlyStorage<BulletComponent>();
        _query = init.CreateQuery(new EntityQueryArgs().With<BulletComponent>().With<Transform2D>());
        _timestep = init.GetReadOnlyResource<TimeStep>();
        _entityManager = init.GetEntityManager();
        _gameState = init.GetReadOnlyResource<GameState>();
    }

    public void Update()
    {
        if (_gameState.Get().CurrentState == GameStateTypes.EndGame || _gameState.Get().CurrentState == GameStateTypes.LevelCompleted)
        {
            foreach (ref readonly var entity in _query)
            {
                _entityManager.Destroy(entity);
            }
            return;
        }

        var maxHeight = _gameState.Get().BoardSize.Height;
        var timestep = _timestep.Get();
        foreach (ref readonly var entity in _query)
        {
            ref var transform = ref _transform[entity];
            if (transform.Position.Y > maxHeight || transform.Position.Y < 0)
            {
                _entityManager.Destroy(entity);
            }
            else
            {
                var speed = _bullet[entity].Down ? -20 : 20;
                transform.Position.Y += Speed * SpeedMultiplier * speed * timestep.DeltaTimeSecondsF;
            }
        }
    }

    public bool ShouldRun() => _query.HasEntities();
}
