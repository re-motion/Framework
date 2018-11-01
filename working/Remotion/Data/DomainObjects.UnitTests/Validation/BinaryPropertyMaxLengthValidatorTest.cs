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
  public class BinaryPropertyMaxLengthValidatorTest : StandardMappingTest
  {
    private BinaryPropertyMaxLengthValidator _validator;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _validator = new BinaryPropertyMaxLengthValidator();
    }

    [Test]
    public void ValidateDataContainer_PropertyWithoutMaxLength_DoesNotThrow ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ClassWithAllDataTypes> (DomainObjectIDs.ClassWithAllDataTypes1);

      var dataContainer = CreatePersistableData (StateType.New, domainObject).DataContainer;
      dataContainer.SetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "BinaryProperty"), new byte[10]);

      Assert.That (() => _validator.Validate (dataContainer), Throws.Nothing);
    }

    [Test]
    public void ValidateDataContainer_PropertyHasMaxLength_AndPropertyValueIsNotTooLong_DoesNotThrow ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ClassWithAllDataTypes> (DomainObjectIDs.ClassWithAllDataTypes1);

      var dataContainer = CreatePersistableData (StateType.New, domainObject).DataContainer;
      dataContainer.SetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "NullableBinaryProperty"), new byte[1000000]);

      Assert.That (() => _validator.Validate (dataContainer), Throws.Nothing);
    }

    [Test]
    public void ValidateDataContainer_PropertyHasMaxLength_AndPropertyValueIsNull_DoesNotThrow ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ClassWithAllDataTypes> (DomainObjectIDs.ClassWithAllDataTypes1);

      var dataContainer = CreatePersistableData (StateType.New, domainObject).DataContainer;
      dataContainer.SetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "NullableBinaryProperty"), null);

      Assert.That (() => _validator.Validate (dataContainer), Throws.Nothing);
    }

    [Test]
    public void ValidateDataContainer_DoesNotRaisePropertyValueReadEvents ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ClassWithAllDataTypes> (DomainObjectIDs.ClassWithAllDataTypes1);

      var dataContainer = CreatePersistableData (StateType.New, domainObject).DataContainer;
      dataContainer.SetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "NullableBinaryProperty"), new byte [10]);
      var eventListenerStub = MockRepository.GenerateStub<IDataContainerEventListener>();
      dataContainer.SetEventListener (eventListenerStub);

      _validator.Validate (dataContainer);

      eventListenerStub.AssertWasNotCalled (_ => _.PropertyValueReading (null, null, ValueAccess.Current), mo => mo.IgnoreArguments());
    }

    [Test]
    public void ValidateDataContainer_PropertyHasMaxLength_AndPropertyValueIsTooLong_ThrowsException ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ClassWithAllDataTypes> (DomainObjectIDs.ClassWithAllDataTypes1);

      var dataContainer = CreatePersistableData (StateType.New, domainObject).DataContainer;
      dataContainer.SetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "NullableBinaryProperty"), new byte [1000001]);

      Assert.That (
          () => _validator.Validate (dataContainer),
          Throws.TypeOf<PropertyValueTooLongException>().With.Message.Matches (
              @"Value for property 'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.ClassWithAllDataTypes\.NullableBinaryProperty' "
              + @"of domain object ''ClassWithAllDataTypes|.*|System\.Guid'' is too long. Maximum size: 1000001."));
    }
    [Test]
    public void ValidateDataContainer_PropertyHasMaxLength_AndPropertyValueIsTooLong_AndPropertyIsTransactionProperty_DoesNotThrow ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ClassWithAllDataTypes> (DomainObjectIDs.ClassWithAllDataTypes1);

      var dataContainer = CreatePersistableData (StateType.New, domainObject).DataContainer;
      dataContainer.SetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "TransactionOnlyBinaryProperty"), new byte[1000001]);

      Assert.That (() => _validator.Validate (dataContainer), Throws.Nothing);
    }

    [Test]
    public void ValidatePersistableData_PropertyHasMaxLength_AndPropertyValueIsTooLong_AndPropertyIsTransactionProperty_ThrowsException ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ClassWithAllDataTypes> (DomainObjectIDs.ClassWithAllDataTypes1);

      var dataItem = CreatePersistableData (StateType.New, domainObject);
      dataItem.DataContainer.SetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "TransactionOnlyBinaryProperty"), new byte[1000001]);

      Assert.That (
          () => _validator.Validate (ClientTransaction.CreateRootTransaction(), dataItem),
          Throws.TypeOf<PropertyValueTooLongException>().With.Message.Matches (
              @"Value for property 'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.ClassWithAllDataTypes\.TransactionOnlyBinaryProperty' "
              + @"of domain object ''ClassWithAllDataTypes|.*|System\.Guid'' is too long. Maximum number of characters: 100."));
    }

    [Test]
    public void ValidatePersistableData_IgnoresDeletedObjects ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<ClassWithAllDataTypes> (DomainObjectIDs.ClassWithAllDataTypes1);

      var dataItem = CreatePersistableData (StateType.Deleted, domainObject);
      dataItem.DataContainer.SetValue (GetPropertyDefinition (typeof (ClassWithAllDataTypes), "NullableBinaryProperty"), new byte [1000001]);

      Assert.That (() => _validator.Validate (ClientTransaction.CreateRootTransaction(), dataItem), Throws.Nothing);
    }

    [Test]
    public void ValidateDataContainer_IntegrationTest_PropertyWithValueOk ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var person = ClassWithAllDataTypes.NewObject ();
        person.BinaryProperty = new byte[10];
        person.NullableBinaryProperty = new byte[1000000];
        person.StringProperty = "value";
        person.StringPropertyWithoutMaxLength = "value";

        var dataContainer = person.InternalDataContainer;
        Assert.That (() => _validator.Validate (dataContainer), Throws.Nothing);
      }
    }

    [Test]
    public void ValidateDataContainer_IntegrationTest_PropertyNotOk ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var person = ClassWithAllDataTypes.NewObject();
        person.BinaryProperty = new byte[10];
        person.NullableBinaryProperty = new byte[1000001];
        person.StringProperty = "value";
        person.StringPropertyWithoutMaxLength = "value";

        var dataContainer = person.InternalDataContainer;
        Assert.That (
            () => _validator.Validate (dataContainer),
            Throws.TypeOf<PropertyValueTooLongException>().With.Message.Matches (
                @"Value for property 'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.ClassWithAllDataTypes\.NullableBinaryProperty' "
                + @"of domain object ''ClassWithAllDataTypes|.*|System\.Guid'' is too long. Maximum size: 1000001."));
      }
    }

    [Test]
    public void ValidatePersistableData_IntegrationTest_PropertyWithValueOk ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var person = ClassWithAllDataTypes.NewObject ();
        person.BinaryProperty = new byte[10];
        person.NullableBinaryProperty = new byte[1000000];
        person.StringProperty = "value";
        person.StringPropertyWithoutMaxLength = "value";

        var persistableData = PersistableDataObjectMother.Create (ClientTransaction.Current, person);
        Assert.That (() => _validator.Validate (ClientTransaction.Current, persistableData), Throws.Nothing);
      }
    }

    [Test]
    public void ValidatePersistableData_IntegrationTest_PropertyNotOk ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var person = ClassWithAllDataTypes.NewObject();
        person.BinaryProperty = new byte[10];
        person.NullableBinaryProperty = new byte[1000001];
        person.StringProperty = "value";
        person.StringPropertyWithoutMaxLength = "value";

        var persistableData = PersistableDataObjectMother.Create (ClientTransaction.Current, person);
        Assert.That (
            () => _validator.Validate (ClientTransaction.Current, persistableData),
            Throws.TypeOf<PropertyValueTooLongException>().With.Message.Matches (
                @"Value for property 'Remotion\.Data\.DomainObjects\.UnitTests\.TestDomain\.ClassWithAllDataTypes\.NullableBinaryProperty' "
                + @"of domain object ''ClassWithAllDataTypes|.*|System\.Guid'' is too long. Maximum size: 1000001."));
      }
    }

    private PersistableData CreatePersistableData (StateType domainObjectState, DomainObject domainObject)
    {
      var dataContainer = DataContainer.CreateNew (domainObject.ID);
      return new PersistableData (domainObject, domainObjectState, dataContainer, Enumerable.Empty<IRelationEndPoint>());
    }
  }
}