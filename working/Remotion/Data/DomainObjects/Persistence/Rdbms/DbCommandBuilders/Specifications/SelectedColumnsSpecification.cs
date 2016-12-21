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
using System.Linq;
using System.Text;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications
{
  /// <summary>
  /// The <see cref="SelectedColumnsSpecification"/> specifies that all specified columns should be selected.
  /// </summary>
  public class SelectedColumnsSpecification : ISelectedColumnsSpecification
  {
    private readonly ColumnDefinition[] _selectedColumns;

    public SelectedColumnsSpecification (IEnumerable<ColumnDefinition> selectedColumns)
    {
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);

      _selectedColumns = selectedColumns.ToArray();
    }

    public ReadOnlyCollection<ColumnDefinition> SelectedColumns
    {
      get { return Array.AsReadOnly(_selectedColumns); }
    }

    public void AppendProjection (StringBuilder stringBuilder, ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("stringBuilder", stringBuilder);
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

      stringBuilder.Append (string.Join (", ", _selectedColumns.Select (c => c == null ? "NULL" : sqlDialect.DelimitIdentifier(c.Name))));
    }

    public ISelectedColumnsSpecification Union (IEnumerable<ColumnDefinition> additionalColumns)
    {
      ArgumentUtility.CheckNotNull ("additionalColumns", additionalColumns);

      return new SelectedColumnsSpecification (_selectedColumns.Union (additionalColumns));
    }

    public ISelectedColumnsSpecification AdjustForTable (TableDefinition table)
    {
      ArgumentUtility.CheckNotNull ("table", table);

      return new SelectedColumnsSpecification (table.CalculateAdjustedColumnList (_selectedColumns));
    }
  }
}