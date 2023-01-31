using Titan.Core.Maths;
using Titan.ECS.Components;

namespace Space.Game;

internal struct GameState : IResource
{
    public readonly Size BoardSize;
    public GameStateTypes CurrentState;
    public GameState(Size boardSize)
    {
        BoardSize = boardSize;
    }
}
