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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.IntegrationTests;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Data.DomainObjects.Validation;

namespace Remotion.Data.DomainObjects.UnitTests.Validation
{
  [TestFixture]
  public class NotNullablePropertyValidatorTest : StandardMappingTest
  {
     private NotNullablePropertyValidator _validator;

    public override void SetUp ()
    {
      base.SetUp();

      _validator = new NotNullablePropertyValidator();
    }

    [Test]
    public void ValidateDataContainer_PropertyIsNullable_AndPropertyValueIsNull_DoesNotThrow ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Person>(DomainObjectIDs.Person1);

      var dataContainer = CreatePersistableData(new DomainObjectState.Builder().SetNew().Value, domainObject).DataContainer;
      dataContainer.SetValue(GetPropertyDefinition(typeof(Person), "Name"), "Not Null");
      dataContainer.SetValue(GetPropertyDefinition(typeof(Person), "AssociatedCustomerCompany"), null);

      Assert.That(() => _validator.Validate(dataContainer), Throws.Nothing);
    }

    [Test]
    public void ValidateDataContainer_DoesNotRaisePropertyValueReadEvents ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Person>(DomainObjectIDs.Person1);

      var dataContainer = CreatePersistableData(new DomainObjectState.Builder().SetNew().Value, domainObject).DataContainer;
      dataContainer.SetValue(GetPropertyDefinition(typeof(Person), "Name"), "Not Null");
      dataContainer.SetValue(GetPropertyDefinition(typeof(Person), "AssociatedCustomerCompany"), null);
      var eventListenerStub = new Mock<IDataContainerEventListener>();
      dataContainer.SetEventListener(eventListenerStub.Object);

      _validator.Validate(dataContainer);

      eventListenerStub.Verify(
          _ => _.PropertyValueReading(It.IsAny<DataContainer>(), It.IsAny<PropertyDefinition>(), It.IsAny<ValueAccess>()),
          Times.Never());
    }

    [Test]
    public void ValidateDataContainer_PropertyIsNotNullable_AndPropertyValueIsNull_ThrowsException ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Person>(DomainObjectIDs.Person1);

      var dataContainer = CreatePersistableData(new DomainObjectState.Builder().SetNew().Value, domainObject).DataContainer;
      dataContainer.SetValue(GetPropertyDefinition(typeof(Person), "Name"), null);
      dataContainer.SetValue(GetPropertyDefinition(typeof(Person), "AssociatedCustomerCompany"), DomainObjectIDs.Customer1);

      Assert.That(
          () => _validator.Validate(dataContainer),
          Throws.TypeOf<PropertyValueNotSetException>().With.Message.Matches(
              @"Not-nullable property 'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.Person\.Name' of domain object "
              + @"'Person|.*|System\.Guid' cannot be null."));
    }

    [Test]
    public void ValidateDataContainer_PropertyHasMaxLength_AndPropertyValueIsTooLong_AndPropertyIsTransactionProperty_DoesNotThrow ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ClassWithAllDataTypes>(DomainObjectIDs.ClassWithAllDataTypes1);

      var dataContainer = CreatePersistableData(new DomainObjectState.Builder().SetNew().Value, domainObject).DataContainer;
      dataContainer.SetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "ExtensibleEnumProperty"), Color.Values.Red());
      dataContainer.SetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "StringProperty"), "String");
      dataContainer.SetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "StringPropertyWithoutMaxLength"), "StringWithoutLengthLimit");
      dataContainer.SetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "BinaryProperty"), new byte[] { 08, 15 });
      dataContainer.SetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "TransactionOnlyBinaryProperty"), new byte[] { 47, 11 });
      dataContainer.SetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "DateTimeProperty"), new DateTime(2012, 12, 12));
      dataContainer.SetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "DateProperty"), new DateTime(2012, 12, 12));
      dataContainer.SetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "TransactionOnlyStringProperty"), "Value is longer than the allowed maximum of one hundred characters. To achieve this, these words have been added.");

      Assert.That(() => _validator.Validate(dataContainer), Throws.Nothing);
    }

    [Test]
    public void ValidatePersistableData_PropertyHasMaxLength_AndPropertyValueIsNull_AndPropertyIsTransactionProperty_ThrowsException ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ClassWithAllDataTypes>(DomainObjectIDs.ClassWithAllDataTypes1);

      var dataItem = CreatePersistableData(new DomainObjectState.Builder().SetNew().Value, domainObject);
      dataItem.DataContainer.SetValue(GetPropertyDefinition(typeof(ClassWithAllDataTypes), "TransactionOnlyStringProperty"), null);

      Assert.That(
          () => _validator.Validate(ClientTransaction.CreateRootTransaction(), dataItem),
          Throws.TypeOf<PropertyValueNotSetException>().With.Message.Matches(
              @"Value for property 'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.ClassWithAllDataTypes\.TransactionOnlyStringProperty' "
              + @"of domain object ''ClassWithAllDataTypes|.*|System\.Guid'' cannot be null."));
    }

    [Test]
    public void ValidatePersistableData_IgnoresDeletedObjects ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Person>(DomainObjectIDs.Person1);

      var dataItem = CreatePersistableData(new DomainObjectState.Builder().SetDeleted().Value, domainObject);
      dataItem.DataContainer.SetValue(GetPropertyDefinition(typeof(Person), "Name"), null);

      Assert.That(() => _validator.Validate(ClientTransaction.CreateRootTransaction(), dataItem), Throws.Nothing);
    }

    [Test]
    public void ValidateDataContainer_IntegrationTest_PropertyOk ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var person = Person.NewObject();
        person.Name = "Not Null";

        var dataContainer = person.InternalDataContainer;
        Assert.That(() => _validator.Validate(dataContainer), Throws.Nothing);
      }
    }

    [Test]
    public void ValidateDataContainer_IntegrationTest_PropertyNotOk ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var person = Person.NewObject();
        person.Name = null;

        var dataContainer = person.InternalDataContainer;
        Assert.That(
            () => _validator.Validate(dataContainer),
            Throws.TypeOf<PropertyValueNotSetException>().With.Message.Matches(
              @"Not-nullable property 'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.Person\.Name' of domain object "
              + @"'Person|.*|System\.Guid' cannot be null."));
      }
    }

    [Test]
    public void ValidatePersistableData_IntegrationTest_PropertyOk ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var person = Person.NewObject();
        person.Name = "Not Null";

        var persistableData = PersistableDataObjectMother.Create(ClientTransaction.Current, person);
        Assert.That(() => _validator.Validate(ClientTransaction.Current, persistableData), Throws.Nothing);
      }
    }

    [Test]
    public void ValidatePersistableData_IntegrationTest_PropertyNotOk ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var person = Person.NewObject();
        person.Name = null;

        var persistableData = PersistableDataObjectMother.Create(ClientTransaction.Current, person);
        Assert.That(
            () => _validator.Validate(ClientTransaction.Current, persistableData),
            Throws.TypeOf<PropertyValueNotSetException>().With.Message.Matches(
              @"Not-nullable property 'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.Person\.Name' of domain object "
              + @"'Person|.*|System\.Guid' cannot be null."));
      }
    }

    private PersistableData CreatePersistableData (DomainObjectState domainObjectState, DomainObject domainObject)
    {
      var dataContainer = DataContainer.CreateNew(domainObject.ID);
      return new PersistableData(domainObject, domainObjectState, dataContainer, Enumerable.Empty<IRelationEndPoint>());
    }

  }
}
