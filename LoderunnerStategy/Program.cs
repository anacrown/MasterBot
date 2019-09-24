using System;
using BotBase;
using FileSystemDataProvider;
using LoderunnerStategy.AISolver;

namespace LoderunnerStategy
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            AppData.SetRelative("../../../Debugger/App_Data");
#endif

            //System.Diagnostics.Debugger.Launch();

            var startTime = DateTime.Now;
            var solver = new LoderunnerSolver(new LoderunnerSolverSettings());
#if DEBUG
            var dataLogger = new FileSystemDataLogger(new FileSystemDataLoggerSettings());
#endif
            var dataProvider = new WebSocketDataProvider.WebSocketDataProvider();

            dataProvider.DataReceived += (sender, frame) =>
            {
#if DEBUG
                dataLogger.Log("PaperIoStrategy", startTime, frame);
#endif

                try
                {
                    if (solver.Answer("paperIoStrategy", startTime, frame, out var response))
                    {
                        dataProvider.SendResponse(response);
#if DEBUG
                        dataLogger.Log("PaperIoStrategy", startTime, DateTime.Now, frame.FrameNumber, response);
#endif
                    }
                }
                catch (Exception e)
                {
#if DEBUG
                    dataLogger.Log("PaperIoStrategy", startTime, frame, e);
#endif
                }
            };

            dataProvider.Start();
        }
    }
}
