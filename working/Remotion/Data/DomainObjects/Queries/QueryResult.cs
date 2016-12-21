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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints.VirtualEndPoints.CollectionEndPoints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// Represents a typed result of a collection query.
  /// </summary>
  /// <typeparam name="T">A common type of the <see cref="DomainObject"/> instances returned by the query. This is the same as the type parameter
  /// supplied to <see cref="IQueryManager.GetCollection{T}"/>.</typeparam>
  /// <seealso cref="IQueryManager.GetCollection"/>
  /// <remarks>
  /// <para>
  /// The result might contain duplicates or <see langword="null"/> values when calling <see cref="AsEnumerable"/> or <see cref="ToArray"/>. To filter
  /// them out, use the <see cref="Enumerable.Distinct{TSource}(IEnumerable{TSource})"/> and 
  /// <see cref="Enumerable.Where{TSource}(IEnumerable{TSource},Func{TSource,bool})"/> methods.
  /// </para>
  /// <para>
  /// This class implements the <see cref="IQueryResult"/> interface, which can be used if the <typeparamref name="T">T</typeparamref> type
  /// parameter is not known at compile time.
  /// </para>
  /// </remarks>
  public class QueryResult<T> : IQueryResult
      where T : DomainObject
  {
    private readonly IQuery _query;
    private readonly T[] _queryResult;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryResult{T}"/> class.
    /// </summary>
    /// <param name="query">The query that yielded the <paramref name="queryResult"/></param>.
    /// <param name="queryResult">The elements making up the query result. The <see cref="IEnumerable{T}"/> is enumerated exactly once by this class.</param>
    public QueryResult (IQuery query, T[] queryResult)
    {
      ArgumentUtility.CheckNotNull ("query", query);
      ArgumentUtility.CheckNotNull ("queryResult", queryResult);

      _query = query;
      _queryResult = queryResult;
    }

    /// <summary>
    /// Gets the number of <see cref="DomainObject"/> instances returned by the query.
    /// </summary>
    /// <value>The result count.</value>
    public int Count
    {
      get { return _queryResult.Length; }
    }

    /// <summary>
    /// Gets the query used to construct this result.
    /// </summary>
    /// <value>The query that yielded the result.</value>
    public IQuery Query
    {
      get { return _query; }
    }

    /// <summary>
    /// Determines whether the result set contains duplicates.
    /// </summary>
    /// <returns>
    /// 	<see langword="true" /> if result set contains duplicates; otherwise, <see langword="false" />.
    /// </returns>
    /// <remarks>
    /// This method needs additional memory to hold up to <see cref="Count"/> elements to check for duplicates, and it iterates over the result 
    /// elements, visiting each at most once.
    /// </remarks>
    [Obsolete ("This feature has not yet been implemented - at the moment, queries cannot return duplicates. (1.13.176.0, RM-791).")]
    public bool ContainsDuplicates ()
    {
      var visitedElements = new HashSet<T> ();

      foreach (var resultElement in _queryResult)
      {
        if (visitedElements.Contains (resultElement))
          return true;

        visitedElements.Add (resultElement);
      }

      return false;
    }

    /// <summary>
    /// Determines whether result set contains <see langword="null"/> values.
    /// </summary>
    /// <returns>
    /// 	<see langword="true" /> if result set contains <see langword="null"/> values; otherwise, <see langword="false" />.
    /// </returns>
    /// <remarks>
    /// This method iterates over the result elements, visiting each at most once.
    /// </remarks>
    public bool ContainsNulls ()
    {
      return _queryResult.Contains (null);
    }

    /// <summary>
    /// Returns the query result set as an enumerable object. Might contain duplicates or <see langword="null"/> values.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="IEnumerable{T}"/> containing the <see cref="DomainObject"/> instances yielded by the query.
    /// </returns>
    public IEnumerable<T> AsEnumerable ()
    {
      return _queryResult;
    }

    IEnumerable<DomainObject> IQueryResult.AsEnumerable ()
    {
      return _queryResult;
    }

    /// <summary>
    /// Returns the query result set as an array. Might contain duplicates or <see langword="null"/> values.
    /// </summary>
    /// <returns>
    /// An array containing the <see cref="DomainObject"/> instances yielded by the query.
    /// </returns>
    public T[] ToArray ()
    {
      return (T[]) _queryResult.Clone();
    }

    DomainObject[] IQueryResult.ToArray ()
    {
      return ToArray ();
    }

    /// <summary>
    /// Returns the query result set as an <see cref="ObjectList{T}"/>. If the result set contains duplicates or <see langword="null"/> values, this
    /// method throws an exception.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="ObjectList{T}"/> containing the <see cref="DomainObject"/> instances yielded by the query.
    /// </returns>
    /// <exception cref="UnexpectedQueryResultException">The query contains <see langword="null"/> values or duplicates.</exception>
    public ObjectList<T> ToObjectList ()
    {
      return ToObjectList (_queryResult);
    }

    ObjectList<DomainObject> IQueryResult.ToObjectList ()
    {
      return ToObjectList<DomainObject> (_queryResult);
    }

    /// <summary>
    /// Returns the query result set as the custom collection as specified by <see cref="IQuery.CollectionType"/>. If the result set contains
    /// duplicates or <see langword="null"/> values, this method throws an exception. If no custom collection was specified, a standard
    /// <see cref="DomainObjectCollection"/> is returned.
    /// </summary>
    /// <returns>
    /// An instance of <see cref="DomainObjectCollection"/> containing the <see cref="DomainObject"/> instances yielded by the query. The
    /// concrete type of this collection is determined by the <see cref="IQuery.CollectionType"/> property of the query used.
    /// </returns>
    /// <exception cref="UnexpectedQueryResultException">The query contains <see langword="null"/> values or duplicates.</exception>
    public DomainObjectCollection ToCustomCollection ()
    {
      var collectionType = Query.CollectionType ?? typeof (DomainObjectCollection);
      try
      {
        return DomainObjectCollectionFactory.Instance.CreateCollection (collectionType, _queryResult);
      }
      catch (Exception ex)
      {
        var message = string.Format ("Cannot create a custom collection of type '{0}' for the query result: {1}", collectionType.Name, ex.Message);
        throw new UnexpectedQueryResultException (message, ex);
      }
    }

    private ObjectList<TResult> ToObjectList<TResult> (IEnumerable<TResult> values)
      where TResult : DomainObject
    {
      try
      {
        return new ObjectList<TResult> (values);
      }
      catch (Exception ex)
      {
        throw new UnexpectedQueryResultException ("Cannot create an ObjectList for the query result: " + ex.Message, ex);
      }
    }
  }
}
