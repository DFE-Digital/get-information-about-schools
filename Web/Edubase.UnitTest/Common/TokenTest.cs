using Edubase.Common;
using Edubase.UnitTest.Mocks;
using NUnit.Framework;
using System.Linq;
using MoreLinq;
using Edubase.Data.Entity;
using System;

namespace Edubase.UnitTest.Common
{
    [TestFixture]
    public class TokenTest
    {
        [Test]
        public void ValidateTokenPartitionsKeysAreAll4Digits()
        {
            var testDays = (new DateTime(2027, 12, 31) - DateTime.Now).TotalDays;

            for (int i = 0; i < testDays; i++)
            {
                var d = DateTime.Now.AddDays(i);
                try
                {
                    var t = new Token(d);
                    Assert.That(t.PartitionKey.Length, Is.EqualTo(4));
                }
                catch (Exception ex)
                {
                    Assert.Fail($"Failed with date {d} and exception: {ex}");
                }
            }   
        }
    }
}
