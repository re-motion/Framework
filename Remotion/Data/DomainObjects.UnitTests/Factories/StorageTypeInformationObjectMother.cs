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
        Type dotNetType = null,
        TypeConverter dotNetTypeConverter = null)
    {
      return new StorageTypeInformation (
          storageType ?? typeof (string),
          storageTypeName ?? "nvarchar(max)",
          storageDbType ?? DbType.String,
          isStorageTypeNullable ?? true,
          dotNetType ?? typeof (string),
          dotNetTypeConverter ?? new DefaultConverter (typeof (string)));
    }

    public static StorageTypeInformation CreateUniqueIdentifierStorageTypeInformation (bool isNullable = false)
    {
      return new StorageTypeInformation (typeof (Guid), "uniqueidentifier", DbType.Guid, isNullable, typeof (Guid), new DefaultConverter (typeof (Guid)));
    }

    public static StorageTypeInformation CreateVarchar100StorageTypeInformation (bool isNullable = false)
    {
      return new StorageTypeInformation (typeof (string), "varchar(100)", DbType.String, isNullable, typeof (string), new DefaultConverter (typeof (string)));
    }

    public static StorageTypeInformation CreateDateTimeStorageTypeInformation (bool isNullable = false)
    {
      return new StorageTypeInformation (typeof (DateTime), "datetime", DbType.DateTime, isNullable, typeof (DateTime), new DefaultConverter (typeof (DateTime)));
    }

    public static IStorageTypeInformation CreateBitStorageTypeInformation (bool isNullable = false)
    {
      return new StorageTypeInformation (typeof (bool), "bit", DbType.Boolean, isNullable, typeof (bool), new DefaultConverter (typeof (bool)));
    }
  }
}