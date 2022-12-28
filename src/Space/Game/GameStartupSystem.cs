using System.Numerics;
using Space.Player;
using Titan.BuiltIn.Components;
using Titan.ECS;
using Titan.Systems;

namespace Space.Game;

internal struct GameStartupSystem : ISystem
{
    private EntityManager _entityManager;
    private MutableResource<GameState> _gameState;
    private ComponentManager _componentsManager;

    public void Init(in SystemInitializer init)
    {
        _entityManager = init.GetEntityManager();
        _gameState = init.GetMutableResource<GameState>();
        _componentsManager = init.GetComponentManager();
    }

    public void Update()
    {
        var entity = _entityManager.Create();

        _componentsManager.AddComponent(entity, new Transform2D(Vector2.One * 200));
        _componentsManager.AddComponent(entity, new PlayerComponent { CurrentSpeed = 1.0f });
        //_componentsManager.AddComponent(entity, new Sprite
        //{
        //    //Asset = //
        //    Color = Color.Red
        //});

        _gameState.Get().CurrentState = GameStateTypes.Playing;
    }

    public bool ShouldRun() => _gameState.Get().CurrentState == GameStateTypes.Startup;
}
