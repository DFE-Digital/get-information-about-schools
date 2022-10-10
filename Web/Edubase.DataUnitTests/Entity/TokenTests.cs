using System;
using Edubase.Common.Formatting;
using Edubase.Data.Entity;
using Xunit;

namespace Edubase.DataTests.Entity.UnitTests
{
    public class TokenTests
    {
        [Fact]
        public void ValidateTokenPartitionsKeysAreOnly4Digits()
        {
            var testDays = 36500;

            for (var i = -3560; i < testDays; i += 10)
            {
                var d = DateTime.Now.AddDays(i);

                var t = new Token(d);
                Assert.Equal(4, t.PartitionKey.Length);
            }
        }
    }
}
