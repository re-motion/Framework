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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.StorageProviderCommands
{
  /// <summary>
  /// Executes a given <see cref="IRdbmsProviderCommand{T}"/> and associates the result with a given <see cref="ObjectID"/>, 
  /// checking whether the return value actually matches the expected <see cref="ObjectID"/>.
  /// </summary>
  public class SingleDataContainerAssociateWithIDCommand : IRdbmsProviderCommand<ObjectLookupResult<DataContainer>>
  {
    private readonly ObjectID _expectedObjectID;
    private readonly IRdbmsProviderCommand<DataContainer?> _innerCommand;

    public SingleDataContainerAssociateWithIDCommand (ObjectID expectedObjectID, IRdbmsProviderCommand<DataContainer?> innerCommand)
    {
      ArgumentUtility.CheckNotNull("expectedObjectID", expectedObjectID);
      ArgumentUtility.CheckNotNull("innerCommand", innerCommand);

      _expectedObjectID = expectedObjectID;
      _innerCommand = innerCommand;
    }

    public ObjectID ExpectedObjectID
    {
      get { return _expectedObjectID; }
    }

    public IRdbmsProviderCommand<DataContainer?> InnerCommand
    {
      get { return _innerCommand; }
    }

    public ObjectLookupResult<DataContainer> Execute (IRdbmsProviderCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull("executionContext", executionContext);

      var dataContainer = InnerCommand.Execute(executionContext);
      if (dataContainer != null && dataContainer.ID != _expectedObjectID)
      {
        var message = string.Format(
            "The ObjectID of the loaded DataContainer '{0}' and the expected ObjectID '{1}' differ.", dataContainer.ID, _expectedObjectID);
        throw new PersistenceException(message);
      }
      return new ObjectLookupResult<DataContainer>(_expectedObjectID, dataContainer);
    }
  }
}
