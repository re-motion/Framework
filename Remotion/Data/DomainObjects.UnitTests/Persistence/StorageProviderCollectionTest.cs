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
using System.Data.SqlClient;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2014;
using Remotion.Data.DomainObjects.Tracing;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence
{
  [TestFixture]
  public class StorageProviderCollectionTest : StandardMappingTest
  {
    private Mock<IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>> _storageProviderCommandFactoryStub;

    private StorageProvider _provider;

    private StorageProviderCollection _collection;

    public override void SetUp ()
    {
      base.SetUp();

      _storageProviderCommandFactoryStub = new Mock<IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext>>();

      _provider = new RdbmsProvider(
          new RdbmsProviderDefinition("TestDomain", new SqlStorageObjectFactory(), "ConnectionString", "ReadOnlyConnectionString"),
          "ConnectionString",
          NullPersistenceExtension.Instance,
          _storageProviderCommandFactoryStub.Object,
          () => new SqlConnection());

      _collection = new StorageProviderCollection();
    }

    [Test]
    public void ContainsProviderTrue ()
    {
      _collection.Add(_provider);
      Assert.That(_collection.Contains(_provider), Is.True);
    }

    [Test]
    public void ContainsProviderFalse ()
    {
      _collection.Add(_provider);

      var rdbmsProviderDefinition = (RdbmsProviderDefinition)_provider.StorageProviderDefinition;

      StorageProvider copy = new RdbmsProvider(
          rdbmsProviderDefinition,
          rdbmsProviderDefinition.ConnectionString,
          NullPersistenceExtension.Instance,
          _storageProviderCommandFactoryStub.Object,
          () => new SqlConnection());
      Assert.That(_collection.Contains(copy), Is.False);
    }
  }
}
