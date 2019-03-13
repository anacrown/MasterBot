using System.Windows;

namespace CodenjoyBot.Interfaces
{
    public interface ISolver
    {
        void Initialize();

        UIElement Control { get; }

        string Answer(Board.Board board);
    }
}
