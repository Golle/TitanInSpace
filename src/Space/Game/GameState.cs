using Titan.Core.Maths;
using Titan.ECS.Components;

namespace Space.Game;

internal struct GameState : IResource
{
    public readonly Size BoardSize;
    public readonly uint InvaderRows;
    public readonly uint InvaderColumns;
    public readonly float InvaderMinShootingCooldown;
    public readonly float InvaderMaxShootingCooldown;

    public GameStateTypes CurrentState;
    public int Score;
    public GameState(Size boardSize, uint invaderRows, uint invaderColumns, float invaderMinShootingCooldown, float invaderMaxShootingCooldown)
    {
        BoardSize = boardSize;
        InvaderRows = invaderRows;
        InvaderColumns = invaderColumns;
        InvaderMinShootingCooldown = invaderMinShootingCooldown;
        InvaderMaxShootingCooldown = invaderMaxShootingCooldown;
    }
}
