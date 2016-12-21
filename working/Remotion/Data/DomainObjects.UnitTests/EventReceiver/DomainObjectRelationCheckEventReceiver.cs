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
using System.Collections;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  public class DomainObjectRelationCheckEventReceiver : DomainObjectEventReceiver
  {
    // types

    // static members and constants

    // member fields
    private IDictionary changingRelatedObjects;
    private IDictionary changedRelatedObjects;
    private StateType changingObjectState;
    private StateType changedObjectState;
    
    // construction and disposing

    public DomainObjectRelationCheckEventReceiver (DomainObject domainObject)
      : this (domainObject, false)
    {
    }

    public DomainObjectRelationCheckEventReceiver (DomainObject domainObject, bool cancel)
      : base (domainObject, cancel)
    {
      changingRelatedObjects = new Dictionary<string, DomainObject> ();
      changedRelatedObjects = new Dictionary<string, DomainObject> ();
    }

    // methods and properties

    public DomainObject GetChangingRelatedDomainObject (string propertyName)
    {
      return (DomainObject) changingRelatedObjects[propertyName];
    }

    public DomainObject GetChangedRelatedDomainObject (string propertyName)
    {
      return (DomainObject) changedRelatedObjects[propertyName];
    }

    protected override void DomainObject_RelationChanging (object sender, RelationChangingEventArgs args)
    {
      TestDomainBase domainObject = (TestDomainBase) sender;

      changingObjectState = domainObject.State;

      string changedProperty = args.RelationEndPointDefinition.PropertyName;

      if (CardinalityType.One == domainObject.InternalDataContainer.ClassDefinition.GetRelationEndPointDefinition (changedProperty).Cardinality)
      {
        DomainObject relatedDomainObject = domainObject.GetRelatedObject (changedProperty);
        changingRelatedObjects.Add (changedProperty, relatedDomainObject);
      }
      else
      {
        DomainObjectCollection relatedDomainObjectCollection = domainObject.GetRelatedObjects (changedProperty);
        changingRelatedObjects.Add (changedProperty, relatedDomainObjectCollection.Clone (true));
      }

      base.DomainObject_RelationChanging (sender, args);
    }

    protected override void DomainObject_RelationChanged (object sender, RelationChangedEventArgs args)
    {
      TestDomainBase domainObject = (TestDomainBase) sender;

      changedObjectState = domainObject.State;

      string changedProperty = args.RelationEndPointDefinition.PropertyName;

      if (CardinalityType.One == domainObject.InternalDataContainer.ClassDefinition.GetRelationEndPointDefinition (changedProperty).Cardinality)
      {
        DomainObject relatedDomainObject = domainObject.GetRelatedObject (changedProperty);
        changedRelatedObjects.Add (changedProperty, relatedDomainObject);
      }
      else
      {
        DomainObjectCollection relatedDomainObjectCollection = domainObject.GetRelatedObjects (changedProperty);
        changedRelatedObjects.Add (changedProperty, relatedDomainObjectCollection.Clone (true));
      }

      base.DomainObject_RelationChanged (sender, args);
    }
  }
}
