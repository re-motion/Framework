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
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class ClassDefinitionCollectionFactoryTest
  {
    private Dictionary<Type, ClassDefinition> _classDefinitions;
    private ClassDefinitionCollectionFactory _classDefinitionCollectionFactory;
    private Mock<IMappingObjectFactory> _mappingObjectFactoryMock;
    private ClassDefinition _fakeClassDefinition;

    [SetUp]
    public void SetUp ()
    {
      _classDefinitions = new Dictionary<Type, ClassDefinition>();
      _mappingObjectFactoryMock = new Mock<IMappingObjectFactory>(MockBehavior.Strict);
      _classDefinitionCollectionFactory = new ClassDefinitionCollectionFactory(_mappingObjectFactoryMock.Object);
      _fakeClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(Order));
    }

    [Test]
    public void CreateClassDefinitionCollection ()
    {
      _mappingObjectFactoryMock
          .Setup(mock => mock.CreateClassDefinition(typeof(Order), null))
          .Returns(_fakeClassDefinition)
          .Verifiable();

      var classDefinitions = _classDefinitionCollectionFactory.CreateClassDefinitionCollection(new[] { typeof(Order) });

      _mappingObjectFactoryMock.Verify();

      Assert.That(classDefinitions, Is.EqualTo(new[] { _fakeClassDefinition }));
    }

    [Test]
    public void CreateClassDefinitionCollection_NoTypes ()
    {
      var classDefinitions = _classDefinitionCollectionFactory.CreateClassDefinitionCollection(new Type[0]);

      Assert.That(classDefinitions, Is.Empty);
    }

    [Test]
    public void CreateClassDefinitionCollection_DerivedClassAreSet ()
    {
      var fakeClassDefinitionCompany = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(Company));
      var fakeClassDefinitionPartner = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Partner), baseClass: fakeClassDefinitionCompany);
      var fakeClassDefinitionCustomer = ClassDefinitionObjectMother.CreateClassDefinition(classType: typeof(Customer), baseClass: fakeClassDefinitionCompany);

      _mappingObjectFactoryMock
          .Setup(mock => mock.CreateClassDefinition(typeof(Order), null))
          .Returns(_fakeClassDefinition)
          .Verifiable();
      _mappingObjectFactoryMock
          .Setup(mock => mock.CreateClassDefinition(typeof(Company), null))
          .Returns(fakeClassDefinitionCompany)
          .Verifiable();
      _mappingObjectFactoryMock
          .Setup(mock => mock.CreateClassDefinition(typeof(Partner), fakeClassDefinitionCompany))
          .Returns(fakeClassDefinitionPartner)
          .Verifiable();
      _mappingObjectFactoryMock
          .Setup(mock => mock.CreateClassDefinition(typeof(Customer), fakeClassDefinitionCompany))
          .Returns(fakeClassDefinitionCustomer)
          .Verifiable();

      var classDefinitions =
          _classDefinitionCollectionFactory.CreateClassDefinitionCollection(
              new[] { typeof(Order), typeof(Company), typeof(Partner), typeof(Customer) });

      _mappingObjectFactoryMock.Verify();

      Assert.That(classDefinitions.Length, Is.EqualTo(4));

      var orderClassDefinition = classDefinitions.Single(cd => cd.Type == typeof(Order));
      Assert.That(orderClassDefinition.DerivedClasses.Count, Is.EqualTo(0));

      var companyClassDefinition = classDefinitions.Single(cd => cd.Type == typeof(Company));
      Assert.That(companyClassDefinition.DerivedClasses, Is.EquivalentTo(new[] { fakeClassDefinitionPartner, fakeClassDefinitionCustomer }));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClass_WithStorageGroupAttribute_IgnoresBaseClass ()
    {
      _mappingObjectFactoryMock
          .Setup(mock => mock.CreateClassDefinition(typeof(ClassWithDifferentProperties), null))
          .Returns(_fakeClassDefinition)
          .Verifiable();

      var result = _classDefinitionCollectionFactory.GetClassDefinition(_classDefinitions, typeof(ClassWithDifferentProperties));

      _mappingObjectFactoryMock.Verify();
      Assert.That(result, Is.SameAs(_fakeClassDefinition));
    }

    [Test]
    public void GetClassDefinition_ForNonDerivedClass_WithoutStorageGroupAttribute ()
    {
      _mappingObjectFactoryMock
          .Setup(mock => mock.CreateClassDefinition(typeof(ClassWithoutStorageGroupWithDifferentProperties), null))
          .Returns(_fakeClassDefinition)
          .Verifiable();

      var result = _classDefinitionCollectionFactory.GetClassDefinition(_classDefinitions, typeof(ClassWithoutStorageGroupWithDifferentProperties));

      _mappingObjectFactoryMock.Verify();
      Assert.That(result, Is.SameAs(_fakeClassDefinition));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClass ()
    {
      var fakeBaseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(ClassWithDifferentProperties));
      _mappingObjectFactoryMock
          .Setup(mock => mock.CreateClassDefinition(typeof(ClassWithDifferentProperties), null))
          .Returns(fakeBaseClassDefinition)
          .Verifiable();
      _mappingObjectFactoryMock
          .Setup(mock => mock.CreateClassDefinition(typeof(DerivedClassWithDifferentProperties), fakeBaseClassDefinition))
          .Returns(_fakeClassDefinition)
          .Verifiable();

      var result = _classDefinitionCollectionFactory.GetClassDefinition(_classDefinitions, typeof(DerivedClassWithDifferentProperties));

      _mappingObjectFactoryMock.Verify();
      Assert.That(result, Is.SameAs(_fakeClassDefinition));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClass_WithBaseClassAlreadyInClassDefinitionCollection ()
    {
      var expectedBaseClass = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(ClassWithDifferentProperties));
      _classDefinitions.Add(expectedBaseClass.Type, expectedBaseClass);

      _mappingObjectFactoryMock
          .Setup(mock => mock.CreateClassDefinition(typeof(DerivedClassWithDifferentProperties), expectedBaseClass))
          .Returns(_fakeClassDefinition)
          .Verifiable();

      var actual = _classDefinitionCollectionFactory.GetClassDefinition(_classDefinitions, typeof(DerivedClassWithDifferentProperties));

      _mappingObjectFactoryMock.Verify();
      Assert.That(actual, Is.Not.Null);
      Assert.That(_classDefinitions.Count, Is.EqualTo(2));
      Assert.That(_fakeClassDefinition, Is.SameAs(actual));
    }

    [Test]
    public void GetClassDefinition_ForDerivedClass_WithDerivedClassAlreadyInClassDefinitionCollection ()
    {
      var existing = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins(typeof(Order));
      _classDefinitions.Add(existing.Type, existing);

      var actual = _classDefinitionCollectionFactory.GetClassDefinition(_classDefinitions, typeof(Order));

      _mappingObjectFactoryMock.Verify();
      Assert.That(actual, Is.Not.Null);
      Assert.That(_classDefinitions.Count, Is.EqualTo(1));
      Assert.That(_classDefinitions[typeof(Order)], Is.SameAs(actual));
      Assert.That(actual, Is.SameAs(existing));
    }
  }
}
