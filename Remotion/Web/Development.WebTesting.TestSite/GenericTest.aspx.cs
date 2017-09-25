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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.TestSite.GenericPages;
using Remotion.Web.Development.WebTesting.TestSite.Infrastructure;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.Development.WebTesting.TestSite
{
  public partial class GenericTest : GenericTestPageBase<GenericTestOptions>
  {
    private readonly GenericTestPageParameterCollection _parameters = new GenericTestPageParameterCollection();

    private GenericTestOptions _ambiguousControlOptions;
    private GenericTestOptions _hiddenControlOptions;
    private GenericTestOptions _visibleControlOptions;

    public GenericTest ()
    {
      Register ("anchor", new AnchorGenericTestPage());
      Register ("command", new CommandGenericTestPage());
      Register ("dropDownList", new SimpleGenericTestPage<DropDownList>());
      Register ("dropDownMenu", new DropDownMenuGenericTestPage());
      Register ("formGrid", new FormGridGenericTestPage());
      Register ("imageButton", new SimpleGenericTestPage<ImageButton>());
      Register ("image", new SimpleGenericTestPage<Image>());
      Register ("label", new SimpleGenericTestPage<Label>());
      Register ("listMenu", new SimpleGenericTestPage<ListMenu>());
      Register ("scope", new SimpleGenericTestPage<Panel>());
      Register ("singleView", new SimpleGenericTestPage<SingleView>());
      Register ("tabbedMenu", new SimpleGenericTestPage<TabbedMenu>());
      Register ("tabbedMultiView", new SimpleGenericTestPage<TabbedMultiView>());
      Register ("textBox", new SimpleGenericTestPage<TextBox>());
      Register ("treeView", new SimpleGenericTestPage<TreeView>());
      Register ("webButton", new WebButtonGenericTestPage());
      Register ("webTabStrip", new SimpleGenericTestPage<WebTabStrip>());
      Register ("webTreeView", new SimpleGenericTestPage<WebTreeView>());
    }

    /// <inheritdoc />
    protected override GenericTestOptions AmbiguousControlOptions
    {
      get { return _ambiguousControlOptions; }
    }

    /// <inheritdoc />
    protected override Control AmbiguousControlPanel
    {
      get { return PanelAmbiguousControl; }
    }

    /// <inheritdoc />
    protected override GenericTestOptions HiddenControlOptions
    {
      get { return _hiddenControlOptions; }
    }

    /// <inheritdoc />
    protected override Control HiddenControlPanel
    {
      get { return PanelHiddenControl; }
    }

    /// <inheritdoc />
    protected override GenericTestPageParameterCollection Parameters
    {
      get { return _parameters; }
    }

    /// <inheritdoc />
    protected override GenericTestOptions VisibleControlOptions
    {
      get { return _visibleControlOptions; }
    }

    /// <inheritdoc />
    protected override Control VisibleControlPanel
    {
      get { return PanelVisibleControl; }
    }

    /// <inheritdoc />
    protected override void OnInit (EventArgs e)
    {
      // Constants for all the controls on this generic page
      const string ambiguousID = "AmbiguousControl", ambiguousTextContent = "AmbiguousTextContent", ambiguousTitle = "AmbiguousTitle";
      const string hiddenID = "HiddenControl", hiddenTextContent = "HiddenTextContent", hiddenTitle = "HiddenTitle";
      const string visibleID = "VisibleControl", visibleTextContent = "VisibleTextContent", visibleTitle = "AmbiguousTitle";
      const string visibleIndex = "1", hiddenIndex = "133";

      // Options for creating the controls
      _ambiguousControlOptions = new GenericTestOptions (ambiguousID, ambiguousTextContent, ambiguousTitle);
      _hiddenControlOptions = new GenericTestOptions (hiddenID, hiddenTextContent, hiddenTitle);
      _visibleControlOptions = new GenericTestOptions (visibleID, visibleTextContent, visibleTitle);

      // "Real" HTML ids of the controls
      var hiddenHtmlID = string.Concat ("body_", hiddenID);
      var visibleHtmlID = string.Concat ("body_", visibleID);

      // Parameters which will be passed to the client
      _parameters.Add (TestConstants.HtmlIDSelectorID, visibleHtmlID, hiddenHtmlID);
      _parameters.Add (TestConstants.IndexSelectorID, visibleIndex, hiddenIndex, visibleHtmlID);
      _parameters.Add (TestConstants.LocalIDSelectorID, visibleID, hiddenID, visibleHtmlID);
      _parameters.Add (TestConstants.FirstSelectorID, visibleHtmlID);
      _parameters.Add (TestConstants.SingleSelectorID, visibleHtmlID);
      _parameters.Add (TestConstants.TextContentSelectorID, visibleTextContent, hiddenTextContent, visibleHtmlID);
      _parameters.Add (TestConstants.TitleSelectorID, visibleTitle, hiddenTitle, visibleHtmlID);
      _parameters.Add (TestConstants.ItemIDSelectorID, visibleID, hiddenID, visibleHtmlID);

      base.OnInit (e);
    }

    /// <inheritdoc />
    protected override void SetTestInformation (string information)
    {
      var master = Master as Layout;
      if (master == null)
        throw new InvalidOperationException ("The master page of the generic test page is not set.");
      master.SetTestInformation (information);
    }
  }
}