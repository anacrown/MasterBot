using System;
using System.Runtime.Serialization;
using BotBase;
using BotBase.Interfaces;

namespace LoderunnerStategy.AISolver
{
    public class LoderunnerSolver : ISolver
    {
        public LoderunnerSolverSettings Settings { get; }

        public LoderunnerSolver(LoderunnerSolverSettings settings)
        {
            Settings = settings;
        }

        public LoderunnerSolver(SerializationInfo info, StreamingContext context) : this(info.GetValue("Settings", typeof(LoderunnerSolverSettings)) as LoderunnerSolverSettings)
        {

        }

        public event EventHandler<LogRecord> LogDataReceived;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Settings", Settings);
        }

        public void Initialize() { }

        public bool Answer(string instanceName, DateTime startTime, DataFrame frame, out string response)
        {
            response = string.Empty;

            return true;
        }
    }
}
