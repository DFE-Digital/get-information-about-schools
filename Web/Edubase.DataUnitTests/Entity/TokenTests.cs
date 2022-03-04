using System;
using Edubase.Common.Formatting;
using Edubase.Data.Entity;
using Xunit;

namespace Edubase.DataTests.Entity.UnitTests
{
    public class TokenTests
    {
        [Fact]
        public void ValidateTokenPartitionsKeysAreAtLeast4Digits()
        {
            var testDays = 3650;

            for (var i = 0; i < testDays; i++)
            {
                var d = DateTime.Now.AddDays(i);

                var t = new Token(d);
                Assert.True(t.PartitionKey.Length > 3);
            }
        }
        
        [Fact]
        public void ValidateTokensCanBeDecoded()
        {
            var testDays = 3650;
            for (var i = 0; i < testDays; i++)
            {
                var d = DateTime.Now.AddDays(i);

                var encode = Base62.FromDate(d);
                Assert.True(encode != null);

                var decode = Base62.Decode(encode);
                Assert.True(decode == int.Parse(string.Concat(d.Year, d.Month, d.Day)));
            }
        }
    }
}
