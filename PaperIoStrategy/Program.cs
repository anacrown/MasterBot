using System;
using BotBase;
using Newtonsoft.Json;
using PaperIoStrategy.AISolver;
using PaperIoStrategy.DataContract;

namespace PaperIoStrategy
{
    internal class Program
    {
        static void Main(string[] args)
        {
//            System.Diagnostics.Debugger.Launch();

            var solver = new PaperIoSolver();
            var dataProvider = new DataProvider();

            dataProvider.DataReceived += (sender, frame) =>
            {

                if (solver.Answer(frame.Board, out var response))
                {
                    dataProvider.SendResponse(response);
                }

            };

            dataProvider.Start();

        }
    }

    public class DataProvider
    {
        public event EventHandler Started;
        public event EventHandler Stopped;
        public event EventHandler<DataFrame> DataReceived;

        public string Title => Name;
        public string Name { get; } = "PaperIoSolver";

        public bool Cancel { get; set; } = false;
        public void Start()
        {
            uint time = 0;

            while (true)
            {
                var board = Console.ReadLine();

                if (string.IsNullOrEmpty(board)) break;

                DataReceived?.Invoke(this, new DataFrame(time, board));

                if (Cancel) break;

                time++;
            }
        }

        public void Stop()
        {
            Cancel = true;
            OnStopped();
        }
        public void SendResponse(string response)
        {
            Console.WriteLine(response);
        }
        protected virtual void OnStarted() => Started?.Invoke(this, EventArgs.Empty);
        protected virtual void OnStopped() => Stopped?.Invoke(this, EventArgs.Empty);
        protected virtual void OnDataReceived(DataFrame e) => DataReceived?.Invoke(this, e);
    }

    public class PaperIoSolver
    {
        private JPacket _startInfo;

        public event EventHandler<Board> BoardChanged;

        public bool Answer(string frame, out string response)
        {
            response = string.Empty;
            var jPacket = JsonConvert.DeserializeObject<JPacket>(frame);

            if (jPacket.PacketType == JPacketType.EndGame)
            {
                return false;
            }

            var board = new Board(new DataFrame(0, frame), jPacket.PacketType == JPacketType.StartGame ? _startInfo = jPacket : jPacket.Merge(_startInfo));
            OnBoardChanged(board);

            if (board.JPacket.PacketType != JPacketType.Tick) return false;

            response = board.GetResponse();

            return true;
        }

        protected virtual void OnBoardChanged(Board e) => BoardChanged?.Invoke(this, e);
    }
}
