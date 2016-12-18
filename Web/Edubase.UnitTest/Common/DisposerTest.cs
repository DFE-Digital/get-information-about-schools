using Edubase.Common;
using Edubase.UnitTest.Mocks;
using NUnit.Framework;
using System.Linq;
using MoreLinq;

namespace Edubase.UnitTest.Common
{
    [TestFixture]
    public class DisposerTest
    {
        [Test]
        public void ObjectIsDisposed()
        {
            var subject = new Disposable();
            using (Disposer.Capture(subject)) { }
            Assert.IsTrue(subject.disposedValue);
        }

        [Test]
        public void ObjectsAreDisposed()
        {
            var subjects = new[] { new Disposable(), new Disposable(), new Disposable(), new Disposable(), new Disposable() };
            using (Disposer.Capture(subjects)) { }
            subjects.ForEach(x => Assert.IsTrue(x.disposedValue));
        }

        [Test]
        public void UsingInvokesMethodsInRightOrder()
        {
            string test = "";
            Disposer.Using(() => new Disposable(), x => test += "1", x => test += "3", x => test += "2");
            Assert.AreEqual("123", test);
        }

    }
}
