using System.Windows;

namespace CodenjoyBot.Interfaces
{
    public interface ISupportControls
    {
        UIElement Control { get; }
        UIElement DebugControl { get; }
    }
}
