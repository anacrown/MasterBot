using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using BotBase;
using BotBase.Interfaces;

namespace SpaceRaceStrategy
{
    [Serializable]
    public class SpaceRaceSolverSettings : SolverSettingsBase
    {
        public SpaceRaceSolverSettings() : base()
        {
        }

        public SpaceRaceSolverSettings(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        public override ISolver CreateSolver() => new SpaceRaceSolver(this);
    }
}
