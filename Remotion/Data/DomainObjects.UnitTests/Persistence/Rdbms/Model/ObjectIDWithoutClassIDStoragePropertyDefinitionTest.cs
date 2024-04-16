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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ObjectIDWithoutClassIDStoragePropertyDefinitionTest : StandardMappingTest
  {
    private ClassDefinition _classDefinition;
    private Mock<IRdbmsStoragePropertyDefinition> _valuePropertyStub;

    private ObjectIDWithoutClassIDStoragePropertyDefinition _objectIDWithoutClassIDStoragePropertyDefinition;

    private Mock<IColumnValueProvider> _columnValueProviderStub;
    private Mock<IDbCommand> _dbCommandStub;
    private Mock<IDbDataParameter> _dbDataParameterStub;
    private ColumnDefinition _valueColumnDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinition = DomainObjectIDs.Order1.ClassDefinition;

      _valueColumnDefinition = ColumnDefinitionObjectMother.CreateColumn();
      _valuePropertyStub = new Mock<IRdbmsStoragePropertyDefinition>();

      _objectIDWithoutClassIDStoragePropertyDefinition = new ObjectIDWithoutClassIDStoragePropertyDefinition(
          _valuePropertyStub.Object, _classDefinition);

      _columnValueProviderStub = new Mock<IColumnValueProvider>();
      _dbCommandStub = new Mock<IDbCommand>();
      _dbDataParameterStub = new Mock<IDbDataParameter>();
      _dbCommandStub.Setup(stub => stub.CreateParameter()).Returns(_dbDataParameterStub.Object);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_objectIDWithoutClassIDStoragePropertyDefinition.ValueProperty, Is.SameAs(_valuePropertyStub.Object));
      Assert.That(_objectIDWithoutClassIDStoragePropertyDefinition.ClassDefinition, Is.SameAs(_classDefinition));
    }

    [Test]
    public void Initialization_WithAbstractClassDefinition ()
    {
      var abstractClassDefinition = GetClassDefinition(typeof(TIFileSystemItem));
      Assert.That(abstractClassDefinition.IsAbstract, Is.True);

      Assert.That(
          () => new ObjectIDWithoutClassIDStoragePropertyDefinition(_valuePropertyStub.Object, abstractClassDefinition),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "ObjectIDs without ClassIDs cannot have abstract ClassDefinitions.", "classDefinition"));
    }

    [Test]
    public void PropertyType ()
    {
      Assert.That(_objectIDWithoutClassIDStoragePropertyDefinition.PropertyType, Is.SameAs(typeof(ObjectID)));
    }

    [Test]
    public void CanCreateForeignKeyConstraint ()
    {
      Assert.That(_objectIDWithoutClassIDStoragePropertyDefinition.CanCreateForeignKeyConstraint, Is.True);
    }

    [Test]
    public void GetColumnsForComparison ()
    {
      _valuePropertyStub.Setup(stub => stub.GetColumnsForComparison()).Returns(new[] { _valueColumnDefinition });

      Assert.That(_objectIDWithoutClassIDStoragePropertyDefinition.GetColumnsForComparison(), Is.EqualTo(new[] { _valueColumnDefinition }));
    }

    [Test]
    public void GetColumns ()
    {
      _valuePropertyStub.Setup(stub => stub.GetColumns()).Returns(new[] { _valueColumnDefinition });
      Assert.That(_objectIDWithoutClassIDStoragePropertyDefinition.GetColumns(), Is.EqualTo(new[] { _valueColumnDefinition }));
    }

    [Test]
    public void SplitValue ()
    {
      var columnValue = new ColumnValue(_valueColumnDefinition, DomainObjectIDs.Order1);

      _valuePropertyStub.Setup(stub => stub.SplitValue(DomainObjectIDs.Order1.Value)).Returns(new[] { columnValue });

      var result = _objectIDWithoutClassIDStoragePropertyDefinition.SplitValue(DomainObjectIDs.Order1);

      Assert.That(result, Is.EqualTo(new[] { columnValue }));
    }

    [Test]
    public void SplitValue_NullValue ()
    {
      var columnValue = new ColumnValue(_valueColumnDefinition, DomainObjectIDs.Order1);

      _valuePropertyStub.Setup(stub => stub.SplitValue(null)).Returns(new[] { columnValue });

      var result = _objectIDWithoutClassIDStoragePropertyDefinition.SplitValue(null);

      Assert.That(result, Is.EqualTo(new[] { columnValue }));
    }

    [Test]
    public void SplitValue_InvalidClassDefinition ()
    {
      var columnValue = new ColumnValue(_valueColumnDefinition, DomainObjectIDs.OrderItem1);

      _valuePropertyStub.Setup(stub => stub.SplitValue(DomainObjectIDs.OrderItem1.Value)).Returns(new[] { columnValue });
      Assert.That(
          () => _objectIDWithoutClassIDStoragePropertyDefinition.SplitValue(DomainObjectIDs.OrderItem1),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The specified ObjectID has an invalid ClassDefinition.", "value"));
    }

    [Test]
    public void SplitValueForComparison ()
    {
      var columnValue1 = new ColumnValue(_valueColumnDefinition, null);
      _valuePropertyStub.Setup(stub => stub.SplitValueForComparison(DomainObjectIDs.Order1.Value)).Returns(new[] { columnValue1 });

      var result = _objectIDWithoutClassIDStoragePropertyDefinition.SplitValueForComparison(DomainObjectIDs.Order1).ToArray();

      Assert.That(result, Is.EqualTo(new[] { columnValue1 }));
    }

    [Test]
    public void SplitValueForComparison_NullValue ()
    {
      var columnValue1 = new ColumnValue(_valueColumnDefinition, null);
      _valuePropertyStub.Setup(stub => stub.SplitValueForComparison(null)).Returns(new[] { columnValue1 });

      var result = _objectIDWithoutClassIDStoragePropertyDefinition.SplitValueForComparison(null).ToArray();

      Assert.That(result, Is.EqualTo(new[] { columnValue1 }));
    }

    [Test]
    public void SplitValueForComparison_InvalidClassDefinition ()
    {
      Assert.That(
          () => _objectIDWithoutClassIDStoragePropertyDefinition.SplitValueForComparison(DomainObjectIDs.OrderItem2),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The specified ObjectID has an invalid ClassDefinition.", "value"));
    }

    [Test]
    public void SplitValuesForComparison ()
    {
      var row1 = new ColumnValueTable.Row(new[] { "1" });
      var row2 = new ColumnValueTable.Row(new[] { "2" });
      var columnValueTable = new ColumnValueTable(new[] { _valueColumnDefinition }, new[] { row1, row2 });

      _valuePropertyStub
          .Setup(stub => stub.SplitValuesForComparison(new[] { DomainObjectIDs.Order1.Value, DomainObjectIDs.Order3.Value }))
          .Returns(columnValueTable);

      var result = _objectIDWithoutClassIDStoragePropertyDefinition.SplitValuesForComparison(new object[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });

      ColumnValueTableTestHelper.CheckTable(columnValueTable, result);
    }

    [Test]
    public void SplitValuesForComparison_NullValue ()
    {
      var row1 = new ColumnValueTable.Row(new[] { "1" });
      var row2 = new ColumnValueTable.Row(new[] { "2" });
      var columnValueTable = new ColumnValueTable(new[] { _valueColumnDefinition }, new[] { row1, row2 });

      _valuePropertyStub
          .Setup(stub => stub.SplitValuesForComparison(new[] { null, DomainObjectIDs.Order3.Value }))
          .Returns(columnValueTable);

      var result = _objectIDWithoutClassIDStoragePropertyDefinition.SplitValuesForComparison(new object[] { null, DomainObjectIDs.Order3 });

      ColumnValueTableTestHelper.CheckTable(columnValueTable, result);
    }

    [Test]
    public void SplitValuesForComparison_InvalidClassDefinition ()
    {
      // Exception is only triggered when somebody actually accesses the arguments
      _valuePropertyStub
          .Setup(stub => stub.SplitValuesForComparison(It.IsAny<IEnumerable<object>>()))
          .Callback((IEnumerable<object> values) => values.ToArray())
          .Returns(new ColumnValueTable());
      Assert.That(
          () => _objectIDWithoutClassIDStoragePropertyDefinition.SplitValuesForComparison(new object[] { DomainObjectIDs.OrderItem1, DomainObjectIDs.OrderItem2 })
          .Columns.ToArray(),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The specified ObjectID has an invalid ClassDefinition.", "values"));
    }

    [Test]
    public void CombineValue ()
    {
      _valuePropertyStub.Setup(stub => stub.CombineValue(_columnValueProviderStub.Object)).Returns(DomainObjectIDs.Order1.Value);

      var result = _objectIDWithoutClassIDStoragePropertyDefinition.CombineValue(_columnValueProviderStub.Object);

      Assert.That(result, Is.TypeOf(typeof(ObjectID)));
      Assert.That(((ObjectID)result).Value.ToString(), Is.EqualTo(DomainObjectIDs.Order1.Value.ToString()));
      Assert.That(((ObjectID)result).ClassDefinition, Is.SameAs(_classDefinition));
    }

    [Test]
    public void CombineValue_ValueIsNull_ReturnsNull ()
    {
      _valuePropertyStub.Setup(stub => stub.CombineValue(_columnValueProviderStub.Object)).Returns((object)null);

      var result = _objectIDWithoutClassIDStoragePropertyDefinition.CombineValue(_columnValueProviderStub.Object);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void UnifyWithEquivalentProperties_CombinesProperties ()
    {
      var valueProperty1Mock = new Mock<IRdbmsStoragePropertyDefinition>(MockBehavior.Strict);
      var property1 = new ObjectIDWithoutClassIDStoragePropertyDefinition(valueProperty1Mock.Object, _classDefinition);

      var valueProperty2Stub = new Mock<IRdbmsStoragePropertyDefinition>();
      var property2 = new ObjectIDWithoutClassIDStoragePropertyDefinition(valueProperty2Stub.Object, _classDefinition);

      var valueProperty3Stub = new Mock<IRdbmsStoragePropertyDefinition>();
      var property3 = new ObjectIDWithoutClassIDStoragePropertyDefinition(valueProperty3Stub.Object, _classDefinition);

      var fakeUnifiedValueProperty = new Mock<IRdbmsStoragePropertyDefinition>();
      valueProperty1Mock
          .Setup(mock => mock.UnifyWithEquivalentProperties(new[] { valueProperty2Stub.Object, valueProperty3Stub.Object }))
          .Returns(fakeUnifiedValueProperty.Object)
          .Verifiable();

      var result = property1.UnifyWithEquivalentProperties(new[] { property2, property3 });

      fakeUnifiedValueProperty.Verify();

      Assert.That(result, Is.TypeOf<ObjectIDWithoutClassIDStoragePropertyDefinition>());
      Assert.That(((ObjectIDWithoutClassIDStoragePropertyDefinition)result).ValueProperty, Is.SameAs(fakeUnifiedValueProperty.Object));
      Assert.That(((ObjectIDWithoutClassIDStoragePropertyDefinition)result).ClassDefinition, Is.SameAs(_classDefinition));
    }

    [Test]
    public void UnifyWithEquivalentProperties_ThrowsForDifferentStoragePropertyType ()
    {
      var property2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty();

      Assert.That(
          () => _objectIDWithoutClassIDStoragePropertyDefinition.UnifyWithEquivalentProperties(new[] { property2 }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Only equivalent properties can be combined, but this property has type 'ObjectIDWithoutClassIDStoragePropertyDefinition', and the "
              + "given property has type 'SimpleStoragePropertyDefinition'.", "equivalentProperties"));
    }

    [Test]
    public void UnifyWithEquivalentProperties_ThrowsForDifferentClassDefinitions ()
    {
      var property2 = new ObjectIDWithoutClassIDStoragePropertyDefinition(_valuePropertyStub.Object, GetClassDefinition(typeof(OrderItem)));

      Assert.That(
          () => _objectIDWithoutClassIDStoragePropertyDefinition.UnifyWithEquivalentProperties(new[] { property2 }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Only equivalent properties can be combined, but this property has class definition "
              + "'ClassDefinition: Order', and the given property has "
              + "class definition 'ClassDefinition: OrderItem'.", "equivalentProperties"));
    }

    [Test]
    public void CreateForeignKeyConstraint ()
    {
      var referencedColumnDefinition = ColumnDefinitionObjectMother.CreateColumn("c2");

      var referencedValuePropertyStub = new Mock<IRdbmsStoragePropertyDefinition>();
      referencedValuePropertyStub.Setup(stub => stub.GetColumnsForComparison()).Returns(new[] { referencedColumnDefinition });

      var referencedObjectIDProperty = new ObjectIDStoragePropertyDefinition(
          referencedValuePropertyStub.Object,
          SimpleStoragePropertyDefinitionObjectMother.ClassIDProperty);

      _valuePropertyStub
          .Setup(stub => stub.GetColumnsForComparison())
          .Returns(new[] { _valueColumnDefinition });

      var result = _objectIDWithoutClassIDStoragePropertyDefinition.CreateForeignKeyConstraint(
          cols =>
          {
            Assert.That(cols, Is.EqualTo(new[] { _valueColumnDefinition }));
            return "fkname";
          },
          new EntityNameDefinition("entityschema", "entityname"),
          referencedObjectIDProperty);

      Assert.That(result.ConstraintName, Is.EqualTo("fkname"));
      Assert.That(result.ReferencedTableName, Is.EqualTo(new EntityNameDefinition("entityschema", "entityname")));
      Assert.That(result.ReferencingColumns, Is.EqualTo(new[] { _valueColumnDefinition }));
      Assert.That(result.ReferencedColumns, Is.EqualTo(new[] { referencedColumnDefinition }));
    }
  }
}
