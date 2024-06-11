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
using System.Collections;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Parameters;

/// <summary>
/// SQL-Server-based implementation that can create <see cref="IDataParameterDefinition"/> instances that uses table-valued parameters to handle parameter values that are
/// collections (<see cref="ICollection"/>, <see cref="ICollection{T}"/>, and <see cref="IReadOnlyCollection{T}"/>).
/// </summary>
public class SqlTableValuedDataParameterDefinitionFactory : IDataParameterDefinitionFactory
{
  public IQueryParameterRecordDefinitionFinder QueryParameterRecordDefinitionFinder { get; }
  public IDataParameterDefinitionFactory NextDataParameterDefinitionFactory { get; }

  public SqlTableValuedDataParameterDefinitionFactory (
      IQueryParameterRecordDefinitionFinder queryParameterRecordDefinitionFinder,
      IDataParameterDefinitionFactory nextDataParameterDefinitionFactory)
  {
    ArgumentUtility.CheckNotNull(nameof(queryParameterRecordDefinitionFinder), queryParameterRecordDefinitionFinder);
    ArgumentUtility.CheckNotNull(nameof(nextDataParameterDefinitionFactory), nextDataParameterDefinitionFactory);

    NextDataParameterDefinitionFactory = nextDataParameterDefinitionFactory;
    QueryParameterRecordDefinitionFinder = queryParameterRecordDefinitionFinder;
  }

  /// <summary>
  /// Creates an <see cref="IDataParameterDefinition"/> from a <paramref name="queryParameter"/> with a collection <see cref="QueryParameter.Value"/>; for other values,
  /// responsibility is passed to the <see cref="NextDataParameterDefinitionFactory"/>.
  /// </summary>
  public IDataParameterDefinition CreateDataParameterDefinition (QueryParameter queryParameter, IQuery query)
  {
    ArgumentUtility.CheckNotNull(nameof(queryParameter), queryParameter);
    ArgumentUtility.CheckNotNull(nameof(query), query);

    var recordDefinition = QueryParameterRecordDefinitionFinder.GetRecordDefinition(queryParameter, query);
    if (recordDefinition != null)
      return new SqlTableValuedDataParameterDefinition(recordDefinition);

    return NextDataParameterDefinitionFactory.CreateDataParameterDefinition(queryParameter, query);
  }


}
