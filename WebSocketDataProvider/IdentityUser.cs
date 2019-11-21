using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using WebSocketDataProvider.Annotations;

namespace WebSocketDataProvider
{
    [Serializable]
    public class IdentityUser : INotifyPropertyChanged, ISerializable
    {
        private string _uri;
        private string _secretCode;
        private string _userName;
        private string _serverUri;

        public string ServerUri
        {
            get => _serverUri;
            set
            {
                _serverUri = value;
                _uri = ToUriString();
                OnPropertyChanged();
                OnPropertyChanged(nameof(Uri));
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value;
                _uri = ToUriString();
                OnPropertyChanged();
                OnPropertyChanged(nameof(Uri));
            }
        }

        public string SecretCode
        {
            get => _secretCode;
            set
            {
                _secretCode = value;
                _uri = ToUriString();
                OnPropertyChanged();
                OnPropertyChanged(nameof(Uri));
            }
        }

        public string Uri
        {
            get => _uri;
            set
            {
                _uri = value;
                ParseUri(_uri);
                OnPropertyChanged();
            }
        }

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

        public IdentityUser(SerializationInfo info, StreamingContext context) : this(info.GetString(nameof(ServerUri)), info.GetString(nameof(UserName)), info.GetString(nameof(SecretCode))) { }

        //http://codenjoy.com/codenjoy-contest/board/player/j99lpu1l8skamhdzbyq9?code=7040034271572867319
        //http://localhost:8080/codenjoy-contest/board/player/nais@mail.ru?code=13476795611535248716

        public void ParseUri(string str)
        {
            try
            {
                var uri = new Uri(str);

                _serverUri = $"ws://{uri.Host}:{uri.Port}/codenjoy-contest/ws";
                _userName = uri.Segments.LastOrDefault();
                _secretCode = uri.Query.Replace("?code=", string.Empty);

                OnPropertyChanged(nameof(ServerUri));
                OnPropertyChanged(nameof(UserName));
                OnPropertyChanged(nameof(SecretCode));
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

        protected bool Equals(IdentityUser other)
        {
            return string.Equals(ServerUri, other.ServerUri) && string.Equals(UserName, other.UserName) && string.Equals(SecretCode, other.SecretCode);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((IdentityUser) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (ServerUri != null ? ServerUri.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (UserName != null ? UserName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (SecretCode != null ? SecretCode.GetHashCode() : 0);
                return hashCode;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ServerUri), ServerUri);
            info.AddValue(nameof(UserName), UserName);
            info.AddValue(nameof(SecretCode), SecretCode);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}