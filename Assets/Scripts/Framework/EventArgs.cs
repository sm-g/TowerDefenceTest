using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

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
