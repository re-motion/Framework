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
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  public class SequenceEventReceiver : EventReceiverBase
  {
    // types

    // static members and constants

    // member fields

    private readonly DomainObject[] _domainObjects;
    private readonly DomainObjectCollection[] _collections;
    private readonly List<ChangeState> _states = new List<ChangeState>();
    private int _cancelEventNumber;

    // construction and disposing

    public SequenceEventReceiver (DomainObjectCollection collection)
        : this(new DomainObject[0], new[] { collection })
    {
    }

    public SequenceEventReceiver (DomainObject domainObject)
        : this(new[] { domainObject }, new DomainObjectCollection[0])
    {
    }

    public SequenceEventReceiver (DomainObject[] domainObjects, DomainObjectCollection[] collections)
        : this(domainObjects, collections, 0)
    {
    }

    public SequenceEventReceiver (DomainObjectCollection collection, int cancelEventNumber)
        : this(new DomainObject[0], new[] { collection }, cancelEventNumber)
    {
    }

    public SequenceEventReceiver (DomainObject[] domainObjects, DomainObjectCollection[] collections, int cancelEventNumber)
    {
      ArgumentUtility.CheckNotNull("domainObjects", domainObjects);
      ArgumentUtility.CheckNotNull("collections", collections);

      _domainObjects = domainObjects;
      _collections = collections;
      _cancelEventNumber = cancelEventNumber;

      foreach (var domainObject in domainObjects)
      {
        domainObject.Deleting += DomainObject_Deleting;
        domainObject.Deleted += DomainObject_Deleted;
        domainObject.PropertyChanging += DomainObject_PropertyChanging;
        domainObject.PropertyChanged += DomainObject_PropertyChanged;
        domainObject.RelationChanging += DomainObject_RelationChanging;
        domainObject.RelationChanged += DomainObject_RelationChanged;
      }

      foreach (var collection in collections)
      {
        collection.Adding += Collection_Changing;
        collection.Added += Collection_Changed;
        collection.Removing += Collection_Changing;
        collection.Removed += Collection_Changed;
        collection.Deleting += Collection_Deleting;
        collection.Deleted += Collection_Deleted;
      }
    }

    // methods and properties

    public int CancelEventNumber
    {
      get { return _cancelEventNumber; }
      set { _cancelEventNumber = value; }
    }

    public ChangeState this [int index]
    {
      get { return _states[index]; }
    }

    public int Count
    {
      get { return _states.Count; }
    }

    public void Check (ChangeState[] expectedStates)
    {
      for (int i = 0; i < expectedStates.Length; i++)
      {
        if (i >= _states.Count)
          Assert.Fail("Missing event: " + expectedStates[i].Message);

        try
        {
          _states[i].Check(expectedStates[i]);
        }
        catch (Exception e)
        {
          Assert.Fail(
              string.Format(
                  "{0}\r\nExpected state: {1} - {2} - {3} \r\nActual state: {4} - {5} - {6}",
                  e.Message,
                  expectedStates[i].GetType().Name,
                  expectedStates[i].Message,
                  expectedStates[i].Sender,
                  _states[i].GetType().Name,
                  _states[i].Message,
                  _states[i].Sender));
        }
      }

      Assert.That(_states.Count, Is.EqualTo(expectedStates.Length), "Length");
    }

    public void Unregister ()
    {
      foreach (var domainObject in _domainObjects)
      {
        domainObject.Deleting -= DomainObject_Deleting;
        domainObject.Deleted -= DomainObject_Deleted;
        domainObject.PropertyChanging -= DomainObject_PropertyChanging;
        domainObject.PropertyChanged -= DomainObject_PropertyChanged;
        domainObject.RelationChanging -= DomainObject_RelationChanging;
        domainObject.RelationChanged -= DomainObject_RelationChanged;
      }

      foreach (var collection in _collections)
      {
        collection.Adding -= Collection_Changing;
        collection.Added -= Collection_Changed;
        collection.Removing -= Collection_Changing;
        collection.Removed -= Collection_Changed;
      }
    }

    public void DomainObject_PropertyChanged (object sender, PropertyChangeEventArgs args)
    {
      _states.Add(new PropertyChangeState(sender, args.PropertyDefinition, args.OldValue, args.NewValue));
    }

    public void DomainObject_PropertyChanging (object sender, PropertyChangeEventArgs args)
    {
      _states.Add(new PropertyChangeState(sender, args.PropertyDefinition, args.OldValue, args.NewValue));

      if (_states.Count == _cancelEventNumber)
        CancelOperation();
    }

    public void DomainObject_RelationChanging (object sender, RelationChangingEventArgs args)
    {
      _states.Add(new RelationChangeState(sender, args.RelationEndPointDefinition.PropertyName, args.OldRelatedObject, args.NewRelatedObject));

      if (_states.Count == _cancelEventNumber)
        CancelOperation();
    }

    public void DomainObject_RelationChanged (object sender, RelationChangedEventArgs args)
    {
      _states.Add(new RelationChangeState(sender, args.RelationEndPointDefinition.PropertyName, null, null));
    }

    public void DomainObject_Deleting (object sender, EventArgs args)
    {
      _states.Add(new ObjectDeletionState(sender));

      if (_states.Count == _cancelEventNumber)
        CancelOperation();
    }

    public void DomainObject_Deleted (object sender, EventArgs args)
    {
      _states.Add(new ObjectDeletionState(sender));
    }

    public void Collection_Changing (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      _states.Add(new CollectionChangeState(sender, args.DomainObject));

      if (_states.Count == _cancelEventNumber)
        CancelOperation();
    }

    public void Collection_Changed (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      _states.Add(new CollectionChangeState(sender, args.DomainObject));
    }

    public void Collection_Deleting (object sender, EventArgs args)
    {
      _states.Add(new CollectionDeletionState(sender));

      if (_states.Count == _cancelEventNumber)
        CancelOperation();
    }

    public void Collection_Deleted (object sender, EventArgs args)
    {
      _states.Add(new CollectionDeletionState(sender));
    }
  }
}
