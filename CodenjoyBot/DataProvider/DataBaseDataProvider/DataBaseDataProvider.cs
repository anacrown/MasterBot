using System;
using System.Runtime.Serialization;
using System.Windows;
using CodenjoyBot.Interfaces;

namespace CodenjoyBot.DataProvider.DataBaseDataProvider
{
    [Serializable]
    public class DataBaseDataProvider : IDataProvider
    {
        private UIElement _control;
        private UIElement _debugControl;

        public UIElement Control => _control ?? (_control = new DataBaseDataProviderControl(this));

        public UIElement DebugControl => _debugControl ?? (_debugControl = new DataBaseDataProviderDebugControl(this));

        public DataBaseDataProvider() { }

        protected DataBaseDataProvider(SerializationInfo info, StreamingContext context) : this() { }

        public void GetObjectData(SerializationInfo info, StreamingContext context) { }

        public string Name { get; }

        public void Start()
        {
            
        }

        public void Stop()
        {
            
        }

        public void SendResponse(string response)
        {
            
        }

        public event EventHandler<DataFrame> DataReceived;

        public event EventHandler<LogRecord> LogDataReceived;
    }
}
