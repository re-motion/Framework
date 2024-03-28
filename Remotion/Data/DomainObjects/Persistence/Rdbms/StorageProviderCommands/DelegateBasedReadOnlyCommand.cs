using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

/// <summary>
/// The <see cref="DelegateBasedReadOnlyCommand{TIn,TOut}"/> executes an <see cref="IRdbmsProviderReadOnlyCommand{T}"/>
/// and applies a specified operation-transformation to the result.
/// </summary>
public class DelegateBasedReadOnlyCommand<TIn, TOut> : IRdbmsProviderReadOnlyCommand<TOut>
{
  private readonly IRdbmsProviderReadOnlyCommand<TIn> _command;
  private readonly Func<TIn, TOut> _operation;

  public DelegateBasedReadOnlyCommand (IRdbmsProviderReadOnlyCommand<TIn> command, Func<TIn, TOut> operation)
  {
    ArgumentUtility.CheckNotNull("command", command);
    ArgumentUtility.CheckNotNull("operation", operation);

    _command = command;
    _operation = operation;
  }

  public IRdbmsProviderReadOnlyCommand<TIn> Command
  {
    get { return _command; }
  }

  public Func<TIn, TOut> Operation
  {
    get { return _operation; }
  }

  public TOut Execute (IRdbmsProviderReadWriteCommandExecutionContext executionContext)
  {
    var executionResult = _command.Execute(executionContext);
    return _operation(executionResult);
  }

  public TOut Execute (IRdbmsProviderReadOnlyCommandExecutionContext executionContext)
  {
    var executionResult = _command.Execute(executionContext);
    return _operation(executionResult);
  }
}
