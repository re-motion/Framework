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

namespace Remotion.Data.DomainObjects.Queries
{
/// <summary>
/// Represents a collection of <see cref="QueryParameter"/> objects.
/// </summary>
public class QueryParameterCollection : CommonCollection
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  /// <summary>
  /// Initializes a new <b>QueryParameterCollection</b>.
  /// </summary>
  public QueryParameterCollection ()
  {
  }

  // standard constructor for collections
  /// <summary>
  /// Initializes a new <b>QueryParameterCollection</b> as a shallow copy of a given <see cref="QueryParameterCollection"/>.
  /// </summary>
  /// <remarks>
  /// The new <b>QueryParameterCollection</b> has the same items as the given <paramref name="collection"/>.
  /// </remarks>
  /// <param name="collection">The <see cref="QueryParameterCollection"/> to copy. Must not be <see langword="null"/>.</param>
  /// <param name="makeCollectionReadOnly">Indicates whether the new collection should be read-only.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="collection"/> is <see langword="null"/>.</exception>
  public QueryParameterCollection (QueryParameterCollection collection, bool makeCollectionReadOnly)
  {
    ArgumentUtility.CheckNotNull("collection", collection);

    foreach (QueryParameter parameter in collection)
    {
      Add(parameter);
    }

    this.SetIsReadOnly(makeCollectionReadOnly);
  }

  // methods and properties

  /// <summary>
  /// Adds a new <see cref="QueryParameter"/> to the collection with <see cref="QueryParameter.ParameterType"/> of Value.
  /// </summary>
  /// <param name="parameterName">The <see cref="QueryParameter.Name"/> of the new parameter. Must not be <see langword="null"/>.</param>
  /// <param name="parameterValue">The <see cref="QueryParameter.Value"/> of the new parameter.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="parameterName"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="parameterName"/> is an empty string.</exception>
  public void Add (string parameterName, object parameterValue)
  {
    ArgumentUtility.CheckNotNullOrEmpty("parameterName", parameterName);

    Add(new QueryParameter(parameterName, parameterValue));
  }

  /// <summary>
  /// Adds a new <see cref="QueryParameter"/> to the collection.
  /// </summary>
  /// <param name="parameterName">The <see cref="QueryParameter.Name"/> of the new parameter. Must not be <see langword="null"/>.</param>
  /// <param name="parameterValue">The <see cref="QueryParameter.Value"/> of the new parameter.</param>
  /// <param name="parameterType">The <see cref="QueryParameterType"/> of the new parameter.</param>
  /// <exception cref="System.ArgumentNullException"><paramref name="parameterName"/> is <see langword="null"/>.</exception>
  /// <exception cref="System.ArgumentException"><paramref name="parameterName"/> is an empty string.</exception>
  /// <exception cref="System.ArgumentOutOfRangeException"><paramref name="parameterType"/> is not a valid enum value.</exception>
  public void Add (string parameterName, object parameterValue, QueryParameterType parameterType)
  {
    ArgumentUtility.CheckNotNullOrEmpty("parameterName", parameterName);
    ArgumentUtility.CheckValidEnumValue("parameterType", parameterType);

    Add(new QueryParameter(parameterName, parameterValue, parameterType));
  }

  #region Standard implementation for "add-only" collections

  /// <summary>
  /// Determines whether an item is in the <see cref="QueryParameterCollection"/>.
  /// </summary>
  /// <param name="queryParameter">The <see cref="QueryParameter"/> to locate in the collection. Must not be <see langword="null"/>.</param>
  /// <returns><see langword="true"/> if <paramref name="queryParameter"/> is found in the <see cref="QueryParameterCollection"/>; otherwise, false;</returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="queryParameter"/> is <see langword="null"/></exception>
  /// <remarks>This method only returns true, if the same reference is found in the collection.</remarks>
  public bool Contains (QueryParameter queryParameter)
  {
    ArgumentUtility.CheckNotNull("queryParameter", queryParameter);

    return BaseContains(queryParameter.Name, queryParameter);
  }

  /// <summary>
  /// Determines whether an item is in the <see cref="QueryParameterCollection"/>.
  /// </summary>
  /// <param name="name">The <see cref="QueryParameter.Name"/> of the <see cref="QueryParameter"/> to locate in the collection. Must not be <see langword="null"/>.</param>
  /// <returns>
  /// <see langword="true"/> if a <see cref="QueryParameter"/> with a <see cref="QueryParameter.Name"/> of <paramref name="name"/>
  /// is found in the <see cref="QueryParameterCollection"/>; otherwise, <see langword="false"/>.
  /// </returns>
  /// <exception cref="System.ArgumentNullException"><paramref name="name"/> is <see langword="null"/></exception>
  public bool Contains (string name)
  {
    return BaseContainsKey(name);
  }

  /// <summary>
  /// Gets the <see cref="QueryParameter"/> with a given <paramref name="index"/> in the <see cref="QueryParameterCollection"/>.
  /// </summary>
  public QueryParameter this [int index]
  {
    get { return (QueryParameter)BaseGetObject(index); }
  }

  /// <summary>
  /// Gets the <see cref="QueryParameter"/> with a given <paramref name="name"/> in the <see cref="QueryParameterCollection"/>.
  /// </summary>
  /// <remarks>The indexer returns <see langword="null"/> if the given <paramref name="name"/> was not found.</remarks>
  public QueryParameter? this [string name]
  {
    get { return (QueryParameter?)BaseGetObject(name); }
  }

  /// <summary>
  /// Adds a <see cref="QueryParameter"/> to the collection.
  /// </summary>
  /// <param name="parameter">The <see cref="QueryParameter"/> to add.</param>
  /// <returns>The zero-based index where <paramref name="parameter"/> has been added.</returns>
  public int Add (QueryParameter parameter)
  {
    ArgumentUtility.CheckNotNull("parameter", parameter);

    return BaseAdd(parameter.Name, parameter);
  }

  #endregion
}
}
