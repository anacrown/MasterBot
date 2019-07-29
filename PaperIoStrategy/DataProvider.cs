using System;
using BotBase;

namespace PaperIoStrategy
{
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
            uint frameNumber = 0;

            while (true)
            {
                var board = Console.ReadLine();

                if (string.IsNullOrEmpty(board)) break;

                DataReceived?.Invoke(this, new DataFrame(DateTime.Now, board, frameNumber));

                if (Cancel) break;

                frameNumber++;
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
}