using System;
using System.Runtime.Serialization;
using BotBase.Interfaces;

namespace BotBase
{
    [Serializable]
    public abstract class DataLoggerSettingsBase : SettingsBase
    {
        protected DataLoggerSettingsBase() : base()
        {
        }

        protected DataLoggerSettingsBase(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public abstract IDataLogger CreateDataLogger();
    }
}