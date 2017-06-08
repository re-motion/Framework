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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model
{
  /// <summary>
  /// Decorates a <see cref="IStorageTypeInformation"/> and ensures that the size of the <see cref="IDataParameter"/> returned by <see cref="CreateDataParameter"/>
  /// does not specify <b>max</b> for <see cref="string"/> values that fit within the <see cref="FulltextCompatibleMaxLength"/>.
  /// </summary>
  /// <remarks>The SQL Server fulltext keyboards (<c>CONTAINS</c>, etc) do not support parameters of type <c>nvarchar(max)</c> </remarks>
  public class SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator : IStorageTypeInformation
  {
    private readonly IStorageTypeInformation _innerStorageTypeInformation;
    private readonly int _fulltextCompatibleMaxLength;

    public SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator (
        IStorageTypeInformation innerStorageTypeInformation,
        int fulltextCompatibleMaxLength)
    {
      ArgumentUtility.CheckNotNull ("innerStorageTypeInformation", innerStorageTypeInformation);

      _innerStorageTypeInformation = innerStorageTypeInformation;
      _fulltextCompatibleMaxLength = fulltextCompatibleMaxLength;
    }

    public IStorageTypeInformation InnerStorageTypeInformation
    {
      get { return _innerStorageTypeInformation; }
    }

    public int FulltextCompatibleMaxLength
    {
      get { return _fulltextCompatibleMaxLength; }
    }

    public Type StorageType
    {
      get { return _innerStorageTypeInformation.StorageType; }
    }

    public string StorageTypeName
    {
      get { return _innerStorageTypeInformation.StorageTypeName; }
    }

    public bool IsStorageTypeNullable
    {
      get { return _innerStorageTypeInformation.IsStorageTypeNullable; }
    }

    public Type DotNetType
    {
      get { return _innerStorageTypeInformation.DotNetType; }
    }

    public IDbDataParameter CreateDataParameter (IDbCommand command, object value)
    {
      var dbDataParameter = _innerStorageTypeInformation.CreateDataParameter (command, value);

      var isNullValue = dbDataParameter.Value == null || dbDataParameter.Value == DBNull.Value;
      var isStringAndValueDoesNotExceedCompatibleMaxLength =
          dbDataParameter.Value is string && ((string) dbDataParameter.Value).Length <= _fulltextCompatibleMaxLength;
      var isCharArrayAndValueDoesNotExceedCompatibleMaxLength =
          dbDataParameter.Value is char[] && ((char[]) dbDataParameter.Value).Length <= _fulltextCompatibleMaxLength;

      if (isNullValue || isStringAndValueDoesNotExceedCompatibleMaxLength || isCharArrayAndValueDoesNotExceedCompatibleMaxLength)
        dbDataParameter.Size = _fulltextCompatibleMaxLength;

      return dbDataParameter;
    }

    public object Read (IDataReader dataReader, int ordinal)
    {
      return _innerStorageTypeInformation.Read (dataReader, ordinal);
    }

    public object ConvertToStorageType (object dotNetValue)
    {
      return _innerStorageTypeInformation.ConvertToStorageType (dotNetValue);
    }

    public object ConvertFromStorageType (object storageValue)
    {
      return _innerStorageTypeInformation.ConvertFromStorageType (storageValue);
    }

    public IStorageTypeInformation UnifyForEquivalentProperties (IEnumerable<IStorageTypeInformation> equivalentStorageTypes)
    {
      ArgumentUtility.CheckNotNull ("equivalentStorageTypes", equivalentStorageTypes);

      var unwrappedStorageTypes = equivalentStorageTypes
          .Select (
              p => StoragePropertyDefinitionUnificationUtility.CheckAndConvertEquivalentProperty (
                  this,
                  p,
                  "equivalentStorageTypes",
                  info => Tuple.Create<string, object> ("fulltext compatible max-length", info.FulltextCompatibleMaxLength)
                  ))
          .Select (p => p._innerStorageTypeInformation);
      var unifiedInnerStorageType = _innerStorageTypeInformation.UnifyForEquivalentProperties (unwrappedStorageTypes);

      return new SqlFulltextQueryCompatibleStringPropertyStorageTypeInformationDecorator (unifiedInnerStorageType, _fulltextCompatibleMaxLength);
    }
  }
}