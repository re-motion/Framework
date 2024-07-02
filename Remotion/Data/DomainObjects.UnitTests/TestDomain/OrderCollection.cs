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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;

namespace Remotion.Data.DomainObjects.UnitTests.TestDomain
{
  public class OrderCollection : ObjectList<Order>
  {
    public interface ICollectionEventReceiver
    {
      void OnAdding (DomainObjectCollectionChangeEventArgs args);
      void OnAdded (DomainObjectCollectionChangeEventArgs args);
      void OnRemoving (DomainObjectCollectionChangeEventArgs args);
      void OnRemoved (DomainObjectCollectionChangeEventArgs args);

      void OnDeleting ();
      void OnDeleted ();

      void OnReplaceData ();
    }

    private ICollectionEventReceiver _eventReceiver;

    public OrderCollection ()
    {
    }

    // standard constructor for collections
    public OrderCollection (IEnumerable<Order> contents)
      : base(contents)
    {
    }

    public OrderCollection (IDomainObjectCollectionData dataStrategy)
        : base(dataStrategy)
    {
    }

    public void SetEventReceiver (ICollectionEventReceiver eventReceiver)
    {
      _eventReceiver = eventReceiver;
    }

    protected override void OnAdding (DomainObjectCollectionChangeEventArgs args)
    {
      base.OnAdding(args);
      if (_eventReceiver != null)
        _eventReceiver.OnAdding(args);
    }

    protected override void OnAdded (DomainObjectCollectionChangeEventArgs args)
    {
      base.OnAdded(args);
      if (_eventReceiver != null)
        _eventReceiver.OnAdded(args);
    }

    protected override void OnRemoving (DomainObjectCollectionChangeEventArgs args)
    {
      base.OnRemoving(args);
      if (_eventReceiver != null)
        _eventReceiver.OnRemoving(args);
    }

    protected override void OnRemoved (DomainObjectCollectionChangeEventArgs args)
    {
      base.OnRemoved(args);
      if (_eventReceiver != null)
        _eventReceiver.OnRemoved(args);
    }

    protected override void OnDeleting ()
    {
      base.OnDeleting();
      if (_eventReceiver != null)
        _eventReceiver.OnDeleting();
    }

    protected override void OnDeleted ()
    {
      base.OnDeleted();
      if (_eventReceiver != null)
        _eventReceiver.OnDeleted();
    }

    protected override void OnReplaceData ()
    {
      base.OnReplaceData();
      if (_eventReceiver != null)
        _eventReceiver.OnReplaceData();
    }

    public new void Sort (Comparison<DomainObject> comparison)
    {
#pragma warning disable 612,618
      base.Sort(comparison);
#pragma warning restore 612,618
    }
  }
}
