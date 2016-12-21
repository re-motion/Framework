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
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration.DPExtensions;

namespace Remotion.Reflection.CodeGeneration.UnitTests
{
  [TestFixture]
  public class PopStatementTest : SnippetGenerationBaseTest
  {
    [Test]
    public void Pop ()
    {
      var methodEmitter = GetMethodEmitter (false, typeof (void), new Type[0]);
      methodEmitter.AddStatement (new ILStatement (delegate (IMemberEmitter emitter, ILGenerator gen) { gen.Emit (OpCodes.Ldc_I4_0); }));
      methodEmitter.AddStatement (new PopStatement ());
      methodEmitter.AddStatement (new ReturnStatement ());

      InvokeMethod ();
    }
  }
}
