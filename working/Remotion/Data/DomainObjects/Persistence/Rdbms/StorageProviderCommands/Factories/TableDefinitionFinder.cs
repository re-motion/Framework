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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories
{
  /// <summary>
  /// The <see cref="TableDefinitionFinder"/> is responsible to get the <see cref="TableDefinition"/> for an <see cref="ObjectID"/>-class definition.
  /// </summary>
  public class TableDefinitionFinder : ITableDefinitionFinder
  {
    private readonly IRdbmsPersistenceModelProvider _rdbmsPersistenceModelProvider;

    public TableDefinitionFinder (IRdbmsPersistenceModelProvider rdbmsPersistenceModelProvider)
    {
      ArgumentUtility.CheckNotNull ("rdbmsPersistenceModelProvider", rdbmsPersistenceModelProvider);

      _rdbmsPersistenceModelProvider = rdbmsPersistenceModelProvider;
    }

    public TableDefinition GetTableDefinition (ObjectID objectID)
    {
      return InlineRdbmsStorageEntityDefinitionVisitor.Visit<TableDefinition> (
          _rdbmsPersistenceModelProvider.GetEntityDefinition (objectID.ClassDefinition),
          (table, continuation) => table,
          (filterView, continuation) => continuation (filterView.BaseEntity),
          (unionView, continuation) => { throw new InvalidOperationException ("An ObjectID's EntityDefinition cannot be a UnionViewDefinition."); },
          (emptyView, continuation) => { throw new InvalidOperationException ("An ObjectID's EntityDefinition cannot be a EmptyViewDefinition."); });
    }
  }
}