using System;
using CodenjoyBot;
using CodenjoyBot.CodenjoyBotInstance;
using PaperIO_MiniCupsAI;

namespace PaperIoRunner
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            AppData.SetRelative(@"..\..\..\Debugger\App_Data");

            var solver = new PaperIoSolver();
            var dataProvider = new DataProvider();
            var botInstance = new CodenjoyBotInstance(dataProvider, solver);

            botInstance.Start();
        }
    }
}
