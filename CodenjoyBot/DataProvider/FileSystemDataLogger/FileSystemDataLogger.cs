using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
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
            string[] strArray1 = Directory.GetDirectories(CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.MainLogDir);
            for (int index1 = 0; index1 < strArray1.Length; ++index1)
            {
                string nameDir = strArray1[index1];
                string[] strArray2 = Directory.GetDirectories(nameDir);
                for (int index2 = 0; index2 < strArray2.Length; ++index2)
                {
                    string launchDir = strArray2[index2];
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
            if (Directory.Exists(CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.MainLogDir))
                return;
            Directory.CreateDirectory(CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.MainLogDir);
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
            string str = Path.Combine(CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.MainLogDir, battleBotInstanceName);
            if (!Directory.Exists(str))
                Directory.CreateDirectory(str);
            string path = Path.Combine(str, starTime.ToString(Resources.DateFormat));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        public void Log(CodenjoyBot.CodenjoyBotInstance.CodenjoyBotInstance botInstance, DataFrame frame)
        {
            string path = Path.Combine(this.GetLogDirForBattleBotInstance(botInstance.StartTime, botInstance.Name), "Board.txt");
            if (!CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims.ContainsKey(botInstance.Name))
                CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims.Add(botInstance.Name, new ReaderWriterLockSlim());
            do
                ;
            while (CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims[botInstance.Name].IsWriteLockHeld);
            this.OnLogDataReceived(frame.Time, botInstance.Name, "write is hold");
            CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims[botInstance.Name].EnterWriteLock();
            try
            {
                using (StreamWriter streamWriter = File.AppendText(path))
                {
                    streamWriter.Write(string.Format("[{0}]: {1}{2}", (object)frame.Time, (object)frame.Board, (object)Environment.NewLine));
                    streamWriter.Close();
                }
                this.OnLogDataReceived(frame.Time, botInstance.Name, "damp saved");
            }
            catch (Exception ex)
            {
                this.OnLogDataReceived(frame.Time, botInstance.Name, ex.ToString());
            }
            finally
            {
                CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims[botInstance.Name].ExitWriteLock();
            }
        }

        public void Log(CodenjoyBot.CodenjoyBotInstance.CodenjoyBotInstance botInstance, uint time, string response)
        {
            string battleBotInstance = this.GetLogDirForBattleBotInstance(botInstance.StartTime, botInstance.Name);
            Path.Combine(battleBotInstance, "Board.txt");
            string path = Path.Combine(battleBotInstance, "Response.txt");
            if (!CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims.ContainsKey(botInstance.Name))
                CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims.Add(botInstance.Name, new ReaderWriterLockSlim());
            do
                ;
            while (CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims[botInstance.Name].IsWriteLockHeld);
            CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims[botInstance.Name].EnterWriteLock();
            try
            {
                using (StreamWriter streamWriter = File.AppendText(path))
                {
                    streamWriter.Write(string.Format("[{0}]: {1}{2}", (object)time, (object)response, (object)Environment.NewLine));
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                this.OnLogDataReceived(time, botInstance.Name, ex.ToString());
            }
            finally
            {
                CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims[botInstance.Name].ExitWriteLock();
            }
        }

        public void Log(CodenjoyBot.CodenjoyBotInstance.CodenjoyBotInstance botInstance, DataFrame frame, Exception e)
        {
            string path = Path.Combine(this.GetLogDirForBattleBotInstance(botInstance.StartTime, botInstance.Name), "Exception.txt");
            if (!CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims.ContainsKey(botInstance.Name))
                CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims.Add(botInstance.Name, new ReaderWriterLockSlim());
            do
                ;
            while (CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims[botInstance.Name].IsWriteLockHeld);
            this.OnLogDataReceived(frame.Time, botInstance.Name, "write is hold");
            CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims[botInstance.Name].EnterWriteLock();
            try
            {
                using (StreamWriter streamWriter = File.AppendText(path))
                {
                    streamWriter.Write(string.Format("[{0}]: {1}{2}", (object)frame.Time, (object)e, (object)Environment.NewLine));
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                this.OnLogDataReceived(frame.Time, botInstance.Name, ex.ToString());
            }
            finally
            {
                CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims[botInstance.Name].ExitWriteLock();
            }
        }

        public void LogDead(CodenjoyBot.CodenjoyBotInstance.CodenjoyBotInstance botInstance, DataFrame frame)
        {
            string path = Path.Combine(this.GetLogDirForBattleBotInstance(botInstance.StartTime, botInstance.Name), "Dead.txt");
            if (!CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims.ContainsKey(botInstance.Name))
                CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims.Add(botInstance.Name, new ReaderWriterLockSlim());
            while (CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims[botInstance.Name].IsWriteLockHeld)
                this.OnLogDataReceived(frame.Time, botInstance.Name, "write is hold");
            CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims[botInstance.Name].EnterWriteLock();
            try
            {
                using (StreamWriter streamWriter = File.AppendText(path))
                {
                    streamWriter.Write(string.Format("[{0}]: DEAD{1}", (object)frame.Time, (object)Environment.NewLine));
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                this.OnLogDataReceived(frame.Time, botInstance.Name, ex.ToString());
            }
            finally
            {
                CodenjoyBot.DataProvider.FileSystemDataLogger.FileSystemDataLogger.LockerLockSlims[botInstance.Name].ExitWriteLock();
            }
        }

        public event EventHandler<LogRecord> LogDataReceived;

        protected virtual void OnLogDataReceived(
          uint time,
          string battleBotInstanceName,
          string message)
        {
            EventHandler<LogRecord> logDataReceived = this.LogDataReceived;
            if (logDataReceived == null)
                return;
            logDataReceived((object)this, new LogRecord(new DataFrame()
            {
                Time = time
            }, battleBotInstanceName + ": " + message));
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
