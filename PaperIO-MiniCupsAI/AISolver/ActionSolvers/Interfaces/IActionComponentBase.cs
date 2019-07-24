using CodenjoyBot.Board;

namespace PaperIO_MiniCupsAI.ActionSolvers.Interfaces
{
    public interface IActionComponentBase
    {
        bool CanIGoTo(Board board, Direction direction);
    }
}
