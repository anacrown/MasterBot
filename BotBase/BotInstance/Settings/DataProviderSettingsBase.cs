using System;
using System.Runtime.Serialization;
using BotBase.Interfaces;

namespace BotBase
{
    [Serializable]
    public abstract class DataProviderSettingsBase : SettingsBase
    {
        protected DataProviderSettingsBase() : base()
        {
        }

        protected DataProviderSettingsBase(SerializationInfo info, StreamingContext context)
        {

        }

        public abstract IDataProvider CreateDataProvider();
    }
}