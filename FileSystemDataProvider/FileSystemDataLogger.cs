using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using BotBase;
using BotBase.Interfaces;

namespace FileSystemDataProvider
{
    [Serializable]
    public class FileSystemDataLogger : IDataLogger
    {
        private static readonly Dictionary<string, ReaderWriterLockSlim> LockerLockSlims = new Dictionary<string, ReaderWriterLockSlim>();

        public FileSystemDataLoggerSettings Settings { get; set; }

        public IEnumerable<FileSystemLaunchInfo> GetLaunches() =>
            from nameDir in Directory.GetDirectories(Settings.MainLogDir)
            from subDir in Directory.GetDirectories(nameDir)
            let launchDir = subDir
            select new FileSystemLaunchInfo(nameDir, launchDir, Settings.DataFormat);

        public FileSystemDataLogger(FileSystemDataLoggerSettings settings)
        {
            Settings = settings;

            if (!Directory.Exists(Settings.MainLogDir))
                Directory.CreateDirectory(Settings.MainLogDir);
        }

        public FileSystemDataLogger(SerializationInfo info, StreamingContext context) : this(info.GetValue("Settings", typeof(FileSystemDataLoggerSettings)) as FileSystemDataLoggerSettings) { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Settings", Settings);
        }

        private string GetLogDirForBattleBotInstance(DateTime starTime, string battleBotInstanceName)
        {
            var str = Path.Combine(Settings.MainLogDir, battleBotInstanceName);
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
            OnLogDataReceived(frame.FrameNumber, name, "write is hold");
            LockerLockSlims[name].EnterWriteLock();
            try
            {
                using (var streamWriter = File.AppendText(path))
                {
                    streamWriter.Write($"[{frame.Time.ToString(Settings.DataFormat)}] ({frame.FrameNumber}): {frame.Board}{Environment.NewLine}");
                    streamWriter.Close();
                }
                OnLogDataReceived(frame.FrameNumber, name, "damp saved");
            }
            catch (Exception ex)
            {
                OnLogDataReceived(frame.FrameNumber, name, ex.ToString());
            }
            finally
            {
                LockerLockSlims[name].ExitWriteLock();
            }
        }

        public void Log(string name, DateTime startTime, DateTime time, uint frameNumber, string response)
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
                    streamWriter.Write($"[{time.ToString(Settings.DataFormat)}] ({frameNumber}): {response}{Environment.NewLine}");
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                OnLogDataReceived(frameNumber, name, ex.ToString());
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
            OnLogDataReceived(frame.FrameNumber, name, "write is hold");
            LockerLockSlims[name].EnterWriteLock();
            try
            {
                using (var streamWriter = File.AppendText(path))
                {
                    streamWriter.Write($"[{frame.Time.ToString(Settings.DataFormat)}] ({frame.FrameNumber}): {e}{Environment.NewLine}");
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                OnLogDataReceived(frame.FrameNumber, name, ex.ToString());
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
                OnLogDataReceived(frame.FrameNumber, name, "write is hold");
            LockerLockSlims[name].EnterWriteLock();
            try
            {
                using (var streamWriter = File.AppendText(path))
                {
                    streamWriter.Write($"[{(object)frame.FrameNumber}]: DEAD{(object)Environment.NewLine}");
                    streamWriter.Close();
                }
            }
            catch (Exception ex)
            {
                OnLogDataReceived(frame.FrameNumber, name, ex.ToString());
            }
            finally
            {
                LockerLockSlims[name].ExitWriteLock();
            }
        }

        public event EventHandler<LogRecord> LogDataReceived;

        protected virtual void OnLogDataReceived(uint frameNumber, string battleBotInstanceName, string message) => LogDataReceived?.Invoke(this, new LogRecord(new DataFrame(DateTime.Now, "", frameNumber), battleBotInstanceName + ": " + message));
    }
}