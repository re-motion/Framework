﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Validation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation
{
  public sealed class BusinessObjectReferenceDataSourceControlValidationResultDispatchingValidator
      : BaseValidator, IBusinessObjectBoundEditableWebControlValidationResultDispatcher
  {
    public BusinessObjectReferenceDataSourceControlValidationResultDispatchingValidator ()
    {
    }

    public void DispatchValidationFailures (IBusinessObjectValidationResult validationResult)
    {
      ArgumentUtility.CheckNotNull("validationResult", validationResult);

      var control = GetControlToValidate();

      BusinessObjectDataSourceValidationResultDispatcher.DispatchValidationResultForBoundControls(control, validationResult);
    }

    protected override bool EvaluateIsValid ()
    {
      // This validator is never invalid because it just dispatches the errors.
      return true;
    }

    protected override bool ControlPropertiesValid ()
    {
      return true;
    }

    [NotNull]
    private BusinessObjectReferenceDataSourceControl GetControlToValidate ()
    {
      var control = NamingContainer.FindControl(ControlToValidate);
      var bocControl = control as BusinessObjectReferenceDataSourceControl;
      if (bocControl == null)
      {
        throw new InvalidOperationException(
            $"'{nameof(BusinessObjectReferenceDataSourceControlValidationResultDispatchingValidator)}' may only be applied to controls of type '{nameof(BusinessObjectReferenceDataSourceControl)}'.");
      }

      return bocControl;
    }
  }
}
