using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using BotBase;
using CodenjoyBot.Interfaces;
using CodenjoyBot.Properties;

namespace CodenjoyBot.DataProvider.FileSystemDataLogger
{
    [Serializable]
    public class FileSystemDataLogger : IDataLogger, ILogger, ISerializable
    {
        private static readonly Dictionary<string, ReaderWriterLockSlim> LockerLockSlims = new Dictionary<string, ReaderWriterLockSlim>();

        public static string MainLogDir { get; } = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "Logs");

        public static IEnumerable<FileSystemLaunchInfo> GetLaunches()
        {
            var strArray1 = Directory.GetDirectories(MainLogDir);
            for (var index1 = 0; index1 < strArray1.Length; ++index1)
            {
                var nameDir = strArray1[index1];
                var strArray2 = Directory.GetDirectories(nameDir);
                for (var index2 = 0; index2 < strArray2.Length; ++index2)
                {
                    var launchDir = strArray2[index2];
                    yield return new FileSystemLaunchInfo(nameDir, launchDir);
                    launchDir = (string)null;
                }
                strArray2 = (string[])null;
                nameDir = (string)null;
            }
            strArray1 = (string[])null;
        }

        public FileSystemDataLogger()
        {
            if (Directory.Exists(MainLogDir))
                return;
            Directory.CreateDirectory(MainLogDir);
        }

        public FileSystemDataLogger(SerializationInfo info, StreamingContext context)
          : this()
        {
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        private string GetLogDirForBattleBotInstance(DateTime starTime, string battleBotInstanceName)
        {
            var str = Path.Combine(MainLogDir, battleBotInstanceName);
            if (!Directory.Exists(str))
                Directory.CreateDirectory(str);
            var path = Path.Combine(str, starTime.ToString(Resources.DateFormat));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public void Log(CodenjoyBot.CodenjoyBotInstance.CodenjoyBotInstance botInstance, DataFrame frame)
        {
            var path = Path.Combine(GetLogDirForBattleBotInstance(botInstance.StartTime, botInstance.Name), "Board.txt");
            if (!LockerLockSlims.ContainsKey(botInstance.Name))
                LockerLockSlims.Add(botInstance.Name, new ReaderWriterLockSlim());
            while (LockerLockSlims[botInstance.Name].IsWriteLockHeld);
            OnLogDataReceived(frame.Time, botInstance.Name, "write is hold");
            LockerLockSlims[botInstance.Name].EnterWriteLock();
            try
            {
                using (var streamWriter = File.AppendText(path))
                {
                    streamWriter.Write($"[{(object) frame.Time}]: {(object) frame.Board}{(object) Environment.NewLine}");
                    streamWriter.Close();
                }
                OnLogDataReceived(frame.Time, botInstance.Name, "damp saved");
            }
            catch (Exception ex)
            {
                OnLogDataReceived(frame.Time, botInstance.Name, ex.ToString());
            }
            finally
            {
                LockerLockSlims[botInstance.Name].ExitWriteLock();
            }
        }

        public void Log(CodenjoyBot.CodenjoyBotInstance.CodenjoyBotInstance botInstance, uint time, string response)
        {
            var battleBotInstance = GetLogDirForBattleBotInstance(botInstance.StartTime, botInstance.Name);
            Path.Combine(battleBotInstance, "Board.txt");
            var path = Path.Combine(battleBotInstance, "Response.txt");
            if (!LockerLockSlims.ContainsKey(botInstance.Name))
                LockerLockSlims.Add(botInstance.Name, new ReaderWriterLockSlim());
            do
                ;
            while (LockerLockSlims[botInstance.Name].IsWriteLockHeld);
            LockerLockSlims[botInstance.Name].EnterWriteLock();
            try
            {
                using (var streamWriter = File.AppendText(path))
                {
                    streamWriter.Write($"[{(object) time}]: {(object) response}{(object) Environment.NewLine}");
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                OnLogDataReceived(time, botInstance.Name, ex.ToString());
            }
            finally
            {
                LockerLockSlims[botInstance.Name].ExitWriteLock();
            }
        }

        public void Log(CodenjoyBot.CodenjoyBotInstance.CodenjoyBotInstance botInstance, DataFrame frame, Exception e)
        {
            var path = Path.Combine(GetLogDirForBattleBotInstance(botInstance.StartTime, botInstance.Name), "Exception.txt");
            if (!LockerLockSlims.ContainsKey(botInstance.Name))
                LockerLockSlims.Add(botInstance.Name, new ReaderWriterLockSlim());
            do
                ;
            while (LockerLockSlims[botInstance.Name].IsWriteLockHeld);
            OnLogDataReceived(frame.Time, botInstance.Name, "write is hold");
            LockerLockSlims[botInstance.Name].EnterWriteLock();
            try
            {
                using (var streamWriter = File.AppendText(path))
                {
                    streamWriter.Write($"[{(object) frame.Time}]: {(object) e}{(object) Environment.NewLine}");
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                OnLogDataReceived(frame.Time, botInstance.Name, ex.ToString());
            }
            finally
            {
                LockerLockSlims[botInstance.Name].ExitWriteLock();
            }
        }

        public void LogDead(CodenjoyBot.CodenjoyBotInstance.CodenjoyBotInstance botInstance, DataFrame frame)
        {
            var path = Path.Combine(GetLogDirForBattleBotInstance(botInstance.StartTime, botInstance.Name), "Dead.txt");
            if (!LockerLockSlims.ContainsKey(botInstance.Name))
                LockerLockSlims.Add(botInstance.Name, new ReaderWriterLockSlim());
            while (LockerLockSlims[botInstance.Name].IsWriteLockHeld)
                OnLogDataReceived(frame.Time, botInstance.Name, "write is hold");
            LockerLockSlims[botInstance.Name].EnterWriteLock();
            try
            {
                using (var streamWriter = File.AppendText(path))
                {
                    streamWriter.Write($"[{(object) frame.Time}]: DEAD{(object) Environment.NewLine}");
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                OnLogDataReceived(frame.Time, botInstance.Name, ex.ToString());
            }
            finally
            {
                LockerLockSlims[botInstance.Name].ExitWriteLock();
            }
        }

        public event EventHandler<LogRecord> LogDataReceived;

        protected virtual void OnLogDataReceived(
          uint time,
          string battleBotInstanceName,
          string message)
        {
            var logDataReceived = LogDataReceived;
            if (logDataReceived == null)
                return;
            logDataReceived(this, new LogRecord(new DataFrame(time, ""), battleBotInstanceName + ": " + message));
        }
    }

    public class FileSystemLaunchInfo : ILaunchInfo
    {
        public DateTime StartTime { get; }
        public string BotInstanceName { get; }
        public string BotInstanceTitle { get; }

        public string LaunchDir { get; }

        public FileSystemLaunchInfo() { }

        public FileSystemLaunchInfo(string nameDir, string launchDir) : this()
        {
            LaunchDir = launchDir;
            BotInstanceName = Path.GetFileName(nameDir);
            StartTime = DateTime.ParseExact(Path.GetFileName(launchDir), Resources.DateFormat, CultureInfo.CurrentCulture);

            BotInstanceTitle = "";
        }
    }
}
