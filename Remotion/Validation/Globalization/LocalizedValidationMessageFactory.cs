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
using System.Collections;
using System.Threading;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Globalization
{
  [ImplementationFor(typeof(IValidationMessageFactory), Position = Position, Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple)]
  public class LocalizedValidationMessageFactory : IValidationMessageFactory
  {
    public const int Position = 0;

    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.Validation.Globalization.Globalization.LocalizedValidationMessageFactory")]
    public enum ResourceIdentifier
    {
      ValueMustBeEqualValidationMessage,
      ValueMustBeGreaterThanOrEqualValidationMessage,
      ValueMustBeGreaterThanValidationMessage,
      ValueMustBeLessThanOrEqualValidationMessage,
      ValueMustBeLessThanValidationMessage,
      ValueMustHaveExactLengthOfOneValidationMessage,
      ValueMustHaveExactLengthValidationMessage,
      ValueMustHaveMaximumLengthOfOneValidationMessage,
      ValueMustHaveMaximumLengthValidationMessage,
      ValueMustHaveMinimumLengthOfOneValidationMessage,
      ValueMustHaveMinimumLengthValidationMessage,
      ValueMustMatchExclusiveRangeValidationMessage,
      ValueMustMatchInclusiveRangeValidationMessage,
      ValueMustMatchMinAndMaxLengthValidationMessage,
      ValueMustMatchPredicateValidationMessage,
      ValueMustMatchRegularExpressionValidationMessage,
      ValueMustNotBeEmptyCollectionValidationMessage,
      ValueMustNotBeEmptyBinaryValidationMessage,
      ValueMustNotBeEmptyOrWhitespaceValidationMessage,
      ValueMustNotBeEqualValidationMessage,
      ValueMustNotBeNullCollectionValidationMessage,
      ValueMustNotBeNullBooleanValidationMessage,
      ValueMustNotBeNullDateTimeValidationMessage,
      ValueMustNotBeNullDateValidationMessage,
      ValueMustNotBeNullDecimalValidationMessage,
      ValueMustNotBeNullEnumValidationMessage,
      ValueMustNotBeNullIntegerValidationMessage,
      ValueMustNotBeNullReferenceValidationMessage,
      ValueMustNotBeNullStringValidationMessage,
      ValueMustNotExceedDecimalConstraintsValidationMessage,
    }

    private readonly Lazy<IResourceManager> _resourceManager;

    public LocalizedValidationMessageFactory (
        IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull("globalizationService", globalizationService);

      _resourceManager = new Lazy<IResourceManager>(
          () => globalizationService.GetResourceManager(TypeAdapter.Create(typeof(ResourceIdentifier))),
          LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public ValidationMessage? CreateValidationMessageForPropertyValidator (IPropertyValidator validator, IPropertyInformation validatedProperty)
    {
      ArgumentUtility.CheckNotNull("validator", validator);
      ArgumentUtility.CheckNotNull("validatedProperty", validatedProperty);

      var resourceIdentifier = GetResourceIdentifierOrNull(validator, validatedProperty);
      if (!resourceIdentifier.HasValue)
        return null;

      return new ResourceManagerBasedValidationMessage(_resourceManager.Value, resourceIdentifier);
    }

    public ValidationMessage? CreateValidationMessageForObjectValidator (IObjectValidator validator, ITypeInformation validatedType)
    {
      ArgumentUtility.CheckNotNull("validator", validator);
      ArgumentUtility.CheckNotNull("validatedType", validatedType);

      return null;
    }

    private ResourceIdentifier? GetResourceIdentifierOrNull (IPropertyValidator validator, IPropertyInformation validatedProperty)
    {
      var propertyType = validatedProperty.PropertyType;
      var dataType = NullableTypeUtility.GetBasicType(propertyType);

      if (validator is EqualValidator)
        return ResourceIdentifier.ValueMustBeEqualValidationMessage;

      if (validator is ExclusiveRangeValidator)
        return ResourceIdentifier.ValueMustMatchExclusiveRangeValidationMessage;

      if (validator is GreaterThanOrEqualValidator)
        return ResourceIdentifier.ValueMustBeGreaterThanOrEqualValidationMessage;

      if (validator is GreaterThanValidator)
        return ResourceIdentifier.ValueMustBeGreaterThanValidationMessage;

      if (validator is InclusiveRangeValidator)
        return ResourceIdentifier.ValueMustMatchInclusiveRangeValidationMessage;

      if (validator is LessThanOrEqualValidator)
        return ResourceIdentifier.ValueMustBeLessThanOrEqualValidationMessage;

      if (validator is LessThanValidator)
        return ResourceIdentifier.ValueMustBeLessThanValidationMessage;

      if (validator is LengthValidator)
        return ResourceIdentifier.ValueMustMatchMinAndMaxLengthValidationMessage;

      if (validator is ExactLengthValidator exactLengthValidator)
        return GetResourceIdentifierForExactLengthValidator(exactLengthValidator);

      if (validator is MinimumLengthValidator minLengthValidator)
        return GetResourceIdentifierForMinimumLengthValidator(minLengthValidator);

      if (validator is MaximumLengthValidator maxLengthValidator)
        return GetResourceIdentifierForMaximumLengthValidator(maxLengthValidator);

      if (validator is NotEmptyBinaryValidator)
        return ResourceIdentifier.ValueMustNotBeEmptyBinaryValidationMessage;

      if (validator is NotEmptyOrWhitespaceValidator)
        return ResourceIdentifier.ValueMustNotBeEmptyOrWhitespaceValidationMessage;

      if (validator is NotEmptyCollectionValidator)
        return ResourceIdentifier.ValueMustNotBeEmptyCollectionValidationMessage;

      if (validator is NotEqualValidator)
        return ResourceIdentifier.ValueMustNotBeEqualValidationMessage;

      if (validator is NotNullValidator)
        return GetResourceIdentifierForNotNullValidator(dataType);

      if (validator is PredicateValidator)
        return ResourceIdentifier.ValueMustMatchPredicateValidationMessage;

      if (validator is RegularExpressionValidator)
        return ResourceIdentifier.ValueMustMatchRegularExpressionValidationMessage;

      if (validator is DecimalValidator)
        return ResourceIdentifier.ValueMustNotExceedDecimalConstraintsValidationMessage;

      return null;
    }

    private ResourceIdentifier GetResourceIdentifierForExactLengthValidator (ExactLengthValidator validator)
    {
      if (validator.Length == 1)
        return ResourceIdentifier.ValueMustHaveExactLengthOfOneValidationMessage;

      return ResourceIdentifier.ValueMustHaveExactLengthValidationMessage;
    }

    private ResourceIdentifier GetResourceIdentifierForMinimumLengthValidator (MinimumLengthValidator validator)
    {
      if (validator.Min == 1)
        return ResourceIdentifier.ValueMustHaveMinimumLengthOfOneValidationMessage;

      return ResourceIdentifier.ValueMustHaveMinimumLengthValidationMessage;
    }

    private ResourceIdentifier GetResourceIdentifierForMaximumLengthValidator (MaximumLengthValidator validator)
    {
      if (validator.Max == 1)
        return ResourceIdentifier.ValueMustHaveMaximumLengthOfOneValidationMessage;

      return ResourceIdentifier.ValueMustHaveMaximumLengthValidationMessage;
    }

    private ResourceIdentifier GetResourceIdentifierForNotNullValidator (Type dataType)
    {
      if (dataType.IsEnum)
        return ResourceIdentifier.ValueMustNotBeNullEnumValidationMessage;

      var typeCode = Type.GetTypeCode(dataType);
      switch (typeCode)
      {
        case TypeCode.Boolean:
          return ResourceIdentifier.ValueMustNotBeNullBooleanValidationMessage;

        case TypeCode.SByte:
        case TypeCode.Byte:
        case TypeCode.Int16:
        case TypeCode.UInt16:
        case TypeCode.Int32:
        case TypeCode.UInt32:
        case TypeCode.Int64:
        case TypeCode.UInt64:
          return ResourceIdentifier.ValueMustNotBeNullIntegerValidationMessage;

        case TypeCode.Single:
        case TypeCode.Double:
        case TypeCode.Decimal:
          return ResourceIdentifier.ValueMustNotBeNullDecimalValidationMessage;

        case TypeCode.DateTime:
          return ResourceIdentifier.ValueMustNotBeNullDateTimeValidationMessage;

        case TypeCode.String:
          return ResourceIdentifier.ValueMustNotBeNullStringValidationMessage;

        default:
          break;
      }

      if (dataType == typeof(Guid))
        return ResourceIdentifier.ValueMustNotBeNullStringValidationMessage;

      if (typeof(IEnumerable).IsAssignableFrom(dataType))
        return ResourceIdentifier.ValueMustNotBeNullCollectionValidationMessage;

      return ResourceIdentifier.ValueMustNotBeNullReferenceValidationMessage;
    }
  }
}
