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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
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

      Assert.That(result.Length, Is.EqualTo(22));

      AssertFor(0, DbType.String, typeof(String), false);
      AssertFor(1, DbType.Binary, typeof(Byte[]), false);
      AssertFor(2, DbType.Boolean, typeof(Boolean?), false);
      AssertFor(3, DbType.Boolean, typeof(Boolean?), true);
      AssertFor(4, DbType.Byte, typeof(Byte?), false);
      AssertFor(5, DbType.Byte, typeof(Byte?), true);
      AssertFor(6, DbType.DateTime, typeof(DateTime?), false);
      AssertFor(7, DbType.DateTime, typeof(DateTime?), true);
      AssertFor(8, DbType.Decimal, typeof(Decimal?), false);
      AssertFor(9, DbType.Decimal, typeof(Decimal?), true);
      AssertFor(10, DbType.Double, typeof(Double?), false);
      AssertFor(11, DbType.Double, typeof(Double?), true);
      AssertFor(12, DbType.Guid, typeof(Guid?), false);
      AssertFor(13, DbType.Guid, typeof(Guid?), true);
      AssertFor(14, DbType.Int16, typeof(Int16?), false);
      AssertFor(15, DbType.Int16, typeof(Int16?), true);
      AssertFor(16, DbType.Int32, typeof(Int32?), false);
      AssertFor(17, DbType.Int32, typeof(Int32?), true);
      AssertFor(18, DbType.Int64, typeof(Int64?), false);
      AssertFor(19, DbType.Int64, typeof(Int64?), true);
      AssertFor(20, DbType.Single, typeof(Single?), false);
      AssertFor(21, DbType.Single, typeof(Single?), true);
      return;

      void AssertFor (int position, DbType dbType, Type dotnetType, bool isDistinct)
      {
        var current = result[position];
        Assert.That(current, Is.InstanceOf<SqlTableTypeDefinition>());
        var entityNameDefinition = current.As<SqlTableTypeDefinition>().TypeName;
        Assert.That(entityNameDefinition.EntityName, Is.EqualTo($"TVP_{dbType}{(isDistinct ? "_distinct" : null)}"));
        Assert.That(entityNameDefinition.SchemaName, Is.Null);

        Assert.That(current.Properties.Single(), Is.InstanceOf<SimpleStoragePropertyDefinition>());
        Assert.That(current.Properties.Single().As<SimpleStoragePropertyDefinition>().PropertyType, Is.EqualTo(dotnetType));
        Assert.That(current.Properties.Single().As<SimpleStoragePropertyDefinition>().ColumnDefinition.Name, Is.EqualTo("Value"));
        Assert.That(current.Properties.Single().As<SimpleStoragePropertyDefinition>().ColumnDefinition.StorageTypeInfo.DotNetType, Is.EqualTo(dotnetType));

        if (isDistinct)
        {
          Assert.That(current.As<SqlTableTypeDefinition>().Constraints.Count, Is.EqualTo(1));
          var constraints = current.As<SqlTableTypeDefinition>().Constraints;
          Assert.That(constraints.Count, Is.EqualTo(1));

          var constraint = constraints.Single();
          Assert.That(constraint, Is.InstanceOf<UniqueConstraintDefinition>());
          Assert.That(constraint.ConstraintName, Is.EqualTo($"UQ_{entityNameDefinition.EntityName}_Value"));
          var columnDefinitions = constraint.As<UniqueConstraintDefinition>().Columns;
          Assert.That(columnDefinitions.Count, Is.EqualTo(1));
          Assert.That(columnDefinitions.Single(), Is.SameAs(current.Properties.Single().As<SimpleStoragePropertyDefinition>().ColumnDefinition));
        }
        else
        {
          Assert.That(current.As<SqlTableTypeDefinition>().Constraints, Is.Empty);
        }
      }
    }
  }
}
