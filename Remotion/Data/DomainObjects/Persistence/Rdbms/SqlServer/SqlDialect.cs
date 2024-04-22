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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer
{
  /// <summary>
  /// Defines the <see cref="ISqlDialect"/> for MS SQL Server.
  /// </summary>
  public class SqlDialect : ISqlDialect
  {
    public SqlDialect ()
    {
    }

    public virtual string StatementDelimiter
    {
      get { return ";"; }
    }

    public virtual string GetParameterName (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty("name", name);

      if (name.StartsWith("@"))
        return name;
      else
        return "@" + name;
    }

    public virtual string DelimitIdentifier (string identifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty("identifier", identifier);

      return "[" + identifier + "]";
    }

    public IDbDataParameter CreateDataParameter (IDbCommand command, IStorageTypeInformation storageTypeInformation, string parameterName, object? value)
    {
      ArgumentUtility.CheckNotNull("command", command);
      ArgumentUtility.CheckNotNull("storageTypeInformation", storageTypeInformation);
      ArgumentUtility.CheckNotNullOrEmpty("parameterName", parameterName);

      var convertedValue = storageTypeInformation.ConvertToStorageType(value);

      var parameter = command.CreateParameter();
      parameter.ParameterName = GetParameterName(parameterName);
      parameter.Value = convertedValue;
      parameter.DbType = storageTypeInformation.StorageDbType;
      if (storageTypeInformation.StorageTypeLength.HasValue)
      {
        var parameterSize = storageTypeInformation.StorageTypeLength.Value;
        if (parameterSize == SqlStorageTypeInformationProvider.StorageTypeLengthRepresentingMax)
        {
          var fulltextCompatibleMaxLength = storageTypeInformation.StorageDbType switch
          {
              DbType.AnsiString => 8000,
              DbType.String => 4000,
              DbType.AnsiStringFixedLength  => default(int?), // represents char(...), which do not support "max"
              DbType.StringFixedLength => default(int?), // represents nchar(...), which do not support "max"
              _ => default(int?)
          };

          if (fulltextCompatibleMaxLength.HasValue)
          {
            var replaceMaxSizeWithFulltextCompatibleSize = convertedValue switch
            {
                DBNull => true,
                null => true,
                string stringValue => stringValue.Length <= fulltextCompatibleMaxLength.Value,
                char[] charsValue => charsValue.Length <= fulltextCompatibleMaxLength.Value,
                _ => false
            };

            if (replaceMaxSizeWithFulltextCompatibleSize)
              parameter.Size = fulltextCompatibleMaxLength.Value;
          }
          else
          {
            parameter.Size = parameterSize;
          }
        }
        else
        {
          var isValueWithinParameterSize = convertedValue switch
          {
              string stringValue => stringValue.Length <= parameterSize,
              char[] charsValue => charsValue.Length <= parameterSize,
              byte[] bytesValue => bytesValue.Length <= parameterSize,
              _ => true
          };

          if (isValueWithinParameterSize)
            parameter.Size = parameterSize;
        }
      }

      return parameter;
    }
  }
}
