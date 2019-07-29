using System;

namespace BotBase.Interfaces
{
    public interface ILogger
    {
        event EventHandler<LogRecord> LogDataReceived;
    }
}
