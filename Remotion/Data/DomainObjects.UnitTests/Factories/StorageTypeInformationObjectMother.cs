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
using System.ComponentModel;
using System.Data;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Factories
{
  public static class StorageTypeInformationObjectMother
  {

    public static StorageTypeInformation CreateStorageTypeInformation (
        Type storageType = null,
        string storageTypeName = null,
        DbType? storageDbType = null,
        bool? isStorageTypeNullable = null,
        int? storageTypeLength = null,
        Type dotNetType = null,
        TypeConverter dotNetTypeConverter = null)
    {
      var dbType = storageDbType ?? DbType.String;
      var dbTypeSize = storageTypeLength ?? GetDefaultSize(dbType);

      return new StorageTypeInformation(
          storageType ?? typeof(string),
          storageTypeName ?? "nvarchar(max)",
          dbType,
          isStorageTypeNullable ?? true,
          dbTypeSize,
          dotNetType ?? typeof(string),
          dotNetTypeConverter ?? new DefaultConverter(typeof(string)));
    }

    private static int? GetDefaultSize (DbType dbType)
    {
      return dbType switch
      {
          DbType.AnsiString => -1,
          DbType.AnsiStringFixedLength => -1,
          DbType.Binary => -1,
          DbType.String => -1,
          DbType.StringFixedLength => -1,
          _ => null
      };
    }

    public static StorageTypeInformation CreateUniqueIdentifierStorageTypeInformation (bool isNullable = false)
    {
      return new StorageTypeInformation(typeof(Guid), "uniqueidentifier", DbType.Guid, isNullable, null, typeof(Guid), new DefaultConverter(typeof(Guid)));
    }

    public static StorageTypeInformation CreateVarchar100StorageTypeInformation (bool isNullable = false)
    {
      return new StorageTypeInformation(typeof(string), "varchar(100)", DbType.String, isNullable, 100, typeof(string), new DefaultConverter(typeof(string)));
    }

    public static StorageTypeInformation CreateDateTimeStorageTypeInformation (bool isNullable = false)
    {
      return new StorageTypeInformation(typeof(DateTime), "datetime2", DbType.DateTime2, isNullable, null, typeof(DateTime), new DefaultConverter(typeof(DateTime)));
    }

    public static StorageTypeInformation CreateIntStorageTypeInformation (bool isNullable = false)
    {
      return new StorageTypeInformation(typeof(int), "int", DbType.Int32, isNullable, null, typeof(int), new DefaultConverter(typeof(int)));
    }

    public static IStorageTypeInformation CreateBitStorageTypeInformation (bool isNullable = false)
    {
      return new StorageTypeInformation(typeof(bool), "bit", DbType.Boolean, isNullable, null, typeof(bool), new DefaultConverter(typeof(bool)));
    }
  }
}
