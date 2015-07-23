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
using FluentValidation.Resources;
using FluentValidation.Validators;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Validation.Globalization
{
  [ImplementationFor (typeof(IDefaultMessageEvaluator), Lifetime = LifetimeKind.Singleton)]
  public class DefaultMessageEvaluator : IDefaultMessageEvaluator
  {
    public bool HasDefaultMessageAssigned (IPropertyValidator validator)
    {
      ArgumentUtility.CheckNotNull ("validator", validator);

      if (validator is NotNullValidator)
        return IsDefault (validator, Messages.notnull_error);

      if (validator is NotEmptyValidator)
        return IsDefault (validator, Messages.notempty_error);

      if (validator is NotEqualValidator)
        return IsDefault (validator, Messages.notequal_error);

      if (validator is EqualValidator)
        return IsDefault (validator, Messages.equal_error);

      if (validator is CreditCardValidator)
        return IsDefault (validator, Messages.CreditCardError);

      if (validator is EmailValidator)
        return IsDefault (validator, Messages.email_error);

      if (validator is ExactLengthValidator)
        return IsDefault (validator, Messages.exact_length_error);

      if (validator is LengthValidator)
        return IsDefault (validator, Messages.length_error);

      if (validator is ExclusiveBetweenValidator)
        return IsDefault (validator, Messages.exclusivebetween_error);

      if (validator is InclusiveBetweenValidator)
        return IsDefault (validator, Messages.inclusivebetween_error);

      if (validator is LessThanValidator)
        return IsDefault (validator, Messages.lessthan_error);

      if (validator is LessThanOrEqualValidator)
        return IsDefault (validator, Messages.lessthanorequal_error);

      if (validator is GreaterThanValidator)
        return IsDefault (validator, Messages.greaterthan_error);

      if (validator is GreaterThanOrEqualValidator)
        return IsDefault (validator, Messages.greaterthanorequal_error);

      if (validator is PredicateValidator)
        return IsDefault (validator, Messages.predicate_error);

      if (validator is RegularExpressionValidator)
        return IsDefault (validator, Messages.regex_error);

      if (validator is ScalePrecisionValidator)
        return IsDefault (validator, Messages.scale_precision_error);

      return false;
    }

    private bool IsDefault (IPropertyValidator validator, string errorMessage)
    {
      return validator.ErrorMessageSource is LocalizedStringSource && validator.ErrorMessageSource.GetString() == errorMessage;
    }
  }
}