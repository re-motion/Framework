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
using System.Text;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.Web.Legacy.UI.Controls.Rendering
{
  /// <summary>
  /// Base class for all renderers. Contains the essential properties used in rendering.
  /// </summary>
  /// <typeparam name="TControl">The type of control that can be rendered.</typeparam>
  public abstract class QuirksModeRendererBase<TControl>
      where TControl : IStyledControl
  {
    private readonly IResourceUrlFactory _resourceUrlFactory;

    /// <summary>
    /// Initializes the <see cref="Context"/> and the <see cref="Control"/> properties from the arguments.
    /// </summary>
    protected QuirksModeRendererBase (IResourceUrlFactory resourceUrlFactory) 
    {
      ArgumentUtility.CheckNotNull ("resourceUrlFactory", resourceUrlFactory);

      _resourceUrlFactory = resourceUrlFactory;
    }

    public IResourceUrlFactory ResourceUrlFactory
    {
      get { return _resourceUrlFactory; }
    }

    protected void AddStandardAttributesToRender (RenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID);

      if (!string.IsNullOrEmpty (renderingContext.Control.CssClass))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, renderingContext.Control.CssClass);

      CssStyleCollection styles = renderingContext.Control.ControlStyle.GetStyleAttributes (renderingContext.Control);
      foreach (string style in styles.Keys)
      {
        renderingContext.Writer.AddStyleAttribute (style, styles[style]);
      }

      foreach (string attribute in renderingContext.Control.Attributes.Keys)
      {
        string value = renderingContext.Control.Attributes[attribute];
        if (!string.IsNullOrEmpty (value))
          renderingContext.Writer.AddAttribute (attribute, value);
      }
    }

    protected void AppendStringValueOrNullToScript (StringBuilder scriptBuilder, string stringValue)
    {
      if (string.IsNullOrEmpty (stringValue))
        scriptBuilder.Append ("null");
      else
        scriptBuilder.Append ("'").Append (ScriptUtility.EscapeClientScript (stringValue)).Append ("'");
    }

    protected void AppendBooleanValueToScript (StringBuilder scriptBuilder, bool booleanValue)
    {
      scriptBuilder.Append (booleanValue ? "true" : "false");
    }

    protected void CheckScriptManager (IControl control, string errorMessageFormat, params object[] args)
    {
      ArgumentUtility.CheckNotNull ("control", control);
      ArgumentUtility.CheckNotNullOrEmpty ("errorMessageFormat", errorMessageFormat);
      ArgumentUtility.CheckNotNull ("args", args);

      var page = control.Page.WrappedInstance;
      if (page != null && ScriptManager.GetCurrent (page) == null)
        throw new InvalidOperationException (string.Format (errorMessageFormat, args));
    }
  }
}