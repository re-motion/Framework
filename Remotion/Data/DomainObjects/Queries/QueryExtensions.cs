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
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries
{
  public static class QueryExtensions
  {
    public static Query CreateCopyFromTemplate (this IQuery template, Dictionary<object, object> parameterValues)
    {
      ArgumentUtility.CheckNotNull ("template", template);
      ArgumentUtility.CheckNotNull ("parameterValues", parameterValues);

      var query = new Query (template.CopyQueryDefinition(), template.CopyQueryParameters (parameterValues));
      template.CopyFetchQueries(parameterValues, query);
      return query;
    }

    private static QueryDefinition CopyQueryDefinition (this IQuery template)
    {
      return new QueryDefinition (template.ID, template.StorageProviderDefinition, template.Statement, template.QueryType);
    }

    private static QueryParameterCollection CopyQueryParameters (this IQuery template, Dictionary<object, object> parameterValues)
    {
      if (template.Parameters.Count != parameterValues.Count)
      {
        throw new InvalidOperationException (
            String.Format (
                "Number of supplied query parameters ({0}) does not match the number of parameters in the template query ({1}) .",
                template.Parameters.Count,
                parameterValues.Count));
      }

      var queryParameters = new QueryParameterCollection();
      foreach (QueryParameter templateParameter in template.Parameters)
        queryParameters.Add (templateParameter.CopyQueryParameter (parameterValues));

      return queryParameters;
    }

    private static QueryParameter CopyQueryParameter (this QueryParameter template, Dictionary<object, object> parameterValues)
    {
      object parameterValue;
      if (!parameterValues.TryGetValue (template.Value, out parameterValue))
      {
        throw new InvalidOperationException (
            String.Format ("Query parameter '{0}' (lookup key: '{1}') is missing.", template.Name, template.Value));
      }

      return new QueryParameter (template.Name, parameterValue, template.ParameterType);
    }

    private static void CopyFetchQueries (this IQuery template, Dictionary<object, object> parameterValues, Query target)
    {
      foreach (var fetchQuery in template.EagerFetchQueries)
        target.EagerFetchQueries.Add (fetchQuery.Key, fetchQuery.Value.CreateCopyFromTemplate (parameterValues));
    }
  }
}