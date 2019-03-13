using System;
using System.Text;
using System.Windows;
using CodenjoyBot;
using CodenjoyBot.Board;
using CodenjoyBot.DataProvider;
using CodenjoyBot.Interfaces;
using Application = System.Windows.Forms.Application;
using Control = System.Windows.Controls.Control;

namespace Observer
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var botInstance = new CodenjoyBotInstance(new WebSocketDataProvider(
                new IdentityUser("ws://codenjoy.com/codenjoy-contest/ws", "ys5wrdxqw59fqv1lnylg", "8322438021074089892")),
                new EmptySolver());
            botInstance.Start();

            Application.Run();
        }
    }

    class EmptySolver : ISolver
    {
        private System.Windows.Controls.Label _label;
        private readonly StringBuilder _sb = new StringBuilder();

        public void Initialize() { }

        public UIElement Control => _label ?? (_label = new System.Windows.Controls.Label());


        public string Answer(Board board)
        {
            _sb.Clear();
            for (var i = 0; i < board.Size; i++)
                _sb.AppendLine(board.ToString().Substring(i * board.Size, board.Size));

            Console.Clear();
            Console.WriteLine(_sb.ToString());

            return string.Empty;
        }

        public event EventHandler<Board> BoardLoaded;
    }
}
