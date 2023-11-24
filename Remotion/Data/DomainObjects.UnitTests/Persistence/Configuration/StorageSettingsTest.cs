using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration
{
  [TestFixture]
  public class StorageSettingsTest
  {
    [FirstStorageGroup]
    private class Dummy
    {
    }

    [Test]
    public void DefaultStorageProviderDefinition_WithDefaultStorageProviderDefinition_ReturnsDefaultProviderDefinition ()
    {
      var defaultProvider = new UnitTestStorageProviderStubDefinition("default");
      var storageSettings = new StorageSettings(defaultProvider, new[] { defaultProvider });

      Assert.That(storageSettings.DefaultStorageProviderDefinition, Is.EqualTo(defaultProvider));
    }

    [Test]
    public void DefaultStorageProviderDefinition_WithNullDefaultStorageProviderDefinition_ReturnsNull ()
    {
      var stubProvider = new UnitTestStorageProviderStubDefinition("default");
      var storageSettings = new StorageSettings(null, new[] { stubProvider });

      Assert.That(storageSettings.DefaultStorageProviderDefinition, Is.Null);
    }

    [Test]
    public void StorageProviderDefinition ()
    {
      var stubProvider = new UnitTestStorageProviderStubDefinition("default");
      var storageSettings = new StorageSettings(null, new[] { stubProvider });

      Assert.That(storageSettings.StorageProviderDefinitions, Is.EquivalentTo(new [] {stubProvider}));
    }

    [Test]
    public void StorageSettings_WithNonUniqueStorageProvider_ThrowsConfigurationException ()
    {
      var uniqueStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("unique");
      var nonUniqueStorageProviderDefinition1 = new UnitTestStorageProviderStubDefinition("stub");
      var nonUniqueStorageProviderDefinition2 = new UnitTestStorageProviderStubDefinition("stub");

      var storageProviderCollection = new[] { uniqueStorageProviderDefinition, nonUniqueStorageProviderDefinition1, nonUniqueStorageProviderDefinition2 };

      Assert.That(
          () => new StorageSettings(uniqueStorageProviderDefinition, storageProviderCollection),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.EqualTo("There were storage providers with non unique ids defined."));
    }

    [Test]
    public void StorageSettings_WithStorageGroupContainedInSeveralStorageProvider_ThrowsConfigurationException ()
    {
      var storageProvider1 = new UnitTestStorageProviderStubDefinition("unique", new[] { typeof(Dummy) });
      var storageProvider2 = new UnitTestStorageProviderStubDefinition("otherUnique", new[] { typeof(Dummy) });

      var storageProviderCollection = new[] { storageProvider1, storageProvider2 };

      Assert.That(
          () => new StorageSettings(storageProvider1, storageProviderCollection),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.EqualTo($"The storage group with type '{typeof(Dummy)}' is contained in more than one storage provider."));
    }


    [Test]
    public void GetStorageProviderDefinition_ClassWithoutStorageGroupType_NoDefaultStorageProviderDefinitionDefined ()
    {
      var defaultStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("stub");
      var providerCollection = new StorageProviderDefinition[] { defaultStorageProviderDefinition };
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(storageGroupType: typeof(Dummy));

      var storageSettings = new StorageSettings(null, providerCollection);
      Assert.That(
          () => storageSettings.GetStorageProviderDefinition(classDefinition),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.Contains("Missing default storage provider."));
    }

    [Test]
    public void GetStorageProviderDefinition_ClassWithoutStorageGroupType_DefaultStorageProviderDefinitionDefined ()
    {
      var defaultStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("stub");
      var providerCollection = new StorageProviderDefinition[] { defaultStorageProviderDefinition };

      var storageSettings = new StorageSettings(defaultStorageProviderDefinition, providerCollection);
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(storageGroupType: typeof(Dummy));

      var result = storageSettings.GetStorageProviderDefinition(classDefinition);

      Assert.That(result, Is.EqualTo(defaultStorageProviderDefinition));
    }

    [Test]
    public void GetStorageProviderDefinition_ClassWithStorageGroupType_StorageGroupNotDefined_NoDefaultStorageProviderDefinitionDefined ()
    {
      var storageProviderDefinition = new UnitTestStorageProviderStubDefinition("stub");
      var providerCollection = new StorageProviderDefinition[] { storageProviderDefinition };

      var storageSettings = new StorageSettings(null, providerCollection);

      Assert.That(
          () => storageSettings.GetStorageProviderDefinition(typeof(StubStorageGroup1Attribute)),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.Contains("Missing default storage provider."));
    }

    [Test]
    public void GetStorageProviderDefinition_ClassWithStorageGroupType_StorageGroupNotDefined_DefaultStorageProviderDefinitionDefined ()
    {
      var defaultStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("default");
      var providerCollection = new StorageProviderDefinition[] { defaultStorageProviderDefinition };

      var storageSettings = new StorageSettings(defaultStorageProviderDefinition, providerCollection);

      var result = storageSettings.GetStorageProviderDefinition(typeof(StubStorageGroup1Attribute));

      Assert.That(result.Name, Is.EqualTo("default"));
    }

    [Test]
    public void GetStorageProviderDefinition_ClassWithStorageGroupType_StorageGroupDefined ()
    {
      var providerID = "Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration.StubStorageGroup1Attribute, Remotion.Data.UnitTests";
      var storageProviderDefinitionCollection = new List<StorageProviderDefinition>
                                                {
                                                  new UnitTestStorageProviderStubDefinition(providerID, new []{typeof(StubStorageGroup1Attribute)})
                                                };

      var defaultStorageProvider = new UnitTestStorageProviderStubDefinition("default");

      var storageSettings = new StorageSettings(defaultStorageProvider, storageProviderDefinitionCollection);

      var result = storageSettings.GetStorageProviderDefinition(typeof(StubStorageGroup1Attribute));

      Assert.That(result.Name, Is.EqualTo(providerID));
    }

    [Test]
    public void GetStorageProviderDefinition_ClassDefinition ()
    {
      var defaultStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("stub");
      var providerCollection = new StorageProviderDefinition[] { defaultStorageProviderDefinition };

      var storageSettings = new StorageSettings(defaultStorageProviderDefinition, providerCollection);

      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Dummy), baseClass: null);
      var result = storageSettings.GetStorageProviderDefinition(classDefinition);

      Assert.That(result, Is.SameAs(defaultStorageProviderDefinition));
    }

    [Test]
    public void GetStorageProviderDefinition_WithName_ReturnsProviderDefinition ()
    {
      var storageProvider1 = new UnitTestStorageProviderStubDefinition("unique", new[] { typeof(Dummy) });
      var storageProvider2 = new UnitTestStorageProviderStubDefinition("otherUnique", new[] { typeof(FirstStorageGroupAttribute) });

      var storageProviderCollection = new[] { storageProvider1, storageProvider2 };

      var storageSettings = new StorageSettings(storageProvider1, storageProviderCollection);

      var result = storageSettings.GetStorageProviderDefinition("unique");

      Assert.That(result, Is.EqualTo(storageProvider1));
    }

    [Test]
    public void GetStorageProviderDefinition_WithNameNotExistentInProviders_ThrowsConfigurationException ()
    {
      var storageProvider1 = new UnitTestStorageProviderStubDefinition("unique", new[] { typeof(Dummy) });
      var storageProvider2 = new UnitTestStorageProviderStubDefinition("otherUnique", new[] { typeof(FirstStorageGroupAttribute  ) });

      var storageProviderCollection = new[] { storageProvider1, storageProvider2 };

      var storageSettings = new StorageSettings(storageProvider1, storageProviderCollection);

      Assert.That(
          () => storageSettings.GetStorageProviderDefinition("nonExistent"),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.EqualTo($"The requested storage provider 'nonExistent' could not be found."));

    }
  }
}
