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
using System.Text;
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.EditableRowSupport;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Validation;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls.Validation
{
  public sealed class BocListValidationResultDispatchingValidator
      : BaseValidator, IBusinessObjectBoundEditableWebControlValidationResultDispatcher, IValidatorWithDynamicErrorMessage
  {
    private readonly IBocListValidationFailureHandler _validationFailureHandler;

    public BocListValidationResultDispatchingValidator ()
        : this(SafeServiceLocator.Current.GetInstance<IBocListValidationFailureHandler>())
    {
    }

    public BocListValidationResultDispatchingValidator (IBocListValidationFailureHandler validationFailureHandler)
    {
      ArgumentUtility.CheckNotNull("validationFailureHandler", validationFailureHandler);

      _validationFailureHandler = validationFailureHandler;
    }

    public void DispatchValidationFailures (IBusinessObjectValidationResult validationResult)
    {
      ArgumentUtility.CheckNotNull("validationResult", validationResult);

      var bocListControl = GetControlToValidate();
      if (!bocListControl.Visible)
        validationResult = new ReadOnlyBusinessObjectValidationResultDecorator(validationResult);

      DispatchToValidationResultDispatchers(validationResult, bocListControl);

      if (((IBocList)bocListControl).IsInlineValidationDisplayEnabled)
        DispatchToValidationFailureRepository(validationResult, bocListControl);

      //TODO-RM-5906: this call could be optimized away and replaced by an explicit call to Validate() from an external source.
      //              Possibly, we could also tweak this to only track if validate has been called before, and if so, we set the validator invalid.
      //              If it wasn't called before, we don't do anything, someone will call validate for us anyway.
      Validate();
    }

    private void DispatchToValidationResultDispatchers (IBusinessObjectValidationResult validationResult, IBocList bocListControl)
    {
      var rowObjects = bocListControl.Value ?? Array.Empty<IBusinessObject>();
      var columnDefinitions = bocListControl.GetColumnDefinitions();
      var editModeController = bocListControl.EditModeController;
      if (editModeController.IsRowEditModeActive)
      {
        var bocListRow = editModeController.GetEditedRow();
        var editableRow = editModeController.GetEditableRow(bocListRow.Index);
        if (editableRow != null)
          DispatchEditableRowValidationFailures(editableRow, validationResult, columnDefinitions);
      }
      else if (editModeController.IsListEditModeActive)
      {
        for (var i = 0; i < rowObjects.Count; i++)
        {
          var editableRow = editModeController.GetEditableRow(i);
          if (editableRow != null)
            DispatchEditableRowValidationFailures(editableRow, validationResult, columnDefinitions);
        }
      }
    }

    private void DispatchToValidationFailureRepository (IBusinessObjectValidationResult validationResult, IBocList bocListControl)
    {
      var validationFailureRepository = bocListControl.ValidationFailureRepository;
      validationFailureRepository.ClearAllValidationFailures();

      var rowObjects = bocListControl.GetBusinessObjectsForValidation();

      if (rowObjects != null)
      {
        var columnDefinitions = bocListControl.GetColumnDefinitions();
        var columnsAndMatchers = columnDefinitions
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

          var otherRowFailures = validationResult.GetUnhandledValidationFailures(rowObject, markAsHandled: true);
          if (otherRowFailures.Count > 0)
            validationFailureRepository.AddValidationFailuresForDataRow(rowObject, otherRowFailures);
        }

        var editModeController = bocListControl.EditModeController;
        if (editModeController.IsRowEditModeActive)
        {
          var bocListRow = editModeController.GetEditedRow();
          var editableRow = editModeController.GetEditableRow(bocListRow.Index);
          if (editableRow != null)
            DispatchEditableRowValidationFailuresToValidationRepository(editableRow, bocListRow.BusinessObject, columnDefinitions, validationFailureRepository);
        }
        else if (editModeController.IsListEditModeActive)
        {
          for (var i = 0; i < rowObjects.Count; i++)
          {
            var editableRow = editModeController.GetEditableRow(i);
            if (editableRow != null)
              DispatchEditableRowValidationFailuresToValidationRepository(editableRow, rowObjects[i], columnDefinitions, validationFailureRepository);
          }
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
      var bocList = GetControlToValidate();
      var validationFailureRepository = bocList.ValidationFailureRepository;

      var failuresOnBocList = validationFailureRepository.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(false);

      // We check for all validation failures here, as none would/should be marked as handled yet anyways.
      if (failuresOnBocList.Count == 0)
      {
        ErrorMessage = "";
        return true;
      }
      else
      {
        ErrorMessage = GetControlToValidate().GetResourceManager().GetString(BocList.ResourceIdentifier.ValidationFailuresFoundInListErrorMessage);
        return false;
      }
    }

    protected override bool ControlPropertiesValid ()
    {
      string controlToValidate = ControlToValidate;
      if (string.IsNullOrEmpty(controlToValidate))
        return base.ControlPropertiesValid();
      else
        return NamingContainer.FindControl(controlToValidate) != null;
    }

    private void DispatchEditableRowValidationFailures (
        IEditableRow editableRow,
        IBusinessObjectValidationResult validationResult,
        IReadOnlyList<BocColumnDefinition> columnDefinitions)
    {
      for (var i = 0; i < columnDefinitions.Count; i++)
      {
        var validators = editableRow.GetValidators(i);
        if (validators != null)
        {
          foreach (BaseValidator baseValidator in validators)
          {
            if (baseValidator is IBusinessObjectBoundEditableWebControlValidationResultDispatcher resultDispatcher)
            {
              resultDispatcher.DispatchValidationFailures(validationResult);
            }
          }
        }
      }
    }

    private void DispatchEditableRowValidationFailuresToValidationRepository (
        IEditableRow editableRow,
        IBusinessObject rowObject,
        IReadOnlyList<BocColumnDefinition> columnDefinitions,
        IBocListValidationFailureRepository validationFailureRepository)
    {
      for (var i = 0; i < columnDefinitions.Count; i++)
      {
        var controlCollection = editableRow.GetValidators(i);
        if (controlCollection != null)
        {
          foreach (BaseValidator baseValidator in controlCollection)
          {
            if (baseValidator is not IBusinessObjectBoundEditableWebControlValidationResultDispatcher && !baseValidator.IsValid)
            {
              var businessObjectProperties =
                  ((BocSimpleColumnDefinition)columnDefinitions[i])
                  .GetPropertyPath()
                  .Properties;

              Assertion.IsTrue(businessObjectProperties.Count == 1, "There should be exactly one property found on a column definition in edit mode.");

              var property = businessObjectProperties.Single();

              validationFailureRepository.AddValidationFailuresForDataCell(
                  rowObject,
                  columnDefinitions[i],
                  new[] { BusinessObjectValidationFailure.CreateForBusinessObjectProperty(baseValidator.ErrorMessage, rowObject, property) });
            }
          }
        }
      }
    }

    [NotNull]
    private IBocList GetControlToValidate ()
    {
      var control = NamingContainer.FindControl(ControlToValidate);
      var bocListControl = control as IBocList;
      if (bocListControl == null)
      {
        throw new InvalidOperationException(
            $"'{nameof(BocListValidationResultDispatchingValidator)}' may only be applied to controls of type '{nameof(BocList)}'.");
      }

      return bocListControl;
    }

    void IValidatorWithDynamicErrorMessage.RefreshErrorMessage ()
    {
      var bocList = GetControlToValidate();

      var context = new ValidationFailureHandlingContext(bocList);
      _validationFailureHandler.HandleValidationFailures(context);

      var errorMessageBuilder = new StringBuilder();

      context.AppendErrorMessages(errorMessageBuilder);

      if (bocList.ValidationFailureRepository.GetUnhandledValidationFailuresForBocListAndContainingDataRowsAndDataCells(true).Any() || errorMessageBuilder.Length == 0)
      {
        errorMessageBuilder.Append(bocList.GetResourceManager().GetString(BocList.ResourceIdentifier.ValidationFailuresFoundInListErrorMessage));
      }

      ErrorMessage = errorMessageBuilder.ToString();
    }
  }
}
