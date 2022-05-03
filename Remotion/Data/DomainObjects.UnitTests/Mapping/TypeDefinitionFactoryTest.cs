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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting.Enumerables;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class TypeDefinitionFactoryTest
  {
    public class GenericBaseClass<T> : DomainObject
    {
    }

    public class SpecificSubClass1 : GenericBaseClass<string>
    {
    }

    public class SpecificSubClass2 : GenericBaseClass<int>
    {
    }

    public interface ITestInterfaceBase : IDomainObject
    {
    }

    public class ClassImplementsInterface : DomainObject, ITestInterfaceBase
    {
    }

    public interface ITestInterfaceWithTwoImplementations : ITestInterfaceBase
    {
    }

    public interface ITestInterfaceExcludedInMapping
    {
    }

    public class TestBaseClassWithInterfaces : DomainObject, ITestInterfaceWithTwoImplementations
    {
    }

    [TestDomain]
    public class TestClassWithInheritanceRoot : TestBaseClassWithInterfaces, ITestInterfaceExcludedInMapping
    {
    }

    public class TestClassWithoutInheritanceRoot : TestBaseClassWithInterfaces
    {
    }

    private TypeDefinitionFactory _typeDefinitionFactory;
    private Mock<IMappingObjectFactory> _mappingObjectFactoryMock;
    private ClassDefinition _fakeClassDefinition;

    [SetUp]
    public void SetUp ()
    {
      _mappingObjectFactoryMock = new Mock<IMappingObjectFactory>(MockBehavior.Strict);
      _typeDefinitionFactory = new TypeDefinitionFactory(_mappingObjectFactoryMock.Object);
      _fakeClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(Order));
    }

    [Test]
    public void CreateTypeDefinitions ()
    {
      _mappingObjectFactoryMock
          .Setup(_ => _.CreateClassDefinition(typeof(Order), null, It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty())))
          .Returns(_fakeClassDefinition)
          .Verifiable();

      var typeDefinitions = _typeDefinitionFactory.CreateTypeDefinitions(new[] { typeof(Order) });

      _mappingObjectFactoryMock.Verify();

      Assert.That(typeDefinitions, Is.EqualTo(new[] { _fakeClassDefinition }));
    }

    [Test]
    public void CreateTypeDefinitions_NoTypes ()
    {
      var typeDefinitions = _typeDefinitionFactory.CreateTypeDefinitions(new Type[0]);

      Assert.That(typeDefinitions, Is.Empty);
    }

    [Test]
    public void CreateTypeDefinitions_DerivedClassAreSet ()
    {
      var fakeClassDefinitionCompany = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(Company));
      var fakeClassDefinitionPartner = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Partner), baseClass: fakeClassDefinitionCompany);
      var fakeClassDefinitionCustomer = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Customer), baseClass: fakeClassDefinitionCompany);

      _mappingObjectFactoryMock
          .Setup(_ => _.CreateClassDefinition(typeof(Order), null, It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty())))
          .Returns(_fakeClassDefinition);
      _mappingObjectFactoryMock
          .Setup(_ => _.CreateClassDefinition(typeof(Company), null, It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty())))
          .Returns(fakeClassDefinitionCompany);
      _mappingObjectFactoryMock
          .Setup(_ => _.CreateClassDefinition(typeof(Partner), fakeClassDefinitionCompany, It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty())))
          .Returns(fakeClassDefinitionPartner);
      _mappingObjectFactoryMock
          .Setup(_ => _.CreateClassDefinition(typeof(Customer), fakeClassDefinitionCompany, It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty())))
          .Returns(fakeClassDefinitionCustomer);

      var typeDefinitions =
          _typeDefinitionFactory.CreateTypeDefinitions(
              new[] { typeof(Order), typeof(Company), typeof(Partner), typeof(Customer) });

      Assert.That(typeDefinitions, Is.EqualTo(new[] { fakeClassDefinitionCompany, fakeClassDefinitionCustomer, _fakeClassDefinition, fakeClassDefinitionPartner }));

      var orderClassDefinition = (ClassDefinition)typeDefinitions.Single(cd => cd.Type == typeof(Order));
      Assert.That(orderClassDefinition.DerivedClasses.Count, Is.EqualTo(0));

      var companyClassDefinition = (ClassDefinition)typeDefinitions.Single(cd => cd.Type == typeof(Company));
      Assert.That(companyClassDefinition.DerivedClasses, Is.EquivalentTo(new[] { fakeClassDefinitionPartner, fakeClassDefinitionCustomer }));
    }

    [Test]
    public void CreateTypeDefinitions_WithGenericBaseType_UsesGenericTypeDefinition ()
    {
      var specificSubClass1 = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(SpecificSubClass1));
      var specificSubClass2 = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(SpecificSubClass2));

      _mappingObjectFactoryMock
          .Setup(_ => _.CreateClassDefinition(typeof(GenericBaseClass<>), null, It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty())))
          .Returns((ClassDefinition)null);
      _mappingObjectFactoryMock
          .Setup(_ => _.CreateClassDefinition(typeof(SpecificSubClass1), null, It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty())))
          .Returns(specificSubClass1);
      _mappingObjectFactoryMock
          .Setup(_ => _.CreateClassDefinition(typeof(SpecificSubClass2), null, It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty())))
          .Returns(specificSubClass2);

      var typeDefinitions = _typeDefinitionFactory.CreateTypeDefinitions(new[] { typeof(SpecificSubClass1), typeof(SpecificSubClass2) });
      Assert.That(typeDefinitions, Is.EqualTo(new[] { specificSubClass1, specificSubClass2 }));
    }

    [Test]
    public void CreateTypeDefinitions_WithNonClassOrInterface_ReturnsNoTypeDefinition ()
    {
      Assert.That(
          () => _typeDefinitionFactory.CreateTypeDefinitions(new[] { typeof(TypeCode) }),
          Is.Empty);
    }

    [Test]
    public void CreateTypeDefinitions_WithInterface ()
    {
      var interfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(ITestInterfaceBase));

      _mappingObjectFactoryMock
          .Setup(_ => _.CreateInterfaceDefinition(typeof(ITestInterfaceBase), It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty())))
          .Returns(interfaceDefinition);

      var typeDefinitions = _typeDefinitionFactory.CreateTypeDefinitions(new[] { typeof(ITestInterfaceBase) });

      Assert.That(typeDefinitions, Is.EqualTo(new[] { interfaceDefinition }));
    }

    [Test]
    public void CreateTypeDefinitions_WithInterfaceAndClass ()
    {
      var interfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(ITestInterfaceBase));
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(ClassImplementsInterface), implementedInterfaces: new[] { interfaceDefinition });

      _mappingObjectFactoryMock
          .Setup(_ => _.CreateInterfaceDefinition(typeof(ITestInterfaceBase), It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty())))
          .Returns(interfaceDefinition);

      _mappingObjectFactoryMock
          .Setup(_ => _.CreateClassDefinition(typeof(ClassImplementsInterface), null, It.Is<IEnumerable<InterfaceDefinition>>(e => e.SequenceEqual(new[] { interfaceDefinition }))))
          .Returns(classDefinition);

      var typeDefinitions = _typeDefinitionFactory.CreateTypeDefinitions(new[] { typeof(ITestInterfaceBase), typeof(ClassImplementsInterface) });

      Assert.That(typeDefinitions, Is.EqualTo(new TypeDefinition[] { classDefinition, interfaceDefinition }));
    }

    [Test]
    public void CreateTypeDefinitionCollection_InterfacesArePulledIntoInheritanceRoots ()
    {
      var testInterfaceBase = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(ITestInterfaceBase));
      var testInterfaceWithTwoImplementations = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(
          typeof(ITestInterfaceWithTwoImplementations),
          extendedInterfaces: new[] { testInterfaceBase });
      var testClassWithInheritanceRoot = ClassDefinitionObjectMother.CreateClassDefinition(
          classType: typeof(TestClassWithInheritanceRoot),
          implementedInterfaces: new[] { testInterfaceWithTwoImplementations, testInterfaceBase });

      _mappingObjectFactoryMock
          .Setup(_ => _.CreateInterfaceDefinition(typeof(ITestInterfaceBase), It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty())))
          .Returns(testInterfaceBase);
      _mappingObjectFactoryMock
          .Setup(
              _ => _.CreateInterfaceDefinition(
                  typeof(ITestInterfaceWithTwoImplementations),
                  It.Is<IEnumerable<InterfaceDefinition>>(e => e.SequenceEqual(new[] { testInterfaceBase }))))
          .Returns(testInterfaceWithTwoImplementations);
      _mappingObjectFactoryMock
          .Setup(
              _ => _.CreateClassDefinition(
                  typeof(TestClassWithInheritanceRoot),
                  null,
                  It.Is<IEnumerable<InterfaceDefinition>>(e => e.SequenceEqual(new[] { testInterfaceWithTwoImplementations, testInterfaceBase }))))
          .Returns(testClassWithInheritanceRoot);

      var typeDefinitions =
          _typeDefinitionFactory.CreateTypeDefinitions(
              new[] { typeof(ITestInterfaceBase), typeof(ITestInterfaceWithTwoImplementations), typeof(TestBaseClassWithInterfaces), typeof(TestClassWithInheritanceRoot) });

      Assert.That(typeDefinitions, Is.EquivalentTo(new TypeDefinition[] { testInterfaceBase, testInterfaceWithTwoImplementations, testClassWithInheritanceRoot }));
      Assert.That(testInterfaceBase.ExtendingInterfaces, Is.EqualTo(new[] { testInterfaceWithTwoImplementations }));
      Assert.That(testInterfaceBase.ImplementingClasses, Is.EqualTo(new[] { testClassWithInheritanceRoot }));
      Assert.That(testInterfaceWithTwoImplementations.ExtendingInterfaces, Is.Empty);
      Assert.That(testInterfaceWithTwoImplementations.ImplementingClasses, Is.EqualTo(new[] { testClassWithInheritanceRoot }));
      Assert.That(testClassWithInheritanceRoot.DerivedClasses, Is.Empty);
    }

    [Test]
    public void CreateTypeDefinitionCollection_InterfacesAreReferencedOnlyAtTheTypeDefinitionThatIntroducesIt ()
    {
      var testInterfaceBase = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(ITestInterfaceBase));
      var testInterfaceWithTwoImplementations = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(
          typeof(ITestInterfaceWithTwoImplementations),
          extendedInterfaces: new[] { testInterfaceBase });
      var testBaseClass = ClassDefinitionObjectMother.CreateClassDefinition(
          classType: typeof(TestBaseClassWithInterfaces),
          implementedInterfaces: new[] { testInterfaceWithTwoImplementations, testInterfaceBase });
      var testClassWithoutInheritanceRoot = ClassDefinitionObjectMother.CreateClassDefinition(
          classType: typeof(TestClassWithoutInheritanceRoot),
          baseClass: testBaseClass);

      _mappingObjectFactoryMock
          .Setup(_ => _.CreateInterfaceDefinition(typeof(ITestInterfaceBase), It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty())))
          .Returns(testInterfaceBase);
      _mappingObjectFactoryMock
          .Setup(
              _ => _.CreateInterfaceDefinition(
                  typeof(ITestInterfaceWithTwoImplementations),
                  It.Is<IEnumerable<InterfaceDefinition>>(e => e.SequenceEqual(new[] { testInterfaceBase }))))
          .Returns(testInterfaceWithTwoImplementations);
      _mappingObjectFactoryMock
          .Setup(
              _ => _.CreateClassDefinition(
                  typeof(TestBaseClassWithInterfaces),
                  null,
                  It.Is<IEnumerable<InterfaceDefinition>>(e => e.SequenceEqual(new[] { testInterfaceWithTwoImplementations, testInterfaceBase }))))
          .Returns(testBaseClass);
      _mappingObjectFactoryMock
          .Setup(
              _ => _.CreateClassDefinition(
                  typeof(TestClassWithoutInheritanceRoot),
                  testBaseClass,
                  It.Is<IEnumerable<InterfaceDefinition>>(e => e.IsEmpty())))
          .Returns(testClassWithoutInheritanceRoot);

      var typeDefinitions =
          _typeDefinitionFactory.CreateTypeDefinitions(
              new[] { typeof(ITestInterfaceBase), typeof(ITestInterfaceWithTwoImplementations), typeof(TestBaseClassWithInterfaces), typeof(TestClassWithoutInheritanceRoot) });

      Assert.That(
          typeDefinitions,
          Is.EquivalentTo(new TypeDefinition[] { testInterfaceBase, testInterfaceWithTwoImplementations, testBaseClass, testClassWithoutInheritanceRoot }));
      Assert.That(testInterfaceBase.ExtendingInterfaces, Is.EqualTo(new[] { testInterfaceWithTwoImplementations }));
      Assert.That(testInterfaceBase.ImplementingClasses, Is.EqualTo(new[] { testBaseClass }));
      Assert.That(testInterfaceWithTwoImplementations.ExtendingInterfaces, Is.Empty);
      Assert.That(testInterfaceWithTwoImplementations.ImplementingClasses, Is.EqualTo(new[] { testBaseClass }));
      Assert.That(testBaseClass.DerivedClasses, Is.EqualTo(new[] { testClassWithoutInheritanceRoot }));
      Assert.That(testClassWithoutInheritanceRoot.DerivedClasses, Is.Empty);
    }
  }
}
