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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  /// <summary>
  /// The <see cref="StorageTypeInformation"/> provides information about the storage type of a value in a relational database.
  /// In addition, it can create an unnamed <see cref="IDbDataParameter"/> for a value convertible to <see cref="StorageType"/> via 
  /// <see cref="DotNetTypeConverter"/>, or read and convert a value from an <see cref="IDataReader"/>.
  /// </summary>
  /// <remarks>
  /// The <see cref="DotNetTypeConverter"/> must be associated with the in-memory .NET type of the stored value. It is used to convert to the database
  /// representation (represented by <see cref="StorageType"/>) when a <see cref="IDbDataParameter"/> is created, and it is used to convert
  /// values back to the <see cref="DotNetType"/> when a value is read from an <see cref="IDataReader"/>.
  /// </remarks>
  public class StorageTypeInformation : IStorageTypeInformation
  {
    private readonly Type _storageType;
    private readonly string _storageTypeName;
    private readonly DbType _storageDbType;
    private readonly bool _isStorageTypeNullable;
    private readonly Type _dotNetType;
    private readonly TypeConverter _dotNetTypeConverter;

    public StorageTypeInformation (Type storageType, string storageTypeName, DbType storageDbType, bool isStorageTypeNullable, Type dotNetType, TypeConverter dotNetTypeConverter)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("storageTypeName", storageTypeName);
      ArgumentUtility.CheckNotNull ("storageType", storageType);
      ArgumentUtility.CheckNotNull ("dotNetType", dotNetType);
      ArgumentUtility.CheckNotNull ("dotNetTypeConverter", dotNetTypeConverter);

      _storageType = storageType;
      _storageTypeName = storageTypeName;
      _storageDbType = storageDbType;
      _isStorageTypeNullable = isStorageTypeNullable;
      _dotNetType = dotNetType;
      _dotNetTypeConverter = dotNetTypeConverter;
    }

    /// <inheritdoc />
    public Type StorageType
    {
      get { return _storageType; }
    }

    /// <inheritdoc />
    public string StorageTypeName
    {
      get { return _storageTypeName; }
    }

    /// <summary>
    /// Gets the <see cref="DbType"/> value corresponding to the storage type.
    /// </summary>
    /// <value>The <see cref="DbType"/> of the storage type.</value>
    public DbType StorageDbType
    {
      get { return _storageDbType; }
    }

    public bool IsStorageTypeNullable
    {
      get { return _isStorageTypeNullable; }
    }

    /// <inheritdoc />
    public Type DotNetType
    {
      get { return _dotNetType; }
    }

    /// <summary>
    /// Gets a <see cref="System.ComponentModel.TypeConverter"/> that can converts a value from the <see cref="DotNetType"/> (e.g., an enum type) 
    /// to the <see cref="StorageType"/> (e.g., <see cref="int"/>) and back.
    /// </summary>
    /// <value>The type converter for the actual .NET type.</value>
    /// <remarks>
    /// The <see cref="DotNetTypeConverter"/> is used to convert the values passed into <see cref="CreateDataParameter"/> to the underlying 
    /// <see cref="StorageType"/>. That way, an enum value passed into <see cref="CreateDataParameter"/> can be converted to the underlying
    /// <see cref="int"/> type when it is to be written into the database. Conversely, <see cref="Read"/> uses the <see cref="DotNetTypeConverter"/> to
    /// convert values read from the database (which should usually be of the <see cref="StorageType"/>) back to the expected 
    /// <see cref="DotNetType"/>. That way, e.g, an <see cref="int"/> value can become an enum value again.
    /// </remarks>
    public TypeConverter DotNetTypeConverter
    {
      get { return _dotNetTypeConverter; }
    }

    public IDbDataParameter CreateDataParameter (IDbCommand command, object value)
    {
      ArgumentUtility.CheckNotNull ("command", command);

      var convertedValue = ConvertToStorageType(value);

      var parameter = command.CreateParameter ();
      parameter.Value = convertedValue;
      parameter.DbType = StorageDbType;
      return parameter;
    }

    public object Read (IDataReader dataReader, int ordinal)
    {
      ArgumentUtility.CheckNotNull ("dataReader", dataReader);

      var value = dataReader[ordinal];
      return ConvertFromStorageType (value);
    }

    /// <inheritdoc />
    public object ConvertToStorageType (object dotNetValue)
    {
      return DotNetTypeConverter.ConvertTo (dotNetValue, StorageType) ?? DBNull.Value;
    }

    /// <inheritdoc />
    public object ConvertFromStorageType (object storageValue)
    {
      if (storageValue == DBNull.Value)
        storageValue = null;

      return DotNetTypeConverter.ConvertFrom (storageValue);
    }

    public IStorageTypeInformation UnifyForEquivalentProperties (IEnumerable<IStorageTypeInformation> equivalentStorageTypes)
    {
      ArgumentUtility.CheckNotNull ("equivalentStorageTypes", equivalentStorageTypes);
      var castStorageTypes =
          equivalentStorageTypes.Select (
              equivalentInfo =>
              StoragePropertyDefinitionUnificationUtility.CheckAndConvertEquivalentProperty (
                  this,
                  equivalentInfo,
                  "equivalentStorageTypes",
                  info => Tuple.Create<string, object> ("storage type", info.StorageType),
                  info => Tuple.Create<string, object> ("storage type name", info.StorageTypeName),
                  info => Tuple.Create<string, object> ("storage DbType", info.StorageDbType),
                  info => Tuple.Create<string, object> (".NET type", info.DotNetType),
                  info => Tuple.Create<string, object> (".NET type converter type", info.DotNetTypeConverter.GetType())));

      return new StorageTypeInformation (
          _storageType,
          _storageTypeName,
          _storageDbType,
          castStorageTypes.Any (x => x._isStorageTypeNullable) || _isStorageTypeNullable,
          _dotNetType,
          _dotNetTypeConverter);
    }
  }
}