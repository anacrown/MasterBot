using System;
using System.Runtime.Serialization;
using BotBase.Interfaces;

namespace BotBase
{
    [Serializable]
    public abstract class SolverSettingsBase : SettingsBase
    {
        protected SolverSettingsBase() : base()
        {
        }

        protected SolverSettingsBase(SerializationInfo info, StreamingContext context) : base(info, context)
        {

        }

        public abstract ISolver CreateSolver();
    }
}