using System;
using BotBase;
using FileSystemDataProvider;
using WebSocketDataProvider;

namespace SpaceRaceStrategy
{
    internal class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            AppData.SetRelative("../../../Debugger/App_Data");
#endif

            //System.Diagnostics.Debugger.Launch();

            var startTime = DateTime.Now;
            var solver = new SpaceRaceSolver(new SpaceRaceSolverSettings());
#if DEBUG
            var dataLogger = new FileSystemDataLogger(new FileSystemDataLoggerSettings());
#endif
            var dataProvider = new WebSocketDataProvider.WebSocketDataProvider(new WebSocketDataProviderSettings(
                new IdentityUser("http://localhost:8080/another-context/board/player/nais@mail.ru?code=13476795611535248716")));

            dataProvider.DataReceived += (sender, frame) =>
            {
#if DEBUG
                dataLogger.Log("SpaceRaceStrategy", startTime, frame);
#endif

                try
                {
                    if (solver.Answer("SpaceRaceStrategy", startTime, frame, out var response))
                    {
                        dataProvider.SendResponse(response);
#if DEBUG
                        dataLogger.Log("SpaceRaceStrategy", startTime, DateTime.Now, frame.FrameNumber, response);
#endif
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    dataLogger.Log("SpaceRaceStrategy", startTime, frame, e);
#endif
                }
            };

            dataProvider.Start();

        }
    }
}
