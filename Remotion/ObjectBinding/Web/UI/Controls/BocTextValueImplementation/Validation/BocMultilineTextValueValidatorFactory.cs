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
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocTextValueImplementation.Validation
{
  /// <summary>
  /// Implements the <see cref="IBocMultilineTextValueValidatorFactory"/> interface
  /// and creates all validators required to ensure a valid property value (i.e. nullability and formatting).
  /// </summary>
  /// <seealso cref="IBocMultilineTextValueValidatorFactory"/>
  [ImplementationFor(typeof(IBocMultilineTextValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
  public class BocMultilineTextValueValidatorFactory : IBocMultilineTextValueValidatorFactory
  {
    public const int Position = 0;

    public BocMultilineTextValueValidatorFactory ()
    {
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocMultilineTextValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull("control", control);

      if (isReadOnly)
        yield break;

      IResourceManager resourceManager = control.GetResourceManager();
      var requiredFieldValidator = CreateRequiredFieldValidator(control, resourceManager);
      if (requiredFieldValidator != null)
        yield return requiredFieldValidator;

      var lengthValidator = CreateLengthValidator(control, resourceManager);
      if (lengthValidator != null)
        yield return lengthValidator;

      yield return CreateTypeIsStringValidator(control, resourceManager);
    }

    private RequiredFieldValidator? CreateRequiredFieldValidator (IBocMultilineTextValue control, IResourceManager resourceManager)
    {
      var areOptionalValidatorsEnabled = control.AreOptionalValidatorsEnabled;
      var isPropertyTypeRequired = !areOptionalValidatorsEnabled && control.DataSource?.BusinessObject != null && control.Property?.IsNullable == false;
      var isControlRequired = areOptionalValidatorsEnabled && control.IsRequired;

      if (isPropertyTypeRequired || isControlRequired)
      {
        RequiredFieldValidator requiredValidator = new RequiredFieldValidator();
        requiredValidator.ID = control.ID + "_ValidatorRequired";
        requiredValidator.ControlToValidate = control.TargetControl.ID;
        requiredValidator.ErrorMessage = resourceManager.GetString(BocMultilineTextValue.ResourceIdentifier.RequiredValidationMessage);
        requiredValidator.EnableViewState = false;

        return requiredValidator;
      }
      else
      {
        return null;
      }
    }

    private LengthValidator? CreateLengthValidator (IBocMultilineTextValue control, IResourceManager resourceManager)
    {
      var maxLength = control.TextBoxStyle.GetMaxLength();
      var hasControlMaxLength = control.AreOptionalValidatorsEnabled && maxLength.HasValue;

      if (hasControlMaxLength)
      {
        LengthValidator lengthValidator = new LengthValidator();
        lengthValidator.ID = control.ID + "_ValidatorMaxLength";
        lengthValidator.ControlToValidate = control.TargetControl.ID;
        lengthValidator.MaximumLength = maxLength;
        lengthValidator.ErrorMessage = string.Format(
            resourceManager.GetString(BocMultilineTextValue.ResourceIdentifier.MaxLengthValidationMessage),
            maxLength);
        lengthValidator.EnableViewState = false;

        return lengthValidator;
      }
      else
      {
        return null;
      }
    }

    private ControlCharactersCharactersValidator CreateTypeIsStringValidator (IBocMultilineTextValue control, IResourceManager resourceManager)
    {
      ControlCharactersCharactersValidator typeValidator = new ControlCharactersCharactersValidator();
      typeValidator.ID = control.ID + "_ValidatorType";
      typeValidator.ControlToValidate = control.TargetControl.ID;
      typeValidator.SampleTextLength = 5;
      typeValidator.EnableMultilineText = true;
      typeValidator.ErrorMessageFormat = resourceManager.GetString(BocMultilineTextValue.ResourceIdentifier.InvalidCharactersErrorMessage);
      typeValidator.EnableViewState = false;

      return typeValidator;
    }
  }
}
