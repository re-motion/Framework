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
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocReferenceValue"/> controls in Standards Mode.
  /// </summary>
  /// <remarks>
  /// <para>During edit mode, the control is displayed using a <see cref="System.Web.UI.WebControls.DropDownList"/>.</para>
  /// <para>During read-only mode, the control's value is displayed using a <see cref="System.Web.UI.WebControls.Label"/>.</para>
  /// </remarks>
  [ImplementationFor (typeof (IBocReferenceValueRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocReferenceValueRenderer : BocReferenceValueRendererBase<IBocReferenceValue>, IBocReferenceValueRenderer
  {
    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources ("Remotion.ObjectBinding.Web.Globalization.BocReferenceValueRenderer")]
    public enum ResourceIdentifier
    {
      /// <summary> The error message dispayed when the icon could not be loaded from the server. </summary>
      LoadIconFailedErrorMessage,
    }

    private readonly Func<DropDownList> _dropDownListFactoryMethod;

    public BocReferenceValueRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures)
        : this (resourceUrlFactory, globalizationService, renderingFeatures, () => new DropDownList())
    {
    }

    protected BocReferenceValueRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures,
        Func<DropDownList> dropDownListFactoryMethod)
        : base (resourceUrlFactory, globalizationService, renderingFeatures)
    {
      ArgumentUtility.CheckNotNull ("dropDownListFactoryMethod", dropDownListFactoryMethod);
      _dropDownListFactoryMethod = dropDownListFactoryMethod;
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude();
      RegisterBrowserCompatibilityScript (htmlHeadAppender);
      RegisterJavaScriptFiles (htmlHeadAppender);
      RegisterStylesheets (htmlHeadAppender);
    }

    public void Render (BocReferenceValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      base.Render (renderingContext);

      RegisterInitializationScript (renderingContext);
    }

    protected override void AddDiagnosticMetadataAttributes (RenderingContext<IBocReferenceValue> renderingContext)
    {
      base.AddDiagnosticMetadataAttributes (renderingContext);

      var hasAutoPostBack = renderingContext.Control.DropDownListStyle.AutoPostBack.HasValue
                            && renderingContext.Control.DropDownListStyle.AutoPostBack.Value;
      renderingContext.Writer.AddAttribute (DiagnosticMetadataAttributes.TriggersPostBack, hasAutoPostBack.ToString().ToLower());
    }

    protected override void RegisterJavaScriptFiles (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      base.RegisterJavaScriptFiles (htmlHeadAppender);

      string scriptFileKey = typeof (BocReferenceValueRenderer).FullName + "_Script";
      var scriptUrl = ResourceUrlFactory.CreateResourceUrl (typeof (BocReferenceValueRenderer), ResourceType.Html, "BocReferenceValue.js");
      htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptUrl);
    }

    private void RegisterStylesheets (HtmlHeadAppender htmlHeadAppender)
    {
      string styleFileKey = typeof (BocReferenceValueRenderer).FullName + "_Style";
      var styleUrl = ResourceUrlFactory.CreateThemedResourceUrl (typeof (BocReferenceValueRenderer), ResourceType.Html, "BocReferenceValue.css");
      htmlHeadAppender.RegisterStylesheetLink (styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
    }

    private void RegisterInitializationScript (BocReferenceValueRenderingContext renderingContext)
    {
      if (renderingContext.Control.IsReadOnly)
        return;

      if (!renderingContext.Control.Enabled)
        return;

      string key = renderingContext.Control.ClientID + "_InitializationScript";

      var script = new StringBuilder (1000);
      script.Append ("$(document).ready( function() { BocReferenceValue.Initialize(");
      script.AppendFormat ("$('#{0}'), ", renderingContext.Control.GetValueName());

      if (renderingContext.Control.IsIconEnabled())
        script.AppendFormat ("$('#{0} .{1}'), ", renderingContext.Control.ClientID, CssClassCommand);
      else
        script.Append ("null, ");

      script.AppendFormat ("'{0}', ", renderingContext.Control.NullValueString);
      AppendBooleanValueToScript (script, renderingContext.Control.DropDownListStyle.AutoPostBack ?? false);
      script.Append (", ");
      AppendStringValueOrNullToScript (script, GetIconServicePath (renderingContext));
      script.Append (", ");
      script.Append (GetIconContextAsJson (renderingContext.IconWebServiceContext) ?? "null");
      script.Append (", ");
      script.Append (GetCommandInfoAsJson (renderingContext) ?? "null");
      script.Append (", ");
      script.Append (GetResourcesAsJson (renderingContext));
      script.Append ("); } );");

      renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (
          renderingContext.Control,
          typeof (IBocReferenceValue),
          key,
          script.ToString());
    }

    private string GetResourcesAsJson (BocReferenceValueRenderingContext renderingContext)
    {
      var resourceManager = GetResourceManager (renderingContext);
      var jsonBuilder = new StringBuilder (1000);

      jsonBuilder.Append ("{ ");
      jsonBuilder.Append ("LoadIconFailedErrorMessage : ");
      AppendStringValueOrNullToScript (jsonBuilder, resourceManager.GetString (ResourceIdentifier.LoadIconFailedErrorMessage));
      jsonBuilder.Append (" }");

      return jsonBuilder.ToString();
    }

    protected virtual IResourceManager GetResourceManager (BocReferenceValueRenderingContext renderingContext)
    {
      return GetResourceManager (typeof (ResourceIdentifier), renderingContext.Control.GetResourceManager());
    }

    protected override sealed void RenderEditModeValueWithSeparateOptionsMenu (BocRenderingContext<IBocReferenceValue> renderingContext)
    {
      DropDownList dropDownList = GetDropDownList (renderingContext);
      RenderEditModeValue (renderingContext, dropDownList);
    }

    protected override sealed void RenderEditModeValueWithIntegratedOptionsMenu (BocRenderingContext<IBocReferenceValue> renderingContext)
    {
      DropDownList dropDownList = GetDropDownList (renderingContext);
      dropDownList.Attributes.Add ("onclick", DropDownMenu.OnHeadTitleClickScript);
      RenderEditModeValue (renderingContext, dropDownList);
    }

    private void RenderEditModeValue (BocRenderingContext<IBocReferenceValue> renderingContext, DropDownList dropDownList)
    {
      dropDownList.RenderControl (renderingContext.Writer);
    }

    private DropDownList GetDropDownList (BocRenderingContext<IBocReferenceValue> renderingContext)
    {
      var dropDownList = _dropDownListFactoryMethod();
      dropDownList.ClientIDMode = ClientIDMode.Static;
      dropDownList.ID = renderingContext.Control.GetValueName();
      dropDownList.EnableViewState = false;
      dropDownList.Page = renderingContext.Control.Page.WrappedInstance;
      renderingContext.Control.PopulateDropDownList (dropDownList);

      dropDownList.Enabled = renderingContext.Control.Enabled;
      dropDownList.Height = Unit.Empty;
      dropDownList.Width = Unit.Empty;
      dropDownList.ApplyStyle (renderingContext.Control.CommonStyle);
      renderingContext.Control.DropDownListStyle.ApplyStyle (dropDownList);

      return dropDownList;
    }

    public override string GetCssClassBase (IBocReferenceValue control)
    {
      return "bocReferenceValue";
    }
  }
}