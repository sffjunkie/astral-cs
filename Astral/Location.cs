using NodaTime;
using System;

#nullable enable

namespace SFFJunkie.Astral
{
    public class Location
    {
        protected LocationInfo _locationInfo;

        public string Name { get => _locationInfo.Name; set => _locationInfo.Name = value; }
        public string Region { get => _locationInfo.Region; set => _locationInfo.Region = value; }
        public string Timezone { get => _locationInfo.Timezone; set => _locationInfo.Timezone = value; }
        public double Latitude { get => _locationInfo.Latitude; set => _locationInfo.Latitude = value; }
        public double Longitude { get => _locationInfo.Longitude; set => _locationInfo.Longitude = value; }
        public LocationInfo Info { get => _locationInfo; }
        public Observer Observer { get => _locationInfo.Observer; }
        public string TimezoneGroup { get => _locationInfo.TimezoneGroup; }
        public DateTimeZone? TZInfo
        {
            get
            {
                DateTimeZone? tz;
                tz = DateTimeZoneProviders.Tzdb.GetZoneOrNull(Timezone);
                if (tz == null)
                {
                    tz = DateTimeZoneProviders.Bcl.GetZoneOrNull(Timezone);
                }
                return tz;
            }
        }
        public double SolarDepression { get; set; } = Library.Depression.Civil;

        public Location()
        {
            _locationInfo = new LocationInfo();
        }

        public Location(LocationInfo info)
        {
            _locationInfo = info;
        }

        public Location(string name, string region, string timezone, double latitude, double longitude)
        {
            _locationInfo = new LocationInfo(name, region, timezone, latitude, longitude);
        }

        public Location(string name, string region, string timezone, string latitude, string longitude)
        {
            _locationInfo = new LocationInfo(name, region, timezone, Library.DMSToDouble(latitude, 90), Library.DMSToDouble(longitude, 180));
        }

        public bool Equals(Location location)
        {
            return _locationInfo.Equals(location._locationInfo);
        }

        /// <summary>
        /// Calculate solar noon time when the sun is at its highest point.
        /// </summary>
        /// <param name="Date">Date to calculate for. Default is today for the specified tzinfo.</param>
        /// <param name="Local">If true return the date/time in the local timezone.</param>
        /// <param name="ObserverElevation">The elevation of the observer</param>
        /// <returns>Date and time at which noon occurs.</returns>
        public ZonedDateTime Noon(
            LocalDate? Date,
            bool Local = true,
            double ObserverElevation = 0.0)
        {
            string tz = Local ? Timezone : "UTC";
            Observer o = _locationInfo.Observer;
            o.Elevation = ObserverElevation;

            return Sun.Noon(o, Date, tz);
        }

        /// <summary>
        /// Calculate solar noon time when the sun is at its highest point.
        /// </summary>
        /// <param name="Date">Date to calculate for. Default is today for the specified tzinfo.</param>
        /// <param name="Local">If true return the date/time in the local timezone.</param>
        /// <param name="ObserverElevation">The elevation of the observer</param>
        /// <returns>Date and time at which noon occurs.</returns>
        public ZonedDateTime Midnight(
            LocalDate? Date,
            bool Local = true,
            double ObserverElevation = 0.0)
        {
            string timezoneName = Local ? Timezone : "UTC";
            Observer o = _locationInfo.Observer;
            o.Elevation = ObserverElevation;

            return Sun.Midnight(o, Date, timezoneName);
        }

        /// <summary>
        /// Calculate the elevation angle of the sun.
        /// </summary>
        /// <param name="DateAndTime">The date and time for which to calculate the angle. If null the current date and time are used</param>
        /// <param name="ObserverElevation">Elevation of the observer at this location</param>
        /// <param name="WithRefraction">If `true` adjust zenith to take refraction into account</param>
        /// <returns>The elevation angle in degrees.</returns>
        public double Elevation(ZonedDateTime? DateAndTime, double ObserverElevation, bool WithRefraction)
        {
            if (DateAndTime == null)
            {
                DateAndTime = Library.Now(Timezone);
            }
            Observer o = _locationInfo.Observer;
            o.Elevation = ObserverElevation;
            return Sun.Elevation(o, DateAndTime, WithRefraction);
        }

