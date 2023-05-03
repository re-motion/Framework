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
  ///   Abstract base class of <see cref="BusinessObjectPropertyPathValidationFailureMatcher" /> and
  ///   <see cref="CompoundBusinessObjectPropertyPathBasedValidationFailureMatcher" />
  ///   used to reduce code duplication.
  /// </summary>
  public abstract class FailureMatcherBase : IValidationFailureMatcher
  {
    public abstract IReadOnlyCollection<BusinessObjectValidationFailure> GetMatchingValidationFailures (
        IBusinessObject businessObject,
        IBusinessObjectValidationResult validationResult);

    protected IReadOnlyCollection<BusinessObjectValidationFailure> GetMatchingValidationFailures (
        IBusinessObjectPropertyPath propertyPath,
        IBusinessObject businessObject,
        IBusinessObjectValidationResult validationResult)
    {
      ArgumentUtility.CheckNotNull(nameof(propertyPath), propertyPath);
      ArgumentUtility.CheckNotNull(nameof(businessObject), businessObject);
      ArgumentUtility.CheckNotNull(nameof(validationResult), validationResult);

      var result = propertyPath.GetResult(
          businessObject,
          BusinessObjectPropertyPath.UnreachableValueBehavior.ReturnNullForUnreachableValue,
          BusinessObjectPropertyPath.ListValueBehavior.FailForListProperties);

      var resultObject = result.ResultObject;
      if (resultObject == null)
        return Array.Empty<BusinessObjectValidationFailure>();

      var businessObjectProperty = result.ResultProperty;
      if (businessObjectProperty == null)
        return Array.Empty<BusinessObjectValidationFailure>();

      return validationResult.GetValidationFailures(resultObject, businessObjectProperty, true);
    }
  }
}
