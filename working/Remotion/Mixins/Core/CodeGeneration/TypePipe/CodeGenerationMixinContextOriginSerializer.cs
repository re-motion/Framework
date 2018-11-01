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
  /// Serializes a <see cref="MixinContextOrigin"/> object into instructions that reinstantiate an equivalent object when executed.
  /// </summary>
  public class CodeGenerationMixinContextOriginSerializer : IMixinContextOriginSerializer
  {
    private static readonly ConstructorInfo s_constructor = 
        typeof (MixinContextOrigin).GetConstructor (new[] {typeof (string), typeof (Assembly), typeof (string)});
    private static readonly MethodInfo s_assemblyLoadMethod = typeof (Assembly).GetMethod ("Load", new[] { typeof (string) });
    
    private readonly Expression[] _constructorArguments = new Expression[3];

    public Expression GetConstructorInvocationExpression ()
    {
      Assertion.IsNotNull (s_constructor);
      return Expression.New (s_constructor, _constructorArguments);
    }

    public void AddKind (string kind)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("kind", kind);

      _constructorArguments[0] = Expression.Constant (kind);
    }

    public void AddAssembly (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      Assertion.IsNotNull (s_assemblyLoadMethod);
      _constructorArguments[1] = Expression.Call (null, s_assemblyLoadMethod, new[] { Expression.Constant (assembly.FullName) });
    }

    public void AddLocation (string location)
    {
      _constructorArguments[2] = Expression.Constant (location);
    }
  }
}
