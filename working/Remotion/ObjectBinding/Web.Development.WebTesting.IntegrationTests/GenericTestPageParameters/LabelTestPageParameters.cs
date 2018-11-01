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
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.GenericTestPageParameters;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.GenericTestPageParameters
{
  /// <summary>
  /// <see cref="IGenericTestPageParameter"/> for <see cref="LabelTestCaseFactory{TControlSelector,TControl}"/>.
  /// </summary>
  public class LabelTestPageParameters : GenericTestPageParameterBase
  {
    private const int c_parameterCount = 8;

    private string _formGridControlHtmlId;
    private string _readonlyFormGridControlHtmlId;
    private string _oneControlOverMultipleRowsFormGridControlHtmlId;
    private string _columnsShiftedFormGridControlHtmlId;
    private string _formGridMultiControl1HtmlId;
    private string _formGridMultiControl2HtmlId;
    private string _controlNotInFormGridHtmlId;
    private string _formGridLabel;

    public LabelTestPageParameters ()
        : base(TestConstants.LabelTestsID, c_parameterCount)
    {
    }

    /// <summary>
    /// HTML id of the control in the form grid with a label.
    /// </summary>
    public string FormGridControlHtmlId
    {
      get { return _formGridControlHtmlId; }
    }

    /// <summary>
    /// HTML id of a readonly control in a form grid with a label.
    /// </summary>
    public string ReadonlyFormGridControlHtmlId
    {
      get { return _readonlyFormGridControlHtmlId; }
    }

    /// <summary>
    /// HTML id of a control spanning multiple rows in form grid control.
    /// </summary>
    public string OneControlOverMultipleRowsFormGridControlHtmlId
    {
      get { return _oneControlOverMultipleRowsFormGridControlHtmlId; }
    }

    /// <summary>
    /// HTML Id of a control in a form grid where the label and control column is shifted for one position to the right.
    /// </summary>
    public string ColumnsShiftedFormGridControlHtmlId
    {
      get { return _columnsShiftedFormGridControlHtmlId; }
    }

    /// <summary>
    /// The form grid label of the control.
    /// </summary>
    public string FormGridLabel
    {
      get { return _formGridLabel; }
    }

    /// <summary>
    /// HTML Id of the first control inside a form grid with multiple controls.
    /// </summary>
    public string FormGridMultiControl1HtmlId
    {
      get { return _formGridMultiControl1HtmlId; }
    }

    /// <summary>
    /// HTML Id of the second control inside a form grid with multiple controls.
    /// </summary>
    public string FormGridMultiControl2HtmlId
    {
      get { return _formGridMultiControl2HtmlId; }
    }

    /// <summary>
    /// HTML id of a control outside of a form grid.
    /// </summary>
    public string ControlNotInFormGridHtmlId
    {
      get { return _controlNotInFormGridHtmlId; }
    }


    /// <inheritdoc />
    public override void Apply (GenericTestPageParameter data)
    {
      base.Apply (data);

      _formGridControlHtmlId = data[0];
      _readonlyFormGridControlHtmlId = data[1];
      _oneControlOverMultipleRowsFormGridControlHtmlId = data[2];
      _columnsShiftedFormGridControlHtmlId = data[3];
      _formGridMultiControl1HtmlId = data[4];
      _formGridMultiControl2HtmlId = data[5];
      _controlNotInFormGridHtmlId = data[6];
      _formGridLabel = data[7];
    }
  }
}