using SFFJunkie.Astral.Exceptions;
using NodaTime;
using System;

#nullable enable

namespace SFFJunkie.Astral
{
    public static class Sun
    {
        /// <summary>
        /// Calculate the time in the UTC timezone when the sun transits the specificed zenith
        /// </summary>
        /// <param name="Observer">An observer viewing the sun at a specific, latitude, longitude and elevation</param>
        /// <param name="Date">The date to calculate for</param>
        /// <param name="Zenith">The zenith angle for which to calculate the transit time</param>
        /// <param name="Direction">The direction that the sun is traversing</param>
        /// <returns>The time when the sun transits the specificed zenith</returns>
        /// <throws>ValueError if the zenith is not transitted by the sun</throws>
        public static ZonedDateTime TimeAtZenith(
            Observer Observer,
            LocalDate Date,
            double Zenith,
            Library.SunDirection Direction)
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

            double adjustment_for_elevation = 0.0;
            if (Observer.DistanceToFeature == 0.0 && Observer.Elevation > 0.0)
            {
                adjustment_for_elevation = Private.AdjustToHorizon(Observer.Elevation);
            }
            else if (Observer.DistanceToFeature > 0.0)
            {
                adjustment_for_elevation = Private.AdjustToObscuringFeature(Observer.Elevation, Observer.DistanceToFeature);
            }

            var adjustment_for_refraction = Private.RefractionAtZenith(Zenith + adjustment_for_elevation);

            var jd = Library.JulianDay(Date);
            var t = Private.JulianDayToJulianCentury(jd);
            var solarDec = Private.SunDeclination(t);

            var hourangle = Private.HourAngle(
                latitude,
                solarDec,
                Zenith + adjustment_for_elevation - adjustment_for_refraction,
                Direction
            );

            if (Double.IsNaN(hourangle))
            {
                throw new Exception($"Sun never reaches an zenith of {Zenith} degrees at this location.");
            }

            var delta = -Observer.Longitude - Library.Degrees(hourangle);
            var timeDiff = 4.0 * delta;
            var timeUTC = 720.0 + timeDiff - Private.EquationOfTime(t);

            t = Private.JulianDayToJulianCentury(Private.JulianCenturyToJulianDay(t) + timeUTC / 1440.0);
            solarDec = Private.SunDeclination(t);
            hourangle = Private.HourAngle(
                latitude,
                solarDec,
                Zenith + adjustment_for_elevation + adjustment_for_refraction,
                Direction
            );

            if (Double.IsNaN(hourangle))
            {
                throw new Exception($"Sun never reaches an zenith of {Zenith} degrees at this location.");
            }

            delta = -Observer.Longitude - Library.Degrees(hourangle);
            timeDiff = 4.0 * delta;
            timeUTC = 720 + timeDiff - Private.EquationOfTime(t);

            Duration offset = Duration.FromMinutes(timeUTC);
            Instant i = Instant.FromUtc(Date.Year, Date.Month, Date.Day, offset.Hours, offset.Minutes, offset.Seconds);
            return new ZonedDateTime(i, DateTimeZone.Utc);
        }

        /// <summary>
        /// Calculates the time when the sun is at the specified elevation on the specified date.
        /// 
        /// Note:
        /// This method uses positive elevations for those above the horizon.
        /// 
        /// Elevations greater than 90 degrees are converted to a setting sun
        /// i.e.an elevation of 110 will calculate a setting sun at 70 degrees.
        /// </summary>
        /// <param name="Observer">Observer to calculate for</param>
        /// <param name="Elevation">Elevation of the sun in degrees above the horizon to calculate for.</param>
        /// <param name="Date">Date to calculate for. Default is today's date in the timezone `tzinfo`.</param>
        /// <param name="Direction">Determines whether the calculated time is for the sun rising or setting.
        /// Use ``SunDirection.RISING`` or ``SunDirection.SETTING``. Default is rising.</param>
        /// <param name="Timezone">Timezone to return times in. Default is UTC.</param>
        /// <returns>Date and time at which the sun is at the specified elevation.</returns>
        public static ZonedDateTime TimeAtElevation(
            Observer Observer,
            double Elevation,
            LocalDate? Date,
            Library.SunDirection Direction = Library.SunDirection.Rising,
            string Timezone = "UTC")
        {
            if (Elevation > 90.0)
            {
                Elevation = 180.0 - Elevation;
                Direction = Library.SunDirection.Setting;
            }

            LocalDate ld;
            if (Date == null)
            {
                ld = Library.Today(Timezone);
            }
            else
            {
                ld = (LocalDate)Date;
            }

            var zenith = 90 - Elevation;
            ZonedDateTime transitTime = TimeAtZenith(Observer, ld, zenith, Direction);
            return transitTime;
        }

