using SFFJunkie.Astral;
using System;
using Xunit;

namespace SFFJunkie.Astral.Test
{
    public class ObserverTest
    {
        [Fact]
        public void Observer_New_DefaultProperties()
        {
            Observer obs = new();
            Assert.Equal(51.4733, obs.Latitude);
            Assert.Equal(-0.0008333, obs.Longitude);
            Assert.Equal(0.0, obs.Elevation);
            Assert.Equal(0.0, obs.DistanceToFeature);
        }

        [Fact]
        public void Observer_New_WithDoubleArgs()
        {
            Observer obs = new(10, 20, 30, 40);
            Assert.Equal(10.0, obs.Latitude);
            Assert.Equal(20.0, obs.Longitude);
            Assert.Equal(30.0, obs.Elevation);
            Assert.Equal(40.0, obs.DistanceToFeature);
        }

        [Fact]
        public void Observer_New_WithStringArgsThatAreDoubles()
        {
            Observer obs = new("10.0", "20.0", 0);
            Assert.Equal(10.0, obs.Latitude);
            Assert.Equal(20.0, obs.Longitude);
        }

        [Fact]
        public void Observer_New_Degrees()
        {
            Observer obs = new("10°", "20°", 0);
            Assert.Equal(10.0, obs.Latitude);
            Assert.Equal(20.0, obs.Longitude);
        }

        [Fact]
        public void Observer_New_DegreesMinutes()
        {
            Observer obs = new("10°30'", "20°45'", 0);
            Assert.Equal(10.5, obs.Latitude);
            Assert.Equal(20.75, obs.Longitude);
        }

        [Fact]
        public void Observer_New_DegreesMinutesSeconds()
        {
            Observer obs = new("10°30'30\"", "20°45'30\"", 0);
            Assert.Equal(10.5083333, obs.Latitude, 6);
            Assert.Equal(20.7583333, obs.Longitude, 6);
        }

        [Fact]
        public void Observer_New_DegreesAndDirectionN()
        {
            Observer obs = new("10°30'30\"N", "20°45'30\"", 0);
            Assert.Equal(10.5083333, obs.Latitude, 6);
        }

        [Fact]
        public void Observer_New_DegreesAndDirectionS()
        {
            Observer obs = new("10°30'30\"S", "20°45'30\"", 0);
            Assert.Equal(-10.5083333, obs.Latitude, 6);
        }

        [Fact]
        public void Observer_New_DegreesAndDirectionE()
        {
            Observer obs = new("10°30'30\"", "20°45'30\"E", 0);
            Assert.Equal(20.7583333, obs.Longitude, 6);
        }

        [Fact]
        public void Observer_New_DegreesAndDirectionW()
        {
            Observer obs = new("10°30'30\"", "20°45'30\"W", 0);
            Assert.Equal(-20.7583333, obs.Longitude, 6);
        }

        [Fact]
        public void Observer_New_LatitudeOutsideLimits()
        {
            Observer obs = new("91°30'30\"", "20°45'30\"W", 0);
            Assert.Equal(90.0, obs.Latitude);
        }

        [Fact]
        public void Observer_New_LongitudeOutsideLimits()
        {
            Observer obs = new("41°30'30\"", "180°45'30\"W", 0);
            Assert.Equal(-180.0, obs.Longitude);
        }
    }
}
