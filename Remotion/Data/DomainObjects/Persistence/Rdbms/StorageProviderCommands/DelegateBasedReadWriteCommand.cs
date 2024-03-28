using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

/// <summary>
/// The <see cref="DelegateBasedReadWriteCommand{TIn,TOut}"/> executes an <see cref="IRdbmsProviderReadWriteCommand{T}"/>
/// and applies a specified operation-transformation to the result.
/// </summary>
public class DelegateBasedReadWriteCommand<TIn, TOut> : IRdbmsProviderReadWriteCommand<TOut>
{
  private readonly IRdbmsProviderReadWriteCommand<TIn> _command;
  private readonly Func<TIn, TOut> _operation;

  public DelegateBasedReadWriteCommand (IRdbmsProviderReadWriteCommand<TIn> command, Func<TIn, TOut> operation)
  {
    ArgumentUtility.CheckNotNull("command", command);
    ArgumentUtility.CheckNotNull("operation", operation);

    _command = command;
    _operation = operation;
  }

  public IRdbmsProviderReadWriteCommand<TIn> Command
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
}
