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
  public class BusinessObjectValidationResult : IBusinessObjectValidationResult
  {
    public static BusinessObjectValidationResult Create ([NotNull] ValidationResult validationResult)
    {
      ArgumentUtility.CheckNotNull ("validationResult", validationResult);

      var businessObjectValidationFailures = validationResult.Errors.Select (CreateBusinessObjectValidationFailure);
      return new BusinessObjectValidationResult (businessObjectValidationFailures);
    }

    private static BusinessObjectValidationFailure CreateBusinessObjectValidationFailure (ValidationFailure validationFailure)
    {
      switch (validationFailure)
      {
        case PropertyValidationFailure propertyValidationFailure
            when validationFailure.ValidatedObject is IBusinessObject validatedBusinessObject
                 && GetBusinessObjectProperty (validatedBusinessObject, propertyValidationFailure.ValidatedProperty) is IBusinessObjectProperty validatedBusinessObjectProperty:
        {
          return BusinessObjectValidationFailure.CreateForBusinessObjectProperty (
              propertyValidationFailure.LocalizedValidationMessage,
              validatedBusinessObject,
              validatedBusinessObjectProperty);
        }

        case ObjectValidationFailure objectValidationFailure
            when validationFailure.ValidatedObject is IBusinessObject validatedBusinessObject:
        {
          return BusinessObjectValidationFailure.CreateForBusinessObject (
              validatedBusinessObject,
              objectValidationFailure.LocalizedValidationMessage);
        }

        default:
        {
          return BusinessObjectValidationFailure.Create (validationFailure.LocalizedValidationMessage);
        }
      }
    }

    [CanBeNull]
    private static IBusinessObjectProperty GetBusinessObjectProperty (IBusinessObject businessObject, IPropertyInformation property)
    {
      var businessObjectClass = businessObject.BusinessObjectClass;
      Assertion.IsNotNull (businessObjectClass, "The business object's BusinessObjectClass-property evaluated and returned null.");
      return businessObjectClass.GetPropertyDefinition (property.Name);
    }

    private readonly IReadOnlyCollection<BusinessObjectValidationFailure> _businessObjectValidationFailures;

    public BusinessObjectValidationResult (IEnumerable<BusinessObjectValidationFailure> businessObjectValidationFailures)
    {
      ArgumentUtility.CheckNotNull ("businessObjectValidationFailures", businessObjectValidationFailures);

      _businessObjectValidationFailures = businessObjectValidationFailures.ToArray();
    }

    public IEnumerable<BusinessObjectValidationFailure> GetValidationFailures (
        IBusinessObject businessObject,
        IBusinessObjectProperty businessObjectProperty,
        bool markAsHandled)
    {
      return _businessObjectValidationFailures.Where (
          f => Equals (f.ValidatedObject, businessObject)
               && (f.ValidatedProperty == null || f.ValidatedProperty.Identifier == businessObjectProperty.Identifier));
    }

    public IEnumerable<UnhandledBusinessObjectValidationFailure> GetUnhandledValidationFailures (IBusinessObject businessObject)
    {
      return Enumerable.Empty<UnhandledBusinessObjectValidationFailure>();
    }

    // Not part of IBusinessObjectValidationResult interface
    public IEnumerable<ValidationFailure> GetUnhandledValidationFailures ()
    {
      return Enumerable.Empty<ValidationFailure>();
    }

    /* Implementation: Find a way to not requiring a soft-cast to BindableProperty to get to the IPropertyInformation.
       Instead, either support matching via visitor or some other good idea. Keep in mind to not clutter the
       IBusinessObjectProperty interface to accomplish this design goal.
    */
  }
}