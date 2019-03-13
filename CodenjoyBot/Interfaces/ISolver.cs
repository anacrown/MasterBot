using System;

namespace CodenjoyBot.Interfaces
{
    public interface ISolver
    {
        void Initialize();

        string Answer(string instanseName, DateTime startTime, uint time, string board);
        event EventHandler<string> BoardLoaded;
    }
}
