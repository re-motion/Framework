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
using Remotion.Data.DomainObjects.Persistence;
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
      _readOnlyStorageProviderManager = new ReadOnlyStorageProviderManager(_persistenceExtension);
      Assert.That(_readOnlyStorageProviderManager.PersistenceExtension, Is.SameAs(_persistenceExtension));
    }

    public override void TearDown ()
    {
      base.TearDown();
      _readOnlyStorageProviderManager.Dispose();
    }

    [Test]
    public void LookUp ()
    {
      IReadOnlyStorageProvider provider = _readOnlyStorageProviderManager[c_testDomainProviderID];

      Assert.That(provider, Is.Not.Null);
      Assert.That(provider, Is.InstanceOf<ReadOnlyStorageProviderDecorator>());

      var innerStorageProvider = provider.As<ReadOnlyStorageProviderDecorator>().InnerStorageProvider;
      Assert.That(innerStorageProvider.As<RdbmsProvider>().StorageProviderDefinition.Name, Is.EqualTo(c_testDomainProviderID));
    }

    [Test]
    public void Reference ()
    {
      IReadOnlyStorageProvider provider1 = _readOnlyStorageProviderManager[c_testDomainProviderID];
      IReadOnlyStorageProvider provider2 = _readOnlyStorageProviderManager[c_testDomainProviderID];

      Assert.That(provider2, Is.SameAs(provider1));
    }

    [Test]
    public void Disposing ()
    {
      ReadOnlyStorageProviderDecorator provider = null;

      using (_readOnlyStorageProviderManager)
      {
        provider = (ReadOnlyStorageProviderDecorator)_readOnlyStorageProviderManager[c_testDomainProviderID];
        provider.LoadDataContainer(DomainObjectIDs.Order1);

        Assert.That(provider.InnerStorageProvider.As<RdbmsProvider>().IsConnected, Is.True);
      }

      Assert.That(provider.InnerStorageProvider.As<RdbmsProvider>().IsConnected, Is.False);
    }
  }
}
