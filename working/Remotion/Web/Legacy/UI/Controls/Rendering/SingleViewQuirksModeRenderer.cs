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
using Remotion.Web.UI.Controls.SingleViewImplementation;
using Remotion.Web.UI.Controls.SingleViewImplementation.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.Web.Legacy.UI.Controls.Rendering
{
  /// <summary>
  /// Implements <see cref="ISingleViewRenderer"/> for quirks mode rendering of <see cref="SingleView"/> controls.
  /// <seealso cref="ISingleView"/>
  /// </summary>
  public class SingleViewQuirksModeRenderer : QuirksModeRendererBase<ISingleView>, ISingleViewRenderer
  {
    public SingleViewQuirksModeRenderer (IResourceUrlFactory resourceUrlFactory)
      : base(resourceUrlFactory)
    { 
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender, IControl control)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);
      ArgumentUtility.CheckNotNull ("control", control);

      string key = typeof (SingleViewQuirksModeRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (key))
      {
        var styleUrl = ResourceUrlFactory.CreateResourceUrl (typeof (SingleViewQuirksModeRenderer), ResourceType.Html, "SingleView.css");
        htmlHeadAppender.RegisterStylesheetLink (key, styleUrl, HtmlHeadAppender.Priority.Library);
      }

      ScriptUtility.Instance.RegisterJavaScriptInclude (control, htmlHeadAppender);
    }

    public void Render (SingleViewRenderingContext renderingContext)
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

      RenderTopControls (renderingContext);
      RenderView (renderingContext);
      RenderBottomControls (renderingContext);

      renderingContext.Writer.RenderEndTag ();

      renderingContext.Writer.RenderEndTag ();
    }

    protected void AddAttributesToRender (SingleViewRenderingContext renderingContext)
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

    protected virtual void RenderView (SingleViewRenderingContext renderingContext)
    {
      ScriptUtility.Instance.RegisterElementForBorderSpans (renderingContext.Control, "#" + renderingContext.Control.ClientID + "_View > *:first");

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr); // begin tr

      if (renderingContext.Control.IsDesignMode)
        renderingContext.Writer.AddStyleAttribute ("border", "solid 1px black");
      renderingContext.Control.ViewStyle.AddAttributesToRender (renderingContext.Writer);
      if (string.IsNullOrEmpty (renderingContext.Control.ViewStyle.CssClass))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassView);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); // begin td

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.ViewClientID);
      renderingContext.Control.ViewStyle.AddAttributesToRender (renderingContext.Writer);
      if (string.IsNullOrEmpty (renderingContext.Control.ViewStyle.CssClass))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassView);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin outer div

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassViewBody);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin body div

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID + "_View_Content");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div); // begin content div

      //_viewTemplateContainer.RenderControl (writer);
      renderingContext.Control.View.RenderControl (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag (); // end content div
      renderingContext.Writer.RenderEndTag (); // end body div
      renderingContext.Writer.RenderEndTag (); // end outer div

      renderingContext.Writer.RenderEndTag (); // end td
      renderingContext.Writer.RenderEndTag (); // end tr
    }

    protected virtual void RenderTopControls (SingleViewRenderingContext renderingContext)
    {
      Style style = renderingContext.Control.TopControlsStyle;
      PlaceHolder placeHolder = renderingContext.Control.TopControl;
      string cssClass = CssClassTopControls;
      RenderPlaceHolder (renderingContext, style, placeHolder, cssClass);
    }

    protected virtual void RenderBottomControls (SingleViewRenderingContext renderingContext)
    {
      Style style = renderingContext.Control.BottomControlsStyle;
      PlaceHolder placeHolder = renderingContext.Control.BottomControl;
      string cssClass = CssClassBottomControls;
      RenderPlaceHolder (renderingContext, style, placeHolder, cssClass);
    }

    private void RenderPlaceHolder (SingleViewRenderingContext renderingContext, Style style, PlaceHolder placeHolder, string cssClass)
    {
      ScriptUtility.Instance.RegisterElementForBorderSpans (renderingContext.Control, "#" + placeHolder.ClientID + " > *:first");

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr); // begin tr
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
      renderingContext.Writer.RenderEndTag (); // end tr
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