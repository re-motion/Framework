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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model
{
  [TestFixture]
  public class StorageTypeInformationTest
  {
    private StorageTypeInformation _storageTypeInformation;
    private TypeConverter _typeConverterStub;

    [SetUp]
    public void SetUp ()
    {
      _typeConverterStub = MockRepository.GenerateStub<TypeConverter>();
      _storageTypeInformation = new StorageTypeInformation (typeof(bool), "test", DbType.Boolean, false, typeof (int), _typeConverterStub);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_storageTypeInformation.StorageType, Is.EqualTo (typeof (bool)));
      Assert.That (_storageTypeInformation.StorageTypeName, Is.EqualTo ("test"));
      Assert.That (_storageTypeInformation.StorageDbType, Is.EqualTo (DbType.Boolean));
      Assert.That (_storageTypeInformation.DotNetType, Is.EqualTo (typeof (int)));
      Assert.That (_storageTypeInformation.DotNetTypeConverter, Is.SameAs (_typeConverterStub));
    }

    [Test]
    public void ConvertToStorageType ()
    {
      _typeConverterStub.Stub (stub => stub.ConvertTo ("value", _storageTypeInformation.StorageType)).Return ("converted value");

      var result = _storageTypeInformation.ConvertToStorageType ("value");

      Assert.That (result, Is.EqualTo ("converted value"));
    }

    [Test]
    public void ConvertToStorageType_NullInput ()
    {
      _typeConverterStub.Stub (stub => stub.ConvertTo (null, _storageTypeInformation.StorageType)).Return ("converted value");

      var result = _storageTypeInformation.ConvertToStorageType (null);

      Assert.That (result, Is.EqualTo ("converted value"));
    }

    [Test]
    public void ConvertFromStorageType ()
    {
      _typeConverterStub.Stub (stub => stub.ConvertFrom ("value")).Return ("converted value");

      var result = _storageTypeInformation.ConvertFromStorageType ("value");

      Assert.That (result, Is.EqualTo ("converted value"));
    }

    [Test]
    public void ConvertFromStorageType_DBNull ()
    {
      _typeConverterStub.Stub (stub => stub.ConvertFrom (null)).Return ("converted null value");

      var result = _storageTypeInformation.ConvertFromStorageType (DBNull.Value);

      Assert.That (result, Is.EqualTo ("converted null value"));
    }

    [Test]
    public void CreateDataParameter ()
    {
      var commandMock = MockRepository.GenerateStrictMock<IDbCommand> ();
      var dataParameterMock = MockRepository.GenerateStrictMock<IDbDataParameter> ();

      _typeConverterStub.Stub (stub => stub.ConvertTo ("value", _storageTypeInformation.StorageType)).Return ("converted value");

      commandMock.Expect (mock => mock.CreateParameter()).Return (dataParameterMock);
      commandMock.Replay();

      dataParameterMock.Expect (mock => mock.DbType = _storageTypeInformation.StorageDbType);
      dataParameterMock.Expect (mock => mock.Value = "converted value");
      dataParameterMock.Replay();

      var result = _storageTypeInformation.CreateDataParameter (commandMock, "value");

      commandMock.VerifyAllExpectations();
      dataParameterMock.VerifyAllExpectations();

      Assert.That (result, Is.SameAs (dataParameterMock));
    }

    [Test]
    public void CreateDataParameter_NullResult ()
    {
      var commandMock = MockRepository.GenerateStrictMock<IDbCommand> ();
      var dataParameterMock = MockRepository.GenerateStrictMock<IDbDataParameter> ();

      _typeConverterStub.Stub (stub => stub.ConvertTo ("value", _storageTypeInformation.StorageType)).Return (null);

      commandMock.Expect (mock => mock.CreateParameter ()).Return (dataParameterMock);
      commandMock.Replay ();

      dataParameterMock.Expect (mock => mock.DbType = _storageTypeInformation.StorageDbType);
      dataParameterMock.Expect (mock => mock.Value = DBNull.Value);
      dataParameterMock.Replay ();

      var result = _storageTypeInformation.CreateDataParameter (commandMock, "value");

      commandMock.VerifyAllExpectations ();
      dataParameterMock.VerifyAllExpectations ();

      Assert.That (result, Is.SameAs (dataParameterMock));
    }

    [Test]
    public void CreateDataParameter_NullInput ()
    {
      var commandMock = MockRepository.GenerateStrictMock<IDbCommand> ();
      var dataParameterMock = MockRepository.GenerateStrictMock<IDbDataParameter> ();

      _typeConverterStub.Stub (stub => stub.ConvertTo (null, _storageTypeInformation.StorageType)).Return ("converted value");

      commandMock.Expect (mock => mock.CreateParameter ()).Return (dataParameterMock);
      commandMock.Replay ();

      dataParameterMock.Expect (mock => mock.DbType = _storageTypeInformation.StorageDbType);
      dataParameterMock.Expect (mock => mock.Value = "converted value");
      dataParameterMock.Replay ();

      var result = _storageTypeInformation.CreateDataParameter (commandMock, null);

      commandMock.VerifyAllExpectations ();
      dataParameterMock.VerifyAllExpectations ();

      Assert.That (result, Is.SameAs (dataParameterMock));
    }

    [Test]
    public void Read ()
    {
      var dataReaderMock = MockRepository.GenerateStrictMock<IDataReader> ();
      dataReaderMock.Expect (mock => mock[17]).Return ("value");
      dataReaderMock.Replay ();

      _typeConverterStub.Stub (stub => stub.ConvertFrom ("value")).Return ("converted value");

      var result = _storageTypeInformation.Read (dataReaderMock, 17);

      dataReaderMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("converted value"));
    }

    [Test]
    public void Read_DBNull ()
    {
      var dataReaderMock = MockRepository.GenerateStrictMock<IDataReader> ();
      dataReaderMock.Expect (mock => mock[17]).Return (DBNull.Value);
      dataReaderMock.Replay ();

      _typeConverterStub.Stub (stub => stub.ConvertFrom (null)).Return ("converted null value");

      var result = _storageTypeInformation.Read (dataReaderMock, 17);

      dataReaderMock.VerifyAllExpectations ();
      Assert.That (result, Is.EqualTo ("converted null value"));
    }

    [Test]
    public void UnifyForEquivalentProperties_CombinesStorageTypes_AllNonNullable_CombinedIsAlsoNonNullable ()
    {
      var typeInfo1 = new StorageTypeInformation (typeof (string), "X", DbType.Int32, false, typeof (int), new DefaultConverter (typeof (string)));
      var typeInfo2 = new StorageTypeInformation (typeof (string), "X", DbType.Int32, false, typeof (int), new DefaultConverter (typeof (string)));
      var typeInfo3 = new StorageTypeInformation (typeof (string), "X", DbType.Int32, false, typeof (int), new DefaultConverter (typeof (string)));

      var result = typeInfo1.UnifyForEquivalentProperties (new[] { typeInfo2, typeInfo3 });

      Assert.That (result, Is.TypeOf<StorageTypeInformation> ());
      Assert.That (((StorageTypeInformation) result).StorageType, Is.SameAs (typeof (string)));
      Assert.That (((StorageTypeInformation) result).StorageTypeName, Is.EqualTo ("X"));
      Assert.That (((StorageTypeInformation) result).StorageDbType, Is.EqualTo (DbType.Int32));
      Assert.That (((StorageTypeInformation) result).IsStorageTypeNullable, Is.False);
      Assert.That (((StorageTypeInformation) result).DotNetType, Is.SameAs (typeof (int)));
      Assert.That (((StorageTypeInformation) result).DotNetTypeConverter, Is.SameAs (typeInfo1.DotNetTypeConverter));
    }

    [Test]
    public void UnifyForEquivalentProperties_CombinesStorageTypes_SomeNullable_CombinedIsNullable ()
    {
      var typeInfo1 = new StorageTypeInformation (typeof (string), "X", DbType.Int32, false, typeof (int), new DefaultConverter (typeof (string)));
      var typeInfo2 = new StorageTypeInformation (typeof (string), "X", DbType.Int32, false, typeof (int), new DefaultConverter (typeof (string)));
      var typeInfo3 = new StorageTypeInformation (typeof (string), "X", DbType.Int32, true, typeof (int), new DefaultConverter (typeof (string)));

      var result = typeInfo1.UnifyForEquivalentProperties (new[] { typeInfo2, typeInfo3 });

      Assert.That (result, Is.TypeOf<StorageTypeInformation> ());
      Assert.That (((StorageTypeInformation) result).IsStorageTypeNullable, Is.True);
    }

    [Test]
    public void UnifyForEquivalentProperties_CombinesStorageTypes_FirstNullable_CombinedIsNullable ()
    {
      var typeInfo1 = new StorageTypeInformation (typeof (string), "X", DbType.Int32, true, typeof (int), new DefaultConverter (typeof (string)));
      var typeInfo2 = new StorageTypeInformation (typeof (string), "X", DbType.Int32, false, typeof (int), new DefaultConverter (typeof (string)));
      var typeInfo3 = new StorageTypeInformation (typeof (string), "X", DbType.Int32, false, typeof (int), new DefaultConverter (typeof (string)));

      var result = typeInfo1.UnifyForEquivalentProperties (new[] { typeInfo2, typeInfo3 });

      Assert.That (result, Is.TypeOf<StorageTypeInformation> ());
      Assert.That (((StorageTypeInformation) result).IsStorageTypeNullable, Is.True);
    }

    [Test]
    public void UnifyForEquivalentProperties_ThrowsForDifferentStorageTypeInfoType ()
    {
      var typeInfo2 = new FakeStorageTypeInformation ();
      Assert.That (
          () => _storageTypeInformation.UnifyForEquivalentProperties (new[] { typeInfo2 }),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has type 'StorageTypeInformation', and the given property has "
              + "type 'FakeStorageTypeInformation'.\r\nParameter name: equivalentStorageTypes"));
    }

    [Test]
    public void UnifyForEquivalentProperties_ThrowsForDifferentStorageType ()
    {
      var typeInfo1 = new StorageTypeInformation (typeof (string), "X", DbType.Int32, true, typeof (string), new DefaultConverter (typeof (string)));
      var typeInfo2 = new StorageTypeInformation (typeof (int), "X", DbType.Int32, true, typeof (string), new DefaultConverter (typeof (string)));
      Assert.That (
          () => typeInfo1.UnifyForEquivalentProperties (new[] { typeInfo2 }),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has storage type 'System.String', and the given property has "
              + "storage type 'System.Int32'.\r\nParameter name: equivalentStorageTypes"));
    }

    [Test]
    public void UnifyForEquivalentProperties_ThrowsForDifferentStorageDbType ()
    {
      var typeInfo1 = new StorageTypeInformation (typeof (int), "X", DbType.String, true, typeof (string), new DefaultConverter (typeof (string)));
      var typeInfo2 = new StorageTypeInformation (typeof (int), "X", DbType.Int32, true, typeof (string), new DefaultConverter (typeof (string)));
      Assert.That (
          () => typeInfo1.UnifyForEquivalentProperties (new[] { typeInfo2 }),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has storage DbType 'String', and the given property has "
              + "storage DbType 'Int32'.\r\nParameter name: equivalentStorageTypes"));
    }

    [Test]
    public void UnifyForEquivalentProperties_ThrowsForDifferentDotNetType ()
    {
      var typeInfo1 = new StorageTypeInformation (typeof (int), "X", DbType.Int32, true, typeof (string), new DefaultConverter (typeof (string)));
      var typeInfo2 = new StorageTypeInformation (typeof (int), "X", DbType.Int32, true, typeof (int), new DefaultConverter (typeof (string)));
      Assert.That (
          () => typeInfo1.UnifyForEquivalentProperties (new[] { typeInfo2 }),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has .NET type 'System.String', and the given property has "
              + ".NET type 'System.Int32'.\r\nParameter name: equivalentStorageTypes"));
    }


    [Test]
    public void UnifyForEquivalentProperties_ThrowsForDifferentConverterType ()
    {
      var typeInfo1 = new StorageTypeInformation (typeof (int), "X", DbType.Int32, true, typeof (string), new DefaultConverter (typeof (string)));
      var typeInfo2 = new StorageTypeInformation (typeof (int), "X", DbType.Int32, true, typeof (string), new AdvancedEnumConverter (typeof (DbType)));
      Assert.That (
          () => typeInfo1.UnifyForEquivalentProperties (new[] { typeInfo2 }),
          Throws.ArgumentException.With.Message.EqualTo (
              "Only equivalent properties can be combined, but this property has .NET type converter type 'Remotion.Utilities.DefaultConverter', and "
              + "the given property has .NET type converter type 'Remotion.Utilities.AdvancedEnumConverter'.\r\nParameter name: equivalentStorageTypes"));
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

      public Type DotNetType
      {
        get { throw new NotImplementedException(); }
      }

      public IDbDataParameter CreateDataParameter (IDbCommand command, object value)
      {
        throw new NotImplementedException();
      }

      public object Read (IDataReader dataReader, int ordinal)
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