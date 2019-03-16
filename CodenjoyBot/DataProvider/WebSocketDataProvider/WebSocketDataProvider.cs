using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Windows;
using CodenjoyBot.Interfaces;
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

        public event EventHandler<DataFrame> DataReceived;

        public WebSocketDataProvider() { }

        public WebSocketDataProvider(IdentityUser identityUser)
        {
            IdentityUser = identityUser;
        }

        protected WebSocketDataProvider(SerializationInfo info, StreamingContext context)
        {
            IdentityUser = info.GetValue("IdentityUser", typeof(IdentityUser)) as IdentityUser;
        }

        public uint Time { get; private set; }
        public string Name => $"WEB [{IdentityUser?.UserName}]";

        public void Start()
        {
            if (IdentityUser.IsEmty) return;

            Time = 0;
            OnLogDataReceived(Time, $"Open {IdentityUser}");
            _webSocket = new WebSocket(IdentityUser.ToString());

            _webSocket.MessageReceived += WebSocketOnMessageReceived;

            _webSocket.Opened += (sender, args) =>
            {
                OnLogDataReceived(Time, "Opened");
            };
            _webSocket.Closed += (sender, args) =>
            {
                OnLogDataReceived(Time, "Closed");
                Stop();
            };

            _webSocket.Error += (sender, args) =>
            {
                OnLogDataReceived(Time, $"Error occurred: {args.Exception}");
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
            if (_webSocket == null) return;

            _webSocket.Close();
            _webSocket.Dispose();
            _webSocket = null;

            OnLogDataReceived(Time, "Stoped");
        }

        public void SendResponse(string response) => _webSocket?.Send(response);

        private static string ProcessMessage(string message)
        {
            var match = Pattern.Match(message);
            if (!match.Success)
            {
                throw new ApplicationException($"Cannot match message: '{message}'");
            }

            return match.Groups[1].Value;
        }

        public event EventHandler<LogRecord> LogDataReceived;
        protected virtual void OnLogDataReceived(uint time, string message) => LogDataReceived?.Invoke(this, new LogRecord(new DataFrame() { Time = time }, message));

        public UIElement Control => _control ?? (_control = new WebSocketDataProviderControl(this));

        public UIElement DebugControl => _debugControl ?? (_debugControl = new WebSocketDataProviderDebugControl(this));

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("IdentityUser", IdentityUser);
        }
    }
}
