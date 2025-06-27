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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms
{
  /// <summary>
  /// Visits the given <see cref="IRdbmsStorageEntityDefinition"/> and executes a handler based on the entity's type.
  /// </summary>
  public class InlineRdbmsStorageEntityDefinitionVisitor
  {
    public static T Visit<T> (
        IRdbmsStorageEntityDefinition entityDefinition,
        Func<TableDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> tableDefinitionHandler,
        Func<FilterViewDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> filterViewDefinitionHandler,
        Func<UnionViewDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> unionViewDefinitionHandler,
        Func<EmptyViewDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> emptyViewDefinitionHandler)
    {
      ArgumentNullException.ThrowIfNull(entityDefinition);
      ArgumentNullException.ThrowIfNull(tableDefinitionHandler);
      ArgumentNullException.ThrowIfNull(filterViewDefinitionHandler);
      ArgumentNullException.ThrowIfNull(unionViewDefinitionHandler);
      ArgumentNullException.ThrowIfNull(emptyViewDefinitionHandler);

      var visitor = new RdbmsStorageEntityDefinitionVisitor<T>(
          tableDefinitionHandler,
          filterViewDefinitionHandler,
          unionViewDefinitionHandler,
          emptyViewDefinitionHandler);
      entityDefinition.Accept(visitor);
      return visitor.ReturnValue;
    }

    public static void Visit (
        IRdbmsStorageEntityDefinition entityDefinition,
        Action<TableDefinition, Action<IRdbmsStorageEntityDefinition>> tableDefinitionHandler,
        Action<FilterViewDefinition, Action<IRdbmsStorageEntityDefinition>> filterViewDefinitionHandler,
        Action<UnionViewDefinition, Action<IRdbmsStorageEntityDefinition>> unionViewDefinitionHandler,
        Action<EmptyViewDefinition, Action<IRdbmsStorageEntityDefinition>> emptyViewDefinitionHandler)
    {
      ArgumentNullException.ThrowIfNull(entityDefinition);
      ArgumentNullException.ThrowIfNull(tableDefinitionHandler);
      ArgumentNullException.ThrowIfNull(filterViewDefinitionHandler);
      ArgumentNullException.ThrowIfNull(unionViewDefinitionHandler);
      ArgumentNullException.ThrowIfNull(emptyViewDefinitionHandler);

      var visitor = new RdbmsStorageEntityDefinitionVisitor(
          tableDefinitionHandler,
          filterViewDefinitionHandler,
          unionViewDefinitionHandler,
          emptyViewDefinitionHandler);

      entityDefinition.Accept(visitor);
    }

    private class RdbmsStorageEntityDefinitionVisitor<T> : IRdbmsStorageEntityDefinitionVisitor
    {
      private readonly Func<TableDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> _tableDefinitionHandler;
      private readonly Func<FilterViewDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> _filterViewDefinitionHandler;
      private readonly Func<UnionViewDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> _unionViewDefinitionHandler;
      private readonly Func<EmptyViewDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> _emptyViewDefinitionHandler;

      private T _returnValue;

      public RdbmsStorageEntityDefinitionVisitor (
          Func<TableDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> tableDefinitionHandler,
          Func<FilterViewDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> filterViewDefinitionHandler,
          Func<UnionViewDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> unionViewDefinitionHandler,
          Func<EmptyViewDefinition, Func<IRdbmsStorageEntityDefinition, T>, T> emptyViewDefinitionHandler)
      {
        ArgumentNullException.ThrowIfNull(tableDefinitionHandler);
        ArgumentNullException.ThrowIfNull(filterViewDefinitionHandler);
        ArgumentNullException.ThrowIfNull(unionViewDefinitionHandler);
        ArgumentNullException.ThrowIfNull(emptyViewDefinitionHandler);

        _tableDefinitionHandler = tableDefinitionHandler;
        _filterViewDefinitionHandler = filterViewDefinitionHandler;
        _unionViewDefinitionHandler = unionViewDefinitionHandler;
        _emptyViewDefinitionHandler = emptyViewDefinitionHandler;
        _returnValue = default!;
      }

      public T ReturnValue
      {
        get { return _returnValue; }
      }

      public void VisitTableDefinition (TableDefinition tableDefinition)
      {
        ArgumentNullException.ThrowIfNull(tableDefinition);

        _returnValue = _tableDefinitionHandler(tableDefinition, ContinueWithNextEntity);
      }

      public void VisitUnionViewDefinition (UnionViewDefinition unionViewDefinition)
      {
        ArgumentNullException.ThrowIfNull(unionViewDefinition);

        _returnValue = _unionViewDefinitionHandler(unionViewDefinition, ContinueWithNextEntity);
      }

      public void VisitFilterViewDefinition (FilterViewDefinition filterViewDefinition)
      {
        ArgumentNullException.ThrowIfNull(filterViewDefinition);

        _returnValue = _filterViewDefinitionHandler(filterViewDefinition, ContinueWithNextEntity);
      }

      public void VisitEmptyViewDefinition (EmptyViewDefinition emptyViewDefinition)
      {
        ArgumentNullException.ThrowIfNull(emptyViewDefinition);

        _returnValue = _emptyViewDefinitionHandler(emptyViewDefinition, ContinueWithNextEntity);
      }

      private T ContinueWithNextEntity (IRdbmsStorageEntityDefinition entityDefinition)
      {
        entityDefinition.Accept(this);
        return _returnValue;
      }
    }

    private class RdbmsStorageEntityDefinitionVisitor : IRdbmsStorageEntityDefinitionVisitor
    {
      private readonly Action<TableDefinition, Action<IRdbmsStorageEntityDefinition>> _tableDefinitionHandler;
      private readonly Action<FilterViewDefinition, Action<IRdbmsStorageEntityDefinition>> _filterViewDefinitionHandler;
      private readonly Action<UnionViewDefinition, Action<IRdbmsStorageEntityDefinition>> _unionViewDefinitionHandler;
      private readonly Action<EmptyViewDefinition, Action<IRdbmsStorageEntityDefinition>> _emptyViewDefinitionHandler;

      public RdbmsStorageEntityDefinitionVisitor (
          Action<TableDefinition, Action<IRdbmsStorageEntityDefinition>> tableDefinitionHandler,
          Action<FilterViewDefinition, Action<IRdbmsStorageEntityDefinition>> filterViewDefinitionHandler,
          Action<UnionViewDefinition, Action<IRdbmsStorageEntityDefinition>> unionViewDefinitionHandler,
          Action<EmptyViewDefinition, Action<IRdbmsStorageEntityDefinition>> emptyViewDefinitionHandler)
      {
        ArgumentNullException.ThrowIfNull(tableDefinitionHandler);
        ArgumentNullException.ThrowIfNull(filterViewDefinitionHandler);
        ArgumentNullException.ThrowIfNull(unionViewDefinitionHandler);
        ArgumentNullException.ThrowIfNull(emptyViewDefinitionHandler);

        _tableDefinitionHandler = tableDefinitionHandler;
        _filterViewDefinitionHandler = filterViewDefinitionHandler;
        _unionViewDefinitionHandler = unionViewDefinitionHandler;
        _emptyViewDefinitionHandler = emptyViewDefinitionHandler;
      }

      public void VisitTableDefinition (TableDefinition tableDefinition)
      {
        ArgumentNullException.ThrowIfNull(tableDefinition);

        _tableDefinitionHandler(tableDefinition, ContinueWithNextEntity);
      }

      public void VisitUnionViewDefinition (UnionViewDefinition unionViewDefinition)
      {
        ArgumentNullException.ThrowIfNull(unionViewDefinition);

        _unionViewDefinitionHandler(unionViewDefinition, ContinueWithNextEntity);
      }

      public void VisitFilterViewDefinition (FilterViewDefinition filterViewDefinition)
      {
        ArgumentNullException.ThrowIfNull(filterViewDefinition);

        _filterViewDefinitionHandler(filterViewDefinition, ContinueWithNextEntity);
      }

      public void VisitEmptyViewDefinition (EmptyViewDefinition emptyViewDefinition)
      {
        ArgumentNullException.ThrowIfNull(emptyViewDefinition);

        _emptyViewDefinitionHandler(emptyViewDefinition, ContinueWithNextEntity);
      }

      private void ContinueWithNextEntity (IRdbmsStorageEntityDefinition entityDefinition)
      {
        entityDefinition.Accept(this);
      }
    }
  }
}
