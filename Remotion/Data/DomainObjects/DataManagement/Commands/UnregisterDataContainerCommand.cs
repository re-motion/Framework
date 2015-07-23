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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  /// <summary>
  /// Unregisters a <see cref="DataContainer"/> from a <see cref="DataContainerMap"/>.
  /// </summary>
  public class UnregisterDataContainerCommand : IDataManagementCommand
  {
    private readonly ObjectID _objectID;
    private readonly DataContainerMap _map;

    public UnregisterDataContainerCommand (ObjectID objectID, DataContainerMap map)
    {
      ArgumentUtility.CheckNotNull ("objectID", objectID);
      ArgumentUtility.CheckNotNull ("map", map);

      _objectID = objectID;
      _map = map;
    }

    public ObjectID ObjectID
    {
      get { return _objectID; }
    }

    public DataContainerMap Map
    {
      get { return _map; }
    }

    public IEnumerable<Exception> GetAllExceptions ()
    {
      return new Exception[0];
    }

    public void Begin ()
    {
      // Nothing to do here
    }

    public void Perform ()
    {
      _map.Remove (_objectID);
    }

    public void End ()
    {
    }

    public ExpandedCommand ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand (this);
    }
  }
}
