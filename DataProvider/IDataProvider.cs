using System;

namespace DataProvider
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