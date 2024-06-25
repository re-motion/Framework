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
using System.Reflection;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{

/// <summary> This delegate represents a parameterless method called. </summary>
/// <remarks> The method is usually an instance method of a <see cref="WxeStepList"/> or <see cref="WxeFunction"/>. </remarks>
public delegate void WxeMethod ();

/// <summary> This delegate represents a method accepting a <see cref="WxeContext"/> as a parameter. </summary>
/// <remarks> The method is usually an instance method of a <see cref="WxeStepList"/> or <see cref="WxeFunction"/>. </remarks>
public delegate void WxeMethodWithContext (WxeContext context);

/// <summary> Performs a step implemented by an instance method of a <see cref="WxeFunction"/>. </summary>
/// <include file='..\doc\include\ExecutionEngine\WxeMethodStep.xml' path='WxeMethodStep/Class/*' />
public class WxeMethodStep: WxeStep
{
  private static WxeStepList GetTargetFromDelegate (Delegate method)
  {
    var target = method.Target as WxeStepList;
    if (target == null)
    {
      var message = string.Format(
          "The delegate's target must be a non-null WxeStepList, but it was '{0}'. When used within a WxeFunction, the delegate should be a method "
          + "of the surrounding WxeFunction, and it must not be a closure.",
          method.Target != null ? method.Target.GetType().ToString() : "null");
      throw new ArgumentException(message, "method");
    }
    else
      return target;
  }

  private static MethodInfo GetMethodFromDelegate (Delegate method)
  {
    if (method.GetInvocationList().Length != 1)
      throw new ArgumentException("The delegate must contain a single method.", "method");
    else
      return method.Method;
  }

  /// <summary> The <see cref="WxeStepList"/> containing the <b>Method</b> executed by this <b>WxeMethodStep</b>. </summary>
  private WxeStepList _target;
  /// <summary> The name of the method executed by this <b>WxeMethodStep</b>. </summary>
  private string _methodName;
  /// <summary> <see langword="true"/> if the method has a parameter of type <see cref="WxeContext"/>. </summary>
  private bool _hasContext;
  /// <summary> The cached <see cref="WxeMethod"/> delegate used during execution of this <b>WxeMethodStep</b>. </summary>
  private WxeMethod? _method;
  /// <summary> The cached <see cref="WxeMethodWithContext"/> delegate used during execution of this <b>WxeMethodStep</b>. </summary>
  private WxeMethodWithContext? _methodWithContext;

  /// <summary> Initalizes a new instance of the <b>WxeMethodStep</b> type. </summary>
  /// <include file='..\doc\include\ExecutionEngine\WxeMethodStep.xml' path='WxeMethodStep/Ctor/*' />
  public WxeMethodStep (WxeStepList target, MethodInfo method)
  {
    ArgumentUtility.CheckNotNull("target", target);
    ArgumentUtility.CheckNotNull("method", method);

    Type targetType = target.GetType();
    Type declaringType = method.DeclaringType!; // TODO RM-8118: not null assertion

    bool isAssignable = declaringType.IsAssignableFrom(targetType);
    if (! isAssignable || method.IsStatic)
      throw new WxeException("Method step '" + method.Name + "' is not an instance method of the type '" + targetType.GetFullNameSafe() + "'.");

    ParameterInfo[] parameters = method.GetParameters();
    if (parameters.Length > 1)
      throw new WxeException("Method step '" + method.Name + "', declared in type '" + declaringType.GetFullNameSafe() + "', does not support more than one parameter.");
    if (parameters.Length == 1 && ! typeof(WxeContext).IsAssignableFrom(parameters[0].ParameterType))
      throw new WxeException("Method step '" + method.Name + "', declared in type '" + declaringType.GetFullNameSafe() + "', may only have a parameter of type WxeContext.");

    _target = target;
    _methodName = method.Name;
    _hasContext = parameters.Length > 0;
  }

  public WxeMethodStep (Action method)
      : this(
          GetTargetFromDelegate(ArgumentUtility.CheckNotNull("method", method)),
          GetMethodFromDelegate(ArgumentUtility.CheckNotNull("method", method)))
  {
  }

  public WxeMethodStep (Action<WxeContext> method)
      : this(
          GetTargetFromDelegate(ArgumentUtility.CheckNotNull("method", method)),
          GetMethodFromDelegate(ArgumentUtility.CheckNotNull("method", method)))
  {
  }

  //  public WxeMethodStep (WxeMethod method)
  //    : this ((WxeStepList) method.Target, method.Method)
  //  {
  //    _method = method;
  //  }
  //
  //  public WxeMethodStep (WxeMethodWithContext method)
  //    : this ((WxeStepList) method.Target, method.Method)
  //  {
  //    _methodWithContext = method;
  //  }

  /// <summary> Executes the method provided during the initizalion of this <see cref="WxeMethodStep"/>. </summary>
  /// <param name="context"> The <see cref="WxeContext"/> containing the information about the execution. </param>
  public override void Execute (WxeContext context)
  {
    if (_hasContext)
    {
      if (_methodWithContext == null)
      {
        _methodWithContext =
          (WxeMethodWithContext)Delegate.CreateDelegate(typeof(WxeMethodWithContext), _target, _methodName, false);
      }
      _methodWithContext(context);
    }
    else
    {
      if (_method == null)
        _method = (WxeMethod)Delegate.CreateDelegate(typeof(WxeMethod), _target, _methodName, false);
      _method();
    }
  }

  /// <ineritdoc />
  public sealed override bool IsDirtyStateEnabled => base.IsDirtyStateEnabled;
}

}
