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
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure;
using Remotion.Web.Development.WebTesting.TestSite.Infrastructure;
using Remotion.Web.Development.WebTesting.TestSite.Shared.GenericPages;
using Remotion.Web.UI.Controls;
using GenericTestPageParameter = Remotion.Web.Development.WebTesting.IntegrationTests.Infrastructure.GenericTestPageParameter;

namespace Remotion.Web.Development.WebTesting.TestSite.Shared
{
  public partial class GenericTest : GenericTestPageBase<GenericTestOptions>
  {
    protected override Dictionary<string, GenericTestPageParameter> Parameters { get; } = new Dictionary<string, GenericTestPageParameter>();

    public string FrameTestFrameUrl { get; }

    public GenericTest ()
    {
      FrameTestFrameUrl = this.ResolveRootResource("FrameTestFrame.aspx");

      Register("anchor", new AnchorGenericTestPage());
      Register("command", new CommandGenericTestPage());
      Register("dropDownList", new SimpleGenericTestPage<DropDownList>());
      Register("dropDownMenu", new DropDownMenuGenericTestPage());
      Register("formGrid", new FormGridGenericTestPage());
      Register("imageButton", new SimpleGenericTestPage<ImageButton>());
      Register("image", new SimpleGenericTestPage<Image>());
      Register("label", new LabelGenericTestPage());
      Register("listMenu", new ListMenuGenericTestPage());
      Register("scope", new SimpleGenericTestPage<Panel>());
      Register("singleView", new SimpleGenericTestPage<SingleView>());
      Register("tabbedMenu", new TabbedMenuGenericTestPage());
      Register("tabbedMultiView", new SimpleGenericTestPage<TabbedMultiView>());
      Register("textBox", new SimpleGenericTestPage<TextBox>());
      Register("treeView", new TreeViewGenericTestPage());
      Register("webButton", new WebButtonGenericTestPage());
      Register("webTabStrip", new SimpleGenericTestPage<WebTabStrip>());
      Register("webTreeView", new WebTreeViewGenericTestPage());
    }

    protected override GenericTestOptions AmbiguousControlOptions => OptionsFactory.CreateAmbiguous();
    protected override GenericTestOptions VisibleControlOptions => OptionsFactory.CreateVisible();
    protected override GenericTestOptions DisabledControlOptions => OptionsFactory.CreateDisabled();
    protected override GenericTestOptions HiddenControlOptions => OptionsFactory.CreateHidden();

    protected override Control AmbiguousControlPanel => PanelAmbiguousControl;
    protected override Control DisabledControlPanel => PanelDisabledControl;
    protected override Control HiddenControlPanel => PanelHiddenControl;
    protected override Control VisibleControlPanel => PanelVisibleControl;

    private GenericTestOptionsFactory OptionsFactory => new GenericTestOptionsFactory();
    private WebGenericTestPageParameterFactory ParameterFactory => new WebGenericTestPageParameterFactory();

    /// <inheritdoc />
    protected override void OnInit (EventArgs e)
    {
      // Parameters which will be passed to the client
      Parameters.Add(ParameterFactory.CreateHtmlIDSelector());
      Parameters.Add(ParameterFactory.CreateIndexSelector());
      Parameters.Add(ParameterFactory.CreateLocalIdSelector());
      Parameters.Add(ParameterFactory.CreateFirstSelector());
      Parameters.Add(ParameterFactory.CreateSingleSelector());
      Parameters.Add(ParameterFactory.CreateDisabledTests());
      Parameters.Add(ParameterFactory.CreateTextContentSelector());
      Parameters.Add(ParameterFactory.CreateTitleSelector());
      Parameters.Add(ParameterFactory.CreateItemIDSelector());

      base.OnInit(e);
    }

    /// <inheritdoc />
    protected override void SetTestInformation (string information)
    {
      var master = Master as Layout;
      if (master == null)
        throw new InvalidOperationException("The master page of the generic test page is not set.");
      master.SetTestInformation(information);
    }
  }
}
