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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model
{
  /// <summary>
  /// <see cref="SqlSecondaryXmlIndexDefinition"/> represents a secondary xml-column index in a relational database.
  /// </summary>
  public class SqlSecondaryXmlIndexDefinition : SqlIndexDefinitionBase
  {
    private readonly string _indexName;
    private readonly ColumnDefinition _xmlColumn;
    private readonly string _primaryIndexName;
    private readonly SqlSecondaryXmlIndexKind _kind;

    public SqlSecondaryXmlIndexDefinition (
        string indexName,
        ColumnDefinition xmlColumn,
        string primaryIndexName,
        SqlSecondaryXmlIndexKind kind,
        bool? padIndex = null,
        int? fillFactor = null,
        bool? sortInTempDb = null,
        bool? statisticsNoReCompute = null,
        bool? dropExisting = null,
        bool? allowRowLocks = null,
        bool? allowPageLocks = null,
        int? maxDop = null)
        : base(padIndex, fillFactor, sortInTempDb, statisticsNoReCompute, dropExisting, allowRowLocks, allowPageLocks, maxDop)
    {
      ArgumentUtility.CheckNotNullOrEmpty("indexName", indexName);
      ArgumentUtility.CheckNotNull("xmlColumn", xmlColumn);
      ArgumentUtility.CheckNotNullOrEmpty("primaryIndexName", primaryIndexName);

      _indexName = indexName;
      _xmlColumn = xmlColumn;
      _primaryIndexName = primaryIndexName;
      _kind = kind;
    }

    public override string IndexName
    {
      get { return _indexName; }
    }

    public ColumnDefinition XmlColumn
    {
      get { return _xmlColumn; }
    }

    public string PrimaryIndexName
    {
      get { return _primaryIndexName; }
    }

    public SqlSecondaryXmlIndexKind Kind
    {
      get { return _kind; }
    }

    protected override void Accept (ISqlIndexDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull("visitor", visitor);

      visitor.VisitSecondaryXmlIndexDefinition(this);
    }
  }
}
