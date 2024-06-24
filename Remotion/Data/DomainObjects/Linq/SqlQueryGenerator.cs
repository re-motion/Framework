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
using System.Linq.Expressions;
using Remotion.Linq;
using Remotion.Linq.SqlBackend.MappingResolution;
using Remotion.Linq.SqlBackend.SqlGeneration;
using Remotion.Linq.SqlBackend.SqlPreparation;
using Remotion.Linq.SqlBackend.SqlStatementModel;
using Remotion.Linq.SqlBackend.SqlStatementModel.Resolved;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Generates a SQL query for a LINQ <see cref="QueryModel"/>.
  /// </summary>
  public class SqlQueryGenerator : ISqlQueryGenerator
  {
    private readonly ISqlPreparationStage _preparationStage;
    private readonly IMappingResolutionStage _resolutionStage;
    private readonly ISqlGenerationStage _generationStage;

    public SqlQueryGenerator (ISqlPreparationStage preparationStage, IMappingResolutionStage resolutionStage, ISqlGenerationStage generationStage)
    {
      ArgumentUtility.CheckNotNull("preparationStage", preparationStage);
      ArgumentUtility.CheckNotNull("resolutionStage", resolutionStage);
      ArgumentUtility.CheckNotNull("generationStage", generationStage);

      _preparationStage = preparationStage;
      _resolutionStage = resolutionStage;
      _generationStage = generationStage;
    }

    public ISqlPreparationStage PreparationStage
    {
      get { return _preparationStage; }
    }

    public IMappingResolutionStage ResolutionStage
    {
      get { return _resolutionStage; }
    }

    public ISqlGenerationStage GenerationStage
    {
      get { return _generationStage; }
    }

    /// <summary>
    /// Creates a SQL query from a given <see cref="QueryModel"/>.
    /// </summary>
    /// <param name="queryModel">
    ///   The <see cref="QueryModel"/> a sql query is generated for. The query must not contain any eager fetch result operators.
    /// </param>
    /// <returns>A <see cref="SqlCommandData"/> instance containing the SQL text, parameters, and an in-memory projection for the given query model.</returns>
    public virtual SqlQueryGeneratorResult CreateSqlQuery (QueryModel queryModel)
    {
      ArgumentUtility.CheckNotNull("queryModel", queryModel);

      SqlStatement sqlStatement;
      try
      {
        sqlStatement = TransformAndResolveQueryModel(queryModel);
      }
      catch (NotSupportedException ex)
      {
        var message = string.Format("There was an error preparing or resolving query '{0}' for SQL generation. {1}", queryModel, ex.Message);
        throw new NotSupportedException(message, ex);
      }
      catch (UnmappedItemException ex)
      {
        var message = string.Format("Query '{0}' contains an unmapped item. {1}", queryModel, ex.Message);
        throw new UnmappedItemException(message, ex);
      }

      var selectedEntityTypeOrNull = GetSelectedEntityType(sqlStatement.SelectProjection);
      SqlCommandData sqlCommandData;
      try
      {
        sqlCommandData = CreateSqlCommand(sqlStatement);
      }
      catch (NotSupportedException ex)
      {
        var message = string.Format("There was an error generating SQL for the query '{0}'. {1}", queryModel, ex.Message);
        throw new NotSupportedException(message, ex);
      }

      return new SqlQueryGeneratorResult(sqlCommandData, selectedEntityTypeOrNull);
    }

    /// <summary>
    /// Transforms and resolves <see cref="QueryModel"/> to build a <see cref="SqlStatement"/> which represents an AST to generate query text.
    /// </summary>
    /// <param name="queryModel">The <see cref="QueryModel"/> which should be transformed.</param>
    /// <returns>the generated <see cref="SqlStatement"/></returns>
    protected virtual SqlStatement TransformAndResolveQueryModel (QueryModel queryModel)
    {
      var sqlStatement = _preparationStage.PrepareSqlStatement(queryModel, null);
      var mappingResolutionContext = new MappingResolutionContext();
      return _resolutionStage.ResolveSqlStatement(sqlStatement, mappingResolutionContext);
    }

    /// <summary>
    /// Creates a SQL command based on a given <see cref="SqlStatement"/>.
    /// </summary>
    /// <param name="sqlStatement">The <see cref="SqlStatement"/> a SQL query has to be generated for.</param>
    /// <returns><see cref="SqlCommandData"/> which represents the sql query.</returns>
    protected virtual SqlCommandData CreateSqlCommand (SqlStatement sqlStatement)
    {
      var commandBuilder = new TableValuedParameterSqlCommandBuilder();
      _generationStage.GenerateTextForOuterSqlStatement(commandBuilder, sqlStatement);
      return commandBuilder.GetCommand();
    }

    private Type? GetSelectedEntityType (Expression selectProjection)
    {
      var expression = selectProjection;
      while (expression is UnaryExpression)
        expression = ((UnaryExpression)expression).Operand;
      if (expression is SqlEntityExpression)
        return expression.Type;

      return null;
    }
  }
}
