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
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocCheckBox"/> controls.
  /// <seealso cref="IBocCheckBox"/>
  /// </summary>
  [ImplementationFor (typeof (IBocCheckBoxRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocCheckBoxRenderer : BocBooleanValueRendererBase<IBocCheckBox>, IBocCheckBoxRenderer
  {
    private const string c_trueIcon = "CheckBoxTrue.gif";
    private const string c_falseIcon = "CheckBoxFalse.gif";

    private static readonly string s_startUpScriptKey = typeof (BocCheckBoxRenderer).FullName + "_Startup";

    public BocCheckBoxRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures)
        : base (resourceUrlFactory, globalizationService, renderingFeatures)
    {
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      string scriptFileKey = typeof (BocCheckBoxRenderer).FullName + "_Script";
      var scriptUrl = ResourceUrlFactory.CreateResourceUrl (typeof (BocCheckBoxRenderer), ResourceType.Html, "BocCheckbox.js");
      htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptUrl);

      string styleFileKey = typeof (BocCheckBoxRenderer).FullName + "_Style";
      var styleUrl = ResourceUrlFactory.CreateThemedResourceUrl (typeof (BocCheckBoxRenderer), ResourceType.Html, "BocCheckbox.css");
      htmlHeadAppender.RegisterStylesheetLink (styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
    }

    /// <summary>
    /// Renders an image and label in readonly mode, a checkbox and label in edit mode.
    /// </summary>
    public void Render (BocCheckBoxRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      AddAttributesToRender (renderingContext);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      var checkBoxControl = new HtmlInputCheckBox { ID = renderingContext.Control.GetValueName(), ClientIDMode = ClientIDMode.Static };
      var labelControl = new Label();

      string description = GetDescription (renderingContext);

      if (renderingContext.Control.IsReadOnly)
      {
        var imageControl = new Image();
        PrepareImage (renderingContext, imageControl, description);

        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.GetValueName());
        if (renderingContext.Control.Value.HasValue)
          renderingContext.Writer.AddAttribute ("data-value", renderingContext.Control.Value.Value.ToString ());
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
        imageControl.RenderControl (renderingContext.Writer);
        renderingContext.Writer.RenderEndTag ();

        PrepareLabel (renderingContext, description, labelControl);
        labelControl.RenderControl (renderingContext.Writer);
      }
      else
      {
        bool hasClientScript = DetermineClientScriptLevel (renderingContext);
        if (hasClientScript)
        {
          PrepareScripts (renderingContext, checkBoxControl, labelControl);
        }

        checkBoxControl.Checked = renderingContext.Control.Value.Value;
        checkBoxControl.Disabled = !renderingContext.Control.Enabled;

        checkBoxControl.RenderControl (renderingContext.Writer);

        PrepareLabel (renderingContext, description, labelControl);
        labelControl.RenderControl (renderingContext.Writer);
      }

      renderingContext.Writer.RenderEndTag ();
    }

    protected override void AddDiagnosticMetadataAttributes (RenderingContext<IBocCheckBox> renderingContext)
    {
      base.AddDiagnosticMetadataAttributes (renderingContext);

      var hasAutoPostBack = renderingContext.Control.IsAutoPostBackEnabled;
      renderingContext.Writer.AddAttribute (DiagnosticMetadataAttributes.TriggersPostBack, hasAutoPostBack.ToString().ToLower());
    }

    private bool DetermineClientScriptLevel (BocCheckBoxRenderingContext renderingContext)
    {
      return !renderingContext.Control.IsDesignMode;
    }
    
    private void PrepareScripts (BocCheckBoxRenderingContext renderingContext, HtmlInputCheckBox checkBoxControl, Label labelControl)
    {
      string checkBoxScript;
      string labelScript;

      if (renderingContext.Control.Enabled)
      {
        RegisterStartupScriptIfNeeded(renderingContext);

        string script = GetScriptParameters(renderingContext);
        checkBoxScript = "BocCheckBox_OnClick" + script;
        labelScript = "BocCheckBox_ToggleCheckboxValue" + script;
      }
      else
      {
        checkBoxScript = "return false;";
        labelScript = "return false;";
      }
      checkBoxControl.Attributes.Add ("onclick", checkBoxScript);
      labelControl.Attributes.Add ("onclick", labelScript);
    }

    private string GetScriptParameters (BocCheckBoxRenderingContext renderingContext)
    {
      string label = renderingContext.Control.IsDescriptionEnabled ? "$(this).parent().children('span').first()[0]" : "null";
      string checkBox = "$(this).parent().children('input').first()[0]";
      string script = " ("
                      + checkBox + ", "
                      + label + ", "
                      + (string.IsNullOrEmpty (renderingContext.Control.TrueDescription) ? "null" : "'" + renderingContext.Control.TrueDescription + "'") + ", "
                      + (string.IsNullOrEmpty (renderingContext.Control.FalseDescription) ? "null" : "'" + renderingContext.Control.FalseDescription + "'") + ");";

      if (renderingContext.Control.IsAutoPostBackEnabled)
        script += renderingContext.Control.Page.ClientScript.GetPostBackEventReference (renderingContext.Control, "") + ";";
      return script;
    }

    private void RegisterStartupScriptIfNeeded (BocCheckBoxRenderingContext renderingContext)
    {
      if (renderingContext.Control.Page.ClientScript.IsStartupScriptRegistered (typeof (BocCheckBoxRenderer), s_startUpScriptKey))
        return;

      string startupScript = string.Format (
          "BocCheckBox_InitializeGlobals ('{0}', '{1}');",
          renderingContext.Control.DefaultTrueDescription,
          renderingContext.Control.DefaultFalseDescription);
      renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (renderingContext.Control, typeof (BocCheckBoxRenderer), s_startUpScriptKey, startupScript);
    }

    private void PrepareImage (BocCheckBoxRenderingContext renderingContext, Image imageControl, string description)
    {
      var imageUrl = ResourceUrlFactory.CreateThemedResourceUrl (
          typeof (BocCheckBox),
          ResourceType.Image,
          renderingContext.Control.Value.Value ? c_trueIcon : c_falseIcon);

      imageControl.ImageUrl = imageUrl.GetUrl();
      imageControl.AlternateText = description ?? string.Empty;
      imageControl.GenerateEmptyAlternateText = true;
    }

    private void PrepareLabel (BocCheckBoxRenderingContext renderingContext, string description, Label labelControl)
    {
      if (renderingContext.Control.IsDescriptionEnabled)
      {
        labelControl.Text = description;
        labelControl.Width = Unit.Empty;
        labelControl.Height = Unit.Empty;
        labelControl.ApplyStyle (renderingContext.Control.LabelStyle);
      }
    }

    private string GetDescription (BocCheckBoxRenderingContext renderingContext)
    {
      string trueDescription = null;
      string falseDescription = null;
      if (renderingContext.Control.IsDescriptionEnabled)
      {
        string defaultTrueDescription = renderingContext.Control.DefaultTrueDescription;
        string defaultFalseDescription = renderingContext.Control.DefaultFalseDescription;

        trueDescription = (string.IsNullOrEmpty (renderingContext.Control.TrueDescription) ? defaultTrueDescription : renderingContext.Control.TrueDescription);
        falseDescription = (string.IsNullOrEmpty (renderingContext.Control.FalseDescription) ? defaultFalseDescription : renderingContext.Control.FalseDescription);
      }
      return renderingContext.Control.Value.Value ? trueDescription : falseDescription;
    }

    public override string GetCssClassBase(IBocCheckBox control)
    {
      return "bocCheckBox";
    }
  }
}