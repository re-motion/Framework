// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.FunctionalProgramming;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

/// <summary>
/// An <see cref="IMultiDataContainerSaveCommandContext"/> for commands affecting exactly one object.
/// </summary>
public class SingleObjectMultiDataContainerSaveCommandContext : IMultiDataContainerSaveCommandContext
{
  public SingleObjectMultiDataContainerSaveCommandContext (ObjectID id, IDbCommandBuilder commandBuilder)
  {
    ArgumentNullException.ThrowIfNull(id);
    ArgumentNullException.ThrowIfNull(commandBuilder);

    CommandBuilder = commandBuilder;
    ObjectID = id;
  }

  public int ExpectedAffectedRowCount => 1;

  public IDbCommandBuilder CommandBuilder { get; }

  /// <summary>
  /// The <see cref="Remotion.Data.DomainObjects.ObjectID" /> of the affected object.
  /// </summary>
  public ObjectID ObjectID { get; }

  public ConcurrencyViolationException CreateConcurrencyViolationException ()
  {
    return new ConcurrencyViolationException(EnumerableUtility.Singleton(ObjectID));
  }
}
