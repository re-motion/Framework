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
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.SqlServer.Server;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Parameters;

[TestFixture]
public class SqlTableValuedDataParameterDefinitionTest
{
  private static IEnumerable<(object[], bool)> GetTestCollections ()
  {
    yield return ([1, 2, 3, 5, 8, 13, 21], true); // distinct
    yield return ([1, 2, 3, 3, 2, 1], false); // duplicates
    yield return ([], false); // empty
    yield return ([], true); // empty distinct
    yield return ([ClassWithAllDataTypes.EnumType.Value0, ClassWithAllDataTypes.EnumType.Value1, ClassWithAllDataTypes.EnumType.Value2], true); // converted
    yield return (["Value1", "Value2", "Value1"], false);
  }

  [Test]
  [TestCaseSource(nameof(GetTestCollections))]
  public void GetParameterValue_ReturnsSqlTableValuedParameterValue ((object[], bool) testCase)
  {
    var items = testCase.Item1;
    var isDistinct = testCase.Item2;

    var storageTypeInformation = new SqlStorageTypeInformationProvider().GetStorageType(items.FirstOrDefault());
    var convertedValues = items.Select(i => storageTypeInformation.ConvertToStorageType(i)).ToArray();

    var parameterDefinition = new SqlTableValuedDataParameterDefinition(storageTypeInformation, isDistinct);

    var result = parameterDefinition.GetParameterValue(items);

    Assert.That(result, Is.InstanceOf<SqlTableValuedParameterValue>());

    var tvpValue = result.As<SqlTableValuedParameterValue>();

    if(isDistinct)
      Assert.That(tvpValue.TableTypeName, Is.EqualTo($"TVP_{storageTypeInformation.StorageDbType}{SqlTableValuedDataParameterDefinition.DistinctCollectionTableTypeNameSuffix}"));
    else
      Assert.That(tvpValue.TableTypeName, Is.EqualTo($"TVP_{storageTypeInformation.StorageDbType}"));

    Assert.That(tvpValue.ColumnMetaData.Count, Is.EqualTo(1));

    var sqlMetaData = tvpValue.ColumnMetaData.Single();
    Assert.That(sqlMetaData.Name, Is.EqualTo("Value"));

    var conversionHelper = new SqlParameter();
    conversionHelper.DbType = storageTypeInformation.StorageDbType;
    var sqlDbType = conversionHelper.SqlDbType;
    Assert.That(sqlMetaData.SqlDbType, Is.EqualTo(sqlDbType));

    Assert.That(tvpValue.Select(record => record.GetValue(0)), Is.EquivalentTo(convertedValues));
  }

  [Test]
  public void GetParameterValue_WithMismatchedItemType_ThrowsArgumentException ()
  {
    var items = new[] { "Value1", "Value2" };
    var storageTypeInformation = StorageTypeInformationObjectMother.CreateIntStorageTypeInformation();

    var parameterDefinition = new SqlTableValuedDataParameterDefinition(storageTypeInformation, true);

    Assert.That(
        () => parameterDefinition.GetParameterValue(items),
        Throws.InstanceOf<ArgumentException>().With
            .ArgumentExceptionMessageEqualTo($"Item 0 of parameter 'value' has the type '{typeof(string)}' instead of '{typeof(int)}'.", "value"));
  }

  [Test]
  public void CreateDataParameter_WithTableValuedParameter_SetsSqlParameterProperties ()
  {
    var items = new[] { 1, 2, 3, 5, 8, 13, 21 }.ToList();
    var tvpValue = new SqlTableValuedParameterValue("any", new[] { new SqlMetaData("Value", SqlDbType.Int) });
    foreach (var item in items)
      tvpValue.AddRecord(item);

    var parameterDefinition = new SqlTableValuedDataParameterDefinition(Mock.Of<IStorageTypeInformation>(), false);

    var command = new SqlCommand();
    var dataParameter = parameterDefinition.CreateDataParameter(command, "@dummy", tvpValue);
    Assert.That(dataParameter, Is.InstanceOf<SqlParameter>());

    var sqlParameter = dataParameter.As<SqlParameter>();
    Assert.That(sqlParameter.DbType, Is.EqualTo(DbType.Object));
    Assert.That(sqlParameter.SqlDbType, Is.EqualTo(SqlDbType.Structured));
    Assert.That(sqlParameter.TypeName, Is.EqualTo(tvpValue.TableTypeName));
    Assert.That(sqlParameter.Value, Is.SameAs(tvpValue));
  }

  [Test]
  public void CreateDataParameter_WithEmptyTableValuedParameter_SetsParameterValueToDbNull ()
  {
    var tvpValue = new SqlTableValuedParameterValue("any", new[] { new SqlMetaData("Value", SqlDbType.Int) });

    var parameterDefinition = new SqlTableValuedDataParameterDefinition(Mock.Of<IStorageTypeInformation>(), false);

    var command = new SqlCommand();
    var dataParameter = parameterDefinition.CreateDataParameter(command, "@dummy", tvpValue);
    Assert.That(dataParameter, Is.InstanceOf<SqlParameter>());

    var sqlParameter = dataParameter.As<SqlParameter>();
    Assert.That(sqlParameter.DbType, Is.EqualTo(DbType.Object));
    Assert.That(sqlParameter.SqlDbType, Is.EqualTo(SqlDbType.Structured));
    Assert.That(sqlParameter.TypeName, Is.EqualTo(tvpValue.TableTypeName));
    Assert.That(sqlParameter.Value, Is.EqualTo(DBNull.Value));
  }
}
