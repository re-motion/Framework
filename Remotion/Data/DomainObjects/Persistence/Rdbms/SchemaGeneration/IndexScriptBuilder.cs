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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// <see cref="IndexScriptBuilder"/> contains database-independent code to generate scripts to create and drop indexes in a relational database.
  /// </summary>
  public class IndexScriptBuilder : IScriptBuilder
  {
    private readonly IIndexScriptElementFactory _indexScriptElementFactory;
    private readonly ScriptElementCollection _createScriptElements;
    private readonly ScriptElementCollection _dropScriptElements;

    public IndexScriptBuilder (IIndexScriptElementFactory indexScriptElementFactory, ICommentScriptElementFactory commentFactory)
    {
      ArgumentUtility.CheckNotNull("indexScriptElementFactory", indexScriptElementFactory);
      ArgumentUtility.CheckNotNull("commentFactory", commentFactory);

      _indexScriptElementFactory = indexScriptElementFactory;
      _createScriptElements = new ScriptElementCollection();
      _createScriptElements.AddElement(commentFactory.GetCommentElement("Create indexes for tables that were created above"));
      _dropScriptElements = new ScriptElementCollection();
      _dropScriptElements.AddElement(commentFactory.GetCommentElement("Drop all indexes"));
    }

    public IIndexScriptElementFactory IndexScriptElementFactory
    {
      get { return _indexScriptElementFactory; }
    }

    public void AddEntityDefinition (IRdbmsStorageEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull("entityDefinition", entityDefinition);

      InlineRdbmsStorageEntityDefinitionVisitor.Visit(
          entityDefinition,
          (table, continuation) => AddIndexes(table.Indexes, table.TableName),
          (filterView, continuation) => AddIndexes(filterView.Indexes, filterView.ViewName),
          (unionView, contination) => AddIndexes(unionView.Indexes, unionView.ViewName),
          (emptyView, continuation) => { });
    }

    public void AddStructuredTypeDefinition (IRdbmsStructuredTypeDefinition typeDefinition)
    {
      //TODO RM-9197
    }

    public IScriptElement GetCreateScript ()
    {
      return _createScriptElements;
    }

    public IScriptElement GetDropScript ()
    {
      return _dropScriptElements;
    }

    private void AddIndexes (IEnumerable<IIndexDefinition> indexes, EntityNameDefinition ownerName)
    {
      foreach (var index in indexes)
      {
        _createScriptElements.AddElement(_indexScriptElementFactory.GetCreateElement(index, ownerName));
        _dropScriptElements.AddElement(_indexScriptElementFactory.GetDropElement(index, ownerName));
      }
    }
  }
}
