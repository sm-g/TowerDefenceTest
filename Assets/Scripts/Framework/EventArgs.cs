using System;
using System.Diagnostics;
using System.Linq;

namespace Assets.Scripts
{
    [Serializable]
    public class EventArgs<T> : EventArgs
    {
        public readonly T arg;

        [DebuggerStepThrough]
        public EventArgs(T arg)
        {
            this.arg = arg;
        }
    }
}