using System.Configuration;

namespace DataProvider
{
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class IdentityUser
    {
        public string ServerUri { get; set; }
        public string UserName { get; set; }
        public string SecretCode { get; set; }

        public bool IsEmty => string.IsNullOrEmpty(ServerUri) ||
                              string.IsNullOrEmpty(UserName) ||
                              string.IsNullOrEmpty(SecretCode);

        public IdentityUser() { }

        public IdentityUser(string serverUri, string userName, string secretCode)
        {
            SecretCode = secretCode;
            ServerUri = serverUri;
            UserName = userName;
        }

        public override string ToString() => $"{ServerUri}?user={UserName}&code={SecretCode}";
    }
}