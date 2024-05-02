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
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;

/// <summary>
/// Defines the API for creating <see cref="IDataParameterDefinition"/> instances.
/// </summary>
public interface IDataParameterDefinitionFactory
{
  /// <summary>
  /// Creates a <see cref="IDataParameterDefinition"/> for the given <paramref name="queryParameter"/>.
  /// </summary>
  IDataParameterDefinition CreateDataParameterDefinition (QueryParameter queryParameter);
}
