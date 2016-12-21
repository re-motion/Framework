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
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.Web.UI.Controls.TabbedMenuImplementation.Rendering
{
  /// <summary>
  /// Implements <see cref="ITabbedMenuRenderer"/> for standard mode rendering of <see cref="TabbedMenu"/> controls.
  /// <seealso cref="ITabbedMenu"/>
  /// </summary>
  [ImplementationFor (typeof (ITabbedMenuRenderer), Lifetime = LifetimeKind.Singleton)]
  public class TabbedMenuRenderer : RendererBase<ITabbedMenu>, ITabbedMenuRenderer
  {
    public TabbedMenuRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures)
        : base (resourceUrlFactory, globalizationService, renderingFeatures)
    {
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);
      string key = typeof (TabbedMenuRenderer).FullName + "_Style";
      var url = ResourceUrlFactory.CreateThemedResourceUrl (typeof (TabbedMenuRenderer), ResourceType.Html, "TabbedMenu.css");
      htmlHeadAppender.RegisterStylesheetLink (key, url, HtmlHeadAppender.Priority.Library);
    }

    public void Render (TabbedMenuRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      AddAttributesToRender (renderingContext);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Table);

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr); // Begin main menu row

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Colspan, "2");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassMainMenuCell);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin main menu cell
      renderingContext.Control.MainMenuTabStrip.CssClass = CssClassMainMenu;
      renderingContext.Control.MainMenuTabStrip.Width = Unit.Percentage (100);
      renderingContext.Control.MainMenuTabStrip.RenderControl (renderingContext.Writer);
      renderingContext.Writer.RenderEndTag (); // End main menu cell

      renderingContext.Writer.RenderEndTag (); // End main menu row

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr); // Begin sub menu row

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassSubMenuCell);
      if (!renderingContext.Control.SubMenuBackgroundColor.IsEmpty)
      {
        string backGroundColor = ColorTranslator.ToHtml (renderingContext.Control.SubMenuBackgroundColor);
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.BackgroundColor, backGroundColor);
      }
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin sub menu cell
      renderingContext.Control.SubMenuTabStrip.Style["width"] = "auto";
      renderingContext.Control.SubMenuTabStrip.CssClass = CssClassSubMenu;
      renderingContext.Control.SubMenuTabStrip.RenderControl (renderingContext.Writer);
      renderingContext.Writer.RenderEndTag (); // End sub menu cell

      renderingContext.Control.StatusStyle.AddAttributesToRender (renderingContext.Writer);
      if (string.IsNullOrEmpty (renderingContext.Control.StatusStyle.CssClass))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassStatusCell);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin status cell

      if (string.IsNullOrEmpty (renderingContext.Control.StatusText))
        renderingContext.Writer.Write ("&nbsp;");
      else
        renderingContext.Writer.Write (renderingContext.Control.StatusText); // Do not HTML encode

      renderingContext.Writer.RenderEndTag (); // End status cell
      renderingContext.Writer.RenderEndTag (); // End sub menu row
      renderingContext.Writer.RenderEndTag (); // End table
    }

    protected void AddAttributesToRender (TabbedMenuRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      AddStandardAttributesToRender (renderingContext);

      if (renderingContext.Control.IsDesignMode)
        renderingContext.Writer.AddStyleAttribute ("width", "100%");
      if (string.IsNullOrEmpty (renderingContext.Control.CssClass) && string.IsNullOrEmpty (renderingContext.Control.Attributes["class"]))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);
    }

    #region protected virtual string CssClass...

    /// <summary> Gets the CSS-Class applied to the <see cref="WebTabStrip"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStrip</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    protected virtual string CssClassBase
    {
      get { return "tabbedMenu"; }
    }

    /// <summary> Gets the CSS-Class applied to the main menu's tab strip. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMainMenu</c>. </para>
    /// </remarks>
    protected virtual string CssClassMainMenu
    {
      get { return "tabbedMainMenu"; }
    }

    /// <summary> Gets the CSS-Class applied to the sub menu's tab strip. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedSubMenu</c>. </para>
    /// </remarks>
    protected virtual string CssClassSubMenu
    {
      get { return "tabbedSubMenu"; }
    }

    /// <summary> Gets the CSS-Class applied to the main menu cell. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMainMenuCell</c>. </para>
    /// </remarks>
    protected virtual string CssClassMainMenuCell
    {
      get { return "tabbedMainMenuCell"; }
    }

    /// <summary> Gets the CSS-Class applied to the sub menu cell. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedSubMenuCell</c>. </para>
    /// </remarks>
    protected virtual string CssClassSubMenuCell
    {
      get { return "tabbedSubMenuCell"; }
    }

    /// <summary> Gets the CSS-Class applied to the status cell. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMenuStatusCell</c>. </para>
    /// </remarks>
    protected virtual string CssClassStatusCell
    {
      get { return "tabbedMenuStatusCell"; }
    }

    #endregion
  }
}