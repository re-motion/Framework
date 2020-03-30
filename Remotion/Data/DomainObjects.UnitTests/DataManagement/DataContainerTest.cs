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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.TestDomain;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class DataContainerTest : ClientTransactionBaseTest
  {
    private DataContainer _newDataContainer;
    private DataContainer _existingDataContainer;
    private DataContainer _deletedDataContainer;
    private DataContainer _discardedDataContainer;
    private ObjectID _invalidObjectID;
    private IDataContainerEventListener _eventListenerMock;

    private PropertyDefinition _orderNumberProperty;
    private PropertyDefinition _deliveryDateProperty;
    private PropertyDefinition _nonOrderProperty;

    public override void SetUp ()
    {
      base.SetUp();

      var idValue = Guid.NewGuid();

      _newDataContainer = DataContainer.CreateNew (new ObjectID(typeof (Order), idValue));
      _existingDataContainer = DataContainer.CreateForExisting (
          new ObjectID(typeof (Order), idValue),
          null,
          propertyDefinition => propertyDefinition.DefaultValue);

      _deletedDataContainer = DataContainer.CreateForExisting (
          new ObjectID(typeof (Order), idValue),
          null,
          propertyDefinition => propertyDefinition.DefaultValue);
      _deletedDataContainer.Delete();

      _invalidObjectID = new ObjectID(typeof (Order), idValue);
      _discardedDataContainer = DataContainer.CreateNew (_invalidObjectID);
      _discardedDataContainer.Discard();

      _eventListenerMock = MockRepository.GenerateMock<IDataContainerEventListener>();

      _orderNumberProperty = GetPropertyDefinition (typeof (Order), "OrderNumber");
      _deliveryDateProperty = GetPropertyDefinition (typeof (Order), "DeliveryDate");
      _nonOrderProperty = GetPropertyDefinition (typeof (OrderItem), "Order");
    }

    [Test]
    public void CreateNew_IncludesStorageClassPersistentProperties ()
    {
      DataContainer dc = DataContainer.CreateNew (new ObjectID(typeof (ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()));
      var propertyDefinition = GetPropertyDefinition (typeof (ClassWithPropertiesHavingStorageClassAttribute), "Persistent");
      Assert.That (dc.GetValue (propertyDefinition), Is.EqualTo (0));
      Assert.That (dc.GetValue (propertyDefinition, ValueAccess.Original), Is.EqualTo (0));
    }

    [Test]
    public void CreateNew_IncludesStorageClassTransactionProperties ()
    {
      DataContainer dc = DataContainer.CreateNew (new ObjectID(typeof (ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()));
      var propertyDefinition = GetPropertyDefinition (typeof (ClassWithPropertiesHavingStorageClassAttribute), "Transaction");
      Assert.That (dc.GetValue (propertyDefinition), Is.EqualTo (0));
      Assert.That (dc.GetValue (propertyDefinition, ValueAccess.Original), Is.EqualTo (0));
    }

    [Test]
    public void CreateForExisting_IncludesStorageClassPersistentProperties_WithLookupValue ()
    {
      DataContainer dc = DataContainer.CreateForExisting (
          new ObjectID(typeof (ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()),
          1,
          delegate { return 2; });
      var propertyDefinition = GetPropertyDefinition (typeof (ClassWithPropertiesHavingStorageClassAttribute), "Persistent");
      Assert.That (dc.GetValue (propertyDefinition), Is.EqualTo (2));
      Assert.That (dc.GetValue (propertyDefinition, ValueAccess.Original), Is.EqualTo (2));
    }

    [Test]
    public void CreateForExisting_IncludesStorageClassTransactionProperties_WithLookupValue ()
    {
      DataContainer dc = DataContainer.CreateForExisting (
          new ObjectID(typeof (ClassWithPropertiesHavingStorageClassAttribute), Guid.NewGuid()),
          1,
          delegate { return 2; });
      var propertyDefinition = GetPropertyDefinition (typeof (ClassWithPropertiesHavingStorageClassAttribute), "Transaction");
      Assert.That (dc.GetValue (propertyDefinition), Is.EqualTo (2));
      Assert.That (dc.GetValue (propertyDefinition, ValueAccess.Original), Is.EqualTo (2));
    }

    [Test]
    public void SetEventListener ()
    {
      Assert.That (_existingDataContainer.EventListener, Is.Null);
      
      _existingDataContainer.SetEventListener (_eventListenerMock);

      Assert.That (_existingDataContainer.EventListener, Is.Not.Null.And.SameAs (_eventListenerMock));
    }

    [Test]
    public void SetEventListener_Twice ()
    {
      _existingDataContainer.SetEventListener (_eventListenerMock);
      Assert.That (
          () => _existingDataContainer.SetEventListener (_eventListenerMock), 
          Throws.InvalidOperationException.With.Message.EqualTo ("Only one event listener can be registered for a DataContainer."));
    }

    [Test]
    public void GetValue_SetValue ()
    {
      var valueBeforeChange = _existingDataContainer.GetValue (_orderNumberProperty);
      Assert.That (valueBeforeChange, Is.EqualTo (0));
      var originalValueBeforeChange = _existingDataContainer.GetValue (_orderNumberProperty, ValueAccess.Original);
      Assert.That (originalValueBeforeChange, Is.EqualTo (0));

      _existingDataContainer.SetValue (_orderNumberProperty, 17);

      var valueAfterChange = _existingDataContainer.GetValue (_orderNumberProperty);
      Assert.That (valueAfterChange, Is.EqualTo (17));
      var originalValueAfterChange = _existingDataContainer.GetValue (_orderNumberProperty, ValueAccess.Original);
      Assert.That (originalValueAfterChange, Is.EqualTo (0));
    }

    [Test]
    public void GetValue_RaisesEvent ()
    {
      _existingDataContainer.SetEventListener (_eventListenerMock);

      _existingDataContainer.GetValue (_orderNumberProperty, ValueAccess.Original);

      _eventListenerMock.AssertWasCalled (mock => mock.PropertyValueReading (_existingDataContainer, _orderNumberProperty, ValueAccess.Original));
      _eventListenerMock.AssertWasCalled (mock => mock.PropertyValueRead (_existingDataContainer, _orderNumberProperty, 0, ValueAccess.Original));
    }

    [Test]
    public void GetValue_RaisesEvent_Cancellation ()
    {
      _existingDataContainer.SetEventListener (_eventListenerMock);

      var exception = new Exception();
      _eventListenerMock
          .Expect (mock => mock.PropertyValueReading (_existingDataContainer, _orderNumberProperty, ValueAccess.Original))
          .Throw (exception);
      
      Assert.That (() => _existingDataContainer.GetValue (_orderNumberProperty, ValueAccess.Original), Throws.Exception.SameAs (exception));
      _eventListenerMock.AssertWasNotCalled (
          mock => mock.PropertyValueRead (
              Arg<DataContainer>.Is.Anything, Arg<PropertyDefinition>.Is.Anything, Arg<object>.Is.Anything, Arg<ValueAccess>.Is.Anything));
    }

    [Test]
    public void GetValue_Discarded ()
    {
      _existingDataContainer.Discard();

      Assert.That (() => _existingDataContainer.GetValue (_orderNumberProperty), Throws.TypeOf<ObjectInvalidException>());
    }

    [Test]
    public void GetValue_InvalidProperty ()
    {
      Assert.That (
          () => _existingDataContainer.GetValue (_nonOrderProperty), 
          Throws.ArgumentException.With.Message.Contains ("Parameter name: propertyDefinition"));
    }

    [Test]
    public void SetValue ()
    {
      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty), Is.Not.EqualTo (17));
      Assert.That (_existingDataContainer.HasValueBeenTouched (_orderNumberProperty), Is.False);
      Assert.That (_existingDataContainer.HasValueChanged (_orderNumberProperty), Is.False);

      _existingDataContainer.SetValue (_orderNumberProperty, 17);

      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty), Is.EqualTo (17));
      Assert.That (_existingDataContainer.HasValueBeenTouched (_orderNumberProperty), Is.True);
      Assert.That (_existingDataContainer.HasValueChanged (_orderNumberProperty), Is.True);
    }

    [Test]
    public void SetValue_RaisesEvent ()
    {
      _existingDataContainer.SetEventListener (_eventListenerMock);

      _eventListenerMock
          .Expect (mock => mock.PropertyValueChanging (_existingDataContainer, _orderNumberProperty, 0, 17))
          .WhenCalled (mi => Assert.That (_existingDataContainer.GetValueWithoutEvents (_orderNumberProperty, ValueAccess.Current), Is.EqualTo (0)));
      _eventListenerMock
          .Expect (mock => mock.PropertyValueChanged (_existingDataContainer, _orderNumberProperty, 0, 17))
          .WhenCalled (mi => Assert.That (_existingDataContainer.GetValueWithoutEvents (_orderNumberProperty, ValueAccess.Current), Is.EqualTo (17)));

      _existingDataContainer.SetValue (_orderNumberProperty, 17);

      _eventListenerMock.VerifyAllExpectations();
      Assert.That (_existingDataContainer.GetValueWithoutEvents (_orderNumberProperty, ValueAccess.Current), Is.EqualTo (17));
    }

    [Test]
    public void SetValue_RaisesEvent_Cancellation ()
    {
      _existingDataContainer.SetEventListener (_eventListenerMock);

      var exception = new Exception();
      _eventListenerMock
          .Expect (mock => mock.PropertyValueChanging (_existingDataContainer, _orderNumberProperty, 0, 17))
          .Throw (exception);

      Assert.That (() => _existingDataContainer.SetValue (_orderNumberProperty, 17), Throws.Exception.SameAs (exception));

      _eventListenerMock.AssertWasNotCalled (
          mock => mock.PropertyValueChanged (
              Arg<DataContainer>.Is.Anything, Arg<PropertyDefinition>.Is.Anything, Arg<object>.Is.Anything, Arg<object>.Is.Anything));

      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty), Is.EqualTo (0));
    }

    [Test]
    public void SetValue_SameValue_NoEventButStillTouched ()
    {
      var currentValue = _existingDataContainer.GetValue (_orderNumberProperty);
      _existingDataContainer.SetEventListener (_eventListenerMock);

      _existingDataContainer.SetValue (_orderNumberProperty, currentValue);

      _eventListenerMock.AssertWasNotCalled (
          mock => mock.PropertyValueChanging (
              Arg<DataContainer>.Is.Anything,
              Arg<PropertyDefinition>.Is.Anything,
              Arg<object>.Is.Anything,
              Arg<object>.Is.Anything));
      _eventListenerMock.AssertWasNotCalled (
          mock => mock.PropertyValueChanged (
              Arg<DataContainer>.Is.Anything,
              Arg<PropertyDefinition>.Is.Anything,
              Arg<object>.Is.Anything,
              Arg<object>.Is.Anything));

      Assert.That (_existingDataContainer.HasValueBeenTouched (_orderNumberProperty), Is.True);
      Assert.That (_existingDataContainer.HasValueChanged (_orderNumberProperty), Is.False);
    }
    
    [Test]
    public void SetValue_Discarded ()
    {
      _existingDataContainer.Discard ();

      Assert.That (() => _existingDataContainer.SetValue (_orderNumberProperty, 17), Throws.TypeOf<ObjectInvalidException> ());
    }

    [Test]
    public void SetValue_Deleted ()
    {
      _existingDataContainer.Delete ();

      Assert.That (() => _existingDataContainer.SetValue (_orderNumberProperty, 17), Throws.TypeOf<ObjectDeletedException> ());
    }

    [Test]
    public void SetValue_InvalidProperty ()
    {
      Assert.That (
          () => _existingDataContainer.SetValue (_nonOrderProperty, 17), 
          Throws.ArgumentException.With.Message.Contains ("Parameter name: propertyDefinition"));
    }

    [Test]
    public void GetValueWithoutEvents ()
    {
      var valueBeforeChange = _existingDataContainer.GetValueWithoutEvents (_orderNumberProperty, ValueAccess.Current);
      Assert.That (valueBeforeChange, Is.EqualTo (0));
      var originalValueBeforeChange = _existingDataContainer.GetValueWithoutEvents (_orderNumberProperty, ValueAccess.Original);
      Assert.That (originalValueBeforeChange, Is.EqualTo (0));

      _existingDataContainer.SetValue (_orderNumberProperty, 17);

      var valueAfterChange = _existingDataContainer.GetValueWithoutEvents (_orderNumberProperty, ValueAccess.Current);
      Assert.That (valueAfterChange, Is.EqualTo (17));
      var originalValueAfterChange = _existingDataContainer.GetValueWithoutEvents (_orderNumberProperty, ValueAccess.Original);
      Assert.That (originalValueAfterChange, Is.EqualTo (0));
    }

    [Test]
    public void GetValueWithoutEvents_RaisesNoEvent ()
    {
      _existingDataContainer.SetEventListener (_eventListenerMock);

      _existingDataContainer.GetValueWithoutEvents (_orderNumberProperty, ValueAccess.Original);

      _eventListenerMock.AssertWasNotCalled (
          mock => mock.PropertyValueReading (Arg<DataContainer>.Is.Anything, Arg<PropertyDefinition>.Is.Anything, Arg<ValueAccess>.Is.Anything));
      _eventListenerMock.AssertWasNotCalled (
          mock => mock.PropertyValueRead (
              Arg<DataContainer>.Is.Anything, Arg<PropertyDefinition>.Is.Anything, Arg<object>.Is.Anything, Arg<ValueAccess>.Is.Anything));
    }

    [Test]
    public void GetValueWithoutEvents_Discarded ()
    {
      _existingDataContainer.Discard ();

      Assert.That (
          () => _existingDataContainer.GetValueWithoutEvents (_orderNumberProperty, ValueAccess.Current), Throws.TypeOf<ObjectInvalidException>());
    }

    [Test]
    public void GetValueWithoutEvents_InvalidProperty ()
    {
      Assert.That (
          () => _existingDataContainer.GetValueWithoutEvents (_nonOrderProperty, ValueAccess.Current), 
          Throws.ArgumentException.With.Message.Contains ("Parameter name: propertyDefinition"));
    }

    [Test]
    public void HasValueBeenTouched_Discarded ()
    {
      _existingDataContainer.Discard ();

      Assert.That (() => _existingDataContainer.HasValueBeenTouched (_orderNumberProperty), Throws.TypeOf<ObjectInvalidException> ());
    }

    [Test]
    public void HasValueBeenTouched_InvalidProperty ()
    {
      Assert.That (
          () => _existingDataContainer.HasValueBeenTouched (_nonOrderProperty), 
          Throws.ArgumentException.With.Message.Contains ("Parameter name: propertyDefinition"));
    }

    [Test]
    public void TouchValue ()
    {
      Assert.That (_existingDataContainer.HasValueBeenTouched (_orderNumberProperty), Is.False);

      _existingDataContainer.TouchValue (_orderNumberProperty);

      Assert.That (_existingDataContainer.HasValueBeenTouched (_orderNumberProperty), Is.True);
    }

    [Test]
    public void TouchValue_Discarded ()
    {
      _existingDataContainer.Discard ();

      Assert.That (() => _existingDataContainer.TouchValue (_orderNumberProperty), Throws.TypeOf<ObjectInvalidException> ());
    }

    [Test]
    public void TouchValue_InvalidProperty ()
    {
      Assert.That (
          () => _existingDataContainer.TouchValue (_nonOrderProperty), 
          Throws.ArgumentException.With.Message.Contains ("Parameter name: propertyDefinition"));
    }

    [Test]
    public void HasValueChanged ()
    {
      Assert.That (_existingDataContainer.HasValueChanged (_orderNumberProperty), Is.False);

      _existingDataContainer.SetValue (_orderNumberProperty, 1);

      Assert.That (_existingDataContainer.HasValueChanged (_orderNumberProperty), Is.True);

      _existingDataContainer.SetValue (_orderNumberProperty, 0);

      Assert.That (_existingDataContainer.HasValueChanged (_orderNumberProperty), Is.False);
    }

    [Test]
    public void HasValueChanged_Discarded ()
    {
      _existingDataContainer.Discard ();

      Assert.That (() => _existingDataContainer.HasValueChanged (_orderNumberProperty), Throws.TypeOf<ObjectInvalidException> ());
    }

    [Test]
    public void HasValueChanged_InvalidProperty ()
    {
      Assert.That (
          () => _existingDataContainer.HasValueChanged (_nonOrderProperty), 
          Throws.ArgumentException.With.Message.Contains ("Parameter name: propertyDefinition"));
    }

    [Test]
    public void PropertyChange_StateUpdate ()
    {
      var state1 = _existingDataContainer.State;
      Assert.That (state1.IsUnchanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state1), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsUnchanged, Is.True);

      _existingDataContainer.SetValue (_orderNumberProperty, 5);

      var state2 = _existingDataContainer.State;
      Assert.That (state2.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state2), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsChanged, Is.True);

      _existingDataContainer.SetValue (_orderNumberProperty, 0);

      var state3 = _existingDataContainer.State;
      Assert.That (state3.IsUnchanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state3), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void PropertyChange_StateUpdate_WithNewDataContainer ()
    {
      var state1 = _newDataContainer.State;
      Assert.That (state1.IsNew, Is.True);
      Assert.That (_newDataContainer.State.IsNew, Is.True);
      Assert.That (GetNumberOfSetFlags (state1), Is.EqualTo (1));
      Assert.That (_newDataContainer.GetValue (_orderNumberProperty), Is.Not.EqualTo (17));

      _newDataContainer.SetValue (_orderNumberProperty, 17);

      var state2 = _newDataContainer.State;
      Assert.That (state2.IsNew, Is.True);
      Assert.That (GetNumberOfSetFlags (state2), Is.EqualTo (1));
      Assert.That (_newDataContainer.State.IsNew, Is.True);

      _newDataContainer.SetValue (_orderNumberProperty, 0);

      var state3 = _newDataContainer.State;
      Assert.That (state3.IsNew, Is.True);
      Assert.That (GetNumberOfSetFlags (state3), Is.EqualTo (1));
      Assert.That (_newDataContainer.State.IsNew, Is.True);
    }

    [Test]
    public void PropertyChange_RaisesStateUpdated ()
    {
      CheckStateNotification (_existingDataContainer, dc => dc.SetValue (_orderNumberProperty, 5), new DataContainerState.Builder().SetChanged().Value);
      CheckStateNotification (_newDataContainer, dc => dc.SetValue (_orderNumberProperty, 5), new DataContainerState.Builder().SetNew().Value);
    }

    [Test]
    public void PropertyChange_ChangeBack_RaisesStateUpdated ()
    {
      _existingDataContainer.SetValue (_orderNumberProperty, 5);
      CheckStateNotification (_existingDataContainer, dc => dc.SetValue (_orderNumberProperty, 0), new DataContainerState.Builder().SetUnchanged().Value);
    }

    [Test]
    public void CommitValue_CommitsOnlyGivenPropertyValue_AndUpdatesState ()
    {
      _existingDataContainer.SetValue (_orderNumberProperty, 10);
      _existingDataContainer.SetValue (_deliveryDateProperty, new DateTime (2012, 05, 13));

      Assert.That (_existingDataContainer.HasValueChanged (_orderNumberProperty), Is.True);
      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty, ValueAccess.Original), Is.EqualTo (0));
      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty), Is.EqualTo (10));

      Assert.That (_existingDataContainer.HasValueChanged (_deliveryDateProperty), Is.True);
      Assert.That (_existingDataContainer.GetValue (_deliveryDateProperty, ValueAccess.Original), Is.EqualTo (new DateTime()));
      Assert.That (_existingDataContainer.GetValue (_deliveryDateProperty), Is.EqualTo (new DateTime (2012, 05, 13)));

      var state1 = _existingDataContainer.State;
      Assert.That (state1.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state1), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsChanged, Is.True);

      _existingDataContainer.CommitValue (_orderNumberProperty);

      Assert.That (_existingDataContainer.HasValueChanged (_orderNumberProperty), Is.False);
      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty, ValueAccess.Original), Is.EqualTo (10));
      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty), Is.EqualTo (10));

      Assert.That (_existingDataContainer.HasValueChanged (_deliveryDateProperty), Is.True);
      Assert.That (_existingDataContainer.GetValue (_deliveryDateProperty, ValueAccess.Original), Is.EqualTo (new DateTime ()));
      Assert.That (_existingDataContainer.GetValue (_deliveryDateProperty), Is.EqualTo (new DateTime (2012, 05, 13)));

      var state2 = _existingDataContainer.State;
      Assert.That (state2.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state2), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsChanged, Is.True);

      _existingDataContainer.CommitValue (_deliveryDateProperty);

      var state3 = _existingDataContainer.State;
      Assert.That (state3.IsUnchanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state3), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void CommitValue_Discarded ()
    {
      _existingDataContainer.Discard ();

      Assert.That (() => _existingDataContainer.CommitValue (_orderNumberProperty), Throws.TypeOf<ObjectInvalidException> ());
    }

    [Test]
    public void CommitValue_InvalidProperty ()
    {
      Assert.That (
          () => _existingDataContainer.CommitValue (_nonOrderProperty),
          Throws.ArgumentException.With.Message.Contains ("Parameter name: propertyDefinition"));
    }

    [Test]
    public void RollbackValue_RollsBackOnlyGivenPropertyValue_AndUpdatesState ()
    {
      _existingDataContainer.SetValue (_orderNumberProperty, 10);
      _existingDataContainer.SetValue (_deliveryDateProperty, new DateTime (2012, 05, 13));

      Assert.That (_existingDataContainer.HasValueChanged (_orderNumberProperty), Is.True);
      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty, ValueAccess.Original), Is.EqualTo (0));
      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty), Is.EqualTo (10));

      Assert.That (_existingDataContainer.HasValueChanged (_deliveryDateProperty), Is.True);
      Assert.That (_existingDataContainer.GetValue (_deliveryDateProperty, ValueAccess.Original), Is.EqualTo (new DateTime ()));
      Assert.That (_existingDataContainer.GetValue (_deliveryDateProperty), Is.EqualTo (new DateTime (2012, 05, 13)));

      var state1 = _existingDataContainer.State;
      Assert.That (state1.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state1), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsChanged, Is.True);

      _existingDataContainer.RollbackValue (_orderNumberProperty);

      Assert.That (_existingDataContainer.HasValueChanged (_orderNumberProperty), Is.False);
      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty, ValueAccess.Original), Is.EqualTo (0));
      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty), Is.EqualTo (0));

      Assert.That (_existingDataContainer.HasValueChanged (_deliveryDateProperty), Is.True);
      Assert.That (_existingDataContainer.GetValue (_deliveryDateProperty, ValueAccess.Original), Is.EqualTo (new DateTime ()));
      Assert.That (_existingDataContainer.GetValue (_deliveryDateProperty), Is.EqualTo (new DateTime (2012, 05, 13)));

      var state2 = _existingDataContainer.State;
      Assert.That (state2.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state2), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsChanged, Is.True);

      _existingDataContainer.RollbackValue (_deliveryDateProperty);

      var state3 = _existingDataContainer.State;
      Assert.That (state3.IsUnchanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state3), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void RollbackValue_Discarded ()
    {
      _existingDataContainer.Discard ();

      Assert.That (() => _existingDataContainer.RollbackValue (_orderNumberProperty), Throws.TypeOf<ObjectInvalidException> ());
    }

    [Test]
    public void RollbackValue_InvalidProperty ()
    {
      Assert.That (
          () => _existingDataContainer.RollbackValue (_nonOrderProperty),
          Throws.ArgumentException.With.Message.Contains ("Parameter name: propertyDefinition"));
    }

    [Test]
    public void SetValueDataFromSubTransaction_SetsOnlyGivenPropertyValue_AndUpdatesState ()
    {
      var sourceDataContainer = DataContainerObjectMother.CreateExisting (_existingDataContainer.ID);
      sourceDataContainer.SetValue (_orderNumberProperty, 17);
      sourceDataContainer.SetValue (_deliveryDateProperty, new DateTime (2012, 05, 13));

      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty), Is.EqualTo (0));
      Assert.That (_existingDataContainer.GetValue (_deliveryDateProperty), Is.EqualTo (new DateTime()));
      var stateBeforeChange = _existingDataContainer.State;
      Assert.That (stateBeforeChange.IsUnchanged, Is.True);
      Assert.That (GetNumberOfSetFlags (stateBeforeChange), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsUnchanged, Is.True);

      _existingDataContainer.SetValueDataFromSubTransaction (_orderNumberProperty, sourceDataContainer);

      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty), Is.EqualTo (17));
      Assert.That (_existingDataContainer.GetValue (_deliveryDateProperty), Is.EqualTo (new DateTime ()));
      var stateAfterChange = _existingDataContainer.State;
      Assert.That (stateAfterChange.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (stateAfterChange), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsChanged, Is.True);
    }

    [Test]
    public void SetValueDataFromSubTransaction_Discarded ()
    {
      var sourceDataContainer = DataContainerObjectMother.Create (_existingDataContainer.ID);
      _existingDataContainer.Discard ();
      Assert.That (
          () => _existingDataContainer.SetValueDataFromSubTransaction (_orderNumberProperty, sourceDataContainer), 
          Throws.TypeOf<ObjectInvalidException> ());
    }

    [Test]
    public void SetValueDataFromSubTransaction_DiscardedSource ()
    {
      var sourceDataContainer = DataContainerObjectMother.Create (_existingDataContainer.ID);
      sourceDataContainer.Discard ();
      Assert.That (
          () => _existingDataContainer.SetValueDataFromSubTransaction (_orderNumberProperty, sourceDataContainer),
          Throws.TypeOf<ObjectInvalidException> ());
    }

    [Test]
    public void SetValueDataFromSubTransaction_InvalidProperty ()
    {
      var sourceDataContainer = DataContainerObjectMother.Create (_existingDataContainer.ID);
      Assert.That (
          () => _existingDataContainer.SetValueDataFromSubTransaction (_nonOrderProperty, sourceDataContainer),
          Throws.ArgumentException.With.Message.Contains ("Parameter name: propertyDefinition"));
    }

    [Test]
    public void SetValueDataFromSubTransaction_InvalidSource ()
    {
      var sourceDataContainer = DataContainerObjectMother.Create (DomainObjectIDs.Customer1);
      Assert.That (
          () => _existingDataContainer.SetValueDataFromSubTransaction (_orderNumberProperty, sourceDataContainer),
          Throws.ArgumentException.With.Message.EqualTo (
              "Cannot set this data container's property values from 'Customer|55b52e75-514b-4e82-a91b-8f0bb59b80ad|System.Guid'; the data " 
              + "containers do not have the same class definition.\r\nParameter name: source"));
    }

    [Test]
    public void SetTimestamp ()
    {
      _existingDataContainer.SetTimestamp (10);

      Assert.That (_existingDataContainer.Timestamp, Is.EqualTo (10));
    }

    [Test]
    public void Clone_SetsID ()
    {
      var original = _existingDataContainer;
      Assert.That (original.ID, Is.Not.EqualTo (DomainObjectIDs.Order3));

      var clone = original.Clone (DomainObjectIDs.Order3);
      Assert.That (clone.ID, Is.EqualTo (DomainObjectIDs.Order3));
    }

    [Test]
    public void Clone_CopiesState ()
    {
      var originalNew = _newDataContainer;
      Assert.That (originalNew.State.IsNew, Is.True);
      Assert.That (originalNew.State.IsNew, Is.True);

      var originalExisting = _existingDataContainer;
      Assert.That (originalExisting.State.IsUnchanged, Is.True);
      Assert.That (originalExisting.State.IsUnchanged, Is.True);

      var clonedNew = originalNew.Clone (DomainObjectIDs.Order4);
      Assert.That (clonedNew.State.IsNew, Is.True);
      Assert.That (clonedNew.State.IsNew, Is.True);

      var clonedExisting = originalExisting.Clone (DomainObjectIDs.Order5);
      Assert.That (clonedExisting.State.IsUnchanged, Is.True);
      Assert.That (clonedExisting.State.IsUnchanged, Is.True);
    }

    [Test]
    public void Clone_CopiesTimestamp ()
    {
      var original = _newDataContainer;
      original.SetTimestamp (12);

      var clone = original.Clone (DomainObjectIDs.Order3);
      Assert.That (clone.Timestamp, Is.EqualTo (12));
    }

    [Test]
    public void Clone_CopiesPropertyValues ()
    {
      var original = _existingDataContainer;

      var clone = original.Clone (DomainObjectIDs.Order3);

      Assert.That (
          clone.ClassDefinition.GetPropertyDefinitions().Select (pd => clone.GetValue (pd)),
          Is.EqualTo (clone.ClassDefinition.GetPropertyDefinitions().Select (pd => original.GetValue (pd))));
      Assert.That (
          clone.ClassDefinition.GetPropertyDefinitions().Select (pd => clone.GetValue (pd, ValueAccess.Original)),
          Is.EqualTo (clone.ClassDefinition.GetPropertyDefinitions().Select (pd => original.GetValue (pd, ValueAccess.Original))));
    }

    [Test]
    public void Clone_CopiesHasBeenMarkedChanged ()
    {
      var original = _existingDataContainer;
      original.MarkAsChanged();
      Assert.That (original.HasBeenMarkedChanged, Is.True);

      var clone = original.Clone (DomainObjectIDs.Order3);
      Assert.That (clone.HasBeenMarkedChanged, Is.True);
    }

    [Test]
    public void Clone_CopiesHasBeenChangedFlag ()
    {
      var original = _existingDataContainer;
      original.SetValue (_orderNumberProperty, 10);
      var state1 = original.State;
      Assert.That (state1.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state1), Is.EqualTo (1));
      Assert.That (original.State.IsChanged, Is.True);

      var clone = original.Clone (DomainObjectIDs.Order3);
      var state2 = clone.State;
      Assert.That (state2.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state1), Is.EqualTo (1));
      Assert.That (clone.State.IsChanged, Is.True);
    }

    [Test]
    public void Clone_DomainObjectEmpty ()
    {
      var original = _existingDataContainer;
      original.SetDomainObject (DomainObjectMother.CreateFakeObject (original.ID));
      Assert.That (original.HasDomainObject, Is.True);

      var clone = original.Clone (DomainObjectIDs.Order1);
      Assert.That (clone.HasDomainObject, Is.False);
    }

    [Test]
    public void Clone_TransactionEmpty ()
    {
      var original = _existingDataContainer;
      original.SetDomainObject (DomainObjectMother.CreateFakeObject (original.ID));
      TestableClientTransaction.DataManager.RegisterDataContainer (original);
      Assert.That (original.IsRegistered, Is.True);

      var clone = original.Clone (DomainObjectIDs.Order1);
      Assert.That (clone.IsRegistered, Is.False);
    }

    [Test]
    public void Clone_EventListenerEmpty ()
    {
      _existingDataContainer.SetEventListener (_eventListenerMock);
      Assert.That (_existingDataContainer.EventListener, Is.Not.Null);

      var clone = _existingDataContainer.Clone (DomainObjectIDs.Order1);
      Assert.That (clone.EventListener, Is.Null);
    }

    [Test]
    public void CommitState_SetsStateToExisting ()
    {
      _newDataContainer.CommitState ();

      var state = _newDataContainer.State;
      Assert.That (state.IsUnchanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state), Is.EqualTo (1));
      Assert.That (_newDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void CommitState_RaisesStateUpdated ()
    {
      CheckStateNotification (_newDataContainer, dc => dc.CommitState (), new DataContainerState.Builder().SetUnchanged().Value);
    }

    [Test]
    public void CommitState_CommitsPropertyValues ()
    {
      _existingDataContainer.SetValue (_orderNumberProperty, 10);

      Assert.That (_existingDataContainer.HasValueChanged (_orderNumberProperty), Is.True);
      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty, ValueAccess.Original), Is.EqualTo (0));
      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty), Is.EqualTo (10));

      _existingDataContainer.CommitState ();

      Assert.That (_existingDataContainer.HasValueChanged (_orderNumberProperty), Is.False);
      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty, ValueAccess.Original), Is.EqualTo (10));
      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty), Is.EqualTo (10));
    }

    [Test]
    public void CommitState_ResetsChangedFlag ()
    {
      _existingDataContainer.SetValue (_orderNumberProperty, 10);

      var stateBeforeChange = _existingDataContainer.State;
      Assert.That (stateBeforeChange.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (stateBeforeChange), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsChanged, Is.True);

      _existingDataContainer.CommitState ();

      var stateAfterChange = _existingDataContainer.State;
      Assert.That (stateAfterChange.IsUnchanged, Is.True);
      Assert.That (GetNumberOfSetFlags (stateAfterChange), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void CommitState_ResetsMarkedChangedFlag ()
    {
      _existingDataContainer.MarkAsChanged ();
      Assert.That (_existingDataContainer.HasBeenMarkedChanged, Is.True);

      _existingDataContainer.CommitState ();

      Assert.That (_existingDataContainer.HasBeenMarkedChanged, Is.False);
    }

    [Test]
    public void CommitState_DiscardedDataContainer_Throws ()
    {
      Assert.That (
          () => _discardedDataContainer.CommitState (),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void CommitState_DeletedDataContainer_Throws ()
    {
      Assert.That (
          () => _deletedDataContainer.CommitState (),
          Throws.InvalidOperationException
              .With.Message.EqualTo ("Deleted data containers cannot be committed, they have to be discarded."));
    }

    [Test]
    public void RollbackState_SetsStateToExisting ()
    {
      _deletedDataContainer.RollbackState ();

      var state = _deletedDataContainer.State;
      Assert.That (state.IsUnchanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state), Is.EqualTo (1));
      Assert.That (_deletedDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void RollbackState_RaisesStateUpdated ()
    {
      CheckStateNotification (_existingDataContainer, dc => dc.RollbackState (), new DataContainerState.Builder().SetUnchanged().Value);
    }

    [Test]
    public void RollbackState_RollsbackPropertyValues ()
    {
      _existingDataContainer.SetValue (_orderNumberProperty, 10);

      Assert.That (_existingDataContainer.HasValueChanged (_orderNumberProperty), Is.True);
      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty, ValueAccess.Original), Is.EqualTo (0));
      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty), Is.EqualTo (10));

      _existingDataContainer.RollbackState ();

      Assert.That (_existingDataContainer.HasValueChanged (_orderNumberProperty), Is.False);
      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty, ValueAccess.Original), Is.EqualTo (0));
      Assert.That (_existingDataContainer.GetValue (_orderNumberProperty), Is.EqualTo (0));
    }

    [Test]
    public void RollbackState_ResetsChangedFlag ()
    {
      _existingDataContainer.SetValue (_orderNumberProperty, 10);

      var stateBeforeChange = _existingDataContainer.State;
      Assert.That (stateBeforeChange.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (stateBeforeChange), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsChanged, Is.True);

      _existingDataContainer.RollbackState ();

      var stateAfterChange = _existingDataContainer.State;
      Assert.That (stateAfterChange.IsUnchanged, Is.True);
      Assert.That (GetNumberOfSetFlags (stateAfterChange), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void RollbackState_ResetsMarkedChangedFlag ()
    {
      _existingDataContainer.MarkAsChanged ();
      Assert.That (_existingDataContainer.HasBeenMarkedChanged, Is.True);

      _existingDataContainer.RollbackState ();

      Assert.That (_existingDataContainer.HasBeenMarkedChanged, Is.False);
    }

    [Test]
    public void RollbackState_DiscardedDataContainer_Throws ()
    {
      Assert.That (
          () => _discardedDataContainer.RollbackState (),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void RollbackState_NewDataContainer_Throws ()
    {
      Assert.That (
          () => _newDataContainer.RollbackState (),
          Throws.InvalidOperationException
              .With.Message.EqualTo ("New data containers cannot be rolled back, they have to be discarded."));
    }

    [Test]
    public void Delete_SetsStateToDeleted ()
    {
      _existingDataContainer.Delete ();

      var state = _existingDataContainer.State;
      Assert.That (state.IsDeleted, Is.True);
      Assert.That (GetNumberOfSetFlags (state), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsDeleted, Is.True);
    }

     [Test]
    public void Delete_RaisesStateUpdated ()
    {
      CheckStateNotification (_existingDataContainer, dc => dc.Delete(), new DataContainerState.Builder().SetDeleted().Value);
    }

    [Test]
    public void Delete_DiscardedDataContainer_Throws ()
    {
      Assert.That (
          () => _discardedDataContainer.Delete (),
          Throws.InstanceOf<ObjectInvalidException>());
    }

    [Test]
    public void Delete_NewDataContainer_Throws ()
    {
      Assert.That (
          () => _newDataContainer.Delete (),
          Throws.InvalidOperationException
              .With.Message.EqualTo ("New data containers cannot be deleted, they have to be discarded."));
    }

    [Test]
    public void Discard_SetsDiscardedFlag ()
    {
      Assert.That (_newDataContainer.State.IsDiscarded, Is.False);

      _newDataContainer.Discard ();

      Assert.That (_newDataContainer.State.IsDiscarded, Is.True);
    }

    [Test]
    public void Discard_RaisesStateUpdated ()
    {
      CheckStateNotification (_newDataContainer, dc => dc.Discard (), new DataContainerState.Builder().SetDiscarded().Value);
    }

    [Test]
    public void Discard_DisassociatesFromEventListener ()
    {
      _newDataContainer.SetEventListener (_eventListenerMock);
      Assert.That (_newDataContainer.EventListener, Is.Not.Null);

      _newDataContainer.Discard ();

      Assert.That (_newDataContainer.EventListener, Is.Null);
    }

    [Test]
    public void SetDataFromSubTransaction_SetsValues ()
    {
      var sourceDataContainer = DomainObjectIDs.Order1.GetObject<Order> ().InternalDataContainer;
      var newDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order3);
      Assert.That (newDataContainer.GetValue (_orderNumberProperty), Is.Not.EqualTo (1));

      newDataContainer.SetPropertyDataFromSubTransaction (sourceDataContainer);

      Assert.That (newDataContainer.GetValue (_orderNumberProperty), Is.EqualTo (1));
    }

    [Test]
    public void SetDataFromSubTransaction_SetsForeignKeys ()
    {
      var sourceDataContainer = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ().InternalDataContainer;
      var newDataContainer = DataContainer.CreateNew (DomainObjectIDs.OrderTicket2);
      var propertyDefinition = GetPropertyDefinition (typeof (OrderTicket), "Order");
      Assert.That (newDataContainer.GetValue (propertyDefinition), Is.Not.EqualTo (DomainObjectIDs.Order1));

      newDataContainer.SetPropertyDataFromSubTransaction (sourceDataContainer);

      Assert.That (newDataContainer.GetValue (propertyDefinition), Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void SetDataFromSubTransaction_SetsChangedFlag_IfChanged ()
    {
      var sourceDataContainer = DomainObjectIDs.Order1.GetObject<Order> ().InternalDataContainer;
      var existingDataContainer = DomainObjectIDs.Order3.GetObject<Order> ().InternalDataContainer;
      var stateBeforeChange = existingDataContainer.State;
      Assert.That (stateBeforeChange.IsUnchanged, Is.True);
      Assert.That (GetNumberOfSetFlags (stateBeforeChange), Is.EqualTo (1));
      Assert.That (existingDataContainer.State.IsUnchanged, Is.True);

      existingDataContainer.SetPropertyDataFromSubTransaction (sourceDataContainer);

      var stateAfterChange = existingDataContainer.State;
      Assert.That (stateAfterChange.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (stateAfterChange), Is.EqualTo (1));
      Assert.That (existingDataContainer.State.IsChanged, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_ResetsChangedFlag_IfUnchanged ()
    {
      var sourceDataContainer = DomainObjectIDs.Order1.GetObject<Order> ().InternalDataContainer;
      var targetDataContainer = sourceDataContainer.Clone (DomainObjectIDs.Order1);
      targetDataContainer.SetValue (_orderNumberProperty, 10);
      var stateBeforeChange = targetDataContainer.State;
      Assert.That (stateBeforeChange.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (stateBeforeChange), Is.EqualTo (1));
      Assert.That (targetDataContainer.State.IsChanged, Is.True);

      targetDataContainer.SetPropertyDataFromSubTransaction (sourceDataContainer);

      var stateAfterChange = targetDataContainer.State;
      Assert.That (stateAfterChange.IsUnchanged, Is.True);
      Assert.That (GetNumberOfSetFlags (stateAfterChange), Is.EqualTo (1));
      Assert.That (targetDataContainer.State.IsUnchanged, Is.True);
    }

    [Test]
    public void SetDataFromSubTransaction_RaisesStateUpdated_Changed ()
    {
      var sourceDataContainer = _existingDataContainer;
      sourceDataContainer.SetValue (_orderNumberProperty, 12);

      var targetDataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      var state = targetDataContainer.State;
      Assert.That (state.IsUnchanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state), Is.EqualTo (1));
      Assert.That (targetDataContainer.State.IsUnchanged, Is.True);

      CheckStateNotification (
          targetDataContainer,
          dc => dc.SetPropertyDataFromSubTransaction (sourceDataContainer),
          new DataContainerState.Builder().SetChanged().Value);
    }

    [Test]
    public void SetDataFromSubTransaction_RaisesStateUpdated_Unchanged ()
    {
      var sourceDataContainer = DomainObjectIDs.Order1.GetObject<Order> ().InternalDataContainer;
      var targetDataContainer = sourceDataContainer.Clone (DomainObjectIDs.Order3);
      targetDataContainer.SetValue (_orderNumberProperty, 10);
      var state = targetDataContainer.State;
      Assert.That (state.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state), Is.EqualTo (1));
      Assert.That (targetDataContainer.State.IsChanged, Is.True);

      CheckStateNotification (
          targetDataContainer,
          dc => dc.SetPropertyDataFromSubTransaction (sourceDataContainer),
          new DataContainerState.Builder().SetUnchanged().Value);
    }

    [Test]
    public void SetDataFromSubTransaction_RaisesStateUpdated_OtherState ()
    {
      var sourceDataContainer = DomainObjectIDs.Order1.GetObject<Order> ().InternalDataContainer;
      var targetDataContainer = sourceDataContainer.Clone (DomainObjectIDs.Order3);
      targetDataContainer.Delete ();
      var state = targetDataContainer.State;
      Assert.That (state.IsDeleted, Is.True);
      Assert.That (GetNumberOfSetFlags (state), Is.EqualTo (1));
      Assert.That (targetDataContainer.State.IsDeleted, Is.True);

      CheckStateNotification (
          targetDataContainer,
          dc => dc.SetPropertyDataFromSubTransaction (sourceDataContainer),
          new DataContainerState.Builder().SetDeleted().Value);
    }

    [Test]
    public void SetDataFromSubTransaction_DoesntMarkAsChanged ()
    {
      var sourceDataContainer = DomainObjectIDs.Order1.GetObject<Order> ().InternalDataContainer;
      var targetDataContainer = sourceDataContainer.Clone (DomainObjectIDs.Order1);
      sourceDataContainer.MarkAsChanged();
      Assert.That (sourceDataContainer.HasBeenMarkedChanged, Is.True);
      Assert.That (targetDataContainer.HasBeenMarkedChanged, Is.False);

      targetDataContainer.SetPropertyDataFromSubTransaction (sourceDataContainer);

      Assert.That (targetDataContainer.HasBeenMarkedChanged, Is.False);
    }

    [Test]
    public void SetDataFromSubTransaction_InvalidDefinition ()
    {
      var sourceDataContainer = DomainObjectIDs.Order1.GetObject<Order> ().InternalDataContainer;
      var targetDataContainer = DomainObjectIDs.OrderTicket1.GetObject<OrderTicket> ().InternalDataContainer;
      Assert.That (
          () => targetDataContainer.SetPropertyDataFromSubTransaction (sourceDataContainer),
          Throws.ArgumentException
              .With.Message.EqualTo (
                  "Cannot set this data container's property values from 'Order|5682f032-2f0b-494b-a31c-c97f02b89c36|System.Guid'; the data containers do not "
                  + "have the same class definition.\r\nParameter name: source"));
    }

    [Test]
    public void GetIDEvenPossibleWhenDiscarded ()
    {
      Assert.That (_discardedDataContainer.State.IsDiscarded, Is.True);
      Assert.That (_discardedDataContainer.ID, Is.EqualTo (_invalidObjectID));
    }

    [Test]
    public void GetDomainObjectEvenPossibleWhenDiscarded ()
    {
      var domainObject = Order.NewObject ();
      var dataContainerWithObject = domainObject.InternalDataContainer;
      dataContainerWithObject.Discard ();

      Assert.That (dataContainerWithObject.State.IsDiscarded, Is.True);
      Assert.That (dataContainerWithObject.DomainObject, Is.SameAs (domainObject));
    }

    [Test]
    public void MarkAsChanged ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      DataContainer dataContainer = order.InternalDataContainer;
      var state1 = dataContainer.State;
      Assert.That (state1.IsUnchanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state1), Is.EqualTo (1));
      Assert.That (dataContainer.State.IsUnchanged, Is.True);
      dataContainer.MarkAsChanged();
      var state2 = dataContainer.State;
      Assert.That (state2.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state2), Is.EqualTo (1));
      Assert.That (dataContainer.State.IsChanged, Is.True);

      TestableClientTransaction.Rollback();
      var state3 = dataContainer.State;
      Assert.That (state3.IsUnchanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state3), Is.EqualTo (1));
      Assert.That (dataContainer.State.IsUnchanged, Is.True);

      SetDatabaseModifyable();

      dataContainer.MarkAsChanged();
      var state4 = dataContainer.State;
      Assert.That (state4.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state4), Is.EqualTo (1));
      Assert.That (dataContainer.State.IsChanged, Is.True);

      TestableClientTransaction.Commit();
      var state5 = dataContainer.State;
      Assert.That (state5.IsUnchanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state5), Is.EqualTo (1));
      Assert.That (dataContainer.State.IsUnchanged, Is.True);

      DataContainer clone = dataContainer.Clone (DomainObjectIDs.Order1);
      var state6 = clone.State;
      Assert.That (state6.IsUnchanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state6), Is.EqualTo (1));
      Assert.That (clone.State.IsUnchanged, Is.True);

      dataContainer.MarkAsChanged();
      var state7 = dataContainer.State;
      Assert.That (state7.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state7), Is.EqualTo (1));
      Assert.That (dataContainer.State.IsChanged, Is.True);

      clone = dataContainer.Clone (DomainObjectIDs.Order1);
      var state8 = clone.State;
      Assert.That (state8.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (state8), Is.EqualTo (1));
      Assert.That (clone.State.IsChanged, Is.True);
    }

    [Test]
    public void MarkAsChanged_WithoutClientTransaction ()
    {
      var stateBeforeChange = _existingDataContainer.State;
      Assert.That (stateBeforeChange.IsUnchanged, Is.True);
      Assert.That (GetNumberOfSetFlags (stateBeforeChange), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsUnchanged, Is.True);

      _existingDataContainer.MarkAsChanged ();

      var stateAfterChange = _existingDataContainer.State;
      Assert.That (stateAfterChange.IsChanged, Is.True);
      Assert.That (GetNumberOfSetFlags (stateAfterChange), Is.EqualTo (1));
      Assert.That (_existingDataContainer.State.IsChanged, Is.True);
    }

    [Test]
    public void MarkAsChanged_RaisesStateUpdated ()
    {
      CheckStateNotification (_existingDataContainer, dc => dc.MarkAsChanged(), new DataContainerState.Builder().SetChanged().Value);
    }

    [Test]
    public void MarkAsChangedThrowsWhenNew ()
    {
      Order order = Order.NewObject();
      DataContainer dataContainer = order.InternalDataContainer;
      Assert.That (
          () => dataContainer.MarkAsChanged(),
          Throws.InvalidOperationException
              .With.Message.EqualTo (
                  "Only existing DataContainers can be marked as changed."));
    }

    [Test]
    public void MarkAsChangedThrowsWhenDeleted ()
    {
      Order order = DomainObjectIDs.Order1.GetObject<Order> ();
      order.Delete();
      DataContainer dataContainer = order.InternalDataContainer;
      Assert.That (
          () => dataContainer.MarkAsChanged(),
          Throws.InvalidOperationException
              .With.Message.EqualTo (
                  "Only existing DataContainers can be marked as changed."));
    }

    [Test]
    public void ErrorWhenNoClientTransaction ()
    {
      DataContainer dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      Assert.That (
          () => dc.ClientTransaction,
          Throws.InvalidOperationException
              .With.Message.EqualTo (
                  "DataContainer has not been registered with a transaction."));
    }

    [Test]
    public void DomainObject_NoneSet ()
    {
      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      Assert.That (
          () => dc.DomainObject,
          Throws.InvalidOperationException
              .With.Message.EqualTo (
                  "This DataContainer has not been associated with a DomainObject yet."));
    }

    [Test]
    public void SetDomainObject ()
    {
      var domainObject = DomainObjectIDs.Order1.GetObject<Order> ();

      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      dc.SetDomainObject (domainObject);

      Assert.That (dc.DomainObject, Is.SameAs (domainObject));
    }

    [Test]
    public void SetDomainObject_InvalidID ()
    {
      var domainObject = DomainObjectIDs.Order3.GetObject<Order> ();

      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      Assert.That (
          () => dc.SetDomainObject (domainObject),
          Throws.ArgumentException
              .With.Message.EqualTo (
                  "The given DomainObject has another ID than this DataContainer.\r\n"
                  + "Parameter name: domainObject"));
    }

    [Test]
    public void SetDomainObject_DomainObjectAlreadySet ()
    {
      var domainObject1 = DomainObjectIDs.Order1.GetObject<Order> ();
      var domainObject2 = DomainObjectMother.GetObjectInOtherTransaction<Order> (DomainObjectIDs.Order1);

      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      dc.SetDomainObject (domainObject1);
      Assert.That (
          () => dc.SetDomainObject (domainObject2),
          Throws.InvalidOperationException
              .With.Message.EqualTo (
                  "This DataContainer has already been associated with a DomainObject."));
    }

    [Test]
    public void SetDomainObject_SameDomainObjectAlreadySet ()
    {
      var domainObject = DomainObjectIDs.Order1.GetObject<Order> ();

      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      dc.SetDomainObject (domainObject);
      dc.SetDomainObject (domainObject);

      Assert.That (dc.DomainObject, Is.SameAs (domainObject));
    }

    [Test]
    public void HasDomainObject_False ()
    {
      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      Assert.That (dc.HasDomainObject, Is.False);
    }

    [Test]
    public void HasDomainObject_True ()
    {
      var domainObject = DomainObjectIDs.Order1.GetObject<Order> ();

      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      dc.SetDomainObject (domainObject);

      Assert.That (dc.HasDomainObject, Is.True);
    }

    [Test]
    public void SetClientTransaction ()
    {
      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      DataContainerTestHelper.SetClientTransaction (dc, TestableClientTransaction);

      Assert.That (dc.ClientTransaction, Is.SameAs (TestableClientTransaction));
    }

    [Test]
    public void SetClientTransaction_Twice ()
    {
      var dc = DataContainer.CreateNew (DomainObjectIDs.Order1);
      DataContainerTestHelper.SetClientTransaction (dc, TestableClientTransaction);
      Assert.That (
          () => DataContainerTestHelper.SetClientTransaction (dc, TestableClientTransaction),
          Throws.InvalidOperationException
              .With.Message.EqualTo ("This DataContainer has already been registered with a ClientTransaction."));
    }

    private void CheckStateNotification (DataContainer dataContainer, Action<DataContainer> action, DataContainerState expectedState)
    {
      dataContainer.SetEventListener (_eventListenerMock);

      action (dataContainer);

      _eventListenerMock.AssertWasCalled (mock => mock.StateUpdated (dataContainer, expectedState));
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

      return count;
    }
  }
}