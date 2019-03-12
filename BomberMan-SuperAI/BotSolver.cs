using System;
using DataProvider;

namespace BomberMan_SuperAI
{
    public class BotSolver : ISolver
    {
        public void Initialize()
        {
            
        }

        public string Answer(string instanseName, DateTime startTime, uint time, string board)
        {
            /*
             
            Команд несколько: UP, DOWN, LEFT, RIGHT – приводят к движению героя в заданном направлении на 1 клетку; 
            ACT - оставить бомбу на месте героя. Команды движения можно комбинировать с командой ACT, разделяя их через запятую. 
            Порядок (LEFT, ACT) или (ACT, LEFT) - имеет значение, либо двигаемся влево и там ставим бомбу, либо ставим бомбу а затем тикаем влево. 
            Если игрок будет использовать только одну команду ACT, то бомба установится под героем без его перемещения на поле.
             
             */

            Console.WriteLine(board);

            return string.Empty;
        }

        public event EventHandler<string> BoardLoaded;
    }
}
