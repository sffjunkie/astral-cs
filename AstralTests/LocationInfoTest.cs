using SFFJunkie.Astral;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NodaTime;

namespace SFFJunkie.Astral.Test
{
    public class LocationInfoTest
    {
        [Fact]
        public void LocationInfo_New_Default()
        {
            var location = new LocationInfo();
            Assert.Equal("Greenwich", location.Name);
            Assert.Equal("England", location.Region);
            Assert.Equal("Europe/London", location.Timezone);
            Assert.Equal("Europe", location.TimezoneGroup);
            Assert.Equal(51.4733, location.Latitude);
            Assert.Equal(-0.0008333, location.Longitude);
        }

        [Fact]
        public void LocationInfo_New_WithDouble()
        {
            var location = new LocationInfo("Greenwich", "England", "Europe/London", 20.0, 10.0);
            Assert.Equal("Greenwich", location.Name);
            Assert.Equal("England", location.Region);
            Assert.Equal("Europe/London", location.Timezone);
            Assert.Equal("Europe", location.TimezoneGroup);
            Assert.Equal(20.0, location.Latitude);
            Assert.Equal(10.0, location.Longitude);
        }

        [Fact]
        public void LocationInfo_New_WithString()
        {
            var location = new LocationInfo("Greenwich", "England", "Europe/London", "20°", "10°");
            Assert.Equal(20, location.Latitude);
            Assert.Equal(10, location.Longitude);
        }

        [Fact]
        public void LocationInfo_TimezoneGroup()
        {
            var location = new LocationInfo("Greenwich", "England", "Some/Where", 20.0, 10.0);
            Assert.Equal("Some", location.TimezoneGroup);
        }

        [Fact]
        public void LocationInfo_Timezone()
        {
            var location = new LocationInfo("Greenwich", "England", "Europe/London", 20.0, 10.0);
            DateTimeZone expected = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/London");
            Assert.True(expected.Equals(location.TimezoneInfo()));
        }

        [Fact]
        public void LocationInfo_Observer()
        {
            var location = new LocationInfo("Greenwich", "England", "Europe/London", 20.0, 10.0);
            var observer = location.Observer;
            Assert.Equal(20, observer.Latitude);
            Assert.Equal(10, observer.Longitude);
        }
    }
}
