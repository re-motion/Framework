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
using System.Linq.Expressions;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Linq;
using Remotion.Linq.Parsing.Structure;

namespace Remotion.Data.DomainObjects.Linq
{
  public class DomainObjectQueryable<T> : QueryableBase<T>
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainObjectQueryable{T}"/> class.
    /// </summary>
    /// <param name="queryParser">The <see cref="IQueryParser"/> used to parse queries. Specify an instance of 
    /// <see cref="QueryParser"/> for default behavior. See also <see cref="QueryParser.CreateDefault"/>.</param>
    /// <param name="executor">The <see cref="DomainObjectQueryExecutor"/> that is used for the queries.</param>
    /// <remarks>
    /// <para>
    /// This constructor marks the default entry point into a LINQ query for <see cref="DomainObject"/> instances. It is normally used to define
    /// the data source on which the first <c>from</c> expression operates.
    /// </para>
    /// <para>
    /// The <see cref="QueryFactory"/> class wraps this constructor and provides some additional support, so it should usually be preferred to a
    /// direct constructor call.
    /// </para>
    /// </remarks>
    public DomainObjectQueryable (IQueryParser queryParser, IQueryExecutor executor)
      : base (queryParser, executor)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainObjectQueryable{T}"/> class. This is an infrastructure constructor.
    /// </summary>
    /// <param name="provider">The provider to be used for querying.</param>
    /// <param name="expression">The expression encapsulated by this <see cref="DomainObjectQueryable{T}"/> instance.</param>
    /// <remarks>
    /// This constructor is used by the standard query methods defined in <see cref="Queryable"/> when a LINQ query is constructed.
    /// </remarks>
    public DomainObjectQueryable (QueryProviderBase provider, Expression expression)
        : base (provider, expression)
    {
    }

    public override string ToString ()
    {
      return "DomainObjectQueryable<" + typeof (T).Name + ">";
    }

    public DomainObjectQueryExecutor GetExecutor ()
    {
      return (DomainObjectQueryExecutor) Provider.Executor;
    }

    public new QueryProviderBase Provider
    {
      get { return (QueryProviderBase) base.Provider; }
    }
  }
}