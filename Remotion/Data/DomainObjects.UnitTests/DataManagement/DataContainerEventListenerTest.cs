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
using Remotion.Data.DomainObjects.UnitTests.Mapping;

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
      base.SetUp();

      _eventListener = new DataContainerEventListener(EventSinkWithMock.Object);

      _domainObject = DomainObjectMother.CreateFakeObject();
      _dataContainer = DataContainerObjectMother.Create(domainObject: _domainObject);
      _propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo();
    }

    protected override DataContainerEventListener EventListener
    {
      get { return _eventListener; }
    }

    [Test]
    public void PropertyValueReading ()
    {
      EventSinkWithMock.Setup(mock => mock.RaisePropertyValueReadingEvent(_domainObject, _propertyDefinition, ValueAccess.Original)).Verifiable();

      EventListener.PropertyValueReading(_dataContainer, _propertyDefinition, ValueAccess.Original);

      EventSinkWithMock.Verify();
    }

    [Test]
    public void PropertyValueRead ()
    {
      EventSinkWithMock.Setup(mock => mock.RaisePropertyValueReadEvent(_domainObject, _propertyDefinition, "value", ValueAccess.Original)).Verifiable();

      EventListener.PropertyValueRead(_dataContainer, _propertyDefinition, "value", ValueAccess.Original);

      EventSinkWithMock.Verify();
    }

    [Test]
    public void PropertyValueChanging ()
    {
      EventSinkWithMock.Setup(mock1 => mock1.RaisePropertyValueChangingEvent(_domainObject, _propertyDefinition, "oldValue", "newValue")).Verifiable();

      EventListener.PropertyValueChanging(_dataContainer, _propertyDefinition, "oldValue", "newValue");

      EventSinkWithMock.Verify();
    }

    [Test]
    public void PropertyValueChanging_WithObjectIDProperty_DoesNotRaiseEvent ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID();

      EventListener.PropertyValueChanging(_dataContainer, propertyDefinition, "oldValue", "newValue");

      EventSinkWithMock.Verify();
    }

    [Test]
    public void PropertyValueChanged ()
    {
      EventSinkWithMock.Setup(mock => mock.RaisePropertyValueChangedEvent(_domainObject, _propertyDefinition, "oldValue", "newValue")).Verifiable();

      EventListener.PropertyValueChanged(_dataContainer, _propertyDefinition, "oldValue", "newValue");

      EventSinkWithMock.Verify();
    }

    [Test]
    public void PropertyValueChanged_WithObjectIDProperty_DoesNotRaiseEvent ()
    {
      var propertyDefinition = PropertyDefinitionObjectMother.CreateForFakePropertyInfo_ObjectID();

      EventListener.PropertyValueChanged(_dataContainer, propertyDefinition, "oldValue", "newValue");

      EventSinkWithMock.Verify();
    }

    [Test]
    public void StateUpdated ()
    {
      var state = new DataContainerState.Builder().SetChanged().Value;
      EventSinkWithMock.Setup(mock => mock.RaiseDataContainerStateUpdatedEvent(_dataContainer, state)).Verifiable();

      EventListener.StateUpdated(_dataContainer, state);

      EventSinkWithMock.Verify();
    }
  }
}
