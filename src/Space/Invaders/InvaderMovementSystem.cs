using System.Numerics;
using Space.Game;
using Titan.BuiltIn.Components;
using Titan.BuiltIn.Resources;
using Titan.ECS;
using Titan.ECS.Queries;
using Titan.Systems;

namespace Space.Invaders;

internal struct InvaderMovementSystem : ISystem
{
    private MutableStorage<Transform2D> _transform;
    private ReadOnlyResource<TimeStep> _timestep;
    private ReadOnlyResource<GameState> _gamestate;

    private EntityQuery _query;

    private InvaderMoveDirection _direction;
    private EntityManager _entityManager;

    public void Init(in SystemInitializer init)
    {
        _transform = init.GetMutableStorage<Transform2D>();
        _query = init.CreateQuery(new EntityQueryArgs().With<Transform2D>().With<InvaderComponent>());
        _timestep = init.GetReadOnlyResource<TimeStep>();
        _gamestate = init.GetReadOnlyResource<GameState>();
        _entityManager = init.GetEntityManager();
    }

    public void Update()
    {
        ref readonly var gameState = ref _gamestate.Get();
        ref readonly var boardSize = ref _gamestate.Get().BoardSize;
        
        var totalInvaders = gameState.InvaderColumns * gameState.InvaderRows;
        var currentInvaders = _query.Count;
        var invadersDelta = (totalInvaders - currentInvaders) / (float)totalInvaders;


        // tweak this later :)
        const float minSpeed = 6f;
        const float maxSpeed = 80f;
        const float jump = 4f;
        var speed = invadersDelta * maxSpeed + minSpeed;
        
        var timestep = _timestep.Get().DeltaTimeSecondsF;
        var delta = _direction switch
        {
            InvaderMoveDirection.Left => -Vector2.UnitX * speed * timestep,
            InvaderMoveDirection.Right => Vector2.UnitX * speed * timestep,
            InvaderMoveDirection.DownLeft or InvaderMoveDirection.DownRight => -Vector2.UnitY * jump,
            _ => Vector2.Zero
        };

        // update the direction
        _direction = _direction switch
        {
            InvaderMoveDirection.DownRight => InvaderMoveDirection.Right,
            InvaderMoveDirection.DownLeft => InvaderMoveDirection.Left,
            _ => _direction
        };

        var invaderWidth = SpriteRectangles.Monster1_0.Width;
        var nextDirection = _direction;
        foreach (ref readonly var entity in _query)
        {
            ref var transform = ref _transform[entity];
            transform.Position += delta;

            if (_direction == InvaderMoveDirection.Right && transform.Position.X + invaderWidth >= boardSize.Width)
            {
                nextDirection = InvaderMoveDirection.DownLeft;
            }
            else if (_direction == InvaderMoveDirection.Left && transform.Position.X <= 0)
            {
                nextDirection = InvaderMoveDirection.DownRight;
            }

            if (Random.Shared.Next(10000) < 15)
            {
                _entityManager.Destroy(entity);
            }
        }
        _direction = nextDirection;
    }

    public bool ShouldRun() => _query.HasEntities() && _gamestate.Get().CurrentState == GameStateTypes.Playing;
}
