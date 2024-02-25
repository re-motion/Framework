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
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Development.UnitTesting;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration
{
  [TestFixture]
  public class StorageObjectFactoryFactoryTest
  {
    private abstract class AbstractStorageObjectFactory : StorageObjectFactoryBase
    {
    }

    private class DefaultConstructorLessStorageObjectFactory : StorageObjectFactoryBase
    {
      public DefaultConstructorLessStorageObjectFactory (string bla)
      {
      }
    }

    private class ThrowingStorageObjectFactory : StorageObjectFactoryBase
    {
      public ThrowingStorageObjectFactory ()
      {
        throw new InvalidOperationException("Constructor is failing.");
      }
    }

    private abstract class StorageObjectFactoryBase : IStorageObjectFactory
    {
      public StorageProvider CreateStorageProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
      {
        throw new System.NotImplementedException();
      }

      public IPersistenceModelLoader CreatePersistenceModelLoader (StorageProviderDefinition storageProviderDefinition)
      {
        throw new System.NotImplementedException();
      }

      public IDomainObjectQueryGenerator CreateDomainObjectQueryGenerator (
          StorageProviderDefinition storageProviderDefinition,
          IMethodCallTransformerProvider methodCallTransformerProvider,
          ResultOperatorHandlerRegistry resultOperatorHandlerRegistry,
          IMappingConfiguration mappingConfiguration)
      {
        throw new System.NotImplementedException();
      }
    }

    [Test]
    public void Create_WithoutIoCRegistration_CreatesObject ()
    {
      var serviceLocator = DefaultServiceLocator.Create();
      var storageObjectFactoryFactory = new StorageObjectFactoryFactory();

      Assert.That(((IServiceProvider)serviceLocator).GetService(typeof(UnitTestStorageObjectFactoryStub)), Is.Null);

      using (new ServiceLocatorScope(serviceLocator))
      {
        var result = storageObjectFactoryFactory.Create(typeof(UnitTestStorageObjectFactoryStub));

        Assert.That(result, Is.InstanceOf<UnitTestStorageObjectFactoryStub>());
      }
    }

    [Test]
    public void Create_WithIoCRegistration_GetsFromIoC ()
    {
      var serviceLocator = DefaultServiceLocator.Create();
      var storageObjectFactoryFactory = new StorageObjectFactoryFactory();

      var resultFromServiceLocator = ((IServiceProvider)serviceLocator).GetService(typeof(SqlStorageObjectFactory));

      using (new ServiceLocatorScope(serviceLocator))
      {
        var result = storageObjectFactoryFactory.Create(typeof(SqlStorageObjectFactory));

        Assert.That(result, Is.SameAs(resultFromServiceLocator));
      }
    }

    [Test]
    public void Create_WithAbstractType_ThrowsConfigurationException ()
    {
      var storageObjectFactoryFactory = new StorageObjectFactoryFactory();

      Assert.That(
          () => storageObjectFactoryFactory.Create(typeof(AbstractStorageObjectFactory)),
          Throws.InstanceOf<ConfigurationException>().With.Message
              .StartsWith(
                  "The factory type 'Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration.StorageObjectFactoryFactoryTest+AbstractStorageObjectFactory' "
                  + "cannot be instantiated because it is abstract."));
    }

    [Test]
    public void Create_WithoutDefaultConstructor_ThrowsConfigurationException ()
    {
      var storageObjectFactoryFactory = new StorageObjectFactoryFactory();

      Assert.That(
          () => storageObjectFactoryFactory.Create(typeof(DefaultConstructorLessStorageObjectFactory)),
          Throws.InstanceOf<ConfigurationException>().With.Message
              .StartsWith(
                  "The factory type 'Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration.StorageObjectFactoryFactoryTest+DefaultConstructorLessStorageObjectFactory' "
                  + "does not contain a default constructor required for instantiation via the StorageObjectFactoryFactory.\n"
                  + "Register an instance of the factory type with the service locator instead:\n\n"));
    }

    [Test]
    public void Create_WithExceptionDuringInstantiation_ThrowsConfigurationException ()
    {
      var storageObjectFactoryFactory = new StorageObjectFactoryFactory();

      Assert.That(
          () => storageObjectFactoryFactory.Create(typeof(ThrowingStorageObjectFactory)),
          Throws.InstanceOf<ConfigurationException>().With.Message
              .EqualTo(
                  "The factory type 'Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration.StorageObjectFactoryFactoryTest+ThrowingStorageObjectFactory' "
                  + "cannot be instantiated: Constructor is failing."));
    }
  }
}
