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
using System.Linq;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration
{
  /// <summary>
  /// <see cref="ForeignKeyConstraintScriptBuilder"/> contains database-independent code to generate scripts to create and drop foreign constraints in 
  /// a relational database.
  /// </summary>
  public class ForeignKeyConstraintScriptBuilder : IScriptBuilder
  {
    private readonly IForeignKeyConstraintScriptElementFactory _foreignKeyConstraintElementFactory;
    private readonly ScriptElementCollection _createScriptElements;
    private readonly ScriptElementCollection _dropScriptElements;
    private readonly ICommentScriptElementFactory _commentFactory;

    public ForeignKeyConstraintScriptBuilder (
        IForeignKeyConstraintScriptElementFactory foreignKeyConstraintElementFactory, ICommentScriptElementFactory commentFactory)
    {
      ArgumentUtility.CheckNotNull("foreignKeyConstraintElementFactory", foreignKeyConstraintElementFactory);
      ArgumentUtility.CheckNotNull("commentFactory", commentFactory);

      _foreignKeyConstraintElementFactory = foreignKeyConstraintElementFactory;
      _commentFactory = commentFactory;
      _createScriptElements = new ScriptElementCollection();
      _createScriptElements.AddElement(_commentFactory.GetCommentElement("Create foreign key constraints for tables that were created above"));
      _dropScriptElements = new ScriptElementCollection();
      _dropScriptElements.AddElement(_commentFactory.GetCommentElement("Drop foreign keys of all tables"));
    }

    public IForeignKeyConstraintScriptElementFactory ForeignKeyConstraintElementFactory
    {
      get { return _foreignKeyConstraintElementFactory; }
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
      var foreignKeyConstraints = tableDefinition.Constraints.OfType<ForeignKeyConstraintDefinition>();
      foreach (var foreignKeyConstraint in foreignKeyConstraints)
        AddForeignKeyConstraintDefinition(foreignKeyConstraint, tableDefinition.TableName);
    }

    private void AddForeignKeyConstraintDefinition (ForeignKeyConstraintDefinition foreignKeyConstraint, EntityNameDefinition tableName)
    {
      _createScriptElements.AddElement(_foreignKeyConstraintElementFactory.GetCreateElement(foreignKeyConstraint, tableName));
      _dropScriptElements.AddElement(_foreignKeyConstraintElementFactory.GetDropElement(foreignKeyConstraint, tableName));
    }
  }
}
