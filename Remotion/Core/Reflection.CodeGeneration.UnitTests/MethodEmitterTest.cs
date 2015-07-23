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
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Reflection.CodeGeneration.UnitTests.TestDomain;
using Rhino.Mocks;

namespace Remotion.Reflection.CodeGeneration.UnitTests
{
  [TestFixture]
  public class MethodEmitterTest : MethodGenerationTestBase
  {
    [Test]
    public void SimpleMethod ()
    {
      var method = ClassEmitter.CreateMethod ("SimpleMethod", MethodAttributes.Public, typeof (string), new[] { typeof (string) });
      method.ImplementByReturning (
              new MethodInvocationExpression (null, typeof (string).GetMethod ("Concat", new[] { typeof (string), typeof (string) }),
              method.ArgumentReferences[0].ToExpression (), new ConstReference ("Simple").ToExpression ()));

      object returnValue = BuildInstanceAndInvokeMethod(method, "Param");
      Assert.That (returnValue, Is.EqualTo ("ParamSimple"));

      Assert.That (method.MethodBuilder, Is.Not.Null);
      Assert.That (method.ParameterTypes, Is.EqualTo (new[] { typeof (string) }));
    }

    [Test]
    public void StaticMethod ()
    {
      var method = ClassEmitter.CreateMethod (
          "StaticMethod",
          MethodAttributes.Public | MethodAttributes.Static,
          typeof (string),
          new[] { typeof (string) });
      method.ImplementByReturning (
          new MethodInvocationExpression (null, typeof (string).GetMethod ("Concat", new[] { typeof (string), typeof (string) }),
          method.ArgumentReferences[0].ToExpression (), new ConstReference ("Simple").ToExpression ()));

      object returnValue = BuildTypeAndInvokeMethod (method, "Param");
      Assert.That (returnValue, Is.EqualTo ("ParamSimple"));
    }

    [Test]
    public void CopiedMethod ()
    {
      var method = ClassEmitter.CreateMethod ("SimpleMethod", MethodAttributes.Public, typeof (List<int>).GetMethod ("Contains", new[] { typeof (int) }));
      method.ImplementByReturning (new ConstReference (false).ToExpression ());

      object returnValue = BuildInstanceAndInvokeMethod (method, 12);
      Assert.That (returnValue, Is.EqualTo (false));

      Assert.That (method.MethodBuilder, Is.Not.Null);
      Assert.That (method.ReturnType, Is.EqualTo (typeof (bool)));
      Assert.That (method.ParameterTypes, Is.EqualTo (new[] { typeof (int) }));
    }

    [Test]
    public void CopiedMethod_Generic ()
    {
      var method = ClassEmitter.CreateMethod ("SimpleMethod", MethodAttributes.Public, typeof (ClassWithSimpleGenericMethod).GetMethod ("GenericMethod"));
      method.ImplementByReturning (new ConstReference ("done").ToExpression ());

      object returnValue = BuildInstanceAndInvokeMethod (method, new[] { typeof (int), typeof (string), typeof (bool) }, 12, "", false);
      Assert.That (returnValue, Is.EqualTo ("done"));

      Assert.That (method.MethodBuilder, Is.Not.Null);
      Assert.That (method.ReturnType, Is.EqualTo (typeof (string)));
      Assert.That (method.ParameterTypes, Has.Length.EqualTo (3));
      Assert.That (method.ParameterTypes[0].IsGenericParameter, Is.True);
      Assert.That (method.ParameterTypes[0].Name, Is.EqualTo ("T1"));
      Assert.That (method.ParameterTypes[1].IsGenericParameter, Is.True);
      Assert.That (method.ParameterTypes[1].Name, Is.EqualTo ("T2"));
      Assert.That (method.ParameterTypes[2].IsGenericParameter, Is.True);
      Assert.That (method.ParameterTypes[2].Name, Is.EqualTo ("T3"));
    }

    [Test]
    public void ILGenerator ()
    {
      var method = ClassEmitter.CreateMethod ("StaticMethod", MethodAttributes.Public | MethodAttributes.Static, typeof (string), new Type[0]);
      ILGenerator gen = method.ILGenerator;
      Assert.That (gen, Is.Not.Null);
      gen.Emit (OpCodes.Ldstr, "manual retval");
      gen.Emit (OpCodes.Ret);

      object returnValue = BuildTypeAndInvokeMethod (method);
      Assert.That (returnValue, Is.EqualTo ("manual retval"));
    }

