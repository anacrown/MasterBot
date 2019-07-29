using System;
using System.Runtime.Serialization;
using BotBase;
using BotBase.Interfaces;

namespace PaperIoStrategy
{
    [Serializable]
    public class PaperIoSolverSettings : SolverSettingsBase
    {
        public PaperIoSolverSettings() : base()
        {
        }

        public PaperIoSolverSettings(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public override ISolver CreateSolver() => new PaperIoSolver(this);
    }
}