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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model
{
  /// <summary>
  /// <see cref="SqlIndexDefinition"/> represents an index on a table or view in a SQL Server database.
  /// </summary>
  public class SqlIndexDefinition : SqlIndexDefinitionBase
  {
    private readonly string _indexName;
    private readonly ReadOnlyCollection<SqlIndexedColumnDefinition> _columns;
    private readonly ReadOnlyCollection<ColumnDefinition> _includedColumns;
    private readonly bool? _isClustered;
    private readonly bool? _isUnique;
    private readonly bool? _ignoreDupKey;
    private readonly bool? _online;

    public SqlIndexDefinition (
        string indexName,
        IEnumerable<SqlIndexedColumnDefinition> columns,
        IEnumerable<ColumnDefinition> includedColumns = null,
        bool? isClustered = false,
        bool? isUnique = false,
        bool? ignoreDupKey = null,
        bool? online = null,
        bool? padIndex = null,
        int? fillFactor = null,
        bool? sortInTempDb = null,
        bool? statisticsNoReCompute = null,
        bool? dropExisting = null,
        bool? allowRowLocks = null,
        bool? allowPageLocks = null,
        int? maxDop = null)
        : base (padIndex, fillFactor, sortInTempDb, statisticsNoReCompute, dropExisting, allowRowLocks, allowPageLocks, maxDop)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("indexName", indexName);
      ArgumentUtility.CheckNotNullOrEmpty ("columns", columns);
      ArgumentUtility.CheckNotEmpty ("includedColumns", includedColumns);

      _indexName = indexName;
      _columns = columns.ToList().AsReadOnly();
      if (includedColumns != null)
        _includedColumns = includedColumns.ToList().AsReadOnly();
      _isClustered = isClustered;
      _isUnique = isUnique;
      _ignoreDupKey = ignoreDupKey;
      _online = online;
    }

    public override string IndexName
    {
      get { return _indexName; }
    }

    public ReadOnlyCollection<SqlIndexedColumnDefinition> Columns
    {
      get { return _columns; }
    }

    public ReadOnlyCollection<ColumnDefinition> IncludedColumns
    {
      get { return _includedColumns; }
    }

    public bool? IsClustered
    {
      get { return _isClustered; }
    }

    public bool? IsUnique
    {
      get { return _isUnique; }
    }

    public bool? IgnoreDupKey
    {
      get { return _ignoreDupKey; }
    }

    public bool? Online
    {
      get { return _online; }
    }

    protected override void Accept (ISqlIndexDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitIndexDefinition (this);
    }
  }
}