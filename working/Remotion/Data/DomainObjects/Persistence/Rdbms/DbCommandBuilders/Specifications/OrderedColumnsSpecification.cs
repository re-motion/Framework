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
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications
{
  /// <summary>
  /// The <see cref="OrderedColumnsSpecification"/> defines that the selected data should be ordered by the given columns.
  /// </summary>
  public class OrderedColumnsSpecification : IOrderedColumnsSpecification
  {
    public static OrderedColumnsSpecification CreateEmpty ()
    {
      return new OrderedColumnsSpecification (new OrderedColumn[0]);
    }

    private readonly OrderedColumn[] _columns;

    public OrderedColumnsSpecification (IEnumerable<OrderedColumn> columns)
    {
      ArgumentUtility.CheckNotNull ("columns", columns);

      _columns = columns.ToArray();
    }

    public ReadOnlyCollection<OrderedColumn> Columns
    {
      get { return Array.AsReadOnly (_columns); }
    }

    public bool IsEmpty
    {
      get { return !_columns.Any(); }
    }

    public void AppendOrderings (StringBuilder stringBuilder, ISqlDialect sqlDialect)
    {
      ArgumentUtility.CheckNotNull ("stringBuilder", stringBuilder);
      ArgumentUtility.CheckNotNull ("sqlDialect", sqlDialect);

      stringBuilder.Append (
          string.Join (", ", _columns.Select (orderedColumn =>
              sqlDialect.DelimitIdentifier (orderedColumn.ColumnDefinition.Name) + (orderedColumn.SortOrder == SortOrder.Ascending ? " ASC" : " DESC"))));
    }

    public ISelectedColumnsSpecification UnionWithSelectedColumns (ISelectedColumnsSpecification selectedColumns)
    {
      ArgumentUtility.CheckNotNull ("selectedColumns", selectedColumns);

      if (!_columns.Any ())
        return selectedColumns;
      
      return selectedColumns.Union (_columns.Select (orderedColumn => orderedColumn.ColumnDefinition));
    }
  }
}