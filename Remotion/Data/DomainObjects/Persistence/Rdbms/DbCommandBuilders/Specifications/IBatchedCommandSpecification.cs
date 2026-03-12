// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later

using System;
using System.Collections.Generic;
using System.Data.Common;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders.Specifications
{
  /// <summary>
  /// <see cref="IBatchedCommandSpecification"/> defines the API for all implementations that specify how to lock objects in a relational
  /// database.
  /// </summary>
  public interface IBatchedCommandSpecification
  {
    /// <summary>
    /// Creates the <see cref="DbParameter"/> for the given <paramref name="command"/>.
    /// </summary>
    DbParameter CreateDbParameter (DbCommand command, string parameterName);

    /// <summary>
    /// Gets the column names.
    /// </summary>
    IReadOnlyList<string> Columns { get; }

    /// <summary>
    /// Gets the <see cref="TableDefinition"/>.
    /// </summary>
    TableDefinition TableDefinition { get; }
  }
}
