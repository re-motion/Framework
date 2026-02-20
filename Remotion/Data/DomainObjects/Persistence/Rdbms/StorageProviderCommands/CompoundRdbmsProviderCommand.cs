// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

/// <summary>
/// The <see cref="CompoundRdbmsProviderCommand"/> combines multiple <see cref="IRdbmsProviderCommand"/>s 
/// </summary>
public class CompoundRdbmsProviderCommand : IRdbmsProviderCommand
{
  private readonly IReadOnlyList<IRdbmsProviderCommand> _innerCommands;

  public CompoundRdbmsProviderCommand (IEnumerable<IRdbmsProviderCommand> innerCommands)
  {
    ArgumentNullException.ThrowIfNull(innerCommands);

    _innerCommands = innerCommands.ToArray();
  }

  public IReadOnlyList<IRdbmsProviderCommand> InnerCommands => _innerCommands;

  public void Execute (IRdbmsProviderReadWriteCommandExecutionContext executionContext)
  {
    ArgumentNullException.ThrowIfNull(executionContext);

    foreach (var data in _innerCommands)
    {
      data.Execute(executionContext);
    }
  }
}
