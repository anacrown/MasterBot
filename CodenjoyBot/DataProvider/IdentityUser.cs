using System;
using System.Linq;

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

        public IdentityUser(string str) : this()
        {
            ParseUri(str);
        }

        public IdentityUser(string serverUri, string userName, string secretCode) : this()
        {
            SecretCode = secretCode;
            ServerUri = serverUri;
            UserName = userName;
        }

        //http://codenjoy.com/codenjoy-contest/board/player/j99lpu1l8skamhdzbyq9?code=7040034271572867319

        public void ParseUri(string str)
        {
            try
            {
                var uri = new Uri(str);

                ServerUri = $"ws://{uri.Host}:{uri.Port}/codenjoy-contest/ws";
                UserName = uri.Segments.LastOrDefault();
                SecretCode = uri.Query.Replace("?code=", string.Empty);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override string ToString() => $"{ServerUri}?user={UserName}&code={SecretCode}";

        public string ToUriString()
        {
            try
            {
                var uri = new Uri(ToString());

                return $"http://{uri.Host}:{uri.Port}/codenjoy-contest/board/player/{UserName}?code={SecretCode}";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return string.Empty;
            }
        }
    }
}