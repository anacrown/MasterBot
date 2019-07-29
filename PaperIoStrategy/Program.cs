using System;
using BotBase;
using FileSystemDataProvider;

namespace PaperIoStrategy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AppData.SetRelative("../../../Debugger/App_Data");

            //System.Diagnostics.Debugger.Launch();

            var startTime = DateTime.Now;
            var solver = new PaperIoSolver(new PaperIoSolverSettings());
            var dataLogger = new FileSystemDataLogger(new FileSystemDataLoggerSettings());
            var dataProvider = new DataProvider();

            dataProvider.DataReceived += (sender, frame) =>
            {
                dataLogger.Log("PaperIoStrategy", startTime, frame);

                try
                {
                    if (solver.Answer("paperIoStrategy", startTime, frame, out var response))
                    {
                        dataProvider.SendResponse(response);
                        dataLogger.Log("PaperIoStrategy", startTime, DateTime.Now, frame.FrameNumber, response);
                    }
                }
                catch (Exception e)
                {
                    dataLogger.Log("PaperIoStrategy", startTime, frame, e);
                }
            };

            dataProvider.Start();

        }
    }
}
