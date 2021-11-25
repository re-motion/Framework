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
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation.Factories
{
  /// <summary>
  /// Implements various <see cref="IBocListValidatorFactory"/> interfaces and creates validators 
  /// that can apply the <see cref="IBusinessObjectValidationResult"/> object to the respective control.
  /// </summary>
  /// <seealso cref="IBocListValidatorFactory"/>
  [ImplementationFor (typeof(IBocListValidatorFactory), Lifetime = LifetimeKind.Singleton, RegistrationType = RegistrationType.Multiple, Position = Position)]
  public class ValidationBocListValidatorFactory : IBocListValidatorFactory
  {
    public const int Position = BocListValidatorFactory.Position + 1;

    public ValidationBocListValidatorFactory ()
    {
    }

    public IEnumerable<BaseValidator> CreateValidators (IBocList control, bool isReadOnly)
    {
      ArgumentUtility.CheckNotNull("control", control);

      if (isReadOnly)
        yield break;

      yield return CreateBocListValidator(control);
    }

    private BaseValidator CreateBocListValidator (IBocList control)
    {
      var bocValidator = new BocListValidationResultDispatchingValidator();
      bocValidator.ControlToValidate = control.ID;
      bocValidator.ID = control.ID + "_BocListValidator";
      bocValidator.EnableViewState = false;

      return bocValidator;
    }
  }
}