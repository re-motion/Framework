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
  /// Implements a get accessor.
  /// </summary>
  public class ImplementingGetAccessorInterceptor : ImplementingAccessorInterceptorBase
  {
    private static readonly MethodInfo s_propertyGetValue =
        MemberInfoFromExpressionUtility.GetGenericMethodDefinition ((PropertyAccessor o) => o.GetValue<object>());

    public ImplementingGetAccessorInterceptor (MethodInfo interceptedAccessorMethod, string propertyName, Type propertyType)
        : base(interceptedAccessorMethod, propertyName, propertyType)
    {
    }

    protected override MethodInfo AccessorImplementationMethod
    {
      get { return s_propertyGetValue; }
    }

    protected override IEnumerable<Expression> GetArguments (MethodBodyModificationContext ctx)
    {
      // PropertyAccessor.GetValue<T> does not take any arguments.
      return new Expression[0];
    }
  }
}