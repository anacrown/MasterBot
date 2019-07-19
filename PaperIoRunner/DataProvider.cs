using System;
using System.Runtime.Serialization;
using System.Windows;
using CodenjoyBot.DataProvider;
using CodenjoyBot.Interfaces;

namespace PaperIoRunner
{
    [Serializable]
    public class DataProvider : IDataProvider
    {
        public event EventHandler<LogRecord> LogDataReceived;
        public UIElement Control { get; }
        public UIElement DebugControl { get; }

        public DataProvider()
        {

        }

        protected DataProvider(SerializationInfo info, StreamingContext context) : this()
        {

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }

        public string Title => Name;
        public string Name { get; } = "PaperIoSolver";

        public bool Cancel { get; set; } = false;
        public void Start()
        {
            uint time = 0;

            while (true)
            {
                var board = Console.ReadLine();
                DataReceived?.Invoke(this, new DataFrame() { Board = board, Time = time });

                time++;

                if (Cancel) break;
            }
        }

        public void Stop()
        {

        }

        public event EventHandler Started;
        public event EventHandler Stopped;
        public void SendResponse(string response)
        {
            Console.WriteLine(response);
        }

        public event EventHandler<DataFrame> DataReceived;
    }
}