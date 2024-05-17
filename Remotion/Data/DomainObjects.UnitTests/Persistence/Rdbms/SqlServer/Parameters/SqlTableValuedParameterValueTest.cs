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
using System.Data;
using System.Linq;
using Microsoft.SqlServer.Server;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.Parameters
{
  [TestFixture]
  public class SqlTableValuedParameterValueTest
  {
    [Test]
    public void Initialize ()
    {
      var tableTypeName = "Dummy";
      var columnMetaData = new[] { new SqlMetaData("DummyName", SqlDbType.Int) };

      var tvpValue = new SqlTableValuedParameterValue(tableTypeName, columnMetaData);

      Assert.That(tvpValue.TableTypeName, Is.SameAs(tableTypeName));
      Assert.That(tvpValue.ColumnMetaData, Is.EqualTo(columnMetaData));

      Assert.That(tvpValue.Count, Is.EqualTo(0));
      Assert.That(tvpValue, Is.Empty);
    }

    [Test]
    public void AddRecord ()
    {
      var tableTypeName = "Dummy";
      var columnMetaData = new[] { new SqlMetaData("DummyName", SqlDbType.Int) };

      var tvpValue = new SqlTableValuedParameterValue(tableTypeName, columnMetaData);

      tvpValue.AddRecord(42);

      Assert.That(tvpValue.Count, Is.EqualTo(1));
      var record = tvpValue.Single();

      Assert.That(record.FieldCount, Is.EqualTo(1));
      Assert.That(record.GetSqlMetaData(0), Is.EqualTo(columnMetaData[0]));
    }

    [Test]
    public void AddRecord_WithIncorrectTypeOfValues_ThrowsInvalidCastException ()
    {
      var tableTypeName = "Dummy";
      var columnMetaData = new[] { new SqlMetaData("DummyName", SqlDbType.Int) };

      var tvpValue = new SqlTableValuedParameterValue(tableTypeName, columnMetaData);

      Assert.That(() => tvpValue.AddRecord("Not a Number"), Throws.InstanceOf<InvalidCastException>());
    }

    [Test]
    public void AddRecord_WithIncorrectNumberOfValues_ThrowsArgumentException1 ()
    {
      var tableTypeName = "Dummy";
      var columnMetaData = new[] { new SqlMetaData("DummyName", SqlDbType.Int), new SqlMetaData("DummyName", SqlDbType.Int) };

      var tvpValue = new SqlTableValuedParameterValue(tableTypeName, columnMetaData);

      Assert.That(
          () => tvpValue.AddRecord(12),
          Throws.InstanceOf<ArgumentException>().With.ArgumentExceptionMessageEqualTo("Record has 2 values but 1 was provided.", "columnValues"));
    }

    [Test]
    public void AddRecord_WithIncorrectNumberOfValues_ThrowsArgumentException2 ()
    {
      var tableTypeName = "Dummy";
      var columnMetaData = new[] { new SqlMetaData("DummyName", SqlDbType.Int) };

      var tvpValue = new SqlTableValuedParameterValue(tableTypeName, columnMetaData);

      Assert.That(
          () => tvpValue.AddRecord(12, 42),
          Throws.InstanceOf<ArgumentException>().With.ArgumentExceptionMessageEqualTo("Record has 1 value but 2 were provided.", "columnValues"));
    }

    [Test]
    public void GetEnumerator_Plain ()
    {
      var tableTypeName = "Dummy";
      var columnMetaData = new[] { new SqlMetaData("IntValue", SqlDbType.Int), new SqlMetaData("StringValue", SqlDbType.NVarChar, 100) };

      var tvpValue = new SqlTableValuedParameterValue(tableTypeName, columnMetaData);
      tvpValue.AddRecord(42, "Forty-two");
      tvpValue.AddRecord(17, "Seventeen");
      tvpValue.AddRecord(4, "Four");

      var enumerator = ((IEnumerable)tvpValue).GetEnumerator();
      Assert.That(enumerator.MoveNext, Is.True);
      CheckValues(enumerator.Current.As<SqlDataRecord>(), 42, "Forty-two");
      Assert.That(enumerator.MoveNext, Is.True);
      CheckValues(enumerator.Current.As<SqlDataRecord>(), 17, "Seventeen");
      Assert.That(enumerator.MoveNext, Is.True);
      CheckValues(enumerator.Current.As<SqlDataRecord>(), 4, "Four");
      Assert.That(enumerator.MoveNext, Is.False);
      (enumerator as IDisposable)?.Dispose();
    }

    [Test]
    public void GetEnumerator_Generic ()
    {
      var tableTypeName = "Dummy";
      var columnMetaData = new[] { new SqlMetaData("IntValue", SqlDbType.Int), new SqlMetaData("StringValue", SqlDbType.NVarChar, 100) };

      var tvpValue = new SqlTableValuedParameterValue(tableTypeName, columnMetaData);
      tvpValue.AddRecord(42, "Forty-two");
      tvpValue.AddRecord(17, "Seventeen");
      tvpValue.AddRecord(4, "Four");

      using var enumerator = ((IEnumerable<SqlDataRecord>)tvpValue).GetEnumerator();
      Assert.That(enumerator.MoveNext, Is.True);
      CheckValues(enumerator.Current, 42, "Forty-two");
      Assert.That(enumerator.MoveNext, Is.True);
      CheckValues(enumerator.Current, 17, "Seventeen");
      Assert.That(enumerator.MoveNext, Is.True);
      CheckValues(enumerator.Current, 4, "Four");
      Assert.That(enumerator.MoveNext, Is.False);
    }

    private void CheckValues (SqlDataRecord sqlDataRecord, params object[] expectedValues)
    {
      Assert.That(sqlDataRecord, Is.Not.Null);
      var actualValues = new object[sqlDataRecord.FieldCount];
      Assert.That(sqlDataRecord.GetValues(actualValues), Is.EqualTo(expectedValues.Length));
      Assert.That(actualValues, Is.EqualTo(expectedValues));
    }
  }
}
