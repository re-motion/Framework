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
  /// Groups several statements together in a block statement. When the block is emitted, all statements are emitted in the order in which they were
  /// given to the <see cref="BlockStatement"/>.
  /// </summary>
  public class BlockStatement : Statement
  {
    private readonly Statement[] _statements;

    public BlockStatement (params Statement[] statements)
    {
      ArgumentUtility.CheckNotNull ("statements", statements);
      _statements = statements;
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      foreach (var statement in _statements)
        statement.Emit (member, gen);
    }
  }
}
