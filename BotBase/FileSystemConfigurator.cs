using System;
using System.IO;

namespace BotBase
{
    public  static class FileSystemConfigurator
    {
        public static string AppDataDir { get; } = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
        public static string MainLogDir { get; } = Path.Combine(AppDataDir, "Logs");
    }
}
