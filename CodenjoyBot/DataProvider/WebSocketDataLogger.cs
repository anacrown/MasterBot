using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace CodenjoyBot.DataProvider
{
    public class WebSocketDataLogger
    {
        private static WebSocketDataLogger _instance;
        public static WebSocketDataLogger Instance => _instance ?? (_instance = new WebSocketDataLogger());

        public string MainLogDir { get; } = Path.GetFullPath("./Logs");

        private readonly Dictionary<string, ReaderWriterLockSlim> _lockerLockSlims =
            new Dictionary<string, ReaderWriterLockSlim>();

        private WebSocketDataLogger()
        {
            if (!Directory.Exists(MainLogDir))
                Directory.CreateDirectory(MainLogDir);
        }

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
                //MainWindow.Log(time, $"{battleBotInstanceName} write is hold");

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
                //MainWindow.Log(time, ex.ToString());
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
                //MainWindow.Log(time, $"{battleBotInstanceName} write is hold");

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

                //MainWindow.Log(time, $"{battleBotInstanceName} damp saved");
            }
            catch (Exception ex)
            {
                //MainWindow.Log(time, ex.ToString());
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
                //MainWindow.Log(time, $"{battleBotInstanceName} write is hold");

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
                //MainWindow.Log(time, ex.ToString());
            }
            finally
            {
                _lockerLockSlims[battleBotInstanceName].ExitWriteLock();
            }
        }
    }
}
