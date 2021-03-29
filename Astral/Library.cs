///  Copyright 2009 - 2021, Simon Kennedy, sffjunkie+code@gmail.com

///  Licensed under the Apache License, Version 2.0 (the "License");
///  you may not use this file except in compliance with the License.
///  You may obtain a copy of the License at

///      http://www.apache.org/licenses/LICENSE-2.0

///  Unless required by applicable law or agreed to in writing, software
///  distributed under the License is distributed on an "AS IS" BASIS,
///  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
///  See the License for the specific language governing permissions and
///  limitations under the License.

using NodaTime;
using System;
using System.Text.RegularExpressions;

namespace SFFJunkie.Astral
{
    public static class Library
    {
        /// <summary>
        /// Using 32 arc minutes as sun's apparent diameter
        /// </summary>
        public const double SunApparentRadius = 32.0 / (60.0 * 2.0);

        /// <summary>
        /// The current date/time in the timezone specified
        /// </summary>
        /// <param name="TimezoneName">The name of the timezone to return the curtrent time in.</param>
        /// <returns>The current date/time in the timezone specified</returns>
        public static ZonedDateTime Now(string TimezoneName = "UTC")
        {
            var i = new Instant();
            var tz = GetTimezone(TimezoneName);
            var dt = new ZonedDateTime(i, tz);
            return dt;
        }

        /// <summary>
        /// Todays date.
        /// </summary>
        /// <param name="TimezoneName"></param>
        /// <returns></returns>
        public static LocalDate Today(string TimezoneName)
        {
            return Now(TimezoneName).Date;
        }

        public static double DMSToDouble(string DMS, double Limit)
        {
            if (Double.TryParse(DMS, out double result))
            {
                return result;
            }

            var _dms_re = @"(?<deg>\d{1,3})[°]((?<min>\d{1,2})[′'])?((?<sec>\d{1,2})[″""])?(?<dir>[NSEW])?";
            var matches = Regex.Matches(DMS, _dms_re, RegexOptions.IgnoreCase);
            if (matches != null)
            {
                var match = matches[0];
                var groups = match.Groups;

                if (!String.IsNullOrWhiteSpace(groups["deg"].Value))
                {
                    result = Double.Parse(groups["deg"].Value);
                }

                if (!String.IsNullOrWhiteSpace(groups["min"].Value))
                {
                    result += Double.Parse(groups["min"].Value) / 60.0;
                }

                if (!String.IsNullOrWhiteSpace(groups["sec"].Value))
                {
                    result += Double.Parse(groups["sec"].Value) / 3600.0;
                }

                string dir;
                if (!String.IsNullOrWhiteSpace(groups["dir"].Value))
                {
                    dir = groups["dir"].Value;
                }
                else
                {
                    dir = "E";
                }

                if (dir.ToUpper() == "S" || dir.ToUpper() == "W")
                {
                    result = -result;
                }

                return Clamp(result, Limit);
            }
            else
            {
                throw new Exception("Unable to convert degrees/minutes/seconds to float");
            }
        }

        /// <summary>
        /// Clamp a value between ±limit
        /// </summary>
        /// <param name="Value">Value to clamp</param>
        /// <param name="Limit">Limit</param>
        /// <returns>The value clamped between limit</returns>
        public static double Clamp(double Value, double Limit)
        {
            if (Value > Limit)
            {
                Value = Limit;
            }
            else if (Value < -Limit)
            {
                Value = -Limit;
            }
            return Value;
        }

        /// <summary>
        /// The depression angle in degrees for the dawn/dusk calculations
        /// </summary>
        public struct Depression
        {
            public const double Civil = 6.0;
            public const double Nautical = 12.0;
            public const double Astronomical = 18.0;
        }

        /// <summary>
        /// Direction of the sun either RISING or SETTING
        /// </summary>
        public enum SunDirection
        {
            Rising,
            Setting
        }

        public static double JulianDay(LocalDate Date)
        {
            var y = Date.Year;
            var m = Date.Month;
            var d = Date.Day;

            if (m <= 2)
            {
                y -= 1;
                m += 12;
            }

            var a = Math.Floor(y / 100.0);
            var b = 2 - a + Math.Floor(a / 4.0);
            var jd = Math.Floor(365.25 * (y + 4716)) + Math.Floor(30.6001 * (m + 1)) + d + b - 1524.5;
            return jd;
        }

        /// <summary>
        /// Convert from degrees to radians.
        /// </summary>
        /// <param name="Degrees">The number of degrees to convert</param>
        /// <returns>The value in radians</returns>
        public static double Radians(double Degrees)
        {
            return Degrees * Math.PI / 180.0;
        }

        /// <summary>
        /// Convert from radians to degrees
        /// </summary>
        /// <param name="Radians">The number of radians to convert</param>
        /// <returns>The value in degrees</returns>
        public static double Degrees(double Radians)
        {
            return Radians * 180.0 / Math.PI;
        }

        /// <summary>
        /// Modulo that always returns a positive value
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="Divisor"></param>
        /// <returns>The positive modulus</returns>
        public static double Modulo(double Value, double Divisor)
        {
            double result = Value % Divisor;
            if (result < 0.0)
            {
                result += Divisor;
            }
            return result;
        }

        public static double ProperAngle(double Value)
        {
            return Modulo(Value, 360.0);
        }

        /// <summary>
        /// Get a DateTimeZone for the timezone name specified.
        /// 
        /// Looks in both the Tzdb and Bcl providers
        /// </summary>
        /// <param name="Timezone">The timezone name to return</param>
        /// <returns>The DateTimeZone requested</returns>
        public static DateTimeZone GetTimezone(string Timezone)
        {
            if (Timezone == "UTC")
            {
                return DateTimeZone.Utc;
            }

            DateTimeZone tz = DateTimeZoneProviders.Tzdb.GetZoneOrNull(Timezone);
            if (tz == null)
            {
                tz = DateTimeZoneProviders.Bcl.GetZoneOrNull(Timezone);
            }
            return tz;
        }
    }
}
