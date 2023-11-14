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
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.PostBackTargets;

namespace Remotion.Web.Test.Shared.MultiplePostBackCatching
{
  public class TestControlGenerator
  {
    private readonly IResourceUrlFactory _resourceUrlFactory = SafeServiceLocator.Current.GetInstance<IResourceUrlFactory>();

    public event EventHandler<IDEventArgs> Click;
    private readonly Page _page;
    private readonly PostBackEventHandler _postBackEventHandler;

    public TestControlGenerator (Page page, PostBackEventHandler postBackEventHandler)
    {
      ArgumentUtility.CheckNotNull("page", page);
      ArgumentUtility.CheckNotNull("postBackEventHandler", postBackEventHandler);

      _page = page;
      _postBackEventHandler = postBackEventHandler;
    }

    public IEnumerable GetTestControls (string prefix)
    {
      yield return CreateInputControlWithSubmitBehavior(prefix);
      yield return CreateInputControlWithButtonBehavior(prefix);
      yield return CreateButtonControlWithSubmitBehavior(prefix);
      yield return CreateButtonControlWithButtonBehavior(prefix);

      yield return CreateAnchorWithTextAndJavascriptInOnClick(prefix);
      yield return CreateAnchorWithTextAndJavascriptInHref(prefix);
      yield return CreateAnchorWithImageAndJavascriptInOnClick(prefix);
      yield return CreateAnchorWithImageAndJavascriptInHref(prefix);
      yield return CreateAnchorWithSpanAndJavascriptInOnClick(prefix);
      yield return CreateAnchorWithSpanAndJavascriptInHref(prefix);
      yield return CreateAnchorWithLabelAndJavascriptInOnClick(prefix);
      yield return CreateAnchorWithLabelAndJavascriptInHref(prefix);
      yield return CreateAnchorWithBoldAndJavascriptInOnClick(prefix);
      yield return CreateAnchorWithBoldAndJavascriptInHref(prefix);
      yield return CreateAnchorWithNonPostBackJavascriptInOnClick(prefix);
      yield return CreateAnchorWithNonPostBackJavascriptInHref(prefix);
    }

    public bool IsAlertHyperLink (Control control)
    {
      if (control is HyperLink)
      {
        HyperLink hyperLink = (HyperLink)control;
        if (hyperLink.NavigateUrl == "#" && hyperLink.Attributes["onclick"].Contains("alert"))
          return true;
        if (hyperLink.NavigateUrl.Contains("alert"))
          return true;
      }

      return false;
    }

    public bool IsEnabled (Control control)
    {
      if (control is WebControl && ((WebControl)control).Enabled)
        return true;

      if (control is HtmlControl && string.IsNullOrEmpty(((HtmlControl)control).Attributes["disabled"]))
        return true;

      return false;
    }

    private void OnClick (object sender, EventArgs e)
    {
      Control control = (Control)sender;
      if (Click != null)
        Click(this, new IDEventArgs(control.ID));
    }

    private Control CreateInputControlWithSubmitBehavior (string prefix)
    {
      Button button = new Button();
      button.ID = CreateID(prefix, "InputSubmit");
      button.Text = "Submit";
      button.UseSubmitBehavior = true;
      button.Click += OnClick;

      return button;
    }

    private Control CreateInputControlWithButtonBehavior (string prefix)
    {
      Button button = new Button();
      button.ID = CreateID(prefix, "InputButton");
      button.Text = "Button";
      button.UseSubmitBehavior = false;
      button.Click += OnClick;

      return button;
    }

    private Control CreateButtonControlWithSubmitBehavior (string prefix)
    {
      WebButton button = new WebButton();
      button.ID = CreateID(prefix, "ButtonSubmit");
      button.Text = WebString.CreateFromText("Submit");
      button.UseSubmitBehavior = true;
      button.Click += OnClick;
      return button;
    }

    private Control CreateButtonControlWithButtonBehavior (string prefix)
    {
      WebButton button = new WebButton();
      button.ID = CreateID(prefix, "ButtonButton");
      button.Text = WebString.CreateFromText("Button");
      button.UseSubmitBehavior = false;
      button.Click += OnClick;

      return button;
    }

    private Control CreateAnchorWithTextAndJavascriptInOnClick (string prefix)
    {
      HyperLink hyperLink = CreateAnchorWithPostBackJavascriptInOnClick(prefix, "AnchorWithTextAndJavascriptInOnClick");
      hyperLink.Text = "OnClick";

      return hyperLink;
    }

    private Control CreateAnchorWithTextAndJavascriptInHref (string prefix)
    {
      LinkButton linkButton = CreateAnchorWithPostBackJavascriptInHref(prefix, "AnchorWithTextAndJavascriptInHref");
      linkButton.Text = "Href";

      return linkButton;
    }

    private Control CreateAnchorWithImageAndJavascriptInOnClick (string prefix)
    {
      HyperLink hyperLink = CreateAnchorWithPostBackJavascriptInOnClick(prefix, "AnchorWithImageAndJavascriptInOnClick");
      hyperLink.Controls.Add(CreateImage(hyperLink.ID, "OnClick"));

      return hyperLink;
    }

    private Control CreateAnchorWithImageAndJavascriptInHref (string prefix)
    {
      LinkButton linkButton = CreateAnchorWithPostBackJavascriptInHref(prefix, "AnchorWithImageAndJavascriptInHref");
      linkButton.Controls.Add(CreateImage(linkButton.ID, "Href"));

      return linkButton;
    }