    [Test]
    public void GetArgumentExpressions ()
    {
      var method = ClassEmitter.CreateMethod (
          "StaticMethod",
          MethodAttributes.Public | MethodAttributes.Static,
          typeof (string),
          new[] { typeof (string) });
      Expression[] argumentExpressions = method.GetArgumentExpressions ();

      Assert.That (argumentExpressions.Length, Is.EqualTo (method.ArgumentReferences.Length));
      for (int i = 0; i < argumentExpressions.Length; ++i)
        Assert.That (PrivateInvoke.GetNonPublicField (argumentExpressions[i], "reference"), Is.EqualTo (method.ArgumentReferences[i]));
    }

    [Test]
    public void ImplementByReturning ()
    {
      var method = ClassEmitter.CreateMethod ("MethodReturning", MethodAttributes.Public, typeof (string), new Type[0])
          .ImplementByReturning (new ConstReference ("none").ToExpression());

      Assert.That (BuildInstanceAndInvokeMethod (method), Is.EqualTo ("none"));
    }

    [Test]
    public void ImplementByReturningVoid ()
    {
      var method = ClassEmitter.CreateMethod ("MethodReturningVoid", MethodAttributes.Public, typeof (void), new Type[0])
          .ImplementByReturningVoid ();

      Assert.That (BuildInstanceAndInvokeMethod (method), Is.EqualTo (null));
    }

    [Test]
    public void ImplementByReturningDefaultValueType ()
    {
      var intMethod = ClassEmitter.CreateMethod ("IntMethod", MethodAttributes.Public, typeof (int), new Type[0])
          .ImplementByReturningDefault ();
      var dateTimeMethod = ClassEmitter.CreateMethod ("DateTimeMethod", MethodAttributes.Public, typeof (DateTime), new Type[0])
          .ImplementByReturningDefault ();

      object instance = BuildInstance ();

      Assert.That (InvokeMethod (instance, intMethod), Is.EqualTo (0));
      Assert.That (InvokeMethod (instance, dateTimeMethod), Is.EqualTo (new DateTime ()));
    }

    [Test]
    public void ImplementByReturningDefaultReferenceType ()
    {
      var objectMethod = ClassEmitter.CreateMethod ("ObjectMethod", MethodAttributes.Public, typeof (object), new Type[0])
          .ImplementByReturningDefault ();
      var stringMethod = ClassEmitter.CreateMethod ("StringMethod", MethodAttributes.Public, typeof (string), new Type[0])
          .ImplementByReturningDefault ();

      object instance = BuildInstance ();

      Assert.That (InvokeMethod (instance, objectMethod), Is.EqualTo (null));
      Assert.That (InvokeMethod (instance, stringMethod), Is.EqualTo (null));
    }

    [Test]
    public void ImplementByReturningDefaultVoid ()
    {
      var voidMethod = ClassEmitter.CreateMethod ("VoidMethod", MethodAttributes.Public, typeof (void), new Type[0])
          .ImplementByReturningDefault ();

      object instance = BuildInstance ();

      Assert.That (InvokeMethod (instance, voidMethod), Is.EqualTo (null));
      Assert.That (GetMethod (instance, voidMethod).ReturnType, Is.EqualTo (typeof (void)));
    }

    [Test]
    public void ImplementByDelegating ()
    {
      var method = ClassEmitter.CreateMethod (
          "EqualsSelf", MethodAttributes.Public | MethodAttributes.Static, typeof (bool), new[] { typeof (object) });
      method.ImplementByDelegating (method.ArgumentReferences[0], typeof (object).GetMethod ("Equals", new[] {typeof (object)}));

      object instance = BuildInstance ();

      Assert.That (InvokeMethod (instance, method, 5), Is.EqualTo (true));
      Assert.That (InvokeMethod (instance, method, "five"), Is.EqualTo (true));
    }

    [Test]
    public void ImplementByDelegatingToValueType ()
    {
      var method = ClassEmitter.CreateMethod (
          "IntEqualsSelf", MethodAttributes.Public | MethodAttributes.Static, typeof (bool), new[] { typeof (int) });
      LocalReference local = method.DeclareLocal (typeof (int));
      method.AddStatement (new AssignStatement (local, method.ArgumentReferences[0].ToExpression()));
      method.ImplementByDelegating (local, typeof (int).GetMethod ("Equals", new[] { typeof (int) }));

      object instance = BuildInstance ();
      Assert.That (InvokeMethod (instance, method, 5), Is.EqualTo (true));
    }

