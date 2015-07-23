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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Reflection.UnitTests.CodeGeneration.MethodWrapperEmitterTests.TestDomain;
using Remotion.Reflection.UnitTests.TestDomain.MemberInfoAdapter;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Reflection.UnitTests
{
  [TestFixture]
  public class MethodInfoAdapterTest
  {
    private MethodInfo _method;
    private MethodInfo _explicitInterfaceImplementationMethod;
    private MethodInfo _implicitInterfaceImplementationMethod;

    private MethodInfoAdapter _adapter;
    private MethodInfoAdapter _explicitInterfaceAdapter;
    private MethodInfoAdapter _implicitInterfaceAdapter;
    
    [SetUp]
    public void SetUp ()
    {
      _method = typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod ("TestMethod");
      _adapter = MethodInfoAdapter.Create(_method);

      _explicitInterfaceImplementationMethod = typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod (
          "Remotion.Reflection.UnitTests.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.ExplicitInterfaceMethod",
          BindingFlags.NonPublic | BindingFlags.Instance);
      _explicitInterfaceAdapter = MethodInfoAdapter.Create(_explicitInterfaceImplementationMethod);

      _implicitInterfaceImplementationMethod = typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod (
          "ImplicitInterfaceMethod", BindingFlags.Public | BindingFlags.Instance);
      _implicitInterfaceAdapter = MethodInfoAdapter.Create(_implicitInterfaceImplementationMethod);
    }

    [Test]
    public void Create_ReturnsSameInstance ()
    {
      Assert.That (MethodInfoAdapter.Create (_method), Is.SameAs (MethodInfoAdapter.Create (_method)));
    }

    [Test]
    public void Create_ReturnsSameInstance_ForMethodsReflectedFromDifferentLevelsInHierarchy ()
    {
      var methodViaBase = typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod ("TestMethod");
      var methodViaDerived = typeof (DerivedClassWithReferenceType<SimpleReferenceType>).GetMethod ("TestMethod");

      Assert.That (methodViaBase, Is.Not.SameAs (methodViaDerived));
      Assert.That (MethodInfoAdapter.Create (methodViaBase), Is.SameAs (MethodInfoAdapter.Create (methodViaDerived)));
    }

    [Test]
    public void MethodInfo ()
    {
      CheckMethodInfo (_method, _adapter);
      CheckMethodInfo (_implicitInterfaceImplementationMethod, _implicitInterfaceAdapter);
      CheckMethodInfo (_explicitInterfaceImplementationMethod, _explicitInterfaceAdapter);
    }

    [Test]
    public void Name ()
    {
      Assert.That (_adapter.Name, Is.EqualTo (_method.Name));
    }

    [Test]
    public void Name_ImplicitInterface ()
    {
      Assert.That (_implicitInterfaceAdapter.Name, Is.EqualTo (_implicitInterfaceImplementationMethod.Name));
    }

    [Test]
    public void Name_ExplicitInterface ()
    {
      Assert.That (_explicitInterfaceAdapter.Name, Is.EqualTo (_explicitInterfaceImplementationMethod.Name));
    }

    [Test]
    public void DeclaringType ()
    {
      Assert.That (_adapter.DeclaringType, Is.EqualTo (TypeAdapter.Create (_method.DeclaringType)));
    }

    [Test]
    public void GetCustomAttribute ()
    {
      Assert.That (
          _adapter.GetCustomAttribute<SampleAttribute> (true), Is.EqualTo (AttributeUtility.GetCustomAttribute<SampleAttribute> (_method, true)));
    }

    [Test]
    public void GetCustomAttributes ()
    {
      Assert.That (
          _adapter.GetCustomAttributes<SampleAttribute> (true),
          Is.EqualTo (AttributeUtility.GetCustomAttributes<SampleAttribute> (_method, false)));
    }

    [Test]
    public void IsDefined ()
    {
      Assert.That (
          _adapter.IsDefined<SampleAttribute> (true),
          Is.EqualTo (AttributeUtility.IsDefined<SampleAttribute> (_method, true)));
    }

    [Test]
    public void GetReturnType ()
    {
      Assert.That (_adapter.ReturnType, Is.EqualTo (_method.ReturnType));
    }

    [Test]
    public void GetName ()
    {
      Assert.That (_adapter.Name, Is.EqualTo (_method.Name));
    }

    [Test]
    public void GetOriginalDeclaringType ()
    {
      Assert.That (_adapter.GetOriginalDeclaringType(), Is.TypeOf<TypeAdapter>().And.Property ("Type").SameAs (_method.DeclaringType));
    }

    [Test]
    public void Invoke_BaseMethod ()
    {
      var methodInfo = typeof (ClassWithBaseMember).GetMethod ("BaseMethod");
      var adapter = MethodInfoAdapter.Create(methodInfo);
      var result = adapter.Invoke (new ClassWithBaseMember(), new object[] { });

      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (TargetInvocationException))]
    public void Invoke_NullParameterForMethod_GetExceptionFromReflectionApi ()
    {
      var methodInfo = typeof (string).GetMethod ("Insert", new[] { typeof (int), typeof (string) });
      var adapter = MethodInfoAdapter.Create(methodInfo);
      adapter.Invoke ("Test", new object[] { 5, null });
    }

    [Test]
    [ExpectedException (typeof (TargetException), ExpectedMessage = "Object does not match target type.")]
    public void Invoke_WrongInstanceForMethod_GetExceptionFromReflectionApi ()
    {
      var methodInfo = typeof (ClassWithBaseMember).GetMethod ("BaseMethod");
      var adapter = MethodInfoAdapter.Create(methodInfo);
      var result = adapter.Invoke ("Test", new object[0]);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void FindInterfaceImplementation_ImplicitImplementation ()
    {
      var methodInfo = typeof (IInterfaceWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar");
      var adapter = MethodInfoAdapter.Create(methodInfo);

      var implementation = adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));

      var expectedPropertyGetter = typeof (ClassWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar");
      CheckMethodInfo(expectedPropertyGetter, (MethodInfoAdapter) implementation);
    }

    [Test]
    public void FindInterfaceImplementation_ExplicitImplementation ()
    {
      var methodInfo = typeof (IInterfaceWithReferenceType<object>).GetMethod ("get_ExplicitInterfaceScalar");
      var adapter = MethodInfoAdapter.Create(methodInfo);

      var implementation = adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));

      var expectedPropertyGetter = typeof (ClassWithReferenceType<object>).GetMethod (
          "Remotion.Reflection.UnitTests.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.get_ExplicitInterfaceScalar",
          BindingFlags.Instance | BindingFlags.NonPublic);
      CheckMethodInfo(expectedPropertyGetter, (MethodInfoAdapter) implementation);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This method is not an interface method.")]
    public void FindInterfaceImplementation_NonInterfaceMethod ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar");
      var adapter = MethodInfoAdapter.Create(methodInfo);

      adapter.FindInterfaceImplementation (typeof (ClassWithReferenceType<object>));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The implementationType parameter must not be an interface.\r\nParameter name: implementationType")]
    public void FindInterfaceImplementation_ImplementationIsInterface ()
    {
      var methodInfo = typeof (IInterfaceWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar");
      var adapter = MethodInfoAdapter.Create(methodInfo);

      adapter.FindInterfaceImplementation (typeof (IInterfaceWithReferenceType<object>));
    }

    [Test]
    public void FindInterfaceImplementation_ImplementationIsNotAssignableToTheInterface ()
    {
      var methodInfo = typeof (IInterfaceWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar");
      var adapter = MethodInfoAdapter.Create(methodInfo);

      var result = adapter.FindInterfaceImplementation (typeof (object));

      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "This method is itself an interface member, so it cannot have an interface declaration.")]
    public void FindInterfaceDeclaration_DeclaringTypeIsInterface ()
    {
      var methodInfo = typeof (IInterfaceWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar");
      MethodInfoAdapter.Create(methodInfo).FindInterfaceDeclarations();
    }

    [Test]
    public void FindInterfaceDeclaration_ImplicitImplementation ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar");
      var adapter = MethodInfoAdapter.Create(methodInfo);

      var result = adapter.FindInterfaceDeclarations();

      var expectedMethodInfos = new[] { typeof (IInterfaceWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar") };
      CheckUnorderedMethodInfos (expectedMethodInfos, result.Cast<MethodInfoAdapter> ());
    }

    [Test]
    public void FindInterfaceDeclaration_ExplicitImplementation ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod (
          "Remotion.Reflection.UnitTests.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.get_ExplicitInterfaceScalar",
          BindingFlags.Instance | BindingFlags.NonPublic);
      var adapter = MethodInfoAdapter.Create(methodInfo);

      var result = adapter.FindInterfaceDeclarations();

      var expectedMethodInfos = new[] { typeof (IInterfaceWithReferenceType<object>).GetMethod ("get_ExplicitInterfaceScalar") };
      CheckUnorderedMethodInfos (expectedMethodInfos, result.Cast<MethodInfoAdapter> ());
    }

    [Test]
    public void FindInterfaceDeclaration_ExplicitImplementation_FromBaseType ()
    {
      var methodInfo = typeof (DerivedClassWithReferenceType<object>).GetMethod (
          "Remotion.Reflection.UnitTests.TestDomain.MemberInfoAdapter.IInterfaceWithReferenceType<T>.get_ExplicitInterfaceScalar",
          BindingFlags.Instance | BindingFlags.NonPublic);
      var adapter = MethodInfoAdapter.Create(methodInfo);

      var result = adapter.FindInterfaceDeclarations();

      var expectedMethodInfos = new[] { typeof (IInterfaceWithReferenceType<object>).GetMethod ("get_ExplicitInterfaceScalar") };
      CheckUnorderedMethodInfos (expectedMethodInfos, result.Cast<MethodInfoAdapter> ());
    }

    [Test]
    public void FindInterfaceDeclaration_MultipleDeclarations ()
    {
      var methodInfo = MemberInfoFromExpressionUtility.GetMethod ((DomainType obj) => obj.MethodDeclaredByMultipleInterfaces());
      var adapter = MethodInfoAdapter.Create (methodInfo);

      var result = adapter.FindInterfaceDeclarations ();

      var expectedMethodInfos =
          new[]
          {
            MemberInfoFromExpressionUtility.GetMethod ((IInterface1 obj) => obj.MethodDeclaredByMultipleInterfaces()),
            MemberInfoFromExpressionUtility.GetMethod ((IInterface2 obj) => obj.MethodDeclaredByMultipleInterfaces())
          };
      CheckUnorderedMethodInfos (expectedMethodInfos, result.Cast<MethodInfoAdapter>());
    }

    [Test]
    public void FindInterfaceDeclaration_ComparesMethodsWithoutReflectedTypes ()
    {
      var methodInfo = typeof (DerivedClassWithReferenceType<object>).GetMethod ("ImplicitInterfaceMethod");
      var adapter = MethodInfoAdapter.Create(methodInfo);

      Assert.That (methodInfo.ReflectedType, Is.Not.SameAs (methodInfo.DeclaringType));

      var result = adapter.FindInterfaceDeclarations();

      var expectedMethodInfos = new[] { typeof (IInterfaceWithReferenceType<object>).GetMethod ("ImplicitInterfaceMethod") };
      CheckUnorderedMethodInfos (expectedMethodInfos, result.Cast<MethodInfoAdapter> ());
    }

    [Test]
    public void FindInterfaceDeclaration_NoImplementation ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod ("TestMethod");
      var adapter = MethodInfoAdapter.Create(methodInfo);

      var result = adapter.FindInterfaceDeclarations();

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void FindDeclaringProperty_PropertyFound ()
    {
      var methodInfo = typeof (ClassWithReferenceType<object>).GetMethod ("get_ImplicitInterfaceScalar");
      var adapter = MethodInfoAdapter.Create(methodInfo);

      var result = adapter.FindDeclaringProperty();

      CheckProperty (TypeAdapter.Create (typeof (ClassWithReferenceType<object>)), "ImplicitInterfaceScalar", result);
    }

    [Test]
    public void FindDeclaringProperty_NoPropertyCanBeFound ()
    {
      var methodInfo = typeof (ClassWithBaseMember).GetMethod ("BaseMethod");
      var adapter = MethodInfoAdapter.Create(methodInfo);

      var result = adapter.FindDeclaringProperty();

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetFastInvoker_PublicMethod ()
    {
      var methodInfo = typeof (ClassWithReferenceType<string>).GetMethod ("get_ImplicitInterfaceScalar");
      var adapter = MethodInfoAdapter.Create(methodInfo);
      var instance = new ClassWithReferenceType<string>();
      instance.ImplicitInterfaceScalar = "Test";

      var result = adapter.GetFastInvoker<Func<ClassWithReferenceType<string>, string>>();

      Assert.That (result (instance), Is.EqualTo ("Test"));
    }

    [Test]
    public void GetFastInvoker_PrivateMethod ()
    {
      var methodInfo = typeof (ClassWithReferenceType<string>).GetMethod ("get_PrivateProperty", BindingFlags.Instance | BindingFlags.NonPublic);
      var adapter = MethodInfoAdapter.Create(methodInfo);
      var instance = new ClassWithReferenceType<string>();
      PrivateInvoke.SetNonPublicProperty (instance, "PrivateProperty", "Test");

      var result = adapter.GetFastInvoker<Func<ClassWithReferenceType<string>, string>>();

      Assert.That (result (instance), Is.EqualTo ("Test"));
    }

    [Test]
    public void GetFastInvoker_DerivedClassPrivateMethod_Get ()
    {
      var methodInfo = typeof (ClassWithReferenceType<string>).GetMethod ("get_PrivateProperty", BindingFlags.Instance | BindingFlags.NonPublic);
      var adapter = MethodInfoAdapter.Create(methodInfo);
      var instance = new DerivedClassWithReferenceType<string>();
      PrivateInvoke.SetNonPublicProperty (instance, "PrivateProperty", "Test");

      var result = adapter.GetFastInvoker<Func<ClassWithReferenceType<string>, string>>();

      Assert.That (result.GetType(), Is.EqualTo (typeof (Func<ClassWithReferenceType<string>, string>)));
      Assert.That (result (instance), Is.EqualTo ("Test"));
    }

    [Test]
    public void GetFastInvoker_DerivedClassPrivateMethod_Set ()
    {
      var methodInfo = typeof (ClassWithReferenceType<string>).GetMethod ("set_PrivateProperty", BindingFlags.Instance | BindingFlags.NonPublic);
      var adapter = MethodInfoAdapter.Create(methodInfo);
      var instance = new DerivedClassWithReferenceType<string>();
      instance.ImplicitInterfaceScalar = "Test";

      var result = adapter.GetFastInvoker<Action<ClassWithReferenceType<string>, string>>();

      result (instance, "Test");
      Assert.That (PrivateInvoke.GetNonPublicProperty (instance, "PrivateProperty"), Is.EqualTo ("Test"));
      Assert.That (result.GetType(), Is.EqualTo (typeof (Action<ClassWithReferenceType<string>, string>)));
    }

    [Test]
    public void GetParameters_MethodWithoutParameters ()
    {
      Assert.That (_adapter.GetParameters().Length, Is.EqualTo (0));
    }

    [Test]
    public void GetParameters_MethodWithParameters ()
    {
      var method = typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod ("TestMethodWithParameters");
      var adapter = MethodInfoAdapter.Create(method);

      Assert.That (adapter.GetParameters().Length, Is.EqualTo (2));
    }

    [Test]
    public void GetOriginalDeclaration_NoBaseDefinition ()
    {
      var method = typeof (object).GetMethod ("GetType");
      var adapter = MethodInfoAdapter.Create(method);

      var result = adapter.GetOriginalDeclaration();

      Assert.That (result, Is.TypeOf (typeof (MethodInfoAdapter)));
      CheckMethodInfo (method, (MethodInfoAdapter) result);
    }

    [Test]
    public void GetOriginalDeclaration_WithBaseDefinition ()
    {
      var method = typeof (DerivedClassWithReferenceType<SimpleReferenceType>).GetMethod ("get_ImplicitInterfaceScalar");
      var adapter = MethodInfoAdapter.Create(method);

      var result = adapter.GetOriginalDeclaration();

      Assert.That (result, Is.TypeOf (typeof (MethodInfoAdapter)));

      var expectedMethodInfo = typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod ("get_ImplicitInterfaceScalar");
      CheckMethodInfo (expectedMethodInfo, (MethodInfoAdapter) result);
    }

    [Test]
    public void Equals ()
    {
      Assert.That (_adapter.Equals (null), Is.False);
      Assert.That (_adapter.Equals ("test"), Is.False);
      Assert.That (_adapter.Equals (MethodInfoAdapter.Create(typeof (ClassWithOverridingMember).GetMethod ("BaseMethod"))), Is.False);

      Assert.That (_adapter.Equals (MethodInfoAdapter.Create(_method)), Is.True);
      Assert.That (_adapter.Equals (MethodInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod ("TestMethod"))), Is.True);
    }

    [Test]
    public void GetHashcode ()
    {
      Assert.That (
          _adapter.GetHashCode(),
          Is.EqualTo (MethodInfoAdapter.Create(typeof (ClassWithReferenceType<SimpleReferenceType>).GetMethod ("TestMethod")).GetHashCode()));
      Assert.That (
          MethodInfoAdapter.Create(typeof (int[]).GetMethod ("ToString")).GetHashCode(),
          Is.EqualTo (MethodInfoAdapter.Create(typeof (int[]).GetMethod ("ToString")).GetHashCode()));
    }

    [Test]
    public void To_String ()
    {
      Assert.That (_adapter.ToString(), Is.EqualTo ("Void TestMethod()"));
    }

    [Test]
    public void IsNull ()
    {
      Assert.That (((IMethodInformation) _adapter).IsNull, Is.False);
    }

    [Test]
    public void IsSupportedByTypeConversionProvider ()
    {
      var typeConversionProvider = SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>();

      Assert.That (typeConversionProvider.CanConvert (typeof (MethodInfoAdapter), typeof (MethodInfo)), Is.True);
    }

    private void CheckProperty (ITypeInformation expectedDeclaringType, string expectedName, IPropertyInformation actualProperty)
    {
      Assert.That (actualProperty.Name, Is.EqualTo (expectedName));
      Assert.That (actualProperty.DeclaringType, Is.SameAs (expectedDeclaringType));
    }

    // Cannot compare methods using MethodInfo.Equals because MethodInfoAdapter doesn't take the ReflectedType into account, 
    // whereas MethodInfo.Equals does. Therefore, use equality comparer instead.
    private void CheckMethodInfo (MethodInfo expectedMethodInfo, MethodInfoAdapter methodInfoAdapter)
    {
      var actualMethodInfo = methodInfoAdapter.MethodInfo;
      Assert.That (MemberInfoEqualityComparer<MethodInfo>.Instance.Equals (expectedMethodInfo, actualMethodInfo), Is.True);
    }

    private void CheckUnorderedMethodInfos (IEnumerable<MethodInfo> expectedMethodInfos, IEnumerable<MethodInfoAdapter> actualMethodInfoAdapters)
    {
      var orderedExpected = expectedMethodInfos.OrderBy (mi => mi.DeclaringType.FullName).ToArray ();
      var orderedActual = actualMethodInfoAdapters.OrderBy (mi => mi.DeclaringType.FullName).ToArray ();

      Assert.That (orderedActual, Has.Length.EqualTo (orderedExpected.Length));
      for (int i = 0; i < orderedActual.Length; i++)
        CheckMethodInfo (orderedExpected[i], orderedActual[i]);
    }

    public class DomainType : IInterface1, IInterface2
    {
      public void MethodDeclaredByMultipleInterfaces ()
      {
        throw new NotImplementedException ();
      }
    }

    public interface IInterface1
    {
      void MethodDeclaredByMultipleInterfaces ();
    }

    public interface IInterface2
    {
      void MethodDeclaredByMultipleInterfaces ();
    }
  }
}