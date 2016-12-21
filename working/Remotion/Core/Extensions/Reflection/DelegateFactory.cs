﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Linq.Expressions;
using System.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Reflection
{
  /// <summary>
  /// Extracts signatures from delegate types and uses <see cref="LambdaExpression"/> to create delegates enabling their efficient invocation.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [ImplementationFor (typeof (IDelegateFactory))]
  public class DelegateFactory : IDelegateFactory
  {
    public DelegateFactory ()
    {
    }

    public Tuple<Type[], Type> GetSignature (Type delegateType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("delegateType", delegateType, typeof (Delegate));

      var invokeMethod = delegateType.GetMethod ("Invoke");
      Assertion.IsNotNull (invokeMethod, "Delegate has no Invoke() method.");

      var parameterTypes = invokeMethod.GetParameters().Select (p => p.ParameterType).ToArray();
      var returnType = invokeMethod.ReturnType;

      return Tuple.Create (parameterTypes, returnType);
    }

    public Delegate CreateConstructorCall (ConstructorInfo constructor, Type delegateType)
    {
      ArgumentUtility.CheckNotNull ("constructor", constructor);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("delegateType", delegateType, typeof (Delegate));

      var parameters = constructor.GetParameters().Select (p => Expression.Parameter (p.ParameterType, p.Name)).ToArray();
      var constructorCall = Expression.New (constructor, parameters.Cast<Expression>());
      return CreateConvertedDelegate(delegateType, constructorCall, parameters);
    }

    public Delegate CreateDefaultConstructorCall (Type constructedType, Type delegateType)
    {
      ArgumentUtility.CheckNotNull ("constructedType", constructedType);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("delegateType", delegateType, typeof (Delegate));

      var constructorCall = Expression.New (constructedType);
      return CreateConvertedDelegate (delegateType, constructorCall);
    }

    private Delegate CreateConvertedDelegate (Type delegateType, NewExpression constructorCall, ParameterExpression[] parameters = null)
    {
      var returnType = GetSignature (delegateType).Item2;
      var boxedConstructorCall = Expression.Convert (constructorCall, returnType);
      var lambda = Expression.Lambda (delegateType, boxedConstructorCall, parameters);

      return lambda.Compile();
    }
  }
}