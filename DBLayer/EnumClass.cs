using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace DBLayer
{

    public enum CompileAs
    {
        Procedure,
        Objects
    }

    public enum LoadTypes
    {
        Append,
        Truncate
    }

    public enum DbTypes
    {
        Oracle,
        SqlServer
    }


}
