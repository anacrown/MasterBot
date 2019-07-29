using System;
using System.Runtime.Serialization;

namespace BotBase
{
    [Serializable]
    public abstract class SettingsBase : ISerializable
    {
        protected SettingsBase()
        {
        }

        protected SettingsBase(SerializationInfo info, StreamingContext context)
        {

        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }
    }
}