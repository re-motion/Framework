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
using Remotion.Security.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Security
{
  /// <summary>
  /// Implementation of the <see cref="ITransactionFactory"/> interface that creates root <see cref="ClientTransaction"/>s and adds a
  /// <see cref="SecurityClientTransactionExtension"/> when the transaction is created in an application that has a security provider configured.
  /// </summary>
  /// <threadsafety static="true" instance="true"/>
  [Serializable]
  public class SecurityClientTransactionFactory : ClientTransactionFactory
  {
    protected override void OnTransactionCreated (ClientTransaction transaction)
    {
      ArgumentUtility.CheckNotNull("transaction", transaction);

      if (!SecurityConfiguration.Current.DisableAccessChecks)
        transaction.Extensions.Add(new SecurityClientTransactionExtension());
    }
  }
}
