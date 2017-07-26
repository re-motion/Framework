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

namespace Remotion.Web.Development.WebTesting.TestSite.GenericTestPageInfrastructure
{
  /// <summary>
  /// Represent a parameter that is passed to a test via web page.
  /// </summary>
  public class TestParameter
  {
    public static readonly TestParameter HtmlID = new TestParameter (
        TestConstants.HtmlID,
        new TestArguments (TestConstants.VisibleHtmlID, TestConstants.HiddenHtmlID));

    public static readonly TestParameter Index = new TestParameter (
        TestConstants.Index,
        new TestArguments (TestConstants.VisibleIndex.ToString(), TestConstants.HiddenIndex.ToString(), TestConstants.VisibleHtmlID));

    public static readonly TestParameter LocalID = new TestParameter (
        TestConstants.LocalID,
        new TestArguments (TestConstants.VisibleLocalID, TestConstants.HiddenLocalID, TestConstants.VisibleHtmlID));

    public static readonly TestParameter First = new TestParameter (
        TestConstants.First,
        new TestArguments (TestConstants.VisibleHtmlID));

    public static readonly TestParameter Single = new TestParameter (
        TestConstants.Single,
        new TestArguments (TestConstants.VisibleHtmlID));

    public static readonly TestParameter TextContent = new TestParameter (
        TestConstants.TextContent,
        new TestArguments (TestConstants.VisibleTextContent, TestConstants.HiddenTextContent, TestConstants.VisibleHtmlID));

    public static readonly TestParameter Title = new TestParameter (
        TestConstants.Title,
        new TestArguments (TestConstants.VisibleTitle, TestConstants.HiddenTitle, TestConstants.VisibleHtmlID));

    public static readonly TestParameter ItemID = new TestParameter (
        TestConstants.ItemID,
        new TestArguments (TestConstants.VisibleLocalID, TestConstants.HiddenLocalID, TestConstants.VisibleHtmlID));

    private readonly string _name;
    private readonly TestArguments _arguments;


    public TestParameter (string name, TestArguments arguments)
    {
      _name = name;
      _arguments = arguments;
    }

    public string Name
    {
      get { return _name; }
    }

    public TestArguments Arguments
    {
      get { return _arguments; }
    }
  }
}