using Xunit;
using System;

namespace Edubase.Common.Formatting.Tests;

public class Base62Tests
{
    [Fact]
    public void ValidateDatesCanBeDecodedAfterEncoding()
    {
        var testDays = 36500;
        for (var i = -3560; i < testDays; i += 10)
        {
            var d = DateTime.Now.AddDays(i);

            var encode = Base62.FromDate(d);
            Assert.True(encode != null);

            var decode = Base62.Decode(encode);
            Assert.True(decode == int.Parse(string.Concat(d.Year, d.Month, d.Day)));
        }
    }
}
