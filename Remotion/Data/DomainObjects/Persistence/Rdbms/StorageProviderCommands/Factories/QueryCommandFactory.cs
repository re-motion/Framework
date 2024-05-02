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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DataReaders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model.Building;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands.Factories
{
  /// <summary>
  /// The <see cref="QueryCommandFactory"/> is responsible for creating query commands for a relational database.
  /// </summary>
  public class QueryCommandFactory
  {
    private readonly IObjectReaderFactory _objectReaderFactory;
    private readonly IDbCommandBuilderFactory _dbCommandBuilderFactory;
    private readonly IDataStoragePropertyDefinitionFactory _dataStoragePropertyDefinitionFactory;
    private readonly IDataParameterDefinitionFactory _dataParameterDefinitionFactory;

    public QueryCommandFactory (
        IObjectReaderFactory objectReaderFactory,
        IDbCommandBuilderFactory dbCommandBuilderFactory,
        IDataStoragePropertyDefinitionFactory dataStoragePropertyDefinitionFactory,
        IDataParameterDefinitionFactory dataParameterDefinitionFactory)
    {
      ArgumentUtility.CheckNotNull("objectReaderFactory", objectReaderFactory);
      ArgumentUtility.CheckNotNull("dbCommandBuilderFactory", dbCommandBuilderFactory);
      ArgumentUtility.CheckNotNull("dataStoragePropertyDefinitionFactory", dataStoragePropertyDefinitionFactory);
      ArgumentUtility.CheckNotNull("dataParameterDefinitionFactory", dataParameterDefinitionFactory);

      _objectReaderFactory = objectReaderFactory;
      _dbCommandBuilderFactory = dbCommandBuilderFactory;
      _dataStoragePropertyDefinitionFactory = dataStoragePropertyDefinitionFactory;
      _dataParameterDefinitionFactory = dataParameterDefinitionFactory;
    }

    public IObjectReaderFactory ObjectReaderFactory
    {
      get { return _objectReaderFactory; }
    }

    public IDbCommandBuilderFactory DbCommandBuilderFactory
    {
      get { return _dbCommandBuilderFactory; }
    }

    public IDataStoragePropertyDefinitionFactory DataStoragePropertyDefinitionFactory
    {
      get { return _dataStoragePropertyDefinitionFactory; }
    }

    public virtual IRdbmsProviderCommandWithReadOnlySupport<IEnumerable<DataContainer?>> CreateForDataContainerQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      var dbCommandBuilder = CreateDbCommandBuilder(query);
      var dataContainerReader = _objectReaderFactory.CreateDataContainerReader();
      return new MultiObjectLoadCommand<DataContainer?>(new[] { Tuple.Create(dbCommandBuilder, dataContainerReader) });
    }

    public virtual IRdbmsProviderCommandWithReadOnlySupport<IEnumerable<IQueryResultRow>> CreateForCustomQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      var dbCommandBuilder = CreateDbCommandBuilder(query);
      var resultRowReader = _objectReaderFactory.CreateResultRowReader();

      return new MultiObjectLoadCommand<IQueryResultRow>(new[] { Tuple.Create(dbCommandBuilder, resultRowReader) });
    }

    public virtual IRdbmsProviderCommandWithReadOnlySupport<object?> CreateForScalarQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull("query", query);

      var dbCommandBuilder = CreateDbCommandBuilder(query);
      return new ScalarValueLoadCommand(dbCommandBuilder);
    }

    protected virtual QueryParameterWithDataParameterDefinition GetQueryParameterWithDataParameterDefinition (QueryParameter parameter)
    {
      ArgumentUtility.CheckNotNull("parameter", parameter);

      var dataParameterDefinition = _dataParameterDefinitionFactory.CreateDataParameterDefinition(parameter);
      return new QueryParameterWithDataParameterDefinition(parameter, dataParameterDefinition);
    }

    private IDbCommandBuilder CreateDbCommandBuilder (IQuery query)
    {
      // Use ToList to trigger error detection here
      var queryParametersWithType = query.Parameters
          .Cast<QueryParameter>()
          .Select(GetQueryParameterWithDataParameterDefinition)
          .ToList();

      return _dbCommandBuilderFactory.CreateForQuery(query.Statement, queryParametersWithType);
    }
  }
}
