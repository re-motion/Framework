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
using System.Linq;
using System.Reflection;
using Remotion.Mixins.CodeGeneration.Serialization;
using Remotion.Reflection;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// A <see cref="IConcreteMixinTypeIdentifierSerializer"/> that builds expressions that represent the creation of an equivalent 
  /// <see cref="ConcreteMixinTypeIdentifier"/> from constant values.
  /// </summary>
  public class ExpressionConcreteMixinTypeIdentifierSerializer : ConcreteMixinTypeIdentifierSerializerBase
  {
    private static readonly ConstructorInfo s_constructor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new ConcreteMixinTypeIdentifier (null, null, null));

    private static readonly ConstructorInfo s_hashSetConstructor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new HashSet<MethodInfo> (new MethodInfo[0]));

    private static readonly MethodInfo s_resolveMethodMethod =
        MemberInfoFromExpressionUtility.GetMethod (() => MethodResolver.ResolveMethod (null, null, null));

    public Expression CreateExpression ()
    {
      // new ConcreteMixinTypeIdentifier (
      //     MixinType,
      //     new HashSet<MethodInfo> (Overriders)
      //     new HashSet<MethodInfo> (Overridden));

      // Unfortunately, there is a CLR bug that prevents us from simply emitting Expression.Constant (methodInfo): When a method is open generic, 
      // Reflection.Emit will bind the generic type parameter to the outer method (even if that method isn't even generic).
      // Therefore, we need to use MethodResolver, which resolves methods by declaring type, name, and signature.
      // When that bug is fixed, we could use the following code:
      //  return Expression.New (
      //      s_constructor,
      //      Expression.Constant (MixinType),
      //      Expression.New (s_hashSetConstructor, Expression.ArrayConstant (Overriders)),
      //      Expression.New (s_hashSetConstructor, Expression.ArrayConstant (Overridden)));

      return Expression.New (
          s_constructor,
          Expression.Constant (MixinType),
          Expression.New (s_hashSetConstructor, Expression.NewArrayInit (typeof (MethodInfo), Overriders.Select (GetResolveMethodExpression))),
          Expression.New (s_hashSetConstructor, Expression.NewArrayInit (typeof (MethodInfo), Overridden.Select (GetResolveMethodExpression))));
    }

    private Expression GetResolveMethodExpression (MethodInfo methodInfo)
    {
      return Expression.Call (
          s_resolveMethodMethod,
          Expression.Constant (methodInfo.DeclaringType),
          Expression.Constant (methodInfo.Name),
          Expression.Constant (methodInfo.ToString ()));
    }
  }
}