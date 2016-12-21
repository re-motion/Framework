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
using Remotion.Mixins.Context.Serialization;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// A <see cref="IMixinContextOriginSerializer"/> that builds expressions that represent the creation of an equivalent 
  /// <see cref="MixinContextOrigin"/> from constant values.
  /// </summary>
  public class ExpressionMixinContextOriginSerializer : MixinContextOriginSerializerBase
  {
    private static readonly ConstructorInfo s_constructor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new MixinContextOrigin ("kind", null, "location"));

    private static readonly MethodInfo s_assemblyLoadMethod =
        MemberInfoFromExpressionUtility.GetMethod (() => Assembly.Load ("assemblyName"));
      

    public Expression CreateNewExpression ()
    {
      // new MixinContextOrigin (Kind, Assembly.Load (Assembly.FullName), Location)

      return Expression.New (
          s_constructor,
          Expression.Constant (Kind),
          Expression.Call (s_assemblyLoadMethod, Expression.Constant (Assembly.FullName)),
          Expression.Constant (Locaction));
    }
  }
}