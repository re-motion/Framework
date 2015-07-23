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
using Remotion.TypePipe.Dlr.Ast;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.TypePipe
{
  /// <summary>
  /// Intercepts property accessors by wrapping a simple base call into try-finally blocks.
  /// </summary>
  public class WrappingAccessorInterceptor : IAccessorInterceptor
  {
    private static readonly MethodInfo s_preparePropertyAccess =
        MemberInfoFromExpressionUtility.GetMethod (() => CurrentPropertyManager.PreparePropertyAccess ("propertyName"));
    private static readonly MethodInfo s_propertyAccessFinished =
        MemberInfoFromExpressionUtility.GetMethod (() => CurrentPropertyManager.PropertyAccessFinished ());

    private readonly MethodInfo _interceptedAccessorMethod;
    private readonly string _propertyName;

    public WrappingAccessorInterceptor (MethodInfo interceptedAccessorMethod, string propertyName)
    {
      ArgumentUtility.CheckNotNull ("interceptedAccessorMethod", interceptedAccessorMethod);
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      _interceptedAccessorMethod = interceptedAccessorMethod;
      _propertyName = propertyName;
    }

    public void Intercept (MutableType proxyType)
    {
      ArgumentUtility.CheckNotNull ("proxyType", proxyType);

      proxyType.GetOrAddOverride (_interceptedAccessorMethod).SetBody (ctx => WrapBody (CreateBody (ctx)));
    }

    protected virtual Expression CreateBody (MethodBodyModificationContext ctx)
    {
      return ctx.PreviousBody;
    }

    private Expression WrapBody (Expression body)
    {
      return Expression.Block (
          Expression.Call (s_preparePropertyAccess, Expression.Constant (_propertyName)),
          Expression.TryFinally (
              body,
              Expression.Call (s_propertyAccessFinished)));
    }
  }
}