using CodenjoyBot.Interfaces;
using System;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Windows;
using WebSocket4Net;

namespace CodenjoyBot.DataProvider.WebSocketDataProvider
{
    [Serializable]
    public class WebSocketDataProvider : IDataProvider
    {
        public IdentityUser IdentityUser
        {
            get => _identityUser ?? (_identityUser = new IdentityUser());
            set => _identityUser = value;
        }

        private WebSocket _webSocket;
        private IdentityUser _identityUser;
        private UIElement _control;
        private UIElement _debugControl;
        private static readonly Regex Pattern = new Regex("^board=(.*)$");

        public UIElement Control => _control ?? (_control = new WebSocketDataProviderControl(this));

        public UIElement DebugControl => _debugControl ?? (_debugControl = new WebSocketDataProviderDebugControl(this));

        public WebSocketDataProvider() { }

        public WebSocketDataProvider(IdentityUser identityUser) : this()
        {
            IdentityUser = identityUser;
        }

        protected WebSocketDataProvider(SerializationInfo info, StreamingContext context) : this()
        {
            IdentityUser = info.GetValue("IdentityUser", typeof(IdentityUser)) as IdentityUser;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IdentityUser", IdentityUser);
        }

        public uint Time { get; private set; }

        public string Title => IdentityUser.ToString();

        public string Name => $"WEB [{IdentityUser?.UserName}]";

        public void Start()
        {
            if (IdentityUser.IsEmty)
            {
                return;
            }

            Time = 0;
            OnLogDataReceived(Time, $"Open {IdentityUser}");
            _webSocket = new WebSocket(IdentityUser.ToString());

            _webSocket.MessageReceived += WebSocketOnMessageReceived;

            _webSocket.Opened += (sender, args) =>
            {
                OnLogDataReceived(Time, "Opened");
                OnStarted();
            };
            _webSocket.Closed += (sender, args) =>
            {
                OnLogDataReceived(Time, "Closed");
                Stop();
                OnStopped();
            };

            _webSocket.Error += (sender, args) =>
            {
                OnLogDataReceived(Time, $"Error occurred: {args.Exception}");
                OnStopped();
            };

            _webSocket.Open();
        }

        private void WebSocketOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            DataReceived?.Invoke(this, new DataFrame { Board = ProcessMessage(e.Message), Time = Time });

            Time++;
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

            OnLogDataReceived(Time, "Stopped");
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
        protected virtual void OnLogDataReceived(uint time, string message) => LogDataReceived?.Invoke(this, new LogRecord(new DataFrame() { Time = time }, message));

        protected virtual void OnStarted() => Started?.Invoke(this, EventArgs.Empty);

        protected virtual void OnStopped() => Stopped?.Invoke(this, EventArgs.Empty);
    }
}
