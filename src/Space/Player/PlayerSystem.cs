using Titan.BuiltIn.Components;
using Titan.BuiltIn.Resources;
using Titan.Core.Maths;
using Titan.ECS.Queries;
using Titan.Input;
using Titan.Systems;

namespace Space.Player;

internal struct PlayerSystem : ISystem
{
    private MutableStorage<Transform2D> _transform;
    private MutableStorage<PlayerComponent> _player;
    private ReadOnlyResource<TimeStep> _timeStep;
    private InputManager _input;
    private EntityQuery _query;

    public void Init(in SystemInitializer init)
    {
        _transform = init.GetMutableStorage<Transform2D>();
        _player = init.GetMutableStorage<PlayerComponent>();
        _input = init.GetInputManager();
        _query = init.CreateQuery(new EntityQueryArgs().With<PlayerComponent>().With<Transform2D>());
        _timeStep = init.GetReadOnlyResource<TimeStep>();
    }

    public void Update()
    {
        ref readonly var playerEntity = ref _query[0];
        ref readonly var timestep = ref _timeStep.Get();
        ref var player = ref _player.Get(playerEntity);
        player.IsMoving = false;
        var length = Math.Clamp(player.ElapsedTimeMoving / 0.3f, 0f, 1f);
        var speed = Easings.EasyOutCubic(length) * player.MaxSpeed + player.StartSpeed * timestep.DeltaTimeSecondsF;

        if (_input.IsKeyDown(KeyCode.Up) || _input.IsKeyDown(KeyCode.W))
        {
            _transform[playerEntity].Position.Y += speed;
            player.IsMoving = true;
        }

        if (_input.IsKeyDown(KeyCode.Down) || _input.IsKeyDown(KeyCode.S))
        {
            _transform[playerEntity].Position.Y -= speed;
            player.IsMoving = true;
        }

        if (_input.IsKeyDown(KeyCode.Right) || _input.IsKeyDown(KeyCode.D))
        {
            _transform[playerEntity].Position.X += speed;
            player.IsMoving = true;
        }

        if (_input.IsKeyDown(KeyCode.Left) || _input.IsKeyDown(KeyCode.A))
        {
            _transform[playerEntity].Position.X -= speed;
            player.IsMoving = true;
        }

        if (player.IsMoving)
        {
            player.ElapsedTimeMoving += 0.008f;
        }
        else
        {
            player.ElapsedTimeMoving = 0;
        }
    }

    public bool ShouldRun() => _query.HasEntities();
}
