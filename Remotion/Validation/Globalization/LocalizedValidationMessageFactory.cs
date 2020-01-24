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
using System.Threading;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Globalization
{
  [ImplementationFor (typeof (IValidationMessageFactory), Position = Position, Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple)]
  public class LocalizedValidationMessageFactory : IValidationMessageFactory
  {
    public const int Position = 0;

    [ResourceIdentifiers]
    [MultiLingualResources ("Remotion.Validation.Globalization.Globalization.LocalizedValidationMessageFactory")]
    public enum ResourceIdentifier
    {
      ValueMustBeEqualValidationMessage,
      ValueMustHaveExactLengthValidationMessage,
      ValueMustMatchExclusiveRangeValidationMessage,
      ValueMustBeGreaterThanOrEqualValidationMessage,
      ValueMustBeGreaterThanValidationMessage,
      ValueMustMatchInclusiveRangeValidationMessage,
      ValueMustMatchMinAndMaxLengthValidationMessage,
      ValueMustBeLessThanOrEqualValidationMessage,
      ValueMustBeLessThanValidationMessage,
      ValueMustNotBeEmptyValidationMessage,
      ValueMustNotBeEqualValidationMessage,
      ValueMustNotBeNullValidationMessage,
      ValueMustMatchPredicateValidationMessage,
      ValueMustMatchRegularExpressionValidationMessage,
      ValueMustNotExceedScaleAndPrecisionValidationMessage
    }

    private readonly Lazy<IResourceManager> _resourceManager;

    public LocalizedValidationMessageFactory (
        IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);

      _resourceManager = new Lazy<IResourceManager> (
          () => globalizationService.GetResourceManager (TypeAdapter.Create (typeof (ResourceIdentifier))),
          LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public ValidationMessage CreateValidationMessageForPropertyValidator (IPropertyValidator validator, IPropertyInformation validatedProperty)
    {
      ArgumentUtility.CheckNotNull ("validator", validator);
      ArgumentUtility.CheckNotNull ("validatedProperty", validatedProperty);

      var resourceIdentifier = GetResourceIdentifierOrNull (validator.GetType());
      if (!resourceIdentifier.HasValue)
        return null;

      return new ResourceManagerBasedValidationMessage (_resourceManager.Value, resourceIdentifier);
    }

    public ValidationMessage CreateValidationMessageForObjectValidator (IObjectValidator validator, ITypeInformation validatedType)
    {
      ArgumentUtility.CheckNotNull ("validator", validator);
      ArgumentUtility.CheckNotNull ("validatedType", validatedType);

      return null;
    }

    private ResourceIdentifier? GetResourceIdentifierOrNull (Type validatorType)
    {
      if (validatorType == typeof (EqualValidator))
        return ResourceIdentifier.ValueMustBeEqualValidationMessage;
      if (validatorType == typeof (ExactLengthValidator))
        return ResourceIdentifier.ValueMustHaveExactLengthValidationMessage;
      if (validatorType == typeof (ExclusiveRangeValidator))
        return ResourceIdentifier.ValueMustMatchExclusiveRangeValidationMessage;
      if (validatorType == typeof (GreaterThanOrEqualValidator))
        return ResourceIdentifier.ValueMustBeGreaterThanOrEqualValidationMessage;
      if (validatorType == typeof (GreaterThanValidator))
        return ResourceIdentifier.ValueMustBeGreaterThanValidationMessage;
      if (validatorType == typeof (InclusiveRangeValidator))
        return ResourceIdentifier.ValueMustMatchInclusiveRangeValidationMessage;
      if (validatorType == typeof (LengthValidator))
        return ResourceIdentifier.ValueMustMatchMinAndMaxLengthValidationMessage;
      if (validatorType == typeof (LessThanOrEqualValidator))
        return ResourceIdentifier.ValueMustBeLessThanOrEqualValidationMessage;
      if (validatorType == typeof (LessThanValidator))
        return ResourceIdentifier.ValueMustBeLessThanValidationMessage;
      if (validatorType == typeof (MaximumLengthValidator))
        return ResourceIdentifier.ValueMustMatchMinAndMaxLengthValidationMessage;
      if (validatorType == typeof (MinimumLengthValidator))
        return ResourceIdentifier.ValueMustMatchMinAndMaxLengthValidationMessage;
      if (validatorType == typeof (NotEmptyValidator))
        return ResourceIdentifier.ValueMustNotBeEmptyValidationMessage;
      if (validatorType == typeof (NotEqualValidator))
        return ResourceIdentifier.ValueMustNotBeEqualValidationMessage;
      if (validatorType == typeof (NotNullValidator))
        return ResourceIdentifier.ValueMustNotBeNullValidationMessage;
      if (validatorType == typeof (PredicateValidator))
        return ResourceIdentifier.ValueMustMatchPredicateValidationMessage;
      if (validatorType == typeof (RegularExpressionValidator))
        return ResourceIdentifier.ValueMustMatchRegularExpressionValidationMessage;
      if (validatorType == typeof (ScalePrecisionValidator))
        return ResourceIdentifier.ValueMustNotExceedScaleAndPrecisionValidationMessage;
      return null;
    }
  }
}