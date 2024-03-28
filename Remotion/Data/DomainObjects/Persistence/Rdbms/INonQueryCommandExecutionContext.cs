using System;
using System.Data;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms;

/// <summary>
/// Represents the aspect of <see cref="IRdbmsProviderReadOnlyCommandExecutionContext"/> or <see cref="IRdbmsProviderReadWriteCommandExecutionContext"/> that enables the
/// execution of an <see cref="IDbCommand"/> without a return value.
/// </summary>
public interface INonQueryCommandExecutionContext
{
  int ExecuteNonQuery (IDbCommand command);
}
