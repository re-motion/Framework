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
using Remotion.Development.UnitTesting;
using Remotion.Reflection.CodeGeneration.DPExtensions;

namespace Remotion.Reflection.CodeGeneration.UnitTests
{
  [TestFixture]
  public class TryFinallyStatementTest : SnippetGenerationBaseTest
  {
    [Test]
    public void TryFinallyWithoutException ()
    {
      FieldReference tryField = ClassEmitter.CreateField ("TryExecuted", typeof (bool));
      FieldReference finallyField = ClassEmitter.CreateField ("FinallyExecuted", typeof (bool));

      var methodEmitter = GetMethodEmitter (false, typeof (void), new Type[0]);
      Statement[] tryBlock = new Statement[]
      {
        new AssignStatement (tryField, new ConstReference (true).ToExpression())
      };
      Statement[] finallyBlock = new Statement[]
      {
        new AssignStatement (finallyField, new ConstReference (true).ToExpression())
      };

      methodEmitter.AddStatement (new TryFinallyStatement (tryBlock, finallyBlock));
      methodEmitter.AddStatement (new ReturnStatement ());

      InvokeMethod ();
      Assert.That ((bool) PrivateInvoke.GetPublicField (GetBuiltInstance (), tryField.Reference.Name), Is.True);
      Assert.That ((bool) PrivateInvoke.GetPublicField (GetBuiltInstance (), finallyField.Reference.Name), Is.True);
    }

    [Test]
    public void TryFinallyWithException ()
    {
      FieldReference tryField = ClassEmitter.CreateField ("TryExecuted", typeof (bool));
      FieldReference finallyField = ClassEmitter.CreateField ("FinallyExecuted", typeof (bool));

      var methodEmitter = GetMethodEmitter (false, typeof (void), new Type[0]);
      Statement[] tryBlock = new Statement[]
      {
        new ThrowStatement (typeof (Exception), "Expected exception"),
        new AssignStatement (tryField, new ConstReference (true).ToExpression())
      };
      Statement[] finallyBlock = new Statement[]
      {
        new AssignStatement (finallyField, new ConstReference (true).ToExpression())
      };

      methodEmitter.AddStatement (new TryFinallyStatement (tryBlock, finallyBlock));
      methodEmitter.AddStatement (new ReturnStatement ());

      try
      {
        InvokeMethod ();
        Assert.Fail ("Expected exception");
      }
      catch (Exception ex)
      {
        Assert.That (ex.GetType (), Is.EqualTo (typeof (Exception)));
        Assert.That (ex.Message, Is.EqualTo ("Expected exception"));
      }
      Assert.That ((bool) PrivateInvoke.GetPublicField (GetBuiltInstance (), tryField.Reference.Name), Is.False);
      Assert.That ((bool) PrivateInvoke.GetPublicField (GetBuiltInstance (), finallyField.Reference.Name), Is.True);
    }
  }
}
