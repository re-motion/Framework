using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class DBStorageGroupAttributeTest
  {
    [Test]
    public void Initialize ()
    {
      var attribute = new DBStorageGroupAttribute();

      Assert.That(attribute.DefaultStorageClass, Is.EqualTo(DefaultStorageClass.Persistent));
    }
  }
}