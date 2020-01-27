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
using JetBrains.Annotations;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Implementation;
using Remotion.Validation.RuleCollectors;
using Remotion.Validation.Rules;

namespace Remotion.Validation.Validators
{
  public static class ObjectValidatorFactory
  {
    [NotNull]
    public static T Create<T> (
        [NotNull] ITypeInformation validatedType,
        [NotNull] Func<ObjectValidationRuleInitializationParameters, T> validatorFactory,
        [NotNull] IValidationMessageFactory validationMessageFactory)
        where T : IObjectValidator
    {
      ArgumentUtility.CheckNotNull ("validatedType", validatedType);
      ArgumentUtility.CheckNotNull ("validatorFactory", validatorFactory);
      ArgumentUtility.CheckNotNull ("validationMessageFactory", validationMessageFactory);

      var deferredInitializationValidationMessage = new DeferredInitializationValidationMessage();
      var initializationParameters = new ObjectValidationRuleInitializationParameters (deferredInitializationValidationMessage);
      var validator = validatorFactory (initializationParameters);
      Assertion.IsNotNull (validator, "validatorFactory evaluated and returned null.");

      var validationMessage = validationMessageFactory.CreateValidationMessageForObjectValidator (validator, validatedType);
      Assertion.IsNotNull (
          validationMessage,
          "The {0} did not return a result for {1} applied to type '{2}'.",
          nameof (IValidationMessageFactory),
          validator.GetType().Name,
          validatedType.FullName);

      deferredInitializationValidationMessage.Initialize (validationMessage);

      return validator;
    }
  }
}