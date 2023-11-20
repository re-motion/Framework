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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.Validation;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Shared.Controls
{
  public partial class BocEnumValueUserControl : DataEditUserControl, IDataControlWithValidationDispatcher
  {
    public BindableObjectDataSourceControlValidationResultDispatchingValidator DataSourceDispatchingValidator => CurrentObjectValidationResultDispatchingValidator;

    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    public override bool Validate ()
    {
      var noObjectIsValid = NoObject.Validate();
      var baseObjectIsValid = base.Validate();

      return noObjectIsValid && baseObjectIsValid;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender(e);
      SetTestOutput();
    }

    private void SetTestOutput ()
    {
      TestOutput.SetCurrentValueDropDownListNormal(
          MarriageStatusField_DropDownListNormal.Value != null ? MarriageStatusField_DropDownListNormal.Value.ToString() : "");
      TestOutput.SetCurrentValueDropDownListNoAutoPostBack(MarriageStatusField_DropDownListNoAutoPostBack.Value.ToString());

      TestOutput.SetCurrentValueListBoxNormal(
          MarriageStatusField_ListBoxNormal.Value != null ? MarriageStatusField_ListBoxNormal.Value.ToString() : "");
      TestOutput.SetCurrentValueListBoxNoAutoPostBack(MarriageStatusField_ListBoxNoAutoPostBack.Value.ToString());

      TestOutput.SetCurrentValueRadioButtonListNormal(
          MarriageStatusField_RadioButtonListNormal.Value != null ? MarriageStatusField_RadioButtonListNormal.Value.ToString() : "");
      TestOutput.SetCurrentValueRadioButtonListNoAutoPostBack(MarriageStatusField_RadioButtonListNoAutoPostBack.Value.ToString());
      TestOutput.SetCurrentValueRadioButtonListMultiColumn(
          MarriageStatusField_RadioButtonListMultiColumn.Value != null ? MarriageStatusField_RadioButtonListMultiColumn.Value.ToString() : "");
      TestOutput.SetCurrentValueRadioButtonListFlow(
          MarriageStatusField_RadioButtonListFlow.Value != null ? MarriageStatusField_RadioButtonListFlow.Value.ToString() : "");
      TestOutput.SetCurrentValueRadioButtonListOrderedList(
          MarriageStatusField_RadioButtonListOrderedList.Value != null ? MarriageStatusField_RadioButtonListOrderedList.Value.ToString() : "");
      TestOutput.SetCurrentValueRadioButtonListUnorderedList(
          MarriageStatusField_RadioButtonListUnorderedList.Value != null ? MarriageStatusField_RadioButtonListUnorderedList.Value.ToString() : "");
      TestOutput.SetCurrentValueRadioButtonListLabelLeft(
          MarriageStatusField_RadioButtonListLabelLeft.Value != null ? MarriageStatusField_RadioButtonListLabelLeft.Value.ToString() : "");
    }

    private BocEnumValueUserControlTestOutput TestOutput
    {
      get { return (BocEnumValueUserControlTestOutput)((Layout)Page.Master).GetTestOutputControl(); }
    }
  }
}
