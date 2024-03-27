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
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Queries.EagerFetching;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// Represents a default implementation of <see cref="IQuery"/>.
  /// </summary>
  [Serializable]
  public class Query : IQuery
  {
    // types

    // static members and constants

    // member fields

    private readonly QueryDefinition _definition;
    private readonly QueryParameterCollection _parameters;
    private readonly EagerFetchQueryCollection _eagerFetchQueries = new EagerFetchQueryCollection();

    // construction and disposing

    /// <summary>
    /// Initializes a new instance of the <see cref="Query"/> class using a <see cref="Configuration.QueryDefinition"/> and a given collection of <see cref="QueryParameter"/>s.
    /// </summary>
    /// <param name="definition">The <see cref="Configuration.QueryDefinition"/> to use for the query.</param>
    /// <param name="parameters">The <see cref="QueryParameter"/>s to use for executing the query.</param>
    /// <exception cref="System.ArgumentNullException">
    ///   <paramref name="definition"/> is <see langword="null"/>.<br /> -or- <br />
    ///   <paramref name="parameters"/> is <see langword="null"/>.
    /// </exception>
    public Query (QueryDefinition definition, QueryParameterCollection parameters)
    {
      ArgumentUtility.CheckNotNull("definition", definition);
      ArgumentUtility.CheckNotNull("parameters", parameters);

      _definition = definition;
      _parameters = parameters;
    }

    // methods and properties

    /// <summary>
    /// Gets the <see cref="Configuration.QueryDefinition"/> that is associated with the query.
    /// </summary>
    public QueryDefinition Definition
    {
      get { return _definition; }
    }

    public IReadOnlyDictionary<string, object> Metadata
    {
      get { return _definition.Metadata; }
    }

    /// <summary>
    /// Gets the <see cref="Configuration.QueryDefinition.ID"/> of the associated <see cref="Configuration.QueryDefinition"/>.
    /// </summary>
    public string ID
    {
      get { return _definition.ID; }
    }

    /// <summary>
    /// Gets the <see cref="Persistence.Configuration.StorageProviderDefinition"/> of the associated <see cref="Configuration.QueryDefinition"/>.
    /// </summary>
    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _definition.StorageProviderDefinition; }
    }

    /// <summary>
    /// Gets the <see cref="Configuration.QueryDefinition.CollectionType"/> of the associated <see cref="Configuration.QueryDefinition"/>.
    /// </summary>
    public Type? CollectionType
    {
      get { return _definition.CollectionType; }
    }

    /// <summary>
    /// Gets the <see cref="Configuration.QueryDefinition.QueryType"/> of the associated <see cref="Configuration.QueryDefinition"/>.
    /// </summary>
    public QueryType QueryType
    {
      get { return _definition.QueryType; }
    }

    /// <summary>
    /// Gets the <see cref="Configuration.QueryDefinition.Statement"/> of the associated <see cref="Configuration.QueryDefinition"/>.
    /// </summary>
    public string Statement
    {
      get { return _definition.Statement; }
    }

    /// <summary>
    /// Gets the <see cref="QueryParameter"/>s that are used to execute the <see cref="Query"/>.
    /// </summary>
    public QueryParameterCollection Parameters
    {
      get { return _parameters; }
    }

    public EagerFetchQueryCollection EagerFetchQueries
    {
      get { return _eagerFetchQueries; }
    }

    public override string ToString ()
    {
      return string.Format("Query '{0}': {1}", ID, Statement);
    }
  }
}
