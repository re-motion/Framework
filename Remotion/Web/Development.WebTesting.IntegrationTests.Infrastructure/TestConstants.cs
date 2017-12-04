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

// ReSharper disable once CheckNamespace

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  /// <summary>
  /// Contains test specific constants that are used in the IntegrationTest projects as well as the TestSite projects.
  /// </summary>
  public class TestConstants
  {
    public const string GenericPageUrlTemplate = "GenericTest.wxe?control={0}&type={1}";
    public const string GenericPageOutputID = "TestInformationOutput";

    public const string DisplayNameSelectorID = "DisplayName";
    public const string DomainPropertySelectorID = "DomainProperty";
    public const string FirstSelectorID = "First";
    public const string HtmlIDSelectorID = "HtmlID";
    public const string IndexSelectorID = "Index";
    public const string ItemIDSelectorID = "ItemID";
    public const string LocalIDSelectorID = "LocalID";
    public const string SingleSelectorID = "Single";
    public const string TextContentSelectorID = "TextContent";
    public const string TitleSelectorID = "Title";

    public const string DisabledTestsID = "Disabled";
    public const string ReadOnlyTestsID = "ReadOnly";
  }
}