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
using System.Collections.Specialized;
using System.Configuration;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Development.UnitTesting;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Mixins;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration
{
  [TestFixture]
  public class StorageProviderDefinitionTest
  {
    public class FakeStorageObjectFactory : IStorageObjectFactory
    {
      public IStorageProvider CreateStorageProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
      {
        throw new NotImplementedException();
      }

      public IReadOnlyStorageProvider CreateReadOnlyStorageProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
      {
        throw new NotImplementedException();
      }

      public IPersistenceModelLoader CreatePersistenceModelLoader (
          StorageProviderDefinition storageProviderDefinition, IStorageProviderDefinitionFinder storageProviderDefinitionFinder)
      {
        throw new NotImplementedException();
      }

      public IDomainObjectQueryGenerator CreateDomainObjectQueryGenerator (
          StorageProviderDefinition storageProviderDefinition,
          IMethodCallTransformerProvider methodCallTransformerProvider,
          ResultOperatorHandlerRegistry resultOperatorHandlerRegistry,
          IMappingConfiguration mappingConfiguration)
      {
        throw new NotImplementedException();
      }
    }

    private Mock<IStorageObjectFactory> _storageObjectFactoryStub;

    [SetUp]
    public void SetUp ()
    {
      _storageObjectFactoryStub = new Mock<IStorageObjectFactory>();
    }

    [Test]
    public void Initialize_Objects ()
    {
      var providerDefinition = new TestableStorageProviderDefinition("TestProvider", _storageObjectFactoryStub.Object);

      Assert.That(providerDefinition.Name, Is.EqualTo("TestProvider"));
      Assert.That(providerDefinition.Factory, Is.SameAs(_storageObjectFactoryStub.Object));
    }

    [Test]
    public void Initialize_NameValueCollection_FactoryTypeMissing ()
    {
      var nameValueCollection = new NameValueCollection();

      Assert.That(
          () => new TestableStorageProviderDefinition("TestProvider", nameValueCollection),
          Throws.TypeOf<ConfigurationErrorsException>().With.Message.EqualTo(
              "The attribute 'factoryType' is missing in the configuration of the 'TestProvider' provider."));
    }

    [Test]
    public void Initialize_NameValueCollection_FactoryTypeNotFound ()
    {
      var nameValueCollection = new NameValueCollection { { "factoryType", "NonExistingType" } };

      Assert.That(() => new TestableStorageProviderDefinition("TestProvider", nameValueCollection), Throws.TypeOf<TypeLoadException>());
    }

    [Test]
    public void Initialize_NameValueCollection_WithoutServiceLocatorConfiguration ()
    {
      var nameValueCollection = new NameValueCollection { { "factoryType", typeof(FakeStorageObjectFactory).AssemblyQualifiedName } };

      var providerDefinition = new TestableStorageProviderDefinition("TestProvider", nameValueCollection);

      Assert.That(providerDefinition.Name, Is.EqualTo("TestProvider"));
      Assert.That(providerDefinition.Factory, Is.TypeOf<FakeStorageObjectFactory>());
    }

    [Test]
    public void Initialize_NameValueCollection_WithoutServiceLocatorConfiguration_CanBeMixed ()
    {
      var nameValueCollection = new NameValueCollection { { "factoryType", typeof(FakeStorageObjectFactory).AssemblyQualifiedName } };
      using (MixinConfiguration.BuildNew().ForClass<FakeStorageObjectFactory>().AddMixin<FakeMixin>().EnterScope())
      {
        var providerDefinition = new TestableStorageProviderDefinition("TestProvider", nameValueCollection);

        Assert.That(Mixin.Get<FakeMixin>(providerDefinition.Factory), Is.Not.Null);
      }
    }

    [Test]
    public void Initialize_NameValueCollection_WithoutServiceLocatorConfiguration_AbstractType ()
    {
      var nameValueCollection = new NameValueCollection { { "factoryType", typeof(IStorageObjectFactory).AssemblyQualifiedName } };
      Assert.That(
          () => new TestableStorageProviderDefinition("TestProvider", nameValueCollection),
          Throws.TypeOf<ConfigurationErrorsException>().With.Message.EqualTo(
              "The factory type 'Remotion.Data.DomainObjects.Persistence.IStorageObjectFactory' specified in the configuration of the 'TestProvider' "
              + "StorageProvider definition cannot be instantiated because it is abstract. Either register an implementation of "
              + "'IStorageObjectFactory' in the configured service locator, or specify a non-abstract type."));
    }

    [Test]
    public void Initialize_NameValueCollection_WithoutServiceLocatorConfiguration_InstantiationError ()
    {
      var nameValueCollection = new NameValueCollection { { "factoryType", typeof(FakeStorageObjectFactoryWithCtorParameters).AssemblyQualifiedName } };
      Assert.That(
          () => new TestableStorageProviderDefinition("TestProvider", nameValueCollection),
          Throws.TypeOf<ConfigurationErrorsException>().With.Message.EqualTo(
              "The factory type 'Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration.StorageProviderDefinitionTest+FakeStorageObjectFactoryWithCtorParameters' "
              + "specified in the configuration of the 'TestProvider' StorageProvider definition cannot be instantiated: Type "
              + "'Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration.StorageProviderDefinitionTest+FakeStorageObjectFactoryWithCtorParameters' "
              + "does not contain a constructor with the following signature: ()."));
    }

    [Test]
    public void Initialize_NameValueCollection_WithServiceLocatorConfiguration ()
    {
      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.Register(typeof(FakeStorageObjectFactory), typeof(FakeDerivedStorageObjectFactory), LifetimeKind.Singleton);
      using (new ServiceLocatorScope(serviceLocator))
      {
        var nameValueCollection = new NameValueCollection { { "factoryType", typeof(FakeStorageObjectFactory).AssemblyQualifiedName } };

        var providerDefinition = new TestableStorageProviderDefinition("TestProvider", nameValueCollection);

        Assert.That(providerDefinition.Name, Is.EqualTo("TestProvider"));
        Assert.That(providerDefinition.Factory, Is.TypeOf<FakeDerivedStorageObjectFactory>());
      }
    }

    [Test]
    public void Initialize_NameValueCollection_WithServiceLocatorConfiguration_InstantiationError ()
    {
      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.Register(typeof(FakeStorageObjectFactory), typeof(FakeDerivedStorageObjectFactoryWithUnresolvedCtorParameter), LifetimeKind.Singleton);
      using (new ServiceLocatorScope(serviceLocator))
      {
        var nameValueCollection = new NameValueCollection { { "factoryType", typeof(FakeStorageObjectFactory).AssemblyQualifiedName } };

        Assert.That(
          () => new TestableStorageProviderDefinition("TestProvider", nameValueCollection),
          Throws.TypeOf<ConfigurationErrorsException>().With.Message.EqualTo(
              "The factory type 'Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration.StorageProviderDefinitionTest+FakeStorageObjectFactory' "
              + "specified in the configuration of the 'TestProvider' StorageProvider definition cannot be resolved: Could not resolve type "
              + "'Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration.StorageProviderDefinitionTest+FakeStorageObjectFactory': "
              + "Error resolving indirect dependency of constructor parameter 's' of type "
              + "'Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration.StorageProviderDefinitionTest+FakeDerivedStorageObjectFactoryWithUnresolvedCtorParameter': "
              + "No implementation is registered for service type 'System.String'."));
      }
    }

    [Test]
    public new void ToString ()
    {
      var providerDefinition = new TestableStorageProviderDefinition("TestProvider", _storageObjectFactoryStub.Object);

      Assert.That(providerDefinition.ToString(), Is.EqualTo("TestableStorageProviderDefinition: 'TestProvider'"));
    }

    private class FakeDerivedStorageObjectFactory : FakeStorageObjectFactory { }

    public class FakeMixin { }

    public class FakeStorageObjectFactoryWithCtorParameters
    {
      public FakeStorageObjectFactoryWithCtorParameters (string s)
      {
        Dev.Null = s;
      }
    }

    private class FakeDerivedStorageObjectFactoryWithUnresolvedCtorParameter : FakeStorageObjectFactory
    {
      public FakeDerivedStorageObjectFactoryWithUnresolvedCtorParameter (string s)
      {
        Dev.Null = s;
      }
    }
  }
}
