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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.WebTabStripImplementation;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.TabbedMultiViewImplementation.Rendering
{
  /// <summary>
  /// Implements <see cref="ITabbedMultiViewRenderer"/> for standard mode rendering of <see cref="TabbedMultiView"/> controls.
  /// <seealso cref="ITabbedMultiView"/>
  /// </summary>
  [ImplementationFor(typeof(ITabbedMultiViewRenderer), Lifetime = LifetimeKind.Singleton)]
  public class TabbedMultiViewRenderer : RendererBase<ITabbedMultiView>, ITabbedMultiViewRenderer
  {
    private readonly ILabelReferenceRenderer _labelReferenceRenderer;

    public TabbedMultiViewRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures,
        ILabelReferenceRenderer labelReferenceRenderer)
        : base(resourceUrlFactory, globalizationService, renderingFeatures)
    {
      ArgumentUtility.CheckNotNull("labelReferenceRenderer", labelReferenceRenderer);

      _labelReferenceRenderer = labelReferenceRenderer;
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, IControl control)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);
      ArgumentUtility.CheckNotNull("control", control);

      htmlHeadAppender.RegisterWebClientScriptInclude();
      htmlHeadAppender.RegisterCommonStyleSheet();

      string keyStyle = typeof(TabbedMultiViewRenderer).GetFullNameChecked() + "_Style";
      var styleSheetUrl = ResourceUrlFactory.CreateThemedResourceUrl(typeof(TabbedMultiViewRenderer), ResourceType.Html, "TabbedMultiView.css");
      htmlHeadAppender.RegisterStylesheetLink(keyStyle, styleSheetUrl, HtmlHeadAppender.Priority.Library);

      ScriptUtility.Instance.RegisterJavaScriptInclude(control, htmlHeadAppender);
    }

    public void Render (TabbedMultiViewRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      AddAttributesToRender(renderingContext);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, renderingContext.Control.WrapperClientID);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassWrapper);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      RenderTopControls(renderingContext);
      RenderTabStrip(renderingContext);
      RenderActiveView(renderingContext);
      RenderBottomControls(renderingContext);

      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderEndTag();
    }

    protected void AddAttributesToRender (TabbedMultiViewRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      AddStandardAttributesToRender(renderingContext);
      if (string.IsNullOrEmpty(renderingContext.Control.CssClass) && string.IsNullOrEmpty(renderingContext.Control.Attributes["class"]))
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassBase);
    }

    protected virtual void RenderTabStrip (TabbedMultiViewRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      renderingContext.Control.TabStrip.CssClass = CssClassTabStrip;
      renderingContext.Control.TabStrip.RenderControl(renderingContext.Writer);
    }

    protected virtual void RenderActiveView (TabbedMultiViewRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, renderingContext.Control.ActiveViewClientID);
      renderingContext.Control.ActiveViewStyle.AddAttributesToRender(renderingContext.Writer);
      if (string.IsNullOrEmpty(renderingContext.Control.ActiveViewStyle.CssClass))
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassActiveView);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, renderingContext.Control.ActiveViewContentClientID);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassContentBorder);
      var activeTab = renderingContext.Control.TabStrip.Tabs.Cast<IWebTab>().FirstOrDefault(t => t.IsSelected);
      if (activeTab != null)
      {
        // Must point to an element not annotated with role=none to work consistently.
        var labelID = renderingContext.Control.TabStrip.ClientID + "_" + activeTab.ItemID + "_Command";
        _labelReferenceRenderer.AddLabelsReference(renderingContext.Writer, new[] { labelID });
      }
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, "0");
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.TabPanel);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      var view = renderingContext.Control.GetActiveView();
      if (view != null)
      {
        for (int i = 0; i < view.Controls.Count; i++)
        {
          Control control = view.Controls[i];
          control.RenderControl(renderingContext.Writer);
        }
      }

      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderEndTag();
    }

    protected virtual void RenderTopControls (TabbedMultiViewRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      Style style = renderingContext.Control.TopControlsStyle;
      PlaceHolder placeHolder = renderingContext.Control.TopControl;
      string cssClass = CssClassTopControls;
      RenderPlaceHolder(renderingContext, style, placeHolder, cssClass);
    }

    protected virtual void RenderBottomControls (TabbedMultiViewRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      Style style = renderingContext.Control.BottomControlsStyle;
      PlaceHolder placeHolder = renderingContext.Control.BottomControl;
      string cssClass = CssClassBottomControls;
      RenderPlaceHolder(renderingContext, style, placeHolder, cssClass);
    }

    private void RenderPlaceHolder (TabbedMultiViewRenderingContext renderingContext, Style style, PlaceHolder placeHolder, string defaultCssClass)
    {
      string cssClass = defaultCssClass;
      if (!string.IsNullOrEmpty(style.CssClass))
        cssClass = style.CssClass;

      if (placeHolder.Controls.Count == 0)
        cssClass += " " + CssClassEmpty;

      string backupCssClass = style.CssClass;
      style.CssClass = cssClass;
      style.AddAttributesToRender(renderingContext.Writer);
      style.CssClass = backupCssClass;

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, placeHolder.ClientID);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      placeHolder.RenderControl(renderingContext.Writer);

      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderEndTag();
    }

    #region protected virtual string CssClass...

    /// <summary> Gets the CSS-Class applied to the <see cref="TabbedMultiView"/>. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMultiView</c>. </para>
    /// </remarks>
    public virtual string CssClassBase
    {
      get { return "tabbedMultiView"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="TabbedMultiView"/>'s tab strip. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMultiViewTabStrip</c>. </para>
    /// </remarks>
    public virtual string CssClassTabStrip
    {
      get { return "tabbedMultiViewTabStrip"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="TabbedMultiView"/>'s active view. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMultiViewActiveView</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="ITabbedMultiView.ActiveViewStyle"/> is not set. </para>
    /// </remarks>
    public virtual string CssClassActiveView
    {
      get { return "tabbedMultiViewActiveView"; }
    }

    /// <summary> Gets the CSS-Class applied to the top section. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMultiViewTopControls</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="ITabbedMultiView.TopControlsStyle"/> is not set. </para>
    /// </remarks>
    public virtual string CssClassTopControls
    {
      get { return "tabbedMultiViewTopControls"; }
    }

    /// <summary> Gets the CSS-Class applied to the bottom section. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMultiViewBottomControls</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="ITabbedMultiView.BottomControlsStyle"/> is not set. </para>
    /// </remarks>
    public virtual string CssClassBottomControls
    {
      get { return "tabbedMultiViewBottomControls"; }
    }

    /// <summary> Gets the CSS-Class applied to a <c>div</c> wrapping the content and the border elements. </summary>
    /// <remarks> 
    ///   <para> Class: <c>body</c>. </para>
    /// </remarks>
    public virtual string CssClassViewBody
    {
      get { return "body"; }
    }

    /// <summary> Gets the CSS-Class applied to a <c>div</c> intended for formatting the content. </summary>
    /// <remarks> 
    ///   <para> Class: <c>content</c>. </para>
    /// </remarks>
    public virtual string CssClassContent
    {
      get { return "content"; }
    }

    /// <summary> Gets the CSS-Class applied when the section is empty. </summary>
    /// <remarks> 
    ///   <para> Class: <c>empty</c>. </para>
    ///   <para> 
    ///     Applied in addition to the regular CSS-Class. Use <c>td.tabbedMultiViewTopControls.emtpy</c> or 
    ///     <c>td.tabbedMultiViewBottomControls.emtpy</c>as a selector.
    ///   </para>
    /// </remarks>
    public virtual string CssClassEmpty
    {
      get { return "empty"; }
    }

    public string CssClassWrapper
    {
      get { return "wrapper"; }
    }

    public string CssClassContentBorder
    {
      get { return "contentBorder"; }
    }

    public string CssClassScreenReaderText
    {
      get { return CssClassDefinition.ScreenReaderText; }
    }

    #endregion
  }
}
