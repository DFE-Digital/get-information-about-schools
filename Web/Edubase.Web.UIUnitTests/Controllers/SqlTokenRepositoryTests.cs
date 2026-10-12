using System;
using System.Threading.Tasks;
using Edubase.Data.Entity;
using Edubase.Data.Repositories;
using Edubase.Data.Repositories.EF;
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
            Assert.IsAssignableFrom<ITokenRepository>(new SqlTokenRepository(null));
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
            Assert.Throws<ArgumentException>(() => SqlTokenRepository.SplitId(id, out var partitionKey, out var rowKey));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("a")]
        [InlineData("abcd")]
        public void Get_ThrowsArgumentException_WhenIdIsInvalid(string id)
        {
            var sut = new SqlTokenRepository(null);

            Assert.Throws<ArgumentException>(() => sut.Get(id));
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("abcd")]
        public async Task GetAsync_ThrowsArgumentException_WhenIdIsInvalid(string id)
        {
            var sut = new SqlTokenRepository(null);

            await Assert.ThrowsAsync<ArgumentException>(() => sut.GetAsync(id));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("abcd")]
        public async Task DeleteAsync_ThrowsArgumentException_WhenIdIsInvalid(string id)
        {
            var sut = new SqlTokenRepository(null);

            await Assert.ThrowsAsync<ArgumentException>(() => sut.DeleteAsync(id));
        }
    }
}
