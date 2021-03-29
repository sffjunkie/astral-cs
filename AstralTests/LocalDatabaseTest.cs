using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SFFJunkie.Astral.Test
{
    public class LocalDatabaseTest
    {
        [Fact]
        public void LocalDatabase_New()
        {
            LocalDatabase db = new();
            Assert.True(db.Groups.ContainsKey("Asia"));
            Assert.True(db.Groups["Asia"].Locations.Count > 0);
        }

        [Fact]
        public void LocalDatabase_Lookup()
        {
            LocalDatabase db = new();
            var loc = db.Lookup("Abu Dhabi");
            Assert.True(loc != null);
            Assert.Equal("UAE", loc.Region);
        }

        [Fact]
        public void LocalDatabase_LookupWithRegion_NotFirst()
        {
            LocalDatabase db = new();
            var loc = db.Lookup("Abu Dhabi,United Arab Emirates");
            Assert.True(loc != null);
            Assert.Equal("United Arab Emirates", loc.Region);
        }

        [Fact]
        public void LocalDatabase_LookupWithRegion_First()
        {
            LocalDatabase db = new();
            var loc = db.Lookup("Abu Dhabi,UAE");
            Assert.True(loc != null);
            Assert.Equal("UAE", loc.Region);
        }
    }
}
