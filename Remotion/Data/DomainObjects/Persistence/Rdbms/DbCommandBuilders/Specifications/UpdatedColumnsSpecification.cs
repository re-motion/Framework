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
  /// <summary>
  /// <see cref="UpdatedColumnsSpecification"/> specifies what columns should be updated, and with what values.
  /// </summary>
  public class UpdatedColumnsSpecification : IUpdatedColumnsSpecification
  {
    private readonly ColumnValue[] _columnValues;

    public UpdatedColumnsSpecification (IEnumerable<ColumnValue> columnValues)
    {
      ArgumentUtility.CheckNotNull("columnValues", columnValues);

      var columnValuesArray = columnValues.ToArray();
      ArgumentUtility.CheckNotEmpty("columnValues", columnValuesArray);

      _columnValues = columnValuesArray;
    }

    public ReadOnlyCollection<ColumnValue> ColumnValues
    {
      get { return Array.AsReadOnly(_columnValues); }
    }

    public void AppendColumnValueAssignments (StringBuilder statement, IDbCommand dbCommand, ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull("statement", statement);
      ArgumentUtility.CheckNotNull("dbCommand", dbCommand);
      ArgumentUtility.CheckNotNull("sqlDialect", sqlDialect);

      var columnsWithParameters = _columnValues.Select(
          cv =>
          {
            var storageTypeInfo = cv.Column.StorageTypeInfo;
            var parameterName = sqlDialect.GetParameterName(cv.Column.Name);
            var parameter = sqlDialect.CreateDataParameter(dbCommand, storageTypeInfo, parameterName, cv.Value);
            dbCommand.Parameters.Add(parameter);
            return new { ColumnDefinition = cv.Column, Parameter = parameter };
          });

      var updateStatement = string.Join(
          ", ",
          columnsWithParameters.Select(cp => string.Format("{0} = {1}", sqlDialect.DelimitIdentifier(cp.ColumnDefinition.Name), cp.Parameter.ParameterName)));
      statement.Append(updateStatement);
    }
  }
}
