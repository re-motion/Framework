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
using System.Linq;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence
{
  [TestFixture]
  public class ReadOnlyStorageProviderManagerTest : StandardMappingTest
  {
    private ReadOnlyStorageProviderManager _readOnlyStorageProviderManager;
    private IPersistenceExtension _persistenceExtension;

    public override void SetUp ()
    {
      base.SetUp();

      _persistenceExtension = Mock.Of<IPersistenceExtension>();
      _readOnlyStorageProviderManager = new ReadOnlyStorageProviderManager(_persistenceExtension, StorageSettings);
      Assert.That(_readOnlyStorageProviderManager.PersistenceExtension, Is.SameAs(_persistenceExtension));
    }

    public override void TearDown ()
    {
      base.TearDown();
      _readOnlyStorageProviderManager.Dispose();
    }

    [Test]
    public void GetMandatory_WithProviderID ()
    {
#pragma warning disable CS0618 // Type or member is obsolete
      var provider = _readOnlyStorageProviderManager.GetMandatory(c_testDomainProviderID);
#pragma warning restore CS0618 // Type or member is obsolete

      Assert.That(provider, Is.Not.Null);
      Assert.That(provider.GetType(), Is.EqualTo(typeof(RdbmsProvider)));
      Assert.That(provider.As<RdbmsProvider>().StorageProviderDefinition.Name, Is.EqualTo(c_testDomainProviderID));
    }

    [Test]
    public void GetMandatory_WithProviderInstance ()
    {
      var providerDefinition = StorageSettings.GetStorageProviderDefinitions().OfType<RdbmsProviderDefinition>().First();
      var provider = _readOnlyStorageProviderManager.GetMandatory(providerDefinition);

      Assert.That(provider.GetType(), Is.EqualTo(typeof(RdbmsProvider)));
      Assert.That(provider.As<RdbmsProvider>().StorageProviderDefinition, Is.SameAs(providerDefinition));
    }

    [Test]
    public void GetMandatory_WithProviderInstanceNotPartOfStorageSettings ()
    {
      var rdbmsStorageObjectFactoryStub = new Mock<IRdbmsStorageObjectFactory>();
      var providerDefinition = new RdbmsProviderDefinition(c_testDomainProviderID, rdbmsStorageObjectFactoryStub.Object, "connection string", "readonly connection string");

#if DEBUG
      rdbmsStorageObjectFactoryStub
          .Setup(_ => _.CreateStorageProvider(It.IsAny<StorageProviderDefinition>(), It.IsAny<IPersistenceExtension>()))
          .Throws(new AssertionException("Should not be called"));

      Assert.That(
          () => _readOnlyStorageProviderManager.GetMandatory(providerDefinition),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  $"Supplied provider definition '{c_testDomainProviderID}' does not match the provider definition with the same name in the IStorageSettings object."));
#else
      var rdbmsProvider = new RdbmsProvider(
          providerDefinition,
          providerDefinition.ReadOnlyConnectionString,
          Mock.Of<IPersistenceExtension>(),
          Mock.Of<IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>>(),
          () => Mock.Of<System.Data.IDbConnection>());

      rdbmsStorageObjectFactoryStub
          .Setup(_ => _.CreateReadOnlyStorageProvider(It.IsAny<StorageProviderDefinition>(), It.IsAny<IPersistenceExtension>()))
          .Returns(rdbmsProvider);

      var provider = _readOnlyStorageProviderManager.GetMandatory(providerDefinition);

      Assert.That(provider, Is.SameAs(rdbmsProvider));
#endif
    }

    [Test]
    public void GetMandatory_WithProviderDefinition_ReturnsSameInstanceTwice ()
    {
      var storageProviderDefinition = StorageSettings.GetStorageProviderDefinition(c_testDomainProviderID);
      var provider1 = _readOnlyStorageProviderManager.GetMandatory(storageProviderDefinition);
      var provider2 = _readOnlyStorageProviderManager.GetMandatory(storageProviderDefinition);

      Assert.That(provider2, Is.SameAs(provider1));
    }

    [Test]
    public void Disposing ()
    {
      RdbmsProvider provider = null;

      var storageProviderDefinition = StorageSettings.GetStorageProviderDefinition(c_testDomainProviderID);
      using (_readOnlyStorageProviderManager)
      {
        provider = (RdbmsProvider)_readOnlyStorageProviderManager.GetMandatory(storageProviderDefinition);
        provider.LoadDataContainer(DomainObjectIDs.Order1);

        Assert.That(provider.IsConnected, Is.True);
      }

      Assert.That(provider.IsConnected, Is.False);
    }
  }
}
