// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

/// <summary>
/// An <see cref="IRdbmsProviderCommand"/> for commands affecting one or more objects.
/// </summary>
public class BatchedObjectsRdbmsProviderCommand : IRdbmsProviderCommand
{
  public BatchedObjectsRdbmsProviderCommand (IDbCommandBuilder commandBuilder,  IReadOnlyList<DataContainer> affectedDataContainers)
  {
    ArgumentNullException.ThrowIfNull(commandBuilder);
    ArgumentNullException.ThrowIfNull(affectedDataContainers);

    CommandBuilder = commandBuilder;
    AffectedDataContainers = affectedDataContainers;
  }

  public IDbCommandBuilder CommandBuilder { get; }
  public IReadOnlyList<DataContainer> AffectedDataContainers { get; }

  public void Execute (IRdbmsProviderReadWriteCommandExecutionContext executionContext)
  {
    using (var command = CommandBuilder.Create(executionContext))
    {
      int recordsAffected;
      try
      {
        recordsAffected = executionContext.ExecuteNonQuery(command);
      }
      catch (RdbmsProviderException e)
      {
        throw WrapRdbmsProviderException(e);
      }

      if (recordsAffected == AffectedDataContainers.Count)
        return;

      throw CreateConcurrencyViolationException();
    }
  }

  private RdbmsProviderException WrapRdbmsProviderException (RdbmsProviderException e)
  {
    var numberOfIdsToShow = 10;
    var affectedObjectIds = AffectedDataContainers.Select(d => d.ID).ToArray();
    var displayedIds = string.Join(", ", affectedObjectIds.Take(numberOfIdsToShow));
    if (affectedObjectIds.Length > numberOfIdsToShow)
      return new RdbmsProviderException($"Error while saving objects '{displayedIds}, ...'. {e.Message}", e, affectedObjectIds);

    return new RdbmsProviderException($"Error while saving objects '{displayedIds}'. {e.Message}", e, affectedObjectIds);
  }

  private ConcurrencyViolationException CreateConcurrencyViolationException ()
  {
    var ids = AffectedDataContainers.Select(d => d.ID).ToArray();
    if (ids.Length > 10)
    {
      var objectIDs = string.Join(", ", ids.Take(10).Select(id => "'" + id + "'"));
      var message = $"Concurrency violation encountered. One or more object(s) have already been changed by someone else: {objectIDs}, ...";
      return new ConcurrencyViolationException(message, ids, null);
    }

    return new ConcurrencyViolationException(ids);
  }
}
