using Titan.BuiltIn.Components;
using Titan.BuiltIn.Resources;
using Titan.ECS;
using Titan.ECS.Queries;
using Titan.Systems;

namespace Space.Player;

internal struct BulletSystem : ISystem
{
    private MutableStorage<Transform2D> _transform;
    private EntityQuery _query;
    private ReadOnlyResource<TimeStep> _timestep;
    private EntityManager _entityManager;

    private const float _speed = 1f;
    public void Init(in SystemInitializer init)
    {
        _transform = init.GetMutableStorage<Transform2D>();
        _query = init.CreateQuery(new EntityQueryArgs().With<BulletComponent>().With<Transform2D>());
        _timestep = init.GetReadOnlyResource<TimeStep>();
        _entityManager = init.GetEntityManager();
    }

    public void Update()
    {
        var timestep = _timestep.Get();
        foreach (ref readonly var entity in _query)
        {
            ref var transform = ref _transform[entity];
            if (transform.Position.Y > 1000)
            {
                _entityManager.Destroy(entity);
            }
            else
            {
                transform.Position.Y += _speed* 1000 * timestep.DeltaTimeSecondsF;
            }
        }
    }
}
