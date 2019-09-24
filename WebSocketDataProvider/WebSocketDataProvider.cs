using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using BotBase;
using BotBase.Interfaces;
using WebSocket4Net;

namespace WebSocketDataProvider
{
    [Serializable]
    public class WebSocketDataProvider : IDataProvider
    {
        public WebSocketDataProviderSettings Settings { get; }

        private WebSocket _webSocket;
        private static readonly Regex Pattern = new Regex("^board=(.*)$");

        public WebSocketDataProvider() { }

        public WebSocketDataProvider(WebSocketDataProviderSettings settings) : this()
        {
            Settings = settings;
        }

        protected WebSocketDataProvider(SerializationInfo info, StreamingContext context) : this(
            (WebSocketDataProviderSettings) info.GetValue("Settings", typeof(WebSocketDataProviderSettings)))
        {

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) => info.AddValue("Settings", Settings);

        public uint FrameNumber { get; private set; }

        public string Title => Settings.IdentityUser?.ToString();

        public string Name => $"WEB [{Settings.IdentityUser?.UserName}]";

        public void Start()
        {
            if (Settings.IdentityUser.IsEmty)
            {
                return;
            }

            FrameNumber = 0;
            OnLogDataReceived($"Open {Settings.IdentityUser}");
            _webSocket = new WebSocket(Settings.IdentityUser.ToString());

            _webSocket.MessageReceived += WebSocketOnMessageReceived;

            _webSocket.Opened += (sender, args) =>
            {
                OnLogDataReceived("Opened");
                OnStarted();
            };
            _webSocket.Closed += (sender, args) =>
            {
                OnLogDataReceived("Closed");
                OnStopped();
            };

            _webSocket.Error += (sender, args) =>
            {
                OnLogDataReceived($"Error occurred: {args.Exception}");
                OnStopped();
            };

            _webSocket.Open();
        }

        public void Stop()
        {
            if (_webSocket == null)
            {
                return;
            }

            _webSocket.Close();
            _webSocket.Dispose();
            _webSocket = null;

            OnStopped();
            OnLogDataReceived("Stopped");
        }

        public void SendResponse(string response)
        {
            _webSocket?.Send(response);
        }

        private static string ProcessMessage(string message)
        {
            var match = Pattern.Match(message);
            if (!match.Success)
            {
                throw new ApplicationException($"Cannot match message: '{message}'");
            }

            return match.Groups[1].Value;
        }

        public event EventHandler Started;
        public event EventHandler Stopped;

        public event EventHandler<DataFrame> DataReceived;
        public event EventHandler<LogRecord> LogDataReceived;

        protected virtual void OnLogDataReceived(DataFrame frame, string message) => LogDataReceived?.Invoke(this, new LogRecord(frame, message));
        protected virtual void OnLogDataReceived(string message) => LogDataReceived?.Invoke(this, new LogRecord(message));

        private void WebSocketOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            DataReceived?.Invoke(this, new DataFrame(DateTime.Now, ProcessMessage(e.Message), FrameNumber));

            FrameNumber++;
        }

        protected virtual void OnStarted() => Started?.Invoke(this, EventArgs.Empty);

        protected virtual void OnStopped() => Stopped?.Invoke(this, EventArgs.Empty);

        protected bool Equals(WebSocketDataProvider other)
        {
            return Equals(Settings.IdentityUser, other.Settings.IdentityUser);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((WebSocketDataProvider)obj);
        }

        public override int GetHashCode()
        {
            return (Settings.IdentityUser != null ? Settings.IdentityUser.GetHashCode() : 0);
        }
    }
}
