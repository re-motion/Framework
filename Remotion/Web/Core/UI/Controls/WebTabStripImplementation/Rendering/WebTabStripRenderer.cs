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
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering
{
  /// <summary>
  /// Implements <see cref="IWebTabStripRenderer"/> for standard mode rendering of <see cref="WebTabStrip"/> controls.
  /// <seealso cref="IWebTabStrip"/>
  /// </summary>
  [ImplementationFor(typeof(IWebTabStripRenderer), Lifetime = LifetimeKind.Singleton)]
  public class WebTabStripRenderer : RendererBase<IWebTabStrip>, IWebTabStripRenderer
  {
    public WebTabStripRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures)
        : base(resourceUrlFactory, globalizationService, renderingFeatures)
    {
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, IControl control)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);
      ArgumentUtility.CheckNotNull("control", control);

      htmlHeadAppender.RegisterWebClientScriptInclude();
      htmlHeadAppender.RegisterCommonStyleSheet();

      string key = typeof(WebTabStripRenderer).GetFullNameChecked() + "_Style";
      var styleSheetUrl = ResourceUrlFactory.CreateThemedResourceUrl(typeof(WebTabStripRenderer), ResourceType.Html, "TabStrip.css");
      htmlHeadAppender.RegisterStylesheetLink(key, styleSheetUrl, HtmlHeadAppender.Priority.Library);

      ScriptUtility.Instance.RegisterJavaScriptInclude(control, htmlHeadAppender);
    }

    public void Render (WebTabStripRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      renderingContext.Control.Page!.ClientScript.RegisterStartupScriptBlock(
          renderingContext.Control,
          typeof(WebTabStrip),
          Guid.NewGuid().ToString(),
          string.Format("WebTabStrip.Initialize ('#{0}');", renderingContext.Control.ClientID));

      AddAttributesToRender(renderingContext);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);
      RenderBeginTabsPane(renderingContext);

      foreach (var webTabRenderer in renderingContext.WebTabRenderers)
      {
        webTabRenderer.Render(renderingContext);
        renderingContext.Writer.WriteLine();
      }

      RenderEndTabsPane(renderingContext);
      RenderClearingPane(renderingContext);
      renderingContext.Writer.RenderEndTag();
    }

    protected void AddAttributesToRender (WebTabStripRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      AddStandardAttributesToRender(renderingContext);

      if (string.IsNullOrEmpty(renderingContext.Control.CssClass) && string.IsNullOrEmpty(renderingContext.Control.Attributes["class"]))
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassBase);
    }

    private void RenderBeginTabsPane (WebTabStripRenderingContext renderingContext)
    {
      bool isEmpty = renderingContext.Control.Tabs.Count == 0;

      string cssClass = CssClassTabsPane;
      if (isEmpty)
        cssClass += " " + CssClassTabsPaneEmpty;
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.TabList);

      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div); // Begin Div

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.None);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Ul); // Begin List
    }

    private void RenderEndTabsPane (WebTabStripRenderingContext renderingContext)
    {
      renderingContext.Writer.RenderEndTag(); // End List
      renderingContext.Writer.RenderEndTag(); // End Div
    }

    private void RenderClearingPane (WebTabStripRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassClearingPane);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);
      renderingContext.Writer.RenderEndTag();
    }

    #region public virtual string CssClass...

    /// <summary> Gets the CSS-Class applied to the <see cref="WebTabStrip"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStrip</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    public virtual string CssClassBase
    {
      get { return "tabStrip"; }
    }

    /// <summary> Gets the CSS-Class applied to the pane of <see cref="WebTab"/> items. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTabsPane</c>. </para>
    /// </remarks>
    public virtual string CssClassTabsPane
    {
      get { return "tabStripTabsPane"; }
    }

    /// <summary> Gets the CSS-Class applied to the wrapper around each <see cref="WebTab"/> item. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTabWrapper</c>. </para>
    /// </remarks>
    public virtual string CssClassTabWrapper
    {
      get { return "tabStripTabWrapper"; }
    }

    /// <summary> Gets the CSS-Class applied to a pane of <see cref="WebTab"/> items if no items are present. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTabsPane</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>div.tabStripTabsPane.readOnly</c> as a selector. </para>
    /// </remarks>
    public virtual string CssClassTabsPaneEmpty
    {
      get { return "empty"; }
    }

    /// <summary> Gets the CSS-Class applied to a <see cref="WebTab"/>. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTab</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="P:Control.TabStyle"/>. </para>
    /// </remarks>
    public virtual string CssClassTab
    {
      get { return "tabStripTab"; }
    }

    /// <summary> Gets the CSS-Class applied to a <see cref="WebTab"/> if it is selected. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTabSelected</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="P:Control.SelectedTabStyle"/>. </para>
    /// </remarks>
    public virtual string CssClassTabSelected
    {
      get { return "tabStripTabSelected"; }
    }

    /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for formatting the inside of the anchor element. </summary>
    /// <remarks> 
    ///   <para> Class: <c>anchorBody</c>. </para>
    /// </remarks>
    public virtual string CssClassTabAnchorBody
    {
      get { return "anchorBody"; }
    }

    /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for clearing the space after the last tab. </summary>
    /// <remarks> 
    ///   <para> Class: <c>last</c>. </para>
    /// </remarks>
    public virtual string CssClassTabLast
    {
      get { return "last"; }
    }

    /// <summary> Gets the CSS-Class applied to a separator. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTabSeparator</c>. </para>
    /// </remarks>
    public virtual string CssClassSeparator
    {
      get { return "tabStripTabSeparator"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="WebTab"/> when it is displayed disabled. </summary>
    /// <remarks> 
    ///   <para> Class: <c>disabled</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.tabStripTab.disabled</c> as a selector.</para>
    /// </remarks>
    public virtual string CssClassDisabled
    {
      get { return CssClassDefinition.Disabled; }
    }

    /// <summary> Gets the CSS-Class applied to the div used to clear the flow-layout at the end of the tab-strip. </summary>
    /// <remarks> 
    ///   <para> Class: <c>clearingPane</c>. </para>
    /// </remarks>
    public virtual string CssClassClearingPane
    {
      get { return "clearingPane"; }
    }

    #endregion
  }
}
