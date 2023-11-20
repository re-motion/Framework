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
using System.Reflection;
using System.Text;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Validators;

namespace Remotion.Validation.Implementation
{
  /// <summary>
  /// Implements the <see cref="IValidatorFormatter"/> as a decorator 
  /// and adds additional available metadata for <see cref="IPropertyValidator"/> implementations from the <b>FluentValidation</b> library.
  /// </summary>
  [ImplementationFor(typeof(IValidatorFormatter), RegistrationType = RegistrationType.Decorator)]
  public class DiagnosticInformationValidatorFormatterDecorator : IValidatorFormatter
  {
    private readonly IValidatorFormatter _fallBackValidatorFormatter;

    public DiagnosticInformationValidatorFormatterDecorator (IValidatorFormatter fallBackValidatorFormatter)
    {
      ArgumentUtility.CheckNotNull("fallBackValidatorFormatter", fallBackValidatorFormatter);

      _fallBackValidatorFormatter = fallBackValidatorFormatter;
    }

    public IValidatorFormatter FallBackValidatorFormatter
    {
      get { return _fallBackValidatorFormatter; }
    }

    public string Format (IPropertyValidator validator, Func<Type, string> typeNameFormatter)
    {
      ArgumentUtility.CheckNotNull("validator", validator);
      ArgumentUtility.CheckNotNull("typeNameFormatter", typeNameFormatter);

      var validatorType = validator.GetType();
      var typeName = typeNameFormatter(validatorType);

      switch (validator)
      {
        case IRequiredValidator _:
        case NotEmptyBinaryValidator _:
        case NotEmptyOrWhitespaceValidator _:
          return typeName;

        case LengthValidator lengthValidator:
          return FormatLengthValidator(lengthValidator, typeName);

        case ExactLengthValidator minLengthValidator:
          return FormatExactLengthValidator(minLengthValidator, typeName);

        case MinimumLengthValidator minLengthValidator:
          return FormatMinimumLengthValidator(minLengthValidator, typeName);

        case MaximumLengthValidator maxLengthValidator:
          return FormatMaximumLengthValidator(maxLengthValidator, typeName);

        case IRangeValidator rangeValidator:
          return FormatRangeValidator(rangeValidator, typeName);

        case IValueComparisonValidator valueComparisonValidator:
          return FormatComparisonValidator(valueComparisonValidator, typeNameFormatter, typeName);

        case IRegularExpressionValidator regularExpressionValidator:
          return FormatRegularExpressionValidator(regularExpressionValidator, typeName);

        case DecimalValidator decimalValidator:
          return FormatDecimalValidator(decimalValidator, typeName);

        default:
          return _fallBackValidatorFormatter.Format(validator, typeNameFormatter);
      }
    }

    private string FormatLengthValidator (LengthValidator validator, string typeName)
    {
      return string.Format("{0} {{ MinLength = '{1}', MaxLength = '{2}' }}", typeName, validator.Min, validator.Max);
    }

    private string FormatExactLengthValidator (ExactLengthValidator validator, string typeName)
    {
      return string.Format("{0} {{ Length = '{1}' }}", typeName, validator.Length);
    }

    private string FormatMinimumLengthValidator (MinimumLengthValidator validator, string typeName)
    {
      return string.Format("{0} {{ MinLength = '{1}' }}", typeName, validator.Min);
    }

    private string FormatMaximumLengthValidator (MaximumLengthValidator validator, string typeName)
    {
      return string.Format("{0} {{ MaxLength = '{1}' }}", typeName, validator.Max);
    }

    private string FormatRangeValidator (IRangeValidator validator, string typeName)
    {
      return string.Format("{0} {{ From = '{1}', To = '{2}' }}", typeName, validator.From, validator.To);
    }

    private string FormatComparisonValidator (IValueComparisonValidator validator, Func<Type, string> typeNameFormatter, string typeName)
    {
      return string.Format("{0} {{ ValueToCompare = '{1}' }}", typeName, validator.ValueToCompare);
    }

    private string FormatRegularExpressionValidator (IRegularExpressionValidator validator, string typeName)
    {
      return string.Format("{0} {{ Expression = '{1}' }}", typeName, validator.Regex);
    }

    private string FormatDecimalValidator (DecimalValidator validator, string typeName)
    {
      return string.Format(
          "{0} {{ MaxIntegerPlaces = '{1}', MaxDecimalPlaces = '{2}', IgnoreTrailingZeros = {3} }}",
          typeName,
          validator.MaxIntegerPlaces,
          validator.MaxDecimalPlaces,
          validator.IgnoreTrailingZeros);
    }

    private string FormatMemberInfo (MemberInfo memberInfo, Func<Type, string> typeNameFormatter)
    {
      var sb = new StringBuilder();
      if (memberInfo.DeclaringType != null)
        sb.Append(typeNameFormatter(memberInfo.DeclaringType) + ".");
      sb.AppendLine(memberInfo.Name);
      return sb.ToString();
    }
  }
}
