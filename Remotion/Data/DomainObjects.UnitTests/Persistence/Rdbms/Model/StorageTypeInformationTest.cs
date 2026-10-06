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
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  [TestFixture]
  public class StorageTypeInformationTest
  {
    private StorageTypeInformation _storageTypeInformation;
    private Mock<TypeConverter> _typeConverterStub;

    [SetUp]
    public void SetUp ()
    {
      _typeConverterStub = new Mock<TypeConverter>();
      _storageTypeInformation = new StorageTypeInformation(typeof(bool), "test", DbType.Boolean, false, null, typeof(int), _typeConverterStub.Object);
    }

    [Test]
    [TestCase(DbType.Boolean, false, null, TestName = "Initialization_WithDbTypeIsNullable.")]
    [TestCase(DbType.Boolean, true, null, TestName = "Initialization_WithDbTypeIsNotNullable.")]
    [TestCase(DbType.String, false, 5, TestName = "Initialization_WithDbTypeHasSize.")]
    public void Initialization (DbType storageDbType, bool storageTypeNullable, int? storageTypeLength)
    {
      var storageTypeInformation = new StorageTypeInformation(
          typeof(bool),
          "test",
          storageDbType,
          storageTypeNullable,
          storageTypeLength,
          typeof(int),
          _typeConverterStub.Object,
          38,
          3);

      Assert.That(storageTypeInformation.StorageType, Is.EqualTo(typeof(bool)));
      Assert.That(storageTypeInformation.StorageTypeName, Is.EqualTo("test"));
      Assert.That(storageTypeInformation.StorageDbType, Is.EqualTo(storageDbType));
      Assert.That(storageTypeInformation.IsStorageTypeNullable, Is.EqualTo(storageTypeNullable));
      Assert.That(storageTypeInformation.StorageTypeLength, Is.EqualTo(storageTypeLength));
      Assert.That(storageTypeInformation.DotNetType, Is.EqualTo(typeof(int)));
      Assert.That(storageTypeInformation.DotNetTypeConverter, Is.SameAs(_typeConverterStub.Object));
      Assert.That(storageTypeInformation.Precision, Is.EqualTo(38));
      Assert.That(storageTypeInformation.Scale, Is.EqualTo(3));
    }

    [Test]
    public void Initialization_WithDecimalAndPrecisionScale ()
    {
      var storageTypeInformation = new StorageTypeInformation(
          typeof(decimal),
          "decimal (38, 3)",
          DbType.Decimal,
          false,
          null,
          typeof(decimal),
          _typeConverterStub.Object,
          38,
          3);

      Assert.That(storageTypeInformation.StorageType, Is.EqualTo(typeof(decimal)));
      Assert.That(storageTypeInformation.StorageTypeName, Is.EqualTo("decimal (38, 3)"));
      Assert.That(storageTypeInformation.StorageDbType, Is.EqualTo(DbType.Decimal));
      Assert.That(storageTypeInformation.IsStorageTypeNullable, Is.EqualTo(false));
      Assert.That(storageTypeInformation.StorageTypeLength, Is.EqualTo(null));
      Assert.That(storageTypeInformation.DotNetType, Is.EqualTo(typeof(decimal)));
      Assert.That(storageTypeInformation.DotNetTypeConverter, Is.SameAs(_typeConverterStub.Object));
      Assert.That(storageTypeInformation.Precision, Is.EqualTo(38));
      Assert.That(storageTypeInformation.Scale, Is.EqualTo(3));
    }

    [Test]
    [TestCase(null, null)]
    [TestCase(38, null)]
    [TestCase(null, 3)]
    public void Initialization_WithDecimalAndNoPrecisionOrScale_ThrowsArgumentException (byte? precision, byte? scale)
    {
      Assert.That(
          () => new StorageTypeInformation(
              typeof(decimal),
              "decimal (38, 3)",
              DbType.Decimal,
              false,
              null,
              typeof(decimal),
              _typeConverterStub.Object,
              precision,
              scale),
          Throws.ArgumentException
              .With.Message.EqualTo("When creating a StorageTypeInformation with DB type 'Decimal', a precision and a scale must be set."));
    }

    [Test]
    public void ConvertToStorageType ()
    {
      _typeConverterStub.Setup(stub => stub.ConvertTo(null, null, "value", _storageTypeInformation.StorageType)).Returns("converted value");

      var result = _storageTypeInformation.ConvertToStorageType("value");

      Assert.That(result, Is.EqualTo("converted value"));
    }

    [Test]
    public void ConvertToStorageType_NullInput ()
    {
      _typeConverterStub.Setup(stub => stub.ConvertTo(null, null, null, _storageTypeInformation.StorageType)).Returns("converted value");

      var result = _storageTypeInformation.ConvertToStorageType(null);

      Assert.That(result, Is.EqualTo("converted value"));
    }

    [Test]
    public void ConvertFromStorageType ()
    {
      _typeConverterStub.Setup(stub => stub.ConvertFrom(null, CultureInfo.CurrentCulture, "value")).Returns("converted value");

      var result = _storageTypeInformation.ConvertFromStorageType("value");

      Assert.That(result, Is.EqualTo("converted value"));
    }

    [Test]
    public void ConvertFromStorageType_DBNull ()
    {
      _typeConverterStub.Setup(stub => stub.ConvertFrom(null, CultureInfo.CurrentCulture, null)).Returns("converted null value");

      var result = _storageTypeInformation.ConvertFromStorageType(DBNull.Value);

      Assert.That(result, Is.EqualTo("converted null value"));
    }

    [Test]
    public void ConvertFromStorageType_Null ()
    {
      _typeConverterStub.Setup(stub => stub.ConvertFrom(null, CultureInfo.CurrentCulture, null)).Returns("converted null value");

      var result = _storageTypeInformation.ConvertFromStorageType(null);

      Assert.That(result, Is.EqualTo("converted null value"));
    }

    [Test]
    public void Read ()
    {
      var dataReaderMock = new Mock<DbDataReader>(MockBehavior.Strict);
      dataReaderMock.Setup(mock => mock.GetValue(17)).Returns("value").Verifiable();

      _typeConverterStub.Setup(stub => stub.ConvertFrom(null, CultureInfo.CurrentCulture, "value")).Returns("converted value");

      var result = _storageTypeInformation.Read(dataReaderMock.Object, 17);

      dataReaderMock.Verify();
      Assert.That(result, Is.EqualTo("converted value"));
    }

    [Test]
    public void Read_DBNull ()
    {
      var dataReaderMock = new Mock<DbDataReader>(MockBehavior.Strict);
      dataReaderMock.Setup(mock => mock.GetValue(17)).Returns(DBNull.Value).Verifiable();

      _typeConverterStub.Setup(stub => stub.ConvertFrom(null, CultureInfo.CurrentCulture, null)).Returns("converted null value");

      var result = _storageTypeInformation.Read(dataReaderMock.Object, 17);

      dataReaderMock.Verify();
      Assert.That(result, Is.EqualTo("converted null value"));
    }

    [Test]
    public void Read_Null ()
    {
      var dataReaderMock = new Mock<DbDataReader>(MockBehavior.Strict);
      dataReaderMock.Setup(mock => mock.GetValue(17)).Returns((object)null).Verifiable();

      _typeConverterStub.Setup(stub => stub.ConvertFrom(null, CultureInfo.CurrentCulture, null)).Returns("converted null value");

      var result = _storageTypeInformation.Read(dataReaderMock.Object, 17);

      dataReaderMock.Verify();
      Assert.That(result, Is.EqualTo("converted null value"));
    }

    [Test]
    public void UnifyForEquivalentProperties_CombinesStorageTypes_AllNonNullable_CombinedIsAlsoNonNullable ()
    {
      var typeInfo1 = new StorageTypeInformation(typeof(string), "X", DbType.Int32, false, 5, typeof(int), new DefaultConverter(typeof(string)), 28, 5);
      var typeInfo2 = new StorageTypeInformation(typeof(string), "X", DbType.Int32, false, 5, typeof(int), new DefaultConverter(typeof(string)), 28, 5);
      var typeInfo3 = new StorageTypeInformation(typeof(string), "X", DbType.Int32, false, 5, typeof(int), new DefaultConverter(typeof(string)), 28, 5);

      var result = typeInfo1.UnifyForEquivalentProperties(new[] { typeInfo2, typeInfo3 });

      Assert.That(result, Is.TypeOf<StorageTypeInformation>());
      Assert.That(((StorageTypeInformation)result).StorageType, Is.SameAs(typeof(string)));
      Assert.That(((StorageTypeInformation)result).StorageTypeName, Is.EqualTo("X"));
      Assert.That(((StorageTypeInformation)result).StorageDbType, Is.EqualTo(DbType.Int32));
      Assert.That(((StorageTypeInformation)result).IsStorageTypeNullable, Is.False);
      Assert.That(((StorageTypeInformation)result).StorageTypeLength, Is.EqualTo(5));
      Assert.That(((StorageTypeInformation)result).DotNetType, Is.SameAs(typeof(int)));
      Assert.That(((StorageTypeInformation)result).DotNetTypeConverter, Is.SameAs(typeInfo1.DotNetTypeConverter));
      Assert.That(((StorageTypeInformation)result).Precision, Is.EqualTo(28));
      Assert.That(((StorageTypeInformation)result).Scale, Is.EqualTo(5));
    }

    [Test]
    public void UnifyForEquivalentProperties_CombinesStorageTypes_SomeNullable_CombinedIsNullable ()
    {
      var typeInfo1 = new StorageTypeInformation(typeof(string), "X", DbType.Int32, false, 6, typeof(int), new DefaultConverter(typeof(string)));
      var typeInfo2 = new StorageTypeInformation(typeof(string), "X", DbType.Int32, false, 6, typeof(int), new DefaultConverter(typeof(string)));
      var typeInfo3 = new StorageTypeInformation(typeof(string), "X", DbType.Int32, true, 6, typeof(int), new DefaultConverter(typeof(string)));

      var result = typeInfo1.UnifyForEquivalentProperties(new[] { typeInfo2, typeInfo3 });

      Assert.That(result, Is.TypeOf<StorageTypeInformation>());
      Assert.That(((StorageTypeInformation)result).IsStorageTypeNullable, Is.True);
    }

    [Test]
    public void UnifyForEquivalentProperties_CombinesStorageTypes_FirstNullable_CombinedIsNullable ()
    {
      var typeInfo1 = new StorageTypeInformation(typeof(string), "X", DbType.Int32, true, 6, typeof(int), new DefaultConverter(typeof(string)));
      var typeInfo2 = new StorageTypeInformation(typeof(string), "X", DbType.Int32, false, 6, typeof(int), new DefaultConverter(typeof(string)));
      var typeInfo3 = new StorageTypeInformation(typeof(string), "X", DbType.Int32, false, 6, typeof(int), new DefaultConverter(typeof(string)));

      var result = typeInfo1.UnifyForEquivalentProperties(new[] { typeInfo2, typeInfo3 });

      Assert.That(result, Is.TypeOf<StorageTypeInformation>());
      Assert.That(((StorageTypeInformation)result).IsStorageTypeNullable, Is.True);
    }

    [Test]
    public void UnifyForEquivalentProperties_ThrowsForDifferentStorageTypeInfoType ()
    {
      var typeInfo2 = new FakeStorageTypeInformation();
      Assert.That(
          () => _storageTypeInformation.UnifyForEquivalentProperties(new[] { typeInfo2 }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Only equivalent properties can be combined, but this property has type 'StorageTypeInformation', and the given property has "
              + "type 'FakeStorageTypeInformation'.", "equivalentStorageTypes"));
    }

    [Test]
    public void UnifyForEquivalentProperties_ThrowsForDifferentStorageType ()
    {
      var typeInfo1 = new StorageTypeInformation(typeof(string), "X", DbType.Int32, true, null, typeof(string), new DefaultConverter(typeof(string)));
      var typeInfo2 = new StorageTypeInformation(typeof(int), "X", DbType.Int32, true, null, typeof(string), new DefaultConverter(typeof(string)));
      Assert.That(
          () => typeInfo1.UnifyForEquivalentProperties(new[] { typeInfo2 }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Only equivalent properties can be combined, but this property has storage type 'System.String', and the given property has "
              + "storage type 'System.Int32'.", "equivalentStorageTypes"));
    }

    [Test]
    public void UnifyForEquivalentProperties_ThrowsForDifferentStorageDbType ()
    {
      var typeInfo1 = new StorageTypeInformation(typeof(int), "X", DbType.String, true, null, typeof(string), new DefaultConverter(typeof(string)));
      var typeInfo2 = new StorageTypeInformation(typeof(int), "X", DbType.Int32, true, null, typeof(string), new DefaultConverter(typeof(string)));
      Assert.That(
          () => typeInfo1.UnifyForEquivalentProperties(new[] { typeInfo2 }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Only equivalent properties can be combined, but this property has storage DbType 'String', and the given property has "
              + "storage DbType 'Int32'.", "equivalentStorageTypes"));
    }

    [Test]
    public void UnifyForEquivalentProperties_ThrowsForDifferentStorageTypeLength ()
    {
      var typeInfo1 = new StorageTypeInformation(typeof(string), "X", DbType.Int32, true, null, typeof(string), new DefaultConverter(typeof(string)));
      var typeInfo2 = new StorageTypeInformation(typeof(string), "X", DbType.Int32, true, 0, typeof(string), new DefaultConverter(typeof(string)));
      Assert.That(
          () => typeInfo1.UnifyForEquivalentProperties(new[] { typeInfo2 }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Only equivalent properties can be combined, but this property has storage type length 'null', and the given property has "
              + "storage type length '0'.", "equivalentStorageTypes"));
    }

    [Test]
    public void UnifyForEquivalentProperties_ThrowsForDifferentPrecision ()
    {
      var typeInfo1 = new StorageTypeInformation(typeof(decimal), "X", DbType.Decimal, true, null, typeof(decimal), new DefaultConverter(typeof(decimal)), 28, 2);
      var typeInfo2 = new StorageTypeInformation(typeof(decimal), "X", DbType.Decimal, true, null, typeof(decimal), new DefaultConverter(typeof(decimal)), 38, 2);
      Assert.That(
          () => typeInfo1.UnifyForEquivalentProperties(new[] { typeInfo2 }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Only equivalent properties can be combined, but this property has precision '28', and the given property has "
              + "precision '38'.", "equivalentStorageTypes"));
    }

    [Test]
    public void UnifyForEquivalentProperties_ThrowsForDifferentScale ()
    {
      var typeInfo1 = new StorageTypeInformation(typeof(decimal), "X", DbType.Decimal, true, null, typeof(decimal), new DefaultConverter(typeof(decimal)), 28, 5);
      var typeInfo2 = new StorageTypeInformation(typeof(decimal), "X", DbType.Decimal, true, null, typeof(decimal), new DefaultConverter(typeof(decimal)), 28, 2);
      Assert.That(
          () => typeInfo1.UnifyForEquivalentProperties(new[] { typeInfo2 }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Only equivalent properties can be combined, but this property has scale '5', and the given property has "
              + "scale '2'.", "equivalentStorageTypes"));
    }

    [Test]
    public void UnifyForEquivalentProperties_ThrowsForDifferentDotNetType ()
    {
      var typeInfo1 = new StorageTypeInformation(typeof(int), "X", DbType.Int32, true, null, typeof(string), new DefaultConverter(typeof(string)));
      var typeInfo2 = new StorageTypeInformation(typeof(int), "X", DbType.Int32, true, null, typeof(int), new DefaultConverter(typeof(string)));
      Assert.That(
          () => typeInfo1.UnifyForEquivalentProperties(new[] { typeInfo2 }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Only equivalent properties can be combined, but this property has .NET type 'System.String', and the given property has "
              + ".NET type 'System.Int32'.", "equivalentStorageTypes"));
    }


    [Test]
    public void UnifyForEquivalentProperties_ThrowsForDifferentConverterType ()
    {
      var typeInfo1 = new StorageTypeInformation(typeof(int), "X", DbType.Int32, true, null, typeof(string), new DefaultConverter(typeof(string)));
      var typeInfo2 = new StorageTypeInformation(typeof(int), "X", DbType.Int32, true, null, typeof(string), new AdvancedEnumConverter(typeof(DbType)));
      Assert.That(
          () => typeInfo1.UnifyForEquivalentProperties(new[] { typeInfo2 }),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Only equivalent properties can be combined, but this property has .NET type converter type 'Remotion.Utilities.DefaultConverter', and "
              + "the given property has .NET type converter type 'Remotion.Utilities.AdvancedEnumConverter'.", "equivalentStorageTypes"));
    }

    private class FakeStorageTypeInformation : IStorageTypeInformation
    {
      public Type StorageType
      {
        get { throw new NotImplementedException(); }
      }

      public string StorageTypeName
      {
        get { throw new NotImplementedException(); }
      }

      public bool IsStorageTypeNullable
      {
        get { throw new NotImplementedException(); }
      }

      public DbType StorageDbType
      {
        get { throw new NotImplementedException(); }
      }

      public int? StorageTypeLength
      {
        get { throw new NotImplementedException(); }
      }

      public byte? Precision
      {
        get { throw new NotImplementedException(); }
      }

      public byte? Scale
      {
        get { throw new NotImplementedException(); }
      }

      public Type DotNetType
      {
        get { throw new NotImplementedException(); }
      }

      public DbParameter CreateDataParameter (DbCommand command, object value)
      {
        throw new NotImplementedException();
      }

      public object Read (DbDataReader dataReader, int ordinal)
      {
        throw new NotImplementedException();
      }

      public object ConvertToStorageType (object dotNetValue)
      {
        throw new NotImplementedException();
      }

      public object ConvertFromStorageType (object storageValue)
      {
        throw new NotImplementedException();
      }

      public IStorageTypeInformation UnifyForEquivalentProperties (IEnumerable<IStorageTypeInformation> equivalentStorageTypes)
      {
        throw new NotImplementedException();
      }
    }
  }
}
