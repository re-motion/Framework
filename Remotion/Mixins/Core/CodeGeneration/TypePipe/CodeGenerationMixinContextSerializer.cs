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
using Remotion.Mixins.Context;
using Remotion.Mixins.Context.Serialization;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.Utilities;

namespace Remotion.Mixins.CodeGeneration.TypePipe
{
  /// <summary>
  /// Serializes a <see cref="MixinContext"/> object into instructions that reinstantiate an equivalent object when executed.
  /// </summary>
  public class CodeGenerationMixinContextSerializer : IMixinContextSerializer
  {
    private static readonly ConstructorInfo s_constructor =
        typeof (MixinContext).GetConstructor (
            new[] { typeof (MixinKind), typeof (Type), typeof (MemberVisibility), typeof (IEnumerable<Type>), typeof (MixinContextOrigin) });

    private readonly Expression[] _constructorArguments = new Expression[5];

    public Expression GetConstructorInvocationExpression ()
    {
      Assertion.IsNotNull (s_constructor);
      return Expression.New (s_constructor, _constructorArguments);
    }

    public void AddMixinType (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);

      _constructorArguments[1] = Expression.Constant (mixinType);
    }

    public void AddMixinKind (MixinKind mixinKind)
    {
      _constructorArguments[0] = Expression.Constant (mixinKind);
    }

    public void AddIntroducedMemberVisibility (MemberVisibility introducedMemberVisibility)
    {
      _constructorArguments[2] = Expression.Constant (introducedMemberVisibility);
    }

    public void AddExplicitDependencies (IEnumerable<Type> explicitDependencies)
    {
      ArgumentUtility.CheckNotNull ("explicitDependencies", explicitDependencies);

      _constructorArguments[3] = Expression.NewArrayInit (typeof (Type), explicitDependencies.Select (d => (Expression) Expression.Constant (d)));
    }

    public void AddOrigin (MixinContextOrigin origin)
    {
      ArgumentUtility.CheckNotNull ("origin", origin);
      var originSerializer = new CodeGenerationMixinContextOriginSerializer();
      origin.Serialize (originSerializer);
      _constructorArguments[4] = originSerializer.GetConstructorInvocationExpression();
    }
  }
}
