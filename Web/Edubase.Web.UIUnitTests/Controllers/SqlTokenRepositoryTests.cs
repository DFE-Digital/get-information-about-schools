using System;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Web.UI.Controllers.Api;
using Edubase.Web.UI.Models;
using Xunit;

namespace Edubase.Web.UIUnitTests.Controllers
{
    public class SqlTokenRepositoryTests
    {
        [Fact]
        public void SqlTokenRepository_ImplementsITokenRepository()
        {
            Assert.IsAssignableFrom<ITokenRepository>(new SqlTokenRepository());
        }

        [Theory]
        [InlineData("ABCD1234", "ABCD", "1234")]
        [InlineData("ABCDx", "ABCD", "x")]
        [InlineData("0000abcdefghi", "0000", "abcdefghi")]
        public void SplitId_SplitsFirstFourCharactersAsPartitionKey(string id, string expectedPartitionKey,
            string expectedRowKey)
        {
            SqlTokenRepository.SplitId(id, out var partitionKey, out var rowKey);

            Assert.Equal(expectedPartitionKey, partitionKey);
            Assert.Equal(expectedRowKey, rowKey);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("abcd")]
        public void SplitId_ThrowsArgumentException_WhenIdIsInvalid(string id)
        {
            Assert.Throws<ArgumentException>(() => SqlTokenRepository.SplitId(id, out _, out _));
        }

        [Fact]
        public void ToSqlToken_MapsAllFields()
        {
            var token = new Token
            {
                PartitionKey = "ABCD",
                RowKey = "1234",
                Data = "fs=data"
            };

            var row = SqlTokenRepository.ToSqlToken(token);

            Assert.Equal("ABCD", row.PartitionKey);
            Assert.Equal("1234", row.RowKey);
            Assert.Equal("fs=data", row.Data);
        }

        [Fact]
        public void ToSqlToken_ConvertsNullPartitionKeyToEmptyString()
        {
            var token = new Token
            {
                PartitionKey = null,
                RowKey = "1234",
                Data = "fs=data"
            };

            var row = SqlTokenRepository.ToSqlToken(token);
            Assert.Equal(string.Empty, row.PartitionKey);
        }

        [Fact]
        public void FromSqlToken_MapsAllFields_AndReconstructsId()
        {
            var row = new SqlToken
            {
                PartitionKey = "ABCD",
                RowKey = "1234",
                Data = "fs=data"
            };

            var token = SqlTokenRepository.FromSqlToken(row);

            Assert.Equal("ABCD", token.PartitionKey);
            Assert.Equal("1234", token.RowKey);
            Assert.Equal("fs=data", token.Data);
            Assert.Equal("ABCD1234", token.Id);
        }

        [Fact]
        public void Token_RoundTripsThroughSqlToken_PreservingId()
        {
            var original = new Token("formstate=data");

            var row = SqlTokenRepository.ToSqlToken(original);
            var result = SqlTokenRepository.FromSqlToken(row);

            Assert.Equal(original.Id, result.Id);
            Assert.Equal(original.Data, result.Data);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("abcd")]
        public void Get_ThrowsArgumentException_WhenIdIsInvalid(string id)
        {
            var sut = new SqlTokenRepository();

            Assert.Throws<ArgumentException>(() => sut.Get(id));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("abcd")]
        public async Task GetAsync_ThrowsArgumentException_WhenIdIsInvalid(string id)
        {
            var sut = new SqlTokenRepository();

            await Assert.ThrowsAsync<ArgumentException>(() => sut.GetAsync(id));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("abcd")]
        public async Task DeleteAsync_ThrowsArgumentException_WhenIdIsInvalid(string id)
        {
            var sut = new SqlTokenRepository();

            await Assert.ThrowsAsync<ArgumentException>(() => sut.DeleteAsync(id));
        }
    }
}
