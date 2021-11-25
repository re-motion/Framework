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

namespace Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Validation
{
  /// <summary>
  /// Implements the <see cref="IBocBooleanValueValidatorFactory"/> inteface and creates all validators required to ensure a valid property value (i.e. nullability and formatting).
  /// </summary>
  /// <seealso cref="IBocBooleanValueValidatorFactory"/>
  [ImplementationFor (typeof(IBocBooleanValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
  public class BocBooleanValueValidatorFactory : IBocBooleanValueValidatorFactory
  {
    public const int Position = 0;

    public BocBooleanValueValidatorFactory ()
    {
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocBooleanValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull("control", control);

      if (isReadOnly)
        yield break;

      var resourceManager = control.GetResourceManager();

      var requiredFieldValidator = CreateRequiredFieldValidator(control, resourceManager);
      if (requiredFieldValidator != null)
        yield return requiredFieldValidator;
    }

    private RequiredFieldValidator? CreateRequiredFieldValidator (IBocBooleanValue control, IResourceManager resourceManager)
    {
      var areOptionalValidatorsEnabled = control.AreOptionalValidatorsEnabled;
      var isPropertyTypeRequired = !areOptionalValidatorsEnabled && control.DataSource?.BusinessObject != null && control.Property?.IsNullable == false;
      var isControlRequired = areOptionalValidatorsEnabled && control.IsRequired;

      if (isPropertyTypeRequired || isControlRequired)
      {
        var requiredFieldValidator = new RequiredFieldValidator();
        requiredFieldValidator.ID = control.ID + "_ValidatorNotNullItem";
        requiredFieldValidator.ControlToValidate = control.ID;
        requiredFieldValidator.ErrorMessage = resourceManager.GetString(BocBooleanValue.ResourceIdentifier.NullItemValidationMessage);
        requiredFieldValidator.EnableViewState = false;
        return requiredFieldValidator;
      }
      else
      {
        return null;
      }
    }
  }
}
