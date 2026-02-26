// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

/// <summary>
/// An <see cref="IMultiDataContainerSaveCommandContext"/> for commands affecting one or more objects.
/// </summary>
public class BatchedObjectsMultiDataContainerSaveCommandContext : IMultiDataContainerSaveCommandContext
{
  public BatchedObjectsMultiDataContainerSaveCommandContext (TableDefinition tableDefinition, IDbCommandBuilder commandBuilder, int expectedAffectedRowCount)
  {
    ArgumentNullException.ThrowIfNull(tableDefinition);
    ArgumentNullException.ThrowIfNull(commandBuilder);

    TableDefinition = tableDefinition;
    CommandBuilder = commandBuilder;
    ExpectedAffectedRowCount = expectedAffectedRowCount;
  }

  public int ExpectedAffectedRowCount { get; }
  public IDbCommandBuilder CommandBuilder { get; }

  /// <summary>
  ///   The affected <see cref="Remotion.Data.DomainObjects.Persistence.Rdbms.Model.TableDefinition" />.
  /// </summary>
  public TableDefinition TableDefinition { get; }

  public ConcurrencyViolationException CreateConcurrencyViolationException ()
  {
    // TODO: RM-9640/RM-9641/RM-9644/RM-9645 
    throw new NotImplementedException();
  }
}
