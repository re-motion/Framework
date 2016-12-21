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
using System.Reflection.Emit;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration
{
  /// <summary>
  /// Builds the IL code needed to wrap a method call.
  /// </summary>
  public class MethodWrapperEmitter
  {
    private readonly ILGenerator _ilGenerator;
    private readonly MethodInfo _wrappedMethod;
    private readonly Type[] _wrapperParameterTypes;
    private readonly Type _wrapperReturnType;

    public MethodWrapperEmitter (ILGenerator ilGenerator, MethodInfo wrappedMethod, Type[] wrapperParameterTypes, Type wrapperReturnType)
    {
      ArgumentUtility.CheckNotNull ("ilGenerator", ilGenerator);
      ArgumentUtility.CheckNotNull ("wrappedMethod", wrappedMethod);
      ArgumentUtility.CheckNotNullOrItemsNull ("wrapperParameterTypes", wrapperParameterTypes);
      ArgumentUtility.CheckNotNull ("wrapperReturnType", wrapperReturnType);
      if (wrappedMethod.ContainsGenericParameters)
        throw new ArgumentException ("Open generic method definitions are not supported by the MethodWrapperGenerator.", "wrappedMethod");
      CheckParameterCount (wrappedMethod, wrapperParameterTypes);
      CheckInstanceParameterType (wrappedMethod, wrapperParameterTypes);
      CheckParameterTypes (wrappedMethod, wrapperParameterTypes);
      CheckReturnTypes (wrappedMethod, wrapperReturnType);

      _ilGenerator = ilGenerator;
      _wrappedMethod = wrappedMethod;
      _wrapperParameterTypes = wrapperParameterTypes;
      _wrapperReturnType = wrapperReturnType;
    }

    public void EmitStaticMethodBody ()
    {
      EmitInstanceArgument ();
      EmitMethodArguments ();
      EmitMethodCall ();
      EmitReturnStatement ();
    }

    private void CheckParameterCount (MethodInfo wrappedMethod, Type[] wrapperParameterTypes)
    {
      if (wrapperParameterTypes.Length != wrappedMethod.GetParameters().Length + 1)
      {
        throw new ArgumentException (
            string.Format (
                "The number of elements in the wrapperParameterTypes array ({0}) does not match the number of parameters required for invoking the wrappedMethod ({1}).",
                wrapperParameterTypes.Length,
                wrappedMethod.GetParameters().Length + 1),
            "wrapperParameterTypes");
      }
    }

    private void CheckInstanceParameterType (MethodInfo wrappedMethod, Type[] wrapperParameterTypes)
    {
      if (!wrapperParameterTypes[0].IsAssignableFrom (wrappedMethod.DeclaringType))
      {
        throw new ArgumentException (
            string.Format (
                "The wrapperParameterType #0 ('{0}') cannot be assigned to the declaring type ('{1}') of the wrappedMethod.",
                wrapperParameterTypes[0].Name,
                wrappedMethod.DeclaringType.Name),
            "wrapperParameterTypes");
      }
    }

    private void CheckParameterTypes (MethodInfo wrappedMethod, Type[] wrapperParameterTypes)
    {
      foreach (var wrappedParameter in wrappedMethod.GetParameters())
      {
        var wrapperParameterType = wrapperParameterTypes[wrappedParameter.Position + 1];
        if (!wrapperParameterType.IsAssignableFrom (wrappedParameter.ParameterType))
        {
          throw new ArgumentException (
              string.Format (
                  "The wrapperParameterType #{1} ('{0}') cannot be assigned to the type ('{2}') of parameter '{3}' of the wrappedMethod.",
                  wrapperParameterType.Name,
                  wrappedParameter.Position + 1,
                  wrappedParameter.ParameterType.Name,
                  wrappedParameter.Name),
              "wrapperParameterTypes");
        }

        if (wrappedParameter.IsOut)
        {
          throw new ArgumentException (
              string.Format (
                  "Parameter '{0}' of the wrappedMethod is an out parameter, but out parameters are not supported by the MethodWrapperGenerator.",
                  wrappedParameter.Name),
              "wrappedMethod");
        }
        
        if (wrappedParameter.ParameterType.IsByRef)
        {
          throw new ArgumentException (
              string.Format (
                  "Parameter '{0}' of the wrappedMethod is a by-ref parameter, but by-ref parameters are not supported by the MethodWrapperGenerator.",
                  wrappedParameter.Name),
              "wrappedMethod");
        }

        if (wrappedParameter.IsOptional)
        {
          throw new ArgumentException (
              string.Format (
                  "Parameter '{0}' of the wrappedMethod is an optional parameter, but optional parameters are not supported by the MethodWrapperGenerator.",
                  wrappedParameter.Name),
              "wrappedMethod");
        }
      }
    }

    private void CheckReturnTypes (MethodInfo wrappedMethod, Type wrapperReturnType)
    {
      if (!wrapperReturnType.IsAssignableFrom (wrappedMethod.ReturnType))
      {
        throw new ArgumentException (
            string.Format (
                "The wrapperReturnType ('{0}') cannot be assigned from the return type ('{1}') of the wrappedMethod.",
                wrapperReturnType.Name,
                wrappedMethod.ReturnType.Name),
            "wrapperReturnType");
      }
    }

    private void EmitInstanceArgument ()
    {
      if (_wrappedMethod.IsStatic)
        return;

      _ilGenerator.Emit (OpCodes.Ldarg_0);

      if (_wrappedMethod.DeclaringType.IsValueType)
      {
        _ilGenerator.DeclareLocal (_wrappedMethod.DeclaringType);
        if (_wrapperParameterTypes[0] != _wrappedMethod.DeclaringType)
          _ilGenerator.Emit (OpCodes.Unbox_Any, _wrappedMethod.DeclaringType);
        _ilGenerator.Emit (OpCodes.Stloc_0);
        _ilGenerator.Emit (OpCodes.Ldloca_S, 0);
      }
      else
        _ilGenerator.Emit (OpCodes.Castclass, _wrappedMethod.DeclaringType);
    }

    private void EmitMethodArguments ()
    {
      foreach (var parameter in _wrappedMethod.GetParameters())
      {
        var parameterIndex = parameter.Position + 1;

        _ilGenerator.Emit (OpCodes.Ldarg, (ushort) parameterIndex);

        if (_wrapperParameterTypes[parameterIndex] == parameter.ParameterType)
          _ilGenerator.Emit (OpCodes.Nop);
        else if (parameter.ParameterType.IsValueType)
          _ilGenerator.Emit (OpCodes.Unbox_Any, parameter.ParameterType);
        else
          _ilGenerator.Emit (OpCodes.Castclass, parameter.ParameterType);
      }
    }

    private void EmitMethodCall ()
    {
      if (_wrappedMethod.IsVirtual)
        _ilGenerator.Emit (OpCodes.Callvirt, _wrappedMethod);
      else
        _ilGenerator.Emit (OpCodes.Call, _wrappedMethod);
    }

    private void EmitReturnStatement ()
    {
      if (_wrapperReturnType == typeof (void))
        _ilGenerator.Emit (OpCodes.Nop);
      else if (_wrapperReturnType == _wrappedMethod.ReturnType)
        _ilGenerator.Emit (OpCodes.Nop);
      else if (_wrappedMethod.ReturnType.IsValueType)
        _ilGenerator.Emit (OpCodes.Box, _wrappedMethod.ReturnType);

      _ilGenerator.Emit (OpCodes.Ret);
    }
  }
}