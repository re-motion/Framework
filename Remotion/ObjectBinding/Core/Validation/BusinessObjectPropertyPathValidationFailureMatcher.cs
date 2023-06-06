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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Validation
{
  /// <summary>
  ///   Implementation of <see cref="Remotion.ObjectBinding.Validation.IValidationFailureMatcher" /> based on individual
  ///   <see cref="BusinessObjectPropertyPath" /> instances.
  /// </summary>
  public class BusinessObjectPropertyPathValidationFailureMatcher : FailureMatcherBase, IValidationFailureMatcher
  {
    public IBusinessObjectPropertyPath PropertyPath { get; }

    public BusinessObjectPropertyPathValidationFailureMatcher (IBusinessObjectPropertyPath propertyPath)
    {
      ArgumentUtility.CheckNotNull(nameof(propertyPath), propertyPath);

      PropertyPath = propertyPath;
    }

    public override IReadOnlyCollection<BusinessObjectValidationFailure> GetMatchingValidationFailures (
        IBusinessObject businessObject,
        IBusinessObjectValidationResult validationResult)
    {
      ArgumentUtility.CheckNotNull(nameof(businessObject), businessObject);
      ArgumentUtility.CheckNotNull(nameof(validationResult), validationResult);

      return GetMatchingValidationFailures(PropertyPath, businessObject, validationResult);
    }
  }
}
