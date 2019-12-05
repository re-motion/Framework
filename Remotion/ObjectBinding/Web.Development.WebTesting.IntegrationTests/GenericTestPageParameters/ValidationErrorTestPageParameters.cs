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
  /// <see cref="IGenericTestPageParameter"/> for <see cref="ValidationErrorTestCaseFactory{TControlSelector,TControl}"/>.
  /// </summary>
  public class ValidationErrorTestPageParameters : GenericTestPageParameterBase
  {
    private const int c_parameterCount = 7;

    private string _validateButtonId;
    private string _customValidatedControlHtmlId;
    private string _customValidatedReadOnlyControlHtmlId;
    private string _multipleValidatorsControlHtmlId;
    private string _controlNotInFormGridHtmlId;
    private string _controlWithoutValidationHtmlId;
    private string _readOnlyControlHtmlId;

    public ValidationErrorTestPageParameters ()
        : base (TestConstants.ValidationErrorTestsID, c_parameterCount)
    {
    }

    /// <summary>
    /// Html id of validate button control
    /// </summary>
    public string ValidateButtonId
    {
      get { return _validateButtonId; }
    }

    /// <summary>
    /// Html id of a control with custom validator inside a form grid.
    /// </summary>
    public string CustomValidatedControlHtmlId
    {
      get { return _customValidatedControlHtmlId; }
    }

    /// <summary>
    /// Html id of a read-only control with custom validator inside a form grid.
    /// </summary>
    public string CustomValidatedReadOnlyControlHtmlId
    {
      get { return _customValidatedReadOnlyControlHtmlId; }
    }

    /// <summary>
    /// Html id of a control with multiple validators inside a form grid.
    /// </summary>
    public string MultipleValidatorsControlHtmlId
    {
      get { return _multipleValidatorsControlHtmlId; }
    }

    /// <summary>
    /// HTML id of a control outside of a form grid.
    /// </summary>
    public string ControlNotInFormGridHtmlId
    {
      get { return _controlNotInFormGridHtmlId; }
    }

    /// <summary>
    /// Html id of a control without validator inside a form grid.
    /// </summary>
    public string ControlWithoutValidationHtmlId
    {
      get { return _controlWithoutValidationHtmlId; }
    }

    /// <summary>
    /// Html id of a readonly control.
    /// </summary>
    public string ReadOnlyControlHtmlId
    {
      get { return _readOnlyControlHtmlId; }
    }

    /// <inheritdoc />
    public override void Apply (GenericTestPageParameter data)
    {
      base.Apply (data);

      _validateButtonId = data[0];
      _customValidatedControlHtmlId = data[1];
      _customValidatedReadOnlyControlHtmlId = data[2];
      _multipleValidatorsControlHtmlId = data[3];
      _controlNotInFormGridHtmlId = data[4];
      _controlWithoutValidationHtmlId = data[5];
      _readOnlyControlHtmlId = data[6];
    }
  }
}