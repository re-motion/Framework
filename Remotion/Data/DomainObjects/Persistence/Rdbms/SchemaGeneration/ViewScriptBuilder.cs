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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// <see cref="ViewScriptBuilder"/> contains database-independent code to generate scripts to create and drop views in a relational database.
  /// </summary>
  public class ViewScriptBuilder : IScriptBuilder
  {
    private readonly IViewScriptElementFactory<TableDefinition> _tableViewElementFactory;
    private readonly IViewScriptElementFactory<UnionViewDefinition> _unionViewElementFactory;
    private readonly IViewScriptElementFactory<FilterViewDefinition> _filterViewElementFactory;
    private readonly IViewScriptElementFactory<EmptyViewDefinition> _emptyViewElementFactory;
    private readonly ScriptElementCollection _createScriptElements;
    private readonly ScriptElementCollection _dropScriptElements;

    public ViewScriptBuilder (
        IViewScriptElementFactory<TableDefinition> tableViewElementFactory,
        IViewScriptElementFactory<UnionViewDefinition> unionViewElementFactory,
        IViewScriptElementFactory<FilterViewDefinition> filterViewElementFactory,
        IViewScriptElementFactory<EmptyViewDefinition> emptyViewElementFactory,
        ICommentScriptElementFactory commentFactory)
    {
      ArgumentUtility.CheckNotNull("tableViewElementFactory", tableViewElementFactory);
      ArgumentUtility.CheckNotNull("unionViewElementFactory", unionViewElementFactory);
      ArgumentUtility.CheckNotNull("filterViewElementFactory", filterViewElementFactory);
      ArgumentUtility.CheckNotNull("emptyViewElementFactory", emptyViewElementFactory);
      ArgumentUtility.CheckNotNull("commentFactory", commentFactory);

      _tableViewElementFactory = tableViewElementFactory;
      _unionViewElementFactory = unionViewElementFactory;
      _filterViewElementFactory = filterViewElementFactory;
      _emptyViewElementFactory = emptyViewElementFactory;

      _createScriptElements = new ScriptElementCollection();
      _createScriptElements.AddElement(commentFactory.GetCommentElement("Create a view for every class"));
      _dropScriptElements = new ScriptElementCollection();
      _dropScriptElements.AddElement(commentFactory.GetCommentElement("Drop all views"));
    }

    public IViewScriptElementFactory<TableDefinition> TableViewElementFactory
    {
      get { return _tableViewElementFactory; }
    }

    public IViewScriptElementFactory<UnionViewDefinition> UnionViewElementFactory
    {
      get { return _unionViewElementFactory; }
    }

    public IViewScriptElementFactory<FilterViewDefinition> FilterViewElementFactory
    {
      get { return _filterViewElementFactory; }
    }

    public IViewScriptElementFactory<EmptyViewDefinition> EmptyViewElementFactory
    {
      get { return _emptyViewElementFactory; }
    }

    public void AddEntityDefinition (IRdbmsStorageEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull("entityDefinition", entityDefinition);

      InlineRdbmsStorageEntityDefinitionVisitor.Visit(
          entityDefinition,
          (table, continuation) => AddTableDefinition(table),
          (filterView, continuation) => AddFilterViewDefinition(filterView),
          (unionView, contination) => AddUnionViewDefinition(unionView),
          (emptyView, continuation) => AddEmptyViewDefinition(emptyView));
    }

    public void AddStructuredTypeDefinition (IRdbmsStructuredTypeDefinition typeDefinition)
    {
      // views have nothing to do with structured types
    }

    public IScriptElement GetCreateScript ()
    {
      return _createScriptElements;
    }

    public IScriptElement GetDropScript ()
    {
      return _dropScriptElements;
    }

    private void AddTableDefinition (TableDefinition tableDefinition)
    {
      AddElements(_tableViewElementFactory.GetCreateElement(tableDefinition), _tableViewElementFactory.GetDropElement(tableDefinition));
    }

    private void AddUnionViewDefinition (UnionViewDefinition unionViewDefinition)
    {
      AddElements(_unionViewElementFactory.GetCreateElement(unionViewDefinition), _unionViewElementFactory.GetDropElement(unionViewDefinition));
    }

    private void AddFilterViewDefinition (FilterViewDefinition filterViewDefinition)
    {
      AddElements(_filterViewElementFactory.GetCreateElement(filterViewDefinition), _filterViewElementFactory.GetDropElement(filterViewDefinition));
    }

    private void AddEmptyViewDefinition (EmptyViewDefinition emptyViewDefinition)
    {
      AddElements(_emptyViewElementFactory.GetCreateElement(emptyViewDefinition), _emptyViewElementFactory.GetDropElement(emptyViewDefinition));
    }

    private void AddElements (IScriptElement createElement, IScriptElement dropElement)
    {
      _createScriptElements.AddElement(createElement);
      _dropScriptElements.AddElement(dropElement);
    }
  }
}
