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
using Remotion.Data.DomainObjects.DataManagement.CollectionData;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects
{
  public partial class DomainObjectCollection : IDomainObjectCollectionEventRaiser
  {
    void IDomainObjectCollectionEventRaiser.BeginAdd (int index, IDomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      OnAdding(new DomainObjectCollectionChangeEventArgs(domainObject));
    }

    void IDomainObjectCollectionEventRaiser.EndAdd (int index, IDomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      OnAdded(new DomainObjectCollectionChangeEventArgs(domainObject));
    }

    void IDomainObjectCollectionEventRaiser.BeginRemove (int index, IDomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      OnRemoving(new DomainObjectCollectionChangeEventArgs(domainObject));
    }

    void IDomainObjectCollectionEventRaiser.EndRemove (int index, IDomainObject domainObject)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      OnRemoved(new DomainObjectCollectionChangeEventArgs(domainObject));
    }

    void IDomainObjectCollectionEventRaiser.BeginDelete ()
    {
      OnDeleting();
    }

    void IDomainObjectCollectionEventRaiser.EndDelete ()
    {
      OnDeleted();
    }

    void IDomainObjectCollectionEventRaiser.WithinReplaceData ()
    {
      OnReplaceData();
    }
  }
}
