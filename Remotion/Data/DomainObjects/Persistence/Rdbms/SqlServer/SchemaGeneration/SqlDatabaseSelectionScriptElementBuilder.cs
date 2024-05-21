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
using System.Data.SqlClient;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration.ScriptElements;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration
{
  /// <summary>
  /// <see cref="SqlDatabaseSelectionScriptElementBuilder"/> ist responsible to generate a sql-statement to change the database context to the 
  /// specified  database for a sql-server database.
  /// </summary>
  public class SqlDatabaseSelectionScriptElementBuilder : IScriptBuilder
  {
    private readonly string _connectionString;
    private readonly IScriptBuilder _innerScriptBuilder;

    public SqlDatabaseSelectionScriptElementBuilder (IScriptBuilder innerScriptBuilder, string connectionString)
    {
      ArgumentUtility.CheckNotNull("innerScriptBuilder", innerScriptBuilder);
      ArgumentUtility.CheckNotNull("connectionString", connectionString);

      _innerScriptBuilder = innerScriptBuilder;
      _connectionString = connectionString;
    }

    public IScriptBuilder InnerScriptBuilder
    {
      get { return _innerScriptBuilder; }
    }

    public void AddEntityDefinition (IRdbmsStorageEntityDefinition entityDefinition)
    {
      ArgumentUtility.CheckNotNull("entityDefinition", entityDefinition);

      _innerScriptBuilder.AddEntityDefinition(entityDefinition);
    }

    public void AddStructuredTypeDefinition (IRdbmsStructuredTypeDefinition typeDefinition)
    {
      ArgumentUtility.CheckNotNull(nameof(typeDefinition), typeDefinition);

      _innerScriptBuilder.AddStructuredTypeDefinition(typeDefinition);
    }

    public IScriptElement GetCreateScript ()
    {
      var createScriptElements = new ScriptElementCollection();
      createScriptElements.AddElement(new ScriptStatement("USE " + GetDatabaseName()));
      createScriptElements.AddElement(_innerScriptBuilder.GetCreateScript());
      return createScriptElements;
    }

    public IScriptElement GetDropScript ()
    {
      var dropScriptElements = new ScriptElementCollection();
      dropScriptElements.AddElement(new ScriptStatement("USE " + GetDatabaseName()));
      dropScriptElements.AddElement(_innerScriptBuilder.GetDropScript());
      return dropScriptElements;
    }

    private string GetDatabaseName ()
    {
      SqlConnectionStringBuilder connectionStringBuilder;
      try
      {
        connectionStringBuilder = new SqlConnectionStringBuilder(_connectionString);
      }
      catch (ArgumentException ex)
      {
         throw new InvalidOperationException(string.Format("Format of connection string '{0}' is invalid.", _connectionString), ex);
      }

      var initialCatalog = connectionStringBuilder.InitialCatalog;
      if (string.IsNullOrEmpty(initialCatalog))
      {
        throw new InvalidOperationException(
            string.Format("No database name could be found in the given connection string '{0}'.", _connectionString));
      }

      return initialCatalog;
    }
  }
}
