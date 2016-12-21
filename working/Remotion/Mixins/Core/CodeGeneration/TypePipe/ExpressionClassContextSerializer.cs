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
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Serialization;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// A <see cref="IClassContextSerializer"/> that builds expressions that represent the creation of an equivalent <see cref="ClassContext"/>
  /// from constant values.
  /// </summary>
  public class ExpressionClassContextSerializer : ClassContextSerializerBase
  {
    private static readonly ConstructorInfo s_constructor =
        MemberInfoFromExpressionUtility.GetConstructor (() => new ClassContext (null, new MixinContext[0], Type.EmptyTypes));

    public Expression CreateNewExpression ()
    {
      // new ClassContext (Type, new MixinContext[] { ... }, ComposedInterfaces)

      return Expression.New (
          s_constructor,
          Expression.Constant (Type),
          Expression.NewArrayInit (typeof (MixinContext), MixinContexts.Select (CreateMixinContextExpression)),
          Expression.ArrayConstant (ComposedInterfaces));
    }

    private Expression CreateMixinContextExpression (MixinContext mixinContext)
    {
      var serializer = new ExpressionMixinContextSerializer();
      mixinContext.Serialize (serializer);

      return serializer.CreateNewExpression();
    }
  }
}