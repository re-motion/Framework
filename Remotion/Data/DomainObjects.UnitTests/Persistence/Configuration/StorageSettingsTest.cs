using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration
{
  [TestFixture]
  public class StorageSettingsTest
  {


    [Test]
    public void GetDefaultStorageProviderDefinition_WithDefinition ()
    {
      var defaultStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("stub");
      var providerCollection = new StorageProviderDefinition [] { defaultStorageProviderDefinition };

      var storageSettings = new StorageSettings(defaultStorageProviderDefinition, providerCollection, null);

      var output = storageSettings.GetDefaultStorageProviderDefinition();

      Assert.That(output, Is.EqualTo(defaultStorageProviderDefinition));
    }

    [Test]
    public void GetDefaultStorageProviderDefinition_WithNullDefinition ()
    {
      var stubStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("stub");
      var providerCollection = new StorageProviderDefinition [] { stubStorageProviderDefinition };

      var storageSettings = new StorageSettings(null, providerCollection, null);

      var output = storageSettings.GetDefaultStorageProviderDefinition();

      Assert.That(output, Is.EqualTo(null));
    }

    [Test]
    public void GetStorageProviderDefinition_WithClassDefinitionAndGroupType_ReturnsStorageProviderDefinition ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Dummy), baseClass: null);

      var providerCollection = new List<StorageProviderDefinition>();

      var groupName = TypeUtility.GetPartialAssemblyQualifiedName(typeof(Dummy));
      var providerDefinition = new UnitTestStorageProviderStubDefinition(groupName);
      providerCollection.Add(providerDefinition);

      var storageGroup = new ConfigurationElementCollection<StorageGroupElement>();
      var storageElement = new StorageGroupElement(new FirstStorageGroupAttribute(), "DummyProviderName");
      storageGroup.Add(storageElement);

      var settings = new StorageSettings(null, providerCollection, storageGroup);

      var output = settings.GetStorageProviderDefinition(classDefinition);

      Assert.That(output, Is.EqualTo(providerDefinition));
    }

    [FirstStorageGroup]
    private class Dummy
    {
    }
  }
}
