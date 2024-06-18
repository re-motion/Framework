// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Remotion.Mixins.XRef.Utility
{
  public class FastMemberInvokerGenerator
  {
    public Func<object, object[], object> GetFastMethodInvoker (Type declaringType, string memberName, Type[] typeParameters, Type[] argumentTypes, BindingFlags bindingFlags)
    {
      Utilities.ArgumentUtility.CheckNotNull("declaringType", declaringType);
      Utilities.ArgumentUtility.CheckNotNull("memberName", memberName);
      Utilities.ArgumentUtility.CheckNotNull("typeParameters", typeParameters);
      Utilities.ArgumentUtility.CheckNotNull("argumentTypes", argumentTypes);

      var overloads = (MethodBase[])declaringType.GetMember(memberName, MemberTypes.Method, bindingFlags);

      if (overloads.Length == 0)
        throw new MissingMethodException(string.Format("Method '{0}' not found on type '{1}'.", memberName, declaringType));

      MethodInfo method;
      if (typeParameters.Length > 0)
      {
        var methods = overloads.Where(m => m.GetGenericArguments().Length == typeParameters.Length)
            .Select(m => ((MethodInfo)m).MakeGenericMethod(typeParameters));

        method = methods.SingleOrDefault(m => m.GetParameters().Select(p => p.ParameterType).SequenceEqual(argumentTypes));
        if (method == null)
          throw new MissingMethodException(string.Format("Generic overload of method '{0}`{1}' not found on type '{2}'.", memberName, typeParameters.Length, declaringType));
      }
      else
      {
        method = (MethodInfo)Type.DefaultBinder.SelectMethod(bindingFlags, overloads, argumentTypes, null);
        if (method == null)
          throw new MissingMethodException(string.Format("Overload of method '{0}' not found on type '{1}'.", memberName, declaringType));
      }

      return CreateDelegateForMethod(method);
    }

    private Func<object, object[], object> CreateDelegateForMethod (MethodInfo methodInfo)
    {
      if (methodInfo.ReturnType == typeof(void))
        throw new NotSupportedException("Void methods are not supported.");

      var instanceParameter = Expression.Parameter(typeof(object), "instance");
      var argsParameter = Expression.Parameter(typeof(object[]), "args");

      var extractedParameters = from parameterInfo in methodInfo.GetParameters()
          let arrayElementExpression = Expression.ArrayIndex(argsParameter, Expression.Constant(parameterInfo.Position))
          select (Expression)Expression.Convert(arrayElementExpression, parameterInfo.ParameterType);
      var instanceType = methodInfo.IsStatic ? null : Expression.Convert(instanceParameter, methodInfo.DeclaringType);
      var callExpression = Expression.Call(instanceType, methodInfo, extractedParameters);
      var convertedCallResult = Expression.Convert(callExpression, typeof(object));

      var lambda = Expression.Lambda<Func<object, object[], object>>(convertedCallResult, instanceParameter, argsParameter);
      return lambda.Compile();
    }
  }
}
