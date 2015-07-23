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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  [TestFixture]
  public class SerializedObjectIDStoragePropertyDefinitionTest : StandardMappingTest
  {
    private IRdbmsStoragePropertyDefinition _serializedIDPropertyStub;
    private SerializedObjectIDStoragePropertyDefinition _serializedObjectIDStoragePropertyDefinition;

    private IColumnValueProvider _columnValueProviderStub;
    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameterStub;
    private ColumnDefinition _columnDefinition;

    public override void SetUp ()
    {
      base.SetUp();
      
      _serializedIDPropertyStub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      _serializedObjectIDStoragePropertyDefinition = new SerializedObjectIDStoragePropertyDefinition (_serializedIDPropertyStub);

      _columnValueProviderStub = MockRepository.GenerateStub<IColumnValueProvider> ();
      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter>();
      _dbCommandStub.Stub (stub => stub.CreateParameter()).Return (_dbDataParameterStub).Repeat.Once();
      _columnDefinition = ColumnDefinitionObjectMother.CreateColumn ();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.SerializedIDProperty, Is.SameAs (_serializedIDPropertyStub));
    }

    [Test]
    public void PropertyType ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.PropertyType, Is.SameAs (typeof (ObjectID)));
    }

    [Test]
    public void CanCreateForeignKeyConstraint ()
    {
      Assert.That (_serializedObjectIDStoragePropertyDefinition.CanCreateForeignKeyConstraint, Is.False);
    }

    [Test]
    public void GetColumnsForComparison ()
    {
      _serializedIDPropertyStub.Stub (stub => stub.GetColumnsForComparison()).Return (new[] { _columnDefinition });
      Assert.That (_serializedObjectIDStoragePropertyDefinition.GetColumnsForComparison (), Is.EqualTo (new[] { _columnDefinition }));
    }

    [Test]
    public void GetColumns ()
    {
      _serializedIDPropertyStub.Stub (stub => stub.GetColumns ()).Return (new[] { _columnDefinition });
      Assert.That (_serializedObjectIDStoragePropertyDefinition.GetColumns(), Is.EqualTo (_serializedIDPropertyStub.GetColumns()));
    }

    [Test]
    public void SplitValue ()
    {
      var columnValue = new ColumnValue (_columnDefinition, DomainObjectIDs.OrderItem1);

      _serializedIDPropertyStub.Stub (stub => stub.SplitValue (DomainObjectIDs.OrderItem1.ToString())).Return (new[] { columnValue });

      var result = _serializedObjectIDStoragePropertyDefinition.SplitValue (DomainObjectIDs.OrderItem1);

      Assert.That (result, Is.EqualTo (new[] { columnValue }));
    }

    [Test]
    public void SplitValue_NullObjectID ()
    {
      var columnValue = new ColumnValue (_columnDefinition, DomainObjectIDs.OrderItem1);

      _serializedIDPropertyStub.Stub (stub => stub.SplitValue (null)).Return (new[] { columnValue });

      var result = _serializedObjectIDStoragePropertyDefinition.SplitValue (null);

      Assert.That (result, Is.EqualTo (new[] { columnValue }));
    }

    [Test]
    public void SplitValueForComparison ()
    {
      var columnValue1 = new ColumnValue (_columnDefinition, null);
      _serializedIDPropertyStub.Stub (stub => stub.SplitValueForComparison (DomainObjectIDs.Order1.ToString ())).Return (new[] { columnValue1 });

      var result = _serializedObjectIDStoragePropertyDefinition.SplitValueForComparison (DomainObjectIDs.Order1).ToArray ();

      Assert.That (result, Is.EqualTo (new[] { columnValue1 }));
    }

    [Test]
    public void SplitValueForComparison_NullValue ()
    {
      var columnValue1 = new ColumnValue (_columnDefinition, null);
      _serializedIDPropertyStub.Stub (stub => stub.SplitValueForComparison (null)).Return (new[] { columnValue1 });

      var result = _serializedObjectIDStoragePropertyDefinition.SplitValueForComparison (null).ToArray ();

      Assert.That (result, Is.EqualTo (new[] { columnValue1 }));
    }

    [Test]
    public void SplitValuesForComparison ()
    {
      var row1 = new ColumnValueTable.Row (new[] { "1" });
      var row2 = new ColumnValueTable.Row (new[] { "2" });
      var columnValueTable = new ColumnValueTable (new[] { _columnDefinition }, new[] { row1, row2 });

      _serializedIDPropertyStub
          .Stub (stub => stub.SplitValuesForComparison (Arg<IEnumerable<object>>.List.Equal (
              new[] { DomainObjectIDs.Order1.ToString (), DomainObjectIDs.Order3.ToString () })))
          .Return (columnValueTable);

      var result = _serializedObjectIDStoragePropertyDefinition.SplitValuesForComparison (new object[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });

      ColumnValueTableTestHelper.CheckTable (columnValueTable, result);
    }

    [Test]
    public void SplitValuesForComparison_NullValue ()
    {
      var row1 = new ColumnValueTable.Row (new[] { "1" });
      var row2 = new ColumnValueTable.Row (new[] { "2" });
      var columnValueTable = new ColumnValueTable (new[] { _columnDefinition }, new[] { row1, row2 });

      // Bug in Rhino Mocks: List.Equal constraint cannot handle nulls within the sequence
      _serializedIDPropertyStub
          .Stub (stub => stub.SplitValuesForComparison (
              Arg<IEnumerable<object>>.Matches (seq => seq.SequenceEqual (new[] { null, DomainObjectIDs.Order3.ToString () }))))
          .Return (columnValueTable);

      var result = _serializedObjectIDStoragePropertyDefinition.SplitValuesForComparison (new object[] { null, DomainObjectIDs.Order3 });

      ColumnValueTableTestHelper.CheckTable (columnValueTable, result);
    }

    [Test]
    public void CombineValue ()
    {
      _serializedIDPropertyStub.Stub (stub => stub.CombineValue (_columnValueProviderStub)).Return (DomainObjectIDs.Order1.ToString ());

      var result = _serializedObjectIDStoragePropertyDefinition.CombineValue (_columnValueProviderStub);

      Assert.That (result, Is.TypeOf (typeof (ObjectID)));
      Assert.That (((ObjectID) result).Value.ToString (), Is.EqualTo (DomainObjectIDs.Order1.Value.ToString ()));
      Assert.That (((ObjectID) result).ClassID, Is.EqualTo ("Order"));
    }

    [Test]
    public void CombineValue_ValueIsNull_ReturnsNull ()
    {
      _serializedIDPropertyStub.Stub (stub => stub.CombineValue (_columnValueProviderStub)).Return (null);

      var result = _serializedObjectIDStoragePropertyDefinition.CombineValue (_columnValueProviderStub);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void UnifyWithEquivalentProperties_CombinesProperties ()
    {
      var serializedIDProperty1Mock = MockRepository.GenerateStrictMock<IRdbmsStoragePropertyDefinition> ();
      var property1 = new SerializedObjectIDStoragePropertyDefinition (serializedIDProperty1Mock);

      var serializedIDProperty2Stub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition> ();
      var property2 = new SerializedObjectIDStoragePropertyDefinition (serializedIDProperty2Stub);

      var serializedIDProperty3Stub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition> ();
      var property3 = new SerializedObjectIDStoragePropertyDefinition (serializedIDProperty3Stub);

      var fakeUnifiedSerializedIDProperty = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition> ();
      serializedIDProperty1Mock
          .Expect (
              mock => mock.UnifyWithEquivalentProperties (
                  Arg<IEnumerable<IRdbmsStoragePropertyDefinition>>.List.Equal (new[] { serializedIDProperty2Stub, serializedIDProperty3Stub })))
          .Return (fakeUnifiedSerializedIDProperty);

      var result = property1.UnifyWithEquivalentProperties (new[] { property2, property3 });

      fakeUnifiedSerializedIDProperty.VerifyAllExpectations ();

      Assert.That (result, Is.TypeOf<SerializedObjectIDStoragePropertyDefinition> ());
      Assert.That (((SerializedObjectIDStoragePropertyDefinition) result).SerializedIDProperty, Is.SameAs (fakeUnifiedSerializedIDProperty));
    }

    [Test]
    public void UnifyWithEquivalentProperties_ThrowsForDifferentStoragePropertyType ()
    {
      var property2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ();

      Assert.That (
          () => _serializedObjectIDStoragePropertyDefinition.UnifyWithEquivalentProperties (new[] { property2 }),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has type 'SerializedObjectIDStoragePropertyDefinition', and the "
              + "given property has type 'SimpleStoragePropertyDefinition'.\r\nParameter name: equivalentProperties"));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "String-serialized ObjectID values cannot be used as foreign keys.")]
    public void CreateForeignKeyConstraint ()
    {
      _serializedObjectIDStoragePropertyDefinition.CreateForeignKeyConstraint (
          cols => { throw new Exception ("Should not be called."); }, 
          new EntityNameDefinition ("entityschema", "entityname"), 
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty);
    }
  }
}