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
using Remotion.ObjectBinding.BusinessObjectPropertyConstraints;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Validators;

namespace Remotion.ObjectBinding.Validation
{
  [ImplementationFor(
      typeof(IPropertyValidatorToBusinessObjectPropertyConstraintConverter),
      Position = Position,
      RegistrationType = RegistrationType.Multiple,
      Lifetime = LifetimeKind.Singleton)]
  public class PropertyValidatorToBusinessObjectPropertyConstraintConverter : IPropertyValidatorToBusinessObjectPropertyConstraintConverter
  {
    public const int Position = 0;

    public PropertyValidatorToBusinessObjectPropertyConstraintConverter ()
    {
    }

    public IEnumerable<IBusinessObjectPropertyConstraint> Convert (IReadOnlyCollection<IPropertyValidator> propertyValidators)
    {
      ArgumentUtility.CheckNotNull("propertyValidators", propertyValidators);

      if (propertyValidators.OfType<IRequiredValidator>().Any())
        yield return new BusinessObjectPropertyValueRequiredConstraint();

      var maximumLength = propertyValidators.OfType<IMaximumLengthValidator>().Select(v => (int?)v.Max).Min();

      if (maximumLength.HasValue)
        yield return new BusinessObjectPropertyValueLengthConstraint(maximumLength.Value);
    }
  }
}
