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
  /// <see cref="TableScriptBuilder"/> contains database-independent code to generate scripts to create and drop tables in a relational database.
  /// </summary>
  public class TableScriptBuilder : IScriptBuilder
  {
    private readonly ITableScriptElementFactory _elementFactory;
    private readonly ScriptElementCollection _createScriptElements;
    private readonly ScriptElementCollection _dropScriptElements;

    public TableScriptBuilder (ITableScriptElementFactory elementFactory, ICommentScriptElementFactory commentFactory)
    {
      ArgumentUtility.CheckNotNull("elementFactory", elementFactory);
      ArgumentUtility.CheckNotNull("commentFactory", commentFactory);

      _elementFactory = elementFactory;
      _createScriptElements = new ScriptElementCollection();
      _createScriptElements.AddElement(commentFactory.GetCommentElement("Create all tables"));
      _dropScriptElements = new ScriptElementCollection();
      _dropScriptElements.AddElement(commentFactory.GetCommentElement("Drop all tables"));
    }

    public ITableScriptElementFactory ElementFactory
    {
      get { return _elementFactory; }
    }

    public void AddEntityDefinition (IRdbmsStorageEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull("entityDefinition", entityDefinition);

      InlineRdbmsStorageEntityDefinitionVisitor.Visit(
          entityDefinition,
          (table, continuation) => AddTableDefinition(table),
          (filterView, continuation) => { },
          (unionView, contination) => { },
          (emptyView, continuation) => { });
    }

    public void AddStructuredTypeDefinition (IRdbmsStructuredTypeDefinition typeDefinition)
    {
      // tables have nothing to do with structured types
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
      _createScriptElements.AddElement(_elementFactory.GetCreateElement(tableDefinition));
      _dropScriptElements.AddElement(_elementFactory.GetDropElement(tableDefinition));
    }
  }
}
