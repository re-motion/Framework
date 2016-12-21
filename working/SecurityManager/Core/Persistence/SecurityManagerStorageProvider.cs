// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.SecurityManager.Domain;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Persistence
{
  public class SecurityManagerRdbmsProvider : RdbmsProvider
  {
    private readonly RevisionStorageProviderExtension _revisionExtension;

    public SecurityManagerRdbmsProvider (
        RdbmsProviderDefinition definition,
        IPersistenceExtension persistenceExtension,
        IStorageProviderCommandFactory<IRdbmsProviderCommandExecutionContext> commandFactory,
        Func<IDbConnection> connectionFactory)
        : base (
            definition,
            persistenceExtension,
            commandFactory,
            connectionFactory)
    {
      _revisionExtension = new RevisionStorageProviderExtension (
          SafeServiceLocator.Current.GetInstance<IDomainRevisionProvider>(),
          SafeServiceLocator.Current.GetInstance<IUserRevisionProvider>());
    }

    public override void Save (IEnumerable<DataContainer> dataContainers)
    {
      ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

      //TODO RM-5638: Refactor to Streaming-API
      var dataContainersList = dataContainers.ToList();
      base.Save (dataContainersList);
      _revisionExtension.Saved (Connection.WrappedInstance, Transaction.WrappedInstance, dataContainersList);
    }
  }
}