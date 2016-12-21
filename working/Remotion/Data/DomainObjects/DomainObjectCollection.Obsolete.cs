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

namespace Remotion.Data.DomainObjects
{
  public partial class DomainObjectCollection
  {
    [Obsolete (
    "This method is obsolete. Directly call the constructor DomainObjectCollection (IEnumerable<DomainObject>, Type) or use DomainObjectCollectionFactory. (1.13.37)",
    true)]
    public static DomainObjectCollection Create (Type collectionType)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This method is obsolete. Directly call the constructor DomainObjectCollection (IEnumerable<DomainObject>, Type) or use DomainObjectCollectionFactory. (1.13.37)",
        true)]
    public static DomainObjectCollection Create (Type collectionType, Type requiredItemType)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This method is obsolete. Directly call the constructor DomainObjectCollection (IEnumerable<DomainObject>, Type) or use DomainObjectCollectionFactory. (1.13.37)",
        true)]
    public static DomainObjectCollection Create (Type collectionType, IEnumerable<DomainObject> contents)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This method is obsolete. Directly call the constructor DomainObjectCollection (IEnumerable<DomainObject>, Type) or use DomainObjectCollectionFactory. (1.13.37)",
        true)]
    public static DomainObjectCollection Create (
        Type collectionType,
        IEnumerable<DomainObject> contents,
        Type requiredItemType)
    {
      throw new NotImplementedException ();
    }

    [Obsolete ("This method has been renamed and moved. Use UnionWith (extension method declared on DomainObjectCollectionExtensions) instead. (1.13.37)", true)]
    public void Combine (DomainObjectCollection domainObjects)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This method has been renamed and moved. Use GetItemsExcept (extension method declared on DomainObjectCollectionExtensions) instead. (1.13.37)"
        + "Note that the comparison is now based on IDs and that the order of arguments has been reversed for clarity.", true)]
    public DomainObjectCollection GetItemsNotInCollection (DomainObjectCollection domainObjects)
    {
      throw new NotImplementedException ();
    }

    // ReSharper disable UnusedParameter.Local
    [Obsolete (
       "This constructor has been removed. Use the constructor taking IEnumerable<DomainObject> (or Clone) to copy a collection, use AsReadOnly to "
       + "get a read-only version of a collection. (1.13.37)", true)]
    public DomainObjectCollection (DomainObjectCollection collection, bool makeCollectionReadOnly)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This constructor has been removed. Use the constructor taking IEnumerable<DomainObject> (or Clone) to copy a collection, use AsReadOnly to "
        + "get a read-only version of a collection. (1.13.37)", true)]
    public DomainObjectCollection (IEnumerable<DomainObject> domainObjects, Type requiredItemType, bool makeCollectionReadOnly)
    {
      throw new NotImplementedException ();
    }
    // ReSharper restore UnusedParameter.Local

    [Obsolete (
    "This method has been removed. Use SequenceEqual (extension method defined on DomainObjectCollectionExtensions) instead. Note that that "
    + "method does not handle null arguments. (1.13.37)", true)]
    public static bool Compare (DomainObjectCollection collection1, DomainObjectCollection collection2)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This method has been removed. Use SequenceEqual or SetEquals (extension methods defined on DomainObjectCollectionExtensions) instead. "
        + "Note that those methods do not handle null arguments. (1.13.37)", true)]
    public static bool Compare (DomainObjectCollection collection1, DomainObjectCollection collection2, bool ignoreItemOrder)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This method has been removed. ReplaceItemsWithoutNotifications can be used to replace the "
        + "contents of a collection without raising events. (1.13.37)", true)]
    protected internal virtual void ReplaceItems (DomainObjectCollection domainObjects)
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This method has been removed. Use AsReadOnlyCollection (extension method defined on DomainObjectCollectionExtensions) to create a read-only "
        + "wrapper for a DomainObjectCollection. For a more featured wrapper, create a ReadOnlyDomainObjectCollectionAdapter. To create a new "
        + "read-only DomainObjectCollection with a copy of the original collection's data, use Clone (true). (1.13.48)", true)]
    public DomainObjectCollection AsReadOnly ()
    {
      throw new NotImplementedException ();
    }

    [Obsolete (
        "This method has been removed. Create a new DomainObjectCollection via Clone (true) or Clone (false) rather than changing the read-only "
        + "property of an existing collection. (1.13.48)", true)]
    protected void SetIsReadOnly (bool isReadOnly)
    {
      throw new NotImplementedException ();
    }

    [Obsolete ("This method has been removed. Use/override ReplaceItemsWithoutNotifications instead. (1.13.65)", true)]
    protected internal virtual void Rollback (DomainObjectCollection originalDomainObjects)
    {
      throw new NotImplementedException ();
    }

    [Obsolete ("This method has been removed. Use ReplaceItemsWithoutNotifications instead. (1.13.65)", true)]
    protected internal virtual void Commit (IEnumerable<DomainObject> domainObjects)
    {
      throw new NotImplementedException ();
    }
  }
}