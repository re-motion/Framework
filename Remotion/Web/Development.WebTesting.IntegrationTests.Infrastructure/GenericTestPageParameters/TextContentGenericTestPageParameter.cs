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
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.TestCaseFactories;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.GenericTestPageParameters
{
  /// <summary>
  /// <see cref="IGenericTestPageParameter"/> for <see cref="TextContentControlSelectorTestCaseFactory{TControlSelector,TControl}"/>.
  /// </summary>
  public class TextContentGenericTestPageParameter : GenericTestPageParameterBase
  {
    private const int c_parameterCount = 3;

    /// <summary>
    /// Text content of the visible control.
    /// </summary>
    public string VisibleControlTextContent { get; private set; }

    /// <summary>
    /// Text content of the hidden control.
    /// </summary>
    public string HiddenControlTextContent { get; private set; }

    /// <summary>
    /// HTML id of the control with the <see cref="VisibleControlTextContent"/>.
    /// </summary>
    public string FoundControlID { get; private set; }

    public TextContentGenericTestPageParameter ()
        : base(TestConstants.TextContentSelectorID, c_parameterCount)
    {
    }

    /// <inheritdoc />
    public override void Apply (GenericTestPageParameter data)
    {
      base.Apply(data);

      VisibleControlTextContent = data.Arguments[0];
      HiddenControlTextContent = data.Arguments[1];
      FoundControlID = data.Arguments[2];
    }
  }
}