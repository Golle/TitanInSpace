using System.Numerics;
using Titan.BuiltIn.Components;
using Titan.BuiltIn.Events;
using Titan.BuiltIn.Resources;
using Titan.ECS;
using Titan.ECS.Queries;
using Titan.Events;
using Titan.Input;
using Titan.Systems;

namespace Space.Player;

internal struct CameraShakeSystem : ISystem
{
    private EntityQuery _cameraQuery;
    private MutableStorage<Transform2D> _transform;
    private EntityManager _entityManager;
    private InputManager _input;
    private ReadOnlyResource<TimeStep> _timestep;

    private float _shakeTimer;
    private EventsReader<Collision2DEnter> _enter;

    public void Init(in SystemInitializer init)
    {
        _transform = init.GetMutableStorage<Transform2D>();
        _entityManager = init.GetEntityManager();
        _input = init.GetInputManager();
        _cameraQuery = init.CreateQuery(new EntityQueryArgs().With<Camera2D>().With<Transform2D>());
        _timestep = init.GetReadOnlyResource<TimeStep>();
        _enter = init.GetEventsReader<Collision2DEnter>();
    }

    public void Update()
    {
        foreach (ref readonly var  @event in _enter)
        {
            if (@event.Target.Category == CollisionCategories.Player)
            {
                _shakeTimer = 0.4f;
                break;
            }
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
