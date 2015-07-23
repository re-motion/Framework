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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Development.UnitTesting.ObjectMothers;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  [TestFixture]
  public class SimpleStoragePropertyDefinitionTest
  {
    private IStorageTypeInformation _storageTypeInformationStub;
    private ColumnDefinition _innerColumnDefinition;
    private SimpleStoragePropertyDefinition _storagePropertyDefinition;

    private IDbCommand _dbCommandStub;
    private IDbDataParameter _dbDataParameterStub;

    [SetUp]
    public void SetUp ()
    {
      _storageTypeInformationStub = MockRepository.GenerateStub<IStorageTypeInformation>();
      _innerColumnDefinition = ColumnDefinitionObjectMother.CreateColumn (storageTypeInformation: _storageTypeInformationStub);
      _storagePropertyDefinition = new SimpleStoragePropertyDefinition (typeof (string), _innerColumnDefinition);

      _dbCommandStub = MockRepository.GenerateStub<IDbCommand>();
      _dbDataParameterStub = MockRepository.GenerateStub<IDbDataParameter> ();
      _dbCommandStub.Stub (stub => stub.CreateParameter ()).Return (_dbDataParameterStub);
    }

    [Test]
    public void ColumnDefinition ()
    {
      Assert.That (_storagePropertyDefinition.ColumnDefinition, Is.SameAs (_innerColumnDefinition));
    }

    [Test]
    public void GetColumns ()
    {
      Assert.That (_storagePropertyDefinition.GetColumns (), Is.EqualTo (new[] { _innerColumnDefinition }));
    }

    [Test]
    public void GetColumnsForComparison ()
    {
      Assert.That (_storagePropertyDefinition.GetColumnsForComparison(), Is.EqualTo (new[] { _innerColumnDefinition }));
    }

    [Test]
    public void SplitValue ()
    {
      var value = new object();

      var result = _storagePropertyDefinition.SplitValue (value);

      Assert.That (result, Is.EqualTo (new[] { new ColumnValue(_innerColumnDefinition, value) }));
    }

    [Test]
    public void SplitValue_NullValue ()
    {
      var result = _storagePropertyDefinition.SplitValue (null);

      Assert.That (result, Is.EqualTo (new[] { new ColumnValue (_innerColumnDefinition, null) }));
    }

    [Test]
    public void SplitValueForComparison ()
    {
      var value = new object ();

      var result = _storagePropertyDefinition.SplitValueForComparison (value);

      Assert.That (result, Is.EqualTo (new[] { new ColumnValue (_innerColumnDefinition, value) }));
    }

    [Test]
    public void SplitValueForComparison_NullValue ()
    {
      var result = _storagePropertyDefinition.SplitValueForComparison (null);

      Assert.That (result, Is.EqualTo (new[] { new ColumnValue (_innerColumnDefinition, null) }));
    }

    [Test]
    public void SplitValuesForComparison ()
    {
      var value1 = new object ();
      var value2 = new object ();

      var result = _storagePropertyDefinition.SplitValuesForComparison (new[] { value1, value2 });

      var expectedTable = new ColumnValueTable (
          new[] { _innerColumnDefinition }, 
          new[]
          {
              new ColumnValueTable.Row (new[] { value1 }), 
              new ColumnValueTable.Row (new[] { value2 })
          });
      ColumnValueTableTestHelper.CheckTable (expectedTable, result);
    }

    [Test]
    public void SplitValuesForComparison_NullValue ()
    {
      var value2 = new object ();

      var result = _storagePropertyDefinition.SplitValuesForComparison (new[] { null, value2 });

      var expectedTable = new ColumnValueTable (
          new[] { _innerColumnDefinition },
          new[]
          {
              new ColumnValueTable.Row (new object[] { null }), 
              new ColumnValueTable.Row (new[] { value2 })
          });
      ColumnValueTableTestHelper.CheckTable (expectedTable, result);
    }

    [Test]
    public void CombineValue ()
    {
      var columnValueProviderStub = MockRepository.GenerateStub<IColumnValueProvider>();
      columnValueProviderStub.Stub (stub => stub.GetValueForColumn (_innerColumnDefinition)).Return (12);

      var result = _storagePropertyDefinition.CombineValue (columnValueProviderStub);

      Assert.That (result, Is.EqualTo (12));
    }

    [Test]
    public void UnifyWithEquivalentProperties_CombinesPropertiesAndColumns ()
    {
      var isPartOfPrimaryKey = BooleanObjectMother.GetRandomBoolean();
      var property1 = new SimpleStoragePropertyDefinition (
          typeof (int),
          ColumnDefinitionObjectMother.CreateColumn ("Col", CreateDefinedStorageTypeInformation (false), isPartOfPrimaryKey));
      var property2 = new SimpleStoragePropertyDefinition (
          typeof (int),
          ColumnDefinitionObjectMother.CreateColumn ("Col", CreateDefinedStorageTypeInformation (false), isPartOfPrimaryKey));
      var property3 = new SimpleStoragePropertyDefinition (
          typeof (int),
          ColumnDefinitionObjectMother.CreateColumn ("Col", CreateDefinedStorageTypeInformation (true), isPartOfPrimaryKey));

      var result = property1.UnifyWithEquivalentProperties (new[] { property2, property3 });

      Assert.That (result, Is.TypeOf<SimpleStoragePropertyDefinition> ().With.Property ("PropertyType").SameAs (typeof (int)));
      Assert.That (((SimpleStoragePropertyDefinition) result).ColumnDefinition.Name, Is.EqualTo ("Col"));
      Assert.That (((SimpleStoragePropertyDefinition) result).ColumnDefinition.StorageTypeInfo.IsStorageTypeNullable, Is.True);
      Assert.That (((SimpleStoragePropertyDefinition) result).ColumnDefinition.IsPartOfPrimaryKey, Is.EqualTo (isPartOfPrimaryKey));
    }

    [Test]
    public void UnifyWithEquivalentProperties_ThrowsForDifferentStoragePropertyType ()
    {
      var property2 = CompoundStoragePropertyDefinitionObjectMother.CreateWithTwoProperties();

      Assert.That (
          () => _storagePropertyDefinition.UnifyWithEquivalentProperties (new[] { property2 }),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has type 'SimpleStoragePropertyDefinition', and the given property has "
              + "type 'CompoundStoragePropertyDefinition'.\r\nParameter name: equivalentProperties"));
    }

    [Test]
    public void UnifyWithEquivalentProperties_ThrowsForDifferentPropertyType ()
    {
      var columnDefinition = ColumnDefinitionObjectMother.CreateColumn();
      var property1 = new SimpleStoragePropertyDefinition (typeof (int), columnDefinition);
      var property2 = new SimpleStoragePropertyDefinition (typeof (string), columnDefinition);

      Assert.That (
          () => property1.UnifyWithEquivalentProperties (new[] { property2 }),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has property type 'System.Int32', and the given property has "
              + "property type 'System.String'.\r\nParameter name: equivalentProperties"));
    }

    [Test]
    public void UnifyWithEquivalentProperties_ThrowsForDifferentColumnName ()
    {
      var storageTypeInformation = StorageTypeInformationObjectMother.CreateStorageTypeInformation();
      var property1 = new SimpleStoragePropertyDefinition (typeof (int), ColumnDefinitionObjectMother.CreateColumn ("Foo", storageTypeInformation, true));
      var property2 = new SimpleStoragePropertyDefinition (typeof (int), ColumnDefinitionObjectMother.CreateColumn ("Bar", storageTypeInformation, true));

      Assert.That (
          () => property1.UnifyWithEquivalentProperties (new[] { property2 }),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has column name 'Foo', and the given property has "
              + "column name 'Bar'.\r\nParameter name: equivalentProperties"));
    }

    [Test]
    public void UnifyWithEquivalentProperties_ThrowsForDifferentPrimaryKeyFlag ()
    {
      var storageTypeInformation = StorageTypeInformationObjectMother.CreateStorageTypeInformation ();
      var property1 = new SimpleStoragePropertyDefinition (typeof (int), ColumnDefinitionObjectMother.CreateColumn ("Col", storageTypeInformation, true));
      var property2 = new SimpleStoragePropertyDefinition (typeof (int), ColumnDefinitionObjectMother.CreateColumn ("Col", storageTypeInformation, false));

      Assert.That (
          () => property1.UnifyWithEquivalentProperties (new[] { property2 }),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has primary key flag 'True', and the given property has "
              + "primary key flag 'False'.\r\nParameter name: equivalentProperties"));
    }

    private static StorageTypeInformation CreateDefinedStorageTypeInformation (bool isStorageTypeNullable = false)
    {
      return StorageTypeInformationObjectMother.CreateStorageTypeInformation (isStorageTypeNullable: isStorageTypeNullable);
    }
  }
}