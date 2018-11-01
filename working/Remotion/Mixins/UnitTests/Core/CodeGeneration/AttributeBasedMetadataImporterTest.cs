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
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Reflection.CodeGeneration;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  [TestFixture]
  public class AttributeBasedMetadataImporterTest
  {
    [Test]
    public void GetMetadataForMixedType_Wrapper()
    {
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();
      var expectedResult = ClassContextObjectMother.Create(typeof (object));
      importerMock.Expect (mock => mock.GetMetadataForMixedType ((_Type) typeof (object))).Return (expectedResult);

      importerMock.Replay ();
      var result = importerMock.GetMetadataForMixedType (typeof (object));

      Assert.That (result, Is.SameAs (expectedResult));
      importerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadataForMixinType_Identifier ()
    {
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();
      var expectedIdentifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo>(), new HashSet<MethodInfo>());

      SetupImporterMock (importerMock, expectedIdentifier, new Dictionary<MethodInfo, MethodInfo>(), new Dictionary<MethodInfo, MethodInfo>());
      importerMock.Replay ();
      
      var result = importerMock.GetMetadataForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers));

      Assert.That (result.Identifier, Is.SameAs (expectedIdentifier));
      
      importerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadataForMixinType_GeneratedType ()
    {
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();
      var expectedIdentifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());
      
      SetupImporterMock (importerMock, expectedIdentifier, new Dictionary<MethodInfo, MethodInfo> (), new Dictionary<MethodInfo, MethodInfo>());
      importerMock.Replay ();

      var result = importerMock.GetMetadataForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers));

      Assert.That (result.GeneratedType, Is.SameAs (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers)));

      importerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadataForMixinType_GeneratedOverrideInterface ()
    {
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();
      var expectedIdentifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());

      SetupImporterMock (importerMock, expectedIdentifier, new Dictionary<MethodInfo, MethodInfo> (), new Dictionary<MethodInfo, MethodInfo>());
      importerMock.Replay ();

      var result = importerMock.GetMetadataForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers));

      Assert.That (result.GeneratedOverrideInterface, Is.SameAs (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers.IOverriddenMethods)));

      importerMock.VerifyAllExpectations ();
    }

    [Test]
    [ExpectedException (typeof (TypeImportException), ExpectedMessage = "The given type 'System.Object' has a concrete mixin type identifier, but no "
        + "IOverriddenMethods interface.")]
    public void GetMetadataForMixinType_NoOverrideInterface ()
    {
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();
      var expectedIdentifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());

      importerMock.Expect (mock => mock.GetIdentifierForMixinType (typeof (object))).Return (expectedIdentifier);
      importerMock.Replay ();

      importerMock.GetMetadataForMixinType (typeof (object));
    }

    [Test]
    public void GetMetadataForMixinType_Wrappers ()
    {
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();
      var expectedIdentifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());

      var method1 = ReflectionObjectMother.GetSomeNonPublicMethod();
      var method2 = typeof (DateTime).GetMethod ("get_InternalTicks", BindingFlags.NonPublic | BindingFlags.Instance);
      var wrapper1 = typeof (DateTime).GetMethod ("get_Month");
      var wrapper2 = typeof (DateTime).GetMethod ("get_Year");

      SetupImporterMock (
          importerMock, 
          expectedIdentifier, 
          new Dictionary<MethodInfo, MethodInfo> (), 
          new Dictionary<MethodInfo, MethodInfo> { { method1, wrapper1 }, { method2, wrapper2} });
      importerMock.Replay ();

      var result = importerMock.GetMetadataForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers));
      Assert.That (result.GetPubliclyCallableMixinMethod (method1), Is.EqualTo (wrapper1));
      Assert.That (result.GetPubliclyCallableMixinMethod (method2), Is.EqualTo (wrapper2));

      importerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadataForMixinType_OverrideInterfaceMethods ()
    {
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();
      var expectedIdentifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());

      var mixinMethod1 = typeof (DateTime).GetMethod ("get_Now");
      var mixinMethod2 = typeof (DateTime).GetMethod ("get_Day");
      var ifcMethod1 = typeof (DateTime).GetMethod ("get_Month");
      var ifcMethod2 = typeof (DateTime).GetMethod ("get_Year");

      SetupImporterMock (
          importerMock,
          expectedIdentifier,
          new Dictionary<MethodInfo, MethodInfo> { { mixinMethod1, ifcMethod1 }, { mixinMethod2, ifcMethod2 } },
          new Dictionary<MethodInfo, MethodInfo> ());
      importerMock.Replay ();

      var result = importerMock.GetMetadataForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers));
      Assert.That (result.GetOverrideInterfaceMethod (mixinMethod1), Is.SameAs (ifcMethod1));
      Assert.That (result.GetOverrideInterfaceMethod (mixinMethod2), Is.SameAs (ifcMethod2));

      importerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadataForMixinType_NoAttribute ()
    {
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();

      importerMock.Expect (mock => mock.GetIdentifierForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers))).Return (null);
      importerMock.Replay ();

      var result = importerMock.GetMetadataForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers));
      Assert.That (result, Is.Null);

      importerMock.VerifyAllExpectations ();
    }
    
    [Test]
    public void GetMetadataForMixedType ()
    {
      var classContext1 = ClassContextObjectMother.Create(typeof (object));

      var attribute1 = ConcreteMixedTypeAttribute.FromClassContext (classContext1, new Type[0]);

      var typeMock = MockRepository.GenerateMock<_Type> ();
      typeMock.Expect (mock => mock.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false)).Return (new[] { attribute1 });
      typeMock.Replay ();

      var importer = new AttributeBasedMetadataImporter ();
      var result = importer.GetMetadataForMixedType (typeMock);
      Assert.That (result, Is.EqualTo (classContext1));

      typeMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadataForMixedType_NoAttribute ()
    {
      var typeMock = MockRepository.GenerateMock<_Type> ();
      typeMock.Expect (mock => mock.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false)).Return (new ConcreteMixedTypeAttribute[0]);
      typeMock.Replay ();

      var importer = new AttributeBasedMetadataImporter ();
      var result = importer.GetMetadataForMixedType (typeMock);
      Assert.That (result, Is.Null);

      typeMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetIdentifierForMixinType ()
    {
      var typeMock = MockRepository.GenerateMock<_Type> ();
      var identifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo>(), new HashSet<MethodInfo>());
      var attribute = ConcreteMixinTypeAttribute.Create (identifier);

      typeMock.Expect (mock => mock.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false)).Return (new[] { attribute });
      typeMock.Replay ();

      var importer = new AttributeBasedMetadataImporter ();
      var result = importer.GetIdentifierForMixinType (typeMock);
      Assert.That (result, Is.EqualTo (identifier));

      typeMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetIdentifierForMixinType_NoAttribute ()
    {
      var typeMock = MockRepository.GenerateMock<_Type> ();
      typeMock.Expect (mock => mock.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false)).Return (new ConcreteMixinTypeAttribute[0]);
      typeMock.Replay ();

      var importer = new AttributeBasedMetadataImporter ();
      var result = importer.GetIdentifierForMixinType (typeMock);
      Assert.That (result, Is.Null);

      typeMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMethodWrappersForMixinType()
    {
      Type builtType = CreateTypeWithFakeWrappers();

      // fake wrapper methods
      var wrapperMethod1 = builtType.GetMethod ("Wrapper1");
      var wrapperMethod2 = builtType.GetMethod ("Wrapper2");
      var nonWrapperMethod1 = builtType.GetMethod ("NonWrapper1");
      var nonWrapperMethod2 = builtType.GetMethod ("NonWrapper2");
      
      // fake wrapped methods
      var wrappedMethod1 = typeof (DateTime).GetMethod ("get_Now");
      var wrappedMethod2 = typeof (DateTime).GetMethod ("get_Day");

      // fake attributes simulating the relationship between wrapper methods and wrapped methods
      var attribute1 = new GeneratedMethodWrapperAttribute (wrappedMethod1.DeclaringType, wrappedMethod1.Name, wrappedMethod1.ToString());
      var attribute2 = new GeneratedMethodWrapperAttribute (wrappedMethod2.DeclaringType, wrappedMethod2.Name, wrappedMethod2.ToString ());

      // prepare importerMock.GetWrapperAttribute to return attribute1 and attribute2 for wrapperMethod1 and wrapperMethod2
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();
      importerMock.Stub (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "GetWrapperAttribute", nonWrapperMethod1)).Return (null);
      importerMock.Stub (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "GetWrapperAttribute", nonWrapperMethod2)).Return (null);
      importerMock.Stub (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "GetWrapperAttribute", wrapperMethod1)).Return (attribute1);
      importerMock.Stub (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "GetWrapperAttribute", wrapperMethod2)).Return (attribute2);
      importerMock.Replay ();
      
      var typeMock = MockRepository.GenerateMock<_Type> ();
      typeMock.Expect (mock => mock.GetMethods (BindingFlags.Instance | BindingFlags.Public))
          .Return (new[] { nonWrapperMethod1, nonWrapperMethod2, wrapperMethod1, wrapperMethod2 });

      var result = importerMock.GetMethodWrappersForMixinType (typeMock).ToArray();
      Assert.That (result, Is.EqualTo (new Dictionary<MethodInfo, MethodInfo> { { wrappedMethod1, wrapperMethod1 }, { wrappedMethod2, wrapperMethod2 } }));
    }

    [Test]
    public void GetOverrideInterfaceMethodsByMixinMethod ()
    {
      var importer = new AttributeBasedMetadataImporter ();
      var identifier = new ConcreteMixinTypeIdentifier (typeof (MixinWithAbstractMembers), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());
      
      var interfaceType = typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers.IOverriddenMethods);
      var result = importer.GetOverrideInterfaceMethodsByMixinMethod (interfaceType, identifier);

      var ifcMethod1 = interfaceType.GetMethod ("AbstractMethod");
      var mixinMethod1 = identifier.MixinType.GetMethod ("AbstractMethod", BindingFlags.NonPublic | BindingFlags.Instance);
      var ifcMethod2 = interfaceType.GetMethod ("get_AbstractProperty");
      var mixinMethod2 = identifier.MixinType.GetMethod ("get_AbstractProperty", BindingFlags.NonPublic | BindingFlags.Instance);
      var ifcMethod3 = interfaceType.GetMethod ("add_AbstractEvent");
      var mixinMethod3 = identifier.MixinType.GetMethod ("add_AbstractEvent", BindingFlags.NonPublic | BindingFlags.Instance);
      var ifcMethod4 = interfaceType.GetMethod ("remove_AbstractEvent");
      var mixinMethod4 = identifier.MixinType.GetMethod ("remove_AbstractEvent", BindingFlags.NonPublic | BindingFlags.Instance);
      var ifcMethod5 = interfaceType.GetMethod ("RaiseEvent");
      var mixinMethod5 = identifier.MixinType.GetMethod ("RaiseEvent", BindingFlags.NonPublic | BindingFlags.Instance);

      var expected = new Dictionary<MethodInfo, MethodInfo> { 
        { mixinMethod1, ifcMethod1 }, 
        { mixinMethod2, ifcMethod2 }, 
        { mixinMethod3, ifcMethod3 }, 
        { mixinMethod4, ifcMethod4 },
        { mixinMethod5, ifcMethod5 },
      };

      Assert.That (result, Is.EquivalentTo (expected));
    }

    [Test]
    public void GetWrapperAttribute_WithResult ()
    {
      var importer = new AttributeBasedMetadataImporter();
      var method = GetType ().GetMethod ("FakeWrapperMethod");
      var attribute = (GeneratedMethodWrapperAttribute) PrivateInvoke.InvokeNonPublicMethod (importer, "GetWrapperAttribute", method);

      Assert.That (attribute, Is.Not.Null);
      Assert.That (attribute.DeclaringType, Is.EqualTo (typeof (DateTime)));
      Assert.That (attribute.MethodName, Is.EqualTo ("get_Now"));
      Assert.That (attribute.MethodSignature, Is.EqualTo ("System.DateTime get_Now()"));
    }

    [Test]
    public void GetWrapperAttribute_WithoutResult ()
    {
      var importer = new AttributeBasedMetadataImporter ();
      var method = GetType ().GetMethod ("FakeNonWrapperMethod");
      var attribute = (GeneratedMethodWrapperAttribute) PrivateInvoke.InvokeNonPublicMethod (importer, "GetWrapperAttribute", method);

      Assert.That (attribute, Is.Null);
    }

    private void SetupImporterMock (
        AttributeBasedMetadataImporter importerMock,
        ConcreteMixinTypeIdentifier expectedIdentifier,
        Dictionary<MethodInfo, MethodInfo> overrideInterfaceMethods,
        Dictionary<MethodInfo, MethodInfo> methodWrappers)
    {
      importerMock
          .Expect (mock => mock.GetIdentifierForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers)))
          .Return (expectedIdentifier);
      importerMock
          .Expect (mock => mock.GetOverrideInterfaceMethodsByMixinMethod (
                               typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers.IOverriddenMethods),
                               expectedIdentifier))
          .Return (overrideInterfaceMethods);
      importerMock
          .Expect (mock => mock.GetMethodWrappersForMixinType (typeof (LoadableConcreteMixinTypeForMixinWithAbstractMembers)))
          .Return (methodWrappers);
    }


    [GeneratedMethodWrapper (typeof (DateTime), "get_Now", "System.DateTime get_Now()")]
    public void FakeWrapperMethod ()
    {
    }

    public void FakeNonWrapperMethod ()
    {
    }

    private Type CreateTypeWithFakeWrappers ()
    {
      TypeBuilder wrapperClassBuilder = new AdHocCodeGenerator().CreateType ("WrapperClass");

      wrapperClassBuilder.DefineMethod ("Wrapper1", MethodAttributes.Public).GetILGenerator ().Emit (OpCodes.Ret);
      wrapperClassBuilder.DefineMethod ("Wrapper2", MethodAttributes.Public).GetILGenerator ().Emit (OpCodes.Ret);
      wrapperClassBuilder.DefineMethod ("NonWrapper1", MethodAttributes.Public).GetILGenerator ().Emit (OpCodes.Ret);
      wrapperClassBuilder.DefineMethod ("NonWrapper2", MethodAttributes.Public).GetILGenerator ().Emit (OpCodes.Ret);

      return wrapperClassBuilder.CreateType ();
    }
  }
}
