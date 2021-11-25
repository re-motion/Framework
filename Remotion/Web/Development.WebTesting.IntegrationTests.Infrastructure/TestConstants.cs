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

namespace Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure
{
  /// <summary>
  /// Contains test specific constants that are used in the IntegrationTest projects as well as the TestSite projects.
  /// </summary>
  public class TestConstants
  {
    protected string BodyConstant => "body_";

    public string AmbiguousControlID => "AmbiguousControl";
    public string DisabledControlID => "DisabledControl";
    public string ReadOnlyControlID => "ReadOnlyControl";
    public string HiddenControlID => "HiddenControl";
    public string VisibleControlID => "VisibleControl";

    public string AmbiguousTextContentID => "AmbiguousTextContent";
    public string DisabledTextContentID => "DisabledTextContent";
    public string HiddenTextContentID => "HiddenTextContent";
    public string VisibleTextContentID => "VisibleTextContent";

    public string AmbiguousTitleID => "AmbiguousTitle";
    public string DisabledTitleID => "DisabledTitle";
    public string HiddenTitleID => "HiddenTitle";
    public string VisibleTitleID => "VisibleTitle";

    public string GenericPageUrlTemplate => "GenericTest.wxe?control={0}&type={1}";
    public string GenericPageOutputID => "TestInformationOutput";

    public string DisplayNameSelectorID => "DisplayName";
    public string DomainPropertySelectorID => "DomainProperty";
    public string FirstSelectorID => "First";
    public string HtmlIDSelectorID => "HtmlID";
    public string IndexSelectorID => "Index";
    public string ItemIDSelectorID => "ItemID";
    public string LocalIDSelectorID => "LocalID";
    public string SingleSelectorID => "Single";
    public string TextContentSelectorID => "TextContent";
    public string TitleSelectorID => "Title";

    public string DisabledTestsID => "Disabled";
    public string ReadOnlyTestsID => "ReadOnly";

    public string LabelTestsID => "Label";
    public string ValidationErrorTestsID => "ValidationError";

    public string VisibleIndex => "1";
    public string HiddenIndex => "133";

    public string DisabledHtmlID => BodyConstant + DisabledControlID;
    public string HiddenHtmlID => BodyConstant + HiddenControlID;
    public string VisibleHtmlID => BodyConstant + VisibleControlID;
  }
}
