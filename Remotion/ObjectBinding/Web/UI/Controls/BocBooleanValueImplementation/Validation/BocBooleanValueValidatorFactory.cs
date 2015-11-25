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
  [ImplementationFor (typeof (IBocBooleanValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
  public class BocBooleanValueValidatorFactory : IBocBooleanValueValidatorFactory
  {
    public const int Position = 0;

    private const string c_nullString = "null";

    public BocBooleanValueValidatorFactory ()
    {
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocBooleanValue control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull ("control", control);

      if (isReadOnly || !control.IsRequired)
        yield break;

      var resourceManager = control.GetResourceManager();
      yield return CreateRequiredFieldValidator (control, resourceManager);
    }


    private CompareValidator CreateRequiredFieldValidator (IBocBooleanValue control, IResourceManager resourceManager)
    {
      var notNullItemValidator = new CompareValidator ();
      notNullItemValidator.ID = control.ID + "_ValidatorNotNullItem";
      notNullItemValidator.ControlToValidate = control.ID;
      notNullItemValidator.ValueToCompare = c_nullString;
      notNullItemValidator.Operator = ValidationCompareOperator.NotEqual;
      notNullItemValidator.ErrorMessage = resourceManager.GetString (BocBooleanValue.ResourceIdentifier.NullItemValidationMessage);
      
      return notNullItemValidator;
    }
  }
}