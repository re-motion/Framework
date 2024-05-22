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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class RdbmsStructuredTypeDefinitionProviderTest : SchemaGenerationTestBase
  {
    [Test]
    public void GetTypeDefinitions ()
    {
      var provider = new RdbmsStructuredTypeDefinitionProvider();

      var result = provider.GetTypeDefinitions(SchemaGenerationFirstStorageProviderDefinition).ToArray();

      Assert.That(result.Length, Is.EqualTo(23));

      var i = 0;
      AssertFor(i++, DbType.String, typeof(String), false);
      AssertFor(i++, DbType.AnsiString, typeof(String), false);
      AssertFor(i++, DbType.Binary, typeof(Byte[]), false);
      AssertFor(i++, DbType.Boolean, typeof(Boolean?), false);
      AssertFor(i++, DbType.Boolean, typeof(Boolean?), true);
      AssertFor(i++, DbType.Byte, typeof(Byte?), false);
      AssertFor(i++, DbType.Byte, typeof(Byte?), true);
      AssertFor(i++, DbType.DateTime, typeof(DateTime?), false);
      AssertFor(i++, DbType.DateTime, typeof(DateTime?), true);
      AssertFor(i++, DbType.Decimal, typeof(Decimal?), false);
      AssertFor(i++, DbType.Decimal, typeof(Decimal?), true);
      AssertFor(i++, DbType.Double, typeof(Double?), false);
      AssertFor(i++, DbType.Double, typeof(Double?), true);
      AssertFor(i++, DbType.Guid, typeof(Guid?), false);
      AssertFor(i++, DbType.Guid, typeof(Guid?), true);
      AssertFor(i++, DbType.Int16, typeof(Int16?), false);
      AssertFor(i++, DbType.Int16, typeof(Int16?), true);
      AssertFor(i++, DbType.Int32, typeof(Int32?), false);
      AssertFor(i++, DbType.Int32, typeof(Int32?), true);
      AssertFor(i++, DbType.Int64, typeof(Int64?), false);
      AssertFor(i++, DbType.Int64, typeof(Int64?), true);
      AssertFor(i++, DbType.Single, typeof(Single?), false);
      AssertFor(i++, DbType.Single, typeof(Single?), true);

      void AssertFor (int position, DbType dbType, Type dotnetType, bool isDistinct)
      {
        var current = result[position];
        Assert.That(current, Is.InstanceOf<TableTypeDefinition>());
        var entityNameDefinition = current.As<TableTypeDefinition>().TypeName;
        Assert.That(entityNameDefinition.EntityName, Is.EqualTo($"TVP_{dbType}{(isDistinct ? "_Distinct" : null)}"));
        Assert.That(entityNameDefinition.SchemaName, Is.Null);

        Assert.That(current.Properties.Single(), Is.InstanceOf<SimpleStoragePropertyDefinition>());
        Assert.That(current.Properties.Single().As<SimpleStoragePropertyDefinition>().PropertyType, Is.EqualTo(dotnetType));
        Assert.That(current.Properties.Single().As<SimpleStoragePropertyDefinition>().ColumnDefinition.Name, Is.EqualTo("Value"));
        Assert.That(current.Properties.Single().As<SimpleStoragePropertyDefinition>().ColumnDefinition.StorageTypeInfo.DotNetType, Is.EqualTo(dotnetType));

        if (isDistinct)
        {
          Assert.That(current.As<TableTypeDefinition>().Constraints.Count, Is.EqualTo(1));
          var constraints = current.As<TableTypeDefinition>().Constraints;
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
          Assert.That(current.As<TableTypeDefinition>().Constraints, Is.Empty);
        }
      }
    }
  }
}
