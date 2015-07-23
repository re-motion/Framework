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
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  /// <summary>
  /// Expression that emits a <see cref="OpCodes.Castclass"/> opcode, thus converting another expression to a given target type. The 
  /// <see cref="OpCodes.Castclass"/> opcode is emitted without any checks, so this expression must only be used when that opcode is allowed.
  /// </summary>
  public class CastClassExpression : Expression
  {
    private readonly Type _targetType;
    private readonly Expression _right;

    public CastClassExpression (Type targetType, Expression right)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("right", right);

      _targetType = targetType;
      _right = right;
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      ArgumentUtility.CheckNotNull ("gen", gen);

      _right.Emit (member, gen);
      gen.Emit (OpCodes.Castclass, _targetType);
    }
  }
}
