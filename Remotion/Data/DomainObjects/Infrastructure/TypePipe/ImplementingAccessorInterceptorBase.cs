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
using Remotion.TypePipe.Dlr.Ast;
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.TypePipe
{
  /// <summary>
  /// Serves as a base class for <see cref="IAccessorInterceptor"/>s that implement the accessor by calling a method on the
  /// <see cref="PropertyIndexer"/> class, i.e., without calling the base implementation and wrapping them into try-finally blocks.
  /// </summary>
  public abstract class ImplementingAccessorInterceptorBase : WrappingAccessorInterceptor
  {
    private static readonly PropertyInfo s_properties = Assertion.IsNotNull (
        typeof (DomainObject).GetProperty ("Properties", BindingFlags.Instance | BindingFlags.NonPublic),
        "DomainObject.Properties was not found.");
    private static readonly MethodInfo s_getPropertyAccessor = MemberInfoFromExpressionUtility.GetMethod ((PropertyIndexer i) => i["propertyName"]);

    private readonly string _propertyName;
    private readonly Type _propertyType;

    protected ImplementingAccessorInterceptorBase (MethodInfo interceptedAccessorMethod, string propertyName, Type propertyType)
        : base (interceptedAccessorMethod, propertyName)
    {
      ArgumentUtility.CheckNotNull ("propertyType", propertyType);

      _propertyName = propertyName;
      _propertyType = propertyType;
    }

    protected override Expression CreateBody (MethodBodyModificationContext ctx)
    {
      var propertyIndexer = Expression.Property (ctx.This, s_properties);
      var propertyAccessor = Expression.Call (propertyIndexer, s_getPropertyAccessor, Expression.Constant (_propertyName));
      var body = Expression.Call (propertyAccessor, AccessorImplementationMethod.MakeGenericMethod (_propertyType), GetArguments (ctx));

      return body;
    }

    protected abstract MethodInfo AccessorImplementationMethod { get; }
    protected abstract IEnumerable<Expression> GetArguments (MethodBodyModificationContext ctx);
  }
}