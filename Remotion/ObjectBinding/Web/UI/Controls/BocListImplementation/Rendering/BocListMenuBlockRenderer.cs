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
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the menu block of a <see cref="BocList"/>.
  /// </summary>
  /// <remarks>This class should not be instantiated directly. It is meant to be used by a <see cref="BocListRenderer"/>.</remarks>
  [ImplementationFor (typeof (IBocListMenuBlockRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocListMenuBlockRenderer : IBocListMenuBlockRenderer
  {
    private const string c_whiteSpace = "&nbsp;";
    protected const string c_defaultMenuBlockItemOffset = "5pt";
    protected const int c_designModeAvailableViewsListWidthInPoints = 40;

    private readonly BocListCssClassDefinition _cssClasses;

    /// <summary>
    /// Contructs a renderer bound to a <see cref="BocList"/> to render and an <see cref="HtmlTextWriter"/> to render to.
    /// </summary>
    /// <remarks>
    /// This class should not be instantiated directly by clients. Instead, a <see cref="BocListRenderer"/> should use a
    /// factory to obtain an instance of this class.
    /// </remarks>
    public BocListMenuBlockRenderer (BocListCssClassDefinition cssClasses)
    {
      ArgumentUtility.CheckNotNull ("cssClasses", cssClasses);

      _cssClasses = cssClasses;
    }

    public BocListCssClassDefinition CssClasses
    {
      get { return _cssClasses; }
    }

    /// <summary> Renders the menu block of the control. </summary>
    /// <remarks> Contains the drop down list for selcting a column configuration and the options menu.  </remarks> 
    public void Render (BocListRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      string menuBlockItemOffset = c_defaultMenuBlockItemOffset;
      if (!renderingContext.Control.MenuBlockItemOffset.IsEmpty)
        menuBlockItemOffset = renderingContext.Control.MenuBlockItemOffset.ToString();

      RenderAvailableViewsList (renderingContext, menuBlockItemOffset);

      RenderOptionsMenu (renderingContext, menuBlockItemOffset);

      RenderListMenu (renderingContext, menuBlockItemOffset);
    }

    private void RenderListMenu (BocListRenderingContext renderingContext, string menuBlockItemOffset)
    {
      if (!renderingContext.Control.HasListMenu)
        return;

      Assertion.IsTrue (
          renderingContext.Control.ListMenu.Visible,
          "BocList '{0}': The ListMenu must remain visible if BocList.HasListMenu is evaluates 'true'.",
          renderingContext.Control.ID);

      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      renderingContext.Writer.AddStyleAttribute ("margin-bottom", menuBlockItemOffset);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);
      renderingContext.Control.ListMenu.RenderControl (renderingContext.Writer);
      renderingContext.Writer.RenderEndTag();
    }

    private void RenderOptionsMenu (BocListRenderingContext renderingContext, string menuBlockItemOffset)
    {
      if (!renderingContext.Control.HasOptionsMenu)
        return;

      Assertion.IsTrue (
          renderingContext.Control.OptionsMenu.Visible,
          "BocList '{0}': The OptionsMenu must remain visible if BocList.HasOptionsMenu is evaluates 'true'.",
          renderingContext.Control.ID);

      renderingContext.Control.OptionsMenu.Style.Add ("margin-bottom", menuBlockItemOffset);
      renderingContext.Control.OptionsMenu.RenderControl (renderingContext.Writer);
    }

    private void RenderAvailableViewsList (BocListRenderingContext renderingContext, string menuBlockItemOffset)
    {
      if (!renderingContext.Control.HasAvailableViewsList)
        return;

      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      renderingContext.Writer.AddStyleAttribute ("margin-bottom", menuBlockItemOffset);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClasses.AvailableViewsListLabel);

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      string availableViewsListTitle;
      if (string.IsNullOrEmpty (renderingContext.Control.AvailableViewsListTitle))
        availableViewsListTitle = renderingContext.Control.GetResourceManager().GetString (Controls.BocList.ResourceIdentifier.AvailableViewsListTitle);
      else
        availableViewsListTitle = renderingContext.Control.AvailableViewsListTitle;
      // Do not HTML encode.
      renderingContext.Writer.Write (availableViewsListTitle);
      renderingContext.Writer.RenderEndTag();

      renderingContext.Writer.Write (c_whiteSpace);
      
      var availableViewsList = renderingContext.Control.GetAvailableViewsList();
      if (renderingContext.Control.IsDesignMode)
        availableViewsList.Width = Unit.Point (c_designModeAvailableViewsListWidthInPoints);
      availableViewsList.Enabled = !renderingContext.Control.EditModeController.IsRowEditModeActive && 
        !renderingContext.Control.EditModeController.IsListEditModeActive;
      availableViewsList.CssClass = CssClasses.AvailableViewsListDropDownList;
      availableViewsList.RenderControl (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag();
    }
  }
}