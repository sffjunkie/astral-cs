using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFFJunkie.Astral.Exceptions
{
    public class AstralException : System.Exception
    {
        public AstralException(string Message) : base(Message) { }
    }

    public class ElevationNeverReachedException : AstralException
    {
        public ElevationNeverReachedException(string Message) : base(Message) { }
    }

    public class SunAlwaysAboveElevationException : ElevationNeverReachedException
    {
        public SunAlwaysAboveElevationException(string Message) : base(Message) { }
    }

    public class SunAlwaysBelowElevationException : ElevationNeverReachedException
    {
        public SunAlwaysBelowElevationException(string Message) : base(Message) { }
    }
}
