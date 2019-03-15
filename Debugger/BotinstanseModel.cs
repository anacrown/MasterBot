using System.Configuration;
using CodenjoyBot.DataProvider;

namespace Debugger
{
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class BotinstanseModel
    {
        public string SolverType { get; set; }

        public string DataProviderType { get; set; }

        public IdentityUser IdentityUser { get; set; }

        public string BoardFile { get; set; }

        public BotinstanseModel() { }
    }
}