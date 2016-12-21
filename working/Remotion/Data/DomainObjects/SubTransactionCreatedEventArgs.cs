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

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Provides data for the <see cref="ClientTransaction.SubTransactionCreated"/> event.
  /// </summary>
  public class SubTransactionCreatedEventArgs : EventArgs
  {
    private readonly ClientTransaction _subTransaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="SubTransactionCreatedEventArgs"/> class.
    /// </summary>
    /// <param name="subTransaction">The subtransaction created.</param>
    public SubTransactionCreatedEventArgs (ClientTransaction subTransaction)
    {
      ArgumentUtility.CheckNotNull ("subTransaction", subTransaction);
      _subTransaction = subTransaction;
    }

    /// <summary>
    /// Gets the subtransaction created.
    /// </summary>
    /// <value>The new subtransaction for which the event was raised.</value>
    public ClientTransaction SubTransaction
    {
      get { return _subTransaction; }
    }
  }
}