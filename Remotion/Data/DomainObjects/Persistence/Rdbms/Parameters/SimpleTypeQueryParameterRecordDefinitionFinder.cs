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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;

public class SimpleTypeQueryParameterRecordDefinitionFinder : IQueryParameterRecordDefinitionFinder
{
  public IRdbmsStructuredTypeDefinitionFinder StructuredTypeDefinitionFinder { get; }

  public SimpleTypeQueryParameterRecordDefinitionFinder (IRdbmsStructuredTypeDefinitionFinder structuredTypeDefinitionFinder)
  {
    ArgumentUtility.CheckNotNull(nameof(structuredTypeDefinitionFinder), structuredTypeDefinitionFinder);
    StructuredTypeDefinitionFinder = structuredTypeDefinitionFinder;
  }

  public RecordDefinition? GetRecordDefinition (QueryParameter queryParameter, IQuery query)
  {
    ArgumentUtility.CheckNotNull(nameof(queryParameter), queryParameter);
    ArgumentUtility.CheckNotNull(nameof(query), query);

    IRdbmsStructuredTypeDefinition? structuredTypeDefinition;
    try
    {
      structuredTypeDefinition = StructuredTypeDefinitionFinder.GetTypeDefinition(queryParameter, query);
      if (structuredTypeDefinition == null)
        return null;
    }
    catch (NotSupportedException ex)
    {
      var parameterType = queryParameter.Value != null ? $"'{queryParameter.Value?.GetType()}'" : "(unknown)";
      throw new NotSupportedException(
          $"The parameter value's type {parameterType} cannot be mapped by {typeof(SimpleTypeQueryParameterRecordDefinitionFinder)}. "
          + $"Either use a collection of scalar values, or use an implementation of {typeof(IQueryParameterRecordDefinitionFinder)} that supports {parameterType}.",
          ex);
    }

    if (structuredTypeDefinition.Properties.Count == 1 && structuredTypeDefinition.Properties.Single() is SimpleStoragePropertyDefinition simpleStoragePropertyDefinition)
    {
      return new RecordDefinition(
          simpleStoragePropertyDefinition.ColumnDefinition.StorageTypeInfo.StorageDbType.ToString(),
          structuredTypeDefinition,
          new[] { RecordPropertyDefinition.ScalarAsValue(simpleStoragePropertyDefinition) });
    }

    var schemaName = structuredTypeDefinition.TypeName.SchemaName;
    var typeName = structuredTypeDefinition.TypeName.EntityName;
    var tableTypeName = string.IsNullOrEmpty(schemaName) ? typeName : $"{schemaName}.{typeName}";
    throw new NotSupportedException(
        $"The structured type '{tableTypeName}' cannot be mapped by {typeof(SimpleTypeQueryParameterRecordDefinitionFinder)}. "
        + $"Either use a collection of scalar values, or use an implementation of {typeof(IQueryParameterRecordDefinitionFinder)} that supports '{tableTypeName}'.");
  }
}
