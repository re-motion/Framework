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
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.TabbedMultiViewImplementation;
using Remotion.Web.UI.Controls.TabbedMultiViewImplementation.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.Web.Legacy.UI.Controls.Rendering
{
  /// <summary>
  /// Implements <see cref="ITabbedMultiViewRenderer"/> for quirks mode rendering of <see cref="TabbedMultiView"/> controls.
  /// <seealso cref="ITabbedMultiView"/>
  /// </summary>
  public class TabbedMultiViewQuirksModeRenderer : QuirksModeRendererBase<ITabbedMultiView>, ITabbedMultiViewRenderer
  {
    public TabbedMultiViewQuirksModeRenderer (IResourceUrlFactory resourceUrlFactory) 
      : base(resourceUrlFactory)
    { 
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, IControl control)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);
      ArgumentUtility.CheckNotNull ("control", control);

      string key = typeof (TabbedMultiViewQuirksModeRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (key))
      {
        var styleUrl = ResourceUrlFactory.CreateResourceUrl (typeof (TabbedMultiViewQuirksModeRenderer), ResourceType.Html, "TabbedMultiView.css");
        htmlHeadAppender.RegisterStylesheetLink (key, styleUrl, HtmlHeadAppender.Priority.Library);
      }

      ScriptUtility.Instance.RegisterJavaScriptInclude (control, htmlHeadAppender);
    }

    public void Render (TabbedMultiViewRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      AddAttributesToRender (renderingContext);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      if (!string.IsNullOrEmpty (renderingContext.Control.CssClass))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, renderingContext.Control.CssClass);
      else if (!string.IsNullOrEmpty (renderingContext.Control.Attributes["class"]))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, renderingContext.Control.Attributes["class"]);
      else
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Table);

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      RenderTopControls (renderingContext);
      renderingContext.Writer.RenderEndTag ();

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      RenderTabStrip (renderingContext);
      renderingContext.Writer.RenderEndTag ();

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      RenderActiveView (renderingContext);
      renderingContext.Writer.RenderEndTag ();

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      RenderBottomControls (renderingContext);
      renderingContext.Writer.RenderEndTag ();

      renderingContext.Writer.RenderEndTag ();
      renderingContext.Writer.RenderEndTag ();
    }

    protected void AddAttributesToRender (TabbedMultiViewRenderingContext renderingContext)
    {
      AddStandardAttributesToRender (renderingContext);
      if (renderingContext.Control.IsDesignMode)
      {
        renderingContext.Writer.AddStyleAttribute ("width", "100%");
        renderingContext.Writer.AddStyleAttribute ("height", "75%");
      }
      if (string.IsNullOrEmpty (renderingContext.Control.CssClass) && string.IsNullOrEmpty (renderingContext.Control.Attributes["class"]))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);
    }

    protected virtual void RenderTabStrip (TabbedMultiViewRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassTabStrip);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      renderingContext.Control.TabStrip.CssClass = CssClassTabStrip;
      renderingContext.Control.TabStrip.RenderControl (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag (); // end td
    }

    protected virtual void RenderActiveView (TabbedMultiViewRenderingContext renderingContext)
    {
      ScriptUtility.Instance.RegisterElementForBorderSpans (renderingContext.Control, "#" + renderingContext.Control.ActiveViewClientID + " > *:first");

      if (renderingContext.Control.IsDesignMode)
        renderingContext.Writer.AddStyleAttribute ("border", "solid 1px black");
      renderingContext.Control.ActiveViewStyle.AddAttributesToRender (renderingContext.Writer);
      if (string.IsNullOrEmpty (renderingContext.Control.ActiveViewStyle.CssClass))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassActiveView);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.ActiveViewClientID);
      renderingContext.Control.ActiveViewStyle.AddAttributesToRender (renderingContext.Writer);
      if (string.IsNullOrEmpty (renderingContext.Control.ActiveViewStyle.CssClass))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassActiveView);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin outer div

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassViewBody);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin body div

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.ActiveViewClientID + "_Content");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin content div

      var view = renderingContext.Control.GetActiveView ();
      if (view != null)
      {
        for (int i = 0; i < view.Controls.Count; i++)
        {
          Control control = view.Controls[i];
          control.RenderControl (renderingContext.Writer);
        }
      }

      renderingContext.Writer.RenderEndTag (); // end content div
      renderingContext.Writer.RenderEndTag (); // end body div
      renderingContext.Writer.RenderEndTag (); // end outer div

      renderingContext.Writer.RenderEndTag (); // end td
    }

    protected virtual void RenderTopControls (TabbedMultiViewRenderingContext renderingContext)
    {
      Style style = renderingContext.Control.TopControlsStyle;
      PlaceHolder placeHolder = renderingContext.Control.TopControl;
      string cssClass = CssClassTopControls;
      RenderPlaceHolder (renderingContext, style, placeHolder, cssClass);
    }

    protected virtual void RenderBottomControls (TabbedMultiViewRenderingContext renderingContext)
    {
      Style style = renderingContext.Control.BottomControlsStyle;
      PlaceHolder placeHolder = renderingContext.Control.BottomControl;
      string cssClass = CssClassBottomControls;
      RenderPlaceHolder (renderingContext, style, placeHolder, cssClass);
    }

    private void RenderPlaceHolder (TabbedMultiViewRenderingContext renderingContext, Style style, PlaceHolder placeHolder, string cssClass)
    {
      ScriptUtility.Instance.RegisterElementForBorderSpans (renderingContext.Control, "#" + placeHolder.ClientID + " > *:first");

      if (string.IsNullOrEmpty (style.CssClass))
      {
        if (placeHolder.Controls.Count > 0)
          renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
        else
          renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass + " " + CssClassEmpty);
      }
      else
      {
        if (placeHolder.Controls.Count > 0)
          renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, style.CssClass);
        else
          renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, style.CssClass + " " + CssClassEmpty);
      }
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, placeHolder.ClientID);
      style.AddAttributesToRender (renderingContext.Writer);
      if (string.IsNullOrEmpty (style.CssClass))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin outer div

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin content div

      placeHolder.RenderControl (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag (); // end content div
      renderingContext.Writer.RenderEndTag (); // end outer div

      renderingContext.Writer.RenderEndTag (); // end td
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

    #endregion
  }
}