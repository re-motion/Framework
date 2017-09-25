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
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.GenericTestPageParameters;
using Remotion.Web.Development.WebTesting.IntegrationTests.TestCaseFactories;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.GenericTestPageParameters
{
  /// <summary>
  /// <see cref="IGenericTestPageParameter"/> for <see cref="ItemIDControlSelectorTestCaseFactory{TControlSelector,TControl}"/>.
  /// </summary>
  public class ItemIDGenericTestPageParameter : GenericTestPageParameterBase
  {
    private string _foundControlID;
    private string _hiddenControlItemID;
    private string _visibleControlItemID;

    public ItemIDGenericTestPageParameter ()
        : base (TestConstants.ItemIDSelectorID, 3)
    {
    }

    /// <summary>
    /// HTML if of the element with <see cref="VisibleControlItemID"/>.
    /// </summary>
    public string FoundControlID
    {
      get { return _foundControlID; }
    }

    /// <summary>
    /// Item id of the hidden control.
    /// </summary>
    public string HiddenControlItemID
    {
      get { return _hiddenControlItemID; }
    }

    /// <summary>
    /// Item id of the visible control.
    /// </summary>
    public string VisibleControlItemID
    {
      get { return _visibleControlItemID; }
    }

    /// <inheritdoc />
    public override void Apply (GenericTestPageParameter data)
    {
      base.Apply (data);

      _visibleControlItemID = data[0];
      _hiddenControlItemID = data[1];
      _foundControlID = data[2];
    }
  }
}