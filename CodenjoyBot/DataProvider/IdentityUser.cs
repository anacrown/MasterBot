using System;

namespace CodenjoyBot.DataProvider
{
    [Serializable]
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

        //http://codenjoy.com/codenjoy-contest/board/player/j99lpu1l8skamhdzbyq9?code=7040034271572867319
        public override string ToString() => $"{ServerUri}?user={UserName}&code={SecretCode}";
    }
}