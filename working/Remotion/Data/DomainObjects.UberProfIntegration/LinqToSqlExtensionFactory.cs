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
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UberProfIntegration
{
  /// <summary>
  /// Implements <see cref="IPersistenceExtensionFactory"/> for <b><a href="http://l2sprof.com/">Linq to Sql Profiler</a></b>. (Tested for build 661)
  /// <seealso cref="LinqToSqlAppenderProxy"/>
  /// </summary>
  public class LinqToSqlExtensionFactory : IPersistenceExtensionFactory, IClientTransactionExtensionFactory
  {
    public IEnumerable<IPersistenceExtension> CreatePersistenceExtensions (Guid clientTransactionID)
    {
      yield return new LinqToSqlExtension (clientTransactionID, LinqToSqlAppenderProxy.Instance);
    }

    public IEnumerable<IClientTransactionExtension> CreateClientTransactionExtensions (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      if (clientTransaction.ParentTransaction == null)
        yield return new LinqToSqlExtension (clientTransaction.ID, LinqToSqlAppenderProxy.Instance);
    }
  }
}