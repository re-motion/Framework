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
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Combines one or more <see cref="ICollectorValidator"/>-instances. When calling <see cref="CheckValid"/>, all combined 
  /// <see cref="ICollectorValidator"/> instances must confirm that an <see cref="IComponentValidationCollector"/> instance
  /// is valid.
  /// </summary>
  /// <threadsafety static="true" instance="true" />
  [ImplementationFor (typeof (ICollectorValidator), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Compound)]
  public class CompoundCollectorValidator : ICollectorValidator
  {
    private readonly IReadOnlyCollection<ICollectorValidator> _collectorValidators;

    public CompoundCollectorValidator (IEnumerable<ICollectorValidator> collectorValidators)
    {
      ArgumentUtility.CheckNotNull ("collectorValidators", collectorValidators);

      _collectorValidators = collectorValidators.ToList().AsReadOnly();
    }

    public IReadOnlyCollection<ICollectorValidator> CollectorValidators
    {
      get { return _collectorValidators; }
    }

    public void CheckValid (IComponentValidationCollector collector)
    {
      ArgumentUtility.CheckNotNull ("collector", collector);

      foreach (var collectorValidator in _collectorValidators)
        collectorValidator.CheckValid (collector);
     }
  }
}