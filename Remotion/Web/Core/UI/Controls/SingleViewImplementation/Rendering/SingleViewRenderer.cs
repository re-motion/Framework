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

namespace Remotion.Web.UI.Controls.SingleViewImplementation.Rendering
{
  /// <summary>
  /// Implements <see cref="ISingleViewRenderer"/> for standard mode rendering of <see cref="SingleView"/> controls.
  /// <seealso cref="ISingleView"/>
  /// </summary>
  [ImplementationFor(typeof(ISingleViewRenderer), Lifetime = LifetimeKind.Singleton)]
  public class SingleViewRenderer : RendererBase<ISingleView>, ISingleViewRenderer
  {
    public SingleViewRenderer (
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

      string keyStyle = typeof(SingleViewRenderer).GetFullNameChecked() + "_Style";

      var styleSheetUrl = ResourceUrlFactory.CreateThemedResourceUrl(typeof(SingleViewRenderer), ResourceType.Html, "SingleView.css");
      htmlHeadAppender.RegisterStylesheetLink(keyStyle, styleSheetUrl, HtmlHeadAppender.Priority.Library);

      ScriptUtility.Instance.RegisterJavaScriptInclude(control, htmlHeadAppender);
    }

    public void Render (SingleViewRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      AddStandardAttributesToRender(renderingContext);
      if (string.IsNullOrEmpty(renderingContext.Control.CssClass) && string.IsNullOrEmpty(renderingContext.Control.Attributes["class"]))
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassBase);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, renderingContext.Control.WrapperClientID);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassWrapper);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      RenderTopControls(renderingContext);
      RenderView(renderingContext);
      RenderBottomControls(renderingContext);

      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderEndTag();
    }

    public string CssClassWrapper
    {
      get { return "wrapper"; }
    }

    protected virtual void RenderTopControls (SingleViewRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      Style style = renderingContext.Control.TopControlsStyle;
      PlaceHolder placeHolder = renderingContext.Control.TopControl;
      string cssClass = CssClassTopControls;
      RenderPlaceHolder(renderingContext, style, placeHolder, cssClass);
    }

    protected virtual void RenderBottomControls (SingleViewRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      Style style = renderingContext.Control.BottomControlsStyle;
      PlaceHolder placeHolder = renderingContext.Control.BottomControl;
      string cssClass = CssClassBottomControls;
      RenderPlaceHolder(renderingContext, style, placeHolder, cssClass);
    }

    private void RenderPlaceHolder (SingleViewRenderingContext renderingContext, Style style, PlaceHolder placeHolder, string defaultCssClass)
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

    private void RenderView (SingleViewRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, renderingContext.Control.ViewClientID);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassView);
      renderingContext.Control.ViewStyle.AddAttributesToRender(renderingContext.Writer);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, renderingContext.Control.ViewContentClientID);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassContentBorder);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      renderingContext.Control.View.RenderControl(renderingContext.Writer);

      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderEndTag();
      renderingContext.Writer.RenderEndTag();
    }

    #region protected virtual string CssClass...

    /// <summary> Gets the CSS-Class applied to the <see cref="SingleView"/>. </summary>
    /// <remarks> 
    ///   <para> Class: <c>singleView</c>. </para>
    /// </remarks>
    public virtual string CssClassBase
    {
      get { return "singleView"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="SingleView"/>'s active view. </summary>
    /// <remarks> 
    ///   <para> Class: <c>singleViewActiveView</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="P:Control.ViewStyle"/> is not set. </para>
    /// </remarks>
    public virtual string CssClassView
    {
      get { return "singleViewView"; }
    }

    /// <summary> Gets the CSS-Class applied to the top section. </summary>
    /// <remarks> 
    ///   <para> Class: <c>singleViewTopControls</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="P:Control.TopControlsStyle"/> is not set. </para>
    /// </remarks>
    public virtual string CssClassTopControls
    {
      get { return "singleViewTopControls"; }
    }

    /// <summary> Gets the CSS-Class applied to the bottom section. </summary>
    /// <remarks> 
    ///   <para> Class: <c>singleViewBottomControls</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> of the <see cref="P:Control.BottomControlsStyle"/> is not set. </para>
    /// </remarks>
    public virtual string CssClassBottomControls
    {
      get { return "singleViewBottomControls"; }
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

    public virtual string CssClassContentBorder
    {
      get { return "contentBorder"; }
    }

    /// <summary> Gets the CSS-Class applied when the section is empty. </summary>
    /// <remarks> 
    ///   <para> Class: <c>empty</c>. </para>
    ///   <para> 
    ///     Applied in addition to the regular CSS-Class. Use <c>td.singleViewTopControls.emtpy</c> or 
    ///     <c>td.singleViewBottomControls.emtpy</c>as a selector.
    ///   </para>
    /// </remarks>
    public virtual string CssClassEmpty
    {
      get { return "empty"; }
    }

    #endregion
  }
}
