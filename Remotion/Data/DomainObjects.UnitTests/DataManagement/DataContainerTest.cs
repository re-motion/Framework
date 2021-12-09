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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.NUnit.UnitTesting;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class DataContainerTest : ClientTransactionBaseTest
  {
    private DataContainer _newDataContainer;
    private DataContainer _existingDataContainer;
    private DataContainer _deletedDataContainer;
    private DataContainer _discardedDataContainer;
    private DataContainer _newNonPersistentDataContainer;
    private DataContainer _existingNonPersistentDataContainer;
    private DataContainer _dataContainerWithNonPersistentProperty;
    private ObjectID _invalidObjectID;
    private Mock<IDataContainerEventListener> _eventListenerMock;

    private PropertyDefinition _orderNumberProperty;
    private PropertyDefinition _deliveryDateProperty;
    private PropertyDefinition _nonOrderProperty;
    private PropertyDefinition _nonPersistentPropertyOnPersistentDataContainer;
    private PropertyDefinition _propertyOnNonPersistentDataContainer;

    public override void SetUp ()
    {
      base.SetUp();

      var idValue = Guid.NewGuid();

      _newDataContainer = DataContainer.CreateNew(new ObjectID(typeof(Order), idValue));
      _existingDataContainer = DataContainer.CreateForExisting(
          new ObjectID(typeof(Order), idValue),
          null,
          propertyDefinition => propertyDefinition.DefaultValue);

      _deletedDataContainer = DataContainer.CreateForExisting(
          new ObjectID(typeof(Order), idValue),
          null,
          propertyDefinition => propertyDefinition.DefaultValue);
      _deletedDataContainer.Delete();

      _invalidObjectID = new ObjectID(typeof(Order), idValue);
      _discardedDataContainer = DataContainer.CreateNew(_invalidObjectID);
      _discardedDataContainer.Discard();

      _newNonPersistentDataContainer = DataContainer.CreateNew(new ObjectID(typeof(OrderViewModel), idValue));

      _existingNonPersistentDataContainer = DataContainer.CreateForExisting(
          new ObjectID(typeof(OrderViewModel), idValue),
          null,
          propertyDefinition => propertyDefinition.DefaultValue);

      _dataContainerWithNonPersistentProperty = DataContainer.CreateForExisting(
          new ObjectID(typeof(OrderTicket), idValue),
          null,
          propertyDefinition => propertyDefinition.DefaultValue);

      _eventListenerMock = new Mock<IDataContainerEventListener>();

      _orderNumberProperty = GetPropertyDefinition(typeof(Order), "OrderNumber");
      _deliveryDateProperty = GetPropertyDefinition(typeof(Order), "DeliveryDate");
      _nonOrderProperty = GetPropertyDefinition(typeof(OrderItem), "Order");
      _propertyOnNonPersistentDataContainer = GetPropertyDefinition(typeof(OrderViewModel), nameof(OrderViewModel.OrderSum));
      _nonPersistentPropertyOnPersistentDataContainer = GetPropertyDefinition(typeof(OrderTicket), nameof(OrderTicket.Int32TransactionProperty));
    }

    [Test]
    public void CreateNew_IncludesStorageClassPersistentProperties ()
    {
      DataContainer dc = DataContainer.CreateNew(new ObjectID(typeof(ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()));
      var propertyDefinition = GetPropertyDefinition(typeof(ClassWithPropertiesHavingStorageClassAttribute), "Persistent");
      Assert.That(dc.GetValue(propertyDefinition), Is.EqualTo(0));
      Assert.That(dc.GetValue(propertyDefinition, ValueAccess.Original), Is.EqualTo(0));
    }

    [Test]
    public void CreateNew_IncludesStorageClassTransactionProperties ()
    {
      DataContainer dc = DataContainer.CreateNew(new ObjectID(typeof(ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()));
      var propertyDefinition = GetPropertyDefinition(typeof(ClassWithPropertiesHavingStorageClassAttribute), "Transaction");
      Assert.That(dc.GetValue(propertyDefinition), Is.EqualTo(0));
      Assert.That(dc.GetValue(propertyDefinition, ValueAccess.Original), Is.EqualTo(0));
    }

    [Test]
    public void CreateNew_WithLookupValue_IncludesStorageClassPersistentProperties ()
    {
      DataContainer dc = DataContainer.CreateNew(new ObjectID(typeof(ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()), delegate { return 2; });
      var propertyDefinition = GetPropertyDefinition(typeof(ClassWithPropertiesHavingStorageClassAttribute), "Persistent");
      Assert.That(dc.GetValue(propertyDefinition), Is.EqualTo(2));
      Assert.That(dc.GetValue(propertyDefinition, ValueAccess.Original), Is.EqualTo(2));
    }

    [Test]
    public void CreateNew_WithLookupValue_IncludesStorageClassTransactionProperties ()
    {
      DataContainer dc = DataContainer.CreateNew(new ObjectID(typeof(ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()), delegate { return 2; });
      var propertyDefinition = GetPropertyDefinition(typeof(ClassWithPropertiesHavingStorageClassAttribute), "Transaction");
      Assert.That(dc.GetValue(propertyDefinition), Is.EqualTo(2));
      Assert.That(dc.GetValue(propertyDefinition, ValueAccess.Original), Is.EqualTo(2));
    }

    [Test]
    public void CreateNew_NewInHierarchyFlagIsSet ()
    {
      DataContainer dc = DataContainer.CreateNew(new ObjectID(typeof(ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()));
      Assert.That(dc.State.IsNew, Is.True);
      Assert.That(dc.State.IsNewInHierarchy, Is.True);
    }

    [Test]
    public void CreateForExisting_IncludesStorageClassPersistentProperties_WithLookupValue ()
    {
      DataContainer dc = DataContainer.CreateForExisting(
          new ObjectID(typeof(ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()),
          1,
          delegate { return 2; });
      var propertyDefinition = GetPropertyDefinition(typeof(ClassWithPropertiesHavingStorageClassAttribute), "Persistent");
      Assert.That(dc.GetValue(propertyDefinition), Is.EqualTo(2));
      Assert.That(dc.GetValue(propertyDefinition, ValueAccess.Original), Is.EqualTo(2));
    }

    [Test]
    public void CreateForExisting_IncludesStorageClassTransactionProperties_WithLookupValue ()
    {
      DataContainer dc = DataContainer.CreateForExisting(
          new ObjectID(typeof(ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()),
          1,
          delegate { return 2; });
      var propertyDefinition = GetPropertyDefinition(typeof(ClassWithPropertiesHavingStorageClassAttribute), "Transaction");
      Assert.That(dc.GetValue(propertyDefinition), Is.EqualTo(2));
      Assert.That(dc.GetValue(propertyDefinition, ValueAccess.Original), Is.EqualTo(2));
    }

    [Test]
    public void CreateForExisting_NewInHierarchyFlagIsNotSet ()
    {
      DataContainer dc = DataContainer.CreateForExisting(
          new ObjectID(typeof(ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()),
          1,
          delegate { return 2; });
      Assert.That(dc.State.IsUnchanged, Is.True);
      Assert.That(dc.State.IsNew, Is.False);
      Assert.That(dc.State.IsNewInHierarchy, Is.False);
    }

    [Test]
    public void SetEventListener ()
    {
      Assert.That(_existingDataContainer.EventListener, Is.Null);

      _existingDataContainer.SetEventListener(_eventListenerMock.Object);

      Assert.That(_existingDataContainer.EventListener, Is.Not.Null.And.SameAs(_eventListenerMock.Object));
    }

    [Test]
    public void SetEventListener_Twice ()
    {
      _existingDataContainer.SetEventListener(_eventListenerMock.Object);
      Assert.That(
          () => _existingDataContainer.SetEventListener(_eventListenerMock.Object),
          Throws.InvalidOperationException.With.Message.EqualTo("Only one event listener can be registered for a DataContainer."));
    }

    [Test]
    public void GetValue_SetValue_WithPersistentProperty ()
    {
      var valueBeforeChange = _existingDataContainer.GetValue(_orderNumberProperty);
      Assert.That(valueBeforeChange, Is.EqualTo(0));
      var originalValueBeforeChange = _existingDataContainer.GetValue(_orderNumberProperty, ValueAccess.Original);
      Assert.That(originalValueBeforeChange, Is.EqualTo(0));

      _existingDataContainer.SetValue(_orderNumberProperty, 17);

      var valueAfterChange = _existingDataContainer.GetValue(_orderNumberProperty);
      Assert.That(valueAfterChange, Is.EqualTo(17));
      var originalValueAfterChange = _existingDataContainer.GetValue(_orderNumberProperty, ValueAccess.Original);
      Assert.That(originalValueAfterChange, Is.EqualTo(0));
    }

    [Test]
    public void GetValue_SetValue_WithNonPersistentProperty ()
    {
      var valueBeforeChange = _dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer);
      Assert.That(valueBeforeChange, Is.EqualTo(0));
      var originalValueBeforeChange = _dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer, ValueAccess.Original);
      Assert.That(originalValueBeforeChange, Is.EqualTo(0));

      _dataContainerWithNonPersistentProperty.SetValue(_nonPersistentPropertyOnPersistentDataContainer, 17);

      var valueAfterChange = _dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer);
      Assert.That(valueAfterChange, Is.EqualTo(17));
      var originalValueAfterChange = _dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer, ValueAccess.Original);
      Assert.That(originalValueAfterChange, Is.EqualTo(0));
    }

    [Test]
    public void GetValue_RaisesEvent ()
    {
      _existingDataContainer.SetEventListener(_eventListenerMock.Object);

      _existingDataContainer.GetValue(_orderNumberProperty, ValueAccess.Original);

      _eventListenerMock.Verify(mock => mock.PropertyValueReading(_existingDataContainer, _orderNumberProperty, ValueAccess.Original), Times.AtLeastOnce());
      _eventListenerMock.Verify(mock => mock.PropertyValueRead(_existingDataContainer, _orderNumberProperty, 0, ValueAccess.Original), Times.AtLeastOnce());
    }

    [Test]
    public void GetValue_RaisesEvent_Cancellation ()
    {
      _existingDataContainer.SetEventListener(_eventListenerMock.Object);

      var exception = new Exception();
      _eventListenerMock
          .Setup(mock => mock.PropertyValueReading(_existingDataContainer, _orderNumberProperty, ValueAccess.Original))
          .Throws(exception)
          .Verifiable();

      Assert.That(() => _existingDataContainer.GetValue(_orderNumberProperty, ValueAccess.Original), Throws.Exception.SameAs(exception));
      _eventListenerMock.Verify(
          mock => mock.PropertyValueRead(It.IsAny<DataContainer>(), It.IsAny<PropertyDefinition>(), It.IsAny<object>(), It.IsAny<ValueAccess>()),
          Times.Never());
    }

    [Test]
    public void GetValue_Discarded ()
    {
      _existingDataContainer.Discard();

      Assert.That(() => _existingDataContainer.GetValue(_orderNumberProperty), Throws.TypeOf<ObjectInvalidException>());
    }

    [Test]
    public void GetValue_InvalidProperty ()
    {
      Assert.That(
          () => _existingDataContainer.GetValue(_nonOrderProperty),
          Throws.ArgumentException.With.ArgumentExceptionMessageWithParameterNameEqualTo("propertyDefinition"));
    }

    [Test]
    public void SetValue ()
    {
      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty), Is.Not.EqualTo(17));
      Assert.That(_existingDataContainer.HasValueBeenTouched(_orderNumberProperty), Is.False);
      Assert.That(_existingDataContainer.HasValueChanged(_orderNumberProperty), Is.False);

      _existingDataContainer.SetValue(_orderNumberProperty, 17);

      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty), Is.EqualTo(17));
      Assert.That(_existingDataContainer.HasValueBeenTouched(_orderNumberProperty), Is.True);
      Assert.That(_existingDataContainer.HasValueChanged(_orderNumberProperty), Is.True);
    }

    [Test]
    public void SetValue_RaisesEvent ()
    {
      _existingDataContainer.SetEventListener(_eventListenerMock.Object);

      _eventListenerMock
          .Setup(mock => mock.PropertyValueChanging(_existingDataContainer, _orderNumberProperty, 0, 17))
          .Callback(
              (DataContainer dataContainer, PropertyDefinition propertyDefinition, object oldValue, object newValue) =>
                  Assert.That(
                      _existingDataContainer.GetValueWithoutEvents(_orderNumberProperty, ValueAccess.Current),
                      Is.EqualTo(0)))
          .Verifiable();
      _eventListenerMock
          .Setup(mock => mock.PropertyValueChanged(_existingDataContainer, _orderNumberProperty, 0, 17))
          .Callback(
              (DataContainer dataContainer, PropertyDefinition propertyDefinition, object oldValue, object newValue) =>
                  Assert.That(
                      _existingDataContainer.GetValueWithoutEvents(_orderNumberProperty, ValueAccess.Current),
                      Is.EqualTo(17)))
          .Verifiable();

      _existingDataContainer.SetValue(_orderNumberProperty, 17);

      _eventListenerMock.Verify();
      Assert.That(_existingDataContainer.GetValueWithoutEvents(_orderNumberProperty, ValueAccess.Current), Is.EqualTo(17));
    }

    [Test]
    public void SetValue_RaisesEvent_Cancellation ()
    {
      _existingDataContainer.SetEventListener(_eventListenerMock.Object);

      var exception = new Exception();
      _eventListenerMock
          .Setup(mock => mock.PropertyValueChanging(_existingDataContainer, _orderNumberProperty, 0, 17))
          .Throws(exception)
          .Verifiable();

      Assert.That(() => _existingDataContainer.SetValue(_orderNumberProperty, 17), Throws.Exception.SameAs(exception));

      _eventListenerMock.Verify(
          mock => mock.PropertyValueChanged(It.IsAny<DataContainer>(), It.IsAny<PropertyDefinition>(), It.IsAny<object>(), It.IsAny<object>()),
          Times.Never());

      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty), Is.EqualTo(0));
    }

    [Test]
    public void SetValue_SameValue_NoEventButStillTouched ()
    {
      var currentValue = _existingDataContainer.GetValue(_orderNumberProperty);
      _existingDataContainer.SetEventListener(_eventListenerMock.Object);

      _existingDataContainer.SetValue(_orderNumberProperty, currentValue);

      _eventListenerMock.Verify(
          mock => mock.PropertyValueChanging(It.IsAny<DataContainer>(), It.IsAny<PropertyDefinition>(), It.IsAny<object>(), It.IsAny<object>()),
          Times.Never());
      _eventListenerMock.Verify(
          mock => mock.PropertyValueChanged(It.IsAny<DataContainer>(), It.IsAny<PropertyDefinition>(), It.IsAny<object>(), It.IsAny<object>()),
          Times.Never());

      Assert.That(_existingDataContainer.HasValueBeenTouched(_orderNumberProperty), Is.True);
      Assert.That(_existingDataContainer.HasValueChanged(_orderNumberProperty), Is.False);
    }

    [Test]
    public void SetValue_Discarded ()
    {
      _existingDataContainer.Discard();

      Assert.That(() => _existingDataContainer.SetValue(_orderNumberProperty, 17), Throws.TypeOf<ObjectInvalidException>());
    }

    [Test]
    public void SetValue_Deleted ()
    {
      _existingDataContainer.Delete();

      Assert.That(() => _existingDataContainer.SetValue(_orderNumberProperty, 17), Throws.TypeOf<ObjectDeletedException>());
    }

    [Test]
    public void SetValue_InvalidProperty ()
    {
      Assert.That(
          () => _existingDataContainer.SetValue(_nonOrderProperty, 17),
          Throws.ArgumentException.With.ArgumentExceptionMessageWithParameterNameEqualTo("propertyDefinition"));
    }

    [Test]
    public void GetValueWithoutEvents ()
    {
      var valueBeforeChange = _existingDataContainer.GetValueWithoutEvents(_orderNumberProperty, ValueAccess.Current);
      Assert.That(valueBeforeChange, Is.EqualTo(0));
      var originalValueBeforeChange = _existingDataContainer.GetValueWithoutEvents(_orderNumberProperty, ValueAccess.Original);
      Assert.That(originalValueBeforeChange, Is.EqualTo(0));

      _existingDataContainer.SetValue(_orderNumberProperty, 17);

      var valueAfterChange = _existingDataContainer.GetValueWithoutEvents(_orderNumberProperty, ValueAccess.Current);
      Assert.That(valueAfterChange, Is.EqualTo(17));
      var originalValueAfterChange = _existingDataContainer.GetValueWithoutEvents(_orderNumberProperty, ValueAccess.Original);
      Assert.That(originalValueAfterChange, Is.EqualTo(0));
    }

    [Test]
    public void GetValueWithoutEvents_RaisesNoEvent ()
    {
      _existingDataContainer.SetEventListener(_eventListenerMock.Object);

      _existingDataContainer.GetValueWithoutEvents(_orderNumberProperty, ValueAccess.Original);

      _eventListenerMock.Verify(
          mock => mock.PropertyValueReading(It.IsAny<DataContainer>(), It.IsAny<PropertyDefinition>(), It.IsAny<ValueAccess>()),
          Times.Never());
      _eventListenerMock.Verify(
          mock => mock.PropertyValueRead(It.IsAny<DataContainer>(), It.IsAny<PropertyDefinition>(), It.IsAny<object>(), It.IsAny<ValueAccess>()),
          Times.Never());
    }

    [Test]
    public void GetValueWithoutEvents_Discarded ()
    {
      _existingDataContainer.Discard();

      Assert.That(
          () => _existingDataContainer.GetValueWithoutEvents(_orderNumberProperty, ValueAccess.Current), Throws.TypeOf<ObjectInvalidException>());
    }

    [Test]
    public void GetValueWithoutEvents_InvalidProperty ()
    {
      Assert.That(
          () => _existingDataContainer.GetValueWithoutEvents(_nonOrderProperty, ValueAccess.Current),
          Throws.ArgumentException.With.ArgumentExceptionMessageWithParameterNameEqualTo("propertyDefinition"));
    }

    [Test]
    public void HasValueBeenTouched_Discarded ()
    {
      _existingDataContainer.Discard();

      Assert.That(() => _existingDataContainer.HasValueBeenTouched(_orderNumberProperty), Throws.TypeOf<ObjectInvalidException>());
    }

    [Test]
    public void HasValueBeenTouched_InvalidProperty ()
    {
      Assert.That(
          () => _existingDataContainer.HasValueBeenTouched(_nonOrderProperty),
          Throws.ArgumentException.With.ArgumentExceptionMessageWithParameterNameEqualTo("propertyDefinition"));
    }

    [Test]
    public void TouchValue ()
    {
      Assert.That(_existingDataContainer.HasValueBeenTouched(_orderNumberProperty), Is.False);

      _existingDataContainer.TouchValue(_orderNumberProperty);

      Assert.That(_existingDataContainer.HasValueBeenTouched(_orderNumberProperty), Is.True);
    }

    [Test]
    public void TouchValue_Discarded ()
    {
      _existingDataContainer.Discard();

      Assert.That(() => _existingDataContainer.TouchValue(_orderNumberProperty), Throws.TypeOf<ObjectInvalidException>());
    }

    [Test]
    public void TouchValue_InvalidProperty ()
    {
      Assert.That(
          () => _existingDataContainer.TouchValue(_nonOrderProperty),
          Throws.ArgumentException.With.ArgumentExceptionMessageWithParameterNameEqualTo("propertyDefinition"));
    }

    [Test]
    public void HasValueChanged ()
    {
      Assert.That(_existingDataContainer.HasValueChanged(_orderNumberProperty), Is.False);

      _existingDataContainer.SetValue(_orderNumberProperty, 1);

      Assert.That(_existingDataContainer.HasValueChanged(_orderNumberProperty), Is.True);

      _existingDataContainer.SetValue(_orderNumberProperty, 0);

      Assert.That(_existingDataContainer.HasValueChanged(_orderNumberProperty), Is.False);
    }

    [Test]
    public void HasValueChanged_Discarded ()
    {
      _existingDataContainer.Discard();

      Assert.That(() => _existingDataContainer.HasValueChanged(_orderNumberProperty), Throws.TypeOf<ObjectInvalidException>());
    }

    [Test]
    public void HasValueChanged_InvalidProperty ()
    {
      Assert.That(
          () => _existingDataContainer.HasValueChanged(_nonOrderProperty),
          Throws.ArgumentException.With.ArgumentExceptionMessageWithParameterNameEqualTo("propertyDefinition"));
    }

    [Test]
    public void PropertyChange_StateUpdate ()
    {
      var state1 = _existingDataContainer.State;
      Assert.That(state1.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state1), Is.EqualTo(1));
      Assert.That(_existingDataContainer.State.IsUnchanged, Is.True);

      _existingDataContainer.SetValue(_orderNumberProperty, 5);

      var state2 = _existingDataContainer.State;
      Assert.That(state2.IsChanged, Is.True);
      Assert.That(state2.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state2), Is.EqualTo(2));
      Assert.That(_existingDataContainer.State.IsChanged, Is.True);
      Assert.That(_existingDataContainer.State.IsPersistentDataChanged, Is.True);

      _existingDataContainer.SetValue(_orderNumberProperty, 0);

      var state3 = _existingDataContainer.State;
      Assert.That(state3.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state3), Is.EqualTo(1));
      Assert.That(_existingDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void PropertyChange_StateUpdate_WithNewDataContainer ()
    {
      var state1 = _newDataContainer.State;
      Assert.That(state1.IsNew, Is.True);
      Assert.That(state1.IsNewInHierarchy, Is.True);
      Assert.That(_newDataContainer.State.IsNew, Is.True);
      Assert.That(_newDataContainer.State.IsNewInHierarchy, Is.True);
      Assert.That(GetNumberOfSetFlags(state1), Is.EqualTo(2));
      Assert.That(_newDataContainer.GetValue(_orderNumberProperty), Is.Not.EqualTo(17));

      _newDataContainer.SetValue(_orderNumberProperty, 17);

      var state2 = _newDataContainer.State;
      Assert.That(state2.IsNew, Is.True);
      Assert.That(state2.IsNewInHierarchy, Is.True);
      Assert.That(state2.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state2), Is.EqualTo(3));
      Assert.That(_newDataContainer.State.IsNew, Is.True);
      Assert.That(_newDataContainer.State.IsNewInHierarchy, Is.True);
      Assert.That(_newDataContainer.State.IsPersistentDataChanged, Is.True);

      _newDataContainer.SetValue(_orderNumberProperty, 0);

      var state3 = _newDataContainer.State;
      Assert.That(state3.IsNew, Is.True);
      Assert.That(state3.IsNewInHierarchy, Is.True);
      Assert.That(GetNumberOfSetFlags(state3), Is.EqualTo(2));
      Assert.That(_newDataContainer.State.IsNew, Is.True);
      Assert.That(_newDataContainer.State.IsNewInHierarchy, Is.True);
    }

    [Test]
    public void PropertyChange_StateUpdate_WithNewNonPersistentDataContainer ()
    {
      var state1 = _newNonPersistentDataContainer.State;
      Assert.That(state1.IsNew, Is.True);
      Assert.That(state1.IsNewInHierarchy, Is.True);
      Assert.That(_newNonPersistentDataContainer.State.IsNew, Is.True);
      Assert.That(_newNonPersistentDataContainer.State.IsNewInHierarchy, Is.True);
      Assert.That(GetNumberOfSetFlags(state1), Is.EqualTo(2));
      Assert.That(_newNonPersistentDataContainer.GetValue(_propertyOnNonPersistentDataContainer), Is.Not.EqualTo(17));

      _newNonPersistentDataContainer.SetValue(_propertyOnNonPersistentDataContainer, 17);

      var state2 = _newNonPersistentDataContainer.State;
      Assert.That(state2.IsNew, Is.True);
      Assert.That(state2.IsNewInHierarchy, Is.True);
      Assert.That(state2.IsNonPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state2), Is.EqualTo(3));
      Assert.That(_newNonPersistentDataContainer.State.IsNew, Is.True);
      Assert.That(_newNonPersistentDataContainer.State.IsNewInHierarchy, Is.True);
      Assert.That(_newNonPersistentDataContainer.State.IsNonPersistentDataChanged, Is.True);

      _newNonPersistentDataContainer.SetValue(_propertyOnNonPersistentDataContainer, 0);

      var state3 = _newNonPersistentDataContainer.State;
      Assert.That(state3.IsNew, Is.True);
      Assert.That(state3.IsNewInHierarchy, Is.True);
      Assert.That(GetNumberOfSetFlags(state3), Is.EqualTo(2));
      Assert.That(_newNonPersistentDataContainer.State.IsNew, Is.True);
      Assert.That(_newNonPersistentDataContainer.State.IsNewInHierarchy, Is.True);
    }

    [Test]
    public void PropertyChange_StateUpdate_WithDeletedDataContainer ()
    {
      var dataContainer = _existingDataContainer;
      dataContainer.SetValue(_orderNumberProperty, 1);
      dataContainer.Delete();

      var state = dataContainer.State;
      Assert.That(state.IsDeleted, Is.True);
      Assert.That(state.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state), Is.EqualTo(2));
      Assert.That(dataContainer.State.IsDeleted, Is.True);
      Assert.That(dataContainer.State.IsPersistentDataChanged, Is.True);
    }

    [Test]
    public void PropertyChange_StateUpdate_WithNonPersistentProperty ()
    {
      var state1 = _dataContainerWithNonPersistentProperty.State;
      Assert.That(state1.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state1), Is.EqualTo(1));
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsUnchanged, Is.True);

      _dataContainerWithNonPersistentProperty.SetValue(_nonPersistentPropertyOnPersistentDataContainer, 5);

      var state2 = _dataContainerWithNonPersistentProperty.State;
      Assert.That(state2.IsChanged, Is.True);
      Assert.That(state2.IsNonPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state2), Is.EqualTo(2));
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsChanged, Is.True);
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsNonPersistentDataChanged, Is.True);

      _dataContainerWithNonPersistentProperty.SetValue(_nonPersistentPropertyOnPersistentDataContainer, 0);

      var state3 = _dataContainerWithNonPersistentProperty.State;
      Assert.That(state3.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state3), Is.EqualTo(1));
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsUnchanged, Is.True);
    }

    [Test]
    public void PropertyChange_StateUpdate_WithNonPersistentDataContainer ()
    {
      var state1 = _existingNonPersistentDataContainer.State;
      Assert.That(state1.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state1), Is.EqualTo(1));
      Assert.That(_existingNonPersistentDataContainer.State.IsUnchanged, Is.True);

      _existingNonPersistentDataContainer.SetValue(_propertyOnNonPersistentDataContainer, 5);

      var state2 = _existingNonPersistentDataContainer.State;
      Assert.That(state2.IsChanged, Is.True);
      Assert.That(state2.IsNonPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state2), Is.EqualTo(2));
      Assert.That(_existingNonPersistentDataContainer.State.IsChanged, Is.True);
      Assert.That(_existingNonPersistentDataContainer.State.IsNonPersistentDataChanged, Is.True);

      _existingNonPersistentDataContainer.SetValue(_propertyOnNonPersistentDataContainer, 0);

      var state3 = _existingNonPersistentDataContainer.State;
      Assert.That(state3.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state3), Is.EqualTo(1));
      Assert.That(_existingNonPersistentDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void PropertyChange_RaisesStateUpdated ()
    {
      CheckStateNotification(
          _existingDataContainer,
          dc => dc.SetValue(_orderNumberProperty, 5),
          new DataContainerState.Builder().SetChanged().SetPersistentDataChanged().Value);
    }

    [Test]
    public void PropertyChange_RaisesStateUpdated_WithNewDataContainer ()
    {
      CheckStateNotification(
          _newDataContainer,
          dc => dc.SetValue(_orderNumberProperty, 5),
          new DataContainerState.Builder().SetNew().SetNewInHierarchy().SetPersistentDataChanged().Value);
    }

    [Test]
    public void PropertyChange_RaisesStateUpdated_WithNonPersistentProperty ()
    {
      CheckStateNotification(
          _dataContainerWithNonPersistentProperty,
          dc => dc.SetValue(_nonPersistentPropertyOnPersistentDataContainer, 5),
          new DataContainerState.Builder().SetChanged().SetNonPersistentDataChanged().Value);
    }

    [Test]
    public void PropertyChange_ChangeBack_RaisesStateUpdated ()
    {
      _existingDataContainer.SetValue(_orderNumberProperty, 5);
      CheckStateNotification(_existingDataContainer, dc => dc.SetValue(_orderNumberProperty, 0), new DataContainerState.Builder().SetUnchanged().Value);
    }

    [Test]
    public void CommitValue_CommitsOnlyGivenPropertyValue_AndUpdatesState ()
    {
      _existingDataContainer.SetValue(_orderNumberProperty, 10);
      _existingDataContainer.SetValue(_deliveryDateProperty, new DateTime(2012, 05, 13));

      Assert.That(_existingDataContainer.HasValueChanged(_orderNumberProperty), Is.True);
      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty, ValueAccess.Original), Is.EqualTo(0));
      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty), Is.EqualTo(10));

      Assert.That(_existingDataContainer.HasValueChanged(_deliveryDateProperty), Is.True);
      Assert.That(_existingDataContainer.GetValue(_deliveryDateProperty, ValueAccess.Original), Is.EqualTo(new DateTime()));
      Assert.That(_existingDataContainer.GetValue(_deliveryDateProperty), Is.EqualTo(new DateTime(2012, 05, 13)));

      var state1 = _existingDataContainer.State;
      Assert.That(state1.IsChanged, Is.True);
      Assert.That(state1.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state1), Is.EqualTo(2));
      Assert.That(_existingDataContainer.State.IsChanged, Is.True);
      Assert.That(_existingDataContainer.State.IsPersistentDataChanged, Is.True);

      _existingDataContainer.CommitValue(_orderNumberProperty);

      Assert.That(_existingDataContainer.HasValueChanged(_orderNumberProperty), Is.False);
      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty, ValueAccess.Original), Is.EqualTo(10));
      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty), Is.EqualTo(10));

      Assert.That(_existingDataContainer.HasValueChanged(_deliveryDateProperty), Is.True);
      Assert.That(_existingDataContainer.GetValue(_deliveryDateProperty, ValueAccess.Original), Is.EqualTo(new DateTime()));
      Assert.That(_existingDataContainer.GetValue(_deliveryDateProperty), Is.EqualTo(new DateTime(2012, 05, 13)));

      var state2 = _existingDataContainer.State;
      Assert.That(state2.IsChanged, Is.True);
      Assert.That(state2.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state2), Is.EqualTo(2));
      Assert.That(_existingDataContainer.State.IsChanged, Is.True);
      Assert.That(_existingDataContainer.State.IsPersistentDataChanged, Is.True);

      _existingDataContainer.CommitValue(_deliveryDateProperty);

      var state3 = _existingDataContainer.State;
      Assert.That(state3.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state3), Is.EqualTo(1));
      Assert.That(_existingDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void CommitValue_CommitsOnlyGivenPropertyValue_AndUpdatesState_WithNonPersistentProperty ()
    {
      _dataContainerWithNonPersistentProperty.SetValue(_nonPersistentPropertyOnPersistentDataContainer, 10);

      Assert.That(_dataContainerWithNonPersistentProperty.HasValueChanged(_nonPersistentPropertyOnPersistentDataContainer), Is.True);
      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer, ValueAccess.Original), Is.EqualTo(0));
      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer), Is.EqualTo(10));

      var state1 = _dataContainerWithNonPersistentProperty.State;
      Assert.That(state1.IsChanged, Is.True);
      Assert.That(state1.IsNonPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state1), Is.EqualTo(2));
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsChanged, Is.True);
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsNonPersistentDataChanged, Is.True);

      _dataContainerWithNonPersistentProperty.CommitValue(_nonPersistentPropertyOnPersistentDataContainer);

      Assert.That(_dataContainerWithNonPersistentProperty.HasValueChanged(_nonPersistentPropertyOnPersistentDataContainer), Is.False);
      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer, ValueAccess.Original), Is.EqualTo(10));
      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer), Is.EqualTo(10));

      var state2 = _dataContainerWithNonPersistentProperty.State;
      Assert.That(state2.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state2), Is.EqualTo(1));
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsUnchanged, Is.True);
    }

    [Test]
    public void CommitValue_Discarded ()
    {
      _existingDataContainer.Discard();

      Assert.That(() => _existingDataContainer.CommitValue(_orderNumberProperty), Throws.TypeOf<ObjectInvalidException>());
    }

    [Test]
    public void CommitValue_InvalidProperty ()
    {
      Assert.That(
          () => _existingDataContainer.CommitValue(_nonOrderProperty),
          Throws.ArgumentException.With.ArgumentExceptionMessageWithParameterNameEqualTo("propertyDefinition"));
    }

    [Test]
    public void RollbackValue_RollsBackOnlyGivenPropertyValue_AndUpdatesState ()
    {
      _existingDataContainer.SetValue(_orderNumberProperty, 10);
      _existingDataContainer.SetValue(_deliveryDateProperty, new DateTime(2012, 05, 13));

      Assert.That(_existingDataContainer.HasValueChanged(_orderNumberProperty), Is.True);
      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty, ValueAccess.Original), Is.EqualTo(0));
      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty), Is.EqualTo(10));

      Assert.That(_existingDataContainer.HasValueChanged(_deliveryDateProperty), Is.True);
      Assert.That(_existingDataContainer.GetValue(_deliveryDateProperty, ValueAccess.Original), Is.EqualTo(new DateTime()));
      Assert.That(_existingDataContainer.GetValue(_deliveryDateProperty), Is.EqualTo(new DateTime(2012, 05, 13)));

      var state1 = _existingDataContainer.State;
      Assert.That(state1.IsChanged, Is.True);
      Assert.That(state1.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state1), Is.EqualTo(2));
      Assert.That(_existingDataContainer.State.IsChanged, Is.True);
      Assert.That(_existingDataContainer.State.IsPersistentDataChanged, Is.True);

      _existingDataContainer.RollbackValue(_orderNumberProperty);

      Assert.That(_existingDataContainer.HasValueChanged(_orderNumberProperty), Is.False);
      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty, ValueAccess.Original), Is.EqualTo(0));
      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty), Is.EqualTo(0));

      Assert.That(_existingDataContainer.HasValueChanged(_deliveryDateProperty), Is.True);
      Assert.That(_existingDataContainer.GetValue(_deliveryDateProperty, ValueAccess.Original), Is.EqualTo(new DateTime()));
      Assert.That(_existingDataContainer.GetValue(_deliveryDateProperty), Is.EqualTo(new DateTime(2012, 05, 13)));

      var state2 = _existingDataContainer.State;
      Assert.That(state2.IsChanged, Is.True);
      Assert.That(state2.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state2), Is.EqualTo(2));
      Assert.That(_existingDataContainer.State.IsChanged, Is.True);
      Assert.That(_existingDataContainer.State.IsPersistentDataChanged, Is.True);

      _existingDataContainer.RollbackValue(_deliveryDateProperty);

      var state3 = _existingDataContainer.State;
      Assert.That(state3.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state3), Is.EqualTo(1));
      Assert.That(_existingDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void RollbackValue_RollsBackOnlyGivenPropertyValue_AndUpdatesState_WithNonPersistentProperty ()
    {
      _dataContainerWithNonPersistentProperty.SetValue(_nonPersistentPropertyOnPersistentDataContainer, 10);

      Assert.That(_dataContainerWithNonPersistentProperty.HasValueChanged(_nonPersistentPropertyOnPersistentDataContainer), Is.True);
      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer, ValueAccess.Original), Is.EqualTo(0));
      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer), Is.EqualTo(10));

      var state1 = _dataContainerWithNonPersistentProperty.State;
      Assert.That(state1.IsChanged, Is.True);
      Assert.That(state1.IsNonPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state1), Is.EqualTo(2));
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsChanged, Is.True);
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsNonPersistentDataChanged, Is.True);

      _dataContainerWithNonPersistentProperty.RollbackValue(_nonPersistentPropertyOnPersistentDataContainer);

      Assert.That(_dataContainerWithNonPersistentProperty.HasValueChanged(_nonPersistentPropertyOnPersistentDataContainer), Is.False);
      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer, ValueAccess.Original), Is.EqualTo(0));
      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer), Is.EqualTo(0));

      var state2 = _dataContainerWithNonPersistentProperty.State;
      Assert.That(state2.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state2), Is.EqualTo(1));
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsUnchanged, Is.True);
    }

    [Test]
    public void RollbackValue_Discarded ()
    {
      _existingDataContainer.Discard();

      Assert.That(() => _existingDataContainer.RollbackValue(_orderNumberProperty), Throws.TypeOf<ObjectInvalidException>());
    }

    [Test]
    public void RollbackValue_InvalidProperty ()
    {
      Assert.That(
          () => _existingDataContainer.RollbackValue(_nonOrderProperty),
          Throws.ArgumentException.With.ArgumentExceptionMessageWithParameterNameEqualTo("propertyDefinition"));
    }

    [Test]
    public void SetPropertyValueDataFromSubTransaction_SetsOnlyGivenPropertyValue_AndUpdatesState ()
    {
      var sourceDataContainer = DataContainerObjectMother.CreateExisting(_existingDataContainer.ID);
      sourceDataContainer.SetValue(_orderNumberProperty, 17);
      sourceDataContainer.SetValue(_deliveryDateProperty, new DateTime(2012, 05, 13));

      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty), Is.EqualTo(0));
      Assert.That(_existingDataContainer.GetValue(_deliveryDateProperty), Is.EqualTo(new DateTime()));
      var stateBeforeChange = _existingDataContainer.State;
      Assert.That(stateBeforeChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(1));
      Assert.That(_existingDataContainer.State.IsUnchanged, Is.True);

      _existingDataContainer.SetPropertyValueFromSubTransaction(_orderNumberProperty, sourceDataContainer);

      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty), Is.EqualTo(17));
      Assert.That(_existingDataContainer.GetValue(_deliveryDateProperty), Is.EqualTo(new DateTime()));
      var stateAfterChange = _existingDataContainer.State;
      Assert.That(stateAfterChange.IsChanged, Is.True);
      Assert.That(stateAfterChange.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(2));
      Assert.That(_existingDataContainer.State.IsChanged, Is.True);
      Assert.That(_existingDataContainer.State.IsPersistentDataChanged, Is.True);
    }

    [Test]
    public void SetPropertyValueDataFromSubTransaction_SetsOnlyGivenPropertyValue_AndUpdatesState_WithNonPersistentProperty ()
    {
      var sourceDataContainer = DataContainerObjectMother.CreateExisting(_dataContainerWithNonPersistentProperty.ID);
      sourceDataContainer.SetValue(_nonPersistentPropertyOnPersistentDataContainer, 17);

      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer), Is.EqualTo(0));
      var stateBeforeChange = _dataContainerWithNonPersistentProperty.State;
      Assert.That(stateBeforeChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(1));
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsUnchanged, Is.True);

      _dataContainerWithNonPersistentProperty.SetPropertyValueFromSubTransaction(_nonPersistentPropertyOnPersistentDataContainer, sourceDataContainer);

      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer), Is.EqualTo(17));
      var stateAfterChange = _dataContainerWithNonPersistentProperty.State;
      Assert.That(stateAfterChange.IsChanged, Is.True);
      Assert.That(stateAfterChange.IsNonPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(2));
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsChanged, Is.True);
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsNonPersistentDataChanged, Is.True);
    }

    [Test]
    public void SetPropertyValueDataFromSubTransaction_Discarded ()
    {
      var sourceDataContainer = DataContainerObjectMother.Create(_existingDataContainer.ID);
      _existingDataContainer.Discard();
      Assert.That(
          () => _existingDataContainer.SetPropertyValueFromSubTransaction(_orderNumberProperty, sourceDataContainer),
          Throws.TypeOf<ObjectInvalidException>());
    }

    [Test]
    public void SetPropertyValueDataFromSubTransaction_DiscardedSource ()
    {
      var sourceDataContainer = DataContainerObjectMother.Create(_existingDataContainer.ID);
      sourceDataContainer.Discard();
      Assert.That(
          () => _existingDataContainer.SetPropertyValueFromSubTransaction(_orderNumberProperty, sourceDataContainer),
          Throws.TypeOf<ObjectInvalidException>());
    }

    [Test]
    public void SetPropertyValueDataFromSubTransaction_InvalidProperty ()
    {
      var sourceDataContainer = DataContainerObjectMother.Create(_existingDataContainer.ID);
      Assert.That(
          () => _existingDataContainer.SetPropertyValueFromSubTransaction(_nonOrderProperty, sourceDataContainer),
          Throws.ArgumentException.With.ArgumentExceptionMessageWithParameterNameEqualTo("propertyDefinition"));
    }

    [Test]
    public void SetPropertyValueDataFromSubTransaction_InvalidSource ()
    {
      var sourceDataContainer = DataContainerObjectMother.Create(DomainObjectIDs.Customer1);
      Assert.That(
          () => _existingDataContainer.SetPropertyValueFromSubTransaction(_orderNumberProperty, sourceDataContainer),
          Throws.ArgumentException.With.ArgumentExceptionMessageEqualTo(
              "Cannot set this data container's property values from 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid'; the data "
              + "containers do not have the same class definition.", "source"));
    }

    [Test]
    public void SetTimestamp ()
    {
      _existingDataContainer.SetTimestamp(10);

      Assert.That(_existingDataContainer.Timestamp, Is.EqualTo(10));
    }

    [Test]
    [Obsolete("DataContainer.Clone() is obsolete.")]
    public void Clone_SetsID ()
    {
      var original = _existingDataContainer;
      Assert.That(original.ID, Is.Not.EqualTo(DomainObjectIDs.Order3));

      var clone = original.Clone(DomainObjectIDs.Order3);
      Assert.That(clone.ID, Is.EqualTo(DomainObjectIDs.Order3));
    }

    [Test]
    [Obsolete("DataContainer.Clone() is obsolete.")]
    public void Clone_CopiesState ()
    {
      var originalNew = _newDataContainer;
      Assert.That(originalNew.State.IsNew, Is.True);
      Assert.That(originalNew.State.IsNew, Is.True);

      var originalExisting = _existingDataContainer;
      Assert.That(originalExisting.State.IsUnchanged, Is.True);
      Assert.That(originalExisting.State.IsUnchanged, Is.True);

      var clonedNew = originalNew.Clone(DomainObjectIDs.Order4);
      Assert.That(clonedNew.State.IsNew, Is.True);
      Assert.That(clonedNew.State.IsNew, Is.True);

      var clonedExisting = originalExisting.Clone(DomainObjectIDs.Order5);
      Assert.That(clonedExisting.State.IsUnchanged, Is.True);
      Assert.That(clonedExisting.State.IsUnchanged, Is.True);
    }

    [Test]
    [Obsolete("DataContainer.Clone() is obsolete.")]
    public void Clone_CopiesTimestamp ()
    {
      var original = _newDataContainer;
      original.SetTimestamp(12);

      var clone = original.Clone(DomainObjectIDs.Order3);
      Assert.That(clone.Timestamp, Is.EqualTo(12));
    }

    [Test]
    [Obsolete("DataContainer.Clone() is obsolete.")]
    public void Clone_CopiesPropertyValues_WithPersistentProperty ()
    {
      var original = _existingDataContainer;

      var clone = original.Clone(DomainObjectIDs.Order3);

      Assert.That(
          clone.ClassDefinition.GetPropertyDefinitions().Select(pd => clone.GetValue(pd)),
          Is.EqualTo(clone.ClassDefinition.GetPropertyDefinitions().Select(pd => original.GetValue(pd))));
      Assert.That(
          clone.ClassDefinition.GetPropertyDefinitions().Select(pd => clone.GetValue(pd, ValueAccess.Original)),
          Is.EqualTo(clone.ClassDefinition.GetPropertyDefinitions().Select(pd => original.GetValue(pd, ValueAccess.Original))));
    }

    [Test]
    [Obsolete("DataContainer.Clone() is obsolete.")]
    public void Clone_CopiesPropertyValues_WithNonPersistentProperty ()
    {
      var original = _dataContainerWithNonPersistentProperty;

      var clone = original.Clone(new ObjectID(original.ClassDefinition, Guid.NewGuid()));

      Assert.That(
          clone.ClassDefinition.GetPropertyDefinitions().Select(pd => clone.GetValue(pd)),
          Is.EqualTo(clone.ClassDefinition.GetPropertyDefinitions().Select(pd => original.GetValue(pd))));
      Assert.That(
          clone.ClassDefinition.GetPropertyDefinitions().Select(pd => clone.GetValue(pd, ValueAccess.Original)),
          Is.EqualTo(clone.ClassDefinition.GetPropertyDefinitions().Select(pd => original.GetValue(pd, ValueAccess.Original))));
    }

    [Test]
    [Obsolete("DataContainer.Clone() is obsolete.")]
    public void Clone_CopiesHasBeenMarkedChanged ()
    {
      var original = _existingDataContainer;
      original.MarkAsChanged();
      Assert.That(original.HasBeenMarkedChanged, Is.True);

      var clone = original.Clone(DomainObjectIDs.Order3);
      Assert.That(clone.HasBeenMarkedChanged, Is.True);
    }

    [Test]
    [Obsolete("DataContainer.Clone() is obsolete.")]
    public void Clone_CopiesHasBeenChangedFlag_WithPersistentProperty ()
    {
      var original = _existingDataContainer;
      original.SetValue(_orderNumberProperty, 10);
      var state1 = original.State;
      Assert.That(state1.IsChanged, Is.True);
      Assert.That(state1.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state1), Is.EqualTo(2));
      Assert.That(original.State.IsChanged, Is.True);
      Assert.That(original.State.IsPersistentDataChanged, Is.True);

      var clone = original.Clone(DomainObjectIDs.Order3);
      var state2 = clone.State;
      Assert.That(state2.IsChanged, Is.True);
      Assert.That(state2.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state1), Is.EqualTo(2));
      Assert.That(clone.State.IsChanged, Is.True);
      Assert.That(clone.State.IsPersistentDataChanged, Is.True);
    }

    [Test]
    [Obsolete("DataContainer.Clone() is obsolete.")]
    public void Clone_CopiesHasBeenChangedFlag_WithNonPersistentProperty ()
    {
      var original = _dataContainerWithNonPersistentProperty;
      original.SetValue(_nonPersistentPropertyOnPersistentDataContainer, 10);
      var state1 = original.State;
      Assert.That(state1.IsChanged, Is.True);
      Assert.That(state1.IsNonPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state1), Is.EqualTo(2));
      Assert.That(original.State.IsChanged, Is.True);
      Assert.That(original.State.IsNonPersistentDataChanged, Is.True);

      var clone = original.Clone(new ObjectID(original.ClassDefinition, Guid.NewGuid()));
      var state2 = clone.State;
      Assert.That(state2.IsChanged, Is.True);
      Assert.That(state2.IsNonPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state1), Is.EqualTo(2));
      Assert.That(clone.State.IsChanged, Is.True);
      Assert.That(clone.State.IsNonPersistentDataChanged, Is.True);
    }

    [Test]
    [Obsolete("DataContainer.Clone() is obsolete.")]
    public void Clone_DomainObjectEmpty ()
    {
      var original = _existingDataContainer;
      original.SetDomainObject(DomainObjectMother.CreateFakeObject(original.ID));
      Assert.That(original.HasDomainObject, Is.True);

      var clone = original.Clone(DomainObjectIDs.Order1);
      Assert.That(clone.HasDomainObject, Is.False);
    }

    [Test]
    [Obsolete("DataContainer.Clone() is obsolete.")]
    public void Clone_TransactionEmpty ()
    {
      var original = _existingDataContainer;
      original.SetDomainObject(DomainObjectMother.CreateFakeObject(original.ID));
      TestableClientTransaction.DataManager.RegisterDataContainer(original);
      Assert.That(original.IsRegistered, Is.True);

      var clone = original.Clone(DomainObjectIDs.Order1);
      Assert.That(clone.IsRegistered, Is.False);
    }

    [Test]
    [Obsolete("DataContainer.Clone() is obsolete.")]
    public void Clone_EventListenerEmpty ()
    {
      _existingDataContainer.SetEventListener(_eventListenerMock.Object);
      Assert.That(_existingDataContainer.EventListener, Is.Not.Null);

      var clone = _existingDataContainer.Clone(DomainObjectIDs.Order1);
      Assert.That(clone.EventListener, Is.Null);
    }

    [Test]
    public void CommitState_SetsStateToExisting ()
    {
      _newDataContainer.CommitState();

      var state = _newDataContainer.State;
      Assert.That(state.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state), Is.EqualTo(2));
      Assert.That(_newDataContainer.State.IsUnchanged, Is.True);
      Assert.That(_newDataContainer.State.IsNewInHierarchy, Is.True);
    }

    [Test]
    public void CommitState_RaisesStateUpdated ()
    {
      CheckStateNotification(_newDataContainer, dc => dc.CommitState(), new DataContainerState.Builder().SetUnchanged().SetNewInHierarchy().Value);
    }

    [Test]
    public void CommitState_CommitsPropertyValues_WithPersistentProperty ()
    {
      _existingDataContainer.SetValue(_orderNumberProperty, 10);

      Assert.That(_existingDataContainer.HasValueChanged(_orderNumberProperty), Is.True);
      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty, ValueAccess.Original), Is.EqualTo(0));
      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty), Is.EqualTo(10));

      _existingDataContainer.CommitState();

      Assert.That(_existingDataContainer.HasValueChanged(_orderNumberProperty), Is.False);
      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty, ValueAccess.Original), Is.EqualTo(10));
      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty), Is.EqualTo(10));
    }

    [Test]
    public void CommitState_CommitsPropertyValues_WithNonPersistentProperty ()
    {
      _dataContainerWithNonPersistentProperty.SetValue(_nonPersistentPropertyOnPersistentDataContainer, 10);

      Assert.That(_dataContainerWithNonPersistentProperty.HasValueChanged(_nonPersistentPropertyOnPersistentDataContainer), Is.True);
      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer, ValueAccess.Original), Is.EqualTo(0));
      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer), Is.EqualTo(10));

      _dataContainerWithNonPersistentProperty.CommitState();

      Assert.That(_dataContainerWithNonPersistentProperty.HasValueChanged(_nonPersistentPropertyOnPersistentDataContainer), Is.False);
      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer, ValueAccess.Original), Is.EqualTo(10));
      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer), Is.EqualTo(10));
    }

    [Test]
    public void CommitState_ResetsChangedFlag_WithPersistentProperty ()
    {
      _existingDataContainer.SetValue(_orderNumberProperty, 10);

      var stateBeforeChange = _existingDataContainer.State;
      Assert.That(stateBeforeChange.IsChanged, Is.True);
      Assert.That(stateBeforeChange.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(2));
      Assert.That(_existingDataContainer.State.IsChanged, Is.True);
      Assert.That(_existingDataContainer.State.IsPersistentDataChanged, Is.True);

      _existingDataContainer.CommitState();

      var stateAfterChange = _existingDataContainer.State;
      Assert.That(stateAfterChange.IsUnchanged, Is.True);
      Assert.That(stateAfterChange.IsPersistentDataChanged, Is.False);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(1));
      Assert.That(_existingDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void CommitState_ResetsChangedFlag_WithNonPersistentProperty ()
    {
      _dataContainerWithNonPersistentProperty.SetValue(_nonPersistentPropertyOnPersistentDataContainer, 10);

      var stateBeforeChange = _dataContainerWithNonPersistentProperty.State;
      Assert.That(stateBeforeChange.IsChanged, Is.True);
      Assert.That(stateBeforeChange.IsNonPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(2));
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsChanged, Is.True);
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsNonPersistentDataChanged, Is.True);

      _dataContainerWithNonPersistentProperty.CommitState();

      var stateAfterChange = _dataContainerWithNonPersistentProperty.State;
      Assert.That(stateAfterChange.IsUnchanged, Is.True);
      Assert.That(stateAfterChange.IsNonPersistentDataChanged, Is.False);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(1));
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsUnchanged, Is.True);
    }

    [Test]
    public void CommitState_ResetsMarkedChangedFlag_WithPersistentDataContainer ()
    {
      _existingDataContainer.MarkAsChanged();
      Assert.That(_existingDataContainer.HasBeenMarkedChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(_existingDataContainer.State), Is.EqualTo(2));
      Assert.That(_existingDataContainer.State.IsChanged, Is.True);
      Assert.That(_existingDataContainer.State.IsPersistentDataChanged, Is.True);

      _existingDataContainer.CommitState();

      Assert.That(_existingDataContainer.HasBeenMarkedChanged, Is.False);
      Assert.That(GetNumberOfSetFlags(_existingDataContainer.State), Is.EqualTo(1));
      Assert.That(_existingDataContainer.State.IsUnchanged, Is.True);
      Assert.That(_existingDataContainer.State.IsPersistentDataChanged, Is.False);
    }

    [Test]
    public void CommitState_ResetsMarkedChangedFlag_WithNonPersistentDataContainer ()
    {
      _existingNonPersistentDataContainer.MarkAsChanged();
      Assert.That(_existingNonPersistentDataContainer.HasBeenMarkedChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(_existingNonPersistentDataContainer.State), Is.EqualTo(2));
      Assert.That(_existingNonPersistentDataContainer.State.IsChanged, Is.True);
      Assert.That(_existingNonPersistentDataContainer.State.IsNonPersistentDataChanged, Is.True);

      _existingNonPersistentDataContainer.CommitState();

      Assert.That(_existingNonPersistentDataContainer.HasBeenMarkedChanged, Is.False);
      Assert.That(GetNumberOfSetFlags(_dataContainerWithNonPersistentProperty.State), Is.EqualTo(1));
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsUnchanged, Is.True);
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsNonPersistentDataChanged, Is.False);
    }

    [Test]
    public void CommitState_DiscardedDataContainer_Throws ()
    {
      Assert.That(
          () => _discardedDataContainer.CommitState(),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void CommitState_DeletedDataContainer_Throws ()
    {
      Assert.That(
          () => _deletedDataContainer.CommitState(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Deleted data containers cannot be committed, they have to be discarded."));
    }

    [Test]
    public void CommitPropertyValuesOnNewDataContainer_LeavesStateUnchanged ()
    {
      _newDataContainer.CommitPropertyValuesOnNewDataContainer();

      var state = _newDataContainer.State;
      Assert.That(state.IsNew, Is.True);
      Assert.That(state.IsNewInHierarchy, Is.True);
      Assert.That(GetNumberOfSetFlags(state), Is.EqualTo(2));
      Assert.That(_newDataContainer.State.IsNew, Is.True);
      Assert.That(_newDataContainer.State.IsNewInHierarchy, Is.True);
    }

    [Test]
    public void CommitPropertyValuesOnNewDataContainer_RaisesStateUpdated ()
    {
      CheckStateNotification(_newDataContainer, dc => dc.CommitPropertyValuesOnNewDataContainer(), new DataContainerState.Builder().SetNew().SetNewInHierarchy().Value);
    }

    [Test]
    public void CommitPropertyValuesOnNewDataContainer_CommitsPropertyValues_WithPersistentProperty ()
    {
      _newDataContainer.SetValue(_orderNumberProperty, 10);

      Assert.That(_newDataContainer.HasValueChanged(_orderNumberProperty), Is.True);
      Assert.That(_newDataContainer.GetValue(_orderNumberProperty, ValueAccess.Original), Is.EqualTo(0));
      Assert.That(_newDataContainer.GetValue(_orderNumberProperty), Is.EqualTo(10));

      _newDataContainer.CommitPropertyValuesOnNewDataContainer();

      Assert.That(_newDataContainer.HasValueChanged(_orderNumberProperty), Is.False);
      Assert.That(_newDataContainer.GetValue(_orderNumberProperty, ValueAccess.Original), Is.EqualTo(10));
      Assert.That(_newDataContainer.GetValue(_orderNumberProperty), Is.EqualTo(10));
    }

    [Test]
    public void CommitPropertyValuesOnNewDataContainer_CommitsPropertyValues_WithNonPersistentProperty ()
    {
      _newNonPersistentDataContainer.SetValue(_propertyOnNonPersistentDataContainer, 10);

      Assert.That(_newNonPersistentDataContainer.HasValueChanged(_propertyOnNonPersistentDataContainer), Is.True);
      Assert.That(_newNonPersistentDataContainer.GetValue(_propertyOnNonPersistentDataContainer, ValueAccess.Original), Is.EqualTo(0));
      Assert.That(_newNonPersistentDataContainer.GetValue(_propertyOnNonPersistentDataContainer), Is.EqualTo(10));

      _newNonPersistentDataContainer.CommitPropertyValuesOnNewDataContainer();

      Assert.That(_newNonPersistentDataContainer.HasValueChanged(_propertyOnNonPersistentDataContainer), Is.False);
      Assert.That(_newNonPersistentDataContainer.GetValue(_propertyOnNonPersistentDataContainer, ValueAccess.Original), Is.EqualTo(10));
      Assert.That(_newNonPersistentDataContainer.GetValue(_propertyOnNonPersistentDataContainer), Is.EqualTo(10));
    }

    [Test]
    public void CommitPropertyValuesOnNewDataContainer_ResetsChangedFlag_WithPersistentProperty ()
    {
      _newDataContainer.SetValue(_orderNumberProperty, 10);

      var stateBeforeChange = _newDataContainer.State;
      Assert.That(stateBeforeChange.IsNew, Is.True);
      Assert.That(stateBeforeChange.IsNewInHierarchy, Is.True);
      Assert.That(stateBeforeChange.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(3));
      Assert.That(_newDataContainer.State.IsNew, Is.True);
      Assert.That(_newDataContainer.State.IsNewInHierarchy, Is.True);
      Assert.That(_newDataContainer.State.IsPersistentDataChanged, Is.True);

      _newDataContainer.CommitPropertyValuesOnNewDataContainer();

      var stateAfterChange = _newDataContainer.State;
      Assert.That(stateAfterChange.IsNew, Is.True);
      Assert.That(stateAfterChange.IsNewInHierarchy, Is.True);
      Assert.That(stateAfterChange.IsPersistentDataChanged, Is.False);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(2));
      Assert.That(_newDataContainer.State.IsNew, Is.True);
      Assert.That(_newDataContainer.State.IsNewInHierarchy, Is.True);
    }

    [Test]
    public void CommitPropertyValuesOnNewDataContainer_ResetsChangedFlag_WithNonPersistentProperty ()
    {
      _newNonPersistentDataContainer.SetValue(_propertyOnNonPersistentDataContainer, 10);

      var stateBeforeChange = _newNonPersistentDataContainer.State;
      Assert.That(stateBeforeChange.IsNew, Is.True);
      Assert.That(stateBeforeChange.IsNewInHierarchy, Is.True);
      Assert.That(stateBeforeChange.IsNonPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(3));
      Assert.That(_newNonPersistentDataContainer.State.IsNew, Is.True);
      Assert.That(_newNonPersistentDataContainer.State.IsNewInHierarchy, Is.True);
      Assert.That(_newNonPersistentDataContainer.State.IsNonPersistentDataChanged, Is.True);

      _newNonPersistentDataContainer.CommitPropertyValuesOnNewDataContainer();

      var stateAfterChange = _newNonPersistentDataContainer.State;
      Assert.That(stateAfterChange.IsNew, Is.True);
      Assert.That(stateAfterChange.IsNewInHierarchy, Is.True);
      Assert.That(stateAfterChange.IsNonPersistentDataChanged, Is.False);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(2));
      Assert.That(_newNonPersistentDataContainer.State.IsNew, Is.True);
      Assert.That(_newNonPersistentDataContainer.State.IsNewInHierarchy, Is.True);
    }

    [Test]
    public void CommitPropertyValuesOnNewDataContainer_DiscardedDataContainer_Throws ()
    {
      Assert.That(
          () => _discardedDataContainer.CommitPropertyValuesOnNewDataContainer(),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void CommitPropertyValuesOnNewDataContainer_UnchangedDataContainer_Throws ()
    {
      Assert.That(
          () => _existingDataContainer.CommitPropertyValuesOnNewDataContainer(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Only new data containers can commit their property state as unchanged."));
    }

    [Test]
    public void CommitPropertyValuesOnNewDataContainer_ChangedDataContainer_Throws ()
    {
      _existingDataContainer.SetValue(_orderNumberProperty, 10);

      Assert.That(
          () => _existingDataContainer.CommitPropertyValuesOnNewDataContainer(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Only new data containers can commit their property state as unchanged."));
    }

    [Test]
    public void CommitPropertyValuesOnNewDataContainer_DeletedDataContainer_Throws ()
    {
      Assert.That(
          () => _deletedDataContainer.CommitPropertyValuesOnNewDataContainer(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Only new data containers can commit their property state as unchanged."));
    }

    [Test]
    public void RollbackState_SetsStateToExisting ()
    {
      _deletedDataContainer.RollbackState();

      var state = _deletedDataContainer.State;
      Assert.That(state.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state), Is.EqualTo(1));
      Assert.That(_deletedDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void RollbackState_RaisesStateUpdated ()
    {
      CheckStateNotification(_existingDataContainer, dc => dc.RollbackState(), new DataContainerState.Builder().SetUnchanged().Value);
    }

    [Test]
    public void RollbackState_RollsbackPropertyValues_WithPersistentProperty ()
    {
      _existingDataContainer.SetValue(_orderNumberProperty, 10);

      Assert.That(_existingDataContainer.HasValueChanged(_orderNumberProperty), Is.True);
      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty, ValueAccess.Original), Is.EqualTo(0));
      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty), Is.EqualTo(10));

      _existingDataContainer.RollbackState();

      Assert.That(_existingDataContainer.HasValueChanged(_orderNumberProperty), Is.False);
      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty, ValueAccess.Original), Is.EqualTo(0));
      Assert.That(_existingDataContainer.GetValue(_orderNumberProperty), Is.EqualTo(0));
    }

    [Test]
    public void RollbackState_RollsbackPropertyValues_WithNonPersistentProperty ()
    {
      _dataContainerWithNonPersistentProperty.SetValue(_nonPersistentPropertyOnPersistentDataContainer, 10);

      Assert.That(_dataContainerWithNonPersistentProperty.HasValueChanged(_nonPersistentPropertyOnPersistentDataContainer), Is.True);
      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer, ValueAccess.Original), Is.EqualTo(0));
      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer), Is.EqualTo(10));

      _dataContainerWithNonPersistentProperty.RollbackState();

      Assert.That(_dataContainerWithNonPersistentProperty.HasValueChanged(_nonPersistentPropertyOnPersistentDataContainer), Is.False);
      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer, ValueAccess.Original), Is.EqualTo(0));
      Assert.That(_dataContainerWithNonPersistentProperty.GetValue(_nonPersistentPropertyOnPersistentDataContainer), Is.EqualTo(0));
    }

    [Test]
    public void RollbackState_ResetsChangedFlag_WithPersistentProperty ()
    {
      _existingDataContainer.SetValue(_orderNumberProperty, 10);

      var stateBeforeChange = _existingDataContainer.State;
      Assert.That(stateBeforeChange.IsChanged, Is.True);
      Assert.That(stateBeforeChange.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(2));
      Assert.That(_existingDataContainer.State.IsChanged, Is.True);
      Assert.That(_existingDataContainer.State.IsPersistentDataChanged, Is.True);

      _existingDataContainer.RollbackState();

      var stateAfterChange = _existingDataContainer.State;
      Assert.That(stateAfterChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(1));
      Assert.That(_existingDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void RollbackState_ResetsChangedFlag_WithNonPersistentProperty ()
    {
      _dataContainerWithNonPersistentProperty.SetValue(_nonPersistentPropertyOnPersistentDataContainer, 10);

      var stateBeforeChange = _dataContainerWithNonPersistentProperty.State;
      Assert.That(stateBeforeChange.IsChanged, Is.True);
      Assert.That(stateBeforeChange.IsNonPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(2));
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsChanged, Is.True);
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsNonPersistentDataChanged, Is.True);

      _dataContainerWithNonPersistentProperty.RollbackState();

      var stateAfterChange = _dataContainerWithNonPersistentProperty.State;
      Assert.That(stateAfterChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(1));
      Assert.That(_dataContainerWithNonPersistentProperty.State.IsUnchanged, Is.True);
    }

    [Test]
    public void RollbackState_ResetsMarkedChangedFlag_WithPersistentDataContainer ()
    {
      _existingDataContainer.MarkAsChanged();
      Assert.That(_existingDataContainer.HasBeenMarkedChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(_existingDataContainer.State), Is.EqualTo(2));
      Assert.That(_existingDataContainer.State.IsChanged, Is.True);
      Assert.That(_existingDataContainer.State.IsPersistentDataChanged, Is.True);

      _existingDataContainer.RollbackState();

      Assert.That(_existingDataContainer.HasBeenMarkedChanged, Is.False);
      Assert.That(GetNumberOfSetFlags(_existingDataContainer.State), Is.EqualTo(1));
      Assert.That(_existingDataContainer.State.IsUnchanged, Is.True);
      Assert.That(_existingDataContainer.State.IsPersistentDataChanged, Is.False);
    }

    [Test]
    public void RollbackState_ResetsMarkedChangedFlag_WithNonPersistentDataContainer ()
    {
      _existingNonPersistentDataContainer.MarkAsChanged();
      Assert.That(_existingNonPersistentDataContainer.HasBeenMarkedChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(_existingNonPersistentDataContainer.State), Is.EqualTo(2));
      Assert.That(_existingNonPersistentDataContainer.State.IsChanged, Is.True);
      Assert.That(_existingNonPersistentDataContainer.State.IsNonPersistentDataChanged, Is.True);

      _existingNonPersistentDataContainer.RollbackState();

      Assert.That(_existingNonPersistentDataContainer.HasBeenMarkedChanged, Is.False);
      Assert.That(GetNumberOfSetFlags(_existingNonPersistentDataContainer.State), Is.EqualTo(1));
      Assert.That(_existingNonPersistentDataContainer.State.IsUnchanged, Is.True);
      Assert.That(_existingNonPersistentDataContainer.State.IsNonPersistentDataChanged, Is.False);
    }

    [Test]
    public void RollbackState_DiscardedDataContainer_Throws ()
    {
      Assert.That(
          () => _discardedDataContainer.RollbackState(),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void RollbackState_NewDataContainer_Throws ()
    {
      Assert.That(
          () => _newDataContainer.RollbackState(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("New data containers cannot be rolled back, they have to be discarded."));
    }

    [Test]
    public void Delete_SetsStateToDeleted ()
    {
      _existingDataContainer.Delete();

      var state = _existingDataContainer.State;
      Assert.That(state.IsDeleted, Is.True);
      Assert.That(GetNumberOfSetFlags(state), Is.EqualTo(1));
      Assert.That(_existingDataContainer.State.IsDeleted, Is.True);
    }

     [Test]
    public void Delete_RaisesStateUpdated ()
    {
      CheckStateNotification(_existingDataContainer, dc => dc.Delete(), new DataContainerState.Builder().SetDeleted().Value);
    }

    [Test]
    public void Delete_DiscardedDataContainer_Throws ()
    {
      Assert.That(
          () => _discardedDataContainer.Delete(),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void Delete_NewDataContainer_Throws ()
    {
      Assert.That(
          () => _newDataContainer.Delete(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("New data containers cannot be deleted, they have to be discarded."));
    }

    [Test]
    public void Discard_SetsDiscardedFlag ()
    {
      Assert.That(_newDataContainer.State.IsDiscarded, Is.False);

      _newDataContainer.Discard();

      Assert.That(_newDataContainer.State.IsDiscarded, Is.True);
    }

    [Test]
    public void Discard_RaisesStateUpdated ()
    {
      CheckStateNotification(_newDataContainer, dc => dc.Discard(), new DataContainerState.Builder().SetDiscarded().Value);
    }

    [Test]
    public void Discard_DisassociatesFromEventListener ()
    {
      _newDataContainer.SetEventListener(_eventListenerMock.Object);
      Assert.That(_newDataContainer.EventListener, Is.Not.Null);

      _newDataContainer.Discard();

      Assert.That(_newDataContainer.EventListener, Is.Null);
    }

    [Test]
    public void SetDataFromSubTransaction_SetsValues_WithStorageClassPeristent ()
    {
      var sourceDataContainer = DomainObjectIDs.Order1.GetObject<Order>().InternalDataContainer;
      var newDataContainer = DataContainer.CreateNew(DomainObjectIDs.Order3);
      Assert.That(newDataContainer.GetValue(_orderNumberProperty), Is.Not.EqualTo(1));

      newDataContainer.SetDataFromSubTransaction(sourceDataContainer);

      Assert.That(newDataContainer.GetValue(_orderNumberProperty), Is.EqualTo(1));
    }

    [Test]
    public void SetDataFromSubTransaction_SetsValues_WithNonPersistentProperty ()
    {
      var sourceDataContainer = _dataContainerWithNonPersistentProperty;
      sourceDataContainer.SetValue(_nonPersistentPropertyOnPersistentDataContainer, 42);
      var newDataContainer = DataContainer.CreateNew(new ObjectID(sourceDataContainer.ClassDefinition, Guid.NewGuid()));
      Assert.That(newDataContainer.GetValue(_nonPersistentPropertyOnPersistentDataContainer), Is.Not.EqualTo(42));

      newDataContainer.SetDataFromSubTransaction(sourceDataContainer);

      Assert.That(newDataContainer.GetValue(_nonPersistentPropertyOnPersistentDataContainer), Is.EqualTo(42));
    }

    [Test]
    public void SetDataFromSubTransaction_SetsForeignKeys ()
    {
      var sourceDataContainer = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>().InternalDataContainer;
      var newDataContainer = DataContainer.CreateNew(DomainObjectIDs.OrderTicket2);
      var propertyDefinition = GetPropertyDefinition(typeof(OrderTicket), "Order");
      Assert.That(newDataContainer.GetValue(propertyDefinition), Is.Not.EqualTo(DomainObjectIDs.Order1));

      newDataContainer.SetDataFromSubTransaction(sourceDataContainer);

      Assert.That(newDataContainer.GetValue(propertyDefinition), Is.EqualTo(DomainObjectIDs.Order1));
    }

    [Test]
    public void SetDataFromSubTransaction_SetsChangedFlag_IfChanged_WithPersistentProperty ()
    {
      var sourceDataContainer = DomainObjectIDs.Order1.GetObject<Order>().InternalDataContainer;
      var existingDataContainer = DomainObjectIDs.Order3.GetObject<Order>().InternalDataContainer;
      var stateBeforeChange = existingDataContainer.State;
      Assert.That(stateBeforeChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(1));
      Assert.That(existingDataContainer.State.IsUnchanged, Is.True);

      existingDataContainer.SetDataFromSubTransaction(sourceDataContainer);

      var stateAfterChange = existingDataContainer.State;
      Assert.That(stateAfterChange.IsChanged, Is.True);
      Assert.That(stateAfterChange.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(2));
      Assert.That(existingDataContainer.State.IsChanged, Is.True);
      Assert.That(existingDataContainer.State.IsPersistentDataChanged, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_SetsChangedFlag_IfChanged_WithNonPersistentProperty ()
    {
      var sourceDataContainer = DataContainer.CreateForExisting(
          new ObjectID((ClassDefinition)_nonPersistentPropertyOnPersistentDataContainer.TypeDefinition, Guid.NewGuid()),
          null,
          propertyDefinition => propertyDefinition.DefaultValue);
      sourceDataContainer.SetValue(_nonPersistentPropertyOnPersistentDataContainer, 42);

      var existingDataContainer = _dataContainerWithNonPersistentProperty;

      var stateBeforeChange = existingDataContainer.State;
      Assert.That(stateBeforeChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(1));
      Assert.That(existingDataContainer.State.IsUnchanged, Is.True);

      existingDataContainer.SetDataFromSubTransaction(sourceDataContainer);

      var stateAfterChange = existingDataContainer.State;
      Assert.That(stateAfterChange.IsChanged, Is.True);
      Assert.That(stateAfterChange.IsNonPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(2));
      Assert.That(existingDataContainer.State.IsChanged, Is.True);
      Assert.That(existingDataContainer.State.IsNonPersistentDataChanged, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_ResetsChangedFlag_IfUnchanged_WithPersistentProperty ()
    {
      var sourceDataContainer = DomainObjectIDs.Order1.GetObject<Order>().InternalDataContainer;
      var targetDataContainer = Copy(sourceDataContainer, DomainObjectIDs.Order1);
      targetDataContainer.SetValue(_orderNumberProperty, 10);
      var stateBeforeChange = targetDataContainer.State;
      Assert.That(stateBeforeChange.IsChanged, Is.True);
      Assert.That(stateBeforeChange.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(2));
      Assert.That(targetDataContainer.State.IsChanged, Is.True);
      Assert.That(targetDataContainer.State.IsPersistentDataChanged, Is.True);

      targetDataContainer.SetDataFromSubTransaction(sourceDataContainer);

      var stateAfterChange = targetDataContainer.State;
      Assert.That(stateAfterChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(1));
      Assert.That(targetDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_ResetsChangedFlag_IfUnchanged_WithNonPersistentProperty ()
    {
      var sourceDataContainer = _dataContainerWithNonPersistentProperty;
      sourceDataContainer.SetValue(_nonPersistentPropertyOnPersistentDataContainer, 42);
      var targetDataContainer = DataContainer.CreateForExisting(sourceDataContainer.ID, sourceDataContainer.Timestamp, pd => sourceDataContainer.GetValueWithoutEvents(pd));
      targetDataContainer.SetValue(_nonPersistentPropertyOnPersistentDataContainer, 10);
      var stateBeforeChange = targetDataContainer.State;
      Assert.That(stateBeforeChange.IsChanged, Is.True);
      Assert.That(stateBeforeChange.IsNonPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(2));
      Assert.That(targetDataContainer.State.IsChanged, Is.True);
      Assert.That(targetDataContainer.State.IsNonPersistentDataChanged, Is.True);

      targetDataContainer.SetDataFromSubTransaction(sourceDataContainer);

      var stateAfterChange = targetDataContainer.State;
      Assert.That(stateAfterChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(1));
      Assert.That(targetDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_RaisesStateUpdated_Changed ()
    {
      var sourceDataContainer = _existingDataContainer;
      sourceDataContainer.SetValue(_orderNumberProperty, 12);

      var targetDataContainer = DataContainer.CreateForExisting(DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      var state = targetDataContainer.State;
      Assert.That(state.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state), Is.EqualTo(1));
      Assert.That(targetDataContainer.State.IsUnchanged, Is.True);

      CheckStateNotification(
          targetDataContainer,
          dc => dc.SetDataFromSubTransaction(sourceDataContainer),
          new DataContainerState.Builder().SetChanged().SetPersistentDataChanged().Value);
    }

    [Test]
    public void SetDataFromSubTransaction_RaisesStateUpdated_Unchanged ()
    {
      var sourceDataContainer = DomainObjectIDs.Order1.GetObject<Order>().InternalDataContainer;
      var targetDataContainer = Copy(sourceDataContainer, DomainObjectIDs.Order3);
      targetDataContainer.SetValue(_orderNumberProperty, 10);
      var state = targetDataContainer.State;
      Assert.That(state.IsChanged, Is.True);
      Assert.That(state.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state), Is.EqualTo(2));
      Assert.That(targetDataContainer.State.IsChanged, Is.True);
      Assert.That(targetDataContainer.State.IsPersistentDataChanged, Is.True);

      CheckStateNotification(
          targetDataContainer,
          dc => dc.SetDataFromSubTransaction(sourceDataContainer),
          new DataContainerState.Builder().SetUnchanged().Value);
    }

    [Test]
    public void SetDataFromSubTransaction_RaisesStateUpdated_OtherState ()
    {
      var sourceDataContainer = DomainObjectIDs.Order1.GetObject<Order>().InternalDataContainer;
      var targetDataContainer = Copy(sourceDataContainer, DomainObjectIDs.Order3);
      targetDataContainer.Delete();
      var state = targetDataContainer.State;
      Assert.That(state.IsDeleted, Is.True);
      Assert.That(GetNumberOfSetFlags(state), Is.EqualTo(1));
      Assert.That(targetDataContainer.State.IsDeleted, Is.True);

      CheckStateNotification(
          targetDataContainer,
          dc => dc.SetDataFromSubTransaction(sourceDataContainer),
          new DataContainerState.Builder().SetDeleted().Value);
    }

    [Test]
    public void SetDataFromSubTransaction_DoNotSetMarkAsChangedWhenSourceHasNotBeenMarkedChanged ()
    {
      var sourceDataContainer = DomainObjectIDs.Order1.GetObject<Order>().InternalDataContainer;
      var targetDataContainer = Copy(sourceDataContainer, DomainObjectIDs.Order1);
      Assert.That(sourceDataContainer.HasBeenMarkedChanged, Is.False);
      Assert.That(targetDataContainer.HasBeenMarkedChanged, Is.False);

      targetDataContainer.SetDataFromSubTransaction(sourceDataContainer);

      Assert.That(targetDataContainer.HasBeenMarkedChanged, Is.False);
    }

    [Test]
    public void SetDataFromSubTransaction_SetMarkAsChangedWhenSourceHasBeenMarkedChanged ()
    {
      var sourceDataContainer = DomainObjectIDs.Order1.GetObject<Order>().InternalDataContainer;
      var targetDataContainer = Copy(sourceDataContainer, DomainObjectIDs.Order1);
      sourceDataContainer.MarkAsChanged();
      Assert.That(sourceDataContainer.HasBeenMarkedChanged, Is.True);
      Assert.That(targetDataContainer.HasBeenMarkedChanged, Is.False);

      targetDataContainer.SetDataFromSubTransaction(sourceDataContainer);

      Assert.That(targetDataContainer.HasBeenMarkedChanged, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_InvalidDefinition ()
    {
      var sourceDataContainer = DomainObjectIDs.Order1.GetObject<Order>().InternalDataContainer;
      var targetDataContainer = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket>().InternalDataContainer;
      Assert.That(
          () => targetDataContainer.SetDataFromSubTransaction(sourceDataContainer),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "Cannot set this data container's property values from 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'; the data containers do not "
                  + "have the same class definition.", "source"));
    }

    [Test]
    public void GetIDEvenPossibleWhenDiscarded ()
    {
      Assert.That(_discardedDataContainer.State.IsDiscarded, Is.True);
      Assert.That(_discardedDataContainer.ID, Is.EqualTo(_invalidObjectID));
    }

    [Test]
    public void GetDomainObjectEvenPossibleWhenDiscarded ()
    {
      var domainObject = Order.NewObject();
      var dataContainerWithObject = domainObject.InternalDataContainer;
      dataContainerWithObject.Discard();

      Assert.That(dataContainerWithObject.State.IsDiscarded, Is.True);
      Assert.That(dataContainerWithObject.DomainObject, Is.SameAs(domainObject));
    }

    [Test]
    public void MarkAsChanged ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      DataContainer dataContainer = order.InternalDataContainer;
      var state1 = dataContainer.State;
      Assert.That(state1.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state1), Is.EqualTo(1));
      Assert.That(dataContainer.State.IsUnchanged, Is.True);
      dataContainer.MarkAsChanged();
      var state2 = dataContainer.State;
      Assert.That(state2.IsChanged, Is.True);
      Assert.That(state2.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state2), Is.EqualTo(2));
      Assert.That(dataContainer.State.IsChanged, Is.True);
      Assert.That(dataContainer.State.IsPersistentDataChanged, Is.True);

      TestableClientTransaction.Rollback();
      var state3 = dataContainer.State;
      Assert.That(state3.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state3), Is.EqualTo(1));
      Assert.That(dataContainer.State.IsUnchanged, Is.True);

      dataContainer.MarkAsChanged();
      var state4 = dataContainer.State;
      Assert.That(state4.IsChanged, Is.True);
      Assert.That(state4.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state4), Is.EqualTo(2));
      Assert.That(dataContainer.State.IsChanged, Is.True);
      Assert.That(dataContainer.State.IsPersistentDataChanged, Is.True);

      TestableClientTransaction.Commit();
      var state5 = dataContainer.State;
      Assert.That(state5.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(state5), Is.EqualTo(1));
      Assert.That(dataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void MarkAsChanged_WithoutClientTransaction ()
    {
      var stateBeforeChange = _existingDataContainer.State;
      Assert.That(stateBeforeChange.IsUnchanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateBeforeChange), Is.EqualTo(1));
      Assert.That(_existingDataContainer.State.IsUnchanged, Is.True);

      _existingDataContainer.MarkAsChanged();

      var stateAfterChange = _existingDataContainer.State;
      Assert.That(stateAfterChange.IsChanged, Is.True);
      Assert.That(stateAfterChange.IsPersistentDataChanged, Is.True);
      Assert.That(GetNumberOfSetFlags(stateAfterChange), Is.EqualTo(2));
      Assert.That(_existingDataContainer.State.IsChanged, Is.True);
      Assert.That(_existingDataContainer.State.IsPersistentDataChanged, Is.True);
    }

    [Test]
    public void MarkAsChanged_RaisesStateUpdated ()
    {
      CheckStateNotification(_existingDataContainer, dc => dc.MarkAsChanged(), new DataContainerState.Builder().SetChanged().SetPersistentDataChanged().Value);
    }

    [Test]
    public void MarkAsChangedThrowsWhenNew ()
    {
      Order order = Order.NewObject();
      DataContainer dataContainer = order.InternalDataContainer;
      Assert.That(
          () => dataContainer.MarkAsChanged(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Only existing DataContainers can be marked as changed."));
    }

    [Test]
    public void MarkAsChangedThrowsWhenDeleted ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order>();
      order.Delete();
      DataContainer dataContainer = order.InternalDataContainer;
      Assert.That(
          () => dataContainer.MarkAsChanged(),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "Only existing DataContainers can be marked as changed."));
    }

    [Test]
    public void SetNewInHierarchy_ForNewDataContainer_ThrowsInvalidOperationException ()
    {
      Assert.That(
          () => _newDataContainer.SetNewInHierarchy(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Only existing DataContainers can be marked as new-in-hierarchy."));
    }

    [Test]
    public void SetNewInHierarchy_ForExistingDataContainer_IsNewInHierarchyIsSet ()
    {
      Assert.That(_existingDataContainer.State.IsNewInHierarchy, Is.False);

      _existingDataContainer.SetNewInHierarchy();

      Assert.That(_existingDataContainer.State.IsNewInHierarchy, Is.True);
      Assert.That(_existingDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void SetNewInHierarchy_ForDeletedDataContainer_ThrowsInvalidOperationException ()
    {
      Assert.That(
          () => _deletedDataContainer.SetNewInHierarchy(),
          Throws.InvalidOperationException
              .With.Message.EqualTo("Only existing DataContainers can be marked as new-in-hierarchy."));
    }

    [Test]
    public void SetNewInHierarchy_ForDiscardedDataContainer_ThrowsObjectInvalidException ()
    {
      Assert.That(
          () => _discardedDataContainer.SetNewInHierarchy(),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void ClearNewInHierarchy_ForNewDataContainer_ThrowsInvalidOperationException ()
    {
      Assert.That(_newDataContainer.State.IsNewInHierarchy, Is.True);
      Assert.That(_newDataContainer.State.IsNew, Is.True);

      _newDataContainer.ClearNewInHierarchy();

      Assert.That(_newDataContainer.State.IsNewInHierarchy, Is.False);
      Assert.That(_newDataContainer.State.IsNew, Is.True);
    }

    [Test]
    public void ClearNewInHierarchy_ForExistingDataContainer_IsNewInHierarchyIsNotSet ()
    {
      _existingDataContainer.SetNewInHierarchy();
      Assert.That(_existingDataContainer.State.IsNewInHierarchy, Is.True);
      Assert.That(_existingDataContainer.State.IsUnchanged, Is.True);

      _existingDataContainer.ClearNewInHierarchy();

      Assert.That(_existingDataContainer.State.IsNewInHierarchy, Is.False);
      Assert.That(_existingDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void ClearNewInHierarchy_ForDeletedDataContainer_IsNewInHierarchyIsNotSet ()
    {
      _existingDataContainer.SetNewInHierarchy();
      _existingDataContainer.Delete();
      Assert.That(_existingDataContainer.State.IsNewInHierarchy, Is.True);
      Assert.That(_existingDataContainer.State.IsDeleted, Is.True);

      _existingDataContainer.ClearNewInHierarchy();

      Assert.That(_existingDataContainer.State.IsNewInHierarchy, Is.False);
      Assert.That(_existingDataContainer.State.IsDeleted, Is.True);
    }

    [Test]
    public void ClearNewInHierarchy_ForDiscardedDataContainer_ThrowsObjectInvalidException ()
    {
      Assert.That(
          () => _discardedDataContainer.ClearNewInHierarchy(),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void ErrorWhenNoClientTransaction ()
    {
      DataContainer dc = DataContainer.CreateNew(DomainObjectIDs.Order1);
      Assert.That(
          () => dc.ClientTransaction,
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "DataContainer has not been registered with a transaction."));
    }

    [Test]
    public void DomainObject_NoneSet ()
    {
      var dc = DataContainer.CreateNew(DomainObjectIDs.Order1);
      Assert.That(
          () => dc.DomainObject,
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "This DataContainer has not been associated with a DomainObject yet."));
    }

    [Test]
    public void SetDomainObject ()
    {
      var domainObject = DomainObjectIDs.Order1.GetObject<Order>();

      var dc = DataContainer.CreateNew(DomainObjectIDs.Order1);
      dc.SetDomainObject(domainObject);

      Assert.That(dc.DomainObject, Is.SameAs(domainObject));
    }

    [Test]
    public void SetDomainObject_InvalidID ()
    {
      var domainObject = DomainObjectIDs.Order3.GetObject<Order>();

      var dc = DataContainer.CreateNew(DomainObjectIDs.Order1);
      Assert.That(
          () => dc.SetDomainObject(domainObject),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The given DomainObject has another ID than this DataContainer.",
                  "domainObject"));
    }

    [Test]
    public void SetDomainObject_DomainObjectAlreadySet ()
    {
      var domainObject1 = DomainObjectIDs.Order1.GetObject<Order>();
      var domainObject2 = DomainObjectMother.GetObjectInOtherTransaction<Order>(DomainObjectIDs.Order1);

      var dc = DataContainer.CreateNew(DomainObjectIDs.Order1);
      dc.SetDomainObject(domainObject1);
      Assert.That(
          () => dc.SetDomainObject(domainObject2),
          Throws.InvalidOperationException
              .With.Message.EqualTo(
                  "This DataContainer has already been associated with a DomainObject."));
    }

    [Test]
    public void SetDomainObject_SameDomainObjectAlreadySet ()
    {
      var domainObject = DomainObjectIDs.Order1.GetObject<Order>();

      var dc = DataContainer.CreateNew(DomainObjectIDs.Order1);
      dc.SetDomainObject(domainObject);
      dc.SetDomainObject(domainObject);

      Assert.That(dc.DomainObject, Is.SameAs(domainObject));
    }

    [Test]
    public void HasDomainObject_False ()
    {
      var dc = DataContainer.CreateNew(DomainObjectIDs.Order1);
      Assert.That(dc.HasDomainObject, Is.False);
    }

    [Test]
    public void HasDomainObject_True ()
    {
      var domainObject = DomainObjectIDs.Order1.GetObject<Order>();

      var dc = DataContainer.CreateNew(DomainObjectIDs.Order1);
      dc.SetDomainObject(domainObject);

      Assert.That(dc.HasDomainObject, Is.True);
    }

    [Test]
    public void SetClientTransaction ()
    {
      var dc = DataContainer.CreateNew(DomainObjectIDs.Order1);
      DataContainerTestHelper.SetClientTransaction(dc, TestableClientTransaction);

      Assert.That(dc.ClientTransaction, Is.SameAs(TestableClientTransaction));
    }

    [Test]
    public void SetClientTransaction_Twice ()
    {
      var dc = DataContainer.CreateNew(DomainObjectIDs.Order1);
      DataContainerTestHelper.SetClientTransaction(dc, TestableClientTransaction);
      Assert.That(
          () => DataContainerTestHelper.SetClientTransaction(dc, TestableClientTransaction),
          Throws.InvalidOperationException
              .With.Message.EqualTo("This DataContainer has already been registered with a ClientTransaction."));
    }

    private void CheckStateNotification (DataContainer dataContainer, Action<DataContainer> action, DataContainerState expectedState)
    {
      dataContainer.SetEventListener(_eventListenerMock.Object);

      action(dataContainer);

      _eventListenerMock.Verify(mock => mock.StateUpdated(dataContainer, expectedState), Times.AtLeastOnce());
    }

    private int GetNumberOfSetFlags (DataContainerState dataContainerState)
    {
      int count = 0;
      if (dataContainerState.IsNew)
        count++;
      if (dataContainerState.IsChanged)
        count++;
      if (dataContainerState.IsDeleted)
        count++;
      if (dataContainerState.IsDiscarded)
        count++;
      if (dataContainerState.IsUnchanged)
        count++;
      if (dataContainerState.IsPersistentDataChanged)
        count++;
      if (dataContainerState.IsNonPersistentDataChanged)
        count++;
      if (dataContainerState.IsNewInHierarchy)
        count++;

      return count;
    }

    private DataContainer Copy (DataContainer source, ObjectID newID)
    {
      Assert.That(source.State.IsUnchanged, Is.True);
      return DataContainer.CreateForExisting(
          newID,
          source.Timestamp,
          pd => source.GetValueWithoutEvents(pd, ValueAccess.Current));
    }
  }
}
