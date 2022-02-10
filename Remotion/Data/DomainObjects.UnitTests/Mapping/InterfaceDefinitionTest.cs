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
using Moq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class InterfaceDefinitionTest
  {
    private interface IDomainBase
    {
    }

    private interface IPerson : IDomainBase
    {
    }

    private interface ICustomer : IPerson
    {
    }

    private interface IOrganizationalUnit : IDomainBase
    {
    }

    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;

    private InterfaceDefinition _domainBaseInterface;
    private InterfaceDefinition _personInterface;
    private InterfaceDefinition _customerInterface;
    private InterfaceDefinition _organizationalUnitInterface;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition("DefaultStorageProvider");

      _domainBaseInterface = InterfaceDefinitionObjectMother.CreateInterfaceDefinitionWithDefaultProperties(typeof(IDomainBase));
      _personInterface = InterfaceDefinitionObjectMother.CreateInterfaceDefinitionWithDefaultProperties(typeof(IPerson), extendedInterfaces: new[] { _domainBaseInterface });
      _customerInterface = InterfaceDefinitionObjectMother.CreateInterfaceDefinitionWithDefaultProperties(typeof(ICustomer), extendedInterfaces: new[] { _personInterface });
      _organizationalUnitInterface = InterfaceDefinitionObjectMother.CreateInterfaceDefinitionWithDefaultProperties(
          typeof(IOrganizationalUnit),
          extendedInterfaces: new[] { _domainBaseInterface });

      _domainBaseInterface.SetExtendingInterfaces(new[] { _personInterface, _organizationalUnitInterface });
      _personInterface.SetExtendingInterfaces(new[] { _customerInterface });
    }

    [Test]
    public void Initialize ()
    {
      var actual = new InterfaceDefinition(
          typeof(IDomainBase),
          Array.Empty<InterfaceDefinition>(),
          null,
          DefaultStorageClass.Transaction);

      Assert.That(actual.Type, Is.SameAs(typeof(IDomainBase)));
      Assert.That(actual.ExtendedInterfaces, Is.Empty);
      Assert.That(actual.DefaultStorageClass, Is.EqualTo(DefaultStorageClass.Transaction));

      Assert.That(actual.HasStorageEntityDefinitionBeenSet, Is.False);
      Assert.That(
          () => actual.StorageEntityDefinition,
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "StorageEntityDefinition has not been set for type 'Remotion.Data.DomainObjects.UnitTests.Mapping.InterfaceDefinitionTest+IDomainBase'."));

      Assert.That(
          () => actual.ImplementingClasses,
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "No implementing classes have been set for interface 'Remotion.Data.DomainObjects.UnitTests.Mapping.InterfaceDefinitionTest+IDomainBase'."));
      Assert.That(
          () => actual.ExtendingInterfaces,
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "No extending interfaces have been set for interface 'Remotion.Data.DomainObjects.UnitTests.Mapping.InterfaceDefinitionTest+IDomainBase'."));

      Assert.That(actual.IsReadOnly, Is.False);
    }

    [Test]
    public void IsPartOfInheritanceHierarchy ()
    {
      var detachedInterfaceDefinition = TypeDefinitionObjectMother.CreateInterfaceDefinitionWithDefaultProperties();
      var extendedInterfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinitionWithDefaultProperties();
      var extendingInterfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinitionWithDefaultProperties(extendedInterfaces: new[] { extendedInterfaceDefinition });
      var implementedInterfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinitionWithDefaultProperties();
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(implementedInterfaces: new[] { implementedInterfaceDefinition });

      extendedInterfaceDefinition.SetExtendingInterfaces(new[] { extendingInterfaceDefinition });
      implementedInterfaceDefinition.SetImplementingClasses(new[] { classDefinition });

      Assert.That(detachedInterfaceDefinition.IsPartOfInheritanceHierarchy, Is.False);
      Assert.That(extendingInterfaceDefinition.IsPartOfInheritanceHierarchy, Is.True);
      Assert.That(extendingInterfaceDefinition.IsPartOfInheritanceHierarchy, Is.True);
      Assert.That(extendingInterfaceDefinition.IsPartOfInheritanceHierarchy, Is.True);
    }

    [Test]
    public void SetImplementingClasses ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition(implementedInterfaces: new[] { _domainBaseInterface });
      _domainBaseInterface.SetImplementingClasses(new[] { classDefinition });

      Assert.That(_domainBaseInterface.ImplementingClasses.Count, Is.EqualTo(1));
      Assert.That(_domainBaseInterface.ImplementingClasses[0], Is.SameAs(classDefinition));
    }

    [Test]
    public void SetImplementingClasses_ReadOnly_Throws ()
    {
      var interfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinitionWithDefaultProperties(typeof(IDomainBase));
      interfaceDefinition.SetReadOnly();

      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      Assert.That(
          () => interfaceDefinition.SetImplementingClasses(new[] { classDefinition }),
          Throws.TypeOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Interface 'Remotion.Data.DomainObjects.UnitTests.Mapping.InterfaceDefinitionTest+IDomainBase' is read-only."));
    }

    [Test]
    public void SetImplementingClasses_WithInvalidClassDefinition_Throws ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      Assert.That(
          () => _domainBaseInterface.SetImplementingClasses(new[] { classDefinition }),
          Throws.TypeOf<MappingException>()
              .With.Message.EqualTo(
                  "Interface 'Remotion.Data.DomainObjects.UnitTests.Mapping.InterfaceDefinitionTest+IDomainBase' "
                  + "cannot be implemented by class 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order', "
                  + "because 'IDomainBase' is not contained in the list of implemented interfaces on 'Order'."));
    }

    [Test]
    public void SetExtendingInterfaces ()
    {
      _domainBaseInterface.SetExtendingInterfaces(new[] { _personInterface });

      Assert.That(_domainBaseInterface.ExtendingInterfaces.Count, Is.EqualTo(1));
      Assert.That(_domainBaseInterface.ExtendingInterfaces[0], Is.SameAs(_personInterface));
    }

    [Test]
    public void SetExtendingInterfaces_ReadOnly_Throws ()
    {
      var interfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinitionWithDefaultProperties(typeof(IDomainBase));
      interfaceDefinition.SetReadOnly();

      Assert.That(
          () => interfaceDefinition.SetExtendingInterfaces(new[] { _personInterface }),
          Throws.TypeOf<NotSupportedException>()
              .With.Message.EqualTo(
                  "Interface 'Remotion.Data.DomainObjects.UnitTests.Mapping.InterfaceDefinitionTest+IDomainBase' is read-only."));
    }

    [Test]
    public void SetExtendingInterfaces_WithInvalidClassDefinition_Throws ()
    {
      var interfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      Assert.That(
          () => _domainBaseInterface.SetExtendingInterfaces(new[] { interfaceDefinition }),
          Throws.TypeOf<MappingException>()
              .With.Message.EqualTo(
                  "Interface 'Remotion.Data.DomainObjects.UnitTests.Mapping.InterfaceDefinitionTest+IDomainBase' "
                  + "cannot be extended by interface 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.IOrder', "
                  + "because 'IDomainBase' is not contained in the list of extended interfaces on 'IOrder'."));
    }

    [Test]
    public void Accept ()
    {
      var mock = new Mock<ITypeDefinitionVisitor>(MockBehavior.Strict);
      mock.Setup(_ => _.VisitInterfaceDefinition(_domainBaseInterface)).Verifiable();
      _domainBaseInterface.Accept(mock.Object);

      mock.Verify();
    }

    [Test]
    public void AcceptWithResult ()
    {
      var mock = new Mock<ITypeDefinitionVisitor<string>>();
      mock.Setup(_ => _.VisitInterfaceDefinition(_domainBaseInterface)).Returns("asd");

      Assert.That(_domainBaseInterface.Accept(mock.Object), Is.EqualTo("asd"));
    }

    [Test]
    public void SetPropertyDefinitions_WithIncorrectParent_Throws ()
    {
      var customer = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(ICustomer));
      var organizationalUnit = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(IOrganizationalUnit));

      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(customer, "Name");
      Assert.That(
          () => organizationalUnit.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { propertyDefinition }, true)),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Property 'Name' cannot be added to interface 'Remotion.Data.DomainObjects.UnitTests.Mapping.InterfaceDefinitionTest+IOrganizationalUnit', because it was initialized for "
                  + "type 'Remotion.Data.DomainObjects.UnitTests.Mapping.InterfaceDefinitionTest+ICustomer'."));
    }

    [Test]
    public void SetPropertyDefinitions_WithExistingProperty_Throws ()
    {
      var domainBase = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(IDomainBase));
      domainBase.SetPropertyDefinitions(new PropertyDefinitionCollection());

      var person = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(IPerson));
      var personProperty = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(person, "Name");
      person.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { personProperty }, true));

      var customer = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(ICustomer), new[] { domainBase, person });
      var customerProperty = PropertyDefinitionObjectMother.CreateForFakePropertyInfo(customer, "Name");

      Assert.That(
          () => customer.SetPropertyDefinitions(new PropertyDefinitionCollection(new[] { customerProperty }, true)),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Property 'Name' cannot be added to interface 'Remotion.Data.DomainObjects.UnitTests.Mapping.InterfaceDefinitionTest+ICustomer', because extended interface "
                  + "'Remotion.Data.DomainObjects.UnitTests.Mapping.InterfaceDefinitionTest+IPerson' already defines a property with the same name."));
    }

    [Test]
    public void SetRelationEndPointDefinitions_WithIncorrectParent_Throws ()
    {
      var customer = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(ICustomer));
      var organizationalUnit = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(IOrganizationalUnit));

      var relationEndPointDefinition = new VirtualObjectRelationEndPointDefinition(customer, "Name", true, new NullPropertyInformation());
      Assert.That(
          () => organizationalUnit.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { relationEndPointDefinition }, true)),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Relation end point for property 'Name' cannot be added to interface 'Remotion.Data.DomainObjects.UnitTests.Mapping.InterfaceDefinitionTest+IOrganizationalUnit', because it was initialized for "
                  + "interface 'Remotion.Data.DomainObjects.UnitTests.Mapping.InterfaceDefinitionTest+ICustomer'."));
    }

    [Test]
    public void SetRelationEndPointDefinitions_WithExistingProperty_Throws ()
    {
      var domainBase = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(IDomainBase));
      domainBase.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection());

      var person = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(IPerson));
      var personRelationEndPointDefinition = new VirtualObjectRelationEndPointDefinition(person, "Name", true, new NullPropertyInformation());
      person.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { personRelationEndPointDefinition }, true));

      var customer = InterfaceDefinitionObjectMother.CreateInterfaceDefinition(typeof(ICustomer), new[] { domainBase, person });
      var customerRelationEndPointDefinition = new VirtualObjectRelationEndPointDefinition(customer, "Name", true, new NullPropertyInformation());

      Assert.That(
          () => customer.SetRelationEndPointDefinitions(new RelationEndPointDefinitionCollection(new[] { customerRelationEndPointDefinition }, true)),
          Throws.InstanceOf<MappingException>()
              .With.Message.EqualTo(
                  "Relation end point for property 'Name' cannot be added to interface 'Remotion.Data.DomainObjects.UnitTests.Mapping.InterfaceDefinitionTest+ICustomer', because extended interface "
                  + "'Remotion.Data.DomainObjects.UnitTests.Mapping.InterfaceDefinitionTest+IPerson' already defines a relation end point with the same property name."));
    }

    [Test]
    public void SetReadOnly ()
    {
      var interfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinitionWithDefaultProperties();
      Assert.That(interfaceDefinition.IsReadOnly, Is.False);

      interfaceDefinition.SetReadOnly();

      Assert.That(interfaceDefinition.IsReadOnly, Is.True);
    }

    [Test]
    public void SetReadOnly_WithUnsetImplementingClasses_Throws ()
    {
      var interfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      Assert.That(
          () => interfaceDefinition.SetReadOnly(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot set the type definition read-only as the implementing classes are not set."));
    }

    [Test]
    public void SetReadOnly_WithUnsetExtendingInterfaces_Throws ()
    {
      var interfaceDefinition = InterfaceDefinitionObjectMother.CreateInterfaceDefinition();
      interfaceDefinition.SetImplementingClasses(Enumerable.Empty<ClassDefinition>());

      Assert.That(
          () => interfaceDefinition.SetReadOnly(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Cannot set the type definition read-only as the extending interfaces are not set."));
    }

    [Test]
    public void IsAssignableFrom ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithDefaultProperties(implementedInterfaces: new[] { _customerInterface });
      _customerInterface.SetImplementingClasses(new[] { classDefinition });

      Assert.That(_domainBaseInterface.IsAssignableFrom(_domainBaseInterface), Is.True);
      Assert.That(_domainBaseInterface.IsAssignableFrom(classDefinition), Is.True);
      Assert.That(_organizationalUnitInterface.IsAssignableFrom(classDefinition), Is.False);
      Assert.That(_personInterface.IsAssignableFrom(_customerInterface), Is.True);
      Assert.That(_personInterface.IsAssignableFrom(_organizationalUnitInterface), Is.False);
    }
  }
}
