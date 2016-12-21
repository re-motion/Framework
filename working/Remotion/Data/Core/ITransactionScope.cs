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

namespace Remotion.Data
{
  /// <summary>
  /// Represents a transaction scope, ie. an execution region where a certain <cref see="ITransaction"/> is the current transaction.
  /// </summary>
  public interface ITransactionScope
  {
    /// <summary>
    /// Gets a flag that describes whether this is the active scope.
    /// </summary>
    bool IsActiveScope { get; }

    /// <summary>
    /// Gets the transaction managed by this scope.
    /// </summary>
    /// <value>The scoped transaction.</value>
    ITransaction ScopedTransaction { get; }

    /// <summary>
    /// Leaves the scope, which means that <see cref="ScopedTransaction"/> is no loner the current transaction. 
    /// </summary>
    /// <remarks>
    /// This method reactivates the scope surrounding this scope. If no surrounding scope exists, there is no current transaction after this method 
    /// is executed.
    /// </remarks>
    void Leave ();
  }
}
