using Titan.BuiltIn.Components;
using Titan.BuiltIn.Resources;
using Titan.ECS.Queries;
using Titan.Systems;

namespace Space.Enemies;

internal struct EnemyAnimationSystem : ISystem
{
    private EntityQuery _query;
    private ReadOnlyResource<TimeStep> _timeStep;
    private MutableStorage<EnemyComponent> _enemy;
    private MutableStorage<Sprite> _sprite;

    public void Init(in SystemInitializer init)
    {
        _enemy = init.GetMutableStorage<EnemyComponent>();
        _sprite = init.GetMutableStorage<Sprite>();
        _query = init.CreateQuery(new EntityQueryArgs().With<EnemyComponent>().With<Sprite>());
        _timeStep = init.GetReadOnlyResource<TimeStep>();
    }

    public void Update()
    {
        ref readonly var timestep = ref _timeStep.Get();
        foreach (ref readonly var entity in _query)
        {
            ref var enemy = ref _enemy[entity];
            enemy.TimeElapsed += timestep.DeltaTimeSecondsF;

            if (enemy.TimeElapsed >= 0.25f)
            {
                enemy.SpriteIndex = (enemy.SpriteIndex + 1) % 2;
                ref var sprite = ref _sprite[entity];
                sprite.SourceRect = enemy.SpriteIndex == 0 ? enemy.Sprite1 : enemy.Sprite2;
                enemy.TimeElapsed = 0;
            }
        }
    }
}
