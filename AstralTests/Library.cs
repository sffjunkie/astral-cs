using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFFJunkie.Astral.Test
{
    public class Library
    {
        public static bool DateTimesAlmostEqual(ZonedDateTime zdt1, ZonedDateTime zdt2, int differenceAllowed = 60)
        {
            Duration d = zdt1 - zdt2;
            return Math.Abs(d.Seconds) < differenceAllowed;
        }
    }
}
