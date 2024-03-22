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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  [TestFixture]
  public class SerializedObjectIDStoragePropertyDefinitionTest : StandardMappingTest
  {
    private Mock<IRdbmsStoragePropertyDefinition> _serializedIDPropertyStub;
    private SerializedObjectIDStoragePropertyDefinition _serializedObjectIDStoragePropertyDefinition;

    private Mock<IColumnValueProvider> _columnValueProviderStub;
    private Mock<IDbCommand> _dbCommandStub;
    private Mock<IDbDataParameter> _dbDataParameterStub;
    private ColumnDefinition _columnDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _serializedIDPropertyStub = new Mock<IRdbmsStoragePropertyDefinition>();
      _serializedObjectIDStoragePropertyDefinition = new SerializedObjectIDStoragePropertyDefinition(_serializedIDPropertyStub.Object);

      _columnValueProviderStub = new Mock<IColumnValueProvider>();
      _dbCommandStub = new Mock<IDbCommand>();
      _dbDataParameterStub = new Mock<IDbDataParameter>();
      _dbCommandStub.Setup(stub => stub.CreateParameter()).Returns(_dbDataParameterStub.Object);
      _columnDefinition = ColumnDefinitionObjectMother.CreateColumn();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That(_serializedObjectIDStoragePropertyDefinition.SerializedIDProperty, Is.SameAs(_serializedIDPropertyStub.Object));
    }

    [Test]
    public void PropertyType ()
    {
      Assert.That(_serializedObjectIDStoragePropertyDefinition.PropertyType, Is.SameAs(typeof(ObjectID)));
    }

    [Test]
    public void CanCreateForeignKeyConstraint ()
    {
      Assert.That(_serializedObjectIDStoragePropertyDefinition.CanCreateForeignKeyConstraint, Is.False);
    }

    [Test]
    public void GetColumnsForComparison ()
    {
      _serializedIDPropertyStub.Setup(stub => stub.GetColumnsForComparison()).Returns(new[] { _columnDefinition });
      Assert.That(_serializedObjectIDStoragePropertyDefinition.GetColumnsForComparison(), Is.EqualTo(new[] { _columnDefinition }));
    }

    [Test]
    public void GetColumns ()
    {
      _serializedIDPropertyStub.Setup(stub => stub.GetColumns()).Returns(new[] { _columnDefinition });
      Assert.That(_serializedObjectIDStoragePropertyDefinition.GetColumns(), Is.EqualTo(_serializedIDPropertyStub.Object.GetColumns()));
    }

    [Test]
    public void SplitValue ()
    {
      var columnValue = new ColumnValue(_columnDefinition, DomainObjectIDs.OrderItem1);

      _serializedIDPropertyStub.Setup(stub => stub.SplitValue(DomainObjectIDs.OrderItem1.ToString())).Returns(new[] { columnValue });

      var result = _serializedObjectIDStoragePropertyDefinition.SplitValue(DomainObjectIDs.OrderItem1);

      Assert.That(result, Is.EqualTo(new[] { columnValue }));
    }

    [Test]
    public void SplitValue_NullObjectID ()
    {
      var columnValue = new ColumnValue(_columnDefinition, DomainObjectIDs.OrderItem1);

      _serializedIDPropertyStub.Setup(stub => stub.SplitValue(null)).Returns(new[] { columnValue });

      var result = _serializedObjectIDStoragePropertyDefinition.SplitValue(null);

      Assert.That(result, Is.EqualTo(new[] { columnValue }));
    }

    [Test]
    public void SplitValueForComparison ()
    {
      var columnValue1 = new ColumnValue(_columnDefinition, null);
      _serializedIDPropertyStub.Setup(stub => stub.SplitValueForComparison(DomainObjectIDs.Order1.ToString())).Returns(new[] { columnValue1 });

      var result = _serializedObjectIDStoragePropertyDefinition.SplitValueForComparison(DomainObjectIDs.Order1).ToArray();

      Assert.That(result, Is.EqualTo(new[] { columnValue1 }));
    }

    [Test]
    public void SplitValueForComparison_NullValue ()
    {
      var columnValue1 = new ColumnValue(_columnDefinition, null);
      _serializedIDPropertyStub.Setup(stub => stub.SplitValueForComparison(null)).Returns(new[] { columnValue1 });

      var result = _serializedObjectIDStoragePropertyDefinition.SplitValueForComparison(null).ToArray();

      Assert.That(result, Is.EqualTo(new[] { columnValue1 }));
    }

    [Test]
    public void SplitValuesForComparison ()
    {
      var row1 = new ColumnValueTable.Row(new[] { "1" });
      var row2 = new ColumnValueTable.Row(new[] { "2" });
      var columnValueTable = new ColumnValueTable(new[] { _columnDefinition }, new[] { row1, row2 });

      _serializedIDPropertyStub
          .Setup(stub => stub.SplitValuesForComparison(new[] { DomainObjectIDs.Order1.ToString(), DomainObjectIDs.Order3.ToString() }))
          .Returns(columnValueTable);

      var result = _serializedObjectIDStoragePropertyDefinition.SplitValuesForComparison(new object[] { DomainObjectIDs.Order1, DomainObjectIDs.Order3 });

      ColumnValueTableTestHelper.CheckTable(columnValueTable, result);
    }

    [Test]
    public void SplitValuesForComparison_NullValue ()
    {
      var row1 = new ColumnValueTable.Row(new[] { "1" });
      var row2 = new ColumnValueTable.Row(new[] { "2" });
      var columnValueTable = new ColumnValueTable(new[] { _columnDefinition }, new[] { row1, row2 });

      _serializedIDPropertyStub
          .Setup(stub => stub.SplitValuesForComparison(new[] { null, DomainObjectIDs.Order3.ToString() }))
          .Returns(columnValueTable);

      var result = _serializedObjectIDStoragePropertyDefinition.SplitValuesForComparison(new object[] { null, DomainObjectIDs.Order3 });

      ColumnValueTableTestHelper.CheckTable(columnValueTable, result);
    }

    [Test]
    public void CombineValue ()
    {
      _serializedIDPropertyStub.Setup(stub => stub.CombineValue(_columnValueProviderStub.Object)).Returns(DomainObjectIDs.Order1.ToString());

      var result = _serializedObjectIDStoragePropertyDefinition.CombineValue(_columnValueProviderStub.Object);

      Assert.That(result, Is.TypeOf(typeof(ObjectID)));
      Assert.That(((ObjectID)result).Value.ToString(), Is.EqualTo(DomainObjectIDs.Order1.Value.ToString()));
      Assert.That(((ObjectID)result).ClassID, Is.EqualTo("Order"));
    }

    [Test]
    public void CombineValue_ValueIsNull_ReturnsNull ()
    {
      _serializedIDPropertyStub.Setup(stub => stub.CombineValue(_columnValueProviderStub.Object)).Returns((object)null);

      var result = _serializedObjectIDStoragePropertyDefinition.CombineValue(_columnValueProviderStub.Object);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void UnifyWithEquivalentProperties_CombinesProperties ()
    {
      var serializedIDProperty1Mock = new Mock<IRdbmsStoragePropertyDefinition>(MockBehavior.Strict);
      var property1 = new SerializedObjectIDStoragePropertyDefinition(serializedIDProperty1Mock.Object);

      var serializedIDProperty2Stub = new Mock<IRdbmsStoragePropertyDefinition>();
      var property2 = new SerializedObjectIDStoragePropertyDefinition(serializedIDProperty2Stub.Object);

      var serializedIDProperty3Stub = new Mock<IRdbmsStoragePropertyDefinition>();
      var property3 = new SerializedObjectIDStoragePropertyDefinition(serializedIDProperty3Stub.Object);

      var fakeUnifiedSerializedIDProperty = new Mock<IRdbmsStoragePropertyDefinition>();
      serializedIDProperty1Mock
          .Setup(mock => mock.UnifyWithEquivalentProperties(new[] { serializedIDProperty2Stub.Object, serializedIDProperty3Stub.Object }))
          .Returns(fakeUnifiedSerializedIDProperty.Object)
          .Verifiable();

      var result = property1.UnifyWithEquivalentProperties(new[] { property2, property3 });

      fakeUnifiedSerializedIDProperty.Verify();

      Assert.That(result, Is.TypeOf<SerializedObjectIDStoragePropertyDefinition>());
      Assert.That(((SerializedObjectIDStoragePropertyDefinition)result).SerializedIDProperty, Is.SameAs(fakeUnifiedSerializedIDProperty.Object));
    }

    [Test]
    public void UnifyWithEquivalentProperties_ThrowsForDifferentStoragePropertyType ()
    {
      var property2 = SimpleStoragePropertyDefinitionObjectMother.CreateStorageProperty();

      Assert.That(
          () => _serializedObjectIDStoragePropertyDefinition.UnifyWithEquivalentProperties(new[] { property2 }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Only equivalent properties can be combined, but this property has type 'SerializedObjectIDStoragePropertyDefinition', and the "
              + "given property has type 'SimpleStoragePropertyDefinition'.", "equivalentProperties"));
    }

    [Test]
    public void CreateForeignKeyConstraint ()
    {
      Assert.That(
          () => _serializedObjectIDStoragePropertyDefinition.CreateForeignKeyConstraint(
          cols => { throw new Exception("Should not be called."); },
          new EntityNameDefinition("entityschema", "entityname"),
          ObjectIDStoragePropertyDefinitionObjectMother.ObjectIDProperty),
          Throws.InstanceOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "String-serialized ObjectID values cannot be used as foreign keys."));
    }
  }
}
