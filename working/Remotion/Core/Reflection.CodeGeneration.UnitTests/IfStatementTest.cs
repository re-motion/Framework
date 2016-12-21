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
  public class IfStatementTest : SnippetGenerationBaseTest
  {
    [Test]
    public void IfWithTrueCondition ()
    {
      var methodEmitter = GetMethodEmitter (false, typeof (string), new Type[0]);
      methodEmitter.AddStatement (new IfStatement (new SameConditionExpression (NullExpression.Instance, NullExpression.Instance),
          new ReturnStatement (new ConstReference ("True"))));
      methodEmitter.AddStatement (new ReturnStatement (new ConstReference ("False")));

      Assert.That (InvokeMethod(), Is.EqualTo ("True"));
    }

    [Test]
    public void FalseCondition ()
    {
      var methodEmitter = GetMethodEmitter (false, typeof (string), new Type[0]);
      methodEmitter.AddStatement (new IfStatement (new SameConditionExpression (NullExpression.Instance, new ConstReference ("5").ToExpression()),
          new ReturnStatement (new ConstReference ("True"))));
      methodEmitter.AddStatement (new ReturnStatement (new ConstReference ("False")));

      Assert.That (InvokeMethod (), Is.EqualTo ("False"));
    }
  }
}
