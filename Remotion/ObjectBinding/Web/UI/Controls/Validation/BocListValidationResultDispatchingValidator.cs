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
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.ObjectBinding.Validation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation
{
  public sealed class BocListValidationResultDispatchingValidator : BaseValidator, IBusinessObjectBoundEditableWebControlValidationResultDispatcher
  {
    private BusinessObjectValidationFailure[] _validationFailuresForBusinessObjectPropertyOfBocList = Array.Empty<BusinessObjectValidationFailure>();

    public BocListValidationResultDispatchingValidator ()
    {
    }

    public void DispatchValidationFailures (IBusinessObjectValidationResult validationResult)
    {
      ArgumentUtility.CheckNotNull("validationResult", validationResult);

      var bocListControl = GetControlToValidate();
      var dispatchToValidationFailureRepository = bocListControl
          .GetColumnDefinitions()
          .Any(cd => cd is BocValidationErrorIndicatorColumnDefinition);

      DispatchToValidationResultDispatchers(validationResult, bocListControl);

      if (dispatchToValidationFailureRepository)
        DispatchToValidationFailureRepository(validationResult, bocListControl);

      //TODO-RM-5906: this call could be optimized away and replaced by an explicit call to Validate() from an external source.
      //              Possibly, we could also tweak this to only track if validate has been called before, and if so, we set the validator invalid.
      //              If it wasn't called before, we don't do anything, someone will call validate for us anyway.
      Validate();
    }

    private void DispatchToValidationResultDispatchers (IBusinessObjectValidationResult validationResult, BocList bocListControl)
    {
      var validatorsMatchingToControls = EnumerableUtility.SelectRecursiveDepthFirst(
              bocListControl as Control,
              child => child.Controls.Cast<Control>().Where(item => item is not INamingContainer))
          .OfType<IBusinessObjectBoundEditableWebControlValidationResultDispatcher>();

      foreach (var validator in validatorsMatchingToControls)
        validator.DispatchValidationFailures(validationResult);
    }

    private void DispatchToValidationFailureRepository (IBusinessObjectValidationResult validationResult, BocList bocListControl)
    {
      var validationFailureRepository = bocListControl.ValidationFailureRepository;
      validationFailureRepository.ClearAllValidationFailures();

      var rowObjects = bocListControl.Value;

      if (rowObjects != null)
      {
        var columnsAndMatchers = bocListControl
            .GetColumnDefinitions()
            .OfType<IBocColumnDefinitionWithValidationSupport>()
            .Select(c => new { Column = (BocColumnDefinition)c, Matcher = c.GetValidationFailureMatcher() })
            .ToArray();

        foreach (var rowObject in rowObjects)
        {
          foreach (var columnAndMatcher in columnsAndMatchers)
          {
            var matchedFailures = columnAndMatcher.Matcher.GetMatchingValidationFailures(rowObject, validationResult);
            if (matchedFailures.Count > 0)
              validationFailureRepository.AddValidationFailuresForDataCell(rowObject, columnAndMatcher.Column, matchedFailures);
          }

          var otherRowFailures = validationResult.GetUnhandledValidationFailures(rowObject);
          if (otherRowFailures.Count > 0)
            validationFailureRepository.AddValidationFailuresForDataRow(rowObject, otherRowFailures);
        }
      }

      var businessObject = bocListControl.DataSource?.BusinessObject;
      var property = bocListControl.Property;

      if (businessObject == null)
        return;
      if (property == null)
        return;

      var listPropertyFailures = validationResult.GetValidationFailures(businessObject, property, markAsHandled: true);
      if (listPropertyFailures.Count > 0)
        validationFailureRepository.AddValidationFailuresForBocList(listPropertyFailures);
    }

    protected override bool EvaluateIsValid ()
    {
      var sb = new StringBuilder();

      var bocList = GetControlToValidate();
      var validationFailureRepository = bocList.ValidationFailureRepository;

      foreach (var validationFailure in validationFailureRepository.GetUnhandledValidationFailuresForBocList(true))
      {
        sb.AppendLine(validationFailure.Failure.ErrorMessage);
      }

      var rowObjects = bocList.Value ?? Array.Empty<IBusinessObject>();
      var hasRowsWithValidationFailures = rowObjects.Any(r => validationFailureRepository.HasValidationFailuresForDataRow(r));

      if (hasRowsWithValidationFailures)
      {
        sb.Append("### BocList has validation failures.");
      }

      ErrorMessage = sb.ToString();

      return _validationFailuresForBusinessObjectPropertyOfBocList.Length == 0 && hasRowsWithValidationFailures;
    }

    protected override bool ControlPropertiesValid ()
    {
      string controlToValidate = ControlToValidate;
      if (string.IsNullOrEmpty(controlToValidate))
        return base.ControlPropertiesValid();
      else
        return NamingContainer.FindControl(controlToValidate) != null;
    }

    [NotNull]
    private BocList GetControlToValidate ()
    {
      var control = NamingContainer.FindControl(ControlToValidate);
      var bocListControl = control as BocList;
      if (bocListControl == null)
      {
        throw new InvalidOperationException(
            $"'{nameof(BocListValidationResultDispatchingValidator)}' may only be applied to controls of type '{nameof(BocList)}'.");
      }

      return bocListControl;
    }
  }
}
