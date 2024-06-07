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
using System.Data;
using Remotion.Data.DomainObjects.DataManagement;
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
        string connectionString,
        IPersistenceExtension persistenceExtension,
        IRdbmsProviderCommandFactory commandFactory,
        Func<IDbConnection> connectionFactory)
        : base(
            definition,
            connectionString,
            persistenceExtension,
            commandFactory,
            connectionFactory)
    {
      _revisionExtension = new RevisionStorageProviderExtension(
          SafeServiceLocator.Current.GetInstance<IDomainRevisionProvider>(),
          SafeServiceLocator.Current.GetInstance<IUserRevisionProvider>(),
          SafeServiceLocator.Current.GetInstance<IUserNamesRevisionProvider>(),
          commandFactory);
    }

    public override void Save (IReadOnlyCollection<DataContainer> dataContainers)
    {
      ArgumentUtility.CheckNotNull("dataContainers", dataContainers);

      var dataContainersList = dataContainers;
      base.Save(dataContainersList);
      _revisionExtension.Saved(this, dataContainersList);
    }
  }
}
