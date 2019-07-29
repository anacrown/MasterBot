using System;
using System.IO;
using System.Runtime.Serialization;
using BotBase;
using BotBase.Interfaces;

namespace FileSystemDataProvider
{
    [Serializable]
    public class FileSystemDataLoggerSettings : DataLoggerSettingsBase
    {
        public string MainLogDir { get; set; } = FileSystemConfigurator.MainLogDir;
        public string DataFormat { get; set; } = "yyyy.MM.dd hh.mm.ss";

        public FileSystemDataLoggerSettings() : base()
        {
        }

        protected FileSystemDataLoggerSettings(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            MainLogDir = info.GetString("MainLogDir");
            DataFormat = info.GetString("DataFormat");
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("MainLogDir", MainLogDir);
            info.AddValue("DataFormat", DataFormat);
        }

        public override IDataLogger CreateDataLogger() => new FileSystemDataLogger(this);

    }
}