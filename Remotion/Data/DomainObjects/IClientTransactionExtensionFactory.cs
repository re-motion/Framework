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
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects
{
  /// <summary>
  /// Defines an interface for creating <see cref="IClientTransactionExtension"/> objects.
  /// </summary>
  /// <remarks>
  /// Register implementations of this interface with the <see cref="ServiceLocator.Current"/> <see cref="IServiceLocator"/>. When a new 
  /// <see cref="ClientTransaction"/> is created, the factories are iterated and the created listeners will be added to the 
  /// <see cref="ClientTransaction"/>.
  /// </remarks>
  public interface IClientTransactionExtensionFactory
  {
    /// <summary>
    /// Creates <see cref="IClientTransactionExtension"/> objects to be added to the given <see cref="ClientTransaction"/>.
    /// </summary>
    /// <param name="clientTransaction">The <see cref="ClientTransaction"/> that will receive the extensions. Note that this object is currently
    /// being constructed and may not yet be ready for use.
    /// </param>
    /// <returns>
    /// A sequence of <see cref="IClientTransactionExtension"/> instances that should be added to the <paramref name="clientTransaction"/>.
    /// </returns>
    /// <remarks>
    /// <see cref="CreateClientTransactionExtensions"/> is invoked while the <paramref name="clientTransaction"/>'s constructor is executing, but has
    /// not yet finished. No <see cref="DomainObject"/> instances should be accessed in the context of the <paramref name="clientTransaction"/> while 
    /// the method is running.  The <see cref="ClientTransaction.ParentTransaction"/> property can safely be accessed.
    /// </remarks>
    IEnumerable<IClientTransactionExtension> CreateClientTransactionExtensions (ClientTransaction clientTransaction);
  }
}
