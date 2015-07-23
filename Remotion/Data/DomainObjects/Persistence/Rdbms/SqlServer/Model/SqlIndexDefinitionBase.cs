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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model
{
  /// <summary>
  /// Acts as a base class for classes defining SQL Server indexes.
  /// </summary>
  public abstract class SqlIndexDefinitionBase : IIndexDefinition
  {
    private readonly bool? _padIndex;
    private readonly int? _fillFactor;
    private readonly bool? _sortInDb;
    private readonly bool? _statisticsNoReCompute;
    private readonly bool? _dropExisiting;
    private readonly bool? _allowRowLocks;
    private readonly bool? _allowPageLocks;
    private readonly int? _maxDop;

    protected SqlIndexDefinitionBase (
        bool? padIndex,
        int? fillFactor,
        bool? sortInTempDb,
        bool? statisticsNoReCompute,
        bool? dropExisting,
        bool? allowRowLocks,
        bool? allowPageLocks,
        int? maxDop)
    {
      _padIndex = padIndex;
      _fillFactor = fillFactor;
      _sortInDb = sortInTempDb;
      _statisticsNoReCompute = statisticsNoReCompute;
      _dropExisiting = dropExisting;
      _allowRowLocks = allowRowLocks;
      _allowPageLocks = allowPageLocks;
      _maxDop = maxDop;
    }

    public abstract string IndexName { get; }

    public bool? PadIndex
    {
      get { return _padIndex; }
    }

    public int? FillFactor
    {
      get { return _fillFactor; }
    }

    public bool? SortInDb
    {
      get { return _sortInDb; }
    }

    public bool? StatisticsNoReCompute
    {
      get { return _statisticsNoReCompute; }
    }

    public bool? DropExisiting
    {
      get { return _dropExisiting; }
    }

    public bool? AllowRowLocks
    {
      get { return _allowRowLocks; }
    }

    public bool? AllowPageLocks
    {
      get { return _allowPageLocks; }
    }

    public int? MaxDop
    {
      get { return _maxDop; }
    }

    public void Accept (IIndexDefinitionVisitor visitor)
    {
      var specificVisitor = visitor as ISqlIndexDefinitionVisitor;
      if (specificVisitor != null)
        Accept (specificVisitor);
    }

    protected abstract void Accept (ISqlIndexDefinitionVisitor visitor);
  }
}