using Edubase.Data.Repositories;
using Faker;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edubase.IntegrationTest.Repos
{
    [TestFixture]
    public class GlossaryRepositoryTest
    {
        [Test]
        public async Task InsertGlossaryItems()
        {
            var subject = new GlossaryRepository();
            for (int i = 0; i < 100; i++)
            {
                await subject.CreateAsync(new Data.Entity.GlossaryItem
                {
                    Content = Lorem.Sentence(100),
                    Title = Lorem.Sentence(2)
                });
            }
        }

        [Test]
        public async Task QueryGlossaryItems()
        {
            var subject = new GlossaryRepository();
            var result = await subject.GetAllAsync(100);
            Assert.AreEqual(100, result.Items.Count());
            foreach (var item in result.Items)
            {
                Assert.IsNotEmpty(item.Content);
            }
        }

        [Test]
        public async Task DeleteGlossaryItems()
        {
            var subject = new GlossaryRepository();
            var result = await subject.GetAllAsync(100);
            foreach (var item in result.Items)
            {
                await subject.DeleteAsync(item.RowKey);
            }
        }
    }
}
