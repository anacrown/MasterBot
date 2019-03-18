using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using CodenjoyBot.Interfaces;

namespace CodenjoyBot.DataProvider.FileSystemDataLogger
{
    [Serializable]
    public class FileSystemDataLogger : IDataLogger
    {
        public static string MainLogDir { get; } = Path.GetFullPath("./Logs");

        private static readonly Dictionary<string, ReaderWriterLockSlim> _lockerLockSlims =
            new Dictionary<string, ReaderWriterLockSlim>();

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

            var dir = Path.Combine(botInstanceDir, starTime.ToString("yyyy.MM.dd hh.mm.ss"));

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            return dir;
        }

        public void Log(string battleBotInstanceName, DateTime startTime, uint time, Exception e)
        {
            var dir = GetLogDirForBattleBotInstance(startTime, battleBotInstanceName);
            var exceptionFile = Path.Combine(dir, "Exception.txt");

            if (!_lockerLockSlims.ContainsKey(battleBotInstanceName))
                _lockerLockSlims.Add(battleBotInstanceName, new ReaderWriterLockSlim());

            while (_lockerLockSlims[battleBotInstanceName].IsWriteLockHeld) ;
                OnLogDataReceived(time, battleBotInstanceName, "write is hold");

            _lockerLockSlims[battleBotInstanceName].EnterWriteLock();

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
                OnLogDataReceived(time, battleBotInstanceName, ex.ToString());
            }
            finally
            {
                _lockerLockSlims[battleBotInstanceName].ExitWriteLock();
            }
        }

        public void Log(string battleBotInstanceName, DateTime startTime, uint time, string board, string response)
        {
            var dir = GetLogDirForBattleBotInstance(startTime, battleBotInstanceName);
            var boardFile = Path.Combine(dir, "Board.txt");
            var responseFile = Path.Combine(dir, "Response.txt");

            if (!_lockerLockSlims.ContainsKey(battleBotInstanceName))
                _lockerLockSlims.Add(battleBotInstanceName, new ReaderWriterLockSlim());

            while (_lockerLockSlims[battleBotInstanceName].IsWriteLockHeld) ;
                OnLogDataReceived(time, battleBotInstanceName, "write is hold");

            _lockerLockSlims[battleBotInstanceName].EnterWriteLock();

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

                OnLogDataReceived(time, battleBotInstanceName, "damp saved");
            }
            catch (Exception ex)
            {
                OnLogDataReceived(time, battleBotInstanceName, ex.ToString());
            }
            finally
            {
                _lockerLockSlims[battleBotInstanceName].ExitWriteLock();
            }
        }

        public void LogDead(string battleBotInstanceName, DateTime startTime, uint time)
        {
            var dir = GetLogDirForBattleBotInstance(startTime, battleBotInstanceName);
            var deadFile = Path.Combine(dir, "Dead.txt");

            if (!_lockerLockSlims.ContainsKey(battleBotInstanceName))
                _lockerLockSlims.Add(battleBotInstanceName, new ReaderWriterLockSlim());

            while (_lockerLockSlims[battleBotInstanceName].IsWriteLockHeld)
                OnLogDataReceived(time, battleBotInstanceName, "write is hold");

                _lockerLockSlims[battleBotInstanceName].EnterWriteLock();

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
                OnLogDataReceived(time, battleBotInstanceName, ex.ToString());
            }
            finally
            {
                _lockerLockSlims[battleBotInstanceName].ExitWriteLock();
            }
        }

        public event EventHandler<LogRecord> LogDataReceived;
        protected virtual void OnLogDataReceived(uint time, string battleBotInstanceName, string message) => 
            LogDataReceived?.Invoke(this, new LogRecord(new DataFrame(){Time = time}, $"{battleBotInstanceName}: {message}"));
    }
}
