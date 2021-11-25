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
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Results;

namespace Remotion.ObjectBinding.Validation
{
  /// <summary>
  /// Implementation of the <see cref="IBusinessObjectValidationResult"/> interface for <see cref="ValidationResult"/>.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class BusinessObjectValidationResult : IBusinessObjectValidationResult
  {
    public static BusinessObjectValidationResult Create ([NotNull] ValidationResult validationResult)
    {
      ArgumentUtility.CheckNotNull("validationResult", validationResult);

      var validationFailures = validationResult.Errors.Select(e => (CreateBusinessObjectValidationFailure(e), e));
      return new BusinessObjectValidationResult(validationFailures);
    }

    private static BusinessObjectValidationFailure CreateBusinessObjectValidationFailure (ValidationFailure validationFailure)
    {
      // TODO: RM-6056: Implementation: Find a way to not requiring a soft-cast to BindableProperty to get to the IPropertyInformation.
      // Instead, either support matching via visitor or some other good idea. Keep in mind to not clutter the
      // IBusinessObjectProperty interface to accomplish this design goal.

      switch (validationFailure)
      {
        case PropertyValidationFailure propertyValidationFailure
            when validationFailure.ValidatedObject is IBusinessObject validatedBusinessObject
                 && GetBusinessObjectProperty(validatedBusinessObject, propertyValidationFailure.ValidatedProperty) is IBusinessObjectProperty validatedBusinessObjectProperty:
        {
          return BusinessObjectValidationFailure.CreateForBusinessObjectProperty(
              propertyValidationFailure.LocalizedValidationMessage,
              validatedBusinessObject,
              validatedBusinessObjectProperty);
        }

        case ObjectValidationFailure objectValidationFailure
            when validationFailure.ValidatedObject is IBusinessObject validatedBusinessObject:
        {
          return BusinessObjectValidationFailure.CreateForBusinessObject(
              validatedBusinessObject,
              objectValidationFailure.LocalizedValidationMessage);
        }

        default:
        {
          return BusinessObjectValidationFailure.Create(validationFailure.LocalizedValidationMessage);
        }
      }
    }

    [CanBeNull]
    private static IBusinessObjectProperty? GetBusinessObjectProperty (IBusinessObject businessObject, IPropertyInformation property)
    {
      var businessObjectClass = businessObject.BusinessObjectClass;
      Assertion.IsNotNull(businessObjectClass, "The business object's BusinessObjectClass-property evaluated and returned null.");
      return businessObjectClass.GetPropertyDefinition(property.Name);
    }

    private readonly IReadOnlyCollection<(BusinessObjectValidationFailure BusinessObjectValidationFailure, ValidationFailure ValidationFailure)> _validationFailures;

    private readonly HashSet<(BusinessObjectValidationFailure BusinessObjectValidationFailure, ValidationFailure ValidationFailure)> _unhandledValidationFailures;

    private BusinessObjectValidationResult (IEnumerable<(BusinessObjectValidationFailure BusinessObjectValidationFailure, ValidationFailure ValidationFailure)> businessObjectValidationFailures)
    {
      ArgumentUtility.CheckNotNull("businessObjectValidationFailures", businessObjectValidationFailures);

      _validationFailures = businessObjectValidationFailures.ToArray();
      _unhandledValidationFailures = new HashSet<(BusinessObjectValidationFailure BusinessObjectValidationFailure, ValidationFailure ValidationFailure)>(_validationFailures);
    }

    public IReadOnlyCollection<BusinessObjectValidationFailure> GetValidationFailures (
        IBusinessObject businessObject,
        IBusinessObjectProperty businessObjectProperty,
        bool markAsHandled)
    {
      ArgumentUtility.CheckNotNull("businessObject", businessObject);
      ArgumentUtility.CheckNotNull("businessObjectProperty", businessObjectProperty);

      var validationFailures = _validationFailures
          .Where(f => Equals(f.BusinessObjectValidationFailure.ValidatedObject, businessObject))
          .Where(f => f.BusinessObjectValidationFailure.ValidatedProperty?.Identifier == businessObjectProperty.Identifier);

      var result = new List<BusinessObjectValidationFailure>();
      foreach (var validationFailure in validationFailures)
      {
        result.Add(validationFailure.BusinessObjectValidationFailure);

        if (markAsHandled)
          _unhandledValidationFailures.Remove(validationFailure);
      }

      return result;
    }

    public IReadOnlyCollection<UnhandledBusinessObjectValidationFailure> GetUnhandledValidationFailures (IBusinessObject businessObject)
    {
      ArgumentUtility.CheckNotNull("businessObject", businessObject);

      return _unhandledValidationFailures
          .Where(f => Equals(f.BusinessObjectValidationFailure.ValidatedObject, businessObject))
          .Select(f => f.BusinessObjectValidationFailure)
          .Select(f => new UnhandledBusinessObjectValidationFailure(f.ErrorMessage, f.ValidatedProperty))
          .ToArray();
    }

    public IReadOnlyCollection<ValidationFailure> GetUnhandledValidationFailures ()
    {
      return _unhandledValidationFailures.Select(f => f.ValidationFailure).ToArray();
    }
  }
}
