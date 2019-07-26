namespace BotBase.BotInstance
{
    public class LogRecord
    {
        public string Message { get; }
        public DataFrame? DataFrame { get; }

        public LogRecord(string message)
        {
            Message = message;
        }

        public LogRecord(DataFrame dataFrame, string message) : this(message)
        {
            DataFrame = dataFrame;
        }

        public override string ToString()
        {

            return DataFrame.HasValue ? $"[{DataFrame.Value.Time}]: {Message}" : Message;
        }
    }
}