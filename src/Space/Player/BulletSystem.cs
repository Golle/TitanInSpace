using Space.Game;
using Titan.BuiltIn.Components;
using Titan.BuiltIn.Resources;
using Titan.ECS;
using Titan.ECS.Queries;
using Titan.Systems;

namespace Space.Player;

internal struct BulletSystem : ISystem
{
    private ReadOnlyResource<TimeStep> _timestep;
    private MutableStorage<Transform2D> _transform;
    private ReadOnlyResource<GameState> _gameState;
    private EntityQuery _query;
    private EntityManager _entityManager;

    private const float Speed = 1f;
    private const float SpeedMultiplier = 10;
    public void Init(in SystemInitializer init)
    {
        _transform = init.GetMutableStorage<Transform2D>();
        _query = init.CreateQuery(new EntityQueryArgs().With<BulletComponent>().With<Transform2D>());
        _timestep = init.GetReadOnlyResource<TimeStep>();
        _entityManager = init.GetEntityManager();
        _gameState = init.GetReadOnlyResource<GameState>();
    }

    public void Update()
    {
        var maxHeight = _gameState.Get().BoardSize.Height;
        var timestep = _timestep.Get();
        foreach (ref readonly var entity in _query)
        {
            ref var transform = ref _transform[entity];
            if (transform.Position.Y > maxHeight)
            {
                _entityManager.Destroy(entity);
            }
            else
            {
                transform.Position.Y += Speed * SpeedMultiplier * 20 * timestep.DeltaTimeSecondsF;
            }
        }
    }
}
