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
  public class FieldInfoReferenceTest : SnippetGenerationBaseTest
  {
    public override void SetUp ()
    {
      base.SetUp ();
      ClassWithPublicFields.StaticReferenceTypeField = "InitialStatic";
    }

    [Test]
    public void LoadAndStoreStatic ()
    {
      FieldInfo fieldInfo = typeof (ClassWithPublicFields).GetField ("StaticReferenceTypeField");
      var methodEmitter = GetMethodEmitter (false, typeof (string), new Type[0]);

      LocalReference local = methodEmitter.DeclareLocal (typeof (string));
      FieldInfoReference fieldReference = new FieldInfoReference (null, fieldInfo);
      methodEmitter
          .AddStatement (new AssignStatement (local, fieldReference.ToExpression ()))
          .AddStatement (new AssignStatement (fieldReference, new ConstReference ("Replacement").ToExpression ()))
          .AddStatement (new ReturnStatement (local));

      Assert.That (ClassWithPublicFields.StaticReferenceTypeField, Is.EqualTo ("InitialStatic"));
      Assert.That (InvokeMethod (), Is.EqualTo ("InitialStatic"));
      Assert.That (ClassWithPublicFields.StaticReferenceTypeField, Is.EqualTo ("Replacement"));
    }

    [Test]
    public void LoadAndStoreInstance ()
    {
      FieldInfo fieldInfo = typeof (ClassWithPublicFields).GetField ("ReferenceTypeField");
      var methodEmitter = GetMethodEmitter (false, typeof (string), new[] { typeof (ClassWithPublicFields) });

      LocalReference local = methodEmitter.DeclareLocal (typeof (string));
      FieldInfoReference fieldReference = new FieldInfoReference (methodEmitter.ArgumentReferences[0], fieldInfo);
      methodEmitter
          .AddStatement (new AssignStatement (local, fieldReference.ToExpression ()))
          .AddStatement (new AssignStatement (fieldReference, new ConstReference ("Replacement").ToExpression ()))
          .AddStatement (new ReturnStatement (local));

      ClassWithPublicFields parameter = new ClassWithPublicFields ();
      Assert.That (parameter.ReferenceTypeField, Is.EqualTo ("Initial"));
      Assert.That (InvokeMethod (parameter), Is.EqualTo ("Initial"));
      Assert.That (parameter.ReferenceTypeField, Is.EqualTo ("Replacement"));
    }

    [Test]
    public void LoadAndStoreAddressStatic ()
    {
      FieldInfo fieldInfo = typeof (ClassWithPublicFields).GetField ("StaticReferenceTypeField");
      var methodEmitter = GetMethodEmitter (false, typeof (string), new Type[0]);

      LocalReference local = methodEmitter.DeclareLocal (typeof (string));
      FieldInfoReference fieldReference = new FieldInfoReference (null, fieldInfo);

      Expression addressOfFieldExpression = fieldReference.ToAddressOfExpression ();
      Reference indirectReference =
          new IndirectReference (new ExpressionReference (typeof (string).MakeByRefType(), addressOfFieldExpression, methodEmitter));
      
      methodEmitter
          .AddStatement (new AssignStatement (local, indirectReference.ToExpression ()))
          .AddStatement (new AssignStatement (indirectReference, new ConstReference ("Replacement").ToExpression ()))
          .AddStatement (new ReturnStatement (local));

      Assert.That (ClassWithPublicFields.StaticReferenceTypeField, Is.EqualTo ("InitialStatic"));
      Assert.That (InvokeMethod(), Is.EqualTo ("InitialStatic"));
      Assert.That (ClassWithPublicFields.StaticReferenceTypeField, Is.EqualTo ("Replacement"));
    }

    [Test]
    public void LoadAndStoreAddressInstance ()
    {
      FieldInfo fieldInfo = typeof (ClassWithPublicFields).GetField ("ReferenceTypeField");
      var methodEmitter = GetMethodEmitter (false, typeof (string), new[] { typeof (ClassWithPublicFields) });

      LocalReference local = methodEmitter.DeclareLocal (typeof (string));
      FieldInfoReference fieldReference = new FieldInfoReference (methodEmitter.ArgumentReferences[0], fieldInfo);

      Expression addressOfFieldExpression = fieldReference.ToAddressOfExpression ();
      Reference indirectReference =
          new IndirectReference (new ExpressionReference (typeof (string).MakeByRefType (), addressOfFieldExpression, methodEmitter));

      methodEmitter
          .AddStatement (new AssignStatement (local, indirectReference.ToExpression ()))
          .AddStatement (new AssignStatement (indirectReference, new ConstReference ("Replacement").ToExpression ()))
          .AddStatement (new ReturnStatement (local));

      ClassWithPublicFields parameter = new ClassWithPublicFields ();
      Assert.That (parameter.ReferenceTypeField, Is.EqualTo ("Initial"));
      Assert.That (InvokeMethod (parameter), Is.EqualTo ("Initial"));
      Assert.That (parameter.ReferenceTypeField, Is.EqualTo ("Replacement"));
    }
  }
}