        /// <summary>
        /// Calculate the azimuth angle of the sun.
        /// </summary>
        /// <param name="DateAndTime">The date and time for which to calculate the angle. If null the current date and time are used</param>
        /// <param name="ObserverElevation">Elevation of the observer at this location</param>
        /// <returns>The azimith angle in degrees.</returns>
        public double Azimuth(ZonedDateTime? DateAndTime, double ObserverElevation)
        {
            if (DateAndTime == null)
            {
                DateAndTime = Library.Now(Timezone);
            }
            Observer o = _locationInfo.Observer;
            o.Elevation = ObserverElevation;
            return Sun.Azimith(o, DateAndTime);
        }

        /// <summary>
        /// Calculate the sun's angle of elevation.
        /// </summary>
        /// <param name="DateAndTime">The date and time for which to calculate the angle. If null the current date and time are used</param>
        /// <param name="WithRefraction">If `true` adjust zenith to take refraction into account</param>
        /// <returns>The elevation angle in degrees above the horizon.</returns>
        public double Elevation(
            ZonedDateTime? DateAndTime,
            bool WithRefraction = true)
        {
            Observer o = _locationInfo.Observer;
            return Sun.Elevation(o, DateAndTime, WithRefraction);
        }

        /// <summary>
        /// Calculate dawn time.
        /// </summary>
        /// <param name="Date">Date to calculate for. Default is today's date in the timezone `Timezone`.</param>
        /// <param name="Local">If true return the date/time in the local timezone.</param>
        /// <param name="Depression">Number of degrees below the horizon to use to calculate dawn. Default is for Civil dawn i.e. 6.0</param>
        /// <returns>Date and time at which dawn occurs.</returns>
        /// <throws>ElevationNeverReachedException if the sun never reaches the specified depression</throws>
        public ZonedDateTime Dawn(
            LocalDate? Date,
            bool Local = true,
            double Depression = Library.Depression.Civil)
        {
            Observer o = _locationInfo.Observer;
            string timezoneName = Local ? Timezone : "UTC";
            return Sun.Dawn(o, Date, Depression, timezoneName);
        }

        /// <summary>
        /// Calculate sunrise time.
        /// </summary>
        /// <param name="Date">Date to calculate for. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="Local">If true return the date/time in the local timezone.</param>
        /// <returns>Date and time at which sunrise occurs.</returns>
        /// <throws>ElevationNeverReachedException if the sun never reaches the specified depression</throws>
        public ZonedDateTime Sunrise(
            LocalDate? Date,
            bool Local = true)
        {
            Observer o = _locationInfo.Observer;
            string timezoneName = Local ? Timezone : "UTC";
            return Sun.Sunrise(o, Date, timezoneName);
        }

        /// <summary>
        /// Calculate sunset time.
        /// </summary>
        /// <param name="Date">Date to calculate for. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="Local">If true return the date/time in the local timezone.</param>
        /// <returns>Date and time at which sunset occurs.</returns>
        /// <throws>ElevationNeverReachedException if the sun never reaches the specified depression</throws>
        public ZonedDateTime Sunset(
            LocalDate? Date,
            bool Local = true)
        {
            Observer o = _locationInfo.Observer;
            string timezoneName = Local ? Timezone : "UTC";
            return Sun.Sunset(o, Date, timezoneName);
        }

        /// <summary>
        /// Calculate dusk time.
        /// </summary>
        /// <param name="Date">Date to calculate for. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="Local">If true return the date/time in the local timezone.</param>
        /// <param name="Depression">Number of degrees below the horizon to use to calculate dawn. Default is for Civil dawn i.e. 6.0</param>
        /// <returns>Date and time at which dusk occurs.</returns>
        /// <throws>ElevationNeverReachedException if the sun never reaches the specified depression</throws>
        public ZonedDateTime Dusk(
            LocalDate? Date,
            bool Local = true,
            double Depression = Library.Depression.Civil)
        {
            Observer o = _locationInfo.Observer;
            string timezoneName = Local ? Timezone : "UTC";
            return Sun.Dusk(o, Date, Depression, timezoneName);
        }

        /// <summary>
        /// Calculate daylight start and end times.
        /// </summary>
        /// <param name="Date">Date to calculate for. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="Local">If true return the date/time in the local timezone.</param>
        /// <returns>A tuple of the date and time at which daylight starts and ends.</returns>
        /// <throws>ElevationNeverReachedException if sunrise does not occur on the specified date or sunset on the following day</throws>
        public Tuple<ZonedDateTime, ZonedDateTime> Daylight(
            LocalDate? Date,
            bool Local = true)
        {
            Observer o = _locationInfo.Observer;
            string timezoneName = Local ? Timezone : "UTC";
            return Sun.Daylight(o, Date, timezoneName);
        }

