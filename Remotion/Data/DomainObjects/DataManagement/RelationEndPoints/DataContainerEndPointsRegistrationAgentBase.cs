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
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Provices typical implementations of <see cref="RegisterEndPoints"/> and <see cref="CreateUnregisterEndPointsCommand"/> with template methods,
  /// <see cref="GetOwnedEndPointIDs"/> and <see cref="GetUnregisterProblem"/>, to be implemented by concrete implementations.
  /// </summary>
  public abstract class DataContainerEndPointsRegistrationAgentBase : IDataContainerEndPointsRegistrationAgent
  {
    private readonly IRelationEndPointFactory _endPointFactory;
    private readonly IRelationEndPointRegistrationAgent _registrationAgent;

    protected DataContainerEndPointsRegistrationAgentBase (
        IRelationEndPointFactory endPointFactory, IRelationEndPointRegistrationAgent registrationAgent)
    {
      ArgumentUtility.CheckNotNull ("endPointFactory", endPointFactory);
      ArgumentUtility.CheckNotNull ("registrationAgent", registrationAgent);

      _endPointFactory = endPointFactory;
      _registrationAgent = registrationAgent;
    }

    public IRelationEndPointFactory EndPointFactory
    {
      get { return _endPointFactory; }
    }

    public IRelationEndPointRegistrationAgent RegistrationAgent
    {
      get { return _registrationAgent; }
    }

    protected abstract IEnumerable<RelationEndPointID> GetOwnedEndPointIDs (DataContainer dataContainer);
    protected abstract string GetUnregisterProblem (IRelationEndPoint endPoint, RelationEndPointMap relationEndPointMap);

    public void RegisterEndPoints (DataContainer dataContainer, RelationEndPointMap relationEndPointMap)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("relationEndPointMap", relationEndPointMap);

      foreach (var id in GetOwnedEndPointIDs (dataContainer))
      {
        var endPoint = id.Definition.IsVirtual
                           ? (IRelationEndPoint) _endPointFactory.CreateVirtualEndPoint (id, true)
                           : _endPointFactory.CreateRealObjectEndPoint (id, dataContainer);
        _registrationAgent.RegisterEndPoint (endPoint, relationEndPointMap);
      }
    }

    public IDataManagementCommand CreateUnregisterEndPointsCommand (DataContainer dataContainer, RelationEndPointMap relationEndPointMap)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("relationEndPointMap", relationEndPointMap);

      var loadedEndPoints = new List<IRelationEndPoint>();
      var problems = new List<string>();

      foreach (var endPointID in GetOwnedEndPointIDs (dataContainer))
      {
        var endPoint = relationEndPointMap[endPointID];
        if (endPoint != null)
        {
          loadedEndPoints.Add (endPoint);

          var problem = GetUnregisterProblem (endPoint, relationEndPointMap);
          if (problem != null)
            problems.Add (problem);
        }
      }

      if (problems.Count > 0)
      {
        var message = string.Format (
            "The relations of object '{0}' cannot be unloaded."
            + Environment.NewLine
            + "{1}",
            dataContainer.ID,
            string.Join (Environment.NewLine, problems));
        return new ExceptionCommand (new InvalidOperationException (message));
      }
      else
        return new UnregisterEndPointsCommand (loadedEndPoints, _registrationAgent, relationEndPointMap);
    }
  }
}