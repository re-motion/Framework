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
using System.Reflection;
using System.Text;
using System.Web.UI;
using Remotion.Collections;
using Remotion.Globalization;
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{
  /// <summary>
  /// Base class for all renderers. Contains the essential properties used in rendering.
  /// </summary>
  /// <typeparam name="TControl">The type of control that can be rendered.</typeparam>
  public abstract class RendererBase<TControl>
      where TControl : IStyledControl, IControlWithDiagnosticMetadata
  {
    private readonly ICache<Tuple<Type, IResourceManager>, IResourceManager> _resourceManagerCache =
        CacheFactory.Create<Tuple<Type, IResourceManager>, IResourceManager>();

    private readonly IResourceUrlFactory _resourceUrlFactory;
    private readonly IGlobalizationService _globalizationService;
    private readonly IRenderingFeatures _renderingFeatures;

    /// <summary>
    /// Initializes the <see cref="Context"/> and the <see cref="Control"/> properties from the arguments.
    /// </summary>
    protected RendererBase (IResourceUrlFactory resourceUrlFactory, IGlobalizationService globalizationService, IRenderingFeatures renderingFeatures)
    {
      ArgumentUtility.CheckNotNull ("resourceUrlFactory", resourceUrlFactory);
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);
      ArgumentUtility.CheckNotNull ("renderingFeatures", renderingFeatures);

      _resourceUrlFactory = resourceUrlFactory;
      _globalizationService = globalizationService;
      _renderingFeatures = renderingFeatures;
    }

    /// <summary>
    /// Gets the <see cref="IResourceUrlFactory"/> that can be used for resolving resource urls.
    /// </summary>
    protected IResourceUrlFactory ResourceUrlFactory
    {
      get { return _resourceUrlFactory; }
    }

    /// <summary>
    /// Returns the configured <see cref="IRenderingFeatures"/> object.
    /// </summary>
    protected IRenderingFeatures RenderingFeatures
    {
      get { return _renderingFeatures; }
    }

    /// <summary>
    /// Returns whether the diagnostic metadata rendering feature is enabled and thereby whether a specific control renderer should render such
    /// information as additional data attributes.
    /// </summary>
    protected bool IsDiagnosticMetadataRenderingEnabled
    {
      get { return _renderingFeatures.EnableDiagnosticMetadata; }
    }

    protected void AddStandardAttributesToRender (RenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID);

      if (!string.IsNullOrEmpty (renderingContext.Control.CssClass))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, renderingContext.Control.CssClass);

      CssStyleCollection styles = renderingContext.Control.ControlStyle.GetStyleAttributes (renderingContext.Control);
      foreach (string style in styles.Keys)
        renderingContext.Writer.AddStyleAttribute (style, styles[style]);

      foreach (string attribute in renderingContext.Control.Attributes.Keys)
      {
        string value = renderingContext.Control.Attributes[attribute];
        if (!string.IsNullOrEmpty (value))
          renderingContext.Writer.AddAttribute (attribute, value);
      }

      if (IsDiagnosticMetadataRenderingEnabled)
        AddDiagnosticMetadataAttributes (renderingContext);
    }

    protected virtual void AddDiagnosticMetadataAttributes (RenderingContext<TControl> renderingContext)
    {
      renderingContext.Writer.AddAttribute (DiagnosticMetadataAttributes.ControlType, renderingContext.Control.ControlType);
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

    /// <summary> Find the <see cref="IResourceManager"/> for this renderer. </summary>
    /// <param name="localResourcesType"> 
    ///   A type with the <see cref="MultiLingualResourcesAttribute"/> applied to it. Typically an <b>enum</b> or the derived class itself.
    /// </param>
    /// <param name="controlResourceManager"> The <see cref="IResourceManager"/> of the control for which the rendering is done. </param>
    /// <returns>An <see cref="IResourceManager"/> from which all resources for this renderer can be obtained.</returns>
    protected IResourceManager GetResourceManager (Type localResourcesType, IResourceManager controlResourceManager)
    {
      ArgumentUtility.CheckNotNull ("localResourcesType", localResourcesType);
      ArgumentUtility.CheckNotNull ("controlResourceManager", controlResourceManager);

      return _resourceManagerCache.GetOrCreateValue (
          Tuple.Create (localResourcesType, controlResourceManager),
          key => ResourceManagerSet.Create (key.Item2, _globalizationService.GetResourceManager (key.Item1)));
    }
  }
}