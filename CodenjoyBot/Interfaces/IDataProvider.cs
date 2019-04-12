using System;
using System.Runtime.Serialization;
using CodenjoyBot.DataProvider;

namespace CodenjoyBot.Interfaces
{
    public interface IDataProvider : ILogger, ISupportControls, ISerializable
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