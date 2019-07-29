using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystemDataProvider
{
    public  static class FileSystemConfigurator
    {
        public static string AppDataDir { get; } = AppDomain.CurrentDomain.GetData("DataDirectory").ToString();
        public static string MainLogDir { get; } = Path.Combine(AppDataDir, "Logs");
    }
}
