using System;
using System.Runtime.Serialization;
using BotBase;
using BotBase.Board;
using BotBase.Interfaces;
using SpaceRaceStrategy.AISolver;

namespace SpaceRaceStrategy
{
    public class SpaceRaceSolver : ISolver
    {
        public SpaceRaceSolver(SpaceRaceSolverSettings settings)
        {
            Settings = settings;
        }

        public SpaceRaceSolver(SerializationInfo info, StreamingContext context) : this(info.GetValue("Settings", typeof(SpaceRaceSolverSettings)) as SpaceRaceSolverSettings)
        {

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Settings", Settings);
        }

        public SpaceRaceSolverSettings Settings { get; set; }

        public event EventHandler<LogRecord> LogDataReceived;
        public event EventHandler<Board> BoardChanged; 

        public void Initialize()
        {
            
        }

        public bool Answer(string instanceName, DateTime startTime, DataFrame frame, out string response)
        {
            response = "";

            var board = new Board(instanceName, startTime, frame);
            BoardChanged?.Invoke(this, board);

            return false;
        }
    }
}
