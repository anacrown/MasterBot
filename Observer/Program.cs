using System;
using System.Runtime.Serialization;
using System.Text;
using System.Windows;
using CodenjoyBot.Board;
using CodenjoyBot.CodenjoyBotInstance;
using CodenjoyBot.DataProvider;
using CodenjoyBot.DataProvider.WebSocketDataProvider;
using CodenjoyBot.Interfaces;
using Application = System.Windows.Forms.Application;

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

        public EmptySolver() { }

        protected EmptySolver(SerializationInfo info, StreamingContext context) : this() { }

        public void GetObjectData(SerializationInfo info, StreamingContext context) { }

        public void Initialize() { }

        public UIElement Control => _label ?? (_label = new System.Windows.Controls.Label());
        public UIElement DebugControl => Control;


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
        public event EventHandler<LogRecord> LogDataReceived;
    }
}
