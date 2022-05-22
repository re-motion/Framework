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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence
{
  /// <summary>
  /// Holds the data needed to persist a <see cref="IDomainObject"/>.
  /// </summary>
  public class PersistableData
  {
    private readonly IDomainObject _domainObject;
    private readonly DomainObjectState _domainObjectState;
    private readonly DataContainer _dataContainer;
    private readonly IEnumerable<IRelationEndPoint> _associatedEndPointSequence;

    public PersistableData (
        IDomainObject domainObject,
        DomainObjectState domainObjectState,
        DataContainer dataContainer,
        IEnumerable<IRelationEndPoint> associatedEndPointSequence)
    {
      ArgumentUtility.CheckNotNull("domainObject", domainObject);
      ArgumentUtility.CheckNotNull("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull("associatedEndPointSequence", associatedEndPointSequence);

      _domainObject = domainObject;
      _domainObjectState = domainObjectState;
      _dataContainer = dataContainer;
      _associatedEndPointSequence = associatedEndPointSequence;
    }

    public IDomainObject DomainObject
    {
      get { return _domainObject; }
    }

    public DomainObjectState DomainObjectState
    {
      get { return _domainObjectState; }
    }

    public DataContainer DataContainer
    {
      get { return _dataContainer; }
    }

    public IEnumerable<IRelationEndPoint> GetAssociatedEndPoints ()
    {
      return _associatedEndPointSequence;
    }
  }
}
