using System;
using System.Runtime.Serialization;
using BotBase;
using BotBase.Interfaces;

namespace LoderunnerStategy.AISolver
{
    [Serializable]
    public class LoderunnerSolverSettings : SolverSettingsBase
    {
        public LoderunnerSolverSettings() : base()
        {
        }

        public LoderunnerSolverSettings(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public override ISolver CreateSolver() => new LoderunnerSolver(this);
    }
}