using System.Configuration;
using CodenjoyBot.DataProvider;

namespace Debugger
{
    public enum DataProviderType
    {
        WebSocket, FileSystem
    }

    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class BotinstanseModel
    {
        public DataProviderType ProviderType { get; set; }

        public IdentityUser IdentityUser { get; set; }

        public string BoardFile { get; set; }

        public BotinstanseModel() { }
    }
}