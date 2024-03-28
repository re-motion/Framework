using System;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms;

/// <summary>
/// An execution context in which an <see cref="IRdbmsProviderReadOnlyCommand{T}"/> can be executed.
/// </summary>
public interface IRdbmsProviderReadOnlyCommandExecutionContext : IDbCommandFactory, IScalarCommandExecutionContext, IDataReaderCommandExecutionContext
{
}
