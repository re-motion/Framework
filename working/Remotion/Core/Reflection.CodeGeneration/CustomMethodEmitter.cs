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
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration
{
  public class CustomMethodEmitter : IMethodEmitter
  {
    private readonly MethodEmitter _innerEmitter;
    private readonly CustomClassEmitter _declaringType;
    private readonly string _name;

    private readonly Type[] _parameterTypes;

    public CustomMethodEmitter (CustomClassEmitter declaringType, string name, MethodAttributes attributes, Type returnType, Type[] parameterTypes)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("attributes", attributes);
      ArgumentUtility.CheckNotNull ("returnType", returnType);
      ArgumentUtility.CheckNotNull ("parameterTypes", parameterTypes);

      MethodEmitter innerEmitter = declaringType.InnerEmitter.CreateMethod (name, attributes, returnType, parameterTypes);

      _innerEmitter = innerEmitter;
      _declaringType = declaringType;
      _name = name;
      _parameterTypes = parameterTypes;
    }

    public CustomMethodEmitter (CustomClassEmitter declaringType, string name, MethodAttributes attributes, MethodInfo methodToUseAsATemplate)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);
      ArgumentUtility.CheckNotNull ("attributes", attributes);
      ArgumentUtility.CheckNotNull ("methodToUseAsATemplate", methodToUseAsATemplate);

      MethodEmitter innerEmitter = declaringType.InnerEmitter.CreateMethod (name, attributes, methodToUseAsATemplate);

      _innerEmitter = innerEmitter;
      _declaringType = declaringType;
      _name = name;
      _parameterTypes = _innerEmitter.Arguments.Select (a => a.Type).ToArray ();
    }

    public MethodBuilder MethodBuilder
    {
      get { return _innerEmitter.MethodBuilder; }
    }

    internal MethodEmitter InnerEmitter
    {
      get { return _innerEmitter; }
    }

    public ILGenerator ILGenerator
    {
      get { return _innerEmitter.CodeBuilder.Generator; }
    }

    public string Name
    {
      get { return _name; }
    }

    public ArgumentReference[] ArgumentReferences
    {
      get { return InnerEmitter.Arguments; }
    }

    public Type ReturnType
    {
      get { return _innerEmitter.ReturnType; }
    }

    public Type[] ParameterTypes
    {
      get { return _parameterTypes; }
    }

    public Expression[] GetArgumentExpressions ()
    {
      Expression[] argumentExpressions = new Expression[ArgumentReferences.Length];
      for (int i = 0; i < argumentExpressions.Length; ++i)
        argumentExpressions[i] = ArgumentReferences[i].ToExpression ();
      return argumentExpressions;
    }

    public IMethodEmitter ImplementByReturning (Expression result)
    {
      ArgumentUtility.CheckNotNull ("result", result);
      return AddStatement (new ReturnStatement (result));
    }

    public IMethodEmitter ImplementByReturningVoid ()
    {
      return AddStatement (new ReturnStatement ());
    }

    public IMethodEmitter ImplementByReturningDefault ()
    {
      if (ReturnType == typeof (void))
        return ImplementByReturningVoid();
      else
        return ImplementByReturning (new InitObjectExpression (this, ReturnType));
    }

    public IMethodEmitter ImplementByDelegating (TypeReference implementer, MethodInfo methodToCall)
    {
      AddDelegatingCallStatements (methodToCall, implementer, true);
      return this;
    }

    public IMethodEmitter ImplementByBaseCall (MethodInfo baseMethod)
    {
      ArgumentUtility.CheckNotNull ("baseMethod", baseMethod);

      if (baseMethod.IsAbstract)
        throw new ArgumentException (string.Format ("The given method {0}.{1} is abstract.", baseMethod.DeclaringType.FullName, baseMethod.Name),
            "baseMethod");
      
      AddDelegatingCallStatements (baseMethod, new TypeReferenceWrapper (SelfReference.Self, _declaringType.TypeBuilder), false);
      return this;
    }

    private void AddDelegatingCallStatements (MethodInfo methodToCall, TypeReference owner, bool callVirtual)
    {
      Expression[] argumentExpressions = GetArgumentExpressions();

      TypedMethodInvocationExpression delegatingCall;
      if (callVirtual)
        delegatingCall = new AutomaticMethodInvocationExpression (owner, methodToCall, argumentExpressions);
      else
        delegatingCall = new TypedMethodInvocationExpression (owner, methodToCall, argumentExpressions);

      AddStatement (new ReturnStatement (delegatingCall));
    }

    public IMethodEmitter ImplementByThrowing (Type exceptionType, string message)
    {
      ArgumentUtility.CheckNotNull ("exceptionType", exceptionType);
      ArgumentUtility.CheckNotNull ("message", message);
      AddStatement (new ThrowStatement (exceptionType, message));
      return this;
    }

    public IMethodEmitter AddStatement (Statement statement)
    {
      ArgumentUtility.CheckNotNull ("statement", statement);
      _innerEmitter.CodeBuilder.AddStatement (statement);
      return this;
    }

    public LocalReference DeclareLocal (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return _innerEmitter.CodeBuilder.DeclareLocal (type);
    }

    public void AddCustomAttribute (CustomAttributeBuilder customAttribute)
    {
      ArgumentUtility.CheckNotNull ("customAttribute", customAttribute);
      _innerEmitter.MethodBuilder.SetCustomAttribute (customAttribute);
    }

    void IMethodEmitter.AcceptStatement (Statement statement, ILGenerator gen)
    {
      ArgumentUtility.CheckNotNull ("statement", statement);
      ArgumentUtility.CheckNotNull ("gen", gen);

      statement.Emit (_innerEmitter, gen);
    }

    void IMethodEmitter.AcceptExpression (Expression expression, ILGenerator gen)
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("gen", gen);

      expression.Emit (_innerEmitter, gen);
    }
  }
}
