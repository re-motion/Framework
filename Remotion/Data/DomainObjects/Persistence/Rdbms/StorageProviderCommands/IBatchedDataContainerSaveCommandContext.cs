// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

public interface IBatchedDataContainerSaveCommandContext
{
  /// <summary>
  /// The <see cref="IDbCommandBuilder" /> for this <see cref="SingleObjectRdbmsProviderCommand" />
  /// </summary>
  IDbCommandBuilder CommandBuilder { get; }

  void Execute (IRdbmsProviderReadWriteCommandExecutionContext executionContext);
}
