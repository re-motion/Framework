using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.NonPersistent.Model;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.NonPersistent.Model
{
  [TestFixture]
  public class NonPersistentStoragePropertyTest
  {
    [Test]
    public void GetInstance_ReturnsSameInstance ()
    {
      var instance = NonPersistentStorageProperty.Instance;

      Assert.That (instance, Is.Not.Null);
      Assert.That (instance, Is.SameAs (NonPersistentStorageProperty.Instance));
    }
  }
}