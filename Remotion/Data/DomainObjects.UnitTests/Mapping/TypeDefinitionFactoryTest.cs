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
          .Setup(mock => mock.CreateClassDefinition(typeof(Order), null, It.Is<IEnumerable<InterfaceDefinition>>(e => !e.Any())))
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
          .Setup(mock => mock.CreateClassDefinition(typeof(Order), null, It.Is<IEnumerable<InterfaceDefinition>>(e => !e.Any())))
          .Returns(_fakeClassDefinition);
      _mappingObjectFactoryMock
          .Setup(mock => mock.CreateClassDefinition(typeof(Company), null, It.Is<IEnumerable<InterfaceDefinition>>(e => !e.Any())))
          .Returns(fakeClassDefinitionCompany);
      _mappingObjectFactoryMock
          .Setup(mock => mock.CreateClassDefinition(typeof(Partner), fakeClassDefinitionCompany, It.Is<IEnumerable<InterfaceDefinition>>(e => !e.Any())))
          .Returns(fakeClassDefinitionPartner);
      _mappingObjectFactoryMock
          .Setup(mock => mock.CreateClassDefinition(typeof(Customer), fakeClassDefinitionCompany, It.Is<IEnumerable<InterfaceDefinition>>(e => !e.Any())))
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
          .Setup(mock => mock.CreateClassDefinition(typeof(GenericBaseClass<>), null, It.Is<IEnumerable<InterfaceDefinition>>(f => !f.Any())))
          .Returns((ClassDefinition)null);
      _mappingObjectFactoryMock
          .Setup(mock => mock.CreateClassDefinition(typeof(SpecificSubClass1), null, It.Is<IEnumerable<InterfaceDefinition>>(f => !f.Any())))
          .Returns(specificSubClass1);
      _mappingObjectFactoryMock
          .Setup(mock => mock.CreateClassDefinition(typeof(SpecificSubClass2), null, It.Is<IEnumerable<InterfaceDefinition>>(f => !f.Any())))
          .Returns(specificSubClass2);

      var typeDefinitions = _typeDefinitionFactory.CreateTypeDefinitions(new[] { typeof(SpecificSubClass1), typeof(SpecificSubClass2) });
      Assert.That(typeDefinitions, Is.EqualTo(new [] {specificSubClass1, specificSubClass2 }));
    }

    [Test]
    public void CreateTypeDefinitions_WithNonClassOrInterface_Throws ()
    {
      Assert.That(
          () => _typeDefinitionFactory.CreateTypeDefinitions(new[] { typeof(TypeCode) }),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot create a builder node for type 'System.TypeCode' because it is not a class-type."));
    }

    [Test]
    public void CreateTypeDefinitionCollection_InterfacesArePulledIntoInheritanceRoots ()
    {
      var testInterfaceBase = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(ITestInterfaceBase));
      var testInterfaceWithDoubleImplementation = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(
          typeof(ITestInterfaceWithTwoImplementations),
          extendedInterfaces: new[] { testInterfaceBase });
      var testClassWithInheritanceRoot = ClassDefinitionObjectMother.CreateClassDefinition(
          classType: typeof(TestClassWithInheritanceRoot),
          implementedInterfaces: new[] { testInterfaceWithDoubleImplementation, testInterfaceBase });

      _mappingObjectFactoryMock
          .Setup(e => e.CreateInterfaceDefinition(typeof(ITestInterfaceBase), It.Is<IEnumerable<InterfaceDefinition>>(f => !f.Any())))
          .Returns(testInterfaceBase);
      _mappingObjectFactoryMock
          .Setup(e => e.CreateInterfaceDefinition(typeof(ITestInterfaceWithTwoImplementations), It.Is<IEnumerable<InterfaceDefinition>>(f => f.Single() == testInterfaceBase)))
          .Returns(testInterfaceWithDoubleImplementation);
      _mappingObjectFactoryMock
          .Setup(
              e => e.CreateClassDefinition(
                  typeof(TestClassWithInheritanceRoot),
                  null,
                  It.Is<IEnumerable<InterfaceDefinition>>(f => f.SequenceEqual(new[] { testInterfaceWithDoubleImplementation, testInterfaceBase }))))
          .Returns(testClassWithInheritanceRoot);

      var typeDefinitions =
          _typeDefinitionFactory.CreateTypeDefinitions(
              new[] { typeof(ITestInterfaceBase), typeof(ITestInterfaceWithTwoImplementations), typeof(TestBaseClassWithInterfaces), typeof(TestClassWithInheritanceRoot) });

      _mappingObjectFactoryMock.VerifyAll();

      Assert.That(typeDefinitions, Is.EquivalentTo(new TypeDefinition[] { testInterfaceBase, testInterfaceWithDoubleImplementation, testClassWithInheritanceRoot }));
      Assert.That(testInterfaceBase.ExtendingInterfaces, Is.EqualTo(new[] { testInterfaceWithDoubleImplementation }));
      Assert.That(testInterfaceBase.ImplementingClasses, Is.EqualTo(new[] { testClassWithInheritanceRoot }));
      Assert.That(testInterfaceWithDoubleImplementation.ExtendingInterfaces, Is.Empty);
      Assert.That(testInterfaceWithDoubleImplementation.ImplementingClasses, Is.EqualTo(new[] { testClassWithInheritanceRoot }));
      Assert.That(testClassWithInheritanceRoot.DerivedClasses, Is.Empty);
    }

    [Test]
    public void CreateTypeDefinitionCollection_InterfacesAreReferencesOnlyAtTheFirstOccurence ()
    {
      var testInterfaceBase = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(ITestInterfaceBase));
      var testInterfaceWithDoubleImplementation = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(
          typeof(ITestInterfaceWithTwoImplementations),
          extendedInterfaces: new[] { testInterfaceBase });
      var testBaseClass = ClassDefinitionObjectMother.CreateClassDefinition(
          classType: typeof(TestBaseClassWithInterfaces),
          implementedInterfaces: new[] { testInterfaceWithDoubleImplementation, testInterfaceBase });
      var testClassWithoutInheritanceRoot = ClassDefinitionObjectMother.CreateClassDefinition(
          classType: typeof(TestClassWithoutInheritanceRoot),
          baseClass: testBaseClass);

      _mappingObjectFactoryMock
          .Setup(e => e.CreateInterfaceDefinition(typeof(ITestInterfaceBase), It.Is<IEnumerable<InterfaceDefinition>>(f => !f.Any())))
          .Returns(testInterfaceBase);
      _mappingObjectFactoryMock
          .Setup(e => e.CreateInterfaceDefinition(typeof(ITestInterfaceWithTwoImplementations), It.Is<IEnumerable<InterfaceDefinition>>(f => f.Single() == testInterfaceBase)))
          .Returns(testInterfaceWithDoubleImplementation);
      _mappingObjectFactoryMock
          .Setup(
              e => e.CreateClassDefinition(
                  typeof(TestBaseClassWithInterfaces),
                  null,
                  It.Is<IEnumerable<InterfaceDefinition>>(f => f.SequenceEqual(new[] { testInterfaceWithDoubleImplementation, testInterfaceBase }))))
          .Returns(testBaseClass);
      _mappingObjectFactoryMock
          .Setup(
              e => e.CreateClassDefinition(
                  typeof(TestClassWithoutInheritanceRoot),
                  testBaseClass,
                  It.Is<IEnumerable<InterfaceDefinition>>(f => !f.Any())))
          .Returns(testClassWithoutInheritanceRoot);

      var typeDefinitions =
          _typeDefinitionFactory.CreateTypeDefinitions(
              new[] { typeof(ITestInterfaceBase), typeof(ITestInterfaceWithTwoImplementations), typeof(TestBaseClassWithInterfaces), typeof(TestClassWithoutInheritanceRoot) });

      _mappingObjectFactoryMock.VerifyAll();

      Assert.That(
          typeDefinitions,
          Is.EquivalentTo(new TypeDefinition[] { testInterfaceBase, testInterfaceWithDoubleImplementation, testBaseClass, testClassWithoutInheritanceRoot }));
      Assert.That(testInterfaceBase.ExtendingInterfaces, Is.EqualTo(new[] { testInterfaceWithDoubleImplementation }));
      Assert.That(testInterfaceBase.ImplementingClasses, Is.EqualTo(new[] { testBaseClass }));
      Assert.That(testInterfaceWithDoubleImplementation.ExtendingInterfaces, Is.Empty);
      Assert.That(testInterfaceWithDoubleImplementation.ImplementingClasses, Is.EqualTo(new[] { testBaseClass }));
      Assert.That(testBaseClass.DerivedClasses, Is.EqualTo(new[] { testClassWithoutInheritanceRoot }));
      Assert.That(testClassWithoutInheritanceRoot.DerivedClasses, Is.Empty);
    }
  }
}
