using System;
using System.Text;
using System.Windows.Forms;
using CodenjoyBot;
using CodenjoyBot.DataProvider;
using CodenjoyBot.Interfaces;

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
        private int _size;
        private readonly StringBuilder _sb = new StringBuilder();

        public void Initialize() { }

        public string Answer(string instanseName, DateTime startTime, uint time, string board)
        {
            if (_size == 0)
                _size = (int)Math.Sqrt(board.Length);

            _sb.Clear();
            for (var i = 0; i < _size; i++)
                _sb.AppendLine(board.Substring(i * _size, _size));

            Console.Clear();
            Console.WriteLine(_sb.ToString());

            return string.Empty;
        }

        public event EventHandler<string> BoardLoaded;
    }
}
