using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using BomberMan_SuperAI.Annotations;
using CodenjoyBot.Board;
using CodenjoyBot.Interfaces;

namespace BomberMan_SuperAI
{
    public class BomberSolver : ISolver, INotifyPropertyChanged
    {
        private UserControl _control;

        public void Initialize()
        {
            
        }

        public UIElement Control => _control ?? (_control = new BomberSolverControl(this));

        public string Answer(Board board)
        {
            /*
             
            Команд несколько: UP, DOWN, LEFT, RIGHT – приводят к движению героя в заданном направлении на 1 клетку; 
            ACT - оставить бомбу на месте героя. Команды движения можно комбинировать с командой ACT, разделяя их через запятую. 
            Порядок (LEFT, ACT) или (ACT, LEFT) - имеет значение, либо двигаемся влево и там ставим бомбу, либо ставим бомбу а затем тикаем влево. 
            Если игрок будет использовать только одну команду ACT, то бомба установится под героем без его перемещения на поле.
             
             */

            var move = new[]
            {
                "UP",
                "DOWN",
                "LEFT",
                "RIGHT",
            };

            var act = "ACT";

            var m = new Random().Next(4);
            var a = new Random().Next(2);
            var b = new Random().Next(2);

            var rsp = a == 1 ? b == 1 ? $"{act},{move[m]}" : $"{move[m]},{act}" : move[m];

            Console.WriteLine($"{m} {a} {b} {rsp}");

            return rsp;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
