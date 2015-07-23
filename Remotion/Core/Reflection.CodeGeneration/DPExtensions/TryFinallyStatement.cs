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
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Remotion.Utilities;

namespace Remotion.Reflection.CodeGeneration.DPExtensions
{
  public class TryFinallyStatement : Statement
  {
    private readonly IEnumerable<Statement> _tryStatements;
    private readonly IEnumerable<Statement> _finallyStatements;

    public TryFinallyStatement (IEnumerable<Statement> tryStatements, IEnumerable<Statement> finallyStatements)
    {
      ArgumentUtility.CheckNotNull ("tryStatements", tryStatements);
      ArgumentUtility.CheckNotNull ("finallyStatements", finallyStatements);

      _tryStatements = tryStatements;
      _finallyStatements = finallyStatements;
    }

    public override void Emit (IMemberEmitter member, ILGenerator gen)
    {
      ArgumentUtility.CheckNotNull ("member", member);
      ArgumentUtility.CheckNotNull ("gen", gen);

      gen.BeginExceptionBlock ();

      foreach (Statement statement in _tryStatements)
        statement.Emit (member, gen);

      gen.BeginFinallyBlock ();
      
      foreach (Statement statement in _finallyStatements)
        statement.Emit (member, gen);

      gen.EndExceptionBlock ();
      gen.Emit (OpCodes.Nop); // ensure a leave target for try block
    }
  }
}
