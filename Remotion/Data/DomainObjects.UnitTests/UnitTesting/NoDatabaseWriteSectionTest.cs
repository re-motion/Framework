using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.UnitTesting
{
  [TestFixture]
  public class NoDatabaseWriteSectionTest : StandardMappingTest
  {
    [Test]
    public void Dispose_WithNoChanges_ThrowsNothing ()
    {
      var section = DatabaseAgent.OpenNoDatabaseWriteSection();
      Assert.That(() => section.Dispose(), Throws.Nothing);
    }

    [Test]
    public void Dispose_WithChange_ThrowsInvalidOperationException ()
    {
      var section = DatabaseAgent.OpenNoDatabaseWriteSection();

      using (var clientTransaction = ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var newObject = CreateClassWithAllDataTypes();
        newObject.PopulateMandatoryProperties();

        clientTransaction.Commit();
      }

      Assert.That(
          () => section.Dispose(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Database changed while NoDatabaseWriteSection was open."));
    }

    private ClassWithAllDataTypes CreateClassWithAllDataTypes ()
    {
      ClassWithAllDataTypes newObject = ClassWithAllDataTypes.NewObject();
      newObject.DateProperty = DateTime.Now;
      newObject.DateTimeProperty = DateTime.Now;
      newObject.StringProperty = "value";
      newObject.StringPropertyWithoutMaxLength = "value";
      newObject.TransactionOnlyStringProperty = "value";
      newObject.BinaryProperty = new byte[10];
      newObject.TransactionOnlyBinaryProperty = new byte[10];
      return newObject;
    }
  }
}
