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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Linq.SqlBackend.SqlStatementModel;

namespace Remotion.Data.DomainObjects.UnitTests.Linq;

[TestFixture]
public class TableValuedParameterSqlCommandBuilderTest
{
  [Test]
  public void AppendCollection_Empty_UsesNoParameter ()
  {
    var collection = Array.Empty<int>();
    var constantCollectionExpression = new ConstantCollectionExpression(collection);

    var sqlCommandBuilder = new TableValuedParameterSqlCommandBuilder(1);
    sqlCommandBuilder.AppendCollection(constantCollectionExpression);

    Assert.That(sqlCommandBuilder.GetCommandText(), Is.EqualTo("SELECT NULL WHERE 1 = 0"));
    Assert.That(sqlCommandBuilder.GetCommandParameters().Length, Is.EqualTo(0));
  }

  private static IEnumerable<IEnumerable> GetCollectionsOfDifferentTypes ()
  {
    var data = new[] { 17, 4, 42 };

    yield return new CollectionAndEnumerableOfTStub<int>(data);
    yield return new CollectionStub<int>(data);
    yield return new ReadOnlyCollectionStub<int>(data);
    yield return new ReadOnlyCollectionStub<object>(data.Cast<object>().ToArray());
  }

  [Test]
  [TestCaseSource(nameof(GetCollectionsOfDifferentTypes))]
  public void AppendCollection_CountAtThreshold_UsesSingleParameter (IEnumerable collection)
  {
    var constantCollectionExpression = new ConstantCollectionExpression(collection);

    var sqlCommandBuilder = new TableValuedParameterSqlCommandBuilder(3);
    sqlCommandBuilder.AppendCollection(constantCollectionExpression);

    Assert.That(sqlCommandBuilder.GetCommandText(), Is.EqualTo("SELECT [Value] FROM @1"));
    Assert.That(sqlCommandBuilder.GetCommandParameters().Length, Is.EqualTo(1));

    var parameter = sqlCommandBuilder.GetCommandParameters().Single();
    Assert.That(parameter.Name, Is.EqualTo("@1"));
    Assert.That(parameter.Value, Is.SameAs(collection));
  }

  [Test]
  [TestCaseSource(nameof(GetCollectionsOfDifferentTypes))]
  public void AppendCollection_CountAboveThreshold_UsesSingleParameter (IEnumerable collection)
  {
    var constantCollectionExpression = new ConstantCollectionExpression(collection);

    var sqlCommandBuilder = new TableValuedParameterSqlCommandBuilder(2);
    sqlCommandBuilder.AppendCollection(constantCollectionExpression);

    Assert.That(sqlCommandBuilder.GetCommandText(), Is.EqualTo("SELECT [Value] FROM @1"));
    Assert.That(sqlCommandBuilder.GetCommandParameters().Length, Is.EqualTo(1));

    var parameter = sqlCommandBuilder.GetCommandParameters().Single();
    Assert.That(parameter.Name, Is.EqualTo("@1"));
    Assert.That(parameter.Value, Is.SameAs(collection));
  }

  [Test]
  [TestCaseSource(nameof(GetCollectionsOfDifferentTypes))]
  public void AppendCollection_CountBelowThreshold_UsesMultipleParameters (IEnumerable collection)
  {
    var constantCollectionExpression = new ConstantCollectionExpression(collection);

    var sqlCommandBuilder = new TableValuedParameterSqlCommandBuilder(4);
    sqlCommandBuilder.AppendCollection(constantCollectionExpression);

    Assert.That(sqlCommandBuilder.GetCommandText(), Is.EqualTo("@1, @2, @3"));
    Assert.That(sqlCommandBuilder.GetCommandParameters().Length, Is.EqualTo(3));

    var parameter = sqlCommandBuilder.GetCommandParameters();
    Assert.That(parameter[0].Name, Is.EqualTo("@1"));
    Assert.That(parameter[0].Value, Is.EqualTo(17));
    Assert.That(parameter[1].Name, Is.EqualTo("@2"));
    Assert.That(parameter[1].Value, Is.EqualTo(4));
    Assert.That(parameter[2].Name, Is.EqualTo("@3"));
    Assert.That(parameter[2].Value, Is.EqualTo(42));
  }

  [Test]
  [TestCase(0)]
  [TestCase(-1)]
  [TestCase(-42)]
  [TestCase(int.MinValue)]
  public void AppendCollection_InvalidThreshold_ThrowsException (int threshold)
  {
    Assert.That(() => new TableValuedParameterSqlCommandBuilder(threshold), Throws.InstanceOf<ArgumentOutOfRangeException>());
  }

  [Test]
  public void AppendCollection_NonCollectionEnumerable_ThrowsException ()
  {
    var data = new[] { 17, 4, 42 };
    var enumerableStub = new Mock<IEnumerable>();
    enumerableStub.Setup(o => o.GetEnumerator()).Returns(data.GetEnumerator());
    var constantCollectionExpression = new ConstantCollectionExpression(enumerableStub.Object);

    var sqlCommandBuilder = new TableValuedParameterSqlCommandBuilder(1);
    Assert.That(() => sqlCommandBuilder.AppendCollection(constantCollectionExpression), Throws.InstanceOf<NotSupportedException>());
  }

  private class CollectionAndEnumerableOfTStub<T> : ICollection, IEnumerable<T>
  {
    private readonly T[] _innerCollection;

    public CollectionAndEnumerableOfTStub (IEnumerable<T> values)
    {
      _innerCollection = values.ToArray();
    }

    public IEnumerator GetEnumerator () => _innerCollection.GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator () => ((IEnumerable<T>)_innerCollection).GetEnumerator();
    public void CopyTo (Array array, int index) => ((ICollection)_innerCollection).CopyTo(array, index);
    public int Count => _innerCollection.Length;
    public object SyncRoot => ((ICollection)_innerCollection).SyncRoot;
    public bool IsSynchronized => ((ICollection)_innerCollection).IsSynchronized;
  }

  private class CollectionStub<T> : ICollection<T>
  {
    private readonly ICollection<T> _innerCollection;

    public CollectionStub (IEnumerable<T> values)
    {
      _innerCollection = values.ToArray();
    }

    public IEnumerator<T> GetEnumerator () => _innerCollection.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    public void Add (T item) => _innerCollection.Add(item);
    public void Clear () => _innerCollection.Clear();
    public bool Contains (T item) => _innerCollection.Contains(item);
    public void CopyTo (T[] array, int arrayIndex) => _innerCollection.CopyTo(array, arrayIndex);
    public bool Remove (T item) => _innerCollection.Remove(item);
    public int Count => _innerCollection.Count;
    public bool IsReadOnly => _innerCollection.IsReadOnly;
  }

  private class ReadOnlyCollectionStub<T> : IReadOnlyCollection<T>
  {
    private readonly IReadOnlyCollection<T> _innerCollection;

    public ReadOnlyCollectionStub (IEnumerable<T> values)
    {
      _innerCollection = values.ToArray();
    }

    public IEnumerator<T> GetEnumerator () => _innerCollection.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    public int Count => _innerCollection.Count;
  }
}
