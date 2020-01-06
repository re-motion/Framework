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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Validation.Results;

namespace Remotion.ObjectBinding.Web.Validation.UI.Controls
{
  public sealed class BocDataSourceValidationFailureDisptachingValidator
      : BaseValidator, IBusinessObjectBoundEditableWebControlValidationFailureDispatcher
  {
    public BocDataSourceValidationFailureDisptachingValidator ()
    {
    }

    public IEnumerable<ValidationFailure> DispatchValidationFailures (IEnumerable<ValidationFailure> failures)
    {
      ArgumentUtility.CheckNotNull ("failures", failures);

      var control = NamingContainer.FindControl (ControlToValidate);
      var dataSourceControl = control as BindableObjectDataSourceControl;
      if (dataSourceControl == null)
      {
        throw new InvalidOperationException (
            "BocDataSourceValidationFailureDisptachingValidator may only be applied to controls of type BindableObjectDataSourceControl.");
      }

      // Can only find Validation errors for controls located within the same NamingContainer as the DataSource.
      // This applies also to ReferenceDataSources. 
      var namingContainer = dataSourceControl.NamingContainer;
      var validators =
          EnumerableUtility.SelectRecursiveDepthFirst (
              namingContainer,
              child => child.Controls.Cast<Control>().Where (item => !(item is INamingContainer)))
              .OfType<IBusinessObjectBoundEditableWebControlValidationFailureDispatcher>();

      var controlsWithValidBinding = dataSourceControl.GetBoundControlsWithValidBinding().Cast<Control>();
      var validatorsMatchingToControls = controlsWithValidBinding.Join (
          validators,
          c => c.ID,
          v => ((BaseValidator) v).ControlToValidate,
          (c, v) => v);

      return validatorsMatchingToControls.Aggregate (failures, (f, v) => v.DispatchValidationFailures (f)).ToList();
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
  }
}