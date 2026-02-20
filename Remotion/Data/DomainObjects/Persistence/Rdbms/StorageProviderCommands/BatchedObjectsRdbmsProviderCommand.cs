// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

/// <summary>
/// An <see cref="IRdbmsProviderCommand"/> for commands affecting one or more objects.
/// </summary>
public class BatchedObjectsRdbmsProviderCommand : IRdbmsProviderCommand
{
  public BatchedObjectsRdbmsProviderCommand (TableDefinition tableDefinition, IDbCommandBuilder commandBuilder, DataContainer[] affectedDataContainers)
  {
    ArgumentNullException.ThrowIfNull(tableDefinition);
    ArgumentNullException.ThrowIfNull(commandBuilder);

    TableDefinition = tableDefinition;
    CommandBuilder = commandBuilder;
    AffectedDataContainers = affectedDataContainers;
  }

  public IDbCommandBuilder CommandBuilder { get; }
  public DataContainer[] AffectedDataContainers { get; }

  /// <summary>
  ///   The affected <see cref="Remotion.Data.DomainObjects.Persistence.Rdbms.Model.TableDefinition" />.
  /// </summary>
  public TableDefinition TableDefinition { get; }

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
        var ids = string.Join(", ", AffectedDataContainers.Take(10).Select(d => d.ID));
        if (AffectedDataContainers.Length > 10)
        {
          throw new RdbmsProviderException($"Error while saving objects '{ids}' and {AffectedDataContainers.Length - 10} others. {e.Message}", e);
        }

        throw new RdbmsProviderException($"Error while saving objects '{ids}'. {e.Message}", e);
      }

      if (recordsAffected == AffectedDataContainers.Length)
        return;

      throw CreateConcurrencyViolationException();
    }
  }

  private ConcurrencyViolationException CreateConcurrencyViolationException ()
  {
    var failedObjectIDs = AffectedDataContainers.Take(10).Select(d => d.ID).ToList();
    return new ConcurrencyViolationException(failedObjectIDs);
  }

}
