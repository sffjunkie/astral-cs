using Xunit;

namespace SFFJunkie.Astral.Test.Fixture
{
    public class LocationInfoFixture
    {
        public LocationInfoFixture()
        {
            LondonInfo = new Astral.LocationInfo("London", "England", "Europe/London", 51.50853, -0.12574);
            NewDelhiInfo = new Astral.LocationInfo("New Delhi", "India", "Asia/Kolkata", 28.61, 77.22);
            RiyadhInfo = new Astral.LocationInfo("Riyadh", "Saudi Arabia", "Asia/Riyadh", 24.71355, 46.67530);
        }
        public Astral.LocationInfo LondonInfo { get; private set; }
        public Astral.LocationInfo NewDelhiInfo { get; private set; }
        public Astral.LocationInfo RiyadhInfo { get; private set; }
    }

    [CollectionDefinition("LocationInfo collection")]
    public class DatabaseCollection : ICollectionFixture<LocationInfoFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
