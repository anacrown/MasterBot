using System.Windows;

namespace CodenjoyBot.Interfaces
{
    public interface ISolver : ILogger, ISupportControls
    {
        void Initialize();

        string Answer(Board.Board board);
    }
}
