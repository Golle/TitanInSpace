using System.Numerics;
using Space.Events;
using Titan.BuiltIn.Components;
using Titan.BuiltIn.Resources;
using Titan.ECS;
using Titan.ECS.Queries;
using Titan.Events;
using Titan.Systems;

namespace Space.Player;

internal struct CameraShakeSystem : ISystem
{
    private EntityQuery _cameraQuery;
    private MutableStorage<Transform2D> _transform;
    private EntityManager _entityManager;
    private ReadOnlyResource<TimeStep> _timestep;
    private EventsReader<PlayerHitEvent> _playerHit;

    private float _shakeTimer;

    public void Init(in SystemInitializer init)
    {
        _transform = init.GetMutableStorage<Transform2D>();
        _entityManager = init.GetEntityManager();
        _cameraQuery = init.CreateQuery(new EntityQueryArgs().With<Camera2D>().With<Transform2D>());
        _timestep = init.GetReadOnlyResource<TimeStep>();
        _playerHit = init.GetEventsReader<PlayerHitEvent>();
    }

    public void Update()
    {
        if (_playerHit.HasEvents())
        {
            _shakeTimer = 0.4f;
        }

        if (_shakeTimer <= 0)
        {
            return;
        }
        _shakeTimer -= _timestep.Get().DeltaTimeSecondsF;

        foreach (ref readonly var entity in _cameraQuery)
        {
            var parent = _entityManager.GetParent(entity);
            ref var transform = ref _transform[parent];

            if (_shakeTimer > 0)
            {
                var multiplier = 10f;
                var rand1 = (Random.Shared.NextSingle() * 2f - 1);
                var rand2 = (Random.Shared.NextSingle() * 2f - 1);
                transform.Position.X = rand1 * multiplier;
                transform.Position.Y = rand2 * multiplier;
            }
            else
            {
                transform.Position = Vector2.Zero;
            }

        }
    }
}
