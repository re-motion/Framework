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

    /// <summary>
    /// Html id of validate button control
    /// </summary>
    public string ValidateButtonId { get; private set; }

    /// <summary>
    /// Html id of a control with custom validator inside a form grid.
    /// </summary>
    public string CustomValidatedControlHtmlId { get; private set; }

    /// <summary>
    /// Html id of a read-only control with custom validator inside a form grid.
    /// </summary>
    public string CustomValidatedReadOnlyControlHtmlId { get; private set; }

    /// <summary>
    /// Html id of a control with multiple validators inside a form grid.
    /// </summary>
    public string MultipleValidatorsControlHtmlId { get; private set; }

    /// <summary>
    /// HTML id of a control outside of a form grid.
    /// </summary>
    public string ControlNotInFormGridHtmlId { get; private set; }

    /// <summary>
    /// Html id of a control without validator inside a form grid.
    /// </summary>
    public string ControlWithoutValidationHtmlId { get; private set; }

    /// <summary>
    /// Html id of a read-only control.
    /// </summary>
    public string ReadOnlyControlHtmlId { get; private set; }

    public ValidationErrorTestPageParameters ()
        : base(TestConstants.ValidationErrorTestsID, c_parameterCount)
    {
    }

    /// <inheritdoc />
    public override void Apply (GenericTestPageParameter data)
    {
      base.Apply(data);

      ValidateButtonId = data.Arguments[0];
      CustomValidatedControlHtmlId = data.Arguments[1];
      CustomValidatedReadOnlyControlHtmlId = data.Arguments[2];
      MultipleValidatorsControlHtmlId = data.Arguments[3];
      ControlNotInFormGridHtmlId = data.Arguments[4];
      ControlWithoutValidationHtmlId = data.Arguments[5];
      ReadOnlyControlHtmlId = data.Arguments[6];
    }
  }
}
