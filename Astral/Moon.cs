using NodaTime;
using System;

namespace SFFJunkie.Astral
{
    public static class Moon
    {
        /// <summary>
        /// Calculates the phase of the moon on the current date.
        /// </summary>
        /// <returns>The phase of the moon as a double
        /// 
        /// ============  ==============
        /// 0 .. 6.99     New moon
        /// 7 .. 13.99    First quarter
        /// 14 .. 20.99   Full moon
        /// 21 .. 27.99   Last quarter
        /// ============  ==============
        /// </returns>
        public static double Phase()
        {
            DateTime dt = DateTime.Today;
            LocalDate localDate = new(dt.Year, dt.Month, dt.Day);
            return _phase(localDate);
        }

        /// <summary>
        /// Calculates the phase of the moon on the specified date.
        /// </summary>
        /// <returns>The phase of the moon as a double
        /// 
        /// ============  ==============
        /// 0 .. 6.99     New moon
        /// 7 .. 13.99    First quarter
        /// 14 .. 20.99   Full moon
        /// 21 .. 27.99   Last quarter
        /// ============  ==============
        /// </returns>
        public static double Phase(LocalDate LocalDate)
        {
            return _phase(LocalDate);
        }

        private static double _phase(LocalDate Date)
        {
            var jd = Library.JulianDay(Date);
            var DT = Math.Pow((jd - 2382148.0), 2) / (41048480.0 * 86400.0);
            var T = (jd + DT - 2451545.0) / 36525.0;
            var T2 = Math.Pow(T, 2);
            var T3 = Math.Pow(T, 3);
            var D = 297.85 + (445267.1115 * T) - (0.0016300 * T2) + (T3 / 545868.0);
            D = Library.Radians(Library.ProperAngle(D));
            var M = 357.53 + (35999.0503 * T);
            M = Library.Radians(Library.ProperAngle(M));
            var M1 = 134.96 + (477198.8676 * T) + (0.0089970 * T2) + (T3 / 69699.0);
            M1 = Library.Radians(Library.ProperAngle(M1));
            var elong = Library.Degrees(D) + 6.29 * Math.Sin(M1);
            elong -= 2.10 * Math.Sin(M);
            elong += 1.27 * Math.Sin(2 * D - M1);
            elong += 0.66 * Math.Sin(2 * D);
            elong = Library.ProperAngle(elong);
            elong = (int)elong;
            var phase = ((elong + 6.43) / 360.0) * 28.0;
            phase %= 28.0;
            return phase;
        }
    }
}
