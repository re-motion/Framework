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
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace Remotion.Reflection.CodeGeneration
{
  public interface IMethodEmitter : IAttributableEmitter
  {
    MethodBuilder MethodBuilder { get; }
    ILGenerator ILGenerator { get; }
    string Name { get; }
    ArgumentReference[] ArgumentReferences { get; }
    Type ReturnType { get; }
    Type[] ParameterTypes { get; }
    
    Expression[] GetArgumentExpressions ();
    IMethodEmitter ImplementByReturning (Expression result);
    IMethodEmitter ImplementByReturningVoid ();
    IMethodEmitter ImplementByReturningDefault ();
    IMethodEmitter ImplementByDelegating (TypeReference implementer, MethodInfo methodToCall);
    IMethodEmitter ImplementByBaseCall (MethodInfo baseMethod);
    IMethodEmitter ImplementByThrowing (Type exceptionType, string message);
    IMethodEmitter AddStatement (Statement statement);
    LocalReference DeclareLocal (Type type);

    void AcceptStatement (Statement statement, ILGenerator generator);
    void AcceptExpression (Expression expression, ILGenerator generator);
  }
}
