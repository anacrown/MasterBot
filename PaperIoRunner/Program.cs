using System;
using System.Runtime.Serialization;
using BotBase;
using BotBase.Board;
using CodenjoyBot.Interfaces;
using Newtonsoft.Json;
using PaperIoStrategy.AISolver;
using PaperIoStrategy.DataContract;

namespace PaperIoRunner
{
    public class PaperIoSolver
    {
        private JPacket _startInfo;

        public event EventHandler<Board> BoardChanged;
        public event EventHandler<LogRecord> LogDataReceived;

        public PaperIoSolver()
        {

        }

        public PaperIoSolver(SerializationInfo info, StreamingContext context) : this() { }

        public void GetObjectData(SerializationInfo info, StreamingContext context) { }

        public void Initialize() { }

        public bool Answer(string instanceName, DateTime startTime, DataFrame frame, out string response)
        {
            response = string.Empty;
            var jPacket = JsonConvert.DeserializeObject<JPacket>(frame.Board);

            if (jPacket.PacketType == JPacketType.EndGame)
            {
                return false;
            }

            var board = new Board(frame, jPacket.PacketType == JPacketType.StartGame ? _startInfo = jPacket : jPacket.Merge(_startInfo));
            OnBoardChanged(board);

            if (board.JPacket.PacketType != JPacketType.Tick) return false;

            response = board.GetResponse();

            return true;
        }

        protected virtual void OnLogDataReceived(LogRecord e) => LogDataReceived?.Invoke(this, e);
        protected virtual void OnBoardChanged(Board e) => BoardChanged?.Invoke(this, e);
    }

    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            //AppData.SetRelative(@"..\..\..\Debugger\App_Data");

            var solver = new PaperIoSolver();
            var dataProvider = new DataProvider();

            dataProvider.DataReceived += (sender, frame) =>
            {
                try
                {
                    if (solver.Answer("", DateTime.Now, frame, out var response))
                    {
                        dataProvider.SendResponse(response);
                    }
                }
                catch (Exception e)
                {

                }
            };

            dataProvider.Start();
        }
    }
}
