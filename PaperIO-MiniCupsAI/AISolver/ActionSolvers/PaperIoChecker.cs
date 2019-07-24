using CodenjoyBot.Board;
using PaperIO_MiniCupsAI.ActionSolvers.Interfaces;

namespace PaperIO_MiniCupsAI.ActionSolvers
{
    public class PaperIoChecker : IActionComponentBase
    {
        public bool CanIGoTo(Board board, Direction direction)
        {
            return board.IPlayer.Direction.Invert() != direction &&
                    board.IPlayer.Position[direction].OnBoard(board.Size);
        }
    }
}
