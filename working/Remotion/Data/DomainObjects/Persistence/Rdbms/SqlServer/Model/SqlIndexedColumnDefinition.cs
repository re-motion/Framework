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
  /// <see cref="SqlIndexedColumnDefinition"/> represents an indexed-column in a SQL-server database.
  /// </summary>
  public class SqlIndexedColumnDefinition
  {
    private readonly ColumnDefinition _columnn;
    private readonly IndexOrder? _indexOrder;

    public SqlIndexedColumnDefinition (ColumnDefinition columnn, IndexOrder? indexOrder = null)
    {
      ArgumentUtility.CheckNotNull ("columnn", columnn);

      _columnn = columnn;
      _indexOrder = indexOrder;
    }

    public ColumnDefinition Columnn
    {
      get { return _columnn; }
    }

    public IndexOrder? IndexOrder
    {
      get { return _indexOrder; }
    }
    
  }
}