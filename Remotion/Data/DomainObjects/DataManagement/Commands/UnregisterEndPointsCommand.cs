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
using System.Collections.ObjectModel;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.Commands
{
  /// <summary>
  /// Unregisters a set of end-points from a <see cref="RelationEndPointManager"/>.
  /// </summary>
  public class UnregisterEndPointsCommand : IDataManagementCommand
  {
    private readonly IRelationEndPoint[] _endPoints;
    private readonly IRelationEndPointRegistrationAgent _registrationAgent;
    private readonly RelationEndPointMap _map;

    public UnregisterEndPointsCommand (
        IEnumerable<IRelationEndPoint> endPoints, 
        IRelationEndPointRegistrationAgent registrationAgent, 
        RelationEndPointMap map)
    {
      ArgumentUtility.CheckNotNull ("endPoints", endPoints);
      ArgumentUtility.CheckNotNull ("registrationAgent", registrationAgent);
      ArgumentUtility.CheckNotNull ("map", map);

      _endPoints = endPoints.ToArray ();
      _registrationAgent = registrationAgent;
      _map = map;
    }

    public ReadOnlyCollection<IRelationEndPoint> EndPoints
    {
      get { return Array.AsReadOnly (_endPoints); }
    }

    public IRelationEndPointRegistrationAgent RegistrationAgent
    {
      get { return _registrationAgent; }
    }

    public IRelationEndPointMapReadOnlyView Map
    {
      get { return _map; }
    }

    public IEnumerable<Exception> GetAllExceptions ()
    {
      return new Exception[0];
    }

    public void Begin ()
    {
      // Nothing to do
    }

    public void Perform ()
    {
      foreach (var endPoint in _endPoints)
      {
        _registrationAgent.UnregisterEndPoint (endPoint, _map);
      }
    }

    public void End ()
    {
      // Nothing to do
    }

    public ExpandedCommand ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand (this);
    }
  }
}
