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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Implementation of the <see cref="IClientTransactionExtensionFactory"/> interface. 
  /// Registers the <see cref="CommitValidationClientTransactionExtension"/> for root <see cref="ClientTransaction"/>s.
  /// </summary>
  [ImplementationFor (typeof(IClientTransactionExtensionFactory), Position = Position, Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple)]
  public class CommitValidationClientTransactionExtensionFactory : IClientTransactionExtensionFactory
  {
    private readonly IPersistableDataValidator _persistableDataValidator;
    public const int Position = 0;

    public CommitValidationClientTransactionExtensionFactory (IPersistableDataValidator persistableDataValidator)
    {
      ArgumentUtility.CheckNotNull ("persistableDataValidator", persistableDataValidator);
      
      _persistableDataValidator = persistableDataValidator;
    }

    public IEnumerable<IClientTransactionExtension> CreateClientTransactionExtensions (ClientTransaction clientTransaction)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);

      if (clientTransaction.RootTransaction == clientTransaction)
        yield return new CommitValidationClientTransactionExtension (_persistableDataValidator);
    }
  }
}