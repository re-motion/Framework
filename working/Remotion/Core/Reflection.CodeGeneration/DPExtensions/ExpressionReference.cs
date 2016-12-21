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
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  // Converts an expression to a reference by saving it as a temporary local variable at time of emitting
  public class ExpressionReference : TypeReference
  {
    private readonly Type _referenceType;
    private readonly Expression _expression;
    private readonly IMethodEmitter _methodEmitter;

    public ExpressionReference (Type referenceType, Expression expression, IMethodEmitter methodEmitter)
        : base (ArgumentUtility.CheckNotNull ("referenceType", referenceType))
    {
      ArgumentUtility.CheckNotNull ("expression", expression);
      ArgumentUtility.CheckNotNull ("methodEmitter", methodEmitter);

      _referenceType = referenceType;
      _methodEmitter = methodEmitter;
      _expression = expression;
    }

    public override void LoadAddressOfReference (ILGenerator gen)
    {
      ArgumentUtility.CheckNotNull ("gen", gen);

      LocalReference local = CreateLocal (gen);
      local.LoadAddressOfReference (gen);
    }

    public override void LoadReference (ILGenerator gen)
    {
      ArgumentUtility.CheckNotNull ("gen", gen);

      LocalReference local = CreateLocal(gen);
      local.LoadReference (gen);
    }

    private LocalReference CreateLocal (ILGenerator gen)
    {
      ArgumentUtility.CheckNotNull ("gen", gen);

      LocalReference local = _methodEmitter.DeclareLocal (_referenceType);
      local.Generate (gen);
      _methodEmitter.AcceptStatement (new AssignStatement (local, _expression), gen);
      return local;
    }

    public override void StoreReference (ILGenerator gen)
    {
      throw new NotSupportedException ("Expressions cannot be assigned to.");
    }
  }
}
