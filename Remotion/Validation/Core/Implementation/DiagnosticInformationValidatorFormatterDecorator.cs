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
  [ImplementationFor (typeof (IValidatorFormatter), RegistrationType = RegistrationType.Decorator)]
  public class DiagnosticInformationValidatorFormatterDecorator : IValidatorFormatter
  {
    private readonly IValidatorFormatter _fallBackValidatorFormatter;

    public DiagnosticInformationValidatorFormatterDecorator (IValidatorFormatter fallBackValidatorFormatter)
    {
      ArgumentUtility.CheckNotNull ("fallBackValidatorFormatter", fallBackValidatorFormatter);

      _fallBackValidatorFormatter = fallBackValidatorFormatter;
    }

    public IValidatorFormatter FallBackValidatorFormatter
    {
      get { return _fallBackValidatorFormatter; }
    }

    public string Format (IPropertyValidator validator, Func<Type, string> typeNameFormatter)
    {
      ArgumentUtility.CheckNotNull ("validator", validator);
      ArgumentUtility.CheckNotNull ("typeNameFormatter", typeNameFormatter);

      var validatorType = validator.GetType();
      var typeName = typeNameFormatter (validatorType);

      if (typeof (INotNullValidator).IsAssignableFrom (validatorType) || typeof (INotEmptyValidator).IsAssignableFrom (validatorType))
        return typeName;

      if (typeof (ILengthValidator).IsAssignableFrom (validatorType))
        return FormatLengthValidator (validator, typeName);

      if (typeof (IRangeValidator).IsAssignableFrom (validatorType))
        return FormatRangeValidator (validator, typeName);

      if (typeof (IValueComparisonValidator).IsAssignableFrom (validatorType))
        return FormatComparisonValidator (validator, typeNameFormatter, typeName);

      if (typeof (IRegularExpressionValidator).IsAssignableFrom (validatorType))
        return FormatRegularExpressionValidator (validator, typeName);

      var precisionValidator = validator as ScalePrecisionValidator;
      if (precisionValidator != null)
        return FormatScalePrecisionValidator (precisionValidator, typeName);

      return _fallBackValidatorFormatter.Format (validator, typeNameFormatter);
    }

    private string FormatLengthValidator (IPropertyValidator validator, string typeName)
    {
      var lengthValidator = (ILengthValidator) validator;
      return string.Format ("{0} {{ MinLength = '{1}', MaxLength = '{2}' }}", typeName, lengthValidator.Min, lengthValidator.Max);
    }

    private string FormatRangeValidator (IPropertyValidator validator, string typeName)
    {
      var betweenValidator = (IRangeValidator) validator;
      return string.Format ("{0} {{ From = '{1}', To = '{2}' }}", typeName, betweenValidator.From, betweenValidator.To);
    }

    private string FormatComparisonValidator (IPropertyValidator validator, Func<Type, string> typeNameFormatter, string typeName)
    {
      var comparisonValidator = (IValueComparisonValidator) validator;
      return string.Format ("{0} {{ ValueToCompare = '{1}' }}", typeName, comparisonValidator.ValueToCompare);
    }

    private string FormatRegularExpressionValidator (IPropertyValidator validator, string typeName)
    {
      var regularExpressionValidator = (IRegularExpressionValidator) validator;
      return string.Format ("{0} {{ Expression = '{1}' }}", typeName, regularExpressionValidator.Regex);
    }

    private string FormatScalePrecisionValidator (ScalePrecisionValidator precisionValidator, string typeName)
    {
      var scalePrecisionValidator = precisionValidator;
      return string.Format (
          "{0} {{ Scale = '{1}', Precision = '{2}' }}",
          typeName,
          scalePrecisionValidator.Scale,
          scalePrecisionValidator.Precision);
    }

    private string FormatMemberInfo (MemberInfo memberInfo, Func<Type, string> typeNameFormatter)
    {
      var sb = new StringBuilder();
      if (memberInfo.DeclaringType != null)
        sb.Append (typeNameFormatter (memberInfo.DeclaringType) + ".");
      sb.AppendLine (memberInfo.Name);
      return sb.ToString();
    }
  }
}