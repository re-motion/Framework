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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  /// <summary>
  /// Represents a command that touches, but does not change the modified end point.
  /// </summary>
  public class RelationEndPointTouchCommand : IDataManagementCommand
  {
    private readonly IRelationEndPoint _endPoint;

    public RelationEndPointTouchCommand (IRelationEndPoint endPoint)
    {
      _endPoint = endPoint;
    }

    public IRelationEndPoint EndPoint
    {
      get { return _endPoint; }
    }

    public IEnumerable<Exception> GetAllExceptions ()
    {
      return Enumerable.Empty<Exception> ();
    }

    public void Begin ()
    {
      // do not issue any notifications
    }

    public void Perform ()
    {
      _endPoint.Touch ();
    }

    public void End ()
    {
      // do not issue any notifications
    }

    ExpandedCommand IDataManagementCommand.ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand (this);
    }
  }
}