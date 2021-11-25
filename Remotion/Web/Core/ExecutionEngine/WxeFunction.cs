// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq;
using System.Text;
using System.Threading;
using JetBrains.Annotations;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// The new <see cref="WxeFunction"/>. Will replace the <see cref="WxeFunction"/> type once implemtation is completed.
  /// </summary>
  [Serializable]
  public abstract class WxeFunction : WxeStepList, IWxeFunctionExecutionContext
  {
    public static bool HasAccess (Type functionType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom("functionType", functionType, typeof(WxeFunction));

      var wxeSecurityAdapter = GetWxeSecurityAdapter();
      if (wxeSecurityAdapter == null)
        return true;

      return wxeSecurityAdapter.HasStatelessAccess(functionType);
    }

    private IWxeFunctionExecutionListener _executionListener = NullExecutionListener.Null;
    private TransactionStrategyBase? _transactionStrategy;
    private ITransactionMode _transactionMode;
    private readonly WxeVariablesContainer _variablesContainer;
    private readonly WxeExceptionHandler _exceptionHandler = new WxeExceptionHandler();
    private string? _functionToken;
    private string? _returnUrl;
    private string? _executionCompletedScript;

    protected WxeFunction (ITransactionMode transactionMode, params object[] actualParameters)
    {
      ArgumentUtility.CheckNotNull("transactionMode", transactionMode);
      ArgumentUtility.CheckNotNull("actualParameters", actualParameters);

      _transactionMode = transactionMode;
      _variablesContainer = new WxeVariablesContainer(this, actualParameters);
    }

    protected WxeFunction (ITransactionMode transactionMode, WxeParameterDeclaration[] parameterDeclarations, object[] actualParameters)
    {
      ArgumentUtility.CheckNotNull("transactionMode", transactionMode);
      ArgumentUtility.CheckNotNull("parameterDeclarations", parameterDeclarations);
      ArgumentUtility.CheckNotNull("actualParameters", actualParameters);

      _transactionMode = transactionMode;
      _variablesContainer = new WxeVariablesContainer(this, actualParameters, parameterDeclarations);
    }

    public override void Execute (WxeContext context)
    {
      ArgumentUtility.CheckNotNull("context", context);
      Assertion.IsNotNull(_executionListener);

      if (!IsExecutionStarted)
      {
        _variablesContainer.EnsureParametersInitialized(null);
        var wxeSecurityAdapter = GetWxeSecurityAdapter();
        _executionListener = new SecurityExecutionListener(this, _executionListener, wxeSecurityAdapter);

        _transactionStrategy = _transactionMode.CreateTransactionStrategy(this, context);
        Assertion.IsNotNull(_transactionStrategy);

        _executionListener = _transactionStrategy.CreateExecutionListener(_executionListener);
        Assertion.IsNotNull(_executionListener);
      }

      try
      {
        _executionListener.OnExecutionPlay(context);
        base.Execute(context);
        _executionListener.OnExecutionStop(context);
      }
      catch (WxeFatalExecutionException)
      {
        // bubble up
        throw;
      }
      catch (ThreadAbortException)
      {
        _executionListener.OnExecutionPause(context);
        throw;
      }
      catch (Exception stepException)
      {
        try
        {
          _executionListener.OnExecutionFail(context, stepException);
        }
        catch (Exception listenerException)
        {
          throw new WxeFatalExecutionException(stepException, listenerException);
        }

        var unwrappedException = WxeHttpExceptionPreservingException.GetUnwrappedException(stepException) ?? stepException;
        if (!_exceptionHandler.Catch(unwrappedException))
        {
          throw new WxeUnhandledException(
              string.Format("An exception ocured while executing WxeFunction '{0}'.", GetType().GetFullNameSafe()),
              stepException);
        }
      }

      if (_exceptionHandler.Exception == null && ParentStep != null)
        _variablesContainer.ReturnParametersToCaller();
    }

    public ITransactionStrategy Transaction
    {
      get { return TransactionStrategy; }
    }

    protected internal TransactionStrategyBase TransactionStrategy
    {
      get { return _transactionStrategy ?? NullTransactionStrategy.Null; }
    }

    protected ITransactionMode TransactionMode
    {
      get { return _transactionMode; }
    }

    protected void SetTransactionMode (ITransactionMode transactionMode)
    {
      ArgumentUtility.CheckNotNull("transactionMode", transactionMode);

      if (_transactionStrategy != null)
        throw new InvalidOperationException("The TransactionMode cannot be set after the TransactionStrategy has been initialized.");

      _transactionMode = transactionMode;
    }

    protected IWxeFunctionExecutionListener ExecutionListener
    {
      get { return _executionListener; }
    }

    protected void SetExecutionListener (IWxeFunctionExecutionListener executionListener)
    {
      ArgumentUtility.CheckNotNull("executionListener", executionListener);

      if (_transactionStrategy != null)
        throw new InvalidOperationException("The ExecutionListener cannot be set after the TransactionStrategy has been initialized.");

      _executionListener = executionListener;
    }

    object?[] IWxeFunctionExecutionContext.GetInParameters ()
    {
      return _variablesContainer.ParameterDeclarations.Where(p => p.IsIn).Select(p => p.GetValue(_variablesContainer.Variables)).ToArray();
    }

    object?[] IWxeFunctionExecutionContext.GetOutParameters ()
    {
      return _variablesContainer.ParameterDeclarations.Where(p => p.IsOut).Select(p => p.GetValue(_variablesContainer.Variables)).ToArray();
    }

    object[] IWxeFunctionExecutionContext.GetVariables ()
    {
      return _variablesContainer.Variables.Values.Cast<object>().ToArray();
    }

    /// <summary>
    /// Gets or sets the URL the browser will be redirected to after the <see cref="WxeFunction"/> has finished executing.
    /// </summary>
    /// <remarks>
    /// If an <see cref="ExecutionCompletedScript"/> is set on the same <see cref="WxeFunction"/>, the <see cref="ReturnUrl"/> will not be used.
    /// </remarks>
    [CanBeNull]
    public string? ReturnUrl
    {
      get { return _returnUrl; }
      set
      {
        if (value != null && value.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
        {
          throw new ArgumentException(
              "The ReturnUrl cannot be a javascript-URL. Use the WxeFunction.SetExecutionCompletedScript(script) method instead.",
              "value");
        }

        _returnUrl = value;
      }
    }

    [CanBeNull]
    public string? ExecutionCompletedScript
    {
      get { return _executionCompletedScript; }
    }

    /// <summary>
    /// Sets the script that will be executed when the <see cref="WxeFunction"/> as completed the execution.
    /// </summary>
    /// <remarks>The <paramref name="script"/> will supersede any <see cref="ReturnUrl"/> set on the same <see cref="WxeFunction"/>.</remarks>
    public void SetExecutionCompletedScript ([NotNull] string script)
    {
      ArgumentUtility.CheckNotNullOrEmpty("script", script);

      _returnUrl = null;
      _executionCompletedScript = script;
    }

    public override NameObjectCollection Variables
    {
      get { return _variablesContainer.Variables; }
    }

    public WxeVariablesContainer VariablesContainer
    {
      get { return _variablesContainer; }
    }

    public WxeExceptionHandler ExceptionHandler
    {
      get { return _exceptionHandler; }
    }

    public string FunctionToken
    {
      get
      {
        if (_functionToken != null)
          return _functionToken;
        WxeFunction? rootFunction = RootFunction;
        if (rootFunction != null && rootFunction != this)
          return rootFunction.FunctionToken;
        throw new InvalidOperationException(
            "The WxeFunction does not have a RootFunction, i.e. the top-most WxeFunction does not have a FunctionToken.");
      }
    }

    internal void SetFunctionToken (string functionToken)
    {
      ArgumentUtility.CheckNotNullOrEmpty("functionToken", functionToken);
      _functionToken = functionToken;
    }

    public override string ToString ()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("WxeFunction: ");
      sb.Append(GetType().Name);
      sb.Append(" (");
      for (int i = 0; i < _variablesContainer.ActualParameters.Length; ++i)
      {
        if (i > 0)
          sb.Append(", ");
        object value = _variablesContainer.ActualParameters[i];
        if (value is WxeVariableReference)
          sb.Append("@" + ((WxeVariableReference) value).Name);
        else if (value is string)
          sb.AppendFormat("\"{0}\"", value);
        else
          sb.Append(value);
      }
      sb.Append(")");
      return sb.ToString();
    }

    internal static IWxeSecurityAdapter? GetWxeSecurityAdapter ()
    {
      return SafeServiceLocator.Current.GetAllInstances<IWxeSecurityAdapter>()
          .SingleOrDefault(() => new InvalidOperationException("Only a single IWxeSecurityAdapter can be registered."));
    }
  }
}
