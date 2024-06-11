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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Parameters;

/// <summary>
/// Finds the matching <see cref="IRdbmsStructuredTypeDefinition"/> for a <see cref="QueryParameter"/>.
/// </summary>
public interface IRdbmsStructuredTypeDefinitionFinder
{
  /// <summary>
  /// Gets the matching <see cref="IRdbmsStructuredTypeDefinition"/> for the <paramref name="queryParameter"/>'s <see cref="QueryParameter.Value"/>,
  /// as long as that parameter value is a collection of scalar items.
  /// </summary>
  /// <param name="queryParameter">The <see cref="QueryParameter"/> for whose <see cref="QueryParameter.Value"/> to find a matching <see cref="IRdbmsStructuredTypeDefinition"/>.
  /// </param>
  /// <param name="query">The <see cref="IQuery"/> that contains the <paramref name="queryParameter"/>, for additional context.</param>
  /// <returns>An <see cref="IRdbmsStructuredTypeDefinition"/> that can be used to transmit the <paramref name="queryParameter"/>'s <see cref="QueryParameter.Value"/>,
  /// or <see langword="null"/> if no <see cref="IRdbmsStructuredTypeDefinition"/> is required.</returns>
  IRdbmsStructuredTypeDefinition? GetTypeDefinition (QueryParameter queryParameter, IQuery query);
}
