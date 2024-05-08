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
  /// <see cref="SynonymScriptBuilder"/> contains database-independent code to generate scripts to create and drop synonyms in a relational database.
  /// </summary>
  public class SynonymScriptBuilder : IScriptBuilder
  {
    private readonly ISynonymScriptElementFactory<TableDefinition> _tableViewElementFactory;
    private readonly ISynonymScriptElementFactory<UnionViewDefinition> _unionViewElementFactory;
    private readonly ISynonymScriptElementFactory<FilterViewDefinition> _filterViewElementFactory;
    private readonly ISynonymScriptElementFactory<EmptyViewDefinition> _emptyViewElementFactory;

    private readonly ScriptElementCollection _createScriptElements;
    private readonly ScriptElementCollection _dropScriptElements;

    public SynonymScriptBuilder (
        ISynonymScriptElementFactory<TableDefinition> tableViewElementFactory,
        ISynonymScriptElementFactory<UnionViewDefinition> unionViewElementFactory,
        ISynonymScriptElementFactory<FilterViewDefinition> filterViewElementFactory,
        ISynonymScriptElementFactory<EmptyViewDefinition> emptyViewElementFactory,
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
      _createScriptElements.AddElement(commentFactory.GetCommentElement("Create synonyms for tables that were created above"));
      _dropScriptElements = new ScriptElementCollection();
      _dropScriptElements.AddElement(commentFactory.GetCommentElement("Drop all synonyms"));
    }

    public ISynonymScriptElementFactory<TableDefinition> TableViewElementFactory
    {
      get { return _tableViewElementFactory; }
    }

    public ISynonymScriptElementFactory<UnionViewDefinition> UnionViewElementFactory
    {
      get { return _unionViewElementFactory; }
    }

    public ISynonymScriptElementFactory<FilterViewDefinition> FilterViewElementFactory
    {
      get { return _filterViewElementFactory; }
    }

    public ISynonymScriptElementFactory<EmptyViewDefinition> EmptyViewElementFactory
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
      foreach (var synonym in tableDefinition.Synonyms)
      {
        AddElements(
            _tableViewElementFactory.GetCreateElement(tableDefinition, synonym),
            _tableViewElementFactory.GetDropElement(tableDefinition, synonym));
      }
    }

    private void AddUnionViewDefinition (UnionViewDefinition unionViewDefinition)
    {
      foreach (var synonym in unionViewDefinition.Synonyms)
      {
        AddElements(
            _unionViewElementFactory.GetCreateElement(unionViewDefinition, synonym),
            _unionViewElementFactory.GetDropElement(unionViewDefinition, synonym));
      }
    }

    private void AddFilterViewDefinition (FilterViewDefinition filterViewDefinition)
    {
      foreach (var synonym in filterViewDefinition.Synonyms)
      {
        AddElements(
            _filterViewElementFactory.GetCreateElement(filterViewDefinition, synonym),
            _filterViewElementFactory.GetDropElement(filterViewDefinition, synonym));
      }
    }

    private void AddEmptyViewDefinition (EmptyViewDefinition emptyViewDefinition)
    {
      foreach (var synonym in emptyViewDefinition.Synonyms)
      {
        AddElements(
            _emptyViewElementFactory.GetCreateElement(emptyViewDefinition, synonym),
            _emptyViewElementFactory.GetDropElement(emptyViewDefinition, synonym));
      }
    }

    private void AddElements (IScriptElement createElement, IScriptElement dropElement)
    {
      _createScriptElements.AddElement(createElement);
      _dropScriptElements.AddElement(dropElement);
    }
  }
}
