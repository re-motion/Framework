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
using System.Reflection;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Reflection.CodeGeneration.DPExtensions;
using Remotion.Reflection.CodeGeneration.UnitTests.TestDomain;

namespace Remotion.Reflection.CodeGeneration.UnitTests
{
  [TestFixture]
  public class CustomAttributeExpressionTest : SnippetGenerationBaseTest
  {
    [Test]
    public void CustomAttributeExpression ()
    {
      var methodEmitter = GetMethodEmitter (false, typeof (Tuple<SimpleAttribute, SimpleAttribute>), new Type[0]);

      LocalReference attributeOwner = methodEmitter.DeclareLocal (typeof (Type));
      methodEmitter.AddStatement (new AssignStatement (attributeOwner, new TypeTokenExpression (typeof (ClassWithCustomAttribute))));

      ConstructorInfo tupleCtor =
          typeof (Tuple<SimpleAttribute, SimpleAttribute>).GetConstructor (new[] {typeof (SimpleAttribute), typeof (SimpleAttribute)});
      Expression tupleExpression = new NewInstanceExpression (tupleCtor,
          new CustomAttributeExpression (attributeOwner, typeof (SimpleAttribute), 0, true),
          new CustomAttributeExpression (attributeOwner, typeof (SimpleAttribute), 1, true));

      methodEmitter.AddStatement (new ReturnStatement (tupleExpression));

      object[] attributes = typeof (ClassWithCustomAttribute).GetCustomAttributes (typeof (SimpleAttribute), true);

      var attributeTuple = (Tuple<SimpleAttribute, SimpleAttribute>) InvokeMethod();
      Assert.That (new object[] { attributeTuple.Item1, attributeTuple.Item2 }, Is.EquivalentTo (attributes));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "Parameter 'attributeOwner' is a 'System.String', which cannot be assigned to type 'System.Reflection.ICustomAttributeProvider'."
        + "\r\nParameter name: attributeOwner")]
    public void CustomAttributeExpressionThrowsOnWrongReferenceType ()
    {
      new CustomAttributeExpression (new LocalReference (typeof (string)), typeof (SimpleAttribute), 0, true);
    }
  }
}
