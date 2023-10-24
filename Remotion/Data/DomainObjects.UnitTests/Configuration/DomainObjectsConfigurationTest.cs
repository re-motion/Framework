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
using System.Configuration;
using NUnit.Framework;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Development;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.Resources;
using Remotion.Development.UnitTesting.IO;
using Remotion.Mixins;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration
{
  [TestFixture]
  public class DomainObjectsConfigurationTest
  {
    [SetUp]
    public void SetUp ()
    {
      DomainObjectsConfiguration.SetCurrent(null);
    }

    [Test]
    public void GetAndSet ()
    {
      IDomainObjectsConfiguration configuration =
          new FakeDomainObjectsConfiguration(new StorageConfiguration());
      DomainObjectsConfiguration.SetCurrent(configuration);

      Assert.That(DomainObjectsConfiguration.Current, Is.SameAs(configuration));
    }

    [Test]
    public void Get ()
    {
      Assert.That(DomainObjectsConfiguration.Current, Is.Not.Null);
    }

    [Test]
    public void Initialize ()
    {
      DomainObjectsConfiguration domainObjectsConfiguration = new DomainObjectsConfiguration();

      Assert.That(domainObjectsConfiguration.Storage, Is.Not.Null);
    }

    [Test]
    public void Initialize_WithConfigurationHavingMinimumSettings ()
    {
      using (TempFile configFile = new TempFile())
      {
        SetUpConfigurationWrapper(ConfigurationFactory.LoadConfigurationFromFile(configFile, ResourceManager.GetDomainObjectsConfigurationWithMinimumSettings()));

        DomainObjectsConfiguration domainObjectsConfiguration = new DomainObjectsConfiguration();

        Assert.That(domainObjectsConfiguration.Storage, Is.Not.Null);
        Assert.That(domainObjectsConfiguration.Storage.DefaultStorageProviderDefinition, Is.Not.Null);
        Assert.That(domainObjectsConfiguration.Storage.DefaultStorageProviderDefinition.Factory, Is.TypeOf<SqlStorageObjectFactory>());
        Assert.That(domainObjectsConfiguration.Storage.StorageProviderDefinitions.Count, Is.EqualTo(1));
        Assert.That(domainObjectsConfiguration.Storage.StorageGroups, Is.Empty);
      }
    }

    [Test]
    public void Initialize_WithResolvedInterfaceStorageObjectFactory ()
    {
      using (TempFile configFile = new TempFile())
      {
        SetUpConfigurationWrapper(
            ConfigurationFactory.LoadConfigurationFromFile(
                configFile, ResourceManager.GetDomainObjectsConfigurationWithResolvedInterfaceStorageObjectFactory()));

        DomainObjectsConfiguration domainObjectsConfiguration = new DomainObjectsConfiguration();

        Assert.That(domainObjectsConfiguration.Storage.DefaultStorageProviderDefinition.Factory, Is.TypeOf<CustomStorageObjectFactory>());
        Assert.That(domainObjectsConfiguration.Storage.StorageGroups, Is.Empty);
      }
    }

    [Test]
    public void Initialize_WithUnresolvedInterfaceStorageObjectFactory ()
    {
      using (TempFile configFile = new TempFile())
      {
        SetUpConfigurationWrapper(
            ConfigurationFactory.LoadConfigurationFromFile(
                configFile, ResourceManager.GetDomainObjectsConfigurationWithUnresolvedInterfaceStorageObjectFactory()));

        DomainObjectsConfiguration domainObjectsConfiguration = new DomainObjectsConfiguration();

        Assert.That(
            () => domainObjectsConfiguration.Storage.DefaultStorageProviderDefinition,
            Throws.TypeOf<ConfigurationErrorsException>().With.Message.StartsWith(
                "The factory type 'Remotion.Data.DomainObjects.UnitTests.Configuration.DomainObjectsConfigurationTest+IUnresolvedCustomStorageObjectFactory' "
                + "specified in the configuration of the 'Test' StorageProvider definition cannot be instantiated because it is abstract. Either "
                + "register an implementation of 'IUnresolvedCustomStorageObjectFactory' in the configured service locator, or specify a non-abstract "
                + "type."));
      }
    }

    [Test]
    public void Initialize_WithMixedStorageObjectFactory ()
    {
      using (TempFile configFile = new TempFile())
      {
        SetUpConfigurationWrapper(
            ConfigurationFactory.LoadConfigurationFromFile(
                configFile, ResourceManager.GetDomainObjectsConfigurationWithMixedStorageObjectFactory()));

        DomainObjectsConfiguration domainObjectsConfiguration = new DomainObjectsConfiguration();

        Assert.That(Mixin.Get<FakeMixin>(domainObjectsConfiguration.Storage.DefaultStorageProviderDefinition.Factory), Is.Not.Null);
      }
    }

    [Test]
    public void Initialize_WithConfigurationHavingCustomMappingLoader ()
    {
      using (TempFile configFile = new TempFile())
      {
        SetUpConfigurationWrapper(ConfigurationFactory.LoadConfigurationFromFile(configFile, ResourceManager.GetDomainObjectsConfigurationWithFakeMappingLoader()));

        DomainObjectsConfiguration domainObjectsConfiguration = new DomainObjectsConfiguration();

        Assert.That(domainObjectsConfiguration.Storage, Is.Not.Null);
        Assert.That(domainObjectsConfiguration.Storage.DefaultStorageProviderDefinition, Is.Not.Null);
        Assert.That(domainObjectsConfiguration.Storage.StorageProviderDefinitions.Count, Is.EqualTo(1));
        Assert.That(domainObjectsConfiguration.Storage.StorageGroups, Is.Empty);
      }
    }

    [Test]
    public void Initialize_WithConfigurationHavingCustomSectionGroupName ()
    {
      using (TempFile configFile = new TempFile())
      {
        System.Configuration.Configuration configuration =
            ConfigurationFactory.LoadConfigurationFromFile(configFile, ResourceManager.GetDomainObjectsConfigurationWithCustomSectionGroupName());
        SetUpConfigurationWrapper(configuration);

        DomainObjectsConfiguration domainObjectsConfiguration = (DomainObjectsConfiguration)configuration.GetSectionGroup("domainObjects");
        // For ReSharper's sake
        Assertion.IsNotNull(domainObjectsConfiguration);

        Assert.That(domainObjectsConfiguration.SectionGroupName, Is.EqualTo("domainObjects"));
        Assert.That(domainObjectsConfiguration.Storage, Is.Not.Null);
        Assert.That(domainObjectsConfiguration.Storage.DefaultStorageProviderDefinition, Is.Not.Null);
        Assert.That(domainObjectsConfiguration.Storage.StorageProviderDefinitions.Count, Is.EqualTo(1));
        Assert.That(domainObjectsConfiguration.Storage.StorageGroups, Is.Empty);
      }
    }

    [Test]
    public void GetStorage_SameInstanceTwice ()
    {
      DomainObjectsConfiguration domainObjectsConfiguration = new DomainObjectsConfiguration();

      Assert.That(domainObjectsConfiguration.Storage, Is.SameAs(domainObjectsConfiguration.Storage));
    }

    private void SetUpConfigurationWrapper (System.Configuration.Configuration configuration)
    {
      ConfigurationWrapper.SetCurrent(ConfigurationWrapper.CreateFromConfigurationObject(configuration));
    }

    public interface ICustomStorageObjectFactory : IStorageObjectFactory
    {
    }

    [ImplementationFor(typeof(ICustomStorageObjectFactory))]
    public class CustomStorageObjectFactory : SqlStorageObjectFactory, ICustomStorageObjectFactory
    {
    }

    public interface IUnresolvedCustomStorageObjectFactory : IRdbmsStorageObjectFactory
    {
    }

    [Uses(typeof(FakeMixin))]
    public class MixedCustomStorageObjectFactory : SqlStorageObjectFactory
    {
    }

    public class FakeMixin { }
  }
}
