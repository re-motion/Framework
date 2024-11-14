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
using Moq;
using Moq.Protected;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Reflection;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.UnitTests.Core.CodeGeneration.TestDomain;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Reflection.CodeGeneration;
using Remotion.Utilities;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  [TestFixture]
  public class AttributeBasedMetadataImporterTest
  {
    [Test]
    public void GetMetadataForMixedType_Wrapper ()
    {
      var importerMock = new Mock<AttributeBasedMetadataImporter>() { CallBase = true };
      var expectedResult = ClassContextObjectMother.Create(typeof(object));
      importerMock.Setup(mock => mock.GetMetadataForMixedType(typeof(object))).Returns(expectedResult).Verifiable();

      var result = importerMock.Object.GetMetadataForMixedType(typeof(object));

      Assert.That(result, Is.SameAs(expectedResult));
      importerMock.Verify();
    }

    [Test]
    public void GetMetadataForMixinType_Identifier ()
    {
      var importerMock = new Mock<AttributeBasedMetadataImporter>() { CallBase = true };
      var expectedIdentifier = new ConcreteMixinTypeIdentifier(typeof(object), new HashSet<MethodInfo>(), new HashSet<MethodInfo>());

      SetupImporterMock(importerMock, expectedIdentifier, new Dictionary<MethodInfo, MethodInfo>(), new Dictionary<MethodInfo, MethodInfo>());

      var result = importerMock.Object.GetMetadataForMixinType(typeof(LoadableConcreteMixinTypeForMixinWithAbstractMembers));

      Assert.That(result.Identifier, Is.SameAs(expectedIdentifier));

      importerMock.Verify();
    }

    [Test]
    public void GetMetadataForMixinType_GeneratedType ()
    {
      var importerMock = new Mock<AttributeBasedMetadataImporter>() { CallBase = true };
      var expectedIdentifier = new ConcreteMixinTypeIdentifier(typeof(object), new HashSet<MethodInfo>(), new HashSet<MethodInfo>());

      SetupImporterMock(importerMock, expectedIdentifier, new Dictionary<MethodInfo, MethodInfo>(), new Dictionary<MethodInfo, MethodInfo>());

      var result = importerMock.Object.GetMetadataForMixinType(typeof(LoadableConcreteMixinTypeForMixinWithAbstractMembers));

      Assert.That(result.GeneratedType, Is.SameAs(typeof(LoadableConcreteMixinTypeForMixinWithAbstractMembers)));

      importerMock.Verify();
    }

    [Test]
    public void GetMetadataForMixinType_GeneratedOverrideInterface ()
    {
      var importerMock = new Mock<AttributeBasedMetadataImporter>() { CallBase = true };
      var expectedIdentifier = new ConcreteMixinTypeIdentifier(typeof(object), new HashSet<MethodInfo>(), new HashSet<MethodInfo>());

      SetupImporterMock(importerMock, expectedIdentifier, new Dictionary<MethodInfo, MethodInfo>(), new Dictionary<MethodInfo, MethodInfo>());

      var result = importerMock.Object.GetMetadataForMixinType(typeof(LoadableConcreteMixinTypeForMixinWithAbstractMembers));

      Assert.That(result.GeneratedOverrideInterface, Is.SameAs(typeof(LoadableConcreteMixinTypeForMixinWithAbstractMembers.IOverriddenMethods)));

      importerMock.Verify();
    }

    [Test]
    public void GetMetadataForMixinType_NoOverrideInterface ()
    {
      var importerMock = new Mock<AttributeBasedMetadataImporter>() { CallBase = true };
      var expectedIdentifier = new ConcreteMixinTypeIdentifier(typeof(object), new HashSet<MethodInfo>(), new HashSet<MethodInfo>());

      importerMock.Setup(mock => mock.GetIdentifierForMixinType(typeof(object))).Returns(expectedIdentifier).Verifiable();
      Assert.That(
          () => importerMock.Object.GetMetadataForMixinType(typeof(object)),
          Throws.InstanceOf<TypeImportException>()
              .With.Message.EqualTo(
                  "The given type 'System.Object' has a concrete mixin type identifier, but no "
                  + "IOverriddenMethods interface."));
    }

    [Test]
    public void GetMetadataForMixinType_Wrappers ()
    {
      var importerMock = new Mock<AttributeBasedMetadataImporter>() { CallBase = true };
      var expectedIdentifier = new ConcreteMixinTypeIdentifier(typeof(object), new HashSet<MethodInfo>(), new HashSet<MethodInfo>());

      var method1 = ReflectionObjectMother.GetSomeNonPublicMethod();
      var method2 = typeof(DateTime).GetMethod("get_UTicks", BindingFlags.NonPublic | BindingFlags.Instance);
      var wrapper1 = typeof(DateTime).GetMethod("get_Month");
      var wrapper2 = typeof(DateTime).GetMethod("get_Year");

      Assertion.IsNotNull(method1, "method1 != null");
      Assertion.IsNotNull(method2, "method2 != null");
      Assertion.IsNotNull(wrapper1, "wrapper1 != null");
      Assertion.IsNotNull(wrapper2, "wrapper2 != null");

      SetupImporterMock(
          importerMock,
          expectedIdentifier,
          new Dictionary<MethodInfo, MethodInfo>(),
          new Dictionary<MethodInfo, MethodInfo> { { method1, wrapper1 }, { method2, wrapper2} });

      var result = importerMock.Object.GetMetadataForMixinType(typeof(LoadableConcreteMixinTypeForMixinWithAbstractMembers));
      Assert.That(result.GetPubliclyCallableMixinMethod(method1), Is.EqualTo(wrapper1));
      Assert.That(result.GetPubliclyCallableMixinMethod(method2), Is.EqualTo(wrapper2));

      importerMock.Verify();
    }

    [Test]
    public void GetMetadataForMixinType_OverrideInterfaceMethods ()
    {
      var importerMock = new Mock<AttributeBasedMetadataImporter>() { CallBase = true };
      var expectedIdentifier = new ConcreteMixinTypeIdentifier(typeof(object), new HashSet<MethodInfo>(), new HashSet<MethodInfo>());

      var mixinMethod1 = typeof(DateTime).GetMethod("get_Now");
      var mixinMethod2 = typeof(DateTime).GetMethod("get_Day");
      var ifcMethod1 = typeof(DateTime).GetMethod("get_Month");
      var ifcMethod2 = typeof(DateTime).GetMethod("get_Year");

      SetupImporterMock(
          importerMock,
          expectedIdentifier,
          new Dictionary<MethodInfo, MethodInfo> { { mixinMethod1, ifcMethod1 }, { mixinMethod2, ifcMethod2 } },
          new Dictionary<MethodInfo, MethodInfo>());

      var result = importerMock.Object.GetMetadataForMixinType(typeof(LoadableConcreteMixinTypeForMixinWithAbstractMembers));
      Assert.That(result.GetOverrideInterfaceMethod(mixinMethod1), Is.SameAs(ifcMethod1));
      Assert.That(result.GetOverrideInterfaceMethod(mixinMethod2), Is.SameAs(ifcMethod2));

      importerMock.Verify();
    }

    [Test]
    public void GetMetadataForMixinType_NoAttribute ()
    {
      var importerMock = new Mock<AttributeBasedMetadataImporter>() { CallBase = true };

      importerMock
          .Setup(mock => mock.GetIdentifierForMixinType(typeof(LoadableConcreteMixinTypeForMixinWithAbstractMembers)))
          .Returns((ConcreteMixinTypeIdentifier)null)
          .Verifiable();

      var result = importerMock.Object.GetMetadataForMixinType(typeof(LoadableConcreteMixinTypeForMixinWithAbstractMembers));
      Assert.That(result, Is.Null);

      importerMock.Verify();
    }

    [Test]
    public void GetMetadataForMixedType ()
    {
      var classContext1 = ClassContextObjectMother.Create(typeof(object));
      var type = typeof(ClassWithConcreteMixedTypeAttribute);
      var importer = new AttributeBasedMetadataImporter();

      var result = importer.GetMetadataForMixedType(type);

      Assert.That(result, Is.EqualTo(classContext1));
    }

    [Test]
    public void GetMetadataForMixedType_WithAttributeOnBaseType ()
    {
      var type = typeof(DerivedClassWithConcreteMixedTypeAttributeOnBaseClass);
      var importer = new AttributeBasedMetadataImporter();

      var result = importer.GetMetadataForMixedType(type);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetMetadataForMixedType_NoAttribute ()
    {
      var type = typeof(ClassWithNoAttributes);
      var importer = new AttributeBasedMetadataImporter();

      var result = importer.GetMetadataForMixedType(type);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetIdentifierForMixinType ()
    {
      var identifier = new ConcreteMixinTypeIdentifier(typeof(object), new HashSet<MethodInfo>(), new HashSet<MethodInfo>());
      var type = typeof(ClassWithConcreteMixinTypeAttribute);
      var importer = new AttributeBasedMetadataImporter();

      var result = importer.GetIdentifierForMixinType(type);

      Assert.That(result, Is.EqualTo(identifier));
    }

    [Test]
    public void GetIdentifierForMixinType_WithAttributeOnBaseType ()
    {
      var type = typeof(DerivedClassWithConcreteMixinTypeAttributeOnBaseClass);
      var importer = new AttributeBasedMetadataImporter();

      var result = importer.GetIdentifierForMixinType(type);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetIdentifierForMixinType_NoAttribute ()
    {
      var type = typeof(ClassWithNoAttributes);
      var importer = new AttributeBasedMetadataImporter();

      var result = importer.GetIdentifierForMixinType(type);

      Assert.That(result, Is.Null);
    }

    [Test]
    public void GetMethodWrappersForMixinType ()
    {
      Type builtType = CreateTypeWithFakeWrappers();

      // fake wrapper methods
      var wrapperMethod1 = builtType.GetMethod("Wrapper1");
      var wrapperMethod2 = builtType.GetMethod("Wrapper2");
      var nonWrapperMethod1 = builtType.GetMethod("NonWrapper1");
      var nonWrapperMethod2 = builtType.GetMethod("NonWrapper2");

      // fake wrapped methods
      var wrappedMethod1 = typeof(DateTime).GetMethod("get_Now");
      var wrappedMethod2 = typeof(DateTime).GetMethod("get_Day");

      // fake attributes simulating the relationship between wrapper methods and wrapped methods
      var attribute1 = new GeneratedMethodWrapperAttribute(wrappedMethod1.DeclaringType, wrappedMethod1.Name, wrappedMethod1.ToString());
      var attribute2 = new GeneratedMethodWrapperAttribute(wrappedMethod2.DeclaringType, wrappedMethod2.Name, wrappedMethod2.ToString());

      // prepare importerMock.GetWrapperAttribute to return attribute1 and attribute2 for wrapperMethod1 and wrapperMethod2
      var importerMock = new Mock<AttributeBasedMetadataImporter>() { CallBase = true };
      importerMock
          .Protected()
          .Setup<GeneratedMethodWrapperAttribute>("GetWrapperAttribute", nonWrapperMethod1)
          .Returns((GeneratedMethodWrapperAttribute)null);
      importerMock
          .Protected()
          .Setup<GeneratedMethodWrapperAttribute>("GetWrapperAttribute", nonWrapperMethod2)
          .Returns((GeneratedMethodWrapperAttribute)null);
      importerMock.Protected().Setup<GeneratedMethodWrapperAttribute>("GetWrapperAttribute", wrapperMethod1).Returns(attribute1);
      importerMock.Protected().Setup<GeneratedMethodWrapperAttribute>("GetWrapperAttribute", wrapperMethod2).Returns(attribute2);

      var result = importerMock.Object.GetMethodWrappersForMixinType(builtType).ToArray();

      Assert.That(result, Is.EqualTo(new Dictionary<MethodInfo, MethodInfo> { { wrappedMethod1, wrapperMethod1 }, { wrappedMethod2, wrapperMethod2 } }));
    }

    [Test]
    public void GetOverrideInterfaceMethodsByMixinMethod ()
    {
      var importer = new AttributeBasedMetadataImporter();
      var identifier = new ConcreteMixinTypeIdentifier(typeof(MixinWithAbstractMembers), new HashSet<MethodInfo>(), new HashSet<MethodInfo>());

      var interfaceType = typeof(LoadableConcreteMixinTypeForMixinWithAbstractMembers.IOverriddenMethods);
      var result = importer.GetOverrideInterfaceMethodsByMixinMethod(interfaceType, identifier);

      var ifcMethod1 = interfaceType.GetMethod("AbstractMethod");
      var mixinMethod1 = identifier.MixinType.GetMethod("AbstractMethod", BindingFlags.NonPublic | BindingFlags.Instance);
      var ifcMethod2 = interfaceType.GetMethod("get_AbstractProperty");
      var mixinMethod2 = identifier.MixinType.GetMethod("get_AbstractProperty", BindingFlags.NonPublic | BindingFlags.Instance);
      var ifcMethod3 = interfaceType.GetMethod("add_AbstractEvent");
      var mixinMethod3 = identifier.MixinType.GetMethod("add_AbstractEvent", BindingFlags.NonPublic | BindingFlags.Instance);
      var ifcMethod4 = interfaceType.GetMethod("remove_AbstractEvent");
      var mixinMethod4 = identifier.MixinType.GetMethod("remove_AbstractEvent", BindingFlags.NonPublic | BindingFlags.Instance);
      var ifcMethod5 = interfaceType.GetMethod("RaiseEvent");
      var mixinMethod5 = identifier.MixinType.GetMethod("RaiseEvent", BindingFlags.NonPublic | BindingFlags.Instance);

      var expected = new Dictionary<MethodInfo, MethodInfo> {
        { mixinMethod1, ifcMethod1 },
        { mixinMethod2, ifcMethod2 },
        { mixinMethod3, ifcMethod3 },
        { mixinMethod4, ifcMethod4 },
        { mixinMethod5, ifcMethod5 },
      };

      Assert.That(result, Is.EquivalentTo(expected));
    }

    [Test]
    public void GetWrapperAttribute_WithResult ()
    {
      var importer = new AttributeBasedMetadataImporter();
      var method = GetType().GetMethod("FakeWrapperMethod");
      var attribute = (GeneratedMethodWrapperAttribute)PrivateInvoke.InvokeNonPublicMethod(importer, "GetWrapperAttribute", method);

      Assert.That(attribute, Is.Not.Null);
      Assert.That(attribute.DeclaringType, Is.EqualTo(typeof(DateTime)));
      Assert.That(attribute.MethodName, Is.EqualTo("get_Now"));
      Assert.That(attribute.MethodSignature, Is.EqualTo("System.DateTime get_Now()"));
    }

    [Test]
    public void GetWrapperAttribute_WithoutResult ()
    {
      var importer = new AttributeBasedMetadataImporter();
      var method = GetType().GetMethod("FakeNonWrapperMethod");
      var attribute = (GeneratedMethodWrapperAttribute)PrivateInvoke.InvokeNonPublicMethod(importer, "GetWrapperAttribute", method);

      Assert.That(attribute, Is.Null);
    }

    private void SetupImporterMock (
        Mock<AttributeBasedMetadataImporter> importerMock,
        ConcreteMixinTypeIdentifier expectedIdentifier,
        Dictionary<MethodInfo, MethodInfo> overrideInterfaceMethods,
        Dictionary<MethodInfo, MethodInfo> methodWrappers)
    {
      importerMock
          .Setup(mock => mock.GetIdentifierForMixinType(typeof(LoadableConcreteMixinTypeForMixinWithAbstractMembers)))
          .Returns(expectedIdentifier)
          .Verifiable();
      importerMock
          .Setup(mock => mock.GetOverrideInterfaceMethodsByMixinMethod(
                               typeof(LoadableConcreteMixinTypeForMixinWithAbstractMembers.IOverriddenMethods),
                               expectedIdentifier))
          .Returns(overrideInterfaceMethods)
          .Verifiable();
      importerMock
          .Setup(mock => mock.GetMethodWrappersForMixinType(typeof(LoadableConcreteMixinTypeForMixinWithAbstractMembers)))
          .Returns(methodWrappers)
          .Verifiable();
    }


    [GeneratedMethodWrapper(typeof(DateTime), "get_Now", "System.DateTime get_Now()")]
    public void FakeWrapperMethod ()
    {
    }

    public void FakeNonWrapperMethod ()
    {
    }

    private Type CreateTypeWithFakeWrappers ()
    {
      TypeBuilder wrapperClassBuilder = new AdHocCodeGenerator(TestContext.CurrentContext.TestDirectory).CreateType("WrapperClass");

      wrapperClassBuilder.DefineMethod("Wrapper1", MethodAttributes.Public).GetILGenerator().Emit(OpCodes.Ret);
      wrapperClassBuilder.DefineMethod("Wrapper2", MethodAttributes.Public).GetILGenerator().Emit(OpCodes.Ret);
      wrapperClassBuilder.DefineMethod("NonWrapper1", MethodAttributes.Public).GetILGenerator().Emit(OpCodes.Ret);
      wrapperClassBuilder.DefineMethod("NonWrapper2", MethodAttributes.Public).GetILGenerator().Emit(OpCodes.Ret);

      return wrapperClassBuilder.CreateType();
    }

    private class ClassWithNoAttributes
    {
    }

    [ConcreteMixedType(new object[] { typeof(object), new object[0], new Type[0] }, new Type[0])]
    private class ClassWithConcreteMixedTypeAttribute
    {
    }

    [ConcreteMixinType(new object[] { typeof(object), new object[0], new object[0] })]
    private class ClassWithConcreteMixinTypeAttribute
    {
    }

    private class DerivedClassWithConcreteMixedTypeAttributeOnBaseClass : ClassWithConcreteMixedTypeAttribute
    {
    }

    private class DerivedClassWithConcreteMixinTypeAttributeOnBaseClass : ClassWithConcreteMixinTypeAttribute
    {
    }
  }
}
