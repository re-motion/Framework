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
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Model;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// The <see cref="SqlIndexScriptElementFactory"/> is responsible to create script-elements for indexes in a sql-server database.
  /// </summary>
  public class SqlIndexScriptElementFactory : SqlElementFactoryBase, IIndexScriptElementFactory
  {
    private readonly ISqlIndexDefinitionScriptElementFactory<SqlIndexDefinition> _indexDefinitionElementFactory;
    private readonly ISqlIndexDefinitionScriptElementFactory<SqlPrimaryXmlIndexDefinition> _primaryIndexDefinitionElementFactory;
    private readonly ISqlIndexDefinitionScriptElementFactory<SqlSecondaryXmlIndexDefinition> _secondaryIndexDefinitionElementFactory;
    private IScriptElement _createScriptElement;
    private IScriptElement _dropScriptElement;

    public SqlIndexScriptElementFactory (
        ISqlIndexDefinitionScriptElementFactory<SqlIndexDefinition> indexDefinitionElmementFactory,
        ISqlIndexDefinitionScriptElementFactory<SqlPrimaryXmlIndexDefinition> primaryIndexDefinitionElementFactory,
        ISqlIndexDefinitionScriptElementFactory<SqlSecondaryXmlIndexDefinition> secondaryIndexDefinitionElementFactory)
    {
      ArgumentUtility.CheckNotNull ("indexDefinitionElmementFactory", indexDefinitionElmementFactory);
      ArgumentUtility.CheckNotNull ("primaryIndexDefinitionElementFactory", primaryIndexDefinitionElementFactory);
      ArgumentUtility.CheckNotNull ("secondaryIndexDefinitionElementFactory", secondaryIndexDefinitionElementFactory);

      _indexDefinitionElementFactory = indexDefinitionElmementFactory;
      _primaryIndexDefinitionElementFactory = primaryIndexDefinitionElementFactory;
      _secondaryIndexDefinitionElementFactory = secondaryIndexDefinitionElementFactory;
    }

    private class IndexDefinitionVisitor : ISqlIndexDefinitionVisitor
    {
      private readonly SqlIndexScriptElementFactory _elementFactory;
      private readonly EntityNameDefinition _ownerName;

      public IndexDefinitionVisitor (SqlIndexScriptElementFactory elementFactory, EntityNameDefinition ownerName)
      {
        _elementFactory = elementFactory;
        _ownerName = ownerName;
      }

      void ISqlIndexDefinitionVisitor.VisitIndexDefinition (SqlIndexDefinition sqlIndexDefinition)
      {
        _elementFactory.AddIndexDefinition (sqlIndexDefinition, _ownerName);
      }

      void ISqlIndexDefinitionVisitor.VisitPrimaryXmlIndexDefinition (SqlPrimaryXmlIndexDefinition primaryXmlIndexDefinition)
      {
        _elementFactory.AddPrimaryXmlIndexDefinition (primaryXmlIndexDefinition, _ownerName);
      }

      void ISqlIndexDefinitionVisitor.VisitSecondaryXmlIndexDefinition (SqlSecondaryXmlIndexDefinition secondaryXmlIndexDefinition)
      {
        _elementFactory.AddSecondaryXmlIndexDefinition (secondaryXmlIndexDefinition, _ownerName);
      }
    }

    public IScriptElement GetCreateElement (IIndexDefinition indexDefinition, EntityNameDefinition ownerName)
    {
      ArgumentUtility.CheckNotNull ("indexDefinition", indexDefinition);

      var visitor = new IndexDefinitionVisitor (this, ownerName);
      indexDefinition.Accept (visitor);
      return _createScriptElement;
    }

    public IScriptElement GetDropElement (IIndexDefinition indexDefinition, EntityNameDefinition ownerName)
    {
      ArgumentUtility.CheckNotNull ("indexDefinition", indexDefinition);

      var visitor = new IndexDefinitionVisitor (this, ownerName);
      indexDefinition.Accept (visitor);
      return _dropScriptElement;
    }

    private void AddIndexDefinition (SqlIndexDefinition indexDefinition, EntityNameDefinition ownerName)
    {
      _createScriptElement = _indexDefinitionElementFactory.GetCreateElement (indexDefinition, ownerName);
      _dropScriptElement = _indexDefinitionElementFactory.GetDropElement (indexDefinition, ownerName);
    }

    private void AddPrimaryXmlIndexDefinition (SqlPrimaryXmlIndexDefinition indexDefinition, EntityNameDefinition ownerName)
    {
      _createScriptElement = _primaryIndexDefinitionElementFactory.GetCreateElement (indexDefinition, ownerName);
      _dropScriptElement = _primaryIndexDefinitionElementFactory.GetDropElement (indexDefinition, ownerName);
    }

    private void AddSecondaryXmlIndexDefinition (SqlSecondaryXmlIndexDefinition indexDefinition, EntityNameDefinition ownerName)
    {
      _createScriptElement = _secondaryIndexDefinitionElementFactory.GetCreateElement (indexDefinition, ownerName);
      _dropScriptElement = _secondaryIndexDefinitionElementFactory.GetDropElement (indexDefinition, ownerName);
    }
  }
}