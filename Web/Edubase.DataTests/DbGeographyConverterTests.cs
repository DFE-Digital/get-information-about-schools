using Xunit;
using Edubase.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Spatial;

namespace Edubase.Data.Tests
{
    public class DbGeographyConverterTests
    {
        [Theory()]
        [InlineData(typeof(DbGeography), true)]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(object), false)]
        public void CanConvertTest(Type type, bool expected)
        {
            var converter = new DbGeographyConverter();

            var result = converter.CanConvert(type);

            Assert.Equal(expected, result);
        }
    }
}