    private Control CreateAnchorWithSpanAndJavascriptInOnClick (string prefix)
    {
      HyperLink hyperLink = CreateAnchorWithPostBackJavascriptInOnClick(prefix, "AnchorWithSpanAndJavascriptInOnClick");
      hyperLink.Controls.Add(CreateHtmlGenericControl(hyperLink.ID, "OnClick", "span"));

      return hyperLink;
    }

    private Control CreateAnchorWithSpanAndJavascriptInHref (string prefix)
    {
      LinkButton linkButton = CreateAnchorWithPostBackJavascriptInHref(prefix, "AnchorWithSpanAndJavascriptInHref");
      linkButton.Controls.Add(CreateHtmlGenericControl(linkButton.ID, "Href", "span"));

      return linkButton;
    }

    private Control CreateAnchorWithLabelAndJavascriptInOnClick (string prefix)
    {
      HyperLink hyperLink = CreateAnchorWithPostBackJavascriptInOnClick(prefix, "AnchorWithLabelAndJavascriptInOnClick");
      hyperLink.Controls.Add(CreateHtmlGenericControl(hyperLink.ID, "OnClick", "label"));

      return hyperLink;
    }

    private Control CreateAnchorWithLabelAndJavascriptInHref (string prefix)
    {
      LinkButton linkButton = CreateAnchorWithPostBackJavascriptInHref(prefix, "AnchorWithLabelAndJavascriptInHref");
      HtmlGenericControl control = CreateHtmlGenericControl(linkButton.ID, "Href", "label");
      control.Attributes["title"] = "Anchor with Label does not work in IE";
      linkButton.Controls.Add(control);
      linkButton.Enabled = _page.Request.Browser.Browser != "IE";

      return linkButton;
    }

    private Control CreateAnchorWithBoldAndJavascriptInOnClick (string prefix)
    {
      HyperLink hyperLink = CreateAnchorWithPostBackJavascriptInOnClick(prefix, "AnchorWithBoldAndJavascriptInOnClick");
      hyperLink.Controls.Add(CreateHtmlGenericControl(hyperLink.ID, "OnClick", "b"));

      return hyperLink;
    }

    private Control CreateAnchorWithBoldAndJavascriptInHref (string prefix)
    {
      LinkButton linkButton = CreateAnchorWithPostBackJavascriptInHref(prefix, "AnchorWithBoldAndJavascriptInHref");
      linkButton.Controls.Add(CreateHtmlGenericControl(linkButton.ID, "Href", "b"));

      return linkButton;
    }

    private Control CreateAnchorWithNonPostBackJavascriptInOnClick (string prefix)
    {
      HyperLink hyperLink = new HyperLink();
      hyperLink.ID = CreateID(prefix, "AnchorWithNonPostBackJavascriptInOnClick");
      hyperLink.Text = "OnClick";
      hyperLink.NavigateUrl = "invalid";
      hyperLink.Attributes["onclick"] = "window.alert ('javascript in onclick handler'); return false;";

      return hyperLink;
    }

    private Control CreateAnchorWithNonPostBackJavascriptInHref (string prefix)
    {
      HyperLink hyperLink = new HyperLink();
      hyperLink.ID = CreateID(prefix, "AnchorWithNonPostBackJavascriptInHref");
      hyperLink.Text = "Href";
      hyperLink.NavigateUrl = "javascript:window.alert ('javascript in onclick handler')";

      return hyperLink;
    }

    private Image CreateImage (string prefix, string text)
    {
      Image image = new Image();
      image.ID = CreateID(prefix, "Inner");
      image.AlternateText = text;
      image.ImageUrl = _resourceUrlFactory.CreateResourceUrl(typeof(TestControlGenerator), ResourceType.Image, "Image.gif").GetUrl();
      image.Style.Add(HtmlTextWriterStyle.BorderStyle, "none");

      return image;
    }

    private HtmlGenericControl CreateHtmlGenericControl (string prefix, string text, string tag)
    {
      HtmlGenericControl span = new HtmlGenericControl(tag);
      span.ID = CreateID(prefix, "Inner");
      span.InnerText = text;

      return span;
    }

    private HyperLink CreateAnchorWithPostBackJavascriptInOnClick (string prefix, string id)
    {
      HyperLink hyperLink = new HyperLink();
      hyperLink.ID = CreateID(prefix, id);
      hyperLink.NavigateUrl = SafeServiceLocator.Current.GetInstance<IFallbackNavigationUrlProvider>().GetURL();
      hyperLink.Attributes["onclick"] = string.Empty;
      hyperLink.PreRender += delegate
      {
        hyperLink.Attributes["onclick"] = _page.ClientScript.GetPostBackEventReference(_postBackEventHandler, hyperLink.ID) + ";return false;";
      };

      return hyperLink;
    }

    private LinkButton CreateAnchorWithPostBackJavascriptInHref (string prefix, string id)
    {
      LinkButton linkButton = new LinkButton();
      linkButton.ID = CreateID(prefix, id);
      linkButton.Click += OnClick;

      return linkButton;
    }

    private string CreateID (string prefix, string id)
    {
      return (string.IsNullOrEmpty(prefix) ? string.Empty : prefix + "_") + id;
    }
  }
}
