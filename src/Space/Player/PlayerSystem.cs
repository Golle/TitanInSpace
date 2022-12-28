using Titan.BuiltIn.Components;
using Titan.Core.Logging;
using Titan.Core.Maths;
using Titan.ECS.Queries;
using Titan.Input;
using Titan.Systems;

namespace Space.Player;

internal struct PlayerSystem : ISystem
{
    private MutableStorage<Transform2D> _transform;
    private ReadOnlyStorage<PlayerComponent> _player;
    private InputManager _input;
    private EntityQuery _query;

    public void Init(in SystemInitializer init)
    {
        _transform = init.GetMutableStorage<Transform2D>();
        _player = init.GetReadOnlyStorage<PlayerComponent>();
        _input = init.GetInputManager();
        _query = init.CreateQuery(new EntityQueryArgs().With<PlayerComponent>().With<Transform2D>());
    }

    public void Update()
    {
        ref readonly var playerEntity = ref _query[0];
        ref readonly var player = ref _player.Get(playerEntity);
        if (_input.IsKeyDown(KeyCode.Up))
        {
            _transform[playerEntity].Position.Y += player.CurrentSpeed;
            Logger.Trace<PlayerSystem>($"Moving up {_transform[playerEntity].Position}");
        }

        if (_input.IsKeyDown(KeyCode.Down))
        {
            _transform[playerEntity].Position.Y -= player.CurrentSpeed;
            Logger.Trace<PlayerSystem>($"Moving down {_transform[playerEntity].Position}");
        }
        
        if (_input.IsKeyDown(KeyCode.Right))
        {
            _transform[playerEntity].Position.X += player.CurrentSpeed;
            Logger.Trace<PlayerSystem>($"Moving right {_transform[playerEntity].Position}");
        }
        
        if (_input.IsKeyDown(KeyCode.Left))
        {
            _transform[playerEntity].Position.X -= player.CurrentSpeed;
            Logger.Trace<PlayerSystem>($"Moving left {_transform[playerEntity].Position}");
        }
    }

    public bool ShouldRun() => _query.HasEntities();
}
