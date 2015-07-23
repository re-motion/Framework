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
  /// Serializes a <see cref="ClassContext"/> object into instructions that reinstantiate an equivalent object when executed.
  /// </summary>
  public class CodeGenerationClassContextSerializer : IClassContextSerializer
  {
    private static readonly ConstructorInfo s_constructor =
        typeof (ClassContext).GetConstructor (new[] { typeof (Type), typeof (IEnumerable<MixinContext>), typeof (IEnumerable<Type>) });

    private readonly Expression[] _constructorArguments = new Expression[3];

    public Expression GetConstructorInvocationExpression ()
    {
      Assertion.IsNotNull (s_constructor);
      return Expression.New (s_constructor, _constructorArguments);
    }

    public void AddClassType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      _constructorArguments[0] = Expression.Constant (type);
    }

    public void AddMixins (IEnumerable<MixinContext> mixinContexts)
    {
      ArgumentUtility.CheckNotNull ("mixinContexts", mixinContexts);

      _constructorArguments[1] = Expression.NewArrayInit (
          typeof (MixinContext),
          mixinContexts.Select (
              mc =>
              {
                var serializer = new CodeGenerationMixinContextSerializer();
                mc.Serialize (serializer);
                return serializer.GetConstructorInvocationExpression();
              }));
    }

    public void AddComposedInterfaces (IEnumerable<Type> composedInterfaces)
    {
      ArgumentUtility.CheckNotNull ("composedInterfaces", composedInterfaces);

      _constructorArguments[2] = Expression.NewArrayInit (typeof (Type), composedInterfaces.Select (ci => (Expression) Expression.Constant (ci)));
    }
  }
}