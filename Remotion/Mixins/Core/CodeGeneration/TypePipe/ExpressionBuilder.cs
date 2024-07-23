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
using Remotion.Mixins.Context;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.TypePipe.Expressions;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// A helper class for code generation that builds complex <see cref="Expression"/>s.
  /// </summary>
  public class ExpressionBuilder : IExpressionBuilder
  {
    public Expression CreateNewClassContext (ClassContext classContext)
    {
      ArgumentUtility.CheckNotNull("classContext", classContext);

      var serializer = new ExpressionClassContextSerializer();
      classContext.Serialize(serializer);

      return serializer.CreateNewExpression();
    }

    public Expression CreateInitialization (MutableType concreteTarget, MethodInfo initializationMethod)
    {
      ArgumentUtility.CheckNotNull("concreteTarget", concreteTarget);
      ArgumentUtility.CheckNotNull("initializationMethod", initializationMethod);

      // this.__InitializationMethod();
      return Expression.Call(new ThisExpression(concreteTarget), initializationMethod, arguments: Array.Empty<Expression>());
    }

    public Expression CreateInitializingDelegation (
        MethodBodyContextBase ctx, MethodInfo initializationMethod, Expression instance, MethodInfo methodToCall)
    {
      ArgumentUtility.CheckNotNull("ctx", ctx);
      ArgumentUtility.CheckNotNull("initializationMethod", initializationMethod);
      ArgumentUtility.CheckNotNull("instance", instance);
      ArgumentUtility.CheckNotNull("methodToCall", methodToCall);

      // <CreateInitialization>
      // instance<GenericParameters>.MethodToCall(<parameters>);

      return Expression.Block(
          CreateInitialization(ctx.DeclaringType, initializationMethod),
          ctx.DelegateTo(instance, methodToCall));
    }
  }
}