    [Test]
    public void ImplementByBaseCall ()
    {
      var method = ClassEmitter.CreateMethod ("NewEquals", MethodAttributes.Public, typeof (bool), new[] { typeof (object) });
      method.ImplementByBaseCall (typeof (object).GetMethod ("Equals", new[] { typeof (object) }));

      object instance = BuildInstance ();

      Assert.That (InvokeMethod (instance, method, 5), Is.EqualTo (false));
      Assert.That (InvokeMethod (instance, method, instance), Is.EqualTo (true));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The given method System.ICloneable.Clone is abstract.",
        MatchType = MessageMatch.Contains)]
    public void ImplementByBaseCallThrowsOnAbstractMethod ()
    {
      var method = ClassEmitter.CreateMethod ("NewEquals", MethodAttributes.Public, typeof (bool), new[]{typeof (object)});
      method.ImplementByBaseCall (typeof (ICloneable).GetMethod ("Clone"));
    }

    [Test]
    [ExpectedException (typeof (NotFiniteNumberException), ExpectedMessage = "Who would have expected this?")]
    public void ImplementByThrowing ()
    {
      var method = ClassEmitter.CreateMethod ("ThrowingMethod", MethodAttributes.Public, typeof (void), new Type[0])
          .ImplementByThrowing (typeof (NotFiniteNumberException), "Who would have expected this?");

      try
      {
        BuildInstanceAndInvokeMethod (method);
      }
      catch (TargetInvocationException ex)
      {
        throw ex.InnerException;
      }
    }

    [Test]
    public void AddStatement ()
    {
      var method = ClassEmitter.CreateMethod ("Statement", MethodAttributes.Public, typeof (void), new Type[0])
        .AddStatement (new ReturnStatement ());

      BuildInstanceAndInvokeMethod (method);
    }

    [Test]
    public void DeclareLocal ()
    {
      var method = ClassEmitter.CreateMethod ("Statement", MethodAttributes.Public, typeof (int), new Type[0]);
      LocalReference local = method.DeclareLocal (typeof (int));
      method.ImplementByReturning (local.ToExpression ());

      Assert.That (BuildInstanceAndInvokeMethod (method), Is.EqualTo (0));
    }

    [Test]
    public void AddCustomAttribute ()
    {
      var method = ClassEmitter.CreateMethod ("Statement", MethodAttributes.Public, typeof (void), new Type[0]);
      method.AddCustomAttribute (new CustomAttributeBuilder (typeof (SimpleAttribute).GetConstructor (Type.EmptyTypes), new object[0]));

      MethodInfo methodInfo = BuildTypeAndGetMethod (method);
      Assert.That (methodInfo.GetCustomAttributes (typeof (SimpleAttribute), false).Length, Is.EqualTo (1));
    }

    [Test]
    public void AcceptStatement ()
    {
      var fakeGenerator = new DynamicMethod ("Test", typeof (void), Type.EmptyTypes).GetILGenerator ();
      var statementMock = MockRepository.GenerateMock<Statement> ();

      var method = ClassEmitter.CreateMethod ("AcceptStatement", MethodAttributes.Public, typeof (void), new Type[0]);
      method.AcceptStatement (statementMock, fakeGenerator);

      statementMock.AssertWasCalled (mock => mock.Emit (Arg<IMemberEmitter>.Matches (e => e.Member == method.MethodBuilder), Arg.Is (fakeGenerator)));
    }

    [Test]
    public void AcceptExpression ()
    {
      var fakeGenerator = new DynamicMethod ("Test", typeof (void), Type.EmptyTypes).GetILGenerator ();
      var expressionMock = MockRepository.GenerateMock<Expression> ();

      var method = ClassEmitter.CreateMethod ("AcceptStatement", MethodAttributes.Public, typeof (void), new Type[0]);
      method.AcceptExpression (expressionMock, fakeGenerator);

      expressionMock.AssertWasCalled (mock => mock.Emit (Arg<IMemberEmitter>.Matches (e => e.Member == method.MethodBuilder), Arg.Is(fakeGenerator)));
    }
  }
}
