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

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

/// <summary>
/// Provides the <see cref="IRdbmsStructuredTypeDefinition"/>s that have single column for a scalar value.
/// </summary>
public interface ISingleScalarStructuredTypeDefinitionProvider
{
  /// <summary>
  /// Gets all supported single-column <see cref="IRdbmsStructuredTypeDefinition"/>s. 
  /// </summary>
  /// <remarks>
  /// The <see cref="SchemaGeneration.RdbmsStructuredTypeDefinitionProvider"/> uses this method to determine for which structured types to create scripts.
  /// </remarks>
  IEnumerable<IRdbmsStructuredTypeDefinition> GetAllStructuredTypeDefinitions ();

  /// <summary>
  /// Gets the single-column <see cref="IRdbmsStructuredTypeDefinition"/> for the given <paramref name="dotNetType"/>.
  /// Implementations may return different <see cref="IRdbmsStructuredTypeDefinition"/>s for different values of <paramref name="forDistinctValues"/>.
  /// </summary>
  /// <param name="dotNetType">The scalar <see cref="Type"/> for which to get the single-column <see cref="IRdbmsStructuredTypeDefinition"/>.</param>
  /// <param name="forDistinctValues">Indicates whether the <see cref="IRdbmsStructuredTypeDefinition"/> will only be used with collections that do not contain duplicates.</param>
  IRdbmsStructuredTypeDefinition GetStructuredTypeDefinition (Type dotNetType, bool forDistinctValues);
}
