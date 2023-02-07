using Titan.Core.Maths;
using Titan.ECS.Components;

namespace Space.Game;

internal struct GameState : IResource
{
    public readonly Size BoardSize;
    public readonly uint InvaderRows;
    public readonly uint InvaderColumns;
    public GameStateTypes CurrentState;
    public GameState(Size boardSize, uint invaderRows, uint invaderColumns)
    {
        BoardSize = boardSize;
        InvaderRows = invaderRows;
        InvaderColumns = invaderColumns;
    }
}
