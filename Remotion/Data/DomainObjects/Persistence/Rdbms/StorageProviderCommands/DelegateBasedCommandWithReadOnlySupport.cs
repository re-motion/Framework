using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands;

/// <summary>
/// The <see cref="DelegateBasedCommandWithReadOnlySupport{TIn,TOut}"/> executes an <see cref="IRdbmsProviderCommandWithReadOnlySupport{T}"/>
/// and applies a specified operation-transformation to the result.
/// </summary>
public class DelegateBasedCommandWithReadOnlySupport<TIn, TOut> : IRdbmsProviderCommandWithReadOnlySupport<TOut>
{
  private readonly IRdbmsProviderCommandWithReadOnlySupport<TIn> _command;
  private readonly Func<TIn, TOut> _operation;

  public DelegateBasedCommandWithReadOnlySupport (IRdbmsProviderCommandWithReadOnlySupport<TIn> command, Func<TIn, TOut> operation)
  {
    ArgumentUtility.CheckNotNull("command", command);
    ArgumentUtility.CheckNotNull("operation", operation);

    _command = command;
    _operation = operation;
  }

  public IRdbmsProviderCommandWithReadOnlySupport<TIn> Command
  {
    get { return _command; }
  }

  public Func<TIn, TOut> Operation
  {
    get { return _operation; }
  }

  public TOut Execute (IRdbmsProviderReadWriteCommandExecutionContext executionContext)
  {
    ArgumentUtility.CheckNotNull("executionContext", executionContext);

    var executionResult = _command.Execute(executionContext);
    return _operation(executionResult);
  }

  public TOut Execute (IRdbmsProviderReadOnlyCommandExecutionContext executionContext)
  {
    ArgumentUtility.CheckNotNull("executionContext", executionContext);

    var executionResult = _command.Execute(executionContext);
    return _operation(executionResult);
  }
}
