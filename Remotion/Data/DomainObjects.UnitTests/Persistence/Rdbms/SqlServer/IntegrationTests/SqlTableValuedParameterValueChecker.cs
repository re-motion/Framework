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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SqlServer.Server;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests;

public static class SqlTableValuedParameterValueChecker
{
  public static void CheckEquals (object actualParameterValue, SqlTableValuedParameterValue expected)
  {
    Assert.That(actualParameterValue, Is.InstanceOf<SqlTableValuedParameterValue>());
    Assert.That(expected, Is.Not.Null);
    var actual = actualParameterValue.As<SqlTableValuedParameterValue>();

    Assert.That(actual.TableTypeName, Is.EqualTo(expected.TableTypeName));
    Assert.That(actual.ColumnMetaData.Count, Is.EqualTo(expected.ColumnMetaData.Count));
    Assert.That(actual.Count, Is.EqualTo(expected.Count));

    using var actualColumnEnumerator = actual.ColumnMetaData.GetEnumerator();
    using var expectedColumnEnumerator = expected.ColumnMetaData.GetEnumerator();
    while (actualColumnEnumerator.MoveNext())
    {
      Assert.That(expectedColumnEnumerator.MoveNext(), Is.True);

      var actualColumn = actualColumnEnumerator.Current;
      Assert.That(actualColumn, Is.Not.Null);

      var expectedColumn = expectedColumnEnumerator.Current;
      Assert.That(expectedColumn, Is.Not.Null);

      Assert.That(actualColumn.Name, Is.EqualTo(expectedColumn.Name));
      Assert.That(actualColumn.DbType, Is.EqualTo(expectedColumn.DbType));
      Assert.That(actualColumn.MaxLength, Is.EqualTo(expectedColumn.MaxLength));
    }

    using var actualRecordEnumerator = ((IEnumerable<SqlDataRecord>)actual).GetEnumerator();
    using var expectedRecordEnumerator = ((IEnumerable<SqlDataRecord>)expected).GetEnumerator();

    while (actualRecordEnumerator.MoveNext())
    {
      Assert.That(expectedRecordEnumerator.MoveNext(), Is.True);

      var actualRecord = actualRecordEnumerator.Current;
      Assert.That(actualRecord, Is.Not.Null);

      var expectedRecord = actualRecordEnumerator.Current;
      Assert.That(expectedRecord, Is.Not.Null);

      Assert.That(actualRecord.FieldCount, Is.EqualTo(expectedRecord.FieldCount));

      for (int col = 0; col < actualRecord.FieldCount; col++)
      {
        var actualValue = actualRecord[col];
        var expectedValue = expectedRecord[col];
        if (actualValue is IEnumerable actualEnumerable && expectedValue is IEnumerable expectedEnumerable)
          Assert.That(actualEnumerable.Cast<object>(), Is.EqualTo(expectedEnumerable.Cast<object>()));
        else
          Assert.That(actualValue, Is.EqualTo(expectedValue));
      }
    }
  }
}
