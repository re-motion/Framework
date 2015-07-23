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
using System.Linq;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Combines one or more <see cref="IPersistableDataValidator"/>-instances and 
  /// delegates the validation to them.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor (typeof (IPersistableDataValidator), RegistrationType = RegistrationType.Compound, Lifetime = LifetimeKind.Singleton)]
  public class CompoundPersistableDataValidator : IPersistableDataValidator
  {
    private readonly IReadOnlyList<IPersistableDataValidator> _validators;

    public CompoundPersistableDataValidator (IEnumerable<IPersistableDataValidator> validators)
    {
      ArgumentUtility.CheckNotNull ("validators", validators);
      _validators = validators.ToList().AsReadOnly();
    }

    public void Validate (ClientTransaction clientTransaction, PersistableData persistableData)
    {
      ArgumentUtility.CheckNotNull ("clientTransaction", clientTransaction);
      ArgumentUtility.CheckNotNull ("persistableData", persistableData);
      
      foreach (var validator in _validators)
        validator.Validate (clientTransaction, persistableData);
    }

    public IReadOnlyList<IPersistableDataValidator> Validators
    {
      get { return _validators; }
    }
  }
}