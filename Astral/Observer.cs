namespace SFFJunkie.Astral
{
    /// <summary>
    /// Defines the location of an observer on Earth.
    /// 
    /// Latitude and longitude can be set either as a float or as a string.
    /// For strings they must be of the form
    ///
    ///     degrees°minutes'seconds"[N|S|E|W] e.g. 51°31'N
    ///
    /// `minutes’` & `seconds”` are optional.
    /// </summary>
    public class Observer
    {
        /// <summary>
        /// Latitude - Northern latitudes should be positive
        /// </summary>
        public double Latitude { get; set; } = 51.4733;

        /// <summary>
        /// Longitude - Eastern longitudes should be positive
        /// </summary>
        public double Longitude { get; set; } = -0.0008333;

        /// <summary>
        /// Elevation in metres above the location or the difference in elevation to the obscuring feature.
        /// </summary>
        public double Elevation { get; set; } = 0.0;

        /// <summary>
        /// Distance to the feature that obscures the sun
        /// </summary>
        public double DistanceToFeature { get; set; } = 0.0;

        public Observer(Observer obs)
        {
            Latitude = obs.Latitude;
            Longitude = obs.Longitude;
            Elevation = obs.Elevation;
            DistanceToFeature = obs.DistanceToFeature;
        }

        public Observer() { }

        public Observer(double Latitude, double Longitude, double Elevation = 0.0, double DistanceToFeature = 0.0)
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
            this.Elevation = Elevation;
            this.DistanceToFeature = DistanceToFeature;
        }

        public Observer(string Latitude, string Longitude, double Elevation, double DistanceToFeature = 0.0)
        {
            this.Latitude = Library.DMSToDouble(Latitude, 90.0);
            this.Longitude = Library.DMSToDouble(Longitude, 180.0);
            this.Elevation = Elevation;
            this.DistanceToFeature = DistanceToFeature;
        }

        public bool Equals(Observer obs)
        {
            return (Latitude == obs.Latitude && Longitude == obs.Longitude && Elevation == obs.Elevation && DistanceToFeature == obs.DistanceToFeature);
        }
    }
}
