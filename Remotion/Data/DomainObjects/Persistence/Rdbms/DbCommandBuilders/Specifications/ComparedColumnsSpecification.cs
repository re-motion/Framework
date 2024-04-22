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
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications
{
  public class ComparedColumnsSpecification : IComparedColumnsSpecification
  {
    private readonly ColumnValue[] _comparedColumnValues;

    public ComparedColumnsSpecification (IEnumerable<ColumnValue> comparedColumnValues)
    {
      ArgumentUtility.CheckNotNull("comparedColumnValues", comparedColumnValues);
      _comparedColumnValues = comparedColumnValues.ToArray();

      if (_comparedColumnValues.Length == 0)
        throw new ArgumentException("The sequence of compared column values must contain at least one element.", "comparedColumnValues");
    }

    public ReadOnlyCollection<ColumnValue> ComparedColumnValues
    {
      get { return Array.AsReadOnly(_comparedColumnValues); }
    }

    public void AddParameters (IDbCommand command, ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull("command", command);
      ArgumentUtility.CheckNotNull("sqlDialect", sqlDialect);

      foreach (var comparedColumnValue in _comparedColumnValues)
      {
        var storageTypeInfo = comparedColumnValue.Column.StorageTypeInfo;
        var parameterName = GetParameterName(comparedColumnValue, sqlDialect);
        var parameter = sqlDialect.CreateDataParameter(command, storageTypeInfo, parameterName, comparedColumnValue.Value);
        command.Parameters.Add(parameter);
      }
    }

    public void AppendComparisons (
        StringBuilder statement, IDbCommand command, ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull("statement", statement);
      ArgumentUtility.CheckNotNull("command", command);
      ArgumentUtility.CheckNotNull("sqlDialect", sqlDialect);

      bool first = true;

      foreach (var comparedColumnValue in _comparedColumnValues)
      {
        if (!first)
          statement.Append(" AND ");

        statement.Append(sqlDialect.DelimitIdentifier(comparedColumnValue.Column.Name));
        statement.Append(" = ");

        statement.Append(GetParameterName(comparedColumnValue, sqlDialect));

        first = false;
      }
    }

    private string GetParameterName (ColumnValue comparedColumnValue, ISqlDialect sqlDialect)
    {
      return sqlDialect.GetParameterName(comparedColumnValue.Column.Name);
    }
  }
}
