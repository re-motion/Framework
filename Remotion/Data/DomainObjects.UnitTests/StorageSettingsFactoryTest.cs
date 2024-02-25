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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2016;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class StorageSettingsFactoryTest
  {
    [Test]
    public void Create ()
    {
      var result = StorageSettingsFactory.CreateForSqlServer("dummyConnectionString");

      Assert.That(result, Is.InstanceOf<RdbmsStorageSettingsFactory>());

      var rdbmsStorageSettingsFactory = (RdbmsStorageSettingsFactory)result;

      Assert.That(rdbmsStorageSettingsFactory.ProviderName, Is.EqualTo("Default"));
      Assert.That(rdbmsStorageSettingsFactory.ConnectionString, Is.EqualTo("dummyConnectionString"));
      Assert.That(rdbmsStorageSettingsFactory.StorageObjectFactoryType, Is.EqualTo(typeof(SqlStorageObjectFactory)));
    }

    [Test]
    public void Create_WithType ()
    {
      var result = StorageSettingsFactory.CreateForSqlServer<UnitTestStorageObjectFactoryStub>("dummyConnectionString");

      Assert.That(result, Is.InstanceOf<RdbmsStorageSettingsFactory>());

      var rdbmsStorageSettingsFactory = (RdbmsStorageSettingsFactory)result;

      Assert.That(rdbmsStorageSettingsFactory.ProviderName, Is.EqualTo("Default"));
      Assert.That(rdbmsStorageSettingsFactory.ConnectionString, Is.EqualTo("dummyConnectionString"));
      Assert.That(rdbmsStorageSettingsFactory.StorageObjectFactoryType, Is.EqualTo(typeof(UnitTestStorageObjectFactoryStub)));
    }
  }
}
