using System.Collections.Generic;
using BotBase.Board;

namespace PaperIoStrategy.AISolver.ActionSolvers.Interfaces
{
    public interface IActionComponentBase
    {
        bool CanIGoTo(Board board, Direction direction);

        IEnumerable<Direction> Order(Board board, IEnumerable<Direction> directions);
    }
}
