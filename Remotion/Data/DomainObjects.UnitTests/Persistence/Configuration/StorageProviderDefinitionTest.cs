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

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Configuration
{
  [TestFixture]
  public class StorageProviderDefinitionTest
  {
    public class FakeStorageObjectFactory : IStorageObjectFactory
    {
      public StorageProvider CreateStorageProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
      {
        throw new NotImplementedException();
      }

      public StorageProvider CreateReadOnlyStorageProvider (StorageProviderDefinition storageProviderDefinition, IPersistenceExtension persistenceExtension)
      {
        throw new NotImplementedException();
      }

      public IPersistenceModelLoader CreatePersistenceModelLoader (
          StorageProviderDefinition storageProviderDefinition)
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
