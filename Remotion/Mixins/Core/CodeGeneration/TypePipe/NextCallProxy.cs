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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Remotion.Mixins.Definitions;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// Encapsulates the implementation details of the "next call proxy" concept.
  /// </summary>
  public class NextCallProxy : INextCallProxy
  {
    private readonly MutableType _type;
    private readonly ConstructorInfo _constructor;
    private readonly TargetClassDefinition _targetClassDefinition;
    private readonly INextCallMethodGenerator _nextCallMethodGenerator;
    private readonly Dictionary<MethodDefinition, MethodInfo> _overriddenMethodToImplementationMap = new Dictionary<MethodDefinition, MethodInfo>();

    public NextCallProxy (
        MutableType type,
        MutableConstructorInfo constructor,
        TargetClassDefinition targetClassDefinition,
        INextCallMethodGenerator nextCallMethodGenerator)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("constructor", constructor);
      ArgumentUtility.CheckNotNull ("targetClassDefinition", targetClassDefinition);
      ArgumentUtility.CheckNotNull ("nextCallMethodGenerator", nextCallMethodGenerator);

      _type = type;
      _constructor = constructor;
      _targetClassDefinition = targetClassDefinition;
      _nextCallMethodGenerator = nextCallMethodGenerator;
    }

    public Type Type
    {
      get { return _type; }
    }

    public Expression CallConstructor (Expression target, Expression depth)
    {
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("depth", depth);

      return Expression.New (_constructor, target, depth);
    }

    public MethodInfo GetProxyMethodForOverriddenMethod (MethodDefinition method)
    {
      Assertion.IsTrue (_overriddenMethodToImplementationMap.ContainsKey (method),
                        "The method " + method.Name + " must be registered with the NextCallProxyGenerator.");
      return _overriddenMethodToImplementationMap[method];
    }

    public void ImplementBaseCallsForOverriddenMethodsOnTarget ()
    {
      foreach (var method in _targetClassDefinition.GetAllMethods().Where (method => method.Overrides.Count > 0))
        ImplementBaseCallForOverridenMethodOnTarget (method);
    }

    private void ImplementBaseCallForOverridenMethodOnTarget (MethodDefinition methodDefinitionOnTarget)
    {
      Assertion.IsTrue (methodDefinitionOnTarget.DeclaringClass == _targetClassDefinition);

      var attributes = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual;
      var md = MethodDeclaration.CreateEquivalent (methodDefinitionOnTarget.MethodInfo);
      var methodOverride = _type.AddMethod (
          methodDefinitionOnTarget.FullName,
          attributes,
          md,
          ctx => _nextCallMethodGenerator.CreateBaseCallToNextInChain (ctx, methodDefinitionOnTarget));

      _overriddenMethodToImplementationMap.Add (methodDefinitionOnTarget, methodOverride);

      // If the base type of the emitter (object) already has the method being overridden (ToString, Equals, etc.), mixins could use the base 
      // implementation of the method rather than coming via the next call interface. Therefore, we need to override that base method and point it
      // towards our next call above.
      Assertion.IsTrue (
          _type.BaseType == typeof (object),
          "This code assumes that only non-generic methods could match on the base type, which holds for object.");
      // Since object has no generic methods, we can use the exact parameter types to find the equivalent method.
      var equivalentMethodOnProxyBase = _type.BaseType.GetMethod (
          methodDefinitionOnTarget.Name,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
          null,
          methodOverride.GetParameters ().Select (p => p.ParameterType).ToArray (),
          null);
      if (equivalentMethodOnProxyBase != null && equivalentMethodOnProxyBase.IsVirtual)
      {
        _type.GetOrAddOverride (equivalentMethodOnProxyBase)
             .SetBody (ctx => ctx.DelegateTo (ctx.This, methodOverride));
      }
    }

    public void ImplementBaseCallsForRequirements ()
    {
      foreach (var requiredType in _targetClassDefinition.RequiredNextCallTypes)
        foreach (var requiredMethod in requiredType.Methods)
          ImplementBaseCallForRequirement (requiredMethod);
    }

    private void ImplementBaseCallForRequirement (RequiredMethodDefinition requiredMethod)
    {
      if (requiredMethod.ImplementingMethod.DeclaringClass == _targetClassDefinition)
        ImplementBaseCallForRequirementOnTarget (requiredMethod);
      else
        ImplementBaseCallForRequirementOnMixin (requiredMethod);
    }

    // Required base call method implemented by "this" -> either overridden or not
    // If overridden, delegate to next in chain, else simply delegate to "this" field
    private void ImplementBaseCallForRequirementOnTarget (RequiredMethodDefinition requiredMethod)
    {
      var methodImplementation = _type.AddExplicitOverride (requiredMethod.InterfaceMethod, ctx => Expression.Default (ctx.ReturnType));
      if (requiredMethod.ImplementingMethod.Overrides.Count == 0) // this is not an overridden method, call method directly on _this
      {
        methodImplementation.SetBody (ctx => _nextCallMethodGenerator.CreateBaseCallToTarget (ctx, requiredMethod.ImplementingMethod));
      }
      else // this is an override, go to next in chain
      {
        // a base call for this might already have been implemented as an overriden method, but we explicitly implement the call chains anyway: it's
        // slightly easier and better for performance
        Assertion.IsFalse (_targetClassDefinition.Methods.ContainsKey (requiredMethod.InterfaceMethod));
        methodImplementation.SetBody (ctx => _nextCallMethodGenerator.CreateBaseCallToNextInChain (ctx, requiredMethod.ImplementingMethod));
      }
    }

    // Required base call method implemented by extension -> either as an overridde or not
    // If an overridde, delegate to next in chain, else simply delegate to the extension implementing it field
    private void ImplementBaseCallForRequirementOnMixin (RequiredMethodDefinition requiredMethod)
    {
      var methodImplementation = _type.AddExplicitOverride (requiredMethod.InterfaceMethod, ctx => Expression.Default (ctx.ReturnType));
      if (requiredMethod.ImplementingMethod.Base == null) // this is not an override, call method directly on extension
      {
        methodImplementation.SetBody (ctx => _nextCallMethodGenerator.CreateBaseCallToTarget (ctx, requiredMethod.ImplementingMethod));
      }
      else // this is an override, go to next in chain
      {
        // a base call for this has already been implemented as an overriden method, but we explicitly implement the call chains anyway: it's
        // slightly easier and better for performance
        Assertion.IsTrue (_overriddenMethodToImplementationMap.ContainsKey (requiredMethod.ImplementingMethod.Base));
        methodImplementation.SetBody (ctx => _nextCallMethodGenerator.CreateBaseCallToNextInChain (ctx, requiredMethod.ImplementingMethod.Base));
      }
    }
  }
}