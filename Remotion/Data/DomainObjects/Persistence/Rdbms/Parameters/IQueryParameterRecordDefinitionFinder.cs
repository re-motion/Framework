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
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;

/// <summary>
/// Finds the <see cref="RecordDefinition"/> to use for transmitting a <see cref="QueryParameter"/> with a structured value. 
/// </summary>
public interface IQueryParameterRecordDefinitionFinder
{
  /// <summary>
  /// Gets the <see cref="RecordDefinition"/> to use for transmitting the <paramref name="queryParameter"/>'s <see cref="QueryParameter.Value"/>
  /// in the context of the given <paramref name="query"/>.
  /// </summary>
  /// <param name="queryParameter">The <see cref="QueryParameter"/> to transmit.</param>
  /// <param name="query">The <see cref="IQuery"/> containing the <paramref name="queryParameter"/>.</param>
  /// <returns>The <see cref="RecordDefinition"/> to use for transmitting the <paramref name="queryParameter"/>,
  /// or <see langword="null"/> if its <see cref="QueryParameter.Value"/> is a scalar.</returns>
  RecordDefinition? GetRecordDefinition (QueryParameter queryParameter, IQuery query);
}
