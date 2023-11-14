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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  [TestFixture]
  public class ObjectIDStoragePropertyDefinitionTest : StandardMappingTest
  {
    private ColumnDefinition _valueColumnDefinition;
    private ColumnDefinition _classIDColumnDefinition;

    private Mock<IRdbmsStoragePropertyDefinition> _valuePropertyStub;
    private Mock<IRdbmsStoragePropertyDefinition> _classIDPropertyStub;

    private ObjectIDStoragePropertyDefinition _objectIDStoragePropertyDefinition;

    private Mock<IColumnValueProvider> _columnValueProviderStub;
    private Mock<IDbDataParameter> _dbDataParameter1Stub;
    private Mock<IDbDataParameter> _dbDataParameter2Stub;
    private Mock<IDbCommand> _dbCommandStub;

    public override void SetUp ()
    {
      base.SetUp();

      _valueColumnDefinition = ColumnDefinitionObjectMother.CreateColumn("Column1");
      _classIDColumnDefinition = ColumnDefinitionObjectMother.CreateColumn("Column2");

      _valuePropertyStub = new Mock<IRdbmsStoragePropertyDefinition>();
      _classIDPropertyStub = new Mock<IRdbmsStoragePropertyDefinition>();
      _objectIDStoragePropertyDefinition = new ObjectIDStoragePropertyDefinition(_valuePropertyStub.Object, _classIDPropertyStub.Object);

      _columnValueProviderStub = new Mock<IColumnValueProvider>();
      _dbCommandStub = new Mock<IDbCommand>();
      _dbDataParameter1Stub = new Mock<IDbDataParameter>();
      _dbDataParameter2Stub = new Mock<IDbDataParameter>();
      _dbCommandStub.Setup(stub => stub.CreateParameter()).Returns(_dbDataParameter1Stub.Object);
      _dbCommandStub.Setup(stub => stub.CreateParameter()).Returns(_dbDataParameter2Stub.Object);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_objectIDStoragePropertyDefinition.ValueProperty, Is.SameAs(_valuePropertyStub.Object));
      Assert.That(_objectIDStoragePropertyDefinition.ClassIDProperty, Is.SameAs(_classIDPropertyStub.Object));
      Assert.That(_objectIDStoragePropertyDefinition.ValueProperty, Is.SameAs(_valuePropertyStub.Object));
      Assert.That(_objectIDStoragePropertyDefinition.ClassIDProperty, Is.SameAs(_classIDPropertyStub.Object));
    }

    [Test]
    public void PropertyType ()
    {
      Assert.That(_objectIDStoragePropertyDefinition.PropertyType, Is.SameAs(typeof(ObjectID)));
    }

    [Test]
    public void CanCreateForeignKeyConstraint ()
    {
      Assert.That(_objectIDStoragePropertyDefinition.CanCreateForeignKeyConstraint, Is.True);
    }

    [Test]
    public void GetColumnsForComparison ()
    {
      _valuePropertyStub.Setup(stub => stub.GetColumnsForComparison()).Returns(new[] { _valueColumnDefinition });
      _classIDPropertyStub.Setup(stub => stub.GetColumnsForComparison()).Returns(new[] { _classIDColumnDefinition });

      Assert.That(_objectIDStoragePropertyDefinition.GetColumnsForComparison(), Is.EqualTo(new[] { _valueColumnDefinition }));
    }

    [Test]
    public void GetColumns ()
    {
      _valuePropertyStub.Setup(stub => stub.GetColumns()).Returns(new[] { _valueColumnDefinition });
      _classIDPropertyStub.Setup(stub => stub.GetColumns()).Returns(new[] { _classIDColumnDefinition });

      Assert.That(_objectIDStoragePropertyDefinition.GetColumns(), Is.EqualTo(new[] { _valueColumnDefinition, _classIDColumnDefinition }));
    }

    [Test]
    public void SplitValue ()
    {
      var columnValue1 = new ColumnValue(_valueColumnDefinition, DomainObjectIDs.Order1);
      var columnValue2 = new ColumnValue(_classIDColumnDefinition, DomainObjectIDs.Order3);

      _valuePropertyStub.Setup(stub => stub.SplitValue(DomainObjectIDs.Order1.Value)).Returns(new[] { columnValue1 });
      _classIDPropertyStub.Setup(stub => stub.SplitValue(DomainObjectIDs.Order1.ClassID)).Returns(new[] { columnValue2 });

      var result = _objectIDStoragePropertyDefinition.SplitValue(DomainObjectIDs.Order1);

      Assert.That(result, Is.EqualTo(new[] { columnValue1, columnValue2 }));
    }

    [Test]
    public void SplitValue_NullValue ()
    {
      var columnValue1 = new ColumnValue(_valueColumnDefinition, null);
      var columnValue2 = new ColumnValue(_classIDColumnDefinition, null);

      _valuePropertyStub.Setup(stub => stub.SplitValue(null)).Returns(new[]{ columnValue1 });
      _classIDPropertyStub.Setup(stub => stub.SplitValue(null)).Returns(new[] { columnValue2});

      var result = _objectIDStoragePropertyDefinition.SplitValue(null);

      Assert.That(result, Is.EqualTo(new[] { columnValue1, columnValue2 }));
    }

    [Test]
    public void SplitValueForComparison ()
    {
      var columnValue1 = new ColumnValue(_valueColumnDefinition, 12);
      _valuePropertyStub.Setup(stub => stub.SplitValueForComparison(DomainObjectIDs.Order1.Value)).Returns(new[] { columnValue1 });

      var result = _objectIDStoragePropertyDefinition.SplitValueForComparison(DomainObjectIDs.Order1).ToArray();

      Assert.That(result, Is.EqualTo(new[] { columnValue1 }));
    }

    [Test]
    public void SplitValueForComparison_NullValue ()
    {
      var columnValue1 = new ColumnValue(_valueColumnDefinition, null);
      _valuePropertyStub.Setup(stub => stub.SplitValueForComparison(null)).Returns(new[] { columnValue1 });

      var result = _objectIDStoragePropertyDefinition.SplitValueForComparison(null).ToArray();

      Assert.That(result, Is.EqualTo(new[] { columnValue1 }));
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

      var result = _objectIDStoragePropertyDefinition.SplitValuesForComparison(new object[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });

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

      var result = _objectIDStoragePropertyDefinition.SplitValuesForComparison(new object[] { null, DomainObjectIDs.Order3 });

      ColumnValueTableTestHelper.CheckTable(columnValueTable, result);
    }

    [Test]
    public void CombineValue ()
    {
      _valuePropertyStub.Setup(stub => stub.CombineValue(_columnValueProviderStub.Object)).Returns(DomainObjectIDs.Order1.Value);
      _classIDPropertyStub.Setup(stub => stub.CombineValue(_columnValueProviderStub.Object)).Returns("Order");

      var result = _objectIDStoragePropertyDefinition.CombineValue(_columnValueProviderStub.Object);

      Assert.That(result, Is.TypeOf(typeof(ObjectID)));
      Assert.That(((ObjectID)result).Value.ToString(), Is.EqualTo(DomainObjectIDs.Order1.Value.ToString()));
      Assert.That(((ObjectID)result).ClassID, Is.EqualTo("Order"));
    }

    [Test]
    public void CombineValue_ValueAndClassIdIsNull_ReturnsNull ()
    {
      _valuePropertyStub.Setup(stub => stub.CombineValue(_columnValueProviderStub.Object)).Returns((object)null);
      _classIDPropertyStub.Setup(stub => stub.CombineValue(_columnValueProviderStub.Object)).Returns((object)null);

      var result = _objectIDStoragePropertyDefinition.CombineValue(_columnValueProviderStub.Object);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void CombineValue_ValueIsNullAndClassIDIsNotNull_ThrowsException ()
    {
      _classIDPropertyStub.Setup(stub => stub.GetColumns()).Returns(new[] { _classIDColumnDefinition });
      _classIDPropertyStub.Setup(stub => stub.CombineValue(_columnValueProviderStub.Object)).Returns("Order");
      Assert.That(
          () => _objectIDStoragePropertyDefinition.CombineValue(_columnValueProviderStub.Object),
          Throws.InstanceOf<RdbmsProviderException>()
              .With.Message.EqualTo("Incorrect database value encountered. The value read from 'Column2' must contain null."));
    }

    [Test]
    public void CombineValue_ValueIsNotNullAndClassIDIsNull_ThrowsException ()
    {
      _classIDPropertyStub.Setup(stub => stub.GetColumns()).Returns(new[] { _classIDColumnDefinition });
      _valuePropertyStub.Setup(stub => stub.CombineValue(_columnValueProviderStub.Object)).Returns(DomainObjectIDs.Order1.Value);
      Assert.That(
          () => _objectIDStoragePropertyDefinition.CombineValue(_columnValueProviderStub.Object),
          Throws.InstanceOf<RdbmsProviderException>()
              .With.Message.EqualTo("Incorrect database value encountered. The value read from 'Column2' must not contain null."));
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

      _valuePropertyStub.Setup(stub => stub.GetColumnsForComparison()).Returns(new[] { _valueColumnDefinition });

      var result = _objectIDStoragePropertyDefinition.CreateForeignKeyConstraint(
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

    [Test]
    public void UnifyWithEquivalentProperties_CombinesProperties ()
    {
      var valueProperty1Mock = new Mock<IRdbmsStoragePropertyDefinition>(MockBehavior.Strict);
      var classIDProperty1Mock = new Mock<IRdbmsStoragePropertyDefinition>(MockBehavior.Strict);
      var property1 = new ObjectIDStoragePropertyDefinition(valueProperty1Mock.Object, classIDProperty1Mock.Object);

      var valueProperty2Stub = new Mock<IRdbmsStoragePropertyDefinition>();
      var classIDProperty2Stub = new Mock<IRdbmsStoragePropertyDefinition>();
      var property2 = new ObjectIDStoragePropertyDefinition(valueProperty2Stub.Object, classIDProperty2Stub.Object);

      var valueProperty3Stub = new Mock<IRdbmsStoragePropertyDefinition>();
      var classIDProperty3Stub = new Mock<IRdbmsStoragePropertyDefinition>();
      var property3 = new ObjectIDStoragePropertyDefinition(valueProperty3Stub.Object, classIDProperty3Stub.Object);

      var fakeUnifiedValueProperty = new Mock<IRdbmsStoragePropertyDefinition>();
      valueProperty1Mock
          .Setup(mock => mock.UnifyWithEquivalentProperties(new[] { valueProperty2Stub.Object, valueProperty3Stub.Object }))
          .Returns(fakeUnifiedValueProperty.Object)
          .Verifiable();
      var fakeUnifiedClassIDProperty = new Mock<IRdbmsStoragePropertyDefinition>();
      classIDProperty1Mock
          .Setup(mock => mock.UnifyWithEquivalentProperties(new[] { classIDProperty2Stub.Object, classIDProperty3Stub.Object }))
          .Returns(fakeUnifiedClassIDProperty.Object)
          .Verifiable();

      var result = property1.UnifyWithEquivalentProperties(new[] { property2, property3 });

      fakeUnifiedValueProperty.Verify();
      fakeUnifiedClassIDProperty.Verify();

      Assert.That(result, Is.TypeOf<ObjectIDStoragePropertyDefinition>());
      Assert.That(((ObjectIDStoragePropertyDefinition)result).ValueProperty, Is.SameAs(fakeUnifiedValueProperty.Object));
      Assert.That(((ObjectIDStoragePropertyDefinition)result).ClassIDProperty, Is.SameAs(fakeUnifiedClassIDProperty.Object));
    }

    [Test]
    public void UnifyWithEquivalentProperties_ThrowsForDifferentStoragePropertyType ()
    {
      var property2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty();

      Assert.That(
          () => _objectIDStoragePropertyDefinition.UnifyWithEquivalentProperties(new[] { property2 }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Only equivalent properties can be combined, but this property has type 'ObjectIDStoragePropertyDefinition', and the given property has "
              + "type 'SimpleStoragePropertyDefinition'.", "equivalentProperties"));
    }
  }
}
