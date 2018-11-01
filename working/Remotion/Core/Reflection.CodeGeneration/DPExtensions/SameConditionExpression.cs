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

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class SameConditionExpression : ConditionExpression
  {
    private Expression _left;
    private Expression _right;

    public SameConditionExpression(Expression left, Expression right)
    {
      _left = left;
      _right = right;
    }

    public override OpCode BranchIfTrue
    {
      get { return OpCodes.Beq; }
    }

    public override OpCode BranchIfFalse
    {
      get { return OpCodes.Bne_Un; }
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      _left.Emit (member, gen);
      _right.Emit (member, gen);
    }
  }
}
