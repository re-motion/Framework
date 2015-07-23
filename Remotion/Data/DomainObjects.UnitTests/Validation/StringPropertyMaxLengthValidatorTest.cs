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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.UnitTests.IntegrationTests;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Validation;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Validation
{
  [TestFixture]
  public class StringPropertyMaxLengthValidatorTest : StandardMappingTest
  {
    private StringPropertyMaxLengthValidator _validator;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _validator = new StringPropertyMaxLengthValidator();
    }

    [Test]
    public void ValidateDataContainer_PropertyWithoutMaxLength_DoesNotThrow ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ClassWithAllDataTypes> (DomainObjectIDs.ClassWithAllDataTypes1);

      var dataContainer = CreatePersistableData (StateType.New, domainObject).DataContainer;
      dataContainer.SetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "StringPropertyWithoutMaxLength"), "value");

      Assert.That (() => _validator.Validate (dataContainer), Throws.Nothing);
    }

    [Test]
    public void ValidateDataContainer_PropertyHasMaxLength_AndPropertyValueIsNotTooLong_DoesNotThrow ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Person> (DomainObjectIDs.Person1);

      var dataContainer = CreatePersistableData (StateType.New, domainObject).DataContainer;
      dataContainer.SetValue (GetPropertyDefinition (typeof (Person), "Name"), new string ('x', 100));

      Assert.That (() => _validator.Validate (dataContainer), Throws.Nothing);
    }

    [Test]
    public void ValidateDataContainer_PropertyHasMaxLength_AndPropertyValueIsNull_DoesNotThrow ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Person> (DomainObjectIDs.Person1);

      var dataContainer = CreatePersistableData (StateType.New, domainObject).DataContainer;
      dataContainer.SetValue (GetPropertyDefinition (typeof (Person), "Name"), null);

      Assert.That (() => _validator.Validate (dataContainer), Throws.Nothing);
    }

    [Test]
    public void ValidateDataContainer_DoesNotRaisePropertyValueReadEvents ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Person> (DomainObjectIDs.Person1);

      var dataContainer = CreatePersistableData (StateType.New, domainObject).DataContainer;
      dataContainer.SetValue (GetPropertyDefinition (typeof (Person), "Name"), new string ('x', 100));
      var eventListenerStub = MockRepository.GenerateStub<IDataContainerEventListener>();
      dataContainer.SetEventListener (eventListenerStub);

      _validator.Validate (dataContainer);

      eventListenerStub.AssertWasNotCalled (_ => _.PropertyValueReading (null, null, ValueAccess.Current), mo => mo.IgnoreArguments());
    }

    [Test]
    public void ValidateDataContainer_PropertyHasMaxLength_AndPropertyValueIsTooLong_ThrowsException ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Person> (DomainObjectIDs.Person1);

      var dataContainer = CreatePersistableData (StateType.New, domainObject).DataContainer;
      dataContainer.SetValue (GetPropertyDefinition (typeof (Person), "Name"), new string ('x', 101));

      Assert.That (
          () => _validator.Validate (dataContainer),
          Throws.TypeOf<PropertyValueTooLongException>().With.Message.Matches (
              @"Value for property 'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.Person\.Name' "
              + @"of domain object ''Person|.*|System\.Guid'' is too long. Maximum number of characters: 100."));
    }

    [Test]
    public void ValidateDataContainer_PropertyHasMaxLength_AndPropertyValueIsTooLong_AndPropertyIsTransactionProperty_DoesNotThrow ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ClassWithAllDataTypes> (DomainObjectIDs.ClassWithAllDataTypes1);

      var dataContainer = CreatePersistableData (StateType.New, domainObject).DataContainer;
      dataContainer.SetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "TransactionOnlyStringProperty"), new string ('x', 101));

      Assert.That (() => _validator.Validate (dataContainer), Throws.Nothing);
    }

    [Test]
    public void ValidatePersistableData_PropertyHasMaxLength_AndPropertyValueIsTooLong_AndPropertyIsTransactionProperty_ThrowsException ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ClassWithAllDataTypes> (DomainObjectIDs.ClassWithAllDataTypes1);

      var dataItem = CreatePersistableData (StateType.New, domainObject);
      dataItem.DataContainer.SetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "TransactionOnlyStringProperty"), new string ('x', 101));

      Assert.That (
          () => _validator.Validate (ClientTransaction.CreateRootTransaction(), dataItem),
          Throws.TypeOf<PropertyValueTooLongException>().With.Message.Matches (
              @"Value for property 'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.ClassWithAllDataTypes\.TransactionOnlyStringProperty' "
              + @"of domain object ''ClassWithAllDataTypes|.*|System\.Guid'' is too long. Maximum number of characters: 100."));
    }

    [Test]
    public void ValidatePersistableData_IgnoresDeletedObjects ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Person> (DomainObjectIDs.Person1);

      var dataItem = CreatePersistableData (StateType.Deleted, domainObject);
      dataItem.DataContainer.SetValue (GetPropertyDefinition (typeof (Person), "Name"), new string ('x', 101));

      Assert.That (() => _validator.Validate (ClientTransaction.CreateRootTransaction(), dataItem), Throws.Nothing);
    }

    [Test]
    public void ValidateDataContainer_IntegrationTest_PropertyWithValueOk ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var person = Person.NewObject ();
        person.Name = "Not Null";

        var dataContainer = person.InternalDataContainer;
        Assert.That (() => _validator.Validate (dataContainer), Throws.Nothing);
      }
    }

    [Test]
    public void ValidateDataContainer_IntegrationTest_PropertyNotOk ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var person = Person.NewObject();
        person.Name = new string ('c', 101);

        var dataContainer = person.InternalDataContainer;
        Assert.That (
            () => _validator.Validate (dataContainer),
            Throws.TypeOf<PropertyValueTooLongException>().With.Message.Matches (
                @"Value for property 'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.Person\.Name' "
                + @"of domain object ''Person|.*|System\.Guid'' is too long. Maximum number of characters: 100."));
      }
    }

    [Test]
    public void ValidatePersistableData_IntegrationTest_PropertyWithValueOk ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var person = Person.NewObject ();
        person.Name = "Not Null";

        var persistableData = PersistableDataObjectMother.Create (ClientTransaction.Current, person);
        Assert.That (() => _validator.Validate (ClientTransaction.Current, persistableData), Throws.Nothing);
      }
    }

    [Test]
    public void ValidatePersistableData_IntegrationTest_PropertyNotOk ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var person = Person.NewObject();
        person.Name = new string ('c', 101);

        var persistableData = PersistableDataObjectMother.Create (ClientTransaction.Current, person);
        Assert.That (
            () => _validator.Validate (ClientTransaction.Current, persistableData),
            Throws.TypeOf<PropertyValueTooLongException>().With.Message.Matches (
                @"Value for property 'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.Person\.Name' "
                + @"of domain object ''Person|.*|System\.Guid'' is too long. Maximum number of characters: 100."));
      }
    }

    private PersistableData CreatePersistableData (StateType domainObjectState, DomainObject domainObject)
    {
      var dataContainer = DataContainer.CreateNew (domainObject.ID);
      return new PersistableData (domainObject, domainObjectState, dataContainer, Enumerable.Empty<IRelationEndPoint>());
    }
  }
}