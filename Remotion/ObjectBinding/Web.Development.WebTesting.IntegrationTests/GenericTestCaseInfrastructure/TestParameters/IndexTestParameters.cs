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
  /// Parameters for <see cref="IndexControlSelectorTestCaseFactory{TControlSelector,TControl}"/>.
  /// </summary>
  public class IndexTestParameters : ITestParameters
  {
    private int _visibleControlIndex;
    private int _hiddenControlIndex;
    private string _foundControlID;

    public IndexTestParameters ()
    {
    }

    public int VisibleControlIndex
    {
      get { return _visibleControlIndex; }
    }

    public int HiddenControlIndex
    {
      get { return _hiddenControlIndex; }
    }

    public string FoundControlID
    {
      get { return _foundControlID; }
    }

    /// <inheritdoc />
    public string Name
    {
      get { return TestConstants.Index; }
    }

    /// <inheritdoc />
    public bool Apply (string[] data)
    {
      if (data.Length != 3)
        return false;

      int visibleControlIndex;
      if (!int.TryParse (data[0], out visibleControlIndex))
        return false;
      _visibleControlIndex = visibleControlIndex;

      int hiddenControlIndex;
      if (!int.TryParse (data[1], out hiddenControlIndex))
        return false;
      _hiddenControlIndex = hiddenControlIndex;

      _foundControlID = data[2];
      return true;
    }
  }
}