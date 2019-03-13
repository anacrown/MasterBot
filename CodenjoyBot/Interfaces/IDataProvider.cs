using System;
using CodenjoyBot.DataProvider;

namespace CodenjoyBot.Interfaces
{
    public interface IDataProvider
    {
        string Name { get; }

        void Start();
        void Stop();

        void SendResponse(string response);
        event EventHandler<DataFrame> DataReceived;
    }
}