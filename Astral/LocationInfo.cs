using NodaTime;

#nullable enable

namespace SFFJunkie.Astral
{
    public class LocationInfo
    {
        private readonly Observer _observer = new();
        private double latitude = 51.4733;
        private double longitude = -0.0008333;

        public string Name { get; set; } = "Greenwich";
        public string Region { get; set; } = "England";
        public string Timezone { get; set; } = "Europe/London";
        public double Latitude {
            get => latitude;
            set {
                latitude = value;
                _observer.Latitude = value;
            }
        }
        public double Longitude {
            get => longitude;
            set
            {
                longitude = value;
                _observer.Longitude = value;
            }
        }

        /// <summary>
        /// Returns a new Observe instance
        /// </summary>
        public Observer Observer
        {
            get
            {
                return new Observer(_observer);
            }
        }

        public LocationInfo() { }

        public LocationInfo(string name, string region, string timezone, double latitude, double longitude)
        {
            Name = name;
            Region = region;
            Timezone = timezone;
            Latitude = latitude;
            Longitude = longitude;
        }

        public LocationInfo(string name, string region, string timezone, string latitude, string longitude)
        {
            Name = name;
            Region = region;
            Timezone = timezone;
            Latitude = Library.DMSToDouble(latitude, 90.0);
            Longitude = Library.DMSToDouble(longitude, 180.0);
        }

        public DateTimeZone? TimezoneInfo()
        {
            var info = DateTimeZoneProviders.Tzdb.GetZoneOrNull(Timezone);
            if (info == null)
            {
                info = DateTimeZoneProviders.Bcl.GetZoneOrNull(Timezone);
            }
            return info;
        }

        public bool Equals(LocationInfo loc)
        {
            return (Name == loc.Name && Region == loc.Region && Latitude == loc.latitude && Longitude == loc.Longitude);
        }

        public string TimezoneGroup
        {
            get => Timezone.Split("/")[0];
        }
    }
}
