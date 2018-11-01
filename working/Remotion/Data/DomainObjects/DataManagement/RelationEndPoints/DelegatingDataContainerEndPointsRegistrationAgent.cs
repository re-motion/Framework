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

namespace Remotion.Data.DomainObjects.DataManagement.RelationEndPoints
{
  /// <summary>
  /// Delegates to <see cref="ExistingDataContainerEndPointsRegistrationAgent"/> and <see cref="NonExistingDataContainerEndPointsRegistrationAgent"/>,
  /// depending on the state of the given <see cref="DataContainer"/>.
  /// </summary>
  [Serializable]
  public class DelegatingDataContainerEndPointsRegistrationAgent : IDataContainerEndPointsRegistrationAgent
  {
    private readonly ExistingDataContainerEndPointsRegistrationAgent _existingDataContainerRegistrationAgent;
    private readonly NonExistingDataContainerEndPointsRegistrationAgent _nonExistingDataContainerRegistrationAgent;

    public DelegatingDataContainerEndPointsRegistrationAgent (IRelationEndPointFactory endPointFactory, IRelationEndPointRegistrationAgent registrationAgent)
    {
      ArgumentUtility.CheckNotNull ("endPointFactory", endPointFactory);
      ArgumentUtility.CheckNotNull ("registrationAgent", registrationAgent);

      _existingDataContainerRegistrationAgent = new ExistingDataContainerEndPointsRegistrationAgent (endPointFactory, registrationAgent);
      _nonExistingDataContainerRegistrationAgent = new NonExistingDataContainerEndPointsRegistrationAgent (endPointFactory, registrationAgent);
    }

    public void RegisterEndPoints (DataContainer dataContainer, RelationEndPointMap relationEndPointMap)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("relationEndPointMap", relationEndPointMap);

      var agent = ChooseAgent (dataContainer);
      agent.RegisterEndPoints (dataContainer, relationEndPointMap);
    }

    public IDataManagementCommand CreateUnregisterEndPointsCommand (DataContainer dataContainer, RelationEndPointMap relationEndPointMap)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);
      ArgumentUtility.CheckNotNull ("relationEndPointMap", relationEndPointMap);

      var agent = ChooseAgent (dataContainer);
      return agent.CreateUnregisterEndPointsCommand (dataContainer, relationEndPointMap);
    }

    private IDataContainerEndPointsRegistrationAgent ChooseAgent (DataContainer dataContainer)
    {
      switch (dataContainer.State)
      {
        case StateType.Changed:
        case StateType.Unchanged:
          return _existingDataContainerRegistrationAgent;
        case StateType.New:
        case StateType.Deleted:
          return _nonExistingDataContainerRegistrationAgent;
        default:
          throw new NotSupportedException ("Cannot register end-points for a DataContainer with state '" + dataContainer.State + "'.");
      }
    }

  }
}