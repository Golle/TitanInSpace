using Titan.ECS.Components;

namespace Space.Game;

internal struct GameState : IResource
{
    public GameStateTypes CurrentState;
}