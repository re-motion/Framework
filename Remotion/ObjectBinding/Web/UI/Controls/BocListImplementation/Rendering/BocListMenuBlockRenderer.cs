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
using Remotion.Mixins;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Globalization;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering the menu block of a <see cref="BocList"/>.
  /// </summary>
  /// <remarks>This class should not be instantiated directly. It is meant to be used by a <see cref="BocListRenderer"/>.</remarks>
  [ImplementationFor(typeof(IBocListMenuBlockRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocListMenuBlockRenderer : IBocListMenuBlockRenderer
  {
    private const string c_whiteSpace = "&nbsp;";
    protected const string c_defaultMenuBlockItemOffset = "5pt";

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
      ArgumentUtility.CheckNotNull("cssClasses", cssClasses);

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
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      RenderAvailableViewsList(renderingContext);

      RenderOptionsMenu(renderingContext);

      RenderListMenu(renderingContext);
    }

    private void RenderListMenu (BocListRenderingContext renderingContext)
    {
      if (!renderingContext.Control.HasListMenu)
        return;

      Assertion.IsTrue(
          renderingContext.Control.ListMenu.Visible,
          "BocList '{0}': The ListMenu must remain visible if BocList.HasListMenu is evaluates 'true'.",
          renderingContext.Control.ID);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClasses.ListMenuContainer);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);
      renderingContext.Control.ListMenu.RenderControl(renderingContext.Writer);
      renderingContext.Writer.RenderEndTag();
    }

    private void RenderOptionsMenu (BocListRenderingContext renderingContext)
    {
      if (!renderingContext.Control.HasOptionsMenu)
        return;

      Assertion.IsTrue(
          renderingContext.Control.OptionsMenu.Visible,
          "BocList '{0}': The OptionsMenu must remain visible if BocList.HasOptionsMenu is evaluates 'true'.",
          renderingContext.Control.ID);

      if (!string.IsNullOrEmpty(renderingContext.Control.ControlServicePath))
      {
        var stringValueParametersDictionary = new Dictionary<string, string?>();
        stringValueParametersDictionary.Add("controlID", renderingContext.Control.ID);
        stringValueParametersDictionary.Add(
            "controlType",
            TypeUtility.GetPartialAssemblyQualifiedName(MixinTypeUtility.GetUnderlyingTargetType(renderingContext.Control.GetType())));
        stringValueParametersDictionary.Add("businessObjectClass", renderingContext.BusinessObjectWebServiceContext.BusinessObjectClass);
        stringValueParametersDictionary.Add("businessObjectProperty", renderingContext.BusinessObjectWebServiceContext.BusinessObjectProperty);
        stringValueParametersDictionary.Add("businessObject", renderingContext.BusinessObjectWebServiceContext.BusinessObjectIdentifier);
        stringValueParametersDictionary.Add("arguments", renderingContext.BusinessObjectWebServiceContext.Arguments);

        renderingContext.Control.OptionsMenu.SetLoadMenuItemStatus(
            renderingContext.Control.ControlServicePath,
            nameof(IBocListWebService.GetMenuItemStatusForOptionsMenu),
            stringValueParametersDictionary);
      }

      renderingContext.Control.OptionsMenu.RenderControl(renderingContext.Writer);
    }

    private void RenderAvailableViewsList (BocListRenderingContext renderingContext)
    {
      if (!renderingContext.Control.HasAvailableViewsList)
        return;

      var availableViewsList = renderingContext.Control.GetAvailableViewsList();
      Assertion.DebugIsNotNull(availableViewsList, "GetAvailableViewsList() must not return null when HasAvailableViewsList is true.");

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, $"{CssClasses.Themed} {CssClasses.AvailableViewsList}");
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClasses.AvailableViewsListLabel);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.For, availableViewsList.ClientID);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Label);

      WebString availableViewsListTitle;
      if (renderingContext.Control.AvailableViewsListTitle.IsEmpty)
      {
        availableViewsListTitle = renderingContext.Control.GetResourceManager().GetText(Controls.BocList.ResourceIdentifier.AvailableViewsListTitle);
      }
      else
      {
        availableViewsListTitle = renderingContext.Control.AvailableViewsListTitle;
      }

      availableViewsListTitle.WriteTo(renderingContext.Writer);
      renderingContext.Writer.RenderEndTag();

      renderingContext.Writer.Write(c_whiteSpace);

      availableViewsList.Enabled = !renderingContext.Control.EditModeController.IsRowEditModeActive &&
                                   !renderingContext.Control.EditModeController.IsListEditModeActive;
      availableViewsList.CssClass = CssClasses.AvailableViewsListDropDownList;
      availableViewsList.RenderControl(renderingContext.Writer);

      renderingContext.Writer.RenderEndTag();
    }
  }
}
