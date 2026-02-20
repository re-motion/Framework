// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

/// <summary>
/// The <see cref="BatchedDataContainerSaveCommand"/> saves a sequence of <see cref="DataContainer"/> instances by executing the given
/// <see cref="DbCommand"/> instances.
/// </summary>
public class BatchedDataContainerSaveCommand : IRdbmsProviderCommand
{
  private readonly IMultiDataContainerSaveCommandContext[] _saveCommandContexts;

  public BatchedDataContainerSaveCommand (IEnumerable<IMultiDataContainerSaveCommandContext> saveCommandContexts)
  {
    ArgumentNullException.ThrowIfNull(saveCommandContexts);

    _saveCommandContexts = saveCommandContexts.ToArray();
  }

  public IMultiDataContainerSaveCommandContext[] Contexts => _saveCommandContexts;

  public void Execute (IRdbmsProviderReadWriteCommandExecutionContext executionContext)
  {
    ArgumentNullException.ThrowIfNull(executionContext);

    foreach (var data in _saveCommandContexts)
    {
      using (var command = data.CommandBuilder.Create(executionContext))
      {
        int recordsAffected;
        try
        {
          recordsAffected = executionContext.ExecuteNonQuery(command);
        }
        catch (RdbmsProviderException e)
        {
          throw new RdbmsProviderException($"Error while saving object '{data}'. {e.Message}", e);
        }

        if (recordsAffected == data.ExpectedAffectedRowCount)
          continue;

        throw data.CreateConcurrencyViolationException();
      }
    }
  }
}
