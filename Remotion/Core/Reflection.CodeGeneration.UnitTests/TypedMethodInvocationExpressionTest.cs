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
  public class TypedMethodInvocationExpressionTest : SnippetGenerationBaseTest
  {
    public class ReferenceType
    {
      public string Method ()
      {
        return "ReferenceTypeMethod";
      }

      public Tuple<int, string> MethodWithArgs (int i, string s)
      {
        return new Tuple<int, string> (i, s);
      }
    }

    public struct ValueType
    {
      public string Method ()
      {
        return "ValueTypeMethod";
      }
    }

    [Test]
    public void TypedMethodInvocationMethodProperty ()
    {
      var method = GetUnsavedMethodEmitter (false, typeof (string), new Type[0]);
      Expression newObject = new NewInstanceExpression (typeof (ReferenceType), Type.EmptyTypes);
      ExpressionReference newObjectReference = new ExpressionReference (typeof (ReferenceType), newObject, method);

      TypedMethodInvocationExpression expression =
          new TypedMethodInvocationExpression (newObjectReference, typeof (ReferenceType).GetMethod ("Method"));
      Assert.That (expression.Method, Is.EqualTo (typeof (ReferenceType).GetMethod ("Method")));
    }

    [Test]
    public void TypedMethodInvocationOnReferenceType ()
    {
      var method = GetMethodEmitter (false, typeof (string), new Type[0]);
      Expression newObject = new NewInstanceExpression (typeof (ReferenceType), Type.EmptyTypes);
      ExpressionReference newObjectReference = new ExpressionReference (typeof (ReferenceType), newObject, method);
      method.ImplementByReturning (new TypedMethodInvocationExpression (newObjectReference,
          typeof (ReferenceType).GetMethod ("Method")));

      Assert.That (InvokeMethod (), Is.EqualTo ("ReferenceTypeMethod"));
    }

    [Test]
    public void TypedMethodInvocationOnValueType ()
    {
      var method = GetMethodEmitter (false, typeof (string), new Type[0]);
      Expression newObject = new InitObjectExpression (method, typeof (ValueType));
      ExpressionReference newObjectReference = new ExpressionReference (typeof (ValueType), newObject, method);
      method.ImplementByReturning (new TypedMethodInvocationExpression (newObjectReference,
          typeof (ValueType).GetMethod ("Method")));

      Assert.That (InvokeMethod (), Is.EqualTo ("ValueTypeMethod"));
    }

    [Test]
    public void TypedMethodInvocationWithComplexOwner ()
    {
      FieldReference fieldReference = ClassEmitter.CreateField ("CallTarget", typeof (ReferenceType));
      FieldInfoReference fieldInfoReference = new FieldInfoReference (SelfReference.Self, fieldReference.Reference);

      var method = GetMethodEmitter (false, typeof (string), new Type[0]);

      method.AddStatement (new AssignStatement (fieldReference, new NewInstanceExpression (typeof (ReferenceType), Type.EmptyTypes)));

      method.ImplementByReturning (new TypedMethodInvocationExpression (fieldInfoReference, typeof (ReferenceType).GetMethod ("Method")));

      Assert.That (InvokeMethod (), Is.EqualTo ("ReferenceTypeMethod"));
    }

    [Test]
    public void TypedMethodInvocationWithArguments ()
    {
      FieldReference fieldReference = ClassEmitter.CreateField ("CallTarget", typeof (ReferenceType));
      FieldInfoReference fieldInfoReference = new FieldInfoReference (SelfReference.Self, fieldReference.Reference);

      var method = GetMethodEmitter (false, typeof (Tuple<int, string>), new Type[0]);

      method.AddStatement (new AssignStatement (fieldReference, new NewInstanceExpression (typeof (ReferenceType), Type.EmptyTypes)));

      method.ImplementByReturning (new TypedMethodInvocationExpression (fieldInfoReference, typeof (ReferenceType).GetMethod ("MethodWithArgs"),
        new ConstReference (1).ToExpression(), new ConstReference ("2").ToExpression()));

      Assert.That (InvokeMethod (), Is.EqualTo (new Tuple<int, string> (1, "2")));
    }
  }
}
