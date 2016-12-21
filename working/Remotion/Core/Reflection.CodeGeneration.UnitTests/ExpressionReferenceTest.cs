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
using Remotion.Reflection.CodeGeneration.UnitTests.TestDomain;

namespace Remotion.Reflection.CodeGeneration.UnitTests
{
  [TestFixture]
  public class ExpressionReferenceTest : SnippetGenerationBaseTest
  {
    [Test]
    public void ExpressionReference ()
    {
      var methodEmitter = GetMethodEmitter (false, typeof (string), new Type[0]);
      
      var expressionReference = new ExpressionReference (typeof (string), new ConstReference ("bla").ToExpression(), methodEmitter);
      methodEmitter.ImplementByReturning (new ReferenceExpression (expressionReference));

      Assert.That (InvokeMethod(), Is.EqualTo ("bla"));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Expressions cannot be assigned to.")]
    public void ExpressionReferenceCannotBeStored ()
    {
      var methodEmitter = GetUnsavedMethodEmitter (false, typeof (void), new Type[0]);
      var expressionReference = new ExpressionReference (typeof (string), new ConstReference ("bla").ToExpression (), methodEmitter);
      expressionReference.StoreReference (null);
    }

    [Test]
    public void LoadAddressOfExpressionReference ()
    {
      var methodEmitter = GetMethodEmitter (false, typeof (string), new Type[0]);

      var expressionReference = new ExpressionReference (
          typeof (StructWithMethod), 
          new InitObjectExpression (methodEmitter, typeof (StructWithMethod)), 
          methodEmitter);
      var addressReference = new ExpressionReference (
          typeof (StructWithMethod).MakeByRefType(), 
          expressionReference.ToAddressOfExpression(), 
          methodEmitter);
      var methodCall =
          new MethodInvocationExpression (addressReference, typeof (StructWithMethod).GetMethod ("Method"));

      methodEmitter.ImplementByReturning (methodCall);

      Assert.That (InvokeMethod(), Is.EqualTo ("StructMethod"));
    }
  }
}
