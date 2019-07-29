﻿using System;
using System.Runtime.Serialization;
using BotBase;
using BotBase.Interfaces;

namespace DataBaseDataProvider
{
    [Serializable]
    public class DataBaseDataLogger : IDataLogger
    {
        public DataBaseDataLogger() { }
        protected DataBaseDataLogger(SerializationInfo info, StreamingContext context) : this() { }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }

//        public void Log(CodenjoyBotInstance.CodenjoyBotInstance botInstance, DataFrame frame, Exception e)
//        {
//            using (var db = new CodenjoyDbContext())
//            {
//                var launch = db.LaunchModels.Find(botInstance.LaunchId);
//                if (launch == null)
//                    throw new Exception("Launch not found");
//
//                var frameModel = launch.Frames.FirstOrDefault(t => t.Time == frame.Time);
//                if (frameModel == null)
//                {
//                    frameModel = new DataFrameModel()
//                    {
//                        Time = frame.Time,
//                        Board = frame.Board,
//                        LaunchModel = launch
//                    };
//
//                    db.DataFrameModels.Add(frameModel);
//                }
//
//                var exception = new ExceptionModel()
//                {
//                    Message = e.Message,
//                    StackTrace = e.StackTrace,
//                    Launch = launch,
//                    Frame = frameModel
//                };
//
//                db.ExceptionModels.Add(exception);
//
//                db.SaveChanges();
//            }
//        }
//
//        public void Log(CodenjoyBotInstance.CodenjoyBotInstance botInstance, DataFrame frame)
//        {
//            throw new NotImplementedException();
//        }
//
//        public void Log(CodenjoyBotInstance.CodenjoyBotInstance botInstance, uint time, string response)
//        {
//            throw new NotImplementedException();
//        }
//
//        public void Log(CodenjoyBotInstance.CodenjoyBotInstance botInstance, DataFrame frame, string response)
//        {
//            using (var db = new CodenjoyDbContext())
//            {
//                var launch = db.LaunchModels.Find(botInstance.LaunchId);
//                if (launch == null)
//                    throw new Exception("Launch not found");
//
//                var frameModel = new DataFrameModel()
//                {
//                    Time = frame.Time,
//                    Board = frame.Board,
//                    Response = response,
//
//                    LaunchModel = launch
//                };
//
//                db.DataFrameModels.Add(frameModel);
//
//                launch.Frames.Add(frameModel);
//
//                db.SaveChanges();
//            }
//        }
//
//        public void LogDead(CodenjoyBotInstance.CodenjoyBotInstance botInstance, DataFrame frame)
//        {
//            using (var db = new CodenjoyDbContext())
//            {
//                var launch = db.LaunchModels.Find(botInstance.LaunchId);
//                if (launch == null)
//                    throw new Exception("Launch not found");
//
//                var frameModel = db.DataFrameModels.FirstOrDefault(t => t.LaunchModelId == launch.Id && t.Time == frame.Time);
//                if (frameModel == null) return;
//
//                frameModel.IsDead = true;
//
//                db.SaveChanges();
//            }
//        }

        public event EventHandler<LogRecord> LogDataReceived;
        public void Log(string name, DateTime startTime, DataFrame frame, Exception e)
        {
            throw new NotImplementedException();
        }

        public void Log(string name, DateTime startTime, DataFrame frame)
        {
            throw new NotImplementedException();
        }

        public void Log(string name, DateTime startTime, uint time, string response)
        {
            throw new NotImplementedException();
        }

        public void LogDead(string name, DateTime startTime, DataFrame frame)
        {
            throw new NotImplementedException();
        }
    }
}
