using System.Numerics;
using Titan.BuiltIn.Components;
using Titan.Core;
using Titan.ECS.Queries;
using Titan.Systems;
using Titan.Windows;

namespace Space.Game;

internal struct GameCameraSystem : ISystem
{
    private EntityQuery _query;
    private MutableStorage<Camera2D> _camera;
    private ReadOnlyResource<GameState> _gameState;
    private ObjectHandle<IWindow> _window;
    private MutableStorage<Transform2D> _transform;

    public void Init(in SystemInitializer init)
    {
        _query = init.CreateQuery(new EntityQueryArgs().With<Transform2D>().With<Camera2D>());
        _camera = init.GetMutableStorage<Camera2D>();
        _transform = init.GetMutableStorage<Transform2D>();

        _gameState = init.GetReadOnlyResource<GameState>(false); // no tracking needed since we'll only read a readonly property. Change this if we need something that can change.
        _window = init.GetManagedApi<IWindow>();
    }

    public void Update()
    {
        foreach (ref readonly var entity in _query)
        {
            ref var camera = ref _camera[entity];
            ref readonly var boardSize = ref _gameState.Get().BoardSize;

            var diff = boardSize / _window.Value.Size;
            var minDiff = MathF.Max(diff.Width, diff.Height);
            camera.Size = _window.Value.Size * minDiff;
            
            ref var transform = ref _transform[entity];
            transform.Position = new Vector2(boardSize.Width/2f, boardSize.Height/2f);
        }
    }
}
