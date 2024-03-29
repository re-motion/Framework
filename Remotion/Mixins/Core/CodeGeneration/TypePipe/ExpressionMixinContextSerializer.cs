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
using System.Reflection;
using Remotion.Mixins.Context;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// Generates an expression that regenerates a <see cref="MixinContext"/> serialized using this class.
  /// </summary>
  public class ExpressionMixinContextSerializer : MixinContextSerializerBase
  {
    private static readonly ConstructorInfo s_constructor =
        MemberInfoFromExpressionUtility.GetConstructor(() => new MixinContext(MixinKind.Used, null!, MemberVisibility.Private, new Type[0], null!));

    public Expression CreateNewExpression ()
    {
      // new MixinContext (MixinKind, MixinType, IntroducedMemberVisibility, ExplicitDependencies, new MixinContextOrigin (...))

      return Expression.New(
          s_constructor,
          Expression.Constant(MixinKind),
          Expression.Constant(MixinType),
          Expression.Constant(IntroducedMemberVisibility),
          Expression.ArrayConstant(ExplicitDependencies),
          CreateOriginExpression(Origin!)); // TODO RM-7691 Change serializer properties to non-nullable return values
    }

    private Expression CreateOriginExpression (MixinContextOrigin origin)
    {
      var serializer = new ExpressionMixinContextOriginSerializer();
      origin.Serialize(serializer);

      return serializer.CreateNewExpression();
    }
  }
}
