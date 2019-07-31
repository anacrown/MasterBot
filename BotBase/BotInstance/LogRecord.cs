namespace BotBase
{
    public class LogRecord
    {
        public string Message { get; }
        public DataFrame DataFrame { get; }

        public LogRecord(string message)
        {
            Message = message;
        }

        public LogRecord(DataFrame dataFrame, string message) : this(message)
        {
            DataFrame = dataFrame;
        }

        public override string ToString() => $"[{DataFrame.FrameNumber}]: {Message}";
    }
}