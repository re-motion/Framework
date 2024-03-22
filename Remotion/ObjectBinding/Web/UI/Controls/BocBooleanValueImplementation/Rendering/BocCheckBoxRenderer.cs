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
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocBooleanValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocCheckBox"/> controls.
  /// <seealso cref="IBocCheckBox"/>
  /// </summary>
  [ImplementationFor(typeof(IBocCheckBoxRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocCheckBoxRenderer : BocBooleanValueRendererBase<IBocCheckBox>, IBocCheckBoxRenderer
  {
    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.ObjectBinding.Web.Globalization.BocCheckBoxRenderer")]
    public enum ResourceIdentifier
    {
      /// <summary> The aria-role description for the checkbox as a read-only element. </summary>
      ScreenReaderLabelForCheckboxReadOnly
    }

    private readonly ILabelReferenceRenderer _labelReferenceRenderer;
    private readonly IValidationErrorRenderer _validationErrorRenderer;

    private static readonly string s_startUpScriptKey = typeof(BocCheckBoxRenderer).GetFullNameChecked() + "_Startup";

    public BocCheckBoxRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures,
        ILabelReferenceRenderer labelReferenceRenderer,
        IValidationErrorRenderer validationErrorRenderer)
        : base(resourceUrlFactory, globalizationService, renderingFeatures)
    {
      ArgumentUtility.CheckNotNull("labelReferenceRenderer", labelReferenceRenderer);
      ArgumentUtility.CheckNotNull("validationErrorRenderer", validationErrorRenderer);

      _labelReferenceRenderer = labelReferenceRenderer;
      _validationErrorRenderer = validationErrorRenderer;
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterObjectBindingWebClientScriptInclude();
      htmlHeadAppender.RegisterCommonStyleSheet();

      string styleFileKey = typeof(BocCheckBoxRenderer).GetFullNameChecked() + "_Style";
      var styleUrl = ResourceUrlFactory.CreateThemedResourceUrl(typeof(BocCheckBoxRenderer), ResourceType.Html, "BocCheckbox.css");
      htmlHeadAppender.RegisterStylesheetLink(styleFileKey, styleUrl, HtmlHeadAppender.Priority.Library);
    }

    /// <summary>
    /// Renders an image and label in readonly mode, a checkbox and label in edit mode.
    /// </summary>
    public void Render (BocCheckBoxRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      AddAttributesToRender(renderingContext);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);

      var validationErrors = GetValidationErrorsToRender(renderingContext).ToArray();
      var validationErrorsID = GetValidationErrorsID(renderingContext);

      var checkBoxControl = new HtmlInputCheckBox { ID = renderingContext.Control.GetValueName(), ClientIDMode = ClientIDMode.Static };
      var labelControl = new Label { ID = renderingContext.Control.ClientID + "_Description", ClientIDMode = ClientIDMode.Static };
      var checkBoxVisualizerControl = new HtmlGenericControl { ID = null, TagName = "span" };

      var labelIDs = renderingContext.Control.GetLabelIDs().ToArray();

      if (renderingContext.Control.IsReadOnly)
      {
        var isChecked = renderingContext.Control.Value == true;
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, renderingContext.Control.GetValueName());
        renderingContext.Writer.AddAttribute("data-value", isChecked.ToString());
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Checkbox);
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.AriaChecked, isChecked ? HtmlAriaCheckedAttributeValue.True : HtmlAriaCheckedAttributeValue.False);
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.AriaReadOnly, HtmlAriaReadOnlyAttributeValue.True);
        renderingContext.Writer.AddAttribute(
            HtmlTextWriterAttribute2.AriaRoleDescription,
            GetResourceManager(renderingContext).GetString(ResourceIdentifier.ScreenReaderLabelForCheckboxReadOnly));
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassDefinition.ScreenReaderText);
        if (renderingContext.Control.Enabled)
        {
          renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Tabindex, "0");
        }
        else
        {
          renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.AriaDisabled, HtmlAriaDisabledAttributeValue.True);
        }

        _labelReferenceRenderer.AddLabelsReference(renderingContext.Writer, labelIDs);

        var attributeCollection = new AttributeCollection(new StateBag());

        if (renderingContext.Control.IsDescriptionEnabled)
          attributeCollection.Add(HtmlTextWriterAttribute2.AriaDescribedBy, labelControl.ClientID);

        _validationErrorRenderer.AddValidationErrorsReference(attributeCollection, validationErrorsID, validationErrors);

        attributeCollection.AddAttributes(renderingContext.Writer);

        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
        renderingContext.Writer.RenderEndTag();

        var description = GetDescription(renderingContext);
        PrepareLabel(renderingContext, description, labelControl);
        labelControl.RenderControl(renderingContext.Writer);
      }
      else
      {
        bool hasClientScript = DetermineClientScriptLevel(renderingContext);
        if (hasClientScript)
        {
          PrepareScripts(renderingContext, checkBoxControl, labelControl, checkBoxVisualizerControl);
        }

        checkBoxControl.Checked = renderingContext.Control.Value!.Value;
        checkBoxControl.Disabled = !renderingContext.Control.Enabled;

        _labelReferenceRenderer.SetLabelReferenceOnControl(checkBoxControl, labelIDs);

        if (renderingContext.Control.IsDescriptionEnabled)
          checkBoxControl.Attributes.Add(HtmlTextWriterAttribute2.AriaDescribedBy, labelControl.ClientID);

        _validationErrorRenderer.SetValidationErrorsReferenceOnControl(checkBoxControl, validationErrorsID, validationErrors);

        checkBoxControl.RenderControl(renderingContext.Writer);
        checkBoxVisualizerControl.RenderControl(renderingContext.Writer);

        if (renderingContext.Control.IsDescriptionEnabled)
        {
          var description = GetDescription(renderingContext);
          PrepareLabel(renderingContext, description, labelControl);
          labelControl.RenderControl(renderingContext.Writer);
        }
      }

      _validationErrorRenderer.RenderValidationErrors(renderingContext.Writer, validationErrorsID, validationErrors);
      renderingContext.Writer.RenderEndTag();
    }

    protected override void AddDiagnosticMetadataAttributes (RenderingContext<IBocCheckBox> renderingContext)
    {
      base.AddDiagnosticMetadataAttributes(renderingContext);

      var hasAutoPostBack = renderingContext.Control.IsAutoPostBackEnabled;
      renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributes.TriggersPostBack, hasAutoPostBack.ToString().ToLower());
    }

    private bool DetermineClientScriptLevel (BocCheckBoxRenderingContext renderingContext)
    {
      return true;
    }

    private void PrepareScripts (BocCheckBoxRenderingContext renderingContext, HtmlInputCheckBox checkBoxControl, Label labelControl, HtmlGenericControl checkBoxVisualizerControl)
    {
      string checkBoxScript;
      string labelScript;

      if (renderingContext.Control.Enabled)
      {
        RegisterStartupScriptIfNeeded(renderingContext);

        string script = GetScriptParameters(renderingContext);
        checkBoxScript = "BocCheckBox.OnClick" + script;
        labelScript = "BocCheckBox.ToggleCheckboxValue" + script;
      }
      else
      {
        checkBoxScript = "return false;";
        labelScript = "return false;";
      }
      checkBoxControl.Attributes.Add("onclick", checkBoxScript);
      labelControl.Attributes.Add("onclick", labelScript);
      checkBoxVisualizerControl.Attributes.Add("onclick", labelScript);
    }

    private string GetScriptParameters (BocCheckBoxRenderingContext renderingContext)
    {
      string label = renderingContext.Control.IsDescriptionEnabled ? "this.parentElement.querySelector (':scope > span.description')" : "null";
      string checkBox = "this.parentElement.querySelector (':scope > input')";
      string script = " ("
                      + checkBox + ", "
                      + label + ", "
                      + (renderingContext.Control.TrueDescription.IsEmpty
                          ? "null"
                          : "'" + ScriptUtility.EscapeClientScript(renderingContext.Control.TrueDescription) + "'") + ", "
                      + (renderingContext.Control.FalseDescription.IsEmpty
                          ? "null"
                          : "'" + ScriptUtility.EscapeClientScript(renderingContext.Control.FalseDescription) + "'") + ");";

      if (renderingContext.Control.IsAutoPostBackEnabled)
        script += renderingContext.Control.Page!.ClientScript.GetPostBackEventReference(renderingContext.Control, "") + ";";
      return script;
    }

    private void RegisterStartupScriptIfNeeded (BocCheckBoxRenderingContext renderingContext)
    {
      if (renderingContext.Control.Page!.ClientScript.IsStartupScriptRegistered(typeof(BocCheckBoxRenderer), s_startUpScriptKey))
        return;

      string startupScript = string.Format(
          "BocCheckBox.InitializeGlobals ('{0}', '{1}');",
          ScriptUtility.EscapeClientScript(renderingContext.Control.DefaultTrueDescription),
          ScriptUtility.EscapeClientScript(renderingContext.Control.DefaultFalseDescription));
      renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock(renderingContext.Control, typeof(BocCheckBoxRenderer), s_startUpScriptKey, startupScript);
    }

    private void PrepareLabel (BocCheckBoxRenderingContext renderingContext, WebString description, Label labelControl)
    {
      labelControl.CssClass = "description";
      labelControl.Text = description.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks);
      labelControl.Width = Unit.Empty;
      labelControl.Height = Unit.Empty;
      labelControl.ApplyStyle(renderingContext.Control.LabelStyle);
    }

    private WebString GetDescription (BocCheckBoxRenderingContext renderingContext)
    {
      WebString defaultTrueDescription = renderingContext.Control.DefaultTrueDescription;
      WebString defaultFalseDescription = renderingContext.Control.DefaultFalseDescription;

      WebString trueDescription = renderingContext.Control.TrueDescription.IsEmpty
          ? defaultTrueDescription
          : renderingContext.Control.TrueDescription;

      WebString falseDescription = renderingContext.Control.FalseDescription.IsEmpty
          ? defaultFalseDescription
          : renderingContext.Control.FalseDescription;

      return renderingContext.Control.Value!.Value ? trueDescription : falseDescription;
    }

    public override string GetCssClassBase (IBocCheckBox control)
    {
      return "bocCheckBox";
    }

    protected virtual IResourceManager GetResourceManager (BocCheckBoxRenderingContext renderingContext)
    {
      return GetResourceManager(typeof(ResourceIdentifier), renderingContext.Control.GetResourceManager());
    }
  }
}
