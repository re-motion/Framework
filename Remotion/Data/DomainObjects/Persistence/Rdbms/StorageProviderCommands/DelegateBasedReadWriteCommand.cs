using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands
{
  /// <summary>
  /// The <see cref="DelegateBasedCommand{TIn,TOut}"/> executes an <see cref="IRdbmsProviderCommand{T}"/>
  /// and applies a specified operation-transformation to the result.
  /// </summary>
  public class DelegateBasedCommand<TIn, TOut> : IRdbmsProviderCommand<TOut>
  {
    private readonly IRdbmsProviderCommand<TIn> _command;
    private readonly Func<TIn, TOut> _operation;

    public DelegateBasedCommand (IRdbmsProviderCommand<TIn> command, Func<TIn, TOut> operation)
    {
      ArgumentUtility.CheckNotNull("command", command);
      ArgumentUtility.CheckNotNull("operation", operation);

      _command = command;
      _operation = operation;
    }

    public IRdbmsProviderCommand<TIn> Command
    {
      get { return _command; }
    }

    public Func<TIn, TOut> Operation
    {
      get { return _operation; }
    }

    public TOut Execute (IRdbmsProviderCommandExecutionContext executionContext)
    {
      var executionResult = _command.Execute(executionContext);
      return _operation(executionResult);
    }
  }
}
