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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.DataManagement.SerializableFakes;
using Remotion.Data.DomainObjects.UnitTests.Mapping;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class DataContainerEventListenerTest : ForwardingEventListenerTestBase<DataContainerEventListener>
  {
    private DataContainerEventListener _eventListener;

    private DomainObject _domainObject;
    private DataContainer _dataContainer;
    private PropertyDefinition _propertyDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _eventListener = new DataContainerEventListener (EventSinkWithMock);

      _domainObject = DomainObjectMother.CreateFakeObject();
      _dataContainer = DataContainerObjectMother.Create (domainObject: _domainObject);
      _propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo ();
    }

    protected override DataContainerEventListener EventListener
    {
      get { return _eventListener; }
    }

    [Test]
    public void PropertyValueReading ()
    {
      EventSinkWithMock.Expect (mock => mock.RaisePropertyValueReadingEvent ( _domainObject, _propertyDefinition, ValueAccess.Original));
      EventSinkWithMock.Replay();

      EventListener.PropertyValueReading (_dataContainer, _propertyDefinition, ValueAccess.Original);

      EventSinkWithMock.VerifyAllExpectations();
    }

    [Test]
    public void PropertyValueRead ()
    {
      EventSinkWithMock.Expect (mock => mock.RaisePropertyValueReadEvent ( _domainObject, _propertyDefinition, "value", ValueAccess.Original));
      EventSinkWithMock.Replay();

      EventListener.PropertyValueRead (_dataContainer, _propertyDefinition, "value", ValueAccess.Original);

      EventSinkWithMock.VerifyAllExpectations();
    }

    [Test]
    public void PropertyValueChanging ()
    {
      EventSinkWithMock.Expect (mock1 => mock1.RaisePropertyValueChangingEvent ( _domainObject, _propertyDefinition, "oldValue", "newValue"));
      EventSinkWithMock.Replay();

      EventListener.PropertyValueChanging (_dataContainer, _propertyDefinition, "oldValue", "newValue");

      EventSinkWithMock.VerifyAllExpectations();
    }

    [Test]
    public void PropertyValueChanging_WithObjectIDProperty_DoesNotRaiseEvent ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID ();
      EventSinkWithMock.Replay();

      EventListener.PropertyValueChanging (_dataContainer, propertyDefinition, "oldValue", "newValue");

      EventSinkWithMock.VerifyAllExpectations();
    }

    [Test]
    public void PropertyValueChanged ()
    {
      EventSinkWithMock.Expect (mock => mock.RaisePropertyValueChangedEvent ( _domainObject, _propertyDefinition, "oldValue", "newValue"));
      EventSinkWithMock.Replay();

      EventListener.PropertyValueChanged (_dataContainer, _propertyDefinition, "oldValue", "newValue");

      EventSinkWithMock.VerifyAllExpectations();
    }

    [Test]
    public void PropertyValueChanged_WithObjectIDProperty_DoesNotRaiseEvent ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID ();
      EventSinkWithMock.Replay();

      EventListener.PropertyValueChanged (_dataContainer, propertyDefinition, "oldValue", "newValue");

      EventSinkWithMock.VerifyAllExpectations();
    }

    [Test]
    public void StateUpdated ()
    {
      EventSinkWithMock.Expect (mock => mock.RaiseDataContainerStateUpdatedEvent ( _dataContainer, StateType.New));
      EventSinkWithMock.Replay();

      EventListener.StateUpdated (_dataContainer, StateType.New);

      EventSinkWithMock.VerifyAllExpectations();
    }
    
    [Test]
    public void Serializable ()
    {
      var instance = new DataContainerEventListener (new SerializableClientTransactionEventSinkFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (instance);

      Assert.That (deserializedInstance.EventSink, Is.Not.Null);
    }
  }
}