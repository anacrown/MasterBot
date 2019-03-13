using System;
using System.Windows;
using System.Windows.Controls;

namespace CodenjoyBot.Interfaces
{
    public interface ISolver
    {
        void Initialize();

        UIElement Control { get; }

        string Answer(Board.Board board);
    }
}
