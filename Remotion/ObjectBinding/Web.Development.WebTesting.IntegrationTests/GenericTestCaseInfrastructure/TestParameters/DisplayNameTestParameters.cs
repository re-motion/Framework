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
using Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure.Factories;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure.TestParameters
{
  /// <summary>
  /// Parameters for <see cref="DisplayNameControlSelectorTestCaseFactory{TControlSelector,TControl}"/>.
  /// </summary>
  public class DisplayNameTestParameters : ITestParameters
  {
    private string _visibleDisplayName;
    private string _hiddenDisplayName;
    private string _foundControlID;

    public DisplayNameTestParameters ()
    {
    }

    public string VisibleDisplayName
    {
      get { return _visibleDisplayName; }
    }

    public string HiddenDisplayName
    {
      get { return _hiddenDisplayName; }
    }

    public string FoundControlID
    {
      get { return _foundControlID; }
    }

    /// <inheritdoc />
    public string Name
    {
      get { return TestConstants.DisplayName; }
    }

    /// <inheritdoc />
    public bool Apply (string[] data)
    {
      if (data.Length != 3)
        return false;

      _visibleDisplayName = data[0];
      _hiddenDisplayName = data[1];
      _foundControlID = data[2];
      return true;
    }
  }
}