        /// <summary>
        /// Calculate night start and end times.
        /// </summary>
        /// <param name="Date">Date to calculate for. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="Local">If true return the date/time in the local timezone.</param>
        /// <returns>A tuple of the date and time at which night starts and ends.</returns>
        /// <throws>ElevationNeverReachedException if dusk does not occur on the specified date or dawn on the following day</throws>
        public Tuple<ZonedDateTime, ZonedDateTime> Night(
            LocalDate? Date,
            bool Local = true)
        {
            Observer o = _locationInfo.Observer;
            string timezoneName = Local ? Timezone : "UTC";
            return Sun.Night(o, Date, timezoneName);
        }

        /// <summary>
        /// Returns the start and end times of Twilight when the sun is traversing in the specified direction.
        /// 
        /// This method defines twilight as being between the time when the sun is at -6 degrees and sunrise/sunset.
        /// </summary>
        /// <param name="Date">Date for which to calculate the times. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="Local">If true return the date/time in the local timezone.</param>
        /// <param name="Direction">Determines whether the time is for the sun rising or setting. Use ``Astral.SunDirection.Rising`` or ``Astral.SunDirection.Setting``.</param>
        /// <returns>A tuple of the start and end of twilight</returns>
        public Tuple<ZonedDateTime, ZonedDateTime> Twilight(
            LocalDate? Date,
            bool Local = true,
            Library.SunDirection Direction = Library.SunDirection.Rising)
        {
            Observer o = _locationInfo.Observer;
            string timezoneName = Local ? Timezone : "UTC";
            return Sun.Twilight(o, Date, Direction, timezoneName);
        }

        /// <summary>
        /// Returns the start and end times of the Golden Hour when the sun is traversing in the specified direction.
        /// 
        /// This method uses the definition from PhotoPills i.e. the
        /// golden hour is when the sun is between 4 degrees below the horizon
        /// and 6 degrees above.
        /// </summary>
        /// <param name="Date">Date for which to calculate the times. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="Local">If true return the date/time in the local timezone.</param>
        /// <param name="Direction">Determines whether the time is for the sun rising or setting. Use ``Astral.SunDirection.Rising`` or ``Astral.SunDirection.Setting``.</param>
        /// <returns>A tuple of the start and end of the Golden Hour</returns>
        public Tuple<ZonedDateTime, ZonedDateTime> GoldenHour(
            LocalDate? Date,
            bool Local = true,
            Library.SunDirection Direction = Library.SunDirection.Rising)
        {
            Observer o = _locationInfo.Observer;
            string timezoneName = Local ? Timezone : "UTC";
            return Sun.GoldenHour(o, Date, Direction, timezoneName);
        }

        /// <summary>
        /// Returns the start and end times of the Blue Hour when the sun is traversing in the specified direction.
        /// 
        /// This method uses the definition from PhotoPills i.e. the
        /// golden hour is when the sun is between 4 degrees below the horizon
        /// and 6 degrees above.
        /// </summary>
        /// <param name="Date">Date for which to calculate the times. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="Local">If true return the date/time in the local timezone.</param>
        /// <param name="Direction">Determines whether the time is for the sun rising or setting. Use ``Astral.SunDirection.Rising`` or ``Astral.SunDirection.Setting``.</param>
        /// <returns>A tuple of the start and end of the Blue Hour</returns>
        public Tuple<ZonedDateTime, ZonedDateTime> BlueHour(
            LocalDate? Date,
            bool Local = true,
            Library.SunDirection Direction = Library.SunDirection.Rising)
        {
            Observer o = _locationInfo.Observer;
            string timezoneName = Local ? Timezone : "UTC";
            return Sun.BlueHour(o, Date, Direction, timezoneName);
        }

        /// <summary>
        /// Calculate Ruhakaalam times.
        /// </summary>
        /// <param name="date">Date to calculate for. Default is today's date in the timezone `timezoneName`.</param>
        /// <param name="DayTime">If true calculate Ruhakaalam times for the daytime otherwsie for the night</param>
        /// <returns>A tuple of the start and end of Ruhakaalam</returns>
        public Tuple<ZonedDateTime, ZonedDateTime> Rahukaalam(
            LocalDate? Date,
            bool Local = true,
            bool DayTime = true)
        {
            Observer o = _locationInfo.Observer;
            string timezoneName = Local ? Timezone : "UTC";
            return Sun.Rahukaalam(o, Date, DayTime, timezoneName);
        }
    }
}
