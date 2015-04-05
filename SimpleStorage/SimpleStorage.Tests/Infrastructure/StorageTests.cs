using System.Linq;
using Domain;
using NUnit.Framework;
using SimpleStorage.Infrastructure;

namespace SimpleStorage.Tests.Infrastructure
{
    [TestFixture]
    public class StorageTests
    {
        private OperationLog operationLog;
        private Storage sut;

        [SetUp]
        public void SetUp()
        {
            operationLog = new OperationLog();
            sut = new Storage(operationLog, new ValueComparer());
        }

        [Test]
        public void GetAll_EmptyStorage_ShouldReturnEmptyList()
        {
            var actual = sut.GetAll();
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GetAll_NonEmptyStorage_ShouldReturnAllValues()
        {
            const string id1 = "id1";
            var value1 = new Value {Content = "content1"};
            sut.Set(id1, value1);
            const string id2 = "id2";
            var value2 = new Value {Content = "content2"};
            sut.Set(id2, value2);

            var actual = sut.GetAll().ToArray();

            Assert.That(actual, Has.Some.Matches<ValueWithId>(m => m.Id == id1 && m.Value == value1));
            Assert.That(actual, Has.Some.Matches<ValueWithId>(m => m.Id == id2 && m.Value == value2));
        }

        [Test]
        public void Get_KnownId_ShouldReturnValue()
        {
            const string id = "id";
            var value = new Value {Content = "content"};
            sut.Set(id, value);

            var actual = sut.Get(id);

            Assert.That(actual, Is.EqualTo(value));
        }

        [Test]
        public void Get_UnknownId_ShouldReturnNull()
        {
            var actual = sut.Get("unknownId");
            Assert.That(actual, Is.Null);
        }

        [Test]
        public void Set_NonExistingId_ShouldCreate()
        {
            const string id = "id";
            var value = new Value {Content = "content", Revision = 0};

            sut.Set(id, value);

            var actual = sut.Get(id);
            Assert.That(actual, Is.EqualTo(value));
        }

        [Test]
        public void Set_NonExistingId_ShouldWriteToOperationLog()
        {
            const string id = "id";
            var value = new Value {Content = "content", Revision = 0};

            sut.Set(id, value);

            var actual = operationLog.Read(0, 1).Single();
            Assert.That(actual.Id, Is.EqualTo(id));
            Assert.That(actual.Value, Is.EqualTo(value));
        }

        [Test]
        public void Set_NonExistingId_ShouldReturnTrue()
        {
            const string id = "id";
            var value = new Value {Content = "content", Revision = 0};

            var actual = sut.Set(id, value);

            Assert.That(actual, Is.True);
        }

        [Test]
        public void Set_StorageContainsNewerValue_ShouldNotOverwrite()
        {
            const string id = "id";
            var value = new Value {Content = "content", Revision = 2};
            sut.Set(id, value);
            var anotherValue = new Value {Content = "anotherContent", Revision = 1};

            sut.Set(id, anotherValue);

            var actual = sut.Get(id);
            Assert.That(actual, Is.EqualTo(value));
        }

        [Test]
        public void Set_StorageContainsNewerValue_ShouldNotWriteToOperationLog()
        {
            const string id = "id";
            var value = new Value {Content = "content", Revision = 2};
            sut.Set(id, value);
            var anotherValue = new Value {Content = "anotherContent", Revision = 1};

            sut.Set(id, anotherValue);

            var actual = operationLog.Read(0, 2).Last();
            Assert.That(actual.Value, Is.EqualTo(value));
        }

        [Test]
        public void Set_StorageContainsNewerValue_ShouldReturnFalse()
        {
            const string id = "id";
            var value = new Value {Content = "content", Revision = 2};
            sut.Set(id, value);
            var anotherValue = new Value {Content = "anotherContent", Revision = 1};

            var actual = sut.Set(id, anotherValue);

            Assert.That(actual, Is.False);
        }

        [Test]
        public void Set_StorageContainsOlderValue_ShouldOverwrite()
        {
            const string id = "id";
            var oldValue = new Value {Content = "oldContent", Revision = 0};
            sut.Set(id, oldValue);
            var newValue = new Value {Content = "newContent", Revision = 1};

            sut.Set(id, newValue);

            var actual = sut.Get(id);
            Assert.That(actual, Is.EqualTo(newValue));
        }

        [Test]
        public void Set_StorageContainsOlderValue_ShouldWriteToOperationLog()
        {
            const string id = "id";
            var oldValue = new Value {Content = "oldContent", Revision = 0};
            sut.Set(id, oldValue);
            var newValue = new Value {Content = "newContent", Revision = 1};

            sut.Set(id, newValue);

            var actual = operationLog.Read(0, 2).Last();
            Assert.That(actual.Value, Is.EqualTo(newValue));
        }

        [Test]
        public void Set_StorageContainsOlderValue_ShouldReturnTrue()
        {
            const string id = "id";
            var oldValue = new Value {Content = "oldContent", Revision = 0};
            sut.Set(id, oldValue);
            var newValue = new Value {Content = "newContent", Revision = 1};

            var actual = sut.Set(id, newValue);

            Assert.That(actual, Is.True);
        }

        [Test]
        public void RemoveAll_Always_ShouldRemoveAllData()
        {
            sut.Set("id", new Value {Content = "content"});

            sut.RemoveAll();

            var actual = sut.GetAll();
            Assert.That(actual, Is.Empty);
        }
    }
}