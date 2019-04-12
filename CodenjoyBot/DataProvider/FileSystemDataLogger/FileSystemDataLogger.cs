using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using CodenjoyBot.Interfaces;
using CodenjoyBot.Properties;

namespace CodenjoyBot.DataProvider.FileSystemDataLogger
{
    [Serializable]
    public class FileSystemDataLogger : IDataLogger
    {
        public static string MainLogDir { get; } = Path.Combine(AppDomain.CurrentDomain.GetData("DataDirectory").ToString(), "Logs");

        private static readonly Dictionary<string, ReaderWriterLockSlim> LockerLockSlims = new Dictionary<string, ReaderWriterLockSlim>();

        public static IEnumerable<FileSystemLaunchInfo> GetLaunches()
        {
            foreach (var nameDir in Directory.GetDirectories(MainLogDir))
            {
                foreach (var launchDir in Directory.GetDirectories(nameDir))
                {
                    yield return new FileSystemLaunchInfo(nameDir, launchDir);
                }
            }
        }

        public FileSystemDataLogger()
        {
            if (!Directory.Exists(MainLogDir))
                Directory.CreateDirectory(MainLogDir);
        }

        public FileSystemDataLogger(SerializationInfo info, StreamingContext context) : this() { }

        public void GetObjectData(SerializationInfo info, StreamingContext context) { }

        private string GetLogDirForBattleBotInstance(DateTime starTime, string battleBotInstanceName)
        {
            var botInstanceDir = Path.Combine(MainLogDir, battleBotInstanceName);

            if (!Directory.Exists(botInstanceDir))
                Directory.CreateDirectory(botInstanceDir);

            var dir = Path.Combine(botInstanceDir, starTime.ToString(Resources.DateFormat));

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }

        public void Log(CodenjoyBotInstance.CodenjoyBotInstance botInstance, uint time, Exception e)
        {
            var dir = GetLogDirForBattleBotInstance(botInstance.StartTime, botInstance.Name);
            var exceptionFile = Path.Combine(dir, "Exception.txt");

            if (!LockerLockSlims.ContainsKey(botInstance.Name))
                LockerLockSlims.Add(botInstance.Name, new ReaderWriterLockSlim());

            while (LockerLockSlims[botInstance.Name].IsWriteLockHeld) ;
            OnLogDataReceived(time, botInstance.Name, "write is hold");

            LockerLockSlims[botInstance.Name].EnterWriteLock();

            try
            {
                using (var exceptionWriter = File.AppendText(exceptionFile))
                {
                    exceptionWriter.Write($"[{time}]: {e}{Environment.NewLine}");
                    exceptionWriter.Close();
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

        public void Log(CodenjoyBotInstance.CodenjoyBotInstance botInstance, uint time, string board, string response)
        {
            var dir = GetLogDirForBattleBotInstance(botInstance.StartTime, botInstance.Name);
            var boardFile = Path.Combine(dir, "Board.txt");
            var responseFile = Path.Combine(dir, "Response.txt");

            if (!LockerLockSlims.ContainsKey(botInstance.Name))
                LockerLockSlims.Add(botInstance.Name, new ReaderWriterLockSlim());

            while (LockerLockSlims[botInstance.Name].IsWriteLockHeld) ;
            OnLogDataReceived(time, botInstance.Name, "write is hold");

            LockerLockSlims[botInstance.Name].EnterWriteLock();

            try
            {
                using (var boardWriter = File.AppendText(boardFile))
                {
                    boardWriter.Write($"[{time}]: {board}{Environment.NewLine}");
                    boardWriter.Close();
                }

                using (var responseWriter = File.AppendText(responseFile))
                {
                    responseWriter.Write($"[{time}]: {response}{Environment.NewLine}");
                    responseWriter.Close();
                }

                OnLogDataReceived(time, botInstance.Name, "damp saved");
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

        public void LogDead(CodenjoyBotInstance.CodenjoyBotInstance botInstance, uint time)
        {
            var dir = GetLogDirForBattleBotInstance(botInstance.StartTime, botInstance.Name);
            var deadFile = Path.Combine(dir, "Dead.txt");

            if (!LockerLockSlims.ContainsKey(botInstance.Name))
                LockerLockSlims.Add(botInstance.Name, new ReaderWriterLockSlim());

            while (LockerLockSlims[botInstance.Name].IsWriteLockHeld)
                OnLogDataReceived(time, botInstance.Name, "write is hold");

            LockerLockSlims[botInstance.Name].EnterWriteLock();

            try
            {
                using (var deadWriter = File.AppendText(deadFile))
                {
                    deadWriter.Write($"[{time}]: DEAD{Environment.NewLine}");
                    deadWriter.Close();
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

        public event EventHandler<LogRecord> LogDataReceived;
        protected virtual void OnLogDataReceived(uint time, string battleBotInstanceName, string message) =>
            LogDataReceived?.Invoke(this, new LogRecord(new DataFrame() { Time = time }, $"{battleBotInstanceName}: {message}"));
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
