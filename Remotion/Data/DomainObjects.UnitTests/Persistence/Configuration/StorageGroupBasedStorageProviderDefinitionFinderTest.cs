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
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration
{
  [TestFixture]
  public class StorageGroupBasedStorageProviderDefinitionFinderTest : StandardMappingTest
  {
    private IStorageSettings _storageSettings;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      var defaultStorageProvider = new UnitTestStorageProviderStubDefinition("Test");
      _storageSettings = new StorageSettings(defaultStorageProvider, new[] { defaultStorageProvider }, null);


      var storageProviderDefinitionHelper =
          (StorageProviderDefinitionHelper)PrivateInvoke.GetNonPublicField(_storageSettings, "_defaultStorageProviderDefinitionHelper");
      storageProviderDefinitionHelper.Provider = null;
    }

    [Test]
    public void GetStorageProviderDefinition_ClassWithoutStorageGroupType_NoDefaultStorageProviderDefinitionDefined ()
    {
      var finder = new StorageGroupBasedStorageProviderDefinitionFinder(_storageSettings);
      Assert.That(
          () => finder.GetStorageProviderDefinition((Type)null, null),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.Contains("Missing default storage provider."));
    }

    [Test]
    public void GetStorageProviderDefinition_ClassWithoutStorageGroupType_NoDefaultStorageProviderDefinitionDefined_WithContext ()
    {
      var finder = new StorageGroupBasedStorageProviderDefinitionFinder(_storageSettings);
      Assert.That(
          () => finder.GetStorageProviderDefinition((Type)null, "Test"),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.Contains("Missing default storage provider. Test"));
    }

    [Test]
    public void GetStorageProviderDefinition_ClassWithoutStorageGroupType_DefaultStorageProviderDefinitionDefined ()
    {
      var finder = new StorageGroupBasedStorageProviderDefinitionFinder(DomainObjectsConfiguration.Current.Storage);
      var result = finder.GetStorageProviderDefinition((Type)null, null);

      Assert.That(result.Name, Is.EqualTo("DefaultStorageProvider"));
    }

    [Test]
    public void GetStorageProviderDefinition_ClassWithStorageGroupType_StorageGroupNotDefined_NoDefaultStorageProviderDefinitionDefined ()
    {
      var finder = new StorageGroupBasedStorageProviderDefinitionFinder(_storageSettings);
      Assert.That(
          () => finder.GetStorageProviderDefinition(typeof(StubStorageGroup1Attribute), null),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.Contains("Missing default storage provider."));
    }

    [Test]
    public void GetStorageProviderDefinition_ClassWithStorageGroupType_StorageGroupNotDefined_NoDefaultStorageProviderDefinitionDefined_WithContext ()
    {
      var finder = new StorageGroupBasedStorageProviderDefinitionFinder(_storageSettings);
      Assert.That(
          () => finder.GetStorageProviderDefinition(typeof(StubStorageGroup1Attribute), "Test"),
          Throws.InstanceOf<ConfigurationException>()
              .With.Message.Contains("Missing default storage provider. Test"));
    }

    [Test]
    public void GetStorageProviderDefinition_ClassWithStorageGroupType_StorageGroupNotDefined_DefaultStorageProviderDefinitionDefined ()
    {
      var finder = new StorageGroupBasedStorageProviderDefinitionFinder(DomainObjectsConfiguration.Current.Storage);
      var result = finder.GetStorageProviderDefinition(typeof(StubStorageGroup1Attribute), null);

      Assert.That(result.Name, Is.EqualTo("DefaultStorageProvider"));
    }

    [Test]
    public void GetStorageProviderDefinition_ClassWithStorageGroupType_StorageGroupDefined ()
    {
      var providerID = "Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration.StubStorageGroup1Attribute, Remotion.Data.UnitTests";
      var storageProviderDefinitionCollection = new ProviderCollection<StorageProviderDefinition>
                                                {
                                                    new UnitTestStorageProviderStubDefinition(providerID)
                                                };
      var storageConfiguration = new StorageConfiguration(
          storageProviderDefinitionCollection,
          DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition);

      storageConfiguration.StorageGroups.Add(new StorageGroupElement(new StubStorageGroup1Attribute(), providerID));
      var finder = new StorageGroupBasedStorageProviderDefinitionFinder(storageConfiguration);

      var result = finder.GetStorageProviderDefinition(typeof(StubStorageGroup1Attribute), null);

      Assert.That(result.Name, Is.EqualTo(providerID));
    }

    [Test]
    public void GetStorageProviderDefinition_ClassDefinition ()
    {
      var finder = new StorageGroupBasedStorageProviderDefinitionFinder(DomainObjectsConfiguration.Current.Storage);
      var result = finder.GetStorageProviderDefinition(Configuration.GetTypeDefinition(typeof(Order)), null);

      Assert.That(result, Is.SameAs(TestDomainStorageProviderDefinition));
    }
  }
}
