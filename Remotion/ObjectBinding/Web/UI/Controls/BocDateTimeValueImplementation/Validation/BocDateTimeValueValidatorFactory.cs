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
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Validation
{
  /// <summary>
  /// Implements the <see cref="IBocDateTimeValueValidatorFactory"/> inteface and creates all validators required to ensure a valid property value (i.e. nullability and formatting).
  /// </summary>
  /// <seealso cref="IBocDateTimeValueValidatorFactory"/>
  [ImplementationFor (typeof (IBocDateTimeValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple,
      Position = Position)]
  public class BocDateTimeValueValidatorFactory : IBocDateTimeValueValidatorFactory
  {
    public const int Position = 0;

    public BocDateTimeValueValidatorFactory ()
    {
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocDateTimeValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (isReadOnly)
        yield break;

      var resourceManager = control.GetResourceManager();

      var requiredFieldValidator = CreateRequiredValidator (control, resourceManager);
      if (requiredFieldValidator != null)
        yield return requiredFieldValidator;

      yield return CreateFormatValidator (control, resourceManager);
    }

    private BocDateTimeRequiredValidator CreateRequiredValidator (IBocDateTimeValue control, IResourceManager resourceManager)
    {
      var areOptionalValidatorsEnabled = control.AreOptionalValidatorsEnabled;
      var isPropertyTypeRequired = !areOptionalValidatorsEnabled && control.DataSource?.BusinessObject != null && control.Property?.IsNullable == false;
      var isControlRequired = areOptionalValidatorsEnabled && control.IsRequired;

      if (isPropertyTypeRequired || isControlRequired)
      {
        var validator = new BocDateTimeRequiredValidator();
        validator.ID = control.ID + "_RequiredDateTimeValidator";
        validator.ControlToValidate = control.ID;
        validator.MissingDateAndTimeErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.MissingDateAndTimeErrorMessage);
        validator.MissingDateOrTimeErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.MissingDateOrTimeErrorMessage);
        validator.MissingDateErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.MissingDateErrorMessage);
        validator.MissingTimeErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.MissingTimeErrorMessage);
        validator.EnableViewState = false;

        return validator;
      }
      else
      {
        return null;
      }
    }

    private BocDateTimeFormatValidator CreateFormatValidator (IBocDateTimeValue control, IResourceManager resourceManager)
    {
      var validator = new BocDateTimeFormatValidator();
      validator.ID = control.ID + "_FormatDateTimeValidator";
      validator.ControlToValidate = control.ID;
      validator.InvalidDateAndTimeErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.InvalidDateAndTimeErrorMessage);
      validator.InvalidDateErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.InvalidDateErrorMessage);
      validator.InvalidTimeErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.InvalidTimeErrorMessage);
      validator.EnableViewState = false;

      return validator;
    }
  }
}