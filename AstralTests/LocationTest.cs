using SFFJunkie.Astral.Test.Fixture;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SFFJunkie.Astral.Test
{
    [Collection("LocationInfo collection")]
    public class LocationTest
    {
        private readonly LocationInfoFixture infoFixture;

        public LocationTest(LocationInfoFixture fixture)
        {
            infoFixture = fixture;
        }

        [Fact]
        public void Location_New()
        {
            Location loc = new();
            Assert.Equal("Greenwich", loc.Name);
            Assert.Equal("England", loc.Region);
            Assert.Equal("Europe/London", loc.Timezone);
            Assert.Equal(51.4733, loc.Latitude);
            Assert.Equal(-0.0008333, loc.Longitude);
        }

        [Fact]
        public void Location_LocationInfo()
        {
            LocationInfo expected = infoFixture.LondonInfo;
            Location loc = new(infoFixture.LondonInfo);
            Assert.True(expected.Equals(loc.Info));
        }

        [Fact]
        public void Location_Observer()
        {
            LocationInfo info = infoFixture.LondonInfo;
            Observer expected = new(info.Latitude, info.Longitude, 0.0, 0.0);
            Location loc = new(infoFixture.LondonInfo);
            Assert.True(expected.Equals(loc.Observer));
        }

        [Fact]
        public void Location_NameProperty()
        {
            Location loc = new();
            loc.Name = "Somewhere";
            Assert.Equal("Somewhere", loc.Name);
        }

        [Fact]
        public void Location_RegionProperty()
        {
            Location loc = new();
            loc.Region = "Somewhere";
            Assert.Equal("Somewhere", loc.Region);
        }

        [Fact]
        public void Location_TimezoneProperty()
        {
            Location loc = new();
            loc.Timezone = "Asia/Kolkata";
            Assert.Equal("Asia/Kolkata", loc.Timezone);
        }

        [Fact]
        public void Location_TimezoneGroupProperty()
        {
            Location loc = new();
            loc.Timezone = "Asia/Kolkata";
            Assert.Equal("Asia", loc.TimezoneGroup);
        }

        [Fact]
        public void Location_LatitudeProperty()
        {
            Location loc = new();
            loc.Latitude = 10;
            Assert.Equal(10, loc.Latitude);
        }

        [Fact]
        public void Location_LongitudeProperty()
        {
            Location loc = new();
            loc.Longitude = 10;
            Assert.Equal(10, loc.Longitude);
        }

        [Fact]
        public void Test_Dawn_Local()
        {
            Instant i = Instant.FromUtc(2015, 8, 1, 4, 41, 44);
            var expected = new ZonedDateTime(i, Astral.Library.GetTimezone(infoFixture.LondonInfo.Timezone));
            var d = new LocalDate(2015, 8, 1);
            var loc = new Location(infoFixture.LondonInfo);
            var actual = loc.Dawn(d);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
            Assert.True(infoFixture.LondonInfo.TimezoneInfo().Equals(actual.Zone));
        }

        [Fact]
        public void Test_Dawn_UTC()
        {
            Instant i = Instant.FromUtc(2015, 8, 1, 3, 41, 44);
            var expected = new ZonedDateTime(i, DateTimeZone.Utc);
            var d = new LocalDate(2015, 8, 1);
            var loc = new Location(infoFixture.LondonInfo);
            var actual = loc.Dawn(d, Local: false);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
            Assert.True(DateTimeZone.Utc.Equals(actual.Zone));
        }

        [Fact]
        public void Test_Sunrise_Local()
        {
            Instant i = Instant.FromUtc(2015, 8, 1, 5, 23, 20);
            var expected = new ZonedDateTime(i, Astral.Library.GetTimezone(infoFixture.LondonInfo.Timezone));
            var d = new LocalDate(2015, 8, 1);
            var loc = new Location(infoFixture.LondonInfo);
            var actual = loc.Sunrise(d);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
            Assert.True(infoFixture.LondonInfo.TimezoneInfo().Equals(actual.Zone));
        }

        [Fact]
        public void Test_Sunrise_UTC()
        {
            Instant i = Instant.FromUtc(2015, 8, 1, 4, 23, 44);
            var expected = new ZonedDateTime(i, DateTimeZone.Utc);
            var d = new LocalDate(2015, 8, 1);
            var loc = new Location(infoFixture.LondonInfo);
            var actual = loc.Sunrise(d, Local: false);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
            Assert.True(DateTimeZone.Utc.Equals(actual.Zone));
        }

        [Fact]
        public void Test_Noon_Local()
        {
            Instant i = Instant.FromUtc(2015, 8, 1, 13, 6, 53);
            var expected = new ZonedDateTime(i, Astral.Library.GetTimezone(infoFixture.LondonInfo.Timezone));
            var d = new LocalDate(2015, 8, 1);
            var loc = new Location(infoFixture.LondonInfo);
            var actual = loc.Noon(d);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
            Assert.True(infoFixture.LondonInfo.TimezoneInfo().Equals(actual.Zone));
        }

        [Fact]
        public void Test_Noon_UTC()
        {
            Instant i = Instant.FromUtc(2015, 8, 1, 12, 6, 53);
            var expected = new ZonedDateTime(i, DateTimeZone.Utc);
            var d = new LocalDate(2015, 8, 1);
            var loc = new Location(infoFixture.LondonInfo);
            var actual = loc.Noon(d, Local: false);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
            Assert.True(DateTimeZone.Utc.Equals(actual.Zone));
        }

        [Fact]
        public void Test_Dusk_Local()
        {
            Instant i = Instant.FromUtc(2015, 12, 1, 16, 35, 11);
            var expected = new ZonedDateTime(i, Astral.Library.GetTimezone(infoFixture.LondonInfo.Timezone));
            var d = new LocalDate(2015, 12, 1);
            var loc = new Location(infoFixture.LondonInfo);
            var actual = loc.Dusk(d);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
            Assert.True(infoFixture.LondonInfo.TimezoneInfo().Equals(actual.Zone));
        }

        [Fact]
        public void Test_Dusk_UTC()
        {
            Instant i = Instant.FromUtc(2015, 12, 1, 16, 35, 11);
            var expected = new ZonedDateTime(i, DateTimeZone.Utc);
            var d = new LocalDate(2015, 12, 1);
            var loc = new Location(infoFixture.LondonInfo);
            var actual = loc.Dusk(d, Local: false);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
            Assert.True(DateTimeZone.Utc.Equals(actual.Zone));
        }

        [Fact]
        public void Test_Sunset_Local()
        {
            Instant i = Instant.FromUtc(2015, 12, 1, 15, 55, 29);
            var expected = new ZonedDateTime(i, Astral.Library.GetTimezone(infoFixture.LondonInfo.Timezone));
            var d = new LocalDate(2015, 12, 1);
            var loc = new Location(infoFixture.LondonInfo);
            var actual = loc.Sunset(d);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
            Assert.True(infoFixture.LondonInfo.TimezoneInfo().Equals(actual.Zone));
        }

        [Fact]
        public void Test_Sunset_UTC()
        {
            Instant i = Instant.FromUtc(2015, 12, 1, 15, 55, 29);
            var expected = new ZonedDateTime(i, DateTimeZone.Utc);
            var d = new LocalDate(2015, 12, 1);
            var loc = new Location(infoFixture.LondonInfo);
            var actual = loc.Sunset(d, Local: false);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
            Assert.True(DateTimeZone.Utc.Equals(actual.Zone));
        }
    }
}
