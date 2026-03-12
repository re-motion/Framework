// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

/// <summary>
/// An <see cref="IRdbmsProviderCommand"/> for acquiring a lock for one or more objects.
/// </summary>
public class BatchedLockRdbmsProviderCommand : IRdbmsProviderCommand
{
  public BatchedLockRdbmsProviderCommand (IDbCommandBuilder commandBuilder, IReadOnlyList<DataContainer> affectedDataContainers)
  {
    ArgumentNullException.ThrowIfNull(commandBuilder);
    ArgumentNullException.ThrowIfNull(affectedDataContainers);

    CommandBuilder = commandBuilder;
    AffectedDataContainers = affectedDataContainers;
  }

  public IReadOnlyList<DataContainer> AffectedDataContainers { get; }
  public IDbCommandBuilder CommandBuilder { get; }

  public void Execute (IRdbmsProviderReadWriteCommandExecutionContext executionContext)
  {
    ArgumentNullException.ThrowIfNull(executionContext);

    using (var command = CommandBuilder.Create(executionContext))
    {
      try
      {
        using (var reader = executionContext.ExecuteReader(command, CommandBehavior.Default))
        {
          var failedIDs = new HashSet<Guid>();
          while (reader.Read())
          {
            failedIDs.Add(reader.GetGuid(0));
          }

          if (failedIDs.Count > 0)
            throw CreateConcurrencyViolationException(failedIDs);
        }
      }
      catch (RdbmsProviderException e)
      {
        throw WrapRdbmsProviderException(e);
      }
    }
  }

  private RdbmsProviderException WrapRdbmsProviderException (RdbmsProviderException e)
  {
    var numberOfIdsToShow = 10;
    var ids = string.Join(", ", AffectedDataContainers.Take(numberOfIdsToShow).Select(d => d.ID));
    if (AffectedDataContainers.Count > numberOfIdsToShow)
      return new RdbmsProviderException($"Error while locking objects '{ids}, ...'. {e.Message}", e);

    return new RdbmsProviderException($"Error while locking objects '{ids}'. {e.Message}", e);
  }

  private ConcurrencyViolationException CreateConcurrencyViolationException (HashSet<Guid> failedIDs)
  {
    // In some edge cases the Guid of an ObjectID is not globally unique therefore
    // it could be possible that for one failed id multiple ObjectIDs are found so 
    // they would be false positives. But we decided that this is an acceptable behaviour.
    var failedObjectIDs = AffectedDataContainers.Where(d => failedIDs.Contains((Guid)d.ID.Value)).Select(d => d.ID).ToArray();
    if (failedObjectIDs.Length > 10)
    {
      var objectIDs = string.Join(", ", failedObjectIDs.Take(10).Select(id => "'" + id + "'"));
      var message = $"Concurrency violation encountered. One or more object(s) have already been changed by someone else: {objectIDs}, ...";
      return new ConcurrencyViolationException(message, failedObjectIDs, null);
    }

    return new ConcurrencyViolationException(failedObjectIDs);
  }
}
