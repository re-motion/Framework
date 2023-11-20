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
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Validation;
using Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.Controls;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Validation.Results;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared
{
  public partial class ControlTestForm : WxePage
  {
    private IDataEditControl _dataEditControl;

    private new ControlTestFunction CurrentFunction
    {
      get { return (ControlTestFunction)base.CurrentFunction; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      LoadUserControl();
      LoadTestOutputUserControl();

      ValidateButton.Click += new EventHandler(ValidateButton_Click);
    }

    protected override void OnLoad (EventArgs e)
    {
      base.OnLoad(e);

      PopulateDataSources();
      LoadValues(IsPostBack);
    }

    private void LoadUserControl ()
    {
      var control = LoadControl(CurrentFunction.UserControl);
      _dataEditControl = (IDataEditControl)control;
      _dataEditControl.ID = "DataEditControl";

      ControlPlaceHolder.Controls.Add(control);
    }

    private void LoadTestOutputUserControl ()
    {
      var testOutputControlPath = CurrentFunction.UserControl.Replace(".ascx", "TestOutput.ascx");
      var testOutputControl = LoadControl(testOutputControlPath);

      TestOutputControlPlaceHolder.Controls.Add(testOutputControl);
    }

    private void ValidateButton_Click (object sender, EventArgs e)
    {
      ValidateDataSources();
    }

    private void PopulateDataSources ()
    {
      if (_dataEditControl != null)
        _dataEditControl.BusinessObject = (IBusinessObject)CurrentFunction.Person;
    }

    private void LoadValues (bool interim)
    {
      if (_dataEditControl != null)
        _dataEditControl.LoadValues(interim);
    }

    private bool SaveValues (bool interim)
    {
      if (_dataEditControl == null)
        return true;

      return _dataEditControl.SaveValues(interim);
    }

    private bool ValidateDataSources ()
    {
      PrepareValidation();

      if (_dataEditControl == null)
        return true;

      var validatableControls = _dataEditControl.DataSource.GetAllBoundControls().Where(c => c.HasValidBinding).OfType<IValidatableControl>();

      var person = (Person)_dataEditControl.BusinessObject;

      var personDateOfBirthProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.DateOfBirth));
      var personDeceasedProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.Deceased));
      var personPartnerProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.Partner));
      var personMarriageStatusProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.MarriageStatus));
      var personCVProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.CV));
      var personCVStringProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.CVString));
      var personLastNameProperty = PropertyInfoAdapter.Create(MemberInfoFromExpressionUtility.GetProperty((Person _) => _.LastName));

      var validationResult = BusinessObjectValidationResult.Create(
          new ValidationResult(
              new ValidationFailure[]
              {
                ValidationFailure.CreatePropertyValidationFailure(
                    person,
                    personDateOfBirthProperty,
                    person.DateOfBirth,
                    "invalid dateOfBirth.",
                    "Localized invalid dateOfBirth."),
                ValidationFailure.CreatePropertyValidationFailure(
                    person,
                    personDeceasedProperty,
                    person.Deceased,
                    "invalid deceased status.",
                    "Localized invalid deceased status."),
                ValidationFailure.CreatePropertyValidationFailure(
                    person,
                    personPartnerProperty,
                    person.Partner,
                    "invalid partner.",
                    "Localized invalid partner."),
                ValidationFailure.CreatePropertyValidationFailure(
                    person,
                    personMarriageStatusProperty,
                    person.MarriageStatus,
                    "invalid marriage status.",
                    "Localized invalid marriage status."),
                ValidationFailure.CreatePropertyValidationFailure(
                    person,
                    personCVProperty,
                    person.CV,
                    "invalid CV.",
                    "Localized invalid CV."),
                ValidationFailure.CreatePropertyValidationFailure(
                    person,
                    personCVStringProperty,
                    person.CVString,
                    "invalid CV string.",
                    "Localized invalid CV string."),
                ValidationFailure.CreatePropertyValidationFailure(
                    person,
                    personLastNameProperty,
                    person.LastName,
                    "invalid last name.",
                    "Localized invalid last name.")
              }));

      if (_dataEditControl is IDataControlWithValidationDispatcher controlWithValidationDispatcher)
      {
        controlWithValidationDispatcher.DataSourceDispatchingValidator.DispatchValidationFailures(validationResult);
      }

      return _dataEditControl.Validate();
    }
  }
}
