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
using System.Data;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model.Building;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;

/// <summary>
/// Adjusts the <see cref="IDbDataParameter.Size"/> of the created max-length <see cref="DbType.String"/> or <see cref="DbType.AnsiString"/> <see cref="IDbDataParameter"/>,
/// so that it is compatible with MSSQL fulltext indexing.
/// </summary>
public class SqlFulltextDataParameterDefinitionDecorator : IDataParameterDefinition
{
  public IDataParameterDefinition InnerDataParameterDefinition { get; }

  public SqlFulltextDataParameterDefinitionDecorator (IDataParameterDefinition innerDataParameterDefinition)
  {
    ArgumentUtility.CheckNotNull(nameof(innerDataParameterDefinition), innerDataParameterDefinition);

    InnerDataParameterDefinition = innerDataParameterDefinition;
  }

  public object GetParameterValue (object? value)
  {
    return InnerDataParameterDefinition.GetParameterValue(value);
  }

  public IDbDataParameter CreateDataParameter (IDbCommand command, string parameterName, object parameterValue)
  {
    ArgumentUtility.CheckNotNull(nameof(command), command);
    ArgumentUtility.CheckNotNullOrEmpty(nameof(parameterName), parameterName);
    ArgumentUtility.CheckNotNull(nameof(parameterValue), parameterValue);

    var parameter = InnerDataParameterDefinition.CreateDataParameter(command, parameterName, parameterValue);

    var stringValue = parameterValue as string;
    var charArray = parameterValue as char[];
    if (stringValue != null || charArray != null)
    {
      var length = stringValue?.Length ?? charArray!.Length;
      var hasMaxLength = parameter.Size == SqlStorageTypeInformationProvider.StorageTypeLengthRepresentingMax;
      var isAnsiString = parameter.DbType == DbType.AnsiString; //AnsiStringFixedLength represents char(...), which does not support char(max)
      var isUnicodeString = parameter.DbType == DbType.String; //StringFixedLength represents char(...), which does not support char(max)

      if (hasMaxLength && isAnsiString && length <= 8000)
        parameter.Size = 8000;

      if (hasMaxLength && isUnicodeString && length <= 4000)
        parameter.Size = 4000;
    }

    return parameter;
  }
}
