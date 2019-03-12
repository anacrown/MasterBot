using System;

namespace DataProvider
{
    public interface ISolver
    {
        void Initialize();

        string Answer(string instanseName, DateTime startTime, uint time, string board);
        //event EventHandler<Board> BoardLoaded;
    }
}
