using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using BotBase;
using BotBase.Interfaces;
using BotBase.Properties;

namespace WebSocketDataProvider
{
    [Serializable]
    public class WebSocketDataProviderSettings : DataProviderSettingsBase, INotifyPropertyChanged
    {

        public IdentityUser IdentityUser { get; set; } = new IdentityUser();

        public override IDataProvider CreateDataProvider() => new WebSocketDataProvider(this);

        public WebSocketDataProviderSettings() : base() { }

        public WebSocketDataProviderSettings(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            IdentityUser = info.GetValue("IdentityUser", typeof(IdentityUser)) as IdentityUser ?? new IdentityUser();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("IdentityUser", IdentityUser);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}