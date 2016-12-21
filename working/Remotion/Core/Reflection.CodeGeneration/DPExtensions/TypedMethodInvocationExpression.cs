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
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  /// <summary>
  /// Replacement for <see cref="MethodInvocationExpression"/> with value type support.
  /// </summary>
  public class TypedMethodInvocationExpression : Expression
  {
    private readonly TypeReference _callTarget;
    private readonly MethodInfo _method;
    private readonly Expression[] _arguments;

    public TypedMethodInvocationExpression (TypeReference callTarget, MethodInfo method, params Expression[] arguments)
    {
      ArgumentUtility.CheckNotNull ("callTarget", callTarget);
      ArgumentUtility.CheckNotNull ("method", method);
      ArgumentUtility.CheckNotNull ("arguments", arguments);

      _callTarget = callTarget;
      _method = method;
      _arguments = arguments;
    }

    public MethodInfo Method
    {
      get { return _method; }
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      LoadOwnersRecursively (_callTarget.OwnerReference, gen);

      if (_callTarget.Type.IsValueType)
        _callTarget.LoadAddressOfReference (gen);
      else
        _callTarget.LoadReference (gen);

      foreach (Expression argument in _arguments)
        argument.Emit (member, gen);

      EmitCall (member, gen);
    }

    private void LoadOwnersRecursively (Reference owner, ILGenerator gen)
    {
      if (owner != null)
      {
        LoadOwnersRecursively (owner.OwnerReference, gen);
        owner.LoadReference (gen);
      }
    }

    protected virtual void EmitCall (IMemberEmitter member, ILGenerator gen)
    {
      gen.Emit (OpCodes.Call, _method);
    }
  }
}
