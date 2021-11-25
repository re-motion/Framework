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
using System.Reflection;
using Remotion.Mixins.Definitions;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// Creates the expression tree for <b>next calls</b> on overridden methods.
  /// </summary>
  /// <remarks>
  /// The same next call proxy is used for all mixins, but during instiantiation, the mixin index (= depth) is provided to the proxy. 
  /// The target type's proxy contains an array of all mixins and when a next call is performed, the mixin to call is looked up via the depth value.
  /// So, for a method where mixins #1 and #5 out of 10 override a specific method, for depth 0 mixin #1 is invoked, for depth 1-4, mixin #5 is invoked,
  /// and for the remaining depth-values, the method on the target class is invoked.
  /// <code><![CDATA[
  /// public override void TheTargetClass.TheMethod()
  /// {
  ///   if (this.__depth == 0)
  ///   {
  ///     ((FirstMixin) this.__this.__extensions[0]).TheMethod();
  ///   }
  ///   else if (this.__depth >= 1 && this.__depth <= 4)
  ///   {
  ///     ((FifthMixin) this.__this.__extensions[4]).TheMethod();
  ///   }
  ///   else
  ///   {
  ///     this.__this.__base__TheMethod();
  ///   }
  /// }
  /// ]]></code>
  /// Note that presently, the generated code uses individual branches for each depth-value. The above snippet is a compact version of this semantics.
  /// </remarks>
  public class NextCallMethodGenerator : INextCallMethodGenerator
  {
    private readonly TargetClassDefinition _targetClassDefinition;
    private readonly ITargetTypeForNextCall _targetTypeForNextCall;
    private readonly Expression _thisField;
    private readonly Expression _depthField;
    private readonly IList<IMixinInfo> _mixinInfos;

    public NextCallMethodGenerator (
        TargetClassDefinition targetClassDefinition,
        ITargetTypeForNextCall targetTypeForNextCall,
        Expression thisField,
        Expression depthField,
        IList<IMixinInfo> mixinInfos)
    {
      ArgumentUtility.CheckNotNull("targetClassDefinition", targetClassDefinition);
      ArgumentUtility.CheckNotNull("targetTypeForNextCall", targetTypeForNextCall);
      ArgumentUtility.CheckNotNull("thisField", thisField);
      ArgumentUtility.CheckNotNull("depthField", depthField);
      ArgumentUtility.CheckNotNull("mixinInfos", mixinInfos);

      _targetClassDefinition = targetClassDefinition;
      _targetTypeForNextCall = targetTypeForNextCall;
      _thisField = thisField;
      _depthField = depthField;
      _mixinInfos = mixinInfos;
    }

    public Expression CreateBaseCallToNextInChain (MethodBodyContextBase ctx, MethodDefinition methodDefinitionOnTarget)
    {
      Assertion.IsTrue(methodDefinitionOnTarget.DeclaringClass == _targetClassDefinition);

      var expressions = new List<Expression>();

      var returnLabel = Expression.Label(ctx.ReturnType);

      for (int potentialDepth = 0; potentialDepth < _targetClassDefinition.Mixins.Count; ++potentialDepth)
      {
        var nextInChain = GetNextInChain(methodDefinitionOnTarget, potentialDepth);
        var baseCallIfDepthMatches = AddBaseCallToTargetIfDepthMatches(ctx, nextInChain, potentialDepth, returnLabel);
        expressions.Add(baseCallIfDepthMatches);
      }

      var baseCall = CreateBaseCallToTarget(ctx, methodDefinitionOnTarget);
      expressions.Add(Expression.Label(returnLabel, baseCall));
      return Expression.Block(expressions);
    }

    private MethodDefinition GetNextInChain (MethodDefinition methodDefinitionOnTarget, int potentialDepth)
    {
      Assertion.IsTrue(methodDefinitionOnTarget.DeclaringClass == _targetClassDefinition);

      for (int i = potentialDepth; i < _targetClassDefinition.Mixins.Count; ++i)
        if (methodDefinitionOnTarget.Overrides.ContainsKey(_targetClassDefinition.Mixins[i].Type))
          return methodDefinitionOnTarget.Overrides[_targetClassDefinition.Mixins[i].Type];
      return methodDefinitionOnTarget;
    }

    private Expression AddBaseCallToTargetIfDepthMatches (MethodBodyContextBase ctx, MethodDefinition target, int requestedDepth, LabelTarget returnLabel)
    {
      return Expression.IfThen(
              Expression.Equal(_depthField, Expression.Constant(requestedDepth)),
              Expression.Return(returnLabel, CreateBaseCallStatement(ctx, target)));
    }

    public Expression CreateBaseCallToTarget (MethodBodyContextBase ctx, MethodDefinition target)
    {
      return CreateBaseCallStatement(ctx, target);
    }

    private Expression CreateBaseCallStatement (MethodBodyContextBase ctx, MethodDefinition target)
    {
      if (target.DeclaringClass == _targetClassDefinition)
        return CreateBaseCallToTargetClassStatement(ctx, target);
      else
        return CreateBaseCallToMixinStatement(ctx, target);
    }

    private Expression CreateBaseCallToTargetClassStatement (MethodBodyContextBase ctx, MethodDefinition target)
    {
      var baseCallMethod = _targetTypeForNextCall.GetBaseCallMethod(target.MethodInfo);

      return ctx.DelegateTo(_thisField, baseCallMethod);
    }

    private Expression CreateBaseCallToMixinStatement (MethodBodyContextBase ctx, MethodDefinition target)
    {
      var mixin = (MixinDefinition) target.DeclaringClass;
      var baseCallMethod = GetMixinMethodToCall(mixin.MixinIndex, target);
      var mixinReference = GetMixinReference(mixin, baseCallMethod.DeclaringType!);

      return ctx.DelegateTo(mixinReference, baseCallMethod);
    }

    private MethodInfo GetMixinMethodToCall (int mixinIndex, MethodDefinition mixinMethod)
    {
      return _mixinInfos[mixinIndex].GetPubliclyCallableMixinMethod(mixinMethod.MethodInfo);
    }

    private Expression GetMixinReference (MixinDefinition mixin, Type concreteMixinType)
    {
      // (ConcreteMixinType) __this.__extensions[mixin.MixinIndex]

      return Expression.Convert(
          Expression.ArrayAccess(
              Expression.Field(_thisField, _targetTypeForNextCall.ExtensionsField),
              Expression.Constant(mixin.MixinIndex)),
          concreteMixinType);
    }
  }
}
