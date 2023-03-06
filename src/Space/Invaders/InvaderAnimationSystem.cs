using Titan.BuiltIn.Components;
using Titan.BuiltIn.Resources;
using Titan.ECS.Queries;
using Titan.Systems;

namespace Space.Invaders;

internal enum InvaderType
{
    Basic = 0,
    Advanced = 1,
    Spaceship = 2
}

internal struct InvaderAnimationSystem : ISystem
{
    private EntityQuery _query;
    private ReadOnlyResource<TimeStep> _timeStep;
    private MutableStorage<InvaderComponent> _invader;
    private MutableStorage<Sprite> _sprite;

    public void Init(in SystemInitializer init)
    {
        _invader = init.GetMutableStorage<InvaderComponent>();
        _sprite = init.GetMutableStorage<Sprite>();
        _query = init.CreateQuery(new EntityQueryArgs().With<InvaderComponent>().With<Sprite>());
        _timeStep = init.GetReadOnlyResource<TimeStep>();
    }

    public void Update()
    {
        ref readonly var timestep = ref _timeStep.Get();
        foreach (ref readonly var entity in _query)
        {
            ref var invader = ref _invader[entity];
            invader.TimeElapsed += timestep.DeltaTimeSecondsF;

            if (invader.TimeElapsed >= 0.25f)
            {
                invader.SpriteIndex = (invader.SpriteIndex + 1) % 2;
                ref var sprite = ref _sprite[entity];
                sprite.SourceRect = SpriteRectangles.Invaders[(int)invader.Type][invader.SpriteIndex];
                invader.TimeElapsed = 0;
            }
        }
    }
}
