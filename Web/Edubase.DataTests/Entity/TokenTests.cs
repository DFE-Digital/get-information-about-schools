using System;
using Edubase.Data.Entity;
using Xunit;

namespace Edubase.DataTests.Entity
{
    public class TokenTests
    {
        [Fact]
        public void ValidateTokenPartitionsKeysAreAll4Digits()
        {
            //TODO: THis needs reviewing to a fixed set of dates that covers a wider range of times
            var testDays = (new DateTime(2027, 12, 31) - DateTime.Now).TotalDays;

            for (var i = 0; i < testDays; i++)
            {
                var d = DateTime.Now.AddDays(i);

                var t = new Token(d);
                Assert.Equal(4, t.PartitionKey.Length);
            }
        }
    }
}
