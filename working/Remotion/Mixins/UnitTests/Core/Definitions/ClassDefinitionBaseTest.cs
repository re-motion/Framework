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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.Definitions.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.Definitions
{
  [TestFixture]
  public class ClassDefinitionBaseTest
  {
    private TestClassDefinition _classDefinition1;
    private TestClassDefinition _classDefinition2;
    private MethodInfo _methodInfo1;
    private MethodInfo _methodInfo2;
    private MethodInfo _methodInfo3;
    private MethodInfo _methodInfoProtected;
    private MethodInfo _methodInfoProtectedInternal;
    private PropertyInfo _propertyInfoWithGetAndSet;
    private PropertyInfo _propertyInfoWithGet;
    private PropertyInfo _propertyInfoWithSet;
    private EventInfo _eventInfo1;
    private EventInfo _eventInfo2;

    [SetUp]
    public void SetUp ()
    {
      _classDefinition1 = new TestClassDefinition (typeof (BaseType1));
      _classDefinition2 = new TestClassDefinition (typeof (BT1Mixin1));
      
      _methodInfo1 = typeof (BaseType1).GetMethod ("VirtualMethod", Type.EmptyTypes);
      _methodInfo2 = typeof (BT1Mixin1).GetMethod ("VirtualMethod");
      _methodInfo3 = typeof (BT1Mixin2).GetMethod ("VirtualMethod");
      _methodInfoProtected = typeof (ClassWithDifferentMemberVisibilities).GetMethod (
          "ProtectedMethod", 
          BindingFlags.NonPublic | BindingFlags.Instance);
      _methodInfoProtectedInternal = typeof (ClassWithDifferentMemberVisibilities).GetMethod (
          "ProtectedInternalMethod", 
          BindingFlags.NonPublic | BindingFlags.Instance);

      _propertyInfoWithGetAndSet = typeof (ClassWithDifferentPropertyKinds).GetProperty ("PropertyWithGetAndSet");
      _propertyInfoWithGet = typeof (ClassWithDifferentPropertyKinds).GetProperty ("PropertyWithGet");
      _propertyInfoWithSet = typeof (ClassWithDifferentPropertyKinds).GetProperty ("PropertyWithSet");

      _eventInfo1 = typeof (ClassWithEvents).GetEvent ("Event1");
      _eventInfo2 = typeof (ClassWithEvents).GetEvent ("Event2");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The type Remotion.Mixins.UnitTests.Core.TestDomain.GenericTargetClass`1[T] "
        + "contains generic parameters, which is not allowed.\r\nParameter name: type")]
    public void Initialization_WithGenericParameters ()
    {
      new TestClassDefinition (typeof (GenericTargetClass<>));
    }

    [Test]
    public void GetAdjustedInterfaceMap_MethodDeclaredOnThisType ()
    {
      var classDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (DerivedImplementingInterface));
      var mapping = classDefinition.GetAdjustedInterfaceMap (typeof (InterfaceImplementedByDerived));
      Assert.That (GetTargetMethod (mapping, "Void Bar()"), Is.EqualTo (typeof (DerivedImplementingInterface).GetMethod ("Bar")));
    }

    [Test]
    public void GetAdjustedInterfaceMap_MethodDeclaredOnBaseType ()
    {
      var classDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (DerivedImplementingInterface));
      var mapping = classDefinition.GetAdjustedInterfaceMap (typeof (InterfaceImplementedByDerived));
      Assert.That (GetTargetMethod (mapping, "Void Foo()"), Is.EqualTo (typeof (BaseWithDerivedImplementingInterface).GetMethod ("Foo")));
    }

    [Test]
    public void GetdjustedInterfaceMap_MethodDeclaredOnThisType_NonGenericOverload ()
    {
      var classDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (DerivedImplementingInterface));
      var mapping = classDefinition.GetAdjustedInterfaceMap (typeof (InterfaceWithGenericOverloadsImplementedByDerived));

      var expected = typeof (DerivedImplementingInterface).GetMethods ().Where (m => m.ToString () == "Void GBar()").Single ();
      Assert.That (GetTargetMethod (mapping, "Void GBar()"), Is.EqualTo (expected));
    }

    [Test]
    public void GetAdjustedInterfaceMap_MethodDeclaredOnThisType_GenericOverload ()
    {
      var classDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (DerivedImplementingInterface));
      var mapping = classDefinition.GetAdjustedInterfaceMap (typeof (InterfaceWithGenericOverloadsImplementedByDerived));

      var expected = typeof (DerivedImplementingInterface).GetMethods ().Where (m => m.ToString () == "Void GBar[T1]()").Single ();
      Assert.That (GetTargetMethod (mapping, "Void GBar[T]()"), Is.EqualTo (expected));
    }

    [Test]
    public void GetAdjustedInterfaceMap_MethodDeclaredOnBaseType_NonGenericOverload ()
    {
      var classDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (DerivedImplementingInterface));
      var mapping = classDefinition.GetAdjustedInterfaceMap (typeof (InterfaceWithGenericOverloadsImplementedByDerived));

      var expected = typeof (BaseWithDerivedImplementingInterface).GetMethods ().Where (m => m.ToString () == "Void GFoo()").Single ();
      Assert.That (GetTargetMethod (mapping, "Void GFoo()"), Is.EqualTo (expected));
    }

    [Test]
    public void GetAdjustedInterfaceMap_MethodDeclaredOnBaseType_GenericOverload ()
    {
      var classDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (DerivedImplementingInterface));
      var mapping = classDefinition.GetAdjustedInterfaceMap (typeof (InterfaceWithGenericOverloadsImplementedByDerived));

      var expected = typeof (BaseWithDerivedImplementingInterface).GetMethods ().Where (m => m.ToString () == "Void GFoo[T2]()").Single ();
      Assert.That (GetTargetMethod (mapping, "Void GFoo[T]()"), Is.EqualTo (expected));
    }

    [Test]
    public void GetAllMembers ()
    {
      var methodDefinition1 = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfo1);
      var propertyDefinition1 = DefinitionObjectMother.CreatePropertyDefinition (_classDefinition1, _propertyInfoWithGetAndSet);
      var eventDefinition1 = DefinitionObjectMother.CreateEventDefinition (_classDefinition1, _eventInfo1);

      Assert.That (_classDefinition1.GetAllMembers ().ToArray (),
          Is.EquivalentTo (new MemberDefinitionBase[] { methodDefinition1, propertyDefinition1, eventDefinition1 }));
    }

    [Test]
    public void GetAllMethods_FindsMethods ()
    {
      var methodDefinition1 = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfo1);
      var methodDefinition2 = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfo2);

      Assert.That (_classDefinition1.GetAllMethods ().ToArray (), Is.EquivalentTo (new[] { methodDefinition1, methodDefinition2 }));
    }

    [Test]
    public void GetAllMethods_FindsPropertyAccessors ()
    {
      var propertyDefinition1 = DefinitionObjectMother.CreatePropertyDefinition (_classDefinition1, _propertyInfoWithGetAndSet);
      var propertyDefinition2 = DefinitionObjectMother.CreatePropertyDefinition (_classDefinition1, _propertyInfoWithGet);
      var propertyDefinition3 = DefinitionObjectMother.CreatePropertyDefinition (_classDefinition1, _propertyInfoWithSet);

      Assert.That (_classDefinition1.GetAllMethods ().ToArray (), Is.EquivalentTo (new[] { 
          propertyDefinition1.GetMethod, 
          propertyDefinition1.SetMethod, 
          propertyDefinition2.GetMethod, 
          propertyDefinition3.SetMethod }));
    }

    [Test]
    public void GetAllMethods_FindsEventAccessors ()
    {
      var eventDefinition1 = DefinitionObjectMother.CreateEventDefinition (_classDefinition1, _eventInfo1);
      var eventDefinition2 = DefinitionObjectMother.CreateEventDefinition (_classDefinition1, _eventInfo2);

      Assert.That (_classDefinition1.GetAllMethods ().ToArray(), Is.EquivalentTo (new[] { 
          eventDefinition1.AddMethod, 
          eventDefinition1.RemoveMethod, 
          eventDefinition2.AddMethod, 
          eventDefinition2.RemoveMethod }));
    }

    [Test]
    public void Accept ()
    {
      var methodDefinition = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfo1);
      var propertyDefinition = DefinitionObjectMother.CreatePropertyDefinition (_classDefinition1, _propertyInfoWithGetAndSet);
      var eventDefinition = DefinitionObjectMother.CreateEventDefinition (_classDefinition1, _eventInfo1);
      var attributeDefinition = DefinitionObjectMother.CreateAttributeDefinition (_classDefinition1);
      
      var visitorMock = MockRepository.GenerateMock<IDefinitionVisitor> ();
      using (visitorMock.GetMockRepository ().Ordered ())
      {
        visitorMock.Expect (mock => mock.Visit (methodDefinition));
        visitorMock.Expect (mock => mock.Visit (propertyDefinition));
        visitorMock.Expect (mock => mock.Visit (eventDefinition));
        visitorMock.Expect (mock => mock.Visit (attributeDefinition));
      }

      visitorMock.Replay ();

      _classDefinition1.Accept (visitorMock);
      Assert.That (_classDefinition1.ChildSpecificAcceptCalled, Is.True);

      visitorMock.VerifyAllExpectations ();
    }

    [Test]
    public void HasOverriddenMembers_True ()
    {
      var overriddenMember = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfo1);
      var overriderMember = DefinitionObjectMother.CreateMethodDefinition (_classDefinition2, _methodInfo2);
      DefinitionObjectMother.DeclareOverride (overriderMember, overriddenMember);

      Assert.That (_classDefinition1.HasOverriddenMembers (), Is.True);
    }

    [Test]
    public void HasOverriddenMembers_False_NoMembers ()
    {
      Assert.That (_classDefinition1.HasOverriddenMembers (), Is.False);
    }

    [Test]
    public void HasOverriddenMembers_False_NoOverrides ()
    {
      DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfo1);
      Assert.That (_classDefinition1.HasOverriddenMembers (), Is.False);
    }

    [Test]
    public void HasProtectedOverriders_True ()
    {
      var overrider = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfoProtected);
      var overridden = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfo2);
      DefinitionObjectMother.DeclareOverride (overrider, overridden);

      Assert.That (_classDefinition1.HasProtectedOverriders (), Is.True);
    }

    [Test]
    public void HasProtectedOverriders_True_ProtectedInternal ()
    {
      var overrider = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfoProtectedInternal);
      var overridden = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfo2);
      DefinitionObjectMother.DeclareOverride (overrider, overridden);

      Assert.That (_classDefinition1.HasProtectedOverriders (), Is.True);
    }

    [Test]
    public void HasProtectedOverriders_False ()
    {
      var overrider = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfo1);
      var overridden = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfo2);
      DefinitionObjectMother.DeclareOverride (overrider, overridden);

      Assert.That (_classDefinition1.HasProtectedOverriders (), Is.False);
    }

    [Test]
    public void GetProtectedOverriders_True ()
    {
      var overrider1 = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfoProtected);
      var overrider2 = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfo1);
      var overridden1 = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfo2);
      var overridden2 = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfo3);
      
      DefinitionObjectMother.DeclareOverride (overrider1, overridden1);
      DefinitionObjectMother.DeclareOverride (overrider2, overridden2);

      Assert.That (_classDefinition1.GetProtectedOverriders ().ToArray(), Is.EqualTo (new[] { overrider1 }));
    }

    [Test]
    public void GetProtectedOverriders_True_ProtectedInternal ()
    {
      var overrider = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfoProtectedInternal);
      var overridden = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfo2);
      DefinitionObjectMother.DeclareOverride (overrider, overridden);

      Assert.That (_classDefinition1.GetProtectedOverriders ().ToArray (), Is.EqualTo (new[] { overrider }));
    }

    [Test]
    public void GetProtectedOverriders_False ()
    {
      var overrider = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfo1);
      var overridden = DefinitionObjectMother.CreateMethodDefinition (_classDefinition1, _methodInfo2);
      DefinitionObjectMother.DeclareOverride (overrider, overridden);

      Assert.That (_classDefinition1.GetProtectedOverriders ().ToArray (), Is.Empty);
    }

    private MethodInfo GetTargetMethod (InterfaceMapping mapping, string interfaceMethodSignature)
    {
      for (int i = 0; i < mapping.InterfaceMethods.Length; ++i)
      {
        if (mapping.InterfaceMethods[i].ToString () == interfaceMethodSignature)
          return mapping.TargetMethods[i];
      }

      Assert.Fail ("Method '" + interfaceMethodSignature + "' not found.");
      throw new NotImplementedException ();
    }
  }
}
