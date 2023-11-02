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
using Remotion.Data.DomainObjects.Queries.Configuration;

namespace Remotion.Data.DomainObjects.Queries
{
  /// <summary>
  /// Defines the API required for resolving <see cref="QueryDefinition"/> by their query ID.
  /// </summary>
  public interface IQueryDefinitionRepository
  {
    /// <summary>
    /// Determines whether an item is in the <see cref="IQueryDefinitionRepository"/>.
    /// </summary>
    /// <param name="queryID">
    /// The <see cref="QueryDefinition.ID"/> of the <see cref="QueryDefinition"/> to locate in the <see cref="IQueryDefinitionRepository"/>.
    /// Must not be <see langword="null"/> or empty.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the <see cref="QueryDefinition"/> with the <paramref name="queryID"/> is found in the <see cref="IQueryDefinitionRepository"/>;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="System.ArgumentNullException"><paramref name="queryID"/> is <see langword="null"/></exception>
    bool Contains (string queryID);

    /// <summary>
    /// Returns the <see cref="QueryDefinition"/> identified through <paramref name="queryID"/>. If no <see cref="QueryDefinition"/> can be found an exception is thrown.
    /// </summary>
    /// <param name="queryID">The <see cref="QueryDefinition.ID"/> of the <see cref="QueryDefinition"/> to be found. Must not be <see langword="null"/> or empty.</param>
    /// <returns>The <see cref="QueryDefinition"/> identified through <paramref name="queryID"/>.</returns>
    /// <exception cref="QueryConfigurationException">The <see cref="QueryDefinition"/> identified through <paramref name="queryID"/> could not be found.</exception>
    QueryDefinition GetMandatory (string queryID);
  }
}
