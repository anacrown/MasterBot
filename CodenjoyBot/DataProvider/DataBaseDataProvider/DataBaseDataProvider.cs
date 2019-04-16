﻿using System;
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

        public string Title { get; }
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

        public event EventHandler Started;
        public event EventHandler Stopped;

        public event EventHandler<DataFrame> DataReceived;

        public event EventHandler<LogRecord> LogDataReceived;

        protected bool Equals(DataBaseDataProvider other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DataBaseDataProvider) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}
