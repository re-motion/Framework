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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Rendering;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocBooleanValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocCheckBox"/> controls.
  /// <seealso cref="IBocCheckBox"/>
  /// </summary>
  /// <include file='..\..\..\..\doc\include\UI\Controls\BocCheckBoxRenderer.xml' path='BocCheckBoxRenderer/Class/*'/>
  public class BocCheckBoxQuirksModeRenderer : BocBooleanValueQuirksModeRendererBase<IBocCheckBox>, IBocCheckBoxRenderer
  {
    private const string c_trueIcon = "CheckBoxTrue.gif";
    private const string c_falseIcon = "CheckBoxFalse.gif";

    private static readonly string s_startUpScriptKey = typeof (BocCheckBoxQuirksModeRenderer).FullName + "_Startup";

    public BocCheckBoxQuirksModeRenderer (IResourceUrlFactory resourceUrlFactory) 
      : base(resourceUrlFactory)
    { 
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      string scriptFileKey = typeof (BocCheckBoxQuirksModeRenderer).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (scriptFileKey))
      {
        var scriptUrl = ResourceUrlFactory.CreateResourceUrl (typeof (BocCheckBoxQuirksModeRenderer), ResourceType.Html, "BocCheckbox.js");
        htmlHeadAppender.RegisterJavaScriptInclude (scriptFileKey, scriptUrl);
      }

      string styleFileKey = typeof (BocCheckBoxQuirksModeRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (styleFileKey))
      {
        var styleUrl = ResourceUrlFactory.CreateResourceUrl (typeof (BocCheckBoxQuirksModeRenderer), ResourceType.Html, "BocCheckbox.css");
        htmlHeadAppender.RegisterStylesheetLink (styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
      }
    }

    /// <summary>
    /// Renders an image and label in readonly mode, a checkbox and label in edit mode.
    /// </summary>
    public void Render (BocCheckBoxRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      AddAttributesToRender (renderingContext, false);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      Label labelControl = new Label { ID = GetLabelName (renderingContext), ClientIDMode = ClientIDMode.Static };
      HtmlInputCheckBox checkBoxControl = new HtmlInputCheckBox { ID = renderingContext.Control.GetValueName (), ClientIDMode = ClientIDMode.Static };
      Image imageControl = new Image { ID = renderingContext.Control.ClientID + "Image" };

      string description = GetDescription (renderingContext);

      if (renderingContext.Control.IsReadOnly)
      {
        PrepareImage (renderingContext, imageControl, description);
        PrepareLabel (renderingContext, description, labelControl);

        imageControl.RenderControl (renderingContext.Writer);
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

        PrepareLabel (renderingContext, description, labelControl);

        checkBoxControl.RenderControl (renderingContext.Writer);
        labelControl.RenderControl (renderingContext.Writer);
      }

      renderingContext.Writer.RenderEndTag ();
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
      string label = renderingContext.Control.IsDescriptionEnabled ? "document.getElementById ('" + GetLabelName (renderingContext) + "')" : "null";
      string checkBox = "document.getElementById ('" + renderingContext.Control.GetValueName() + "')";
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
      if (renderingContext.Control.Page.ClientScript.IsStartupScriptRegistered (typeof (BocCheckBoxQuirksModeRenderer), s_startUpScriptKey))
        return;

      string startupScript = string.Format (
          "BocCheckBox_InitializeGlobals ('{0}', '{1}');",
          renderingContext.Control.DefaultTrueDescription,
          renderingContext.Control.DefaultFalseDescription);
      renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (renderingContext.Control, typeof (BocCheckBoxQuirksModeRenderer), s_startUpScriptKey, startupScript);
    }

    private void PrepareImage (BocCheckBoxRenderingContext renderingContext, Image imageControl, string description)
    {
      var imageUrl = ResourceUrlFactory.CreateResourceUrl (
          typeof (BocCheckBoxQuirksModeRenderer),
          ResourceType.Image,
          renderingContext.Control.Value.Value ? c_trueIcon : c_falseIcon);

      imageControl.ImageUrl = imageUrl.GetUrl();
      imageControl.AlternateText = description ?? string.Empty;
      imageControl.GenerateEmptyAlternateText = true;
      imageControl.Style["vertical-align"] = "middle";
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

    private string GetLabelName (BocCheckBoxRenderingContext renderingContext)
    {
      return renderingContext.Control.ClientID + "_TextValue";
    }

    public override string CssClassBase
    {
      get { return "bocCheckBox"; }
    }
  }
}