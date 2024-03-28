using System;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms;

/// <summary>
/// Represents a command with a return value that does not attempt to modify the database, to be executed by a storage provider.
/// </summary>
public interface IRdbmsProviderReadOnlyCommand<out T> : IRdbmsProviderReadWriteCommand<T>
{
}
