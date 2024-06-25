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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.EagerFetching
{
  /// <summary>
  /// Holds the eager fetch queries for a given <see cref="IQuery"/> instance. See <see cref="IQuery.EagerFetchQueries"/> for more information about
  /// eager fetch queries.
  /// </summary>
  public class EagerFetchQueryCollection : IEnumerable<KeyValuePair<IRelationEndPointDefinition, IQuery>>
  {
    private readonly Dictionary<IRelationEndPointDefinition, IQuery> _fetchQueries = new Dictionary<IRelationEndPointDefinition, IQuery>();

    public int Count
    {
      get { return _fetchQueries.Count; }
    }

    public IEnumerator<KeyValuePair<IRelationEndPointDefinition, IQuery>> GetEnumerator ()
    {
      return _fetchQueries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public void Add (IRelationEndPointDefinition relationEndPointDefinition, IQuery fetchQuery)
    {
      ArgumentUtility.CheckNotNull("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull("fetchQuery", fetchQuery);

      if (_fetchQueries.ContainsKey(relationEndPointDefinition))
      {
        var message = string.Format("There is already an eager fetch query for relation end point '{0}'.", relationEndPointDefinition.PropertyName);
        throw new InvalidOperationException(message);
      }

      _fetchQueries.Add(relationEndPointDefinition, fetchQuery);
    }
  }
}
