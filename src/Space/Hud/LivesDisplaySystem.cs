using System.Diagnostics;
using Space.Game;
using Titan.BuiltIn.Components;
using Titan.Core.Maths;
using Titan.ECS;
using Titan.ECS.Entities;
using Titan.ECS.Queries;
using Titan.Systems;

namespace Space.Hud;
internal struct LivesDisplaySystem : ISystem
{
    private EntityQuery _query;
    private MutableStorage<Sprite> _sprite;
    private EntityManager _entityManager;
    private MutableResource<GameState> _gamestate;

    public void Init(in SystemInitializer init)
    {
        _entityManager = init.GetEntityManager();
        _query = init.CreateQuery(new EntityQueryArgs().With<LivesComponent>());
        _sprite = init.GetMutableStorage<Sprite>();
        _gamestate = init.GetMutableResource<GameState>();
    }

    public void Update()
    {
        Debug.Assert(_query.Count == 1, "Multiple LivesComponents in query.");
        ref readonly var gameState = ref _gamestate.Get();

        Span<Entity> children = stackalloc Entity[(int)gameState.MaxLives];
        var count = _entityManager.GetChildren(_query[0], children);

        for (var i = 0; i < count; i++)
        {
            if (i >= gameState.Lives)
            {
                ref var sprite = ref _sprite[children[i]];
                sprite.Color = Color.Transparent;
            }
        }
    }

    public bool ShouldRun() => _query.HasEntities();
}
