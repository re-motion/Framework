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
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Validation
{
  [ImplementationFor(typeof(IBocDateTimeValueValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
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
      yield return CreateDateTimeValidator (control, resourceManager);
    }

    private BocDateTimeValueValidator CreateDateTimeValidator (IBocDateTimeValue control, IResourceManager resourceManager)
    {
      var dateTimeValueValidator = new BocDateTimeValueValidator();
      dateTimeValueValidator.ID = control.ID + "_ValidatorDateTime";
      dateTimeValueValidator.ControlToValidate = control.ID;
      dateTimeValueValidator.MissingDateAndTimeErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.MissingDateAndTimeErrorMessage);
      dateTimeValueValidator.MissingDateOrTimeErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.MissingDateOrTimeErrorMessage);
      dateTimeValueValidator.MissingDateErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.MissingDateErrorMessage);
      dateTimeValueValidator.MissingTimeErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.MissingTimeErrorMessage);
      dateTimeValueValidator.InvalidDateAndTimeErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.InvalidDateAndTimeErrorMessage);
      dateTimeValueValidator.InvalidDateErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.InvalidDateErrorMessage);
      dateTimeValueValidator.InvalidTimeErrorMessage = resourceManager.GetString (BocDateTimeValue.ResourceIdentifier.InvalidTimeErrorMessage);

      return dateTimeValueValidator;
    }
  }
}