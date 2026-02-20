// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

public interface IMultiDataContainerSaveCommandContext
{
  /// <summary>
  /// The expected number of affected rows
  /// </summary>
  int ExpectedAffectedRowCount { get; }

  /// <summary>
  /// The <see cref="IDbCommandBuilder" /> for this <see cref="SingleObjectMultiDataContainerSaveCommandContext" />
  /// </summary>
  IDbCommandBuilder CommandBuilder { get; }

  /// <summary>
  /// Creates a <see cref="ConcurrencyViolationException"/> but does not throw it.
  /// </summary>
  ConcurrencyViolationException CreateConcurrencyViolationException ();
}
