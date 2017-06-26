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
using Remotion.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure.Factories;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure.TestParameters
{
  /// <summary>
  /// Parameters for <see cref="TextContentControlSelectorTestCaseFactory{TControlSelector,TControl}"/>.
  /// </summary>
  public class TextContentTestParameters : ITestParameters
  {
    private string _visibleControlTextContent;
    private string _hiddenControlTextContent;
    private string _foundControlID;

    public TextContentTestParameters ()
    {
    }

    public string VisibleControlTextContent
    {
      get { return _visibleControlTextContent; }
    }

    public string HiddenControlTextContent
    {
      get { return _hiddenControlTextContent; }
    }

    public string FoundControlID
    {
      get { return _foundControlID; }
    }

    /// <inheritdoc />
    public string Name
    {
      get { return TestConstants.TextContent; }
    }

    /// <inheritdoc />
    public bool Apply (string[] data)
    {
      if (data.Length != 3)
        return false;

      _visibleControlTextContent = data[0];
      _hiddenControlTextContent = data[1];
      _foundControlID = data[2];
      return true;
    }
  }
}