        /// <summary>
        /// Calculate solar noon time when the sun is at its highest point.
        /// </summary>
        /// <param name="Observer">An observer viewing the sun at a specific, latitude, longitude and elevation</param>
        /// <param name="Date">Date to calculate for. Default is today for the specified tzinfo.</param>
        /// <param name="Timezone">Timezone to return times in. Default is UTC.</param>
        /// <returns>Date and time at which noon occurs.</returns>
        public static ZonedDateTime Noon(
            Observer Observer,
            LocalDate? Date,
            string Timezone = "UTC")
        {
            LocalDate ld;
            if (Date == null)
            {
                ld = Library.Today(Timezone);
            }
            else
            {
                ld = (LocalDate)Date;
            }

            var jc = Private.JulianDayToJulianCentury(Library.JulianDay(ld));
            var eqtime = Private.EquationOfTime(jc);
            var timeUTC = (720.0 - (4 * Observer.Longitude) - eqtime) / 60.0;

            int hours = (int)Math.Floor(timeUTC);
            int minutes = (int)Math.Floor((timeUTC - hours) * 60.0);
            int seconds = (int)Math.Floor((((timeUTC - hours) * 60) - minutes) * 60);

            if (seconds > 59)
            {
                seconds -= 60;
                minutes += 1;
            }
            else if (seconds < 0)
            {
                seconds += 60;
                minutes -= 1;
            }

            if (minutes > 59)
            {
                minutes -= 60;
                hours += 1;
            }
            else if (minutes < 0)
            {
                minutes += 60;
                hours -= 1;
            }

            int days = 0;
            if (hours > 23)
            {
                hours -= 24;
                days = 1;
            }
            else if (hours < 0)
            {
                hours += 24;
                days = -1;
            }

            TimeSpan ts = new(days, hours, minutes, seconds);
            Duration d = Duration.FromTimeSpan(ts);
            Instant i = Instant.FromUtc(ld.Year, ld.Month, ld.Day, 0, 0, 0);
            i += d;
            DateTimeZone tz = Library.GetTimezone(Timezone);
            ZonedDateTime noon = new(i, tz);

            if (Timezone != "UTC")
            {
                noon = noon.WithZone(Library.GetTimezone(Timezone));
            }

            return noon;
        }

        /// <summary>
        /// Calculate solar midnight time.
        /// 
        /// Note:
        ///    This calculates the solar midnight that is closest
        ///    to 00:00:00 of the specified date i.e.it may return a time that is on
        ///    the previous day.
        /// </summary>
        /// <param name="Observer">An observer viewing the sun at a specific, latitude, longitude and elevation</param>
        /// <param name="Date">Date to calculate for. Default is today for the specified tzinfo.</param>
        /// <param name="Timezone">Timezone to return times in. Default is UTC.</param>
        /// <returns>Date and time at which midnight occurs.</returns>
        public static ZonedDateTime Midnight(
            Observer Observer,
            LocalDate? Date,
            string Timezone = "UTC")
        {
            LocalDate ld;
            if (Date == null)
            {
                ld = Library.Today(Timezone);
            }
            else
            {
                ld = (LocalDate)Date;
            }

            var jc = Private.JulianDayToJulianCentury(Library.JulianDay(ld) + 0.5 + -Observer.Longitude / 360.0);
            var eqtime = Private.EquationOfTime(jc);
            var timeUTC = (720.0 - (4 * Observer.Longitude) - eqtime) / 60.0;

            int hours = (int)Math.Floor(timeUTC);
            int minutes = (int)Math.Floor((timeUTC - hours) * 60.0);
            int seconds = (int)Math.Floor((((timeUTC - hours) * 60) - minutes) * 60);

            if (seconds > 59)
            {
                seconds -= 60;
                minutes += 1;
            }
            else if (seconds < 0)
            {
                seconds += 60;
                minutes -= 1;
            }

            if (minutes > 59)
            {
                minutes -= 60;
                hours += 1;
            }
            else if (minutes < 0)
            {
                minutes += 60;
                hours -= 1;
            }

            int days = 0;
            if (hours > 23)
            {
                hours -= 24;
                days = 1;
            }
            else if (hours < 0)
            {
                hours += 24;
                days = -1;
            }

            TimeSpan ts = new(days, hours, minutes, seconds);
            Duration d = Duration.FromTimeSpan(ts);
            Instant i = Instant.FromUtc(ld.Year, ld.Month, ld.Day, 0, 0, 0);
            i += d;
            DateTimeZone tz = Library.GetTimezone(Timezone);
            ZonedDateTime midnight = new(i, tz);

            if (Timezone != "UTC")
            {
                midnight = midnight.WithZone(Library.GetTimezone(Timezone));
            }

            return midnight;
        }

        /// <summary>
        /// Calculate the zenith angle of the sun.
        /// </summary>
        /// <param name="Observer">Observer to calculate the solar zenith for</param>
        /// <param name="DateAndTime">The date and time for which to calculate the angle. If null the current date and time are used</param>
        /// <param name="WithRefraction">If `true` adjust zenith to take refraction into account</param>
        /// <returns>The zenith angle in degrees.</returns>
        public static double Zenith(
            Observer Observer,
            ZonedDateTime? DateAndTime,
            bool WithRefraction = true)
        {
            if (DateAndTime == null)
            {
                DateAndTime = Library.Now();
            }
            return Private.ZenithAndAzimuth(Observer, (ZonedDateTime)DateAndTime, WithRefraction).Item1;
        }

        /// <summary>
        /// Calculate the azimuth angle of the sun.
        /// </summary>
        /// <param name="Observer">Observer to calculate the solar zenith for</param>
        /// <param name="DateAndTime">The date and time for which to calculate the angle. If null the current date and time are used</param>
        /// <returns>The azimith angle in degrees.</returns>
        public static double Azimith(
            Observer Observer,
            ZonedDateTime? DateAndTime)
        {
            if (DateAndTime == null)
            {
                DateAndTime = Library.Now();
            }
            return Private.ZenithAndAzimuth(Observer, (ZonedDateTime)DateAndTime).Item2;
        }

        /// <summary>
        /// Calculate the sun's angle of elevation.
        /// </summary>
        /// <param name="Observer">Observer to calculate the solar elevation for</param>
        /// <param name="DateAndTime">The date and time for which to calculate the angle. If null the current date and time are used</param>
        /// <param name="WithRefraction">If `true` adjust zenith to take refraction into account</param>
        /// <returns>The elevation angle in degrees above the horizon.</returns>
        public static double Elevation(
            Observer Observer,
            ZonedDateTime? DateAndTime,
            bool WithRefraction = true)
        {
            if (DateAndTime == null)
            {
                DateAndTime = Library.Now();
            }
            return 90.0 - Zenith(Observer, DateAndTime, WithRefraction);
        }

        /// <summary>
        /// Calculate dawn time.
        /// </summary>
        /// <param name="Observer">Observer to calculate dawn for</param>
        /// <param name="Date">Date to calculate for. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="Depression">Number of degrees below the horizon to use to calculate dawn. Default is for Civil dawn i.e. 6.0</param>
        /// <param name="Timezone">Timezone to return times in. Default is UTC.</param>
        /// <returns>Date and time at which dawn occurs.</returns>
        /// <throws>ElevationNeverReachedException if the sun never reaches the specified depression</throws>
        public static ZonedDateTime Dawn(
            Observer Observer,
            LocalDate? Date,
            double Depression = Library.Depression.Civil,
            string Timezone = "UTC")
        {
            if (Date == null)
            {
                Date = Library.Today(Timezone);
            }
            try
            {
                return TimeAtZenith(Observer, (LocalDate)Date, 90.0 + Depression, Library.SunDirection.Rising)
                    .WithZone(Library.GetTimezone(Timezone));
            }
            catch
            {
                ThrowAlwaysException(Observer, (LocalDate)Date, Depression);
                return new ZonedDateTime();
            }
        }

        /// <summary>
        /// Calculate sunrise time.
        /// </summary>
        /// <param name="Observer">Observer to calculate sunrise for</param>
        /// <param name="Date">Date to calculate for. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="Timezone">Timezone to return times in. Default is UTC.</param>
        /// <returns>Date and time at which sunrise occurs.</returns>
        /// <throws>ElevationNeverReachedException if the sun never reaches the specified depression</throws>
        public static ZonedDateTime Sunrise(
            Observer Observer,
            LocalDate? Date,
            string Timezone = "UTC")
        {
            if (Date == null)
            {
                Date = Library.Today(Timezone);
            }
            try
            {
                return TimeAtZenith(Observer, (LocalDate)Date, 90.0 + Library.SunApparentRadius, Library.SunDirection.Rising)
                    .WithZone(Library.GetTimezone(Timezone));
            }
            catch
            {
                ThrowAlwaysException(Observer, (LocalDate)Date, Library.SunApparentRadius);
                return new ZonedDateTime();
            }
        }

        /// <summary>
        /// Calculate sunset time.
        /// </summary>
        /// <param name="Observer">Observer to calculate sunset for</param>
        /// <param name="Date">Date to calculate for. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="Timezone">Timezone to return times in. Default is UTC.</param>
        /// <returns>Date and time at which sunset occurs.</returns>
        /// <throws>ElevationNeverReachedException if the sun never reaches the specified depression</throws>
        public static ZonedDateTime Sunset(
            Observer Observer,
            LocalDate? Date,
            string Timezone = "UTC")
        {
            if (Date == null)
            {
                Date = Library.Today(Timezone);
            }
            try
            {
                return TimeAtZenith(Observer, (LocalDate)Date, 90.0 + Library.SunApparentRadius, Library.SunDirection.Setting)
                    .WithZone(Library.GetTimezone(Timezone));
            }
            catch
            {
                ThrowAlwaysException(Observer, (LocalDate)Date, Library.SunApparentRadius);
                return new ZonedDateTime();
            }
        }

        /// <summary>
        /// Calculate dusk time.
        /// </summary>
        /// <param name="Observer">Observer to calculate duck for</param>
        /// <param name="Date">Date to calculate for. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="Depression">Number of degrees below the horizon to use to calculate dawn. Default is for Civil dawn i.e. 6.0</param>
        /// <param name="Timezone">Timezone to return times in. Default is UTC.</param>
        /// <returns>Date and time at which dawn occurs.</returns>
        /// <throws>ElevationNeverReachedException if the sun never reaches the specified depression</throws>
        public static ZonedDateTime Dusk(
            Observer Observer,
            LocalDate? Date,
            double Depression = Library.Depression.Civil,
            string Timezone = "UTC")
        {
            if (Date == null)
            {
                Date = Library.Today(Timezone);
            }
            try
            {
                return TimeAtZenith(Observer, (LocalDate)Date, 90.0 + Depression, Library.SunDirection.Setting)
                    .WithZone(Library.GetTimezone(Timezone));
            }
            catch
            {
                ThrowAlwaysException(Observer, (LocalDate)Date, Depression);
                return new ZonedDateTime();
            }
        }

        /// <summary>
        /// Calculate daylight start and end times.
        /// </summary>
        /// <param name="Observer">Observer to calculate duck for</param>
        /// <param name="Date">Date to calculate for. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="Timezone">Timezone to return times in. Default is UTC.</param>
        /// <returns>A tuple of the date and time at which daylight starts and ends.</returns>
        /// <throws>ElevationNeverReachedException if sunrise does not occur on the specified date or sunset on the following day</throws>
        public static Tuple<ZonedDateTime, ZonedDateTime> Daylight(
            Observer Observer,
            LocalDate? Date,
            string Timezone = "UTC")
        {
            if (Date == null)
            {
                Date = Library.Today(Timezone);
            }
            var start = Sunrise(Observer, Date, Timezone);
            var end = Sunset(Observer, Date, Timezone);
            return new Tuple<ZonedDateTime, ZonedDateTime>(start, end);
        }

        /// <summary>
        /// Calculate night start and end times.
        /// </summary>
        /// <param name="Observer">Observer to calculate duck for</param>
        /// <param name="Date">Date to calculate for. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="Timezone">Timezone to return times in. Default is UTC.</param>
        /// <returns>A tuple of the date and time at which night starts and ends.</returns>
        /// <throws>ElevationNeverReachedException if dusk does not occur on the specified date or dawn on the following day</throws>
        public static Tuple<ZonedDateTime, ZonedDateTime> Night(
            Observer Observer,
            LocalDate? Date,
            string Timezone = "UTC")
        {
            if (Date == null)
            {
                Date = Library.Today(Timezone);
            }
            var start = Dusk(Observer, Date, 6, Timezone);
            var tomorrow = (LocalDate)Date + Period.FromDays(1);
            var end = Dawn(Observer, tomorrow, 6, Timezone);
            return new Tuple<ZonedDateTime, ZonedDateTime>(start, end);
        }

        /// <summary>
        /// Returns the start and end times of Twilight when the sun is traversing in the specified direction.
        /// 
        /// This method defines twilight as being between the time when the sun is at -6 degrees and sunrise/sunset.
        /// </summary>
        /// <param name="Observer">Observer to calculate twilight for</param>
        /// <param name="Date">Date for which to calculate the times. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="Direction">Determines whether the time is for the sun rising or setting. Use ``Astral.SunDirection.RISING`` or ``Astral.SunDirection.SETTING``.</param>
        /// <param name="Timezone">Timezone to return times in. Default is UTC.</param>
        /// <returns>A tuple of the start and end of twilight</returns>
        public static Tuple<ZonedDateTime, ZonedDateTime> Twilight(
            Observer Observer,
            LocalDate? Date,
            Library.SunDirection Direction = Library.SunDirection.Rising,
            string Timezone = "UTC")
        {
            if (Date == null)
            {
                Date = Library.Today(Timezone);
            }
            var tz = Library.GetTimezone(Timezone);
            var start = TimeAtZenith(Observer, (LocalDate)Date, 90 + 6, Direction)
                .WithZone(tz);
            ZonedDateTime end;
            if (Direction == Library.SunDirection.Rising)
            {
                end = Sunrise(Observer, (LocalDate)Date, Timezone)
                    .WithZone(tz);

                return new(start, end);
            }
            else
            {
                end = Sunset(Observer, (LocalDate)Date, Timezone)
                    .WithZone(tz);
                return new(end, start);
            }
        }

        /// <summary>
        /// Returns the start and end times of the Golden Hour when the sun is traversing in the specified direction.
        /// 
        /// This method uses the definition from PhotoPills i.e. the
        /// golden hour is when the sun is between 4 degrees below the horizon
        /// and 6 degrees above.
        /// </summary>
        /// <param name="Observer">Observer to calculate the Golden Hour for</param>
        /// <param name="Date">Date for which to calculate the times. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="Direction">Determines whether the time is for the sun rising or setting. Use ``Astral.SunDirection.RISING`` or ``Astral.SunDirection.SETTING``.</param>
        /// <param name="Timezone">Timezone to return times in. Default is UTC.</param>
        /// <returns>A tuple of the start and end of the Golden Hour</returns>
        public static Tuple<ZonedDateTime, ZonedDateTime> GoldenHour(
            Observer Observer,
            LocalDate? Date,
            Library.SunDirection Direction = Library.SunDirection.Rising,
            string Timezone = "UTC")
        {
            if (Date == null)
            {
                Date = Library.Today(Timezone);
            }
            var tz = Library.GetTimezone(Timezone);
            var start = TimeAtZenith(Observer, (LocalDate)Date, 90 + 4, Direction)
                .WithZone(tz);
            var end = TimeAtZenith(Observer, (LocalDate)Date, 90 - 6, Direction)
                    .WithZone(tz);

            if (Direction == Library.SunDirection.Rising)
            {
                return new(start, end);
            }
            else
            {
                return new(end, start);
            }
        }

        /// <summary>
        /// Returns the start and end times of the Blue Hour when the sun is traversing in the specified direction.
        /// 
        /// This method uses the definition from PhotoPills i.e.the
        /// blue hour is when the sun is between 6 and 4 degrees below the horizon.
        /// </summary>
        /// <param name="Observer">Observer to calculate the Blue Hour for</param>
        /// <param name="Date">Date for which to calculate the times. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="Direction">Determines whether the time is for the sun rising or setting. Use ``Astral.SunDirection.RISING`` or ``Astral.SunDirection.SETTING``.</param>
        /// <param name="Timezone">Timezone to return times in. Default is UTC.</param>
        /// <returns>A tuple of the start and end of the Golden Hour</returns>
        public static Tuple<ZonedDateTime, ZonedDateTime> BlueHour(
            Observer Observer,
            LocalDate? Date,
            Library.SunDirection Direction = Library.SunDirection.Rising,
            string Timezone = "UTC")
        {
            if (Date == null)
            {
                Date = Library.Today(Timezone);
            }
            var tz = Library.GetTimezone(Timezone);
            var start = TimeAtZenith(Observer, (LocalDate)Date, 90 + 6, Direction)
                .WithZone(tz);
            var end = TimeAtZenith(Observer, (LocalDate)Date, 90 + 4, Direction)
                    .WithZone(tz);

            if (Direction == Library.SunDirection.Rising)
            {
                return new(start, end);
            }
            else
            {
                return new(end, start);
            }
        }

        /// <summary>
        /// Calculate Ruhakaalam times.
        /// </summary>
        /// <param name="Observer">Observer to calculate rahukaalam for</param>
        /// <param name="Date">Date to calculate for. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="DayTime">If true calculate Ruhakaalam times for the daytime otherwsie for the night</param>
        /// <param name="Timezone">Timezone to return times in. Default is UTC.</param>
        /// <returns>A tuple of the start and end of Ruhakaalam</returns>
        public static Tuple<ZonedDateTime, ZonedDateTime> Rahukaalam(
            Observer Observer,
            LocalDate? Date,
            bool DayTime = true,
            string Timezone = "UTC")
        {
            if (Date == null)
            {
                Date = Library.Today(Timezone);
            }

            ZonedDateTime start;
            ZonedDateTime end;
            if (DayTime)
            {
                start = Sunrise(Observer, (LocalDate)Date, Timezone);
                end = Sunset(Observer, (LocalDate)Date, Timezone);
            }
            else
            {
                start = Sunset(Observer, (LocalDate)Date, Timezone);
                var tomorrow = (LocalDate)Date + Period.FromDays(1);
                end = Sunrise(Observer, tomorrow, Timezone);
            }

            Duration td = start - end;
            Duration octantDuration = td / 8;
            int[] octant_index = new int[] { 1, 6, 4, 5, 3, 2, 7 };

            var weekday = ((LocalDate)Date).DayOfWeek;
            var octant = octant_index[(int)weekday];
            start += octantDuration * octant;
            end = start + octantDuration;

            return new(start, end);
        }

        private static void ThrowAlwaysException(
            Observer Observer,
            LocalDate Date,
            double Depression)
        {
            var z = Zenith(Observer, Noon(Observer, Date));
            if (z > 90.0)
            {
                throw new SunAlwaysBelowElevationException(
                    $"Dawn: Sun is always below {Depression} degrees on this day, at this location.");
            }
            else
            {
                throw new SunAlwaysAboveElevationException(
                    $"Dawn: Sun is always above {Depression} degrees on this day, at this location.");
            }
        }
    }
}
