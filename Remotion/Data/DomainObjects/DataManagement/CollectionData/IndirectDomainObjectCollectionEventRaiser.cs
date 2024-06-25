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

namespace Remotion.Data.DomainObjects.DataManagement.CollectionData
{
  /// <summary>
  /// Implements the <see cref="IDomainObjectCollectionEventRaiser"/> interface by delegating to another implementation of 
  /// <see cref="IDomainObjectCollectionEventRaiser"/>. This is useful when the event raiser needs to be changed after e.g. the 
  /// <see cref="EventRaisingDomainObjectCollectionDataDecorator"/> has been created.
  /// </summary>
  public class IndirectDomainObjectCollectionEventRaiser : IDomainObjectCollectionEventRaiser
  {
    public IndirectDomainObjectCollectionEventRaiser ()
    {
    }

    public IndirectDomainObjectCollectionEventRaiser (IDomainObjectCollectionEventRaiser eventRaiser)
    {
      EventRaiser = eventRaiser;
    }

    public IDomainObjectCollectionEventRaiser? EventRaiser { get; set; }

    public void BeginAdd (int index, DomainObject domainObject)
    {
      GetAndCheckEventRaiser().BeginAdd(index, domainObject);
    }

    public void EndAdd (int index, DomainObject domainObject)
    {
      GetAndCheckEventRaiser().EndAdd(index, domainObject);
    }

    public void BeginRemove (int index, DomainObject domainObject)
    {
      GetAndCheckEventRaiser().BeginRemove(index, domainObject);
    }

    public void EndRemove (int index, DomainObject domainObject)
    {
      GetAndCheckEventRaiser().EndRemove(index, domainObject);
    }

    public void BeginDelete ()
    {
      GetAndCheckEventRaiser().BeginDelete();
    }

    public void EndDelete ()
    {
      GetAndCheckEventRaiser().EndDelete();
    }

    public void WithinReplaceData ()
    {
      GetAndCheckEventRaiser().WithinReplaceData();
    }

    private IDomainObjectCollectionEventRaiser GetAndCheckEventRaiser ()
    {
      if (EventRaiser == null)
      {
        throw new InvalidOperationException("The actual EventRaiser must be set before IndirectDomainObjectCollectionEventRaiser can be used.");
      }
      return EventRaiser;
    }
  }
}
