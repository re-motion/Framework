using System;
using System.Data;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms;

/// <summary>
/// Represents the aspect of <see cref="IRdbmsProviderReadOnlyCommandExecutionContext"/> or <see cref="IRdbmsProviderReadWriteCommandExecutionContext"/> that enables the
/// execution context to create an <see cref="IDbCommand"/>.
/// </summary>
public interface IDbCommandFactory
{
  IDbCommand CreateDbCommand ();
}
