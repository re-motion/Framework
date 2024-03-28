using System;
using System.Data;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms;

/// <summary>
/// Represents the aspect of <see cref="IRdbmsProviderReadOnlyCommandExecutionContext"/> or <see cref="IRdbmsProviderReadWriteCommandExecutionContext"/> that enables the
/// execution of an <see cref="IDbCommand"/> that returns an <see cref="IDataReader"/>.
/// </summary>
public interface IDataReaderCommandExecutionContext
{
  IDataReader ExecuteReader (IDbCommand command, CommandBehavior behavior);
}
