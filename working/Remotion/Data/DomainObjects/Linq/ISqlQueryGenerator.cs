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
using Remotion.Linq;
using Remotion.Linq.SqlBackend.SqlGeneration;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Defines an interface for objects generating a SQL query for a LINQ <see cref="QueryModel"/>.
  /// </summary>
  public interface ISqlQueryGenerator
  {
    /// <summary>
    /// Creates a SQL query from a given <see cref="QueryModel"/>.
    /// </summary>
    /// <param name="queryModel">
    ///   The <see cref="QueryModel"/> a SQL query is generated for. The query must not contain any eager fetch result operators.
    /// </param>
    /// <returns>A <see cref="SqlCommandData"/> instance containing the SQL text, parameters, and an in-memory projection for the given query model.</returns>
    SqlQueryGeneratorResult CreateSqlQuery (QueryModel queryModel);
  }
}