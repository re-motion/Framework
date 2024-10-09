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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.ExtensibleEnums;
using Remotion.Reflection;
using Remotion.Utilities;

using DateOnlyConverter = Remotion.Utilities.DateOnlyConverter;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building
{
  /// <summary>
  /// <see cref="SqlStorageTypeInformationProvider"/> calculates the SQL Server-specific type for a column in a relational database.
  /// </summary>
  public class SqlStorageTypeInformationProvider : IStorageTypeInformationProvider
  {
    internal const int StorageTypeLengthRepresentingMax = -1;

    public SqlStorageTypeInformationProvider ()
    {
    }

    IStorageTypeInformation IStorageTypeInformationProvider.GetStorageTypeForID (bool isStorageTypeNullable)
    {
      return GetStorageTypeForID(isStorageTypeNullable);
    }

    protected virtual StorageTypeInformation GetStorageTypeForID (bool isStorageTypeNullable)
    {
      // storageType Type and dotNetType Type should always be a nullable dotNet types. 
      // Otherwise, special logic is needed to ensure that the ID-types are compatible when performing a LiNQ join.
      // Also, for converting from the DB type, it is only required that the DB type can be converted to the in-memory type, not the other way around.
      return new StorageTypeInformation(
          typeof(Guid?),
          "uniqueidentifier",
          DbType.Guid,
          isStorageTypeNullable,
          null,
          typeof(Guid?),
          new DefaultConverter(typeof(Guid?)));
    }

    IStorageTypeInformation IStorageTypeInformationProvider.GetStorageTypeForSerializedObjectID (bool isStorageTypeNullable)
    {
      return GetStorageTypeForSerializedObjectID(isStorageTypeNullable);
    }

    protected virtual StorageTypeInformation GetStorageTypeForSerializedObjectID (bool isStorageTypeNullable)
    {
      return new StorageTypeInformation(
          typeof(string),
          "varchar (255)",
          DbType.AnsiString,
          isStorageTypeNullable,
          255,
          typeof(string),
          new DefaultConverter(typeof(string)));
    }

    IStorageTypeInformation IStorageTypeInformationProvider.GetStorageTypeForClassID (bool isStorageTypeNullable)
    {
      return GetStorageTypeForClassID(isStorageTypeNullable);
    }

    protected virtual StorageTypeInformation GetStorageTypeForClassID (bool isStorageTypeNullable)
    {
      return new StorageTypeInformation(
          typeof(string),
          "varchar (100)",
          DbType.AnsiString,
          isStorageTypeNullable,
          100,
          typeof(string),
          new DefaultConverter(typeof(string)));
    }

    IStorageTypeInformation IStorageTypeInformationProvider.GetStorageTypeForTimestamp (bool isStorageTypeNullable)
    {
      return GetStorageTypeForTimestamp(isStorageTypeNullable);
    }

    protected virtual StorageTypeInformation GetStorageTypeForTimestamp (bool isStorageTypeNullable)
    {
      return new StorageTypeInformation(
          typeof(byte[]),
          "rowversion",
          DbType.Binary,
          isStorageTypeNullable,
          null,
          typeof(byte[]),
          new DefaultConverter(typeof(byte[])));
    }

    IStorageTypeInformation IStorageTypeInformationProvider.GetStorageType (PropertyDefinition propertyDefinition, bool forceNullable)
    {
      return GetStorageType(propertyDefinition, forceNullable);
    }

    /// <inheritdoc cref="IStorageTypeInformationProvider.GetStorageType(Remotion.Data.DomainObjects.Mapping.PropertyDefinition,bool)"/>
    /// <remarks>If overridden in a derived class, <see cref="GetStorageType (Type)"/> must also be overridden based on the same semantics.</remarks>
    protected virtual StorageTypeInformation GetStorageType (PropertyDefinition propertyDefinition, bool forceNullable)
    {
      ArgumentUtility.CheckNotNull("propertyDefinition", propertyDefinition);

      var dotNetType = propertyDefinition.PropertyType;
      var isNullableInDatabase = propertyDefinition.IsNullable || forceNullable;

      var storageType = GetStorageType(dotNetType, propertyDefinition.MaxLength, isNullableInDatabase, propertyDefinition.PropertyInfo);
      if (storageType == null)
        throw new NotSupportedException(string.Format("Type '{0}' is not supported by this storage provider.", dotNetType));
      return storageType;
    }

    IStorageTypeInformation IStorageTypeInformationProvider.GetStorageType (Type type)
    {
      return GetStorageType(type);
    }

    /// <inheritdoc cref="IStorageTypeInformationProvider.GetStorageType(System.Type)"/>
    /// <remarks>
    /// If overridden in a derived class, <see cref="GetStorageType (PropertyDefinition, bool)"/> must also be overridden based on the same semantics.
    /// </remarks>
    protected virtual StorageTypeInformation GetStorageType (Type type)
    {
      ArgumentUtility.CheckNotNull("type", type);

      var storageType = GetStorageType(type, null, IsNullSupported(type), null);
      if(storageType == null)
        throw new NotSupportedException(string.Format("Type '{0}' is not supported by this storage provider.", type));
      return storageType;
    }

    public IStorageTypeInformation GetStorageType (object? value)
    {
      if (value == null)
      {
        // NULL values of storage type nvarchar(max) and nvarchar (4000) seem to be compatible with columns of all other supported storage types,
        // tested by SqlProviderExecuteCollectionQueryTest.AllDataTypes.
        return new StorageTypeInformation(typeof(object), "nvarchar (max)", DbType.String, true, StorageTypeLengthRepresentingMax, typeof(object), NullValueConverter.Instance);
      }

      return GetStorageType(value.GetType());
    }

    [CanBeNull]
    private StorageTypeInformation? GetStorageType (Type dotNetType, int? maxLength, bool isNullableInDatabase, IPropertyInformation? propertyInformation)
    {
      var underlyingTypeOfNullable = Nullable.GetUnderlyingType(dotNetType);
      if (underlyingTypeOfNullable != null)
        return GetStorageTypeForNullableValueType(dotNetType, underlyingTypeOfNullable, maxLength, isNullableInDatabase, propertyInformation);

      if (dotNetType.IsEnum)
        return GetStorageTypeForEnumType(dotNetType, maxLength, isNullableInDatabase);

      if (ExtensibleEnumUtility.IsExtensibleEnumType(dotNetType))
        return GetStorageTypeForExtensibleEnumType(dotNetType, isNullableInDatabase);

      if (ReflectionUtility.IsStringPropertyValueType(dotNetType))
      {
        string storageTypeName = GetStorageTypeStringForVarType("nvarchar", maxLength);
        return new StorageTypeInformation(
            typeof(string),
            storageTypeName,
            DbType.String,
            isNullableInDatabase,
            maxLength ?? StorageTypeLengthRepresentingMax,
            dotNetType,
            new DefaultConverter(dotNetType));
      }

      if (ReflectionUtility.IsBinaryPropertyValueType(dotNetType))
      {
        string storageTypeName = GetStorageTypeStringForVarType("varbinary", maxLength);
        return new StorageTypeInformation(
            typeof(byte[]),
            storageTypeName,
            DbType.Binary,
            isNullableInDatabase,
            maxLength ?? StorageTypeLengthRepresentingMax,
            dotNetType,
            new DefaultConverter(dotNetType));
      }

      if (dotNetType == typeof(Boolean))
        return new StorageTypeInformation(typeof(Boolean), "bit", DbType.Boolean, isNullableInDatabase, null, dotNetType, new DefaultConverter(dotNetType));
      if (dotNetType == typeof(Byte))
        return new StorageTypeInformation(typeof(Byte), "tinyint", DbType.Byte, isNullableInDatabase, null, dotNetType, new DefaultConverter(dotNetType));
      if (dotNetType == typeof(DateTime))
        return GetDateTimeStorageType(isNullableInDatabase, dotNetType, propertyInformation);
      if (dotNetType == typeof(DateOnly))
        return new StorageTypeInformation(typeof(DateTime), "date", DbType.Date, isNullableInDatabase, null, dotNetType, new DateOnlyConverter());
      if (dotNetType == typeof(Decimal))
        return new StorageTypeInformation(typeof(Decimal), "decimal (38, 3)", DbType.Decimal, isNullableInDatabase, null, dotNetType, new DefaultConverter(dotNetType));
      if (dotNetType == typeof(Double))
        return new StorageTypeInformation(typeof(Double), "float", DbType.Double, isNullableInDatabase, null, dotNetType, new DefaultConverter(dotNetType));
      if (dotNetType == typeof(Guid))
        return new StorageTypeInformation(typeof(Guid), "uniqueidentifier", DbType.Guid, isNullableInDatabase, null, dotNetType, new DefaultConverter(dotNetType));
      if (dotNetType == typeof(Int16))
        return new StorageTypeInformation(typeof(Int16), "smallint", DbType.Int16, isNullableInDatabase, null, dotNetType, new DefaultConverter(dotNetType));
      if (dotNetType == typeof(Int32))
        return new StorageTypeInformation(typeof(Int32), "int", DbType.Int32, isNullableInDatabase, null, dotNetType, new DefaultConverter(dotNetType));
      if (dotNetType == typeof(Int64))
        return new StorageTypeInformation(typeof(Int64), "bigint", DbType.Int64, isNullableInDatabase, null, dotNetType, new DefaultConverter(dotNetType));
      if (dotNetType == typeof(Single))
        return new StorageTypeInformation(typeof(Single), "real", DbType.Single, isNullableInDatabase, null, dotNetType, new DefaultConverter(dotNetType));

      return null;
    }

    private StorageTypeInformation GetDateTimeStorageType (bool isNullableInDatabase, Type dotNetType, IPropertyInformation? propertyInformation)
    {
      if (propertyInformation != null)
      {
        var storageTypeFromPropertyInfo = GetDateTimeStorageTypeFromPropertyInformation(propertyInformation, isNullableInDatabase, dotNetType);
        if (storageTypeFromPropertyInfo != null)
          return storageTypeFromPropertyInfo;
      }
      return new StorageTypeInformation(typeof(DateTime), "datetime2", DbType.DateTime2, isNullableInDatabase, null, dotNetType, new DefaultConverter(dotNetType));
    }

    private StorageTypeInformation? GetDateTimeStorageTypeFromPropertyInformation (IPropertyInformation propertyInfo, bool isNullableInDatabase, Type dotNetType)
    {
      var dateTimeStorageTypeAttribute = propertyInfo.GetCustomAttribute<DateTimeStorageTypeAttribute>(inherited: false);

      if (dateTimeStorageTypeAttribute == null)
        return null;

      var (dbType, storageTypeName) = dateTimeStorageTypeAttribute.StorageType switch
      {
        DateTimeStorageType.DateTime => (DbType.DateTime, "datetime"),
        DateTimeStorageType.DateTime2 => (DbType.DateTime2, "datetime2"),
        _ => throw new InvalidOperationException($"Enum value {dateTimeStorageTypeAttribute.StorageType} is not valid for {nameof(DateTimeStorageType)}.")
      };

      return new StorageTypeInformation(
          typeof(DateTime),
          storageTypeName,
          dbType,
          isNullableInDatabase,
          null,
          dotNetType,
          new DefaultConverter(dotNetType));
    }

    private StorageTypeInformation GetStorageTypeForExtensibleEnumType (Type extensibleEnumType, bool isNullableInDatabase)
    {
      var storageTypeLength = GetColumnWidthForExtensibleEnum(extensibleEnumType);
      var storageType = GetStorageTypeStringForVarType("varchar", storageTypeLength);
      return new StorageTypeInformation(
          typeof(string),
          storageType,
          DbType.AnsiString,
          isNullableInDatabase,
          storageTypeLength ?? StorageTypeLengthRepresentingMax,
          extensibleEnumType,
          new ExtensibleEnumConverter(extensibleEnumType));
    }

    [CanBeNull]
    private StorageTypeInformation? GetStorageTypeForNullableValueType (Type nullableValueType, Type underlyingType, int? maxLength, bool isNullableInDatabase, IPropertyInformation? propertyInformation)
    {
      var underlyingStorageInformation = GetStorageType(underlyingType, maxLength, false, propertyInformation);
      if (underlyingStorageInformation == null)
        return null;

      return new StorageTypeInformation(
          typeof(Nullable<>).MakeGenericType(underlyingStorageInformation.StorageType),
          underlyingStorageInformation.StorageTypeName,
          underlyingStorageInformation.StorageDbType,
          isNullableInDatabase,
          maxLength,
          nullableValueType,
          GetTypeConverter(nullableValueType, underlyingType));
    }

    private TypeConverter GetTypeConverter (Type nullableValueType, Type underlyingType)
    {
      if (underlyingType.IsEnum)
        return new AdvancedEnumConverter(nullableValueType);
      else if (underlyingType == typeof(DateOnly))
        return new DateOnlyConverter();
      else
        return new DefaultConverter(nullableValueType);
    }

    private StorageTypeInformation? GetStorageTypeForEnumType (Type enumType, int? maxLength, bool isNullableInDatabase)
    {
      var underlyingStorageInformation = GetStorageType(Enum.GetUnderlyingType(enumType), maxLength, isNullableInDatabase, null);
      if (underlyingStorageInformation == null)
        return null;

      return new StorageTypeInformation(
          underlyingStorageInformation.StorageType,
          underlyingStorageInformation.StorageTypeName,
          underlyingStorageInformation.StorageDbType,
          underlyingStorageInformation.IsStorageTypeNullable,
          maxLength,
          enumType,
          new AdvancedEnumConverter(enumType));
    }

    private string GetStorageTypeStringForVarType (string varType, int? maxLength)
    {
      return string.Format("{0} ({1})", varType, maxLength.HasValue ? maxLength.ToString() : "max");
    }

    private int? GetColumnWidthForExtensibleEnum (Type extensibleEnumType)
    {
      var extensibleEnumInfos = ExtensibleEnumUtility.GetDefinition(extensibleEnumType).GetValueInfos();
      if (extensibleEnumInfos.Count == 0)
        return null;
      return extensibleEnumInfos.Max(info => info.Value.ID.Length);
    }

    private bool IsNullSupported (Type dotNetType)
    {
      return NullableTypeUtility.IsNullableType(dotNetType);
    }
  }
}
