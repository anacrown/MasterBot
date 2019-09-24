﻿using System;
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
        public IdentityUser IdentityUser
        {
            get => _identityUser ?? (_identityUser = new IdentityUser());
            set => _identityUser = value;
        }

        private WebSocket _webSocket;
        private IdentityUser _identityUser;
        private static readonly Regex Pattern = new Regex("^board=(.*)$");

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

        public uint FrameNumber { get; private set; }

        public string Title => IdentityUser.ToString();

        public string Name => $"WEB [{IdentityUser?.UserName}]";

        public void Start()
        {
            if (IdentityUser.IsEmty)
            {
                return;
            }

            FrameNumber = 0;
            OnLogDataReceived($"Open {IdentityUser}");
            _webSocket = new WebSocket(IdentityUser.ToString());

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
            return Equals(_identityUser, other._identityUser);
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
            return (_identityUser != null ? _identityUser.GetHashCode() : 0);
        }
    }
}
