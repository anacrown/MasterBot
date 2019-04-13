using System;
using System.Linq;
using System.Runtime.Serialization;
using CodenjoyBot.Entity;
using CodenjoyBot.Interfaces;

namespace CodenjoyBot.DataProvider.DataBaseDataLogger
{
    [Serializable]
    public class DataBaseDataLogger : IDataLogger
    {
        public DataBaseDataLogger() { }
        protected DataBaseDataLogger(SerializationInfo info, StreamingContext context) : this() { }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }

        public void Log(CodenjoyBotInstance.CodenjoyBotInstance botInstance, uint time, Exception e)
        {
            using (var db = new CodenjoyDbContext())
            {
                var launch = db.LaunchModels.Find(botInstance.LaunchId);
                if (launch == null)
                    throw new Exception("Launch not found");

                var exception = new ExceptionModel()
                {
                    Message = e.Message,
                    StackTrace = e.StackTrace,

                    LaunchModel = launch
                };

                db.ExceptionModels.Add(exception);

                launch.Exceptions.Add(exception);

                db.SaveChanges();
            }
        }

        public void Log(CodenjoyBotInstance.CodenjoyBotInstance botInstance, uint time, string board, string response)
        {
            using (var db = new CodenjoyDbContext())
            {
                var launch = db.LaunchModels.Find(botInstance.LaunchId);
                if (launch == null)
                    throw new Exception("Launch not found");

                var frame = new DataFrameModel()
                {
                    Time = time,
                    Board = board,
                    Response = response,

                    LaunchModel = launch
                };

                db.DataFrameModels.Add(frame);

                launch.Frames.Add(frame);

                db.SaveChanges();
            }
        }

        public void LogDead(CodenjoyBotInstance.CodenjoyBotInstance botInstance, uint time)
        {
            using (var db = new CodenjoyDbContext())
            {
                var launch = db.LaunchModels.Find(botInstance.LaunchId);
                if (launch == null)
                    throw new Exception("Launch not found");

                var frame = db.DataFrameModels.FirstOrDefault(t => t.LaunchModelId == launch.Id && t.Time == time);
                if (frame == null) return;

                frame.IsDead = true;

                db.SaveChanges();
            }
        }

        public event EventHandler<LogRecord> LogDataReceived;
        protected virtual void OnLogDataReceived(DataFrame frame, string message) => LogDataReceived?.Invoke(this, new LogRecord(frame, message));
    }
}
