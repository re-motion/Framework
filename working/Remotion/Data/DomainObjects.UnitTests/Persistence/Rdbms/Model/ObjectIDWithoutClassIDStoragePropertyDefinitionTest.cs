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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ObjectIDWithoutClassIDStoragePropertyDefinitionTest : StandardMappingTest
  {
    private ClassDefinition _classDefinition;
    private IRdbmsStoragePropertyDefinition _valuePropertyStub;

    private ObjectIDWithoutClassIDStoragePropertyDefinition _objectIDWithoutClassIDStoragePropertyDefinition;

    private IColumnValueProvider _columnValueProviderStub;
    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameterStub;
    private ColumnDefinition _valueColumnDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinition = DomainObjectIDs.Order1.ClassDefinition;

      _valueColumnDefinition = ColumnDefinitionObjectMother.CreateColumn();
      _valuePropertyStub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();

      _objectIDWithoutClassIDStoragePropertyDefinition = new ObjectIDWithoutClassIDStoragePropertyDefinition (
          _valuePropertyStub, _classDefinition);

      _columnValueProviderStub = MockRepository.GenerateStub<IColumnValueProvider> ();
      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter>();
      _dbCommandStub.Stub (stub => stub.CreateParameter()).Return (_dbDataParameterStub).Repeat.Once();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_objectIDWithoutClassIDStoragePropertyDefinition.ValueProperty, Is.SameAs (_valuePropertyStub));
      Assert.That (_objectIDWithoutClassIDStoragePropertyDefinition.ClassDefinition, Is.SameAs (_classDefinition));
    }

    [Test]
    public void Initialization_WithAbstractClassDefinition ()
    {
      var abstractClassDefinition = GetTypeDefinition (typeof (TIFileSystemItem));
      Assert.That (abstractClassDefinition.IsAbstract, Is.True);

      Assert.That (
          () => new ObjectIDWithoutClassIDStoragePropertyDefinition (_valuePropertyStub, abstractClassDefinition),
          Throws.ArgumentException.With.Message.EqualTo (
              "ObjectIDs without ClassIDs cannot have abstract ClassDefinitions.\r\nParameter name: classDefinition"));
    }

    [Test]
    public void PropertyType ()
    {
      Assert.That (_objectIDWithoutClassIDStoragePropertyDefinition.PropertyType, Is.SameAs (typeof (ObjectID)));
    }

    [Test]
    public void CanCreateForeignKeyConstraint ()
    {
      Assert.That (_objectIDWithoutClassIDStoragePropertyDefinition.CanCreateForeignKeyConstraint, Is.True);
    }

    [Test]
    public void GetColumnsForComparison ()
    {
      _valuePropertyStub.Stub (stub => stub.GetColumnsForComparison ()).Return (new[] { _valueColumnDefinition });

      Assert.That (_objectIDWithoutClassIDStoragePropertyDefinition.GetColumnsForComparison(), Is.EqualTo (new[] { _valueColumnDefinition }));
    }

    [Test]
    public void GetColumns ()
    {
      _valuePropertyStub.Stub (stub => stub.GetColumns()).Return (new[] { _valueColumnDefinition });
      Assert.That (_objectIDWithoutClassIDStoragePropertyDefinition.GetColumns(), Is.EqualTo (new[] { _valueColumnDefinition }));
    }

    [Test]
    public void SplitValue ()
    {
      var columnValue = new ColumnValue (_valueColumnDefinition, DomainObjectIDs.Order1);

      _valuePropertyStub.Stub (stub => stub.SplitValue (DomainObjectIDs.Order1.Value)).Return (new[] { columnValue });

      var result = _objectIDWithoutClassIDStoragePropertyDefinition.SplitValue (DomainObjectIDs.Order1);

      Assert.That (result, Is.EqualTo (new[] { columnValue }));
    }

    [Test]
    public void SplitValue_NullValue ()
    {
      var columnValue = new ColumnValue (_valueColumnDefinition, DomainObjectIDs.Order1);

      _valuePropertyStub.Stub (stub => stub.SplitValue (null)).Return (new[] { columnValue });

      var result = _objectIDWithoutClassIDStoragePropertyDefinition.SplitValue (null);

      Assert.That (result, Is.EqualTo (new[] { columnValue }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The specified ObjectID has an invalid ClassDefinition.\r\nParameter name: value")]
    public void SplitValue_InvalidClassDefinition ()
    {
      var columnValue = new ColumnValue (_valueColumnDefinition, DomainObjectIDs.OrderItem1);

      _valuePropertyStub.Stub (stub => stub.SplitValue (DomainObjectIDs.OrderItem1.Value)).Return (new[] { columnValue });

      _objectIDWithoutClassIDStoragePropertyDefinition.SplitValue (DomainObjectIDs.OrderItem1);
    }

    [Test]
    public void SplitValueForComparison ()
    {
      var columnValue1 = new ColumnValue (_valueColumnDefinition, null);
      _valuePropertyStub.Stub (stub => stub.SplitValueForComparison (DomainObjectIDs.Order1.Value)).Return (new[] { columnValue1 });

      var result = _objectIDWithoutClassIDStoragePropertyDefinition.SplitValueForComparison (DomainObjectIDs.Order1).ToArray ();

      Assert.That (result, Is.EqualTo (new[] { columnValue1 }));
    }

    [Test]
    public void SplitValueForComparison_NullValue ()
    {
      var columnValue1 = new ColumnValue (_valueColumnDefinition, null);
      _valuePropertyStub.Stub (stub => stub.SplitValueForComparison (null)).Return (new[] { columnValue1 });

      var result = _objectIDWithoutClassIDStoragePropertyDefinition.SplitValueForComparison (null).ToArray ();

      Assert.That (result, Is.EqualTo (new[] { columnValue1 }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The specified ObjectID has an invalid ClassDefinition.\r\nParameter name: value")]
    public void SplitValueForComparison_InvalidClassDefinition ()
    {
      _objectIDWithoutClassIDStoragePropertyDefinition.SplitValueForComparison (DomainObjectIDs.OrderItem2);
    }

    [Test]
    public void SplitValuesForComparison ()
    {
      var row1 = new ColumnValueTable.Row (new[] { "1" });
      var row2 = new ColumnValueTable.Row (new[] { "2" });
      var columnValueTable = new ColumnValueTable (new[] { _valueColumnDefinition }, new[] { row1, row2 });

      _valuePropertyStub
          .Stub (stub => stub.SplitValuesForComparison (Arg<IEnumerable<object>>.List.Equal (
              new[] { DomainObjectIDs.Order1.Value, DomainObjectIDs.Order3.Value })))
          .Return (columnValueTable);

      var result = _objectIDWithoutClassIDStoragePropertyDefinition.SplitValuesForComparison (new object[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });

      ColumnValueTableTestHelper.CheckTable (columnValueTable, result);
    }

    [Test]
    public void SplitValuesForComparison_NullValue ()
    {
      var row1 = new ColumnValueTable.Row (new[] { "1" });
      var row2 = new ColumnValueTable.Row (new[] { "2" });
      var columnValueTable = new ColumnValueTable (new[] { _valueColumnDefinition }, new[] { row1, row2 });

      // Bug in Rhino Mocks: List.Equal constraint cannot handle nulls within the sequence
      _valuePropertyStub
          .Stub (stub => stub.SplitValuesForComparison (
              Arg<IEnumerable<object>>.Matches (seq => seq.SequenceEqual (new[] { null, DomainObjectIDs.Order3.Value }))))
          .Return (columnValueTable);

      var result = _objectIDWithoutClassIDStoragePropertyDefinition.SplitValuesForComparison (new object[] { null, DomainObjectIDs.Order3 });

      ColumnValueTableTestHelper.CheckTable (columnValueTable, result);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The specified ObjectID has an invalid ClassDefinition.\r\nParameter name: values")]
    public void SplitValuesForComparison_InvalidClassDefinition ()
    {
      // Exception is only triggered when somebody actually accesses the arguments
      _valuePropertyStub
          .Stub (stub => stub.SplitValuesForComparison (Arg<IEnumerable<object>>.Is.Anything))
          .WhenCalled (mi => ((IEnumerable<object>) mi.Arguments[0]).ToArray())
          .Return (new ColumnValueTable());

      _objectIDWithoutClassIDStoragePropertyDefinition.SplitValuesForComparison (new object[] { DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2 })
          .Columns.ToArray();
    }

    [Test]
    public void CombineValue ()
    {
      _valuePropertyStub.Stub (stub => stub.CombineValue (_columnValueProviderStub)).Return (DomainObjectIDs.Order1.Value);

      var result = _objectIDWithoutClassIDStoragePropertyDefinition.CombineValue (_columnValueProviderStub);

      Assert.That (result, Is.TypeOf (typeof (ObjectID)));
      Assert.That (((ObjectID) result).Value.ToString (), Is.EqualTo (DomainObjectIDs.Order1.Value.ToString ()));
      Assert.That (((ObjectID) result).ClassDefinition, Is.SameAs (_classDefinition));
    }

    [Test]
    public void CombineValue_ValueIsNull_ReturnsNull ()
    {
      _valuePropertyStub.Stub (stub => stub.CombineValue (_columnValueProviderStub)).Return (null);

      var result = _objectIDWithoutClassIDStoragePropertyDefinition.CombineValue (_columnValueProviderStub);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void UnifyWithEquivalentProperties_CombinesProperties ()
    {
      var valueProperty1Mock = MockRepository.GenerateStrictMock<IRdbmsStoragePropertyDefinition> ();
      var property1 = new ObjectIDWithoutClassIDStoragePropertyDefinition (valueProperty1Mock, _classDefinition);

      var valueProperty2Stub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition> ();
      var property2 = new ObjectIDWithoutClassIDStoragePropertyDefinition (valueProperty2Stub, _classDefinition);

      var valueProperty3Stub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition> ();
      var property3 = new ObjectIDWithoutClassIDStoragePropertyDefinition (valueProperty3Stub, _classDefinition);

      var fakeUnifiedValueProperty = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition> ();
      valueProperty1Mock
          .Expect (
              mock => mock.UnifyWithEquivalentProperties (
                  Arg<IEnumerable<IRdbmsStoragePropertyDefinition>>.List.Equal (new[] { valueProperty2Stub, valueProperty3Stub })))
          .Return (fakeUnifiedValueProperty);

      var result = property1.UnifyWithEquivalentProperties (new[] { property2, property3 });

      fakeUnifiedValueProperty.VerifyAllExpectations ();

      Assert.That (result, Is.TypeOf<ObjectIDWithoutClassIDStoragePropertyDefinition> ());
      Assert.That (((ObjectIDWithoutClassIDStoragePropertyDefinition) result).ValueProperty, Is.SameAs (fakeUnifiedValueProperty));
      Assert.That (((ObjectIDWithoutClassIDStoragePropertyDefinition) result).ClassDefinition, Is.SameAs (_classDefinition));
    }

    [Test]
    public void UnifyWithEquivalentProperties_ThrowsForDifferentStoragePropertyType ()
    {
      var property2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty ();

      Assert.That (
          () => _objectIDWithoutClassIDStoragePropertyDefinition.UnifyWithEquivalentProperties (new[] { property2 }),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has type 'ObjectIDWithoutClassIDStoragePropertyDefinition', and the "
              + "given property has type 'SimpleStoragePropertyDefinition'.\r\nParameter name: equivalentProperties"));
    }

    [Test]
    public void UnifyWithEquivalentProperties_ThrowsForDifferentClassDefinitions ()
    {
      var property2 = new ObjectIDWithoutClassIDStoragePropertyDefinition (_valuePropertyStub, GetTypeDefinition (typeof (OrderItem)));

      Assert.That (
          () => _objectIDWithoutClassIDStoragePropertyDefinition.UnifyWithEquivalentProperties (new[] { property2 }),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has class definition "
              + "'Remotion.Data.DomainObjects.Mapping.ClassDefinition: Order', and the given property has "
              + "class definition 'Remotion.Data.DomainObjects.Mapping.ClassDefinition: OrderItem'.\r\nParameter name: equivalentProperties"));
    }

    [Test]
    public void CreateForeignKeyConstraint ()
    {
      var referencedColumnDefinition = ColumnDefinitionObjectMother.CreateColumn ("c2");
      
      var referencedValuePropertyStub = MockRepository.GenerateStub<IRdbmsStoragePropertyDefinition>();
      referencedValuePropertyStub.Stub (stub => stub.GetColumnsForComparison()).Return (new[] { referencedColumnDefinition });

      var referencedObjectIDProperty = new ObjectIDStoragePropertyDefinition (
          referencedValuePropertyStub,
          SimpleStoragePropertyDefinitionObjectMother.ClassIDProperty);

      _valuePropertyStub
          .Stub (stub => stub.GetColumnsForComparison())
          .Return (new[] { _valueColumnDefinition });
      
      var result = _objectIDWithoutClassIDStoragePropertyDefinition.CreateForeignKeyConstraint (
          cols =>
          {
            Assert.That (cols, Is.EqualTo (new[] { _valueColumnDefinition }));
            return "fkname";
          },
          new EntityNameDefinition ("entityschema", "entityname"),
          referencedObjectIDProperty);

      Assert.That (result.ConstraintName, Is.EqualTo ("fkname"));
      Assert.That (result.ReferencedTableName, Is.EqualTo (new EntityNameDefinition ("entityschema", "entityname")));
      Assert.That (result.ReferencingColumns, Is.EqualTo (new[] { _valueColumnDefinition }));
      Assert.That (result.ReferencedColumns, Is.EqualTo (new[] { referencedColumnDefinition }));
    }
  }
}