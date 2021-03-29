using System;
using NodaTime;

/// <summary>
/// This class contains public static methods that should be considered private methods
/// i.e. they are not part of the public interface to Astral
/// 
/// This is used so that I can test the methods.
/// </summary>
namespace SFFJunkie.Astral
{
    public class Private
    {
        /// <summary>
        /// Convert a Julian Day number to a Julian Century
        /// </summary>
        /// <param name="JulianDay"></param>
        public static double JulianDayToJulianCentury(double JulianDay)
        {
            return (JulianDay - 2451545.0) / 36525.0;
        }

        /// <summary>
        /// Convert a Julian Century number to a Julian Day
        /// </summary>
        /// <param name="JulianCentury"></param>
        /// <returns></returns>
        public static double JulianCenturyToJulianDay(double JulianCentury)
        {
            return (JulianCentury * 36525.0) + 2451545.0;
        }

        /// <summary>
        /// Calculate the geometric mean longitude of the sun
        /// </summary>
        /// <param name="JulianCentury"></param>
        /// <returns></returns>
        public static double GeometricMeanLongitudeSun(double JulianCentury)
        {
            var l0 = 280.46646 + JulianCentury * (36000.76983 + 0.0003032 * JulianCentury);
            return Library.ProperAngle(l0);
        }

        /// <summary>
        /// Calculate the geometric mean anomaly of the sun
        /// </summary>
        /// <param name="JulianCentury"></param>
        /// <returns></returns>
        public static double GeometricMeanAnomalySun(double JulianCentury)
        {
            return 357.52911 + JulianCentury * (35999.05029 - 0.0001537 * JulianCentury);
        }

        /// <summary>
        /// Calculate the eccentricity of Earth's orbit
        /// </summary>
        /// <param name="JulianCentury"></param>
        /// <returns></returns>
        public static double EccentricLocationEarthOrbit(double JulianCentury)
        {
            return 0.016708634 - JulianCentury * (0.000042037 + 0.0000001267 * JulianCentury);
        }

        /// <summary>
        /// Calculate the equation of the center of the sun
        /// </summary>
        /// <param name="JulianCentury"></param>
        /// <returns></returns>
        public static double SunEquationOfCenter(double JulianCentury)
        {
            var m = GeometricMeanAnomalySun(JulianCentury);

            var mrad = Library.Radians(m);
            var sinm = Math.Sin(mrad);
            var sin2m = Math.Sin(mrad + mrad);
            var sin3m = Math.Sin(mrad + mrad + mrad);

            var c = (
                sinm * (1.914602 - JulianCentury * (0.004817 + 0.000014 * JulianCentury))
                + sin2m * (0.019993 - 0.000101 * JulianCentury)
                + sin3m * 0.000289
            );

            return c;
        }

        /// <summary>
        /// Calculate the sun's true longitude
        /// </summary>
        /// <param name="JulianCentury"></param>
        /// <returns></returns>
        public static double SunTrueLongitude(double JulianCentury)
        {
            var l0 = GeometricMeanLongitudeSun(JulianCentury);
            var c = SunEquationOfCenter(JulianCentury);
            return l0 + c;
        }

        /// <summary>
        /// Calculate the sun's true anomaly
        /// </summary>
        /// <param name="JulianCentury"></param>
        /// <returns></returns>
        public static double SunTrueAnomoly(double JulianCentury)
        {
            var m = GeometricMeanAnomalySun(JulianCentury);
            var c = SunEquationOfCenter(JulianCentury);
            return m + c;
        }

        public static double SunRadVector(double JulianCentury)
        {
            var v = SunTrueAnomoly(JulianCentury);
            var e = EccentricLocationEarthOrbit(JulianCentury);
            return (1.000001018 * (1 - e * e)) / (1 + e * Math.Cos(Library.Radians(v)));
        }

        public static double SunApparentLongitude(double JulianCentury)
        {
            var true_long = SunTrueLongitude(JulianCentury);

            var omega = 125.04 - 1934.136 * JulianCentury;
            return true_long - 0.00569 - 0.00478 * Math.Sin(Library.Radians(omega));
        }

        public static double MeanObliquityOfEcliptic(double JulianCentury)
        {
            var seconds = 21.448 - JulianCentury * (
                46.815 + JulianCentury * (0.00059 - JulianCentury * (0.001813))
            );
            return 23.0 + (26.0 + (seconds / 60.0)) / 60.0;
        }

        public static double ObliquityCorrection(double JulianCentury)
        {
            var e0 = MeanObliquityOfEcliptic(JulianCentury);

            var omega = 125.04 - 1934.136 * JulianCentury;
            return e0 + 0.00256 * Math.Cos(Library.Radians(omega));
        }

        /// <summary>
        /// Calculate the sun's right ascension
        /// </summary>
        /// <param name="JulianCentury"></param>
        /// <returns></returns>
        public static double SunRightAscension(double JulianCentury)
        {
            var oc = ObliquityCorrection(JulianCentury);
            var al = SunApparentLongitude(JulianCentury);

            var tananum = Math.Cos(Library.Radians(oc)) * Math.Sin(Library.Radians(al));
            var tanadenom = Math.Cos(Library.Radians(al));

            return Library.Degrees(Math.Atan2(tananum, tanadenom));
        }

        /// <summary>
        /// Calculate the sun's declination
        /// </summary>
        /// <param name="JulianCentury"></param>
        /// <returns></returns>
        public static double SunDeclination(double JulianCentury)
        {
            var e = ObliquityCorrection(JulianCentury);
            var lambd = SunApparentLongitude(JulianCentury);

            var sint = Math.Sin(Library.Radians(e)) * Math.Sin(Library.Radians(lambd));
            return Library.Degrees(Math.Asin(sint));
        }

        public static double VarY(double JulianCentury)
        {
            var epsilon = ObliquityCorrection(JulianCentury);
            var y = Math.Tan(Library.Radians(epsilon) / 2.0);
            return y * y;
        }

        public static double EquationOfTime(double JulianCentury)
        {
            var l0 = GeometricMeanLongitudeSun(JulianCentury);
            var e = EccentricLocationEarthOrbit(JulianCentury);
            var m = GeometricMeanAnomalySun(JulianCentury);

            var y = VarY(JulianCentury);

            var sin2l0 = Math.Sin(2.0 * Library.Radians(l0));
            var sinm = Math.Sin(Library.Radians(m));
            var cos2l0 = Math.Cos(2.0 * Library.Radians(l0));
            var sin4l0 = Math.Sin(4.0 * Library.Radians(l0));
            var sin2m = Math.Sin(2.0 * Library.Radians(m));

            var Etime = (
                y * sin2l0
                - 2.0 * e * sinm
                + 4.0 * e * y * sinm * cos2l0
                - 0.5 * y * y * sin4l0
                - 1.25 * e * e * sin2m
            );

            return Library.Degrees(Etime) * 4.0;
        }

        /// <summary>
        /// Calculate the hour angle of the sun
        ///
        /// See https://en.wikipedia.org/wiki/Hour_angle#Solar_hour_angle
        /// </summary>
        /// <param name="Latitude">The latitude of the obersver</param>
        /// <param name="Declination">The declination of the sun</param>
        /// <param name="Zenith">The zenith angle of the sun</param>
        /// <param name="Direction">The direction of traversal of the sun</param>
        /// <returns></returns>
        public static Double HourAngle(double Latitude, double Declination, double Zenith, Library.SunDirection Direction)
        {
            var latitude_rad = Library.Radians(Latitude);
            var declination_rad = Library.Radians(Declination);
            var zenith_rad = Library.Radians(Zenith);

            var h = (Math.Cos(zenith_rad) - Math.Sin(latitude_rad) * Math.Sin(declination_rad)) / (
                Math.Cos(latitude_rad) * Math.Cos(declination_rad));

            var HA = Math.Acos(h);

            if (Double.IsNaN(HA))
            {
                return Double.NaN;
            }

            if (Direction == Library.SunDirection.Setting)
            {
                HA = -HA;
            }
            return HA;
        }

        /// <summary>
        /// Calculate the extra degrees of depression that you can see round the earth due to the increase in elevation.
        /// </summary>
        /// <param name="Elevation">Elevation above the earth in metres</param>
        /// <returns>A number of degrees to add to adjust for the elevation of the observer</returns>
        public static double AdjustToHorizon(double Elevation)
        {
            if (Elevation <= 0)
            {
                return 0.0;
            }

            var r = 6356900.0;  // radius of the earth
            var a1 = r;
            var h1 = r + Elevation;
            var theta1 = Math.Acos(a1 / h1);
            return Library.Degrees(theta1);
        }

        /// <summary>
        /// Calculate the number of degrees to adjust for an obscuring feature
        /// </summary>
        /// <param name="Elevation"></param>
        /// <param name="DistanceTo"></param>
        /// <returns></returns>
        public static double AdjustToObscuringFeature(double Elevation, double DistanceTo)
        {
            if (Elevation == 0.0)
            {
                return 0.0;
            }

            var sign = Elevation < 0.0 ? -1 : 1;
            return sign * Library.Degrees(
                Math.Acos(Math.Abs(Elevation) / Math.Sqrt(Math.Pow(Elevation, 2) + Math.Pow(DistanceTo, 2))));
        }

        /// <summary>
        /// Calculate the degrees of refraction of the sun due to the sun's elevation.
        /// </summary>
        /// <param name="Zenith"></param>
        /// <returns></returns>
        public static double RefractionAtZenith(double Zenith)
        {
            var elevation = 90 - Zenith;
            if (elevation >= 85.0)
            {
                return 0;
            }

            double refractionCorrection;
            var te = Math.Tan(Library.Radians(elevation));
            if (elevation > 5.0)
            {
                refractionCorrection = (
                    58.1 / te - 0.07 / (te * te * te) + 0.000086 / (te * te * te * te * te)
                );
            }
            else if (elevation > -0.575)
            {
                var step1 = -12.79 + elevation * 0.711;
                var step2 = 103.4 + elevation * step1;
                var step3 = -518.2 + elevation * step2;
                refractionCorrection = 1735.0 + elevation * step3;
            }
            else
            {
                refractionCorrection = -20.774 / te;
            }

            refractionCorrection /= 3600.0;
            return refractionCorrection;
        }

        public static Tuple<double, double> ZenithAndAzimuth(
            Observer Observer,
            ZonedDateTime DateAndTime,
            bool WithRefraction = true)
        {
            double latitude;
            if (Observer.Latitude > 89.8)
            {
                latitude = 89.8;
            }
            else if (Observer.Latitude < -89.8)
            {
                latitude = -89.8;
            }
            else
            {
                latitude = Observer.Latitude;
            }

            double longitude = Observer.Longitude;

            double zone = -DateAndTime.Offset.ToTimeSpan().TotalHours;
            ZonedDateTime utcDateTime = DateAndTime.WithZone(DateTimeZone.Utc);

            var timenow = utcDateTime.Hour + (utcDateTime.Minute / 60.0) + (utcDateTime.Second / 3600.0);
            var jd = Library.JulianDay(utcDateTime.Date);
            var jc = JulianDayToJulianCentury(jd + timenow / 24.0);
            var solarDec = SunDeclination(jc);
            var eqTime = EquationOfTime(jc);
            var solarTimeFix = eqTime - (4.0 * -longitude) + (60 * zone);
            var trueSolarTime = (
                DateAndTime.Hour * 60.0
                + DateAndTime.Minute
                + DateAndTime.Second / 60.0
                + solarTimeFix
            );

            while (trueSolarTime > 1440)
            {
                trueSolarTime -= 1440;
            }

            var hourangle = trueSolarTime / 4.0 - 180.0;
            if (hourangle < -180)
            {
                hourangle += 360.0;
            }

            var harad = Library.Radians(hourangle);

            var csz = Math.Sin(Library.Radians(latitude)) * Math.Sin(Library.Radians(solarDec)) + Math.Cos(
                Library.Radians(latitude)) * Math.Cos(Library.Radians(solarDec)) * Math.Cos(harad);

            csz = Library.Clamp(csz, 1.0);

            double zenith = Library.Degrees(Math.Acos(csz));

            var azDenom = Math.Cos(Library.Radians(latitude)) * Math.Sin(Library.Radians(zenith));

            double azimuth;
            if (Math.Abs(azDenom) > 0.001)
            {
                var azRad = (
                    (Math.Sin(Library.Radians(latitude)) * Math.Cos(Library.Radians(zenith))) - Math.Sin(Library.Radians(solarDec))) / azDenom;

                azRad = Library.Clamp(azRad, 1.0);
                azimuth = 180.0 - Library.Degrees(Math.Acos(azRad));

                if (hourangle > 0.0)
                {
                    azimuth = -azimuth;
                }
            }
            else
            {
                if (latitude > 0.0)
                {
                    azimuth = 180.0;
                }
                else
                {
                    azimuth = 0.0;
                }
            }

            if (azimuth < 0.0)
            {
                azimuth += 360.0;
            }

            if (WithRefraction)
            {
                zenith -= RefractionAtZenith(zenith);
            }

            return new(zenith, azimuth);
        }
    }
}
