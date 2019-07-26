using System;
using BotBase.BotInstance;

namespace BotBase.Interfaces
{
    public interface IDataProvider : ILogger
    {
        string Title { get; }

        string Name { get; }

        void Start();
        void Stop();

        event EventHandler Started;
        event EventHandler Stopped;

        void SendResponse(string response);
        event EventHandler<DataFrame> DataReceived;
    }
}