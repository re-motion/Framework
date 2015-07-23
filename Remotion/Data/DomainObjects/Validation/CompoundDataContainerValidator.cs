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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Validation
{
  /// <summary>
  /// Combines one or more <see cref="IDataContainerValidator"/>-instances and 
  /// delegates the validation to them.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor (typeof (IDataContainerValidator), RegistrationType = RegistrationType.Compound, Lifetime = LifetimeKind.Singleton)]
  public class CompoundDataContainerValidator : IDataContainerValidator
  {
    private readonly IReadOnlyList<IDataContainerValidator> _validators;

    public CompoundDataContainerValidator (IEnumerable<IDataContainerValidator> validators)
    {
      ArgumentUtility.CheckNotNull ("validators", validators);
      _validators = validators.ToList().AsReadOnly();
    }

    public void Validate (DataContainer dataContainer)
    {
      ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

      foreach (var validator in _validators)
        validator.Validate (dataContainer);
    }

    public IReadOnlyList<IDataContainerValidator> Validators
    {
      get { return _validators; }
    }
  }
}