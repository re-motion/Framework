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
using System.Data;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class RdbmsStructuredTypeDefinitionProviderTest
  {
    [Test]
    public void GetTypeDefinitions ()
    {
      var provider = new RdbmsStructuredTypeDefinitionProvider();

      var storageTypeInformationProvider = (IStorageTypeInformationProvider)new SqlStorageTypeInformationProvider();
      var result = provider.GetTypeDefinitions(storageTypeInformationProvider).ToArray();

      Assert.That(result.Length, Is.EqualTo(12));

      AssertFor(0, DbType.String, typeof(String));
      AssertFor(1, DbType.Binary, typeof(Byte[]));
      AssertFor(2, DbType.Boolean, typeof(Boolean?));
      AssertFor(3, DbType.Byte, typeof(Byte?));
      AssertFor(4, DbType.DateTime, typeof(DateTime?));
      AssertFor(5, DbType.Decimal, typeof(Decimal?));
      AssertFor(6, DbType.Double, typeof(Double?));
      AssertFor(7, DbType.Guid, typeof(Guid?));
      AssertFor(8, DbType.Int16, typeof(Int16?));
      AssertFor(9, DbType.Int32, typeof(Int32?));
      AssertFor(10, DbType.Int64, typeof(Int64?));
      AssertFor(11, DbType.Single, typeof(Single?));
      return;

      void AssertFor (int position, DbType dbType, Type dotnetType)
      {
        var current = result[position];
        Assert.That(current, Is.InstanceOf<TableTypeDefinition>());
        Assert.That(current.As<TableTypeDefinition>().TypeName.TypeName, Is.EqualTo($"TVP_{dbType}"));
        Assert.That(current.As<TableTypeDefinition>().TypeName.SchemaName, Is.Null);

        Assert.That(current.Properties.Single(), Is.InstanceOf<SimpleStoragePropertyDefinition>());
        Assert.That(current.Properties.Single().As<SimpleStoragePropertyDefinition>().PropertyType, Is.EqualTo(dotnetType));
        Assert.That(current.Properties.Single().As<SimpleStoragePropertyDefinition>().ColumnDefinition.Name, Is.EqualTo("Value"));
        Assert.That(current.Properties.Single().As<SimpleStoragePropertyDefinition>().ColumnDefinition.StorageTypeInfo.DotNetType, Is.EqualTo(dotnetType));

        Assert.That(current.Constraints, Is.Empty);
        Assert.That(current.Indexes, Is.Empty);
      }
    }
  }
}
