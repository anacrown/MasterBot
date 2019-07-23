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
        public event EventHandler Started;
        public event EventHandler Stopped;
        public event EventHandler<DataFrame> DataReceived;
        public event EventHandler<LogRecord> LogDataReceived;
        public UIElement Control { get; } = null;
        public UIElement DebugControl { get; } = null;

        public DataProvider() { }

        protected DataProvider(SerializationInfo info, StreamingContext context) : this() { }

        public void GetObjectData(SerializationInfo info, StreamingContext context) { }

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
        protected virtual void OnLogDataReceived(LogRecord e) => LogDataReceived?.Invoke(this, e);
    }
}