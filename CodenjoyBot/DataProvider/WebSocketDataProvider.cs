using System;
using System.Text.RegularExpressions;
using CodenjoyBot.Interfaces;
using WebSocket4Net;

namespace CodenjoyBot.DataProvider
{
    public class WebSocketDataProvider : IDataProvider
    {
        public IdentityUser IdentityUser
        {
            get => _identityUser ?? (_identityUser = new IdentityUser());
            set => _identityUser = value;
        }

        private WebSocket _webSocket;
        private IdentityUser _identityUser;
        private static readonly Regex Pattern = new Regex("^board=(.*)$");

        public event EventHandler<DataFrame> DataReceived;

        public WebSocketDataProvider() { }

        public WebSocketDataProvider(IdentityUser identityUser)
        {
            IdentityUser = identityUser;
        }

        public uint Time { get; private set; }
        public string Name => $"WEB [{IdentityUser?.UserName}]";

        public void Start()
        {
            if (IdentityUser.IsEmty) return;

            Time = 0;
            Console.WriteLine($"Open {IdentityUser}");
            _webSocket = new WebSocket(IdentityUser.ToString());

            _webSocket.MessageReceived += WebSocketOnMessageReceived;

            _webSocket.Opened += (sender, args) =>
            {
                Console.WriteLine("Opened");
            };
            _webSocket.Closed += (sender, args) =>
            {
                Console.WriteLine("Closed");
                Stop();
            };

            _webSocket.Error += (sender, args) =>
            {
                Console.WriteLine($"Error occurred: {args.Exception}");
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

            Console.WriteLine("Stoped");
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
    }
}
