using System;
using System.IO;

namespace BotBase
{
    public static class AppData
    {
        public static void Set() => SetRelative(@"..\..\App_Data\");
        public static void SetRelative(string path) => SetAbsolute(Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path)));
        public static void SetAbsolute(string path) => AppDomain.CurrentDomain.SetData("DataDirectory", path);
    }
}
