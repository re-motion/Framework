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
using FluentValidation;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Validation.Implementation;

namespace Remotion.Validation.Globalization
{
  [ImplementationFor (typeof (IValidationRuleMetadataService), Lifetime = LifetimeKind.Singleton, Position = 1, RegistrationType = RegistrationType.Multiple)]
  public class ValidationRuleGlobalizationService : IValidationRuleMetadataService
  {
    private readonly IDefaultMessageEvaluator _defaultMessageEvaluator;
    private readonly IErrorMessageGlobalizationService _errorMessageGlobalizationService;
    
    public ValidationRuleGlobalizationService (
        IDefaultMessageEvaluator defaultMessageEvaluator,
        IErrorMessageGlobalizationService errorMessageGlobalizationService)
    {
      ArgumentUtility.CheckNotNull ("defaultMessageEvaluator", defaultMessageEvaluator);
      ArgumentUtility.CheckNotNull ("errorMessageGlobalizationService", errorMessageGlobalizationService);

      _defaultMessageEvaluator = defaultMessageEvaluator;
      _errorMessageGlobalizationService = errorMessageGlobalizationService;
    }

    public void ApplyMetadata (IValidationRule validationRule, Type typeToValidate)
    {
      ArgumentUtility.CheckNotNull ("validationRule", validationRule);
      ArgumentUtility.CheckNotNull ("typeToValidate", typeToValidate);
      
      foreach (var validator in validationRule.Validators)
      {
        if (_defaultMessageEvaluator.HasDefaultMessageAssigned (validator))
          validator.ErrorMessageSource = new ErrorMessageStringSource (validator, _errorMessageGlobalizationService, validator.ErrorMessageSource);
      }
    }
  }
}