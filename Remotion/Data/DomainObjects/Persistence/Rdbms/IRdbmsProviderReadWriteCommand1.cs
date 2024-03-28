using System;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms;

/// <summary>
/// Represents a command with a return value to be executed by a storage provider.
/// </summary>
public interface IRdbmsProviderReadWriteCommand<out T>
{
  T Execute (IRdbmsProviderReadWriteCommandExecutionContext executionContext);
}
