using Space.Game;
using Titan.BuiltIn.Components;
using Titan.BuiltIn.Resources;
using Titan.Core.Maths;
using Titan.ECS.Queries;
using Titan.Input;
using Titan.Systems;

namespace Space.Player;

internal struct PlayerMovementSystem : ISystem
{
    private MutableStorage<Transform2D> _transform;
    private MutableStorage<PlayerComponent> _player;
    private ReadOnlyResource<TimeStep> _timeStep;
    private ReadOnlyResource<GameState> _gameState;
    private InputManager _input;
    private EntityQuery _query;

    public void Init(in SystemInitializer init)
    {
        _transform = init.GetMutableStorage<Transform2D>();
        _player = init.GetMutableStorage<PlayerComponent>();
        _input = init.GetInputManager();
        _query = init.CreateQuery(new EntityQueryArgs().With<PlayerComponent>().With<Transform2D>());
        _timeStep = init.GetReadOnlyResource<TimeStep>();
        _gameState = init.GetReadOnlyResource<GameState>();
    }

    public void Update()
    {
        ref readonly var playerEntity = ref _query[0];
        ref readonly var timestep = ref _timeStep.Get();
        ref readonly var gameState = ref _gameState.Get();
        ref var transform = ref _transform[playerEntity];

        ref var player = ref _player.Get(playerEntity);
        player.IsMoving = false;
        var length = Math.Clamp(player.ElapsedTimeMoving / 0.3f, 0f, 1f);
        var speed = (Easings.EasyOutCubic(length) * player.MaxSpeed + player.StartSpeed) * timestep.DeltaTimeSecondsF;

        if (_input.IsKeyDown(KeyCode.Right) || _input.IsKeyDown(KeyCode.D))
        {
            transform.Position.X += speed;
            player.IsMoving = true;
        }

        if (_input.IsKeyDown(KeyCode.Left) || _input.IsKeyDown(KeyCode.A))
        {
            transform.Position.X -= speed;
            player.IsMoving = true;
        }

        if (player.IsMoving)
        {
            player.ElapsedTimeMoving += timestep.DeltaTimeSecondsF;
        }
        else
        {
            player.ElapsedTimeMoving = 0;
        }

        var halfWidth = player.Width / 2f;
        transform.Position.X = Math.Clamp(transform.Position.X, halfWidth, gameState.BoardSize.Width - halfWidth);
    }

    public bool ShouldRun() => _query.HasEntities();
}
