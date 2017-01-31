using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DBLayer
{
    public abstract class Params
    {
        // Methods
        protected Params()
        { }
        public abstract void Add(object Param);

        // Properties
        public abstract object this[int index] { get; }
        public abstract object this[string ParamName] { get; }

    }
}
