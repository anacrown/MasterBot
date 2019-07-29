using System;
using System.Globalization;
using System.IO;

namespace FileSystemDataProvider
{
    public class FileSystemLaunchInfo
    {
        public DateTime StartTime { get; }
        public string BotInstanceName { get; }
        public string BotInstanceTitle { get; }

        public string LaunchDir { get; }

        public FileSystemLaunchInfo() { }

        public FileSystemLaunchInfo(string nameDir, string launchDir, string dateFormat) : this()
        {
            LaunchDir = launchDir;
            BotInstanceName = Path.GetFileName(nameDir);
            StartTime = DateTime.ParseExact(Path.GetFileName(launchDir), dateFormat, CultureInfo.CurrentCulture);

            BotInstanceTitle = "";
        }
    }
}