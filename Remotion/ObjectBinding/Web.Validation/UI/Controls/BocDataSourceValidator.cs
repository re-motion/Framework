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
using FluentValidation.Results;
using Remotion.FunctionalProgramming;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Validation.UI.Controls
{
  public class BocDataSourceValidator : BaseValidator, IBocValidator
  {
    private List<ValidationFailure> _unhandledFailures = new List<ValidationFailure>();
    
    public IEnumerable<ValidationFailure> ApplyValidationFailures (IEnumerable<ValidationFailure> failures)
    {
      var control = NamingContainer.FindControl (ControlToValidate);
      var dataSourceControl = control as BindableObjectDataSourceControl;
      if (dataSourceControl == null)
        throw new InvalidOperationException ("BocDataSourceValidator may only be applied to controls of type BindableObjectDataSourceControl.");

      // Can only find Validation errors for controls located within the same NamingContainer as the DataSource.
      // This applies also to ReferenceDataSources. 
      var namingContainer = dataSourceControl.NamingContainer;
      var validators =
          EnumerableUtility.SelectRecursiveDepthFirst (
              namingContainer,
              child => child.Controls.Cast<Control>().Where (item => !(item is INamingContainer)))
              .OfType<IBocValidator>();

      var unhandledFailures = failures;
      var referenceDataSourceValidators = new List<BocReferenceDataSourceValidator>();

      // ReSharper disable LoopCanBeConvertedToQuery
      foreach (var validator in validators)
      // ReSharper restore LoopCanBeConvertedToQuery
      {
        if (validator is BocReferenceDataSourceValidator)
          referenceDataSourceValidators.Add ((BocReferenceDataSourceValidator) validator);
        else
          unhandledFailures = validator.ApplyValidationFailures (unhandledFailures);
      }

      // ReSharper disable LoopCanBeConvertedToQuery
      foreach (var validator in referenceDataSourceValidators)
          // ReSharper restore LoopCanBeConvertedToQuery
      {
        unhandledFailures = validator.ApplyValidationFailures (unhandledFailures);
      }

      _unhandledFailures = unhandledFailures.ToList();
      ErrorMessage = string.Join ("\r\n", _unhandledFailures.Select (f => f.ErrorMessage));

      if (_unhandledFailures.Any())
        Validate();
      return _unhandledFailures;
    }

    protected override bool EvaluateIsValid ()
    {
      return !_unhandledFailures.Any();
    }

    protected override bool ControlPropertiesValid ()
    {
      return true;
    }
  }
}