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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.Mapping.MixinTestDomain;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration;
using Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.MixedMapping;
using Remotion.Data.DomainObjects.UnitTests.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.TableInheritance;
using Remotion.Development.UnitTesting.Enumerables;
using Remotion.Mixins;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Mapping
{
  [TestFixture]
  public class ClassDefinitionTest : MappingReflectionTestBase
  {
    private ClassDefinition _orderClass;
    private ClassDefinition _distributorClass;

    private ClassDefinition _targetClassForPersistentMixinClass;
    private ClassDefinition _derivedTargetClassForPersistentMixinClass;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private ClassDefinition _domainBaseClass;
    private ClassDefinition _personClass;
    private ClassDefinition _customerClass;
    private ClassDefinition _organizationalUnitClass;

    public override void SetUp ()
    {
      base.SetUp();

      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider");

      _domainBaseClass = ClassDefinitionObjectMother.CreateClassDefinition ("DomainBase");
      _personClass = ClassDefinitionObjectMother.CreateClassDefinition (id: "Person", baseClass: _domainBaseClass);
      _customerClass = ClassDefinitionObjectMother.CreateClassDefinition (id: "Customer", baseClass: _personClass);
      _organizationalUnitClass = ClassDefinitionObjectMother.CreateClassDefinition (id: "OrganizationalUnit", baseClass: _domainBaseClass);

      _domainBaseClass.SetDerivedClasses (new[] { _personClass, _organizationalUnitClass });

      _orderClass = FakeMappingConfiguration.Current.TypeDefinitions[typeof (Order)];
      _distributorClass = FakeMappingConfiguration.Current.TypeDefinitions[typeof (Distributor)];

      _targetClassForPersistentMixinClass =
          FakeMappingConfiguration.Current.TypeDefinitions[typeof (TargetClassForPersistentMixin)];
      _derivedTargetClassForPersistentMixinClass =
          FakeMappingConfiguration.Current.TypeDefinitions[typeof (DerivedTargetClassForPersistentMixin)];
    }

    [Test]
    public void Initialize ()
    {
      var persistentMixinFinder = MockRepository.GenerateStub<IPersistentMixinFinder> ();
      var instanceCreator = MockRepository.GenerateStub<IDomainObjectCreator>();

      var actual = new ClassDefinition ("Order", typeof (Order), false, null, null, persistentMixinFinder, instanceCreator);
      actual.SetDerivedClasses (new ClassDefinition[0]);

      Assert.That (actual.ID, Is.EqualTo ("Order"));
      Assert.That (actual.StorageEntityDefinition, Is.Null);
      Assert.That (actual.ClassType, Is.SameAs (typeof (Order)));
      Assert.That (actual.BaseClass, Is.Null);
      //Assert.That (actual.DerivedClasses.AreResolvedTypesRequired, Is.True);
      Assert.That (actual.IsReadOnly, Is.False);
      Assert.That (actual.PersistentMixinFinder, Is.SameAs (persistentMixinFinder));
      Assert.That (actual.InstanceCreator, Is.SameAs (instanceCreator));
    }

    [Test]
    public void Initialize_IDCreator_CreatesGenericObjectIDs ()
    {
      var persistentMixinFinder = MockRepository.GenerateStub<IPersistentMixinFinder> ();
      var instanceCreator = MockRepository.GenerateStub<IDomainObjectCreator> ();

      var classDefinition = new ClassDefinition ("Order", typeof (Order), false, null, null, persistentMixinFinder, instanceCreator);

      Assert.That (classDefinition.HandleCreator, Is.Not.Null);

      var tableDefinition = TableDefinitionObjectMother.Create (_storageProviderDefinition);
      classDefinition.SetStorageEntity (tableDefinition);

      var result = classDefinition.HandleCreator (DomainObjectIDs.Order1);
      Assert.That (result, Is.TypeOf<DomainObjectHandle<Order>> ());
      Assert.That (result.ObjectID, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void Initialize_IDCreator_ThrowsWhenNoDomainObject ()
    {
      var persistentMixinFinder = MockRepository.GenerateStub<IPersistentMixinFinder> ();
      var instanceCreator = MockRepository.GenerateStub<IDomainObjectCreator> ();

      var classDefinition = new ClassDefinition ("Order", typeof (string), false, null, null, persistentMixinFinder, instanceCreator);

      Assert.That (classDefinition.HandleCreator, Is.Not.Null);
      Assert.That (
          () => classDefinition.HandleCreator (DomainObjectIDs.Order1),
          Throws.InvalidOperationException.With.Message.EqualTo ("Handles cannot be created when the ClassType does not derive from DomainObject."));
    }

    [Test]
    public void InitializeWithNullStorageGroupType ()
    {
      ClassDefinition classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (TIDomainBase), storageGroupType: null);
      
      Assert.That (classDefinition.StorageGroupType, Is.Null);
    }

    [Test]
    public void InitializeWithStorageGroupType ()
    {
      ClassDefinition classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (TIDomainBase), storageGroupType: typeof (DBStorageGroupAttribute));

      Assert.That (classDefinition.StorageGroupType, Is.Not.Null);
      Assert.That (classDefinition.StorageGroupType, Is.SameAs (typeof (DBStorageGroupAttribute)));
    }

    [Test]
    public void SetStorageEntityDefinition ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (_storageProviderDefinition, new EntityNameDefinition (null, "Tablename"));
      
      _domainBaseClass.SetStorageEntity (tableDefinition);

      Assert.That (_domainBaseClass.StorageEntityDefinition, Is.SameAs (tableDefinition));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Class 'DomainBase' is read-only.")]
    public void SetStorageEntityDefinition_ClassIsReadOnly ()
    {
      var tableDefinition = TableDefinitionObjectMother.Create (_storageProviderDefinition, new EntityNameDefinition (null, "Tablename"));
      _domainBaseClass.SetReadOnly();

      _domainBaseClass.SetStorageEntity (tableDefinition);

      Assert.That (_domainBaseClass.StorageEntityDefinition, Is.SameAs (tableDefinition));
    }

    [Test]
    public void SetPropertyDefinitions ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_domainBaseClass);

      _domainBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, false));

      Assert.That (_domainBaseClass.MyPropertyDefinitions.Count, Is.EqualTo (1));
      Assert.That (_domainBaseClass.MyPropertyDefinitions[0], Is.SameAs (propertyDefinition));
      Assert.That (_domainBaseClass.MyPropertyDefinitions.IsReadOnly, Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The property-definitions for class 'DomainBase' have already been set."
        )]
    public void SetPropertyDefinitions_Twice_ThrowsException ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (_domainBaseClass);

      _domainBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, false));
      _domainBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, false));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Class 'DomainBase' is read-only.")]
    public void SetPropertyDefinitions_ClassIsReadOnly ()
    {
      _domainBaseClass.SetReadOnly();

      _domainBaseClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new PropertyDefinition[0], true));
    }

    [Test]
    public void SetRelationEndPointDefinitions ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _domainBaseClass, "Test", false, CardinalityType.One, null, MockRepository.GenerateStub<IPropertyInformation>());

      _domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, false));

      Assert.That (_domainBaseClass.MyRelationEndPointDefinitions.Count, Is.EqualTo (1));
      Assert.That (_domainBaseClass.MyRelationEndPointDefinitions[0], Is.SameAs (endPointDefinition));
      Assert.That (_domainBaseClass.MyRelationEndPointDefinitions.IsReadOnly, Is.True);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Relation end point for property 'Test' cannot be added to class 'DomainBase', because it was initialized for class 'Distributor'.")]
    public void SetRelationEndPointDefinitions_DifferentClassDefinition_ThrowsException ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _distributorClass, "Test", false, CardinalityType.One, null, MockRepository.GenerateStub<IPropertyInformation>());

      _domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, false));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Relation end point for property 'Test' cannot be added to class 'Person', because base class 'DomainBase' already defines a relation end point "
        + "with the same property name.")]
    public void SetRelationEndPointDefinitions_EndPointWithSamePropertyNameWasAlreadyAdded_ThrowsException ()
    {
      var baseEndPointDefinition = new VirtualRelationEndPointDefinition (
          _domainBaseClass, "Test", false, CardinalityType.One, null, MockRepository.GenerateStub<IPropertyInformation>());
      var derivedEndPointDefinition = new VirtualRelationEndPointDefinition (
          _personClass, "Test", false, CardinalityType.One, null, MockRepository.GenerateStub<IPropertyInformation>());

      _domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { baseEndPointDefinition }, true));
      _personClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { derivedEndPointDefinition }, true));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The relation end point definitions for class 'DomainBase' have already been set.")]
    public void SetRelationEndPointDefinitions_Twice_ThrowsException ()
    {
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          _domainBaseClass, "Test", false, CardinalityType.One, null, MockRepository.GenerateStub<IPropertyInformation>());

      _domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, false));
      _domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, false));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Class 'DomainBase' is read-only.")]
    public void SetRelationEndPointDefinitions_ClassIsReadonly ()
    {
      _domainBaseClass.SetReadOnly();

      _domainBaseClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new IRelationEndPointDefinition[0], true));
    }

    [Test]
    public void SetDerivedClasses ()
    {
      _personClass.SetDerivedClasses (new[] { _customerClass });

      Assert.That (_personClass.DerivedClasses.Count, Is.EqualTo (1));
      Assert.That (_personClass.DerivedClasses[0], Is.SameAs (_customerClass));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The derived-classes for class 'Person' have already been set.")]
    public void SetDerivedClasses_Twice_ThrowsException ()
    {
      _personClass.SetDerivedClasses (new[] { _customerClass });
      _personClass.SetDerivedClasses (new[] { _customerClass });
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Class 'Person' is read-only.")]
    public void SetDerivedClasses_ClasssIsReadOnly ()
    {
      _personClass.SetReadOnly();
      _personClass.SetDerivedClasses (new[] { _orderClass });
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Derived class 'Order' cannot be added to class 'Person', because it has no base class definition defined.")]
    public void SetDerivedClasses_DerivedClassHasNoBaseClassDefined ()
    {
      _personClass.SetDerivedClasses (new[] { _orderClass });
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Derived class 'Person' cannot be added to class 'Customer', because it has class 'DomainBase' as its base class definition defined.")]
    public void SetDerivedClasses_DerivedClassHasWrongBaseClassDefined ()
    {
      _customerClass.SetDerivedClasses (new[] { _personClass });
    }

    [Test]
    public void SetReadOnly ()
    {
      ClassDefinition actual = ClassDefinitionObjectMother.CreateClassDefinition ();
      actual.SetDerivedClasses (new ClassDefinition[0]);
      Assert.That (actual.IsReadOnly, Is.False);

      actual.SetReadOnly();

      Assert.That (actual.IsReadOnly, Is.True);
      Assert.That (((ICollection<ClassDefinition>)actual.DerivedClasses).IsReadOnly, Is.True);
    }

    [Test]
    public void GetToString ()
    {
      ClassDefinition actual = ClassDefinitionObjectMother.CreateClassDefinition ("OrderID", typeof (Order));

      Assert.That (actual.ToString(), Is.EqualTo (typeof (ClassDefinition).FullName + ": OrderID"));
    }

    [Test]
    public void GetIsAbstract_True ()
    {
      ClassDefinition actual = ClassDefinitionObjectMother.CreateClassDefinition (isAbstract: true);

      Assert.That (actual.IsAbstract, Is.True);
    }

    [Test]
    public void GetIsAbstract_False ()
    {
      ClassDefinition actual = ClassDefinitionObjectMother.CreateClassDefinition (isAbstract: false);

      Assert.That (actual.IsAbstract, Is.False);
    }

    [Test]
    public void GetRelationEndPointDefinition ()
    {
      Assert.That (
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Company.Ceo"),
          Is.Not.Null);
      Assert.That (
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Partner.ContactPerson"),
          Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'propertyName' cannot be empty.\r\nParameter name: propertyName")]
    public void GetRelationEndPointDefinitionFromEmptyPropertyName ()
    {
      _orderClass.GetRelationEndPointDefinition (string.Empty);
    }

    [Test]
    public void IsRelationEndPointTrue ()
    {
      RelationDefinition orderToOrderItem =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.OrderItem:Remotion.Data.DomainObjects.UnitTests.Mapping."
              + "TestDomain.Integration.OrderItem.Order->Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.OrderItems"];
      IRelationEndPointDefinition endPointDefinition =
          orderToOrderItem.GetEndPointDefinition (
              "Order", "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.OrderItems");

      Assert.That (_orderClass.IsRelationEndPoint (endPointDefinition), Is.True);
    }

    [Test]
    public void IsRelationEndPointFalse ()
    {
      RelationDefinition partnerToPerson =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Partner:Remotion.Data.DomainObjects.UnitTests.Mapping."
              +
              "TestDomain.Integration.Partner.ContactPerson->Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Person.AssociatedPartnerCompany"
              ];
      IRelationEndPointDefinition partnerEndPoint =
          partnerToPerson.GetEndPointDefinition (
              "Partner", "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Partner.ContactPerson");

      Assert.That (_orderClass.IsRelationEndPoint (partnerEndPoint), Is.False);
    }

    [Test]
    [ExpectedException (typeof (ArgumentNullException))]
    public void IsRelationEndPointWithNull ()
    {
      _orderClass.IsRelationEndPoint (null);
    }

    [Test]
    public void IsRelationEndPointWithInheritance ()
    {
      RelationDefinition partnerToPerson =
          FakeMappingConfiguration.Current.RelationDefinitions[
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Partner:"
              +
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Partner.ContactPerson->Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Person.AssociatedPartnerCompany"
              ];
      IRelationEndPointDefinition partnerEndPoint =
          partnerToPerson.GetEndPointDefinition (
              "Partner", "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Partner.ContactPerson");

      Assert.That (_distributorClass.IsRelationEndPoint (partnerEndPoint), Is.True);
    }

    [Test]
    public void GetPropertyDefinition ()
    {
      Assert.That (
          _orderClass.GetPropertyDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.OrderNumber"),
          Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No property definitions have been set for class 'Order'.")]
    public void GetPropertyDefinition_NoPropertyDefinitionsHaveBeenSet_ThrowsException ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition ("Order");
      classDefinition.GetPropertyDefinition ("dummy");
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Parameter 'propertyName' cannot be empty.\r\nParameter name: propertyName")]
    public void GetEmptyPropertyDefinition ()
    {
      _orderClass.GetPropertyDefinition (string.Empty);
    }

    [Test]
    public void GetInheritedPropertyDefinition ()
    {
      Assert.That (
          _distributorClass.GetPropertyDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Partner.ContactPerson"),
          Is.Not.Null);
    }

    [Test]
    public void GetAllPropertyDefinitions_SucceedsWhenReadOnly ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition ();
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new PropertyDefinition[0], true));
      classDefinition.SetReadOnly();

      var result = classDefinition.GetPropertyDefinitions();

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No property definitions have been set for class 'Order'.")]
    public void GetAllPropertyDefinitions_ThrowsWhenPropertiesNotSet ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition ("Order");
      classDefinition.GetPropertyDefinitions();
    }

    [Test]
    public void GetAllPropertyDefinitions_Cached ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition ();
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (classDefinition);
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      classDefinition.SetReadOnly();

      var result1 = classDefinition.GetPropertyDefinitions();
      var result2 = classDefinition.GetPropertyDefinitions();

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void GetAllPropertyDefinitions_ReadOnly ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition ();
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new PropertyDefinition[0], true));
      classDefinition.SetReadOnly();

      var result = classDefinition.GetPropertyDefinitions();

      Assert.That (result.IsReadOnly, Is.True);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' cannot be added to class 'Order', because it was initialized for class 'Company'.")]
    public void AddPropertyToOtherClass ()
    {
      var companyClass = ClassDefinitionObjectMother.CreateClassDefinition ("Company");
      var orderClass = ClassDefinitionObjectMother.CreateClassDefinition ("Order");

      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (companyClass, "Name");
      orderClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' cannot be added to class 'Customer', because base class 'Company' already defines a property with the same name.")]
    public void AddDuplicatePropertyBaseClass ()
    {
      var companyClass = ClassDefinitionObjectMother.CreateClassDefinition ("Company");
      var companyPropertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (companyClass, "Name");
      companyClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { companyPropertyDefinition }, true));

      var customerClass = ClassDefinitionObjectMother.CreateClassDefinition (id: "Customer", baseClass: companyClass);
      var customerPropertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (customerClass, "Name");
      customerClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { customerPropertyDefinition }, true));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "Property 'Name' cannot be added to class 'Supplier', because base class 'Company' already defines a property with the same name.")]
    public void AddDuplicatePropertyBaseOfBaseClass ()
    {
      var companyClass = ClassDefinitionObjectMother.CreateClassDefinition ("Company");
      var companyPropertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (companyClass, "Name");
      companyClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { companyPropertyDefinition }, true));

      var partnerClass = ClassDefinitionObjectMother.CreateClassDefinition (id: "Partner", baseClass: companyClass);
      partnerClass.SetPropertyDefinitions (new PropertyDefinitionCollection());

      var supplierClass = ClassDefinitionObjectMother.CreateClassDefinition (id: "Supplier", baseClass: partnerClass);
      var supplierPropertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (supplierClass, "Name");
      supplierClass.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { supplierPropertyDefinition }, true));
    }

    [Test]
    public void ConstructorWithoutBaseClass ()
    {
      var persistentMixinFinder = MockRepository.GenerateStub<IPersistentMixinFinder> ();
      var instanceCreator = MockRepository.GenerateStub<IDomainObjectCreator> ();

      Assert.That (
          () => new ClassDefinition ("id", typeof (Company), false, null, null, persistentMixinFinder, instanceCreator), 
          Throws.Nothing);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "No relation found for class 'Order' and property 'UndefinedProperty'.")]
    public void GetMandatoryRelationEndPointDefinitionForUndefinedProperty ()
    {
      _orderClass.GetMandatoryRelationEndPointDefinition ("UndefinedProperty");
    }

    [Test]
    public void GetAllRelationEndPointDefinitions_SucceedsWhenReadOnly ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition ();
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          classDefinition, "Test", false, CardinalityType.One, null, MockRepository.GenerateStub<IPropertyInformation>());
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, true));
      classDefinition.SetReadOnly();

      var result = classDefinition.GetRelationEndPointDefinitions();

      Assert.That (result, Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No relation end point definitions have been set for class 'Order'.")]
    public void GetAllRelationEndPointDefinitions_ThrowsWhenRelationsNotSet ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition ("Order");
      classDefinition.GetRelationEndPointDefinitions();
    }

    [Test]
    public void GetAllRelationEndPointDefinitions_Cached ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition ();
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          classDefinition, "Test", false, CardinalityType.One, null, MockRepository.GenerateStub<IPropertyInformation>());
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, true));
      classDefinition.SetReadOnly();

      var result1 = classDefinition.GetRelationEndPointDefinitions();
      var result2 = classDefinition.GetRelationEndPointDefinitions();

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void GetAllRelationEndPointDefinitionss_ReadOnly ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition ();
      classDefinition.SetDerivedClasses (new ClassDefinition[0]);
      var endPointDefinition = new VirtualRelationEndPointDefinition (
          classDefinition, "Test", false, CardinalityType.One, null, MockRepository.GenerateStub<IPropertyInformation>());
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPointDefinition }, true));
      classDefinition.SetReadOnly();

      var result = classDefinition.GetRelationEndPointDefinitions();

      Assert.That (result.IsReadOnly, Is.True);
    }

    [Test]
    public void GetAllRelationEndPointDefinitions_Content ()
    {
      var relationEndPointDefinitions = _orderClass.GetRelationEndPointDefinitions();

      IRelationEndPointDefinition customerEndPoint =
          _orderClass.GetRelationEndPointDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.Customer");
      IRelationEndPointDefinition orderTicketEndPoint =
          _orderClass.GetRelationEndPointDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.OrderTicket");
      IRelationEndPointDefinition orderItemsEndPoint =
          _orderClass.GetRelationEndPointDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.OrderItems");
      IRelationEndPointDefinition officialEndPoint =
          _orderClass.GetRelationEndPointDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.Official");

      Assert.That (
          relationEndPointDefinitions, Is.EquivalentTo (new[] { customerEndPoint, orderTicketEndPoint, orderItemsEndPoint, officialEndPoint }));
    }

    [Test]
    public void GetAllRelationEndPointDefinitionsWithInheritance ()
    {
      var relationEndPointDefinitions = _distributorClass.GetRelationEndPointDefinitions();

      IRelationEndPointDefinition contactPersonEndPoint =
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Partner.ContactPerson");
      IRelationEndPointDefinition ceoEndPoint =
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Company.Ceo");
      IRelationEndPointDefinition industrialSectorEndPoint =
          _distributorClass.GetRelationEndPointDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Company.IndustrialSector");

      Assert.That (relationEndPointDefinitions, Is.Not.Null);
      Assert.That (
          relationEndPointDefinitions,
          Is.EquivalentTo (
              new[]
              {
                  contactPersonEndPoint,
                  ceoEndPoint,
                  industrialSectorEndPoint
              }));
    }

    [Test]
    public void GetDerivedClassesWithoutInheritance ()
    {
      Assert.That (_orderClass.DerivedClasses, Is.Not.Null);
      Assert.That (_orderClass.DerivedClasses.Count, Is.EqualTo (0));
      Assert.That (((ICollection<ClassDefinition>) _orderClass.DerivedClasses).IsReadOnly, Is.True);
    }

    [Test]
    public void GetDerivedClassesWithInheritance ()
    {
      ClassDefinition companyDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Company));

      Assert.That (companyDefinition.DerivedClasses, Is.Not.Null);
      Assert.That (companyDefinition.DerivedClasses.Count, Is.EqualTo (2));
      Assert.That (companyDefinition.DerivedClasses.Any (cd => cd.ID == "Customer"), Is.True);
      Assert.That (companyDefinition.DerivedClasses.Any (cd => cd.ID == "Partner"), Is.True);
      Assert.That (((ICollection<ClassDefinition>) companyDefinition.DerivedClasses).IsReadOnly, Is.True);
    }

    [Test]
    public void IsPartOfInheritanceHierarchy ()
    {
      ClassDefinition companyDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Company));

      Assert.That (companyDefinition.IsPartOfInheritanceHierarchy, Is.True);
      Assert.That (_distributorClass.IsPartOfInheritanceHierarchy, Is.True);
      Assert.That (_orderClass.IsPartOfInheritanceHierarchy, Is.False);
    }

    [Test]
    public void IsRelationEndPointWithAnonymousRelationEndPointDefinition ()
    {
      var classDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Client));
      var relationDefinition = classDefinition.GetRelationEndPointDefinition (typeof (Client).FullName + ".ParentClient").RelationDefinition;
      var anonymousEndPointDefinition = (AnonymousRelationEndPointDefinition) relationDefinition.GetEndPointDefinition ("Client", null);

      Assert.That (classDefinition.IsRelationEndPoint (anonymousEndPointDefinition), Is.False);
    }

    [Test]
    public void MyPropertyDefinitions ()
    {
      var clientDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Client));

      var propertyDefinitions = clientDefinition.MyPropertyDefinitions.ToArray();

      Assert.That (propertyDefinitions.Length, Is.EqualTo (1));
      Assert.That (
          propertyDefinitions[0].PropertyName,
          Is.EqualTo ("Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Client.ParentClient"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No property definitions have been set for class 'Order'.")]
    public void MyPropertyDefinitions_NoPropertiesSet_ThrowsException ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition ("Order");

      classDefinition.MyPropertyDefinitions.ForceEnumeration();
    }

    [Test]
    public void MyRelationEndPointDefinitions ()
    {
      var clientDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Client));

      IRelationEndPointDefinition[] endPointDefinitions = clientDefinition.MyRelationEndPointDefinitions.ToArray();

      Assert.That (endPointDefinitions.Length, Is.EqualTo (1));
      Assert.That (
          Contains (
              endPointDefinitions, "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Client.ParentClient"),
          Is.True);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "No relation end point definitions have been set for class 'Order'.")]
    public void MyRelationEndPointDefinitions_NoRelationEndPointDefinitionsSet_ThrowsException ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition ("Order");

      classDefinition.MyRelationEndPointDefinitions.ForceEnumeration();
    }

    [Test]
    public void MyRelationEndPointDefinitionsCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (FileSystemItem));

      IRelationEndPointDefinition[] endPointDefinitions = fileSystemItemDefinition.MyRelationEndPointDefinitions.ToArray();

      Assert.That (endPointDefinitions.Length, Is.EqualTo (1));
      Assert.That (
          Contains (
              endPointDefinitions,
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"),
          Is.True);
    }

    [Test]
    public void IsMyRelationEndPoint ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Folder));

      IRelationEndPointDefinition folderEndPoint =
          folderDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Folder.FileSystemItems");
      IRelationEndPointDefinition fileSystemItemEndPoint =
          folderDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder");

      Assert.That (folderDefinition.IsMyRelationEndPoint (folderEndPoint), Is.True);
      Assert.That (folderDefinition.IsMyRelationEndPoint (fileSystemItemEndPoint), Is.False);
    }

    [Test]
    public void MyRelationEndPointDefinitionsCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Folder));

      IRelationEndPointDefinition[] endPointDefinitions = folderDefinition.MyRelationEndPointDefinitions.ToArray();

      Assert.That (endPointDefinitions.Length, Is.EqualTo (1));
      Assert.That (
          Contains (
              endPointDefinitions, "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Folder.FileSystemItems"),
          Is.True);
    }

    [Test]
    public void GetRelationEndPointDefinitionsCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (FileSystemItem));

      var endPointDefinitions = fileSystemItemDefinition.GetRelationEndPointDefinitions();

      Assert.That (endPointDefinitions.Count(), Is.EqualTo (1));
      Assert.That (
          Contains (
              endPointDefinitions,
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"),
          Is.True);
    }

    [Test]
    public void GetRelationEndPointDefinitionsCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Folder));

      var endPointDefinitions = folderDefinition.GetRelationEndPointDefinitions();

      Assert.That (endPointDefinitions.Count(), Is.EqualTo (2));
      Assert.That (
          Contains (
              endPointDefinitions, "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Folder.FileSystemItems"),
          Is.True);
      Assert.That (
          Contains (
              endPointDefinitions,
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"),
          Is.True);
    }

    [Test]
    public void GetRelationEndPointDefinitionCompositeBaseClass ()
    {
      ClassDefinition fileSystemItemDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (FileSystemItem));

      Assert.That (
          fileSystemItemDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"),
          Is.Not.Null);
      Assert.That (
          fileSystemItemDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Folder.FileSystemItems"),
          Is.Null);
    }

    [Test]
    public void GetRelationEndPointDefinitionCompositeDerivedClass ()
    {
      ClassDefinition folderDefinition = MappingConfiguration.Current.GetTypeDefinition (typeof (Folder));

      Assert.That (
          folderDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder"),
          Is.Not.Null);
      Assert.That (
          folderDefinition.GetRelationEndPointDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Folder.FileSystemItems"),
          Is.Not.Null);
    }

    [Test]
    public void GetMandatoryPropertyDefinition ()
    {
      Assert.That (
          _orderClass.GetMandatoryPropertyDefinition (
              "Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.OrderNumber"),
          Is.Not.Null);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "Class 'Order' does not contain the property 'InvalidProperty'.")]
    public void GetMandatoryPropertyDefinitionWithInvalidPropertName ()
    {
      _orderClass.GetMandatoryPropertyDefinition ("InvalidProperty");
    }

    [Test]
    public void SetClassDefinitionOfPropertyDefinition ()
    {
      // Note: Never use a ClassDefinition of TestMappingConfiguration or MappingConfiguration here, to ensure
      // this test does not affect other tests through modifying the singleton instances.
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition ();

      PropertyDefinition propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo (classDefinition);
      Assert.That (propertyDefinition.ClassDefinition, Is.SameAs (classDefinition));

      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      Assert.That (propertyDefinition.ClassDefinition, Is.SameAs (classDefinition));
    }

    [Test]
    public void Contains ()
    {
      Assert.That (_orderClass.Contains (PropertyDefinitionObjectMother.CreateForFakePropertyInfo()), Is.False);
      Assert.That (
          _orderClass.Contains (
              _orderClass["Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order.OrderNumber"]),
          Is.True);
    }

    [Test]
    public void GetInheritanceRootClass ()
    {
      ClassDefinition expected = FakeMappingConfiguration.Current.TypeDefinitions[typeof (Company)];
      Assert.That (_distributorClass.GetInheritanceRootClass(), Is.SameAs (expected));
    }

    [Test]
    public void GetAllDerivedClasses ()
    {
      ClassDefinition companyClass = FakeMappingConfiguration.Current.TypeDefinitions[typeof (Company)];
      var allDerivedClasses = companyClass.GetAllDerivedClasses();
      Assert.That (allDerivedClasses, Is.Not.Null);
      Assert.That (allDerivedClasses.Length, Is.EqualTo (4));

      Assert.That (allDerivedClasses.Any (cd => cd.ClassType == typeof (Customer)), Is.True);
      Assert.That (allDerivedClasses.Any (cd => cd.ClassType == typeof (Partner)), Is.True);
      Assert.That (allDerivedClasses.Any (cd => cd.ClassType == typeof (Supplier)), Is.True);
      Assert.That (allDerivedClasses.Any (cd => cd.ClassType == typeof (Distributor)), Is.True);
    }

    [Test]
    public void IsSameOrBaseClassOfFalse ()
    {
      Assert.That (_orderClass.IsSameOrBaseClassOf (_distributorClass), Is.False);
    }

    [Test]
    public void IsSameOrBaseClassOfTrueWithSameClass ()
    {
      Assert.That (_orderClass.IsSameOrBaseClassOf (_orderClass), Is.True);
    }

    [Test]
    public void IsSameOrBaseClassOfTrueWithBaseClass ()
    {
      ClassDefinition companyClass = FakeMappingConfiguration.Current.TypeDefinitions[typeof (Company)];

      Assert.That (companyClass.IsSameOrBaseClassOf (_distributorClass), Is.True);
    }

    private bool Contains (IEnumerable<IRelationEndPointDefinition> endPointDefinitions, string propertyName)
    {
      return endPointDefinitions.Any (endPointDefinition => endPointDefinition.PropertyName == propertyName);
    }

    [Test]
    public void PropertyInfoWithSimpleProperty ()
    {
      PropertyInfo property = typeof (Order).GetProperty ("OrderNumber");
      var propertyDefinition = _orderClass.GetPropertyDefinition (property.DeclaringType.FullName + "." + property.Name);
      Assert.That (propertyDefinition.PropertyInfo, Is.EqualTo (PropertyInfoAdapter.Create (property)));
    }

    [Test]
    public void CreatorIsTypePipeBasedCreator ()
    {
      Assert.That (_orderClass.InstanceCreator, Is.TypeOf<DomainObjectCreator>());
    }

    [Test]
    public void PersistentMixinFinder ()
    {
      var mixinFinder = new PersistentMixinFinderStub (typeof (Order));
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (persistentMixinFinder: mixinFinder);
      Assert.That (classDefinition.PersistentMixinFinder, Is.SameAs (mixinFinder));
    }

    [Test]
    public void PersistentMixins_Empty ()
    {
      var mixins = new Type[0];
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (typeof (Order), mixins);
      Assert.That (classDefinition.PersistentMixins, Is.EqualTo (mixins));
    }

    [Test]
    public void PersistentMixins_NonEmpty ()
    {
      var mixins = new[] { typeof (MixinA), typeof (MixinB) };
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (typeof (Order), mixins);
      Assert.That (classDefinition.PersistentMixins, Is.EqualTo (mixins));
    }
    
    [Test]
    public void ResolveProperty ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");

      var result = _orderClass.ResolveProperty (PropertyInfoAdapter.Create(property));

      var expected = _orderClass.GetPropertyDefinition (typeof (Order).FullName + ".OrderNumber");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveProperty_Twice_ReturnsSamePropertyDefinition ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");

      var result1 = _orderClass.ResolveProperty (PropertyInfoAdapter.Create(property));
      var result2 = _orderClass.ResolveProperty (PropertyInfoAdapter.Create(property));

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void ResolveProperty_StorageClassNoneProperty ()
    {
      var property = typeof (Order).GetProperty ("RedirectedOrderNumber");

      var result = _orderClass.ResolveProperty (PropertyInfoAdapter.Create(property));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void ResolveProperty_MixinProperty ()
    {
      var property = typeof (IMixinAddingPersistentProperties).GetProperty ("PersistentProperty");

      var result = _targetClassForPersistentMixinClass.ResolveProperty (PropertyInfoAdapter.Create(property));

      var expected = _targetClassForPersistentMixinClass.GetPropertyDefinition (
          typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveProperty_MixinPropertyOnBaseClass ()
    {
      var property = typeof (IMixinAddingPersistentProperties).GetProperty ("PersistentProperty");

      var result = _derivedTargetClassForPersistentMixinClass.ResolveProperty (PropertyInfoAdapter.Create(property));

      var expected = _targetClassForPersistentMixinClass.GetPropertyDefinition (
          typeof (MixinAddingPersistentProperties).FullName + ".PersistentProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveRelationEndPoint_OneToOne ()
    {
      var property = typeof (Order).GetProperty ("OrderTicket");

      var result = _orderClass.ResolveRelationEndPoint (PropertyInfoAdapter.Create(property));

      var expected = _orderClass.GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderTicket");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveRelationEndPoint_OneToMany ()
    {
      var property = typeof (Order).GetProperty ("OrderItems");

      var result = _orderClass.ResolveRelationEndPoint (PropertyInfoAdapter.Create(property));

      var expected = _orderClass.GetRelationEndPointDefinition (typeof (Order).FullName + ".OrderItems");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveRelationEndPoint_NonEndPointProperty ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");

      var result = _orderClass.ResolveRelationEndPoint (PropertyInfoAdapter.Create(property));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void ResolveRelationEndPoint_Twice_ReturnsSameRelationDefinition ()
    {
      var property = typeof (Order).GetProperty ("OrderItems");

      var result1 = _orderClass.ResolveRelationEndPoint (PropertyInfoAdapter.Create(property));
      var result2 = _orderClass.ResolveRelationEndPoint (PropertyInfoAdapter.Create(property));

      Assert.That (result1, Is.SameAs (result2));
    }

    [Test]
    public void ResolveRelationEndPoint_MixinRelationProperty_VirtualEndPoint ()
    {
      var property = typeof (IMixinAddingPersistentProperties).GetProperty ("VirtualRelationProperty");

      var result = _targetClassForPersistentMixinClass.ResolveRelationEndPoint (PropertyInfoAdapter.Create(property));

      var expected = _targetClassForPersistentMixinClass.GetRelationEndPointDefinition (
          typeof (MixinAddingPersistentProperties).FullName + ".VirtualRelationProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ResolveRelationEndPoint_MixinRelationProperty_DefinedOnBaseClass ()
    {
      var property = typeof (IMixinAddingPersistentProperties).GetProperty ("RelationProperty");

      var result = _derivedTargetClassForPersistentMixinClass.ResolveRelationEndPoint (PropertyInfoAdapter.Create(property));

      var expected = _targetClassForPersistentMixinClass.GetRelationEndPointDefinition (
          typeof (MixinAddingPersistentProperties).FullName + ".RelationProperty");
      Assert.That (result, Is.SameAs (expected));
    }

    [Test]
    public void ValidateCurrentMixinConfiguration_OkWhenNoChanges ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (typeof (Order), typeof (MixinA));
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (typeof (MixinA)).EnterScope())
      {
        classDefinition.ValidateCurrentMixinConfiguration(); // ok, no changes
      }
    }

    [Test]
    public void ValidateCurrentMixinConfiguration_OkOnInheritanceRootInheritingMixin ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (
          typeof (InheritanceRootInheritingPersistentMixin),
          typeof (MixinAddingPersistentPropertiesAboveInheritanceRoot));

      using (MixinConfiguration
          .BuildNew()
          .ForClass (typeof (TargetClassAboveInheritanceRoot))
          .AddMixins (typeof (MixinAddingPersistentPropertiesAboveInheritanceRoot))
          .EnterScope())
      {
        // ok, no changes, even though the mixins stem from a base class
        classDefinition.ValidateCurrentMixinConfiguration();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The mixin configuration for domain object type 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order' "
        + "was changed after the mapping information was built.\r\n"
        + "Original configuration: ClassContext: 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order'\r\n"
        + "  Mixins: \r\n"
        + "    MixinContext: 'Remotion.Data.DomainObjects.UnitTests.Mapping.MixinTestDomain.MixinA' (Extending,Private,Dependencies=())\r\n"
        + "  ComposedInterfaces: ().\r\n"
        + "Active configuration: ClassContext: 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order'\r\n"
        + "  Mixins: \r\n"
        + "    MixinContext: 'Remotion.Data.DomainObjects.UnitTests.Mapping.MixinTestDomain.NonDomainObjectMixin' (Extending,Private,Dependencies=())\r\n"
        + "    MixinContext: 'Remotion.Data.DomainObjects.UnitTests.Mapping.MixinTestDomain.MixinA' (Extending,Private,Dependencies=())\r\n"
        + "  ComposedInterfaces: ()")]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenAnyChanges_EvenToNonPersistentMixins ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (typeof (Order), typeof (MixinA));

      using (MixinConfiguration.BuildFromActive()
          .ForClass (typeof (Order))
              .Clear()
              .AddMixins (typeof (NonDomainObjectMixin), typeof (MixinA))
          .EnterScope())
      {
        classDefinition.ValidateCurrentMixinConfiguration();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The mixin configuration for domain object type 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order' "
        + "was changed after the mapping information was built.",
        MatchType = MessageMatch.Contains)]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenPersistentMixinMissing ()
    {
      var classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (typeof (Order), typeof (MixinA));
      using (MixinConfiguration.BuildFromActive().ForClass<Order>().Clear().EnterScope())
      {
        classDefinition.ValidateCurrentMixinConfiguration();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The mixin configuration for domain object type 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Order' "
        + "was changed after the mapping information was built.",
        MatchType = MessageMatch.Contains)]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenPersistentMixinsAdded ()
    {
      ClassDefinition classDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (typeof (Order), typeof (MixinA));
      using (
          MixinConfiguration.BuildFromActive().ForClass (typeof (Order)).Clear().AddMixins (
              typeof (NonDomainObjectMixin), typeof (MixinA), typeof (MixinB), typeof (MixinC)).EnterScope())
      {
        classDefinition.ValidateCurrentMixinConfiguration();
      }
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The mixin configuration for domain object type 'Remotion.Data.DomainObjects.UnitTests.Mapping.TestDomain.Integration.Customer' "
        + "was changed after the mapping information was built.",
        MatchType = MessageMatch.Contains)]
    public void ValidateCurrentMixinConfiguration_ThrowsWhenPersistentMixinsChangeOnParentClass ()
    {
      ClassDefinition baseClassDefinition = ClassDefinitionObjectMother.CreateClassDefinitionWithMixins (typeof (Company), typeof (MixinA));
      ClassDefinition classDefinition = ClassDefinitionObjectMother.CreateClassDefinition (classType: typeof (Customer), baseClass: baseClassDefinition);
      using (
          MixinConfiguration.BuildFromActive().ForClass (typeof (Company)).Clear().AddMixins (
              typeof (NonDomainObjectMixin), typeof (MixinA), typeof (MixinB), typeof (MixinC)).EnterScope())
      {
        classDefinition.ValidateCurrentMixinConfiguration();
      }
    }
  }
}