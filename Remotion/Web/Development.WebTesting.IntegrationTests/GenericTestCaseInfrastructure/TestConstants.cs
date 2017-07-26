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
using Remotion.Web.Development.WebTesting.TestSite.GenericTestPageInfrastructure;

namespace Remotion.Web.Development.WebTesting.IntegrationTests.GenericTestCaseInfrastructure
{
  /// <summary>
  /// Test specific constants that are used in multiple places with no common intersections.
  /// </summary>
  /// <remarks>
  /// Changes must be propagated to the other <c>TestConstants</c>.
  /// 
  ///  - <see cref="TestSite.GenericTestPageInfrastructure.TestConstants"/>
  /// </remarks>
  public static class TestConstants
  {
    public const string Category = "WebTest_GenericIntegrationTests";

    public const string Ok = "ok";
    public const string Fail = "fail";

    public const string HtmlID = "htmlID";
    public const string Index = "index";
    public const string LocalID = "localID";
    public const string First = "first";
    public const string Single = "single";
    public const string TextContent = "textContent";
    public const string Title = "title";
    public const string ItemID = "itemID";

    public const string TestUrlTemplate = "GenericTest.wxe?control={0}&type={1}";

    public const string TestInformationOutputID = "TestInformationOutput";
  }
}