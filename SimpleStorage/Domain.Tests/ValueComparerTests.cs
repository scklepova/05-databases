using NUnit.Framework;

namespace Domain.Tests
{
    [TestFixture]
    public class ValueComparerTests
    {
        private ValueComparer sut;

        [SetUp]
        public void SetUp()
        {
            sut = new ValueComparer();
        }

        [Test]
        public void Compare_AllNulls_ShouldReturnZero()
        {
            var actual = sut.Compare(null, null);
            Assert.That(actual, Is.EqualTo(0));
        }

        [Test]
        public void Compare_OnlyFirstParameterIsNull_ShouldReturnMinusOne()
        {
            var actual = sut.Compare(null, new Value {Content = "lalala"});
            Assert.That(actual, Is.EqualTo(-1));
        }

        [Test]
        public void Compare_OnlySecondParameterIsNull_ShouldReturnOne()
        {
            var actual = sut.Compare(new Value {Content = "qqq"}, null);
            Assert.That(actual, Is.EqualTo(1));
        }

        [Test]
        [TestCase(1, 0, 1)]
        [TestCase(1, 2, -1)]
        public void Compare_DistinctRevisions_ShouldUseThem(long revision1, long revision2, int expected)
        {
            var first = new Value {Content = "qqq", Revision = revision1};
            var second = new Value {Content = "lalala", Revision = revision2};

            var actual = sut.Compare(first, second);

            Assert.That(actual, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(null, null, 0)]
        [TestCase("qqq", null, 1)]
        [TestCase(null, "qqq", -1)]
        [TestCase("lalala", "qqq", -1)]
        [TestCase("lalala", "lalala", 0)]
        public void Compare_SameRevisions_ShouldUseContent(string content1, string content2, int expected)
        {
            var first = new Value {Content = content1, Revision = 1};
            var second = new Value {Content = content2, Revision = 1};

            var actual = sut.Compare(first, second);

            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}