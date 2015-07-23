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
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration.DPExtensions;

namespace Remotion.Reflection.CodeGeneration.UnitTests
{
  [TestFixture]
  public class BlockStatementTest : SnippetGenerationBaseTest
  {
    [Test]
    public void Block ()
    {
      var methodEmitter = GetMethodEmitter (false, typeof (int), new Type[0]);
      var local = methodEmitter.DeclareLocal (typeof (int));
      var blockStatement = new BlockStatement (
          new AssignStatement (local, new ConstReference (1).ToExpression ()),
          new ReturnStatement (local));
      methodEmitter.AddStatement (blockStatement);

      Assert.That (InvokeMethod(), Is.EqualTo (1));
    }
  }
}
