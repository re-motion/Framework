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
using Moq;
using Moq.Protected;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  public abstract class DomainObjectMockEventReceiver
  {
    private readonly DomainObject _domainObject;

    protected DomainObjectMockEventReceiver (DomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);

      _domainObject = domainObject;

      _domainObject.Committed += Committed;
      _domainObject.Committing += Committing;
      _domainObject.RolledBack += RolledBack;
      _domainObject.RollingBack += RollingBack;
      _domainObject.Deleted += Deleted;
      _domainObject.Deleting += Deleting;
      _domainObject.PropertyChanged += PropertyChanged;
      _domainObject.PropertyChanging += PropertyChanging;
      _domainObject.RelationChanged += RelationChanged;
      _domainObject.RelationChanging += RelationChanging;
    }

    public abstract void RelationChanging (object sender, RelationChangingEventArgs args);
    public abstract void RelationChanged (object sender, RelationChangedEventArgs args);
    public abstract void PropertyChanging (object sender, PropertyChangeEventArgs args);
    public abstract void PropertyChanged (object sender, PropertyChangeEventArgs args);
    public abstract void Deleting (object sender, EventArgs e);
    public abstract void Deleted (object sender, EventArgs e);
    public abstract void Committing (object sender, DomainObjectCommittingEventArgs e);
    public abstract void Committed (object sender, EventArgs e);
    public abstract void RollingBack (object sender, EventArgs e);
    public abstract void RolledBack (object sender, EventArgs e);

    public void RelationChanging (IRelationEndPointDefinition relationEndPointDefinition, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      this.Setup (
          mock => RelationChanging (
              _domainObject,
              It.Is<RelationChangingEventArgs> (args =>
                  args.RelationEndPointDefinition == relationEndPointDefinition
                      && args.OldRelatedObject == oldRelatedObject
                      && args.NewRelatedObject == newRelatedObject))).Verifiable();
    }

    public void RelationChanged (IRelationEndPointDefinition relationEndPointDefinition, DomainObject oldRelatedObject, DomainObject newRelatedObject)
    {
      this.Setup (
          mock => RelationChanged (
              _domainObject,
              It.Is<RelationChangedEventArgs> (args =>
                  args.RelationEndPointDefinition == relationEndPointDefinition
                      && args.OldRelatedObject == oldRelatedObject
                      && args.NewRelatedObject == newRelatedObject))).Verifiable();
    }

    public void PropertyChanging (PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      this.Setup (
          mock => mock.PropertyChanging (
              _domainObject,
              It.Is<PropertyChangeEventArgs> (                  args => args.PropertyDefinition == propertyDefinition
                          && args.OldValue == oldValue
                          && args.NewValue == newValue))).Verifiable();
    }

    public void PropertyChanged (PropertyDefinition propertyDefinition, object oldValue, object newValue)
    {
      this.Setup (
          mock => mock.PropertyChanged (
              _domainObject,
              It.Is<PropertyChangeEventArgs> (                  args => args.PropertyDefinition == propertyDefinition
                          && args.OldValue == oldValue
                          && args.NewValue == newValue))).Verifiable();
    }

    public void Committing ()
    {
      this.Setup (mock => mock.Committing (_domainObject, It.IsAny<DomainObjectCommittingEventArgs>())).Verifiable();
    }

    public void Committed ()
    {
      this.Setup (mock => mock.Committed (_domainObject, It.IsAny<EventArgs>())).Verifiable();
    }

    public void RollingBack ()
    {
      this.Setup (mock => mock.RollingBack (_domainObject, It.IsAny<EventArgs>())).Verifiable();
    }

    public void RolledBack ()
    {
      this.Setup (mock => mock.RolledBack (_domainObject, It.IsAny<EventArgs>())).Verifiable();
    }
  }
}
