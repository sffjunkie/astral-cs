using SFFJunkie.Astral;
using NodaTime;
using System.Collections.Generic;
using Xunit;

namespace SFFJunkie.Astral.Test
{
    public class MoonTest
    {
        public static IEnumerable<object[]> MoonData =>
            new List<object[]>
            {
                new object[] { new LocalDate(2015, 12, 1), 19.477889 },
                new object[] { new LocalDate(2015, 12, 2), 20.333444 },
                new object[] { new LocalDate(2015, 12, 3), 21.189000 },
                new object[] { new LocalDate(2014, 12, 1), 9.0556666 },
                new object[] { new LocalDate(2014, 12, 2), 10.066777 },
                new object[] { new LocalDate(2014, 1, 1),  27.955666 }
            };

        [Theory]
        [MemberData(nameof(MoonData))]
        public void Moon_Phase(LocalDate date, double expected)
        {
            var actual = Moon.Phase(date);
            Assert.Equal(expected, actual, 3);
        }

    }
}
