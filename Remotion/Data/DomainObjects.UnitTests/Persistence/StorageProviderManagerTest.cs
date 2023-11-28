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
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence
{
  [TestFixture]
  public class StorageProviderManagerTest : StandardMappingTest
  {
    private StorageProviderManager _storageProviderManager;

    public override void SetUp ()
    {
      base.SetUp();

      _storageProviderManager = new StorageProviderManager(NullPersistenceExtension.Instance, StorageSettings);
    }

    public override void TearDown ()
    {
      base.TearDown();
      _storageProviderManager.Dispose();
    }

    [Test]
    public void LookUp ()
    {
      StorageProvider provider = _storageProviderManager.GetMandatory(c_testDomainProviderID);

      Assert.That(provider, Is.Not.Null);
      Assert.That(provider.GetType(), Is.EqualTo(typeof(RdbmsProvider)));
      Assert.That(provider.StorageProviderDefinition.Name, Is.EqualTo(c_testDomainProviderID));
    }

    [Test]
    public void Reference ()
    {
      StorageProvider provider1 = _storageProviderManager.GetMandatory(c_testDomainProviderID);
      StorageProvider provider2 = _storageProviderManager.GetMandatory(c_testDomainProviderID);

      Assert.That(provider2, Is.SameAs(provider1));
    }

    [Test]
    public void Disposing ()
    {
      RdbmsProvider provider = null;

      using (_storageProviderManager)
      {
        provider = (RdbmsProvider)_storageProviderManager.GetMandatory(c_testDomainProviderID);
        provider.LoadDataContainer(DomainObjectIDs.Order1);

        Assert.That(provider.IsConnected, Is.True);
      }

      Assert.That(provider.IsConnected, Is.False);
    }
  }
}
