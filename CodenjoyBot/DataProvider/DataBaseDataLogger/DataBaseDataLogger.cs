using System;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using CodenjoyBot.DataProvider.DataBaseModel;
using CodenjoyBot.Interfaces;

namespace CodenjoyBot.DataProvider.DataBaseDataLogger
{
    [Serializable]
    public class DataBaseDataLogger : IDataLogger
    {
        public DataBaseDataLogger() { }

        protected DataBaseDataLogger(SerializationInfo info, StreamingContext context) : this() { }

        public event EventHandler<LogRecord> LogDataReceived;
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }

        public void Log(string battleBotInstanceName, DateTime startTime, uint time, Exception e)
        {
            using (var db = new DMPContext())
            {
                var launch = db.LaunchModels.FirstOrDefault(
                    t => t.BotInstanceName == battleBotInstanceName && t.LaunchTime == startTime);

                if (launch == null)
                {
                    launch = new LaunchModel()
                    {
                        LaunchTime = startTime,
                        BotInstanceName = battleBotInstanceName
                    };

                    db.LaunchModels.Add(launch);
                }

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

        public void Log(string battleBotInstanceName, DateTime startTime, uint time, string board, string response)
        {
            using (var db = new DMPContext())
            {
                var launch = db.LaunchModels.FirstOrDefault(
                    t => t.BotInstanceName == battleBotInstanceName && t.LaunchTime == startTime);

                if (launch == null)
                {
                    launch = new LaunchModel()
                    {
                        LaunchTime = startTime,
                        BotInstanceName = battleBotInstanceName
                    };

                    db.LaunchModels.Add(launch);
                }

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

        public void LogDead(string battleBotInstanceName, DateTime startTime, uint time)
        {
            using (var db = new DMPContext())
            {
                var launch = db.LaunchModels.FirstOrDefault(
                    t => t.BotInstanceName == battleBotInstanceName && t.LaunchTime == startTime);

                if (launch == null)
                {
                    launch = new LaunchModel()
                    {
                        LaunchTime = startTime,
                        BotInstanceName = battleBotInstanceName
                    };

                    db.LaunchModels.Add(launch);
                }

                var frame = db.DataFrameModels.FirstOrDefault(t => t.LaunchModelId == launch.Id && t.Time == time);
                if (frame == null) return;

                frame.IsDead = true;

                db.SaveChanges();
            }
        }
    }

    public class DMPContext : DbContext
    {
        public DMPContext() : base("DefaultConnection") { }

        public DbSet<LaunchModel> LaunchModels { get; set; }
        public DbSet<DataFrameModel> DataFrameModels { get; set; }
        public DbSet<ExceptionModel> ExceptionModels { get; set; }
    }
}
