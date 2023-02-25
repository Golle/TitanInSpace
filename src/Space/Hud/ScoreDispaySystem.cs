using Space.Game;
using Titan.BuiltIn.Components;
using Titan.ECS;
using Titan.ECS.Entities;
using Titan.ECS.Queries;
using Titan.Systems;

namespace Space.Hud;
internal struct ScoreDispaySystem : ISystem
{
    private EntityQuery _entities;
    private MutableStorage<Sprite> _sprite;
    private EntityManager _entityManager;
    private ReadOnlyResource<GameState> _gameState;

    public void Init(in SystemInitializer init)
    {
        _sprite = init.GetMutableStorage<Sprite>();
        _entityManager = init.GetEntityManager();
        _entities = init.CreateQuery(new EntityQueryArgs().With<ScoreComponent>());
        _gameState = init.GetReadOnlyResource<GameState>();
    }

    public void Update()
    {
        var gameState = _gameState.Get();
        var entity = _entities[0];
        Span<Entity> children = stackalloc Entity[10];
        var count = _entityManager.GetChildren(entity, children);
        var score = gameState.Score;
        for (var i = count - 1; i >= 0; i--)
        {
            var digit = score % 10;
            score /= 10;
            _sprite[children[i]].SourceRect = SpriteRectangles.Numbers[digit];
        }
    }

    public bool ShouldRun()
        => _entities.HasEntities();
}
