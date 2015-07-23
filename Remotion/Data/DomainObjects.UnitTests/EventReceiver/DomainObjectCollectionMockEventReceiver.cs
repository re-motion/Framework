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
using Remotion.Utilities;
using Rhino.Mocks;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;
using Mocks_Property = Rhino.Mocks.Constraints.Property;

namespace Remotion.Data.DomainObjects.UnitTests.EventReceiver
{
  public abstract class DomainObjectCollectionMockEventReceiver
  {
    private readonly DomainObjectCollection _domainObjectCollection;
    // types

    // static members and constants

    // member fields

    // construction and disposing

    protected DomainObjectCollectionMockEventReceiver (DomainObjectCollection domainObjectCollection)
    {
      ArgumentUtility.CheckNotNull ("domainObjectCollection", domainObjectCollection);

      _domainObjectCollection = domainObjectCollection;

      domainObjectCollection.Added += Added;
      domainObjectCollection.Adding += Adding;
      domainObjectCollection.Removed += Removed;
      domainObjectCollection.Removing += Removing;
      domainObjectCollection.Deleted += Deleted;
      domainObjectCollection.Deleting += Deleting;
    }

    // abstract methods and properties

    protected abstract void Added (object sender, DomainObjectCollectionChangeEventArgs args);
    protected abstract void Adding (object sender, DomainObjectCollectionChangeEventArgs args);
    protected abstract void Removed (object sender, DomainObjectCollectionChangeEventArgs args);
    protected abstract void Removing (object sender, DomainObjectCollectionChangeEventArgs args);
    protected abstract void Deleting (object sender, EventArgs args);
    protected abstract void Deleted (object sender, EventArgs args);

    public void Adding (object sender, DomainObject domainObject)
    {
      Adding (null, (DomainObjectCollectionChangeEventArgs) null);
      LastCall.Constraints (Mocks_Is.Same (sender), Mocks_Property.Value ("DomainObject", domainObject));
    }

    public void Adding (DomainObject domainObject)
    {
      Adding (_domainObjectCollection, domainObject);
    }

    public void Adding ()
    {
      Adding (Arg<object>.Is.Anything, Arg<DomainObjectCollectionChangeEventArgs>.Is.Anything);
    }

    public void Added (object sender, DomainObject domainObject)
    {
      Added (null, (DomainObjectCollectionChangeEventArgs) null);
      LastCall.Constraints (Mocks_Is.Same (sender), Mocks_Property.Value ("DomainObject", domainObject));
    }

    public void Added (DomainObject domainObject)
    {
      Added (_domainObjectCollection, domainObject);
    }

    public void Added ()
    {
      Added (Arg<object>.Is.Anything, Arg<DomainObjectCollectionChangeEventArgs>.Is.Anything);
    }

    public void Removing (object sender, DomainObject domainObject)
    {
      Removing (null, (DomainObjectCollectionChangeEventArgs) null);
      LastCall.Constraints (Mocks_Is.Same (sender), Mocks_Property.Value ("DomainObject", domainObject));
    }

    public void Removing (DomainObject domainObject)
    {
      Removing (_domainObjectCollection, domainObject);
    }

    public void Removing ()
    {
      Removing (Arg<object>.Is.Anything, Arg<DomainObjectCollectionChangeEventArgs>.Is.Anything);
    }

    public void Removed (object sender, DomainObject domainObject)
    {
      Removed (null, (DomainObjectCollectionChangeEventArgs) null);
      LastCall.Constraints (Mocks_Is.Same (sender), Mocks_Property.Value ("DomainObject", domainObject));
    }

    public void Removed (DomainObject domainObject)
    {
      Removed (_domainObjectCollection, domainObject);
    }

    public void Removed ()
    {
      Removed (Arg<object>.Is.Anything, Arg<DomainObjectCollectionChangeEventArgs>.Is.Anything);
    }

    public void Deleting ()
    {
      Deleting (Arg.Is (_domainObjectCollection), Arg<EventArgs>.Is.Anything);
    }

    public void Deleted ()
    {
      Deleted (Arg.Is (_domainObjectCollection), Arg<EventArgs>.Is.Anything);
    }
  }
}
