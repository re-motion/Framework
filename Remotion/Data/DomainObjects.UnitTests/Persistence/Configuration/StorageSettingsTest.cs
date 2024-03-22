// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it
// and/or modify it under the terms of the GNU Lesser General Public License
// as published by the Free Software Foundation; either version 2.1 of the
// License, or (at your option) any later version.
//
// re-motion is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
using System;
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGenerationTestDomain;
using Remotion.Development.NUnit.UnitTesting;

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
    public void Initialize_WithSameStorageProviderAddedTwice_ThrowsArgumentException ()
    {
      var uniqueStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("unique");
      var duplicateStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("stub");

      var storageProviderCollection = new[] { duplicateStorageProviderDefinition, uniqueStorageProviderDefinition, duplicateStorageProviderDefinition };

      Assert.That(
          () => new StorageSettings(null, storageProviderCollection),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Storage providers must have distinct names. The following duplicate names where found: 'stub'",
                  "storageProviderDefinitions"));
    }

    [Test]
    public void Initialize_WithNonUniqueStorageProvider_ThrowsArgumentException ()
    {
      var uniqueStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("unique");
      var nonUniqueStorageProviderDefinition1 = new UnitTestStorageProviderStubDefinition("stub");
      var nonUniqueStorageProviderDefinition2 = new UnitTestStorageProviderStubDefinition("stub");

      var storageProviderCollection = new[] { uniqueStorageProviderDefinition, nonUniqueStorageProviderDefinition1, nonUniqueStorageProviderDefinition2 };

      Assert.That(
          () => new StorageSettings(null, storageProviderCollection),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Storage providers must have distinct names. The following duplicate names where found: 'stub'",
                  "storageProviderDefinitions"));
    }

    [Test]
    public void Initialize_WithMultipleNonUniqueStorageProvider_ThrowsArgumentException ()
    {
      var uniqueStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("unique");
      var nonUniqueStorageProviderDefinition1 = new UnitTestStorageProviderStubDefinition("stub1");
      var nonUniqueStorageProviderDefinition2 = new UnitTestStorageProviderStubDefinition("stub2");
      var nonUniqueStorageProviderDefinition3 = new UnitTestStorageProviderStubDefinition("stub1");
      var nonUniqueStorageProviderDefinition4 = new UnitTestStorageProviderStubDefinition("stub2");

      var storageProviderCollection =
          new[]
          {
              uniqueStorageProviderDefinition,
              nonUniqueStorageProviderDefinition1, nonUniqueStorageProviderDefinition2,
              nonUniqueStorageProviderDefinition3, nonUniqueStorageProviderDefinition4
          };

      Assert.That(
          () => new StorageSettings(null, storageProviderCollection),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Storage providers must have distinct names. The following duplicate names where found: 'stub1', 'stub2'",
                  "storageProviderDefinitions"));
    }

    [Test]
    public void Initialize_WithStorageGroupAssignedToMultipleStorageProviders_ThrowsArgumentException ()
    {
      var storageProvider1 = new UnitTestStorageProviderStubDefinition("unique", new[] { typeof(Dummy) });
      var storageProvider2 = new UnitTestStorageProviderStubDefinition("otherUnique", new[] { typeof(Dummy) });

      var storageProviderCollection = new[] { storageProvider1, storageProvider2 };

      Assert.That(
          () => new StorageSettings(null, storageProviderCollection),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Storage group 'Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration.StorageSettingsTest+Dummy' is assigned to multiple storage providers: 'otherUnique' and 'unique'",
                  "storageProviderDefinitions"));
    }

    [Test]
    public void Initialize_WithDuplicateStorageProviderThatHasAStorageGroup_ThrowsArgumentException ()
    {
      var storageProvider = new UnitTestStorageProviderStubDefinition("duplicateProvider", new[] { typeof(Dummy) });

      var storageProviderCollection = new[] { storageProvider, storageProvider };

      Assert.That(
          () => new StorageSettings(null, storageProviderCollection),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Storage providers must have distinct names. The following duplicate names where found: 'duplicateProvider'",
                  "storageProviderDefinitions"));
    }

    [Test]
    public void DefaultStorageProviderDefinition_WithDefaultStorageProviderDefinition_ReturnsDefaultProviderDefinition ()
    {
      var defaultProvider = new UnitTestStorageProviderStubDefinition("default");
      var storageSettings = new StorageSettings(defaultProvider, new[] { defaultProvider });

      Assert.That(storageSettings.GetDefaultStorageProviderDefinition(), Is.EqualTo(defaultProvider));
    }

    [Test]
    public void DefaultStorageProviderDefinition_WithNullDefaultStorageProviderDefinition_ReturnsNull ()
    {
      var stubProvider = new UnitTestStorageProviderStubDefinition("default");
      var storageSettings = new StorageSettings(null, new[] { stubProvider });

      Assert.That(storageSettings.GetDefaultStorageProviderDefinition(), Is.Null);
    }

    [Test]
    public void GetStorageProviderDefinitions_ReturnsListOfStorageProviderDefinitions ()
    {
      var stubProvider = new UnitTestStorageProviderStubDefinition("default");
      var storageSettings = new StorageSettings(null, new[] { stubProvider });

      Assert.That(storageSettings.GetStorageProviderDefinitions(), Is.EquivalentTo(new [] {stubProvider}));
    }

    [Test]
    public void GetStorageProviderDefinition_WithClassWithoutStorageGroupType_WithDefaultStorageProviderDefinitionDefined_ReturnsDefaultStorageProviderDefinition ()
    {
      var defaultStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("stub");
      var providerCollection = new StorageProviderDefinition[] { defaultStorageProviderDefinition };

      var storageSettings = new StorageSettings(defaultStorageProviderDefinition, providerCollection);
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(storageGroupType: typeof(Dummy));

      var result = storageSettings.GetStorageProviderDefinition(classDefinition);

      Assert.That(result, Is.EqualTo(defaultStorageProviderDefinition));
    }

    [Test]
    public void GetStorageProviderDefinition_WithClassWithoutStorageGroupType_WithoutDefaultStorageProviderDefinitionDefined_ThrowsConfigurationException ()
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
    public void GetStorageProviderDefinition_WithClassWithStorageGroupType_WithStorageGroupDefined_ReturnsStorageProviderDefinitionForStorageGroup ()
    {
      var defaultStorageProvider = new UnitTestStorageProviderStubDefinition("default");

      var providerID = "Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration.StubStorageGroup1Attribute, Remotion.Data.UnitTests";
      var storageProviderDefinitionCollection = new List<StorageProviderDefinition>
                                                {
                                                    defaultStorageProvider,
                                                    new UnitTestStorageProviderStubDefinition(providerID, new []{typeof(StubStorageGroup1Attribute)})
                                                };

      var storageSettings = new StorageSettings(defaultStorageProvider, storageProviderDefinitionCollection);

      var result = storageSettings.GetStorageProviderDefinition(typeof(StubStorageGroup1Attribute));

      Assert.That(result.Name, Is.EqualTo(providerID));
    }

    [Test]
    public void GetStorageProviderDefinition_WithClassWithStorageGroupType_WithStorageGroupNotDefined_WithDefaultStorageProviderDefinitionDefined_ReturnsDefaultStorageProviderDefinition ()
    {
      var defaultStorageProviderDefinition = new UnitTestStorageProviderStubDefinition("default");
      var providerCollection = new StorageProviderDefinition[] { defaultStorageProviderDefinition };

      var storageSettings = new StorageSettings(defaultStorageProviderDefinition, providerCollection);

      var result = storageSettings.GetStorageProviderDefinition(typeof(StubStorageGroup1Attribute));

      Assert.That(result.Name, Is.EqualTo("default"));
    }

    [Test]
    public void GetStorageProviderDefinition_WithClassWithStorageGroupType_WithStorageGroupNotDefined_WithoutDefaultStorageProviderDefinitionDefined_ThrowsConfigurationException ()
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
