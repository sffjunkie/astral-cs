using SFFJunkie.Astral.Test.Fixture;
using NodaTime;
using System.Collections.Generic;
using Xunit;

namespace SFFJunkie.Astral.Test.Private
{
    [Collection("LocationInfo collection")]
    public class SunPrivateTest
    {
        private readonly LocationInfoFixture locationInfo;

        public SunPrivateTest(LocationInfoFixture fixture)
        {
            locationInfo = fixture;
        }

        [Theory]
        [InlineData(2455927.5, 0.119986311)]
        [InlineData(2456293.5, 0.130006845)]
        [InlineData(2456444.5, 0.134140999)]
        [InlineData(2402998.5, -1.329130732)]
        [InlineData(2890153.5, 12.00844627)]
        public void JulianCentury_FromJulianDay(double jc, double expected)
        {
            Assert.Equal(expected, Astral.Private.JulianDayToJulianCentury(jc), 7);
        }

        [Theory]
        [InlineData(0.119986311, 2455927.5)]
        [InlineData(0.130006845, 2456293.5)]
        [InlineData(0.134140999, 2456444.5)]
        [InlineData(-1.329130732, 2402998.5)]
        [InlineData(12.00844627, 2890153.5)]
        public void JulianDay_FromJulianCentury(double jd, double expected)
        {
            double actual = Astral.Private.JulianCenturyToJulianDay(jd);
            Assert.Equal(expected, actual, 2);
        }

        [Theory]
        [InlineData(-1.329130732, 310.7374254)]
        [InlineData(12.00844627, 233.8203529)]
        [InlineData(0.184134155, 69.43779106)]
        public void GeometricMeanLongitudeOfTheSun(double jc, double expected)
        {
            double actual = Astral.Private.GeometricMeanLongitudeSun(jc);
            Assert.Equal(expected, actual, 2);
        }

        [Theory]
        [InlineData(0.119986311, 4676.922342)]
        [InlineData(12.00844627, 432650.1681)]
        [InlineData(0.184134155, 6986.1838)]
        public void GeometricMeanAnomolyOfTheSun(double jc, double expected)
        {
            double actual = Astral.Private.GeometricMeanAnomalySun(jc);
            Assert.Equal(expected, actual, 2);
        }

        [Theory]
        [InlineData(0.119986311, 0.016703588)]
        [InlineData(12.00844627, 0.016185564)]
        [InlineData(0.184134155, 0.016700889)]
        public void EccentricLocationEarthOrbit(double jc, double expected)
        {
            double actual = Astral.Private.EccentricLocationEarthOrbit(jc);
            Assert.Equal(expected, actual, 6);
        }

        [Theory]
        [InlineData(0.119986311, -0.104951648)]
        [InlineData(12.00844627, -1.753028843)]
        [InlineData(0.184134155, 1.046852316)]
        public void SunEquationOfCenter(double jc, double expected)
        {
            double actual = Astral.Private.SunEquationOfCenter(jc);
            Assert.Equal(expected, actual, 5);
        }

        [Theory]
        [InlineData(0.119986311, 279.9610686)]
        [InlineData(12.00844627, 232.0673358)]
        [InlineData(0.184134155, 70.48465428)]
        public void SunTrueLongitude(double jc, double expected)
        {
            double actual = Astral.Private.SunTrueLongitude(jc);
            Assert.Equal(expected, actual, 3);
        }

        [Theory]
        [InlineData(0.119986311, 4676.817391)]
        [InlineData(12.00844627, 432648.4151)]
        [InlineData(0.184134155, 6987.230663)]
        public void SunTrueAnomoly(double jc, double expected)
        {
            double actual = Astral.Private.SunTrueAnomoly(jc);
            Assert.Equal(expected, actual, 3);
        }

        [Theory]
        [InlineData(0.119986311, 0.983322329)]
        [InlineData(12.00844627, 0.994653382)]
        [InlineData(0.184134155, 1.013961204)]
        public void SunRadVector(double jc, double expected)
        {
            double actual = Astral.Private.SunRadVector(jc);
            Assert.Equal(expected, actual, 3);
        }

        [Theory]
        [InlineData(0.119986311, 279.95995849827)]
        [InlineData(12.00844627, 232.065823531804)]
        [InlineData(0.184134155, 70.475244256027)]
        public void SunApparentLongitude(double jc, double expected)
        {
            double actual = Astral.Private.SunApparentLongitude(jc);
            Assert.Equal(expected, actual, 3);
        }

        [Theory]
        [InlineData(0.119986311, 23.4377307876356)]
        [InlineData(12.00844627, 23.2839797200388)]
        [InlineData(0.184134155, 23.4368965974579)]
        public void MeanObliquityOfEcliptic(double jc, double expected)
        {
            double actual = Astral.Private.MeanObliquityOfEcliptic(jc);
            Assert.Equal(expected, actual, 3);
        }

        [Theory]
        [InlineData(0.119986311, 23.4369810410121)]
        [InlineData(12.00844627, 23.2852236361575)]
        [InlineData(0.184134155, 23.4352890293474)]
        public void ObliquityCorrection(double jc, double expected)
        {
            double actual = Astral.Private.ObliquityCorrection(jc);
            Assert.Equal(expected, actual, 3);
        }

        [Theory]
        [InlineData(0.119986311, -79.16480352)]
        [InlineData(12.00844627, -130.3163904)]
        [InlineData(0.184134155, 68.86915896)]
        public void SunRightAscension(double jc, double expected)
        {
            double actual = Astral.Private.SunRightAscension(jc);
            Assert.Equal(expected, actual, 3);
        }

        [Theory]
        [InlineData(0.119986311, -23.06317068)]
        [InlineData(12.00844627, -18.16694394)]
        [InlineData(0.184134155, 22.01463552)]
        public void SunDeclination(double jc, double expected)
        {
            double actual = Astral.Private.SunDeclination(jc);
            Assert.Equal(expected, actual, 3);
        }

        [Theory]
        [InlineData(0.119986311, -3.078194825)]
        [InlineData(12.00844627, 16.58348133)]
        [InlineData(0.184134155, 2.232039737)]
        public void EquationOfTime(double jc, double expected)
        {
            double actual = Astral.Private.EquationOfTime(jc);
            Assert.Equal(expected, actual, 3);
        }

        public static IEnumerable<object[]> HourAngleData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2012, 1, 1), 1.03555238 },
                new object[] { new LocalDate(3200, 11, 14), 1.172253118 },
                new object[] { new LocalDate(2018, 6, 1), 2.133712555 },
            };

        [Theory]
        [MemberData(nameof(HourAngleData))]
        public void HourAngle(LocalDate date, double expected)
        {
            var jd = Astral.Library.JulianDay(date);
            var jc = Astral.Private.JulianDayToJulianCentury(jd);
            var decl = Astral.Private.SunDeclination(jc);

            double actual = Astral.Private.HourAngle(locationInfo.LondonInfo.Latitude, decl, 90.8333, Astral.Library.SunDirection.Rising);
            Assert.Equal(expected, actual, 4);
        }

        [Fact]
        public void AdjustToHorizon()
        {
            var actual = Astral.Private.AdjustToHorizon(12000);
            Assert.Equal(3.517744168209966, actual, 5);
        }
    }
}