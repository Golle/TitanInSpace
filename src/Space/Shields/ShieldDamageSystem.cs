using Titan.BuiltIn.Components;
using Titan.ECS;
using Titan.ECS.Queries;
using Titan.Systems;

namespace Space.Shields;

internal struct ShieldDamageSystem : ISystem
{
    private static readonly int MaxDamage = SpriteRectangles.Shields.Length - 1;
    private EntityQuery _query;
    private MutableStorage<Sprite> _sprites;
    private MutableStorage<ShieldComponent> _shields;
    private EntityManager _entityManager;

    public void Init(in SystemInitializer init)
    {
        _entityManager = init.GetEntityManager();
        _sprites = init.GetMutableStorage<Sprite>();
        _shields = init.GetMutableStorage<ShieldComponent>();
        _query = init.CreateQuery(new EntityQueryArgs().With<ShieldComponent>().With<Sprite>());
    }

    public void Update()
    {
        return;
        foreach (ref readonly var entity in _query)
        {
            ref var sprite = ref _sprites[entity];
            ref var shield = ref _shields[entity];

            if (Random.Shared.Next(0, 6000) < 40)
            {
                shield.Damage++;
                if (shield.Damage >= MaxDamage)
                {
                    _entityManager.Destroy(entity);
                }
                else
                {
                    sprite.SourceRect = SpriteRectangles.Shields[shield.Damage];
                }
            }
        }
    }


    public bool ShouldRun() => _query.HasEntities();
}
