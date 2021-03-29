using SFFJunkie.Astral.Test;
using SFFJunkie.Astral.Test.Fixture;
using NodaTime;
using Xunit;
using System;
using System.Collections.Generic;

namespace SFFJunkie.Astral.Test
{
    [Collection("LocationInfo collection")]
    public class SunTest
    {
        private readonly LocationInfoFixture infoFixture;

        public SunTest(LocationInfoFixture fixture)
        {
            infoFixture = fixture;
        }

        [Fact]
        public void Azimuth()
        {
            Instant i = Instant.FromUtc(2001, 6, 21, 13, 11, 0);
            ZonedDateTime zdt = new(i, DateTimeZone.Utc);

            var actual = Sun.Azimith(infoFixture.NewDelhiInfo.Observer, zdt);
            Assert.Equal(292.766381632981, actual, 4);
        }

        [Fact]
        public void Elevation()
        {
            Instant i = Instant.FromUtc(2001, 6, 21, 13, 11, 0);
            ZonedDateTime zdt = new(i, DateTimeZone.Utc);

            var actual = Sun.Elevation(infoFixture.NewDelhiInfo.Observer, zdt);
            Assert.Equal(7.411009003716742, actual, 4);
        }


        [Fact]
        public void Elevation_WithoutRefraction()
        {
            Instant i = Instant.FromUtc(2001, 6, 21, 13, 11, 0);
            ZonedDateTime zdt = new(i, DateTimeZone.Utc);

            var actual = Sun.Elevation(infoFixture.NewDelhiInfo.Observer, zdt, false);
            Assert.Equal(7.293490557358638, actual, 4);
        }

        [Fact]
        public void Azimuth_Above85Degrees()
        {
            Astral.Observer observer = new(86, 77.2);
            Instant i = Instant.FromUtc(2001, 6, 21, 13, 11, 0);
            ZonedDateTime zdt = new(i, DateTimeZone.Utc);

            var actual = Sun.Azimith(observer, zdt);
            Assert.Equal(276.2148, actual, 4);
        }

        [Fact]
        public void Elevation_Above85Degrees()
        {
            Astral.Observer observer = new(86, 77.2);
            Instant i = Instant.FromUtc(2001, 6, 21, 13, 11, 0);
            ZonedDateTime zdt = new(i, DateTimeZone.Utc);

            var actual = Sun.Elevation(observer, zdt);
            Assert.Equal(23.102501151619506, actual, 3);
        }

        public static IEnumerable<object[]> DawnData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2015, 12, 1), Instant.FromUtc(2015, 12, 1, 7, 4, 0) },
                new object[] { new LocalDate(2015, 12, 2), Instant.FromUtc(2015, 12, 2, 7, 5, 0) },
                new object[] { new LocalDate(2015, 12, 3), Instant.FromUtc(2015, 12, 3, 7, 6, 0) },
            };

        [Theory]
        [MemberData(nameof(DawnData))]
        public void Dawn(LocalDate date, Instant i)
        {
            ZonedDateTime expected = new(i, DateTimeZone.Utc);
            var actual = Sun.Dawn(infoFixture.LondonInfo.Observer, date);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
        }

        public static IEnumerable<object[]> DawnNauticalData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2015, 12, 1), Instant.FromUtc(2015, 12, 1, 7, 4, 0) },
                new object[] { new LocalDate(2015, 12, 2), Instant.FromUtc(2015, 12, 2, 7, 5, 0) },
                new object[] { new LocalDate(2015, 12, 3), Instant.FromUtc(2015, 12, 3, 7, 6, 0) },
                new object[] { new LocalDate(2015, 12, 12), Instant.FromUtc(2015, 12, 12, 6, 33, 0) },
                new object[] { new LocalDate(2015, 12, 25), Instant.FromUtc(2015, 12, 25, 6, 41, 0) },
            };

        [Theory]
        [MemberData(nameof(DawnNauticalData))]
        public void Dawn_Nautical(LocalDate date, Instant i)
        {
            ZonedDateTime expected = new(i, DateTimeZone.Utc);
            var actual = Sun.Dawn(infoFixture.LondonInfo.Observer, date, Astral.Library.Depression.Nautical);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
        }

        public static IEnumerable<object[]> DawnAstronomicalData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2015, 12, 1), Instant.FromUtc(2015, 12, 1, 7, 4, 0) },
                new object[] { new LocalDate(2015, 12, 2), Instant.FromUtc(2015, 12, 2, 7, 5, 0) },
                new object[] { new LocalDate(2015, 12, 3), Instant.FromUtc(2015, 12, 3, 7, 6, 0) },
                new object[] { new LocalDate(2015, 12, 12), Instant.FromUtc(2015, 12, 12, 5, 52, 0) },
                new object[] { new LocalDate(2015, 12, 25), Instant.FromUtc(2015, 12, 25, 6, 1, 0) },
            };

        [Theory]
        [MemberData(nameof(DawnAstronomicalData))]
        public void Dawn_Astronomical(LocalDate date, Instant i)
        {
            ZonedDateTime expected = new(i, DateTimeZone.Utc);
            var actual = Sun.Dawn(infoFixture.LondonInfo.Observer, date, Astral.Library.Depression.Astronomical);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
        }

        public static IEnumerable<object[]> SunriseData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2015, 1, 1), Instant.FromUtc(2015, 12, 1, 8, 6, 0) },
                new object[] { new LocalDate(2015, 12, 1), Instant.FromUtc(2015, 12, 1, 7, 43, 0) },
                new object[] { new LocalDate(2015, 12, 2), Instant.FromUtc(2015, 12, 2, 7, 45, 0) },
                new object[] { new LocalDate(2015, 12, 3), Instant.FromUtc(2015, 12, 3, 7, 46, 0) },
                new object[] { new LocalDate(2015, 12, 12), Instant.FromUtc(2015, 12, 12, 7, 56, 0) },
                new object[] { new LocalDate(2015, 12, 25), Instant.FromUtc(2015, 12, 25, 8, 5, 0) },
            };

        [Theory]
        [MemberData(nameof(SunriseData))]
        public void Sunrise(LocalDate date, Instant i)
        {
            ZonedDateTime expected = new(i, DateTimeZone.Utc);
            var actual = Sun.Sunrise(infoFixture.LondonInfo.Observer, date);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
        }

        public static IEnumerable<object[]> SunsetData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2015, 1, 1), Instant.FromUtc(2015, 12, 1, 16, 1, 0) },
                new object[] { new LocalDate(2015, 12, 1), Instant.FromUtc(2015, 12, 1, 15, 55, 0) },
                new object[] { new LocalDate(2015, 12, 2), Instant.FromUtc(2015, 12, 2, 15, 54, 0) },
                new object[] { new LocalDate(2015, 12, 3), Instant.FromUtc(2015, 12, 3, 15, 54, 0) },
                new object[] { new LocalDate(2015, 12, 12), Instant.FromUtc(2015, 12, 12, 15, 51, 0) },
                new object[] { new LocalDate(2015, 12, 25), Instant.FromUtc(2015, 12, 25, 15, 55, 0) },
            };

        [Theory]
        [MemberData(nameof(SunsetData))]
        public void Sunset(LocalDate date, Instant i)
        {
            ZonedDateTime expected = new(i, DateTimeZone.Utc);
            var actual = Sun.Sunset(infoFixture.LondonInfo.Observer, date);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
        }

        public static IEnumerable<object[]> DuskData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2015, 12, 1), Instant.FromUtc(2015, 12, 1, 16, 34, 0) },
                new object[] { new LocalDate(2015, 12, 2), Instant.FromUtc(2015, 12, 2, 16, 34, 0) },
                new object[] { new LocalDate(2015, 12, 3), Instant.FromUtc(2015, 12, 3, 16, 33, 0) },
                new object[] { new LocalDate(2015, 12, 12), Instant.FromUtc(2015, 12, 12, 16, 31, 0) },
                new object[] { new LocalDate(2015, 12, 25), Instant.FromUtc(2015, 12, 25, 16, 36, 0) },
            };

        [Theory]
        [MemberData(nameof(DuskData))]
        public void Dusk(LocalDate date, Instant i)
        {
            ZonedDateTime expected = new(i, DateTimeZone.Utc);
            var actual = Sun.Dusk(infoFixture.LondonInfo.Observer, date);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
        }

        public static IEnumerable<object[]> DuskNauticalData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2015, 12, 1), Instant.FromUtc(2015, 12, 1, 17, 16, 0) },
                new object[] { new LocalDate(2015, 12, 2), Instant.FromUtc(2015, 12, 2, 17, 16, 0) },
                new object[] { new LocalDate(2015, 12, 3), Instant.FromUtc(2015, 12, 3, 17, 16, 0) },
                new object[] { new LocalDate(2015, 12, 12), Instant.FromUtc(2015, 12, 12, 17, 14, 0) },
                new object[] { new LocalDate(2015, 12, 25), Instant.FromUtc(2015, 12, 25, 17, 19, 0) },
            };

        [Theory]
        [MemberData(nameof(DuskNauticalData))]
        public void Dusk_Nautical(LocalDate date, Instant i)
        {
            ZonedDateTime expected = new(i, DateTimeZone.Utc);
            var actual = Sun.Dusk(infoFixture.LondonInfo.Observer, date, Astral.Library.Depression.Nautical);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
        }

        public static IEnumerable<object[]> NoonData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2015, 12, 1), Instant.FromUtc(2015, 12, 1, 11, 49, 0) },
                new object[] { new LocalDate(2015, 12, 2), Instant.FromUtc(2015, 12, 2, 11, 49, 0) },
                new object[] { new LocalDate(2015, 12, 3), Instant.FromUtc(2015, 12, 3, 11, 50, 0) },
                new object[] { new LocalDate(2015, 12, 12), Instant.FromUtc(2015, 12, 12, 11, 54, 0) },
                new object[] { new LocalDate(2015, 12, 25), Instant.FromUtc(2015, 12, 25, 12, 0, 0) },
            };

        [Theory]
        [MemberData(nameof(NoonData))]
        public void Noon(LocalDate date, Instant i)
        {
            ZonedDateTime expected = new(i, DateTimeZone.Utc);
            var actual = Sun.Noon(infoFixture.LondonInfo.Observer, date);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
        }

        public static IEnumerable<object[]> MidnightData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2016, 2, 18), Instant.FromUtc(2016, 2, 18, 0, 14, 0) },
                new object[] { new LocalDate(2016, 10, 26), Instant.FromUtc(2016, 10, 26, 23, 44, 0) },
            };

        [Theory]
        [MemberData(nameof(MidnightData))]
        public void Midnight(LocalDate date, Instant i)
        {
            ZonedDateTime expected = new(i, DateTimeZone.Utc);
            var actual = Sun.Midnight(infoFixture.LondonInfo.Observer, date);
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
        }

        public static IEnumerable<object[]> TwilightRisingData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2019, 8, 29), Instant.FromUtc(2019, 8, 29, 4, 32, 0), Instant.FromUtc(2019, 8, 29, 5, 7, 0) },
            };

        [Theory]
        [MemberData(nameof(TwilightRisingData))]
        public void Twilight_SunRising(LocalDate date, Instant start, Instant end)
        {
            ZonedDateTime expectedStart = new(start, DateTimeZone.Utc);
            ZonedDateTime expectedEnd = new(end, DateTimeZone.Utc);

            Tuple<ZonedDateTime, ZonedDateTime> actual = Sun.Twilight(infoFixture.LondonInfo.Observer, date, Astral.Library.SunDirection.Rising);
            Assert.True(Library.DateTimesAlmostEqual(expectedStart, actual.Item1));
            Assert.True(Library.DateTimesAlmostEqual(expectedEnd, actual.Item2));
        }

        public static IEnumerable<object[]> TwilightSettingData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2019, 8, 29), Instant.FromUtc(2019, 8, 29, 18, 54, 0), Instant.FromUtc(2019, 8, 29, 19, 30, 0) },
            };

        [Theory]
        [MemberData(nameof(TwilightSettingData))]
        public void Twilight_SunSetting(LocalDate date, Instant start, Instant end)
        {
            ZonedDateTime expectedStart = new(start, DateTimeZone.Utc);
            ZonedDateTime expectedEnd = new(end, DateTimeZone.Utc);

            Tuple<ZonedDateTime, ZonedDateTime> actual = Sun.Twilight(infoFixture.LondonInfo.Observer, date, Astral.Library.SunDirection.Setting);
            Assert.True(Library.DateTimesAlmostEqual(expectedStart, actual.Item1));
            Assert.True(Library.DateTimesAlmostEqual(expectedEnd, actual.Item2));
        }

        public static IEnumerable<object[]> RahukaalamData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2015, 12, 1), Instant.FromUtc(2015, 12, 1, 9, 17, 0), Instant.FromUtc(2015, 12, 1, 10, 35, 0) },
                new object[] { new LocalDate(2015, 12, 2), Instant.FromUtc(2015, 12, 2, 6, 40, 0), Instant.FromUtc(2015, 12, 2, 7, 58, 0) },
            };

        [Theory]
        [MemberData(nameof(RahukaalamData))]
        public void Rahukaalam(LocalDate date, Instant start, Instant end)
        {
            ZonedDateTime expectedStart = new(start, DateTimeZone.Utc);
            ZonedDateTime expectedEnd = new(end, DateTimeZone.Utc);

            Tuple<ZonedDateTime, ZonedDateTime> actual = Sun.Rahukaalam(infoFixture.LondonInfo.Observer, date);
            Assert.True(Library.DateTimesAlmostEqual(expectedStart, actual.Item1));
            Assert.True(Library.DateTimesAlmostEqual(expectedEnd, actual.Item2));
        }

        public static IEnumerable<object[]> GoldenHourRisingData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2015, 12, 1), Instant.FromUtc(2015, 12, 1, 1, 10, 10), Instant.FromUtc(2015, 12, 1, 2, 0, 43) },
                new object[] { new LocalDate(2016, 1, 1), Instant.FromUtc(2016, 1, 1, 1, 27, 46), Instant.FromUtc(2016, 1, 1, 2, 19, 1) },
            };

        [Theory]
        [MemberData(nameof(GoldenHourRisingData))]
        public void GoldenHourRising(LocalDate date, Instant start, Instant end)
        {
            ZonedDateTime expectedStart = new(start, DateTimeZone.Utc);
            ZonedDateTime expectedEnd = new(end, DateTimeZone.Utc);

            Tuple<ZonedDateTime, ZonedDateTime> actual = Sun.GoldenHour(infoFixture.LondonInfo.Observer, date, Astral.Library.SunDirection.Rising);
            Assert.True(Library.DateTimesAlmostEqual(expectedStart, actual.Item1));
            Assert.True(Library.DateTimesAlmostEqual(expectedEnd, actual.Item2));
        }

        public static IEnumerable<object[]> GoldenHourSettingData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2016, 5, 18), Instant.FromUtc(2016, 5, 18, 19, 1, 0), Instant.FromUtc(2016, 5, 18, 20, 17, 0) },
            };

        [Theory]
        [MemberData(nameof(GoldenHourSettingData))]
        public void GoldenHourSetting(LocalDate date, Instant start, Instant end)
        {
            ZonedDateTime expectedStart = new(start, DateTimeZone.Utc);
            ZonedDateTime expectedEnd = new(end, DateTimeZone.Utc);

            Tuple<ZonedDateTime, ZonedDateTime> actual = Sun.GoldenHour(infoFixture.LondonInfo.Observer, date, Astral.Library.SunDirection.Setting);
            Assert.True(Library.DateTimesAlmostEqual(expectedStart, actual.Item1));
            Assert.True(Library.DateTimesAlmostEqual(expectedEnd, actual.Item2));
        }

        public static IEnumerable<object[]> BlueHourRisingData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2016, 5, 19), Instant.FromUtc(2016, 5, 19, 3, 19, 0), Instant.FromUtc(2016, 5, 19, 3, 36, 0) },
            };

        [Theory]
        [MemberData(nameof(BlueHourRisingData))]
        public void BlueHourRising(LocalDate date, Instant start, Instant end)
        {
            ZonedDateTime expectedStart = new(start, DateTimeZone.Utc);
            ZonedDateTime expectedEnd = new(end, DateTimeZone.Utc);

            Tuple<ZonedDateTime, ZonedDateTime> actual = Sun.BlueHour(infoFixture.LondonInfo.Observer, date, Astral.Library.SunDirection.Rising);
            Assert.True(Library.DateTimesAlmostEqual(expectedStart, actual.Item1));
            Assert.True(Library.DateTimesAlmostEqual(expectedEnd, actual.Item2));
        }

        public static IEnumerable<object[]> BlueHourSettingData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2016, 5, 19), Instant.FromUtc(2016, 5, 19, 20, 18, 0), Instant.FromUtc(2016, 5, 19, 20, 35, 0) },
            };

        [Theory]
        [MemberData(nameof(BlueHourSettingData))]
        public void BlueHourSetting(LocalDate date, Instant start, Instant end)
        {
            ZonedDateTime expectedStart = new(start, DateTimeZone.Utc);
            ZonedDateTime expectedEnd = new(end, DateTimeZone.Utc);

            Tuple<ZonedDateTime, ZonedDateTime> actual = Sun.BlueHour(infoFixture.LondonInfo.Observer, date, Astral.Library.SunDirection.Setting);
            Assert.True(Library.DateTimesAlmostEqual(expectedStart, actual.Item1));
            Assert.True(Library.DateTimesAlmostEqual(expectedEnd, actual.Item2));
        }

        public static IEnumerable<object[]> DawnLocalData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2015, 12, 1), Instant.FromUtc(2015, 12, 1, 6, 30, 0) },
                new object[] { new LocalDate(2015, 12, 2), Instant.FromUtc(2015, 12, 2, 6, 31, 0) },
                new object[] { new LocalDate(2015, 12, 3), Instant.FromUtc(2015, 12, 3, 6, 31, 0) },
            };

        [Theory]
        [MemberData(nameof(DawnLocalData))]
        public void Dawn_Local(LocalDate date, Instant i)
        {
            ZonedDateTime expected = new(i, DateTimeZone.Utc);
            var actual = Sun.Dawn(infoFixture.LondonInfo.Observer, date, Timezone: "Asia/Kolkata");
            Assert.True(Library.DateTimesAlmostEqual(expected, actual));
        }
    }
}
