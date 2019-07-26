using System.Collections.Generic;
using System.Linq;
using BotBase.Board;
using PaperIoStrategy.AISolver.ActionSolvers.Interfaces;

namespace PaperIoStrategy.AISolver.ActionSolvers
{
    public class PaperIoChecker : IActionComponentBase
    {
        public bool CanIGoTo(Board board, Direction direction)
        {
            return board.IPlayer.Direction.Invert() != direction &&
                   board.IPlayer.Position[direction].OnBoard(board.Size);
        }

        public IEnumerable<Direction> Order(Board board, IEnumerable<Direction> directions)
        {
            return directions.Where(d => CanIGoTo(board, d));
        }
    }
}
