using System;
using System.Linq;
using System.Runtime.Serialization;
using CodenjoyBot.DataProvider.DataBaseModel;
using CodenjoyBot.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public void Log(CodenjoyBotInstance.CodenjoyBotInstance botInstance, uint time, Exception e)
        {
            using (var db = new CodenjoyDbContext())
            {
                var launch = db.LaunchModels.FirstOrDefault(
                    t => t.BotInstanceName == botInstance.Name && t.LaunchTime == botInstance.StartTime);

                if (launch == null)
                {
                    launch = new LaunchModel()
                    {
                        LaunchTime = botInstance.StartTime,
                        BotInstanceName = botInstance.Name,
                        BotInstanceTitle = botInstance.Title
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

        public void Log(CodenjoyBotInstance.CodenjoyBotInstance botInstance, uint time, string board, string response)
        {
            using (var db = new CodenjoyDbContext())
            {
                var launch = db.LaunchModels.FirstOrDefault(
                    t => t.BotInstanceName == botInstance.Name && t.LaunchTime == botInstance.StartTime);

                if (launch == null)
                {
                    launch = new LaunchModel()
                    {
                        LaunchTime = botInstance.StartTime,
                        BotInstanceName = botInstance.Name,
                        BotInstanceTitle = botInstance.Title
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

        public void LogDead(CodenjoyBotInstance.CodenjoyBotInstance botInstance, uint time)
        {
            using (var db = new CodenjoyDbContext())
            {
                var launch = db.LaunchModels.FirstOrDefault(
                    t => t.BotInstanceName == botInstance.Name && t.LaunchTime == botInstance.StartTime);

                if (launch == null)
                {
                    launch = new LaunchModel()
                    {
                        LaunchTime = botInstance.StartTime,
                        BotInstanceName = botInstance.Name,
                        BotInstanceTitle = botInstance.Title
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

    public sealed class CodenjoyDbContext : DbContext
    {
        public CodenjoyDbContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<LaunchModel> LaunchModels { get; set; }
        public DbSet<DataFrameModel> DataFrameModels { get; set; }
        public DbSet<ExceptionModel> ExceptionModels { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = DMPStore.db");
        }
    }
}
