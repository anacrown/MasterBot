using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using BotBase.BotInstance;
using BotBase.Interfaces;

namespace FileSystemDataProvider
{
    [Serializable]
    public class FileSystemDataLogger : IDataLogger
    {
        private static readonly Dictionary<string, ReaderWriterLockSlim> LockerLockSlims = new Dictionary<string, ReaderWriterLockSlim>();

        public static string MainLogDir { get; set; } = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "Logs");
        public static string DataFormat { get; set; } = "yyyy.MM.dd hh.mm.ss";

        public static IEnumerable<FileSystemLaunchInfo> GetLaunches() =>
            from nameDir in Directory.GetDirectories(MainLogDir)
            from subDir in Directory.GetDirectories(nameDir)
            let launchDir = subDir
            select new FileSystemLaunchInfo(nameDir, launchDir);

        public FileSystemDataLogger()
        {
            if (!Directory.Exists(MainLogDir))
                Directory.CreateDirectory(MainLogDir);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) { }

        private string GetLogDirForBattleBotInstance(DateTime starTime, string battleBotInstanceName)
        {
            var str = Path.Combine(MainLogDir, battleBotInstanceName);
            if (!Directory.Exists(str))
                Directory.CreateDirectory(str);
            var path = Path.Combine(str, starTime.ToString("yyyy.MM.dd hh.mm.ss"));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public void Log(string name, DateTime startTime, DataFrame frame)
        {
            var path = Path.Combine(GetLogDirForBattleBotInstance(startTime, name), "Board.txt");
            if (!LockerLockSlims.ContainsKey(name))
                LockerLockSlims.Add(name, new ReaderWriterLockSlim());
            while (LockerLockSlims[name].IsWriteLockHeld) ;
            OnLogDataReceived(frame.Time, name, "write is hold");
            LockerLockSlims[name].EnterWriteLock();
            try
            {
                using (var streamWriter = File.AppendText(path))
                {
                    streamWriter.Write($"[{(object)frame.Time}]: {(object)frame.Board}{(object)Environment.NewLine}");
                    streamWriter.Close();
                }
                OnLogDataReceived(frame.Time, name, "damp saved");
            }
            catch (Exception ex)
            {
                OnLogDataReceived(frame.Time, name, ex.ToString());
            }
            finally
            {
                LockerLockSlims[name].ExitWriteLock();
            }
        }

        public void Log(string name, DateTime startTime, uint time, string response)
        {
            var battleBotInstance = GetLogDirForBattleBotInstance(startTime, name);
            Path.Combine(battleBotInstance, "Board.txt");
            var path = Path.Combine(battleBotInstance, "Response.txt");
            if (!LockerLockSlims.ContainsKey(name))
                LockerLockSlims.Add(name, new ReaderWriterLockSlim());
            do
                ;
            while (LockerLockSlims[name].IsWriteLockHeld);
            LockerLockSlims[name].EnterWriteLock();
            try
            {
                using (var streamWriter = File.AppendText(path))
                {
                    streamWriter.Write($"[{(object)time}]: {(object)response}{(object)Environment.NewLine}");
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                OnLogDataReceived(time, name, ex.ToString());
            }
            finally
            {
                LockerLockSlims[name].ExitWriteLock();
            }
        }

        public void Log(string name, DateTime startTime, DataFrame frame, Exception e)
        {
            var path = Path.Combine(GetLogDirForBattleBotInstance(startTime, name), "Exception.txt");
            if (!LockerLockSlims.ContainsKey(name))
                LockerLockSlims.Add(name, new ReaderWriterLockSlim());
            do
                ;
            while (LockerLockSlims[name].IsWriteLockHeld);
            OnLogDataReceived(frame.Time, name, "write is hold");
            LockerLockSlims[name].EnterWriteLock();
            try
            {
                using (var streamWriter = File.AppendText(path))
                {
                    streamWriter.Write($"[{(object)frame.Time}]: {(object)e}{(object)Environment.NewLine}");
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                OnLogDataReceived(frame.Time, name, ex.ToString());
            }
            finally
            {
                LockerLockSlims[name].ExitWriteLock();
            }
        }

        public void LogDead(string name, DateTime startTime, DataFrame frame)
        {
            var path = Path.Combine(GetLogDirForBattleBotInstance(startTime, name), "Dead.txt");
            if (!LockerLockSlims.ContainsKey(name))
                LockerLockSlims.Add(name, new ReaderWriterLockSlim());
            while (LockerLockSlims[name].IsWriteLockHeld)
                OnLogDataReceived(frame.Time, name, "write is hold");
            LockerLockSlims[name].EnterWriteLock();
            try
            {
                using (var streamWriter = File.AppendText(path))
                {
                    streamWriter.Write($"[{(object)frame.Time}]: DEAD{(object)Environment.NewLine}");
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                OnLogDataReceived(frame.Time, name, ex.ToString());
            }
            finally
            {
                LockerLockSlims[name].ExitWriteLock();
            }
        }

        public event EventHandler<LogRecord> LogDataReceived;

        protected virtual void OnLogDataReceived(uint time,string battleBotInstanceName,string message)
        {
            var logDataReceived = LogDataReceived;
            if (logDataReceived == null)
                return;
            logDataReceived(this, new LogRecord(new DataFrame(time, ""), battleBotInstanceName + ": " + message));
        }
    }
}