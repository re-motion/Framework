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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.Configuration
{
  /// <summary>
  /// Represents a collection of <see cref="QueryDefinition"/>s.
  /// </summary>
  [Serializable]
  [Obsolete("QueryDefinitionCollection is no longer supported. (Version 6.0.0)", true)]
  public class QueryDefinitionCollection : CommonCollection
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    /// <summary>
    /// Initializes a new instance of the <b>QueryDefinitionCollection</b> class.
    /// </summary>
    public QueryDefinitionCollection ()
    {
    }

    // standard constructor for collections

    /// <summary>
    /// Initializes a new <b>QueryDefinitionCollection</b> as a shallow copy of a given <see cref="QueryDefinitionCollection"/>.
    /// </summary>
    /// <remarks>The new <b>QueryDefinitionCollection</b> has the same items as the given <paramref name="collection"/>.</remarks>
    /// <param name="collection">The <see cref="QueryDefinitionCollection"/> to copy. Must not be <see langword="null"/>.</param>
    /// <param name="makeCollectionReadOnly">Indicates whether the new collection should be read-only.</param>
    /// <exception cref="System.ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
    public QueryDefinitionCollection (
        QueryDefinitionCollection collection,
        bool makeCollectionReadOnly)
    {
      ArgumentUtility.CheckNotNull("collection", collection);

      foreach (QueryDefinition queryDefinition in collection)
        Add(queryDefinition);

      this.SetIsReadOnly(makeCollectionReadOnly);
    }

    // methods and properties

    /// <summary>
    /// Returns the <see cref="QueryDefinition"/> identified through <paramref name="queryID"/>. If no <see cref="QueryDefinition"/> can be found an exception is thrown.
    /// </summary>
    /// <param name="queryID">The <see cref="QueryDefinition.ID"/> of the <see cref="QueryDefinition"/> to be found. Must not be <see langword="null"/> or empty.</param>
    /// <returns>The <see cref="QueryDefinition"/> identified through <paramref name="queryID"/>.</returns>
    /// <exception cref="QueryConfigurationException">The <see cref="QueryDefinition"/> identified through <paramref name="queryID"/> could not be found.</exception>
    public QueryDefinition GetMandatory (string queryID)
    {
      ArgumentUtility.CheckNotNullOrEmpty("queryID", queryID);

      var queryDefinition = this[queryID];
      if (queryDefinition == null)
        throw CreateQueryConfigurationException("QueryDefinition '{0}' does not exist.", queryID);

      return queryDefinition;
    }

    private ArgumentException CreateArgumentException (string message, string parameterName, params object[] args)
    {
      return new ArgumentException(string.Format(message, args), parameterName);
    }

    private QueryConfigurationException CreateQueryConfigurationException (
        string message,
        params object[] args)
    {
      return new QueryConfigurationException(string.Format(message, args));
    }

    public void Merge (QueryDefinitionCollection source)
    {
      ArgumentUtility.CheckNotNull("source", source);

      foreach (QueryDefinition query in source)
      {
        var existingQueryDefinition = this[query.ID];
        if (existingQueryDefinition != null)
          throw new DuplicateQueryDefinitionException(existingQueryDefinition, query);
        else
          Add(query);
      }
    }

    #region Standard implementation for "add-only" collections

    /// <summary>
    /// Determines whether an item is in the <see cref="QueryDefinitionCollection"/>.
    /// </summary>
    /// <param name="queryDefinition">The <see cref="QueryDefinition"/> to locate in the <see cref="QueryDefinitionCollection"/>. Must not be <see langword="null"/>.</param>
    /// <returns><see langword="true"/> if <paramref name="queryDefinition"/> is found in the <see cref="QueryDefinitionCollection"/>; otherwise, false;</returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="queryDefinition"/> is <see langword="null"/></exception>
    /// <remarks>This method only returns true, if the same reference is found in the collection.</remarks>
    public bool Contains (QueryDefinition queryDefinition)
    {
      ArgumentUtility.CheckNotNull("queryDefinition", queryDefinition);

      return BaseContains(queryDefinition.ID, queryDefinition);
    }

    /// <summary>
    /// Determines whether an item is in the <see cref="QueryDefinitionCollection"/>.
    /// </summary>
    /// <param name="queryID">
    /// The <see cref="QueryDefinition.ID"/> of the <see cref="QueryDefinition"/> to locate in the <see cref="QueryDefinitionCollection"/>.
    /// Must not be <see langword="null"/> or empty.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="QueryDefinition"/> with the <paramref name="queryID"/> is found in the <see cref="QueryDefinitionCollection"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="queryID"/> is <see langword="null"/></exception>
    public bool Contains (string queryID)
    {
      ArgumentUtility.CheckNotNullOrEmpty("queryID", queryID);
      return BaseContainsKey(queryID);
    }

    /// <summary>
    /// Gets or sets the <see cref="QueryDefinition"/> with a given <paramref name="index"/> in the <see cref="QueryDefinitionCollection"/>.
    /// </summary>
    /// <exception cref="System.ArgumentOutOfRangeException">
    ///   <paramref name="index"/> is less than zero.<br /> -or- <br />
    ///   <paramref name="index"/> is equal to or greater than the number of items in the collection.
    /// </exception>
    public QueryDefinition this [int index]
    {
      get { return (QueryDefinition)BaseGetObject(index); }
    }

    /// <summary>
    /// Gets the <see cref="QueryDefinition"/> with a given <see cref="QueryDefinition.ID"/> from the <see cref="QueryDefinitionCollection"/>.
    /// </summary>
    /// <remarks>The indexer returns <see langword="null"/> if the given <paramref name="queryID"/> was not found.</remarks>
    public QueryDefinition? this [string queryID]
    {
      get
      {
        ArgumentUtility.CheckNotNullOrEmpty("queryID", queryID);
        return (QueryDefinition?)BaseGetObject(queryID);
      }
    }

    /// <summary>
    /// Adds a <see cref="QueryDefinition"/> to the collection.
    /// </summary>
    /// <param name="queryDefinition">The <see cref="QueryDefinition"/> to add. Must not be <see langword="null"/>.</param>
    /// <returns>The zero-based index where the <paramref name="queryDefinition"/> has been added.</returns>
    /// <exception cref="System.NotSupportedException">The collection is read-only.</exception>
    /// <exception cref="System.ArgumentNullException"><paramref name="queryDefinition"/> is <see langword="null"/>.</exception>
    public int Add (QueryDefinition queryDefinition)
    {
      ArgumentUtility.CheckNotNull("queryDefinition", queryDefinition);

      if (Contains(queryDefinition.ID))
      {
        throw CreateArgumentException(
            "QueryDefinition '{0}' already exists in collection.", "queryDefinition", queryDefinition.ID);
      }

      return BaseAdd(queryDefinition.ID, queryDefinition);
    }

    #endregion
  }
}
