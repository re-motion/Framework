// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

/// <summary>
/// An <see cref="IRdbmsProviderCommand"/> for affecting exactly one object.
/// </summary>
public class SingleObjectRdbmsProviderCommand : IRdbmsProviderCommand
{
  public SingleObjectRdbmsProviderCommand (ObjectID id, IDbCommandBuilder commandBuilder)
  {
    ArgumentNullException.ThrowIfNull(id);
    ArgumentNullException.ThrowIfNull(commandBuilder);

    CommandBuilder = commandBuilder;
    ObjectID = id;
  }

  public IDbCommandBuilder CommandBuilder { get; }

  /// <summary>
  /// The <see cref="Remotion.Data.DomainObjects.ObjectID" /> of the affected object.
  /// </summary>
  public ObjectID ObjectID { get; }

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
        throw new RdbmsProviderException($"Error while saving object '{ObjectID}'. {e.Message}", e);
      }

      if (recordsAffected == 1)
        return;

      throw new ConcurrencyViolationException(EnumerableUtility.Singleton(ObjectID));
    }
  }
}
