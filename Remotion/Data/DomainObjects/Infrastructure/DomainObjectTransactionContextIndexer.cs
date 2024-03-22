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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides an indexing property to access a <see cref="DomainObject"/>'s transaction-dependent context for a specific <see cref="ClientTransaction"/>.
  /// </summary>
  public readonly struct DomainObjectTransactionContextIndexer
  {
    private readonly DomainObjectTransactionContextImplementation _transactionContextImplementation;

    public DomainObjectTransactionContextIndexer (DomainObjectTransactionContextImplementation transactionContextImplementation)
    {
      ArgumentUtility.DebugCheckNotNull("transactionContextImplementation", transactionContextImplementation);

      _transactionContextImplementation = transactionContextImplementation;
    }

    /// <exception cref="ClientTransactionsDifferException">The object cannot be used in the given transaction.</exception>
    public DomainObjectTransactionContext this[ClientTransaction clientTransaction] => new(_transactionContextImplementation, clientTransaction);
  }
}
