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

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  public class DomainObjectCollectionEventReceiver : EventReceiverBase
  {
    // types

    // static members and constants

    // member fields

    private DomainObject _addingDomainObject;
    private DomainObject _addedDomainObject;

    private DomainObjectCollection _removingDomainObjects;
    private DomainObjectCollection _removedDomainObjects;

    // construction and disposing

    public DomainObjectCollectionEventReceiver (DomainObjectCollection collection)
      : this(collection, false)
    {
    }

    public DomainObjectCollectionEventReceiver (DomainObjectCollection collection, bool cancel)
    {
      Cancel = cancel;

      collection.Adding += DomainObjectCollection_Adding;
      collection.Added += DomainObjectCollection_Added;

      collection.Removing += DomainObjectCollection_Removing;
      collection.Removed += DomainObjectCollection_Removed;

      collection.Deleting += DomainObjectCollection_Deleting;
      collection.Deleted += DomainObjectCollection_Deleted;

      Reset();
    }

    // methods and properties

    public void Reset ()
    {
      _addingDomainObject = null;
      _addedDomainObject = null;
      HasAddingEventBeenCalled = false;
      HasAddedEventBeenCalled = false;

      _removingDomainObjects = new DomainObjectCollection();
      _removedDomainObjects = new DomainObjectCollection();
      HasRemovingEventBeenCalled = false;
      HasRemovedEventBeenCalled = false;

      HasDeletingEventBeenCalled = false;
      HasDeletedEventBeenCalled = false;
    }

    public bool Cancel { get; set; }

    public DomainObject AddingDomainObject
    {
      get { return _addingDomainObject; }
    }

    public DomainObject AddedDomainObject
    {
      get { return _addedDomainObject; }
    }

    public bool HasAddingEventBeenCalled { get; private set; }
    public bool HasAddedEventBeenCalled { get; private set; }

    public DomainObjectCollection RemovingDomainObjects
    {
      get { return _removingDomainObjects; }
    }

    public DomainObjectCollection RemovedDomainObjects
    {
      get { return _removedDomainObjects; }
    }

    public bool HasRemovingEventBeenCalled { get; private set; }
    public bool HasRemovedEventBeenCalled { get; private set; }

    public bool HasDeletingEventBeenCalled { get; private set; }
    public bool HasDeletedEventBeenCalled { get; private set; }

    private void DomainObjectCollection_Adding (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      HasAddingEventBeenCalled = true;
      _addingDomainObject = args.DomainObject;

      if (Cancel)
        CancelOperation();
    }

    private void DomainObjectCollection_Added (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      _addedDomainObject = args.DomainObject;
      HasAddedEventBeenCalled = true;
    }

    private void DomainObjectCollection_Removing (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      HasRemovingEventBeenCalled = true;
      _removingDomainObjects.Add(args.DomainObject);

      if (Cancel)
        CancelOperation();
    }

    private void DomainObjectCollection_Removed (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      _removedDomainObjects.Add(args.DomainObject);
      HasRemovedEventBeenCalled = true;
    }

    private void DomainObjectCollection_Deleting (object sender, EventArgs args)
    {
      HasDeletingEventBeenCalled = true;

      if (Cancel)
        CancelOperation();
    }

    private void DomainObjectCollection_Deleted (object sender, EventArgs args)
    {
      HasDeletedEventBeenCalled = true;
    }
  }
}
