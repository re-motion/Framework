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
  /// Creates CREATE TYPE and DROP TYPE scripts for SQL Server table types.
  /// </summary>
  public class TableTypeScriptBuilder : IScriptBuilder
  {
    public IStructuredTypeScriptElementFactory ElementFactory { get; }

    private readonly ScriptElementCollection _createScriptElements;
    private readonly ScriptElementCollection _dropScriptElements;

    public TableTypeScriptBuilder (IStructuredTypeScriptElementFactory elementFactory, ICommentScriptElementFactory commentFactory)
    {
      ArgumentUtility.CheckNotNull(nameof(elementFactory), elementFactory);

      ElementFactory = elementFactory;
      _createScriptElements = new ScriptElementCollection();
      _createScriptElements.AddElement(commentFactory.GetCommentElement("Create all structured types"));
      _dropScriptElements = new ScriptElementCollection();
      _dropScriptElements.AddElement(commentFactory.GetCommentElement("Drop all structured types"));
    }

    public void AddEntityDefinition (IRdbmsStorageEntityDefinition entityDefinition)
    {
      // table types have nothing to do with entities
    }

    public void AddStructuredTypeDefinition (IRdbmsStructuredTypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull(nameof(typeDefinition), typeDefinition);

      InlineRdbmsStructuredTypeDefinitionVisitor.Visit(typeDefinition, (type, _) => AddTableTypeDefinition(type));
    }

    public IScriptElement GetCreateScript ()
    {
      return _createScriptElements;
    }

    public IScriptElement GetDropScript ()
    {
      return _dropScriptElements;
    }

    private void AddTableTypeDefinition (TableTypeDefinition tableTypeDefinition)
    {
      _createScriptElements.AddElement(ElementFactory.GetCreateElement(tableTypeDefinition));
      _dropScriptElements.AddElement(ElementFactory.GetDropElement(tableTypeDefinition));
    }
  }
}
