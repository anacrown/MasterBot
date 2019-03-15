using System;
using System.Runtime.Serialization;
using CodenjoyBot.DataProvider;

namespace CodenjoyBot.Interfaces
{
    public interface IDataProvider : ILogger, ISupportControls, ISerializable
    {
        string Name { get; }

        void Start();
        void Stop();

        void SendResponse(string response);
        event EventHandler<DataFrame> DataReceived;
    }
}