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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.CustomDataTypeSupport.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.SqlServer.IntegrationTests.CustomDataTypeSupport
{
  public class SimpleDataTypeStorageTypeInformation : IStorageTypeInformation
  {
    private readonly string _storageTypeName;
    private readonly DbType _storageDbType;
    private readonly bool _isStorageTypeNullable;
    private readonly int? _storageTypeLength;

    public SimpleDataTypeStorageTypeInformation (
        string storageTypeName,
        DbType storageDbType,
        bool isStorageTypeNullable,
        int? storageTypeLength)
    {
      ArgumentUtility.CheckNotNullOrEmpty("storageTypeName", storageTypeName);

      _storageTypeName = storageTypeName;
      _storageDbType = storageDbType;
      _isStorageTypeNullable = isStorageTypeNullable;
      _storageTypeLength = storageTypeLength;
    }

    public Type StorageType
    {
      get { return typeof(SimpleDataType); }
    }

    public string StorageTypeName
    {
      get { return _storageTypeName; }
    }

    public DbType StorageDbType
    {
      get { return _storageDbType; }
    }

    public bool IsStorageTypeNullable
    {
      get { return _isStorageTypeNullable; }
    }

    public int? StorageTypeLength
    {
      get { return _storageTypeLength; }
    }

    public Type DotNetType
    {
      get { return typeof(string); }
    }

    public IDbDataParameter CreateDataParameter (IDbCommand command, object value)
    {
      ArgumentUtility.CheckNotNull("command", command);

      var convertedValue = ConvertToStorageType(value);

      var parameter = command.CreateParameter();
      parameter.Value = convertedValue;
      parameter.DbType = StorageDbType;
      if (StorageTypeLength.HasValue)
      {
        var parameterSize = StorageTypeLength.Value;
        var isStringAndValueExceedsParameterSize = convertedValue is string && ((string) convertedValue).Length > parameterSize;

        if (!isStringAndValueExceedsParameterSize)
          parameter.Size = parameterSize;
      }

      return parameter;
    }

    public object Read (IDataReader dataReader, int ordinal)
    {
      ArgumentUtility.CheckNotNull("dataReader", dataReader);

      var value = dataReader[ordinal];
      return ConvertFromStorageType(value);
    }

    public object ConvertToStorageType (object dotNetValue)
    {
      if (dotNetValue == null)
        return DBNull.Value;
      var simpleDataType = (SimpleDataType) dotNetValue;
      return simpleDataType.StringValue;
    }

    public object ConvertFromStorageType (object storageValue)
    {
      if (storageValue == DBNull.Value)
        return null;

      return new SimpleDataType((string) storageValue);
    }

    public IStorageTypeInformation UnifyForEquivalentProperties (IEnumerable<IStorageTypeInformation> equivalentStorageTypes)
    {
      ArgumentUtility.CheckNotNull("equivalentStorageTypes", equivalentStorageTypes);
      var castStorageTypes =
          equivalentStorageTypes.Select(
              equivalentInfo =>
                  StoragePropertyDefinitionUnificationUtility.CheckAndConvertEquivalentProperty(
                      this,
                      equivalentInfo,
                      "equivalentStorageTypes",
                      info => Tuple.Create<string, object>("storage type name", info.StorageTypeName),
                      info => Tuple.Create<string, object>("storage DbType", info.StorageDbType),
                      info => Tuple.Create<string, object>("storage type length", info.StorageTypeLength)))
              .ToArray();

      return new SimpleDataTypeStorageTypeInformation(
          _storageTypeName,
          _storageDbType,
          _isStorageTypeNullable || castStorageTypes.Any(x => x._isStorageTypeNullable),
          _storageTypeLength);
    }
  }
}
