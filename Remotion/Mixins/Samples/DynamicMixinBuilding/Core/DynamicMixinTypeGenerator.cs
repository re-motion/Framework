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
using System.Reflection.Emit;
using Castle.DynamicProxy;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Reflection.CodeGeneration;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Mixins.Samples.DynamicMixinBuilding.Core
{
  internal class DynamicMixinTypeGenerator
  {
    private static readonly ConstructorInfo s_attributeConstructor = typeof (OverrideTargetAttribute).GetConstructor (Type.EmptyTypes);
    private static readonly MethodInfo s_handlerInvokeMethod = typeof (MethodInvocationHandler).GetMethod ("Invoke");

    private readonly CustomClassEmitter _emitter;
    private readonly IEnumerable<MethodInfo> _methodsToOverride;
    private readonly MethodInvocationHandler _invocationHandler;
    private readonly FieldReference _invocationHandlerField;
    private readonly BaseRequirements _baseCallInterface;

    public DynamicMixinTypeGenerator (ModuleScope scope, Type targetType, IEnumerable<MethodInfo> methodsToOverride, MethodInvocationHandler invocationHandler)
    {
      ArgumentUtility.CheckNotNull ("scope", scope);
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("methodsToOverride", methodsToOverride);
      ArgumentUtility.CheckNotNull ("invocationHandler", invocationHandler);

      if (targetType.ContainsGenericParameters)
        throw new NotSupportedException ("Open generic target types are not supported by this type generator.");

      _methodsToOverride = methodsToOverride;
      _invocationHandler = invocationHandler;

      string className = "DynamicMixinFor_" + targetType.Name;

      _baseCallInterface = BaseRequirements.BuildBaseRequirements (_methodsToOverride, className + "_BaseRequirements", scope);

      Type mixinBase = typeof (Mixin<,>).MakeGenericType (typeof (object), _baseCallInterface.RequirementsType);
      _emitter = new CustomClassEmitter (scope, className, mixinBase);

      _invocationHandlerField = _emitter.CreateStaticField ("InvocationHandler", typeof (MethodInvocationHandler));

      foreach (MethodInfo method in _methodsToOverride)
        AddOverrider (method, _invocationHandlerField);
    }

    public Type BuildType ()
    {
      Type builtType = _emitter.BuildType ();
      builtType.GetField (_invocationHandlerField.Reference.Name).SetValue (null, _invocationHandler);
      return builtType;
    }

    private void AddOverrider (MethodInfo method, Reference invocationHandler)
    {
      var overrider = _emitter.CreateMethod (method.Name, MethodAttributes.Public | MethodAttributes.Virtual, method);
      overrider.AddCustomAttribute (new CustomAttributeBuilder (s_attributeConstructor, new object[0]));

      Reference targetReference = GetTargetReference ();
      Expression overriddenMethodToken = new MethodTokenExpression (method);
      LocalReference argsLocal = CopyArgumentsToLocalVariable (overrider);
      Expression baseMethodInvoker = GetBaseInvokerExpression (method);

      TypeReference handlerReference = new TypeReferenceWrapper (invocationHandler, typeof (MethodInvocationHandler));
      Expression[] handlerArgs = new Expression[] { targetReference.ToExpression(), overriddenMethodToken, argsLocal.ToExpression (), baseMethodInvoker };

      Expression handlerInvocation = new VirtualMethodInvocationExpression (handlerReference, s_handlerInvokeMethod, handlerArgs);

      if (method.ReturnType != typeof (void))
        overrider.ImplementByReturning (new ConvertExpression (method.ReturnType, typeof (object), handlerInvocation));
      else
      {
        overrider.AddStatement (new ExpressionStatement (handlerInvocation));
        overrider.AddStatement (new PopStatement ());
        overrider.ImplementByReturningVoid ();
      }
    }

    private Expression GetBaseInvokerExpression (MethodInfo method)
    {
      MethodInfo baseCallStub = CreateBaseCallStub (method);
      ConstructorInfo invokerCtor = typeof (BaseMethodInvoker).GetConstructor (new Type[] { typeof (object), typeof (IntPtr) });
      Expression invokerExpression = new NewInstanceExpression (invokerCtor, SelfReference.Self.ToExpression (),
          new LoadFunctionExpression (baseCallStub));
      return invokerExpression;
    }

    private MethodInfo CreateBaseCallStub (MethodInfo methodToBeStubbed)
    {
      var stubMethod = _emitter.CreateMethod (
          methodToBeStubbed.Name + "__stub",
          MethodAttributes.Private,
          typeof (object),
          new[] { typeof (object[]) });

      PropertyReference baseReference = GetNextReference ();
      ParameterInfo[] stubbedMethodSignature = methodToBeStubbed.GetParameters ();
      Expression[] baseCallArgs = new Expression[stubbedMethodSignature.Length];
      for (int i = 0; i < baseCallArgs.Length; ++i)
        baseCallArgs[i] = new ConvertExpression (stubbedMethodSignature[i].ParameterType, typeof (object), new LoadArrayElementExpression (i, stubMethod.ArgumentReferences[0], typeof (object)));

      MethodInfo methodOnBaseInterface = _baseCallInterface.GetBaseCallMethod (methodToBeStubbed);
      Expression baseMethodCall = new VirtualMethodInvocationExpression (baseReference, methodOnBaseInterface, baseCallArgs);
      if (methodToBeStubbed.ReturnType != typeof (void))
        stubMethod.ImplementByReturning (baseMethodCall);
      else
      {
        stubMethod.AddStatement (new ExpressionStatement (baseMethodCall));
        stubMethod.ImplementByReturning (NullExpression.Instance);
      }
      return stubMethod.MethodBuilder;
    }

    private PropertyReference GetTargetReference ()
    {
      PropertyInfo targetProperty = _emitter.BaseType.GetProperty ("Target", BindingFlags.NonPublic | BindingFlags.Instance);
      return new PropertyReference (SelfReference.Self, targetProperty);
    }

    private PropertyReference GetNextReference ()
    {
      PropertyInfo nextProperty = _emitter.BaseType.GetProperty ("Next", BindingFlags.NonPublic | BindingFlags.Instance);
      return new PropertyReference (SelfReference.Self, nextProperty);
    }

    private LocalReference CopyArgumentsToLocalVariable (IMethodEmitter overrider)
    {
      LocalReference argsLocal = overrider.DeclareLocal (typeof (object[]));

      ArgumentReference[] argumentReferences = overrider.ArgumentReferences;
      overrider.AddStatement (new AssignStatement (argsLocal, new NewArrayExpression (argumentReferences.Length, typeof (object))));
      for (int i = 0; i < argumentReferences.Length; ++i)
      {
        Expression castArgument = new ConvertExpression (typeof (object), argumentReferences[i].Type, argumentReferences[i].ToExpression ());
        overrider.AddStatement (new AssignArrayStatement (argsLocal, i, castArgument));
      }
      return argsLocal;
    }
  }
}
