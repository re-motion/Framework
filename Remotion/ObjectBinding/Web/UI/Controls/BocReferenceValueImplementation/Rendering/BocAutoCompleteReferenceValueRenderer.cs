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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.Services;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Globalization;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocAutoCompleteReferenceValue"/> controls in Standards Mode.
  /// </summary>
  /// <remarks>
  /// <para>During edit mode, the control is displayed using a <see cref="System.Web.UI.WebControls.TextBox"/> and a pop-up element.</para>
  /// <para>During read-only mode, the control's value is displayed using a <see cref="System.Web.UI.WebControls.Label"/>.</para>
  /// </remarks>
  [ImplementationFor(typeof(IBocAutoCompleteReferenceValueRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocAutoCompleteReferenceValueRenderer : BocReferenceValueRendererBase<IBocAutoCompleteReferenceValue>, IBocAutoCompleteReferenceValueRenderer
  {
    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.ObjectBinding.Web.Globalization.BocAutoCompleteReferenceValueRenderer")]
    public enum ResourceIdentifier
    {
      /// <summary> The error message dispayed when the icon could not be loaded from the server. </summary>
      LoadIconFailedErrorMessage,
      /// <summary> The error message dispayed when the business object information for the current search input could not be loaded from the server. </summary>
      LoadDataFailedErrorMessage,
      /// <summary> The message displayed when the user input does not match the regex required for triggering the drop-down list to open.</summary>
      SearchStringForDropDownDoesNotMatchRegexMessage,
      /// <summary> The message announced to screen reader users when new matches are being loaded from the server. </summary>
      LoadingMoreMatchesMessage,
      /// <summary> The message announced to screen reader users when new matches were loaded from the server and the result list has been expanded. </summary>
      LoadedMoreMatchesMessage,
      /// <summary> The message displayed when the user input does not result in a result.</summary>
      NoDataFoundMessage,
      /// <summary> The aria-role description for the combobox as a read-only element. </summary>
      ScreenReaderLabelForComboboxReadOnly,
    }

    private readonly Func<TextBox> _textBoxFactory;

    public BocAutoCompleteReferenceValueRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures,
        ILabelReferenceRenderer labelReferenceRenderer,
        IValidationErrorRenderer validationErrorRenderer)
        : this(resourceUrlFactory, globalizationService, renderingFeatures, labelReferenceRenderer, validationErrorRenderer, () => new RenderOnlyTextBox())
    {
    }

    protected BocAutoCompleteReferenceValueRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures,
        ILabelReferenceRenderer labelReferenceRenderer,
        IValidationErrorRenderer validationErrorRenderer,
        Func<TextBox> textBoxFactory)
        : base(resourceUrlFactory, globalizationService, renderingFeatures, labelReferenceRenderer, validationErrorRenderer)
    {
      ArgumentUtility.CheckNotNull("textBoxFactory", textBoxFactory);
      _textBoxFactory = textBoxFactory;
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      RegisterJavaScriptFiles(htmlHeadAppender);
      RegisterStylesheets(htmlHeadAppender);
    }

    public void Render (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      base.Render(renderingContext);

      RegisterInitializationScript(renderingContext);

      CheckScriptManager(
          renderingContext.Control,
          "{0} '{1}' requires that the page contains a ScriptManager.",
          renderingContext.Control.GetType().Name,
          renderingContext.Control.ID);

    }

    protected override void AddDiagnosticMetadataAttributes (RenderingContext<IBocAutoCompleteReferenceValue> renderingContext)
    {
      base.AddDiagnosticMetadataAttributes(renderingContext);

      var hasAutoPostBack = renderingContext.Control.TextBoxStyle.AutoPostBack.HasValue && renderingContext.Control.TextBoxStyle.AutoPostBack.Value;
      renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributes.TriggersPostBack, hasAutoPostBack.ToString().ToLower());
    }

    private void RegisterStylesheets (HtmlHeadAppender htmlHeadAppender)
    {
      htmlHeadAppender.RegisterCommonStyleSheet();

      string styleKey = typeof(BocAutoCompleteReferenceValueRenderer).GetFullNameChecked() + "_Style";
      htmlHeadAppender.RegisterStylesheetLink(
          styleKey,
          ResourceUrlFactory.CreateThemedResourceUrl(
              typeof(BocAutoCompleteReferenceValueRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.css"),
          HtmlHeadAppender.Priority.Library);

      string autocompleteStyleKey = typeof(BocAutoCompleteReferenceValueRenderer).GetFullNameChecked() + "_AutoCompleteStyle";
      htmlHeadAppender.RegisterStylesheetLink(
          autocompleteStyleKey,
          ResourceUrlFactory.CreateThemedResourceUrl(
              typeof(BocAutoCompleteReferenceValueRenderer),
              ResourceType.Html,
              "BocAutoCompleteReferenceValue.UI.css"),
          HtmlHeadAppender.Priority.Library);
    }

    private void RegisterInitializationScript (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      if (renderingContext.Control.IsReadOnly)
        return;

      if (!renderingContext.Control.Enabled)
        return;

      string key = renderingContext.Control.ClientID + "_InitializationScript";
      var controlServicePath = GetControlServicePath(renderingContext);
      var isIconUpdateEnabled = controlServicePath != null && renderingContext.Control.IsIconEnabled();

      var script = new StringBuilder(1000);
      script.Append("new BocAutoCompleteReferenceValue(");
      script.AppendFormat("'{0}', ", renderingContext.Control.ClientID);
      script.AppendFormat("'#{0}', ", renderingContext.Control.GetTextValueName());
      script.AppendFormat("'#{0}', ", renderingContext.Control.GetKeyValueName());
      script.AppendFormat("'#{0}',", GetDropDownButtonName(renderingContext));

      if (renderingContext.Control.IsIconEnabled())
      {
        script.AppendFormat("'#{0} .{1}', ", renderingContext.Control.ClientID, CssClassInnerContent);
        script.AppendFormat("'#{0} .{1} img', ", renderingContext.Control.ClientID, CssClassIcon);
      }
      else
      {
        script.Append("null, ");
        script.Append("null, ");
      }

      script.AppendFormat("'{0}', ", controlServicePath);

      script.AppendFormat("{0}, ", renderingContext.Control.CompletionSetCount);
      script.AppendFormat("{0}, ", renderingContext.Control.DropDownDisplayDelay);
      script.AppendFormat("{0}, ", renderingContext.Control.DropDownRefreshDelay);
      script.AppendFormat("{0}, ", renderingContext.Control.SelectionUpdateDelay);

      script.Append(GetSearchStringValidationInfoAsJson(renderingContext));
      script.Append(", ");

      script.AppendFormat("'{0}', ", renderingContext.Control.NullValueString);
      AppendBooleanValueToScript(script, renderingContext.Control.TextBoxStyle.AutoPostBack ?? false);
      script.Append(", ");
      script.Append(GetSearchContextAsJson(renderingContext.BusinessObjectWebServiceContext));
      script.Append(", ");
      AppendBooleanValueToScript(script, isIconUpdateEnabled);
      script.Append(", ");
      script.Append(isIconUpdateEnabled ? GetIconContextAsJson(renderingContext) : "null");
      script.Append(", ");
      script.Append(GetResourcesAsJson(renderingContext));
      script.Append(");");

      renderingContext.Control.Page!.ClientScript.RegisterStartupScriptBlock(
          renderingContext.Control, typeof(IBocAutoCompleteReferenceValue), key, script.ToString());
    }

    private  string GetDropDownButtonName (BocRenderingContext<IBocAutoCompleteReferenceValue> renderingContext)
    {
      return renderingContext.Control.ClientID + "_DropDownButton";
    }

    private string GetSearchContextAsJson (BusinessObjectWebServiceContext searchContext)
    {
      var jsonBuilder = new StringBuilder(1000);

      jsonBuilder.Append("{ ");
      jsonBuilder.Append("businessObjectClass : ");
      AppendStringValueOrNullToScript(jsonBuilder, searchContext.BusinessObjectClass);
      jsonBuilder.Append(", ");
      jsonBuilder.Append("businessObjectProperty : ");
      AppendStringValueOrNullToScript(jsonBuilder, searchContext.BusinessObjectProperty);
      jsonBuilder.Append(", ");
      jsonBuilder.Append("businessObject : ");
      AppendStringValueOrNullToScript(jsonBuilder, searchContext.BusinessObjectIdentifier);
      jsonBuilder.Append(", ");
      jsonBuilder.Append("args : ");
      AppendStringValueOrNullToScript(jsonBuilder, searchContext.Arguments);
      jsonBuilder.Append(" }");

      return jsonBuilder.ToString();
    }

    private string GetSearchStringValidationInfoAsJson (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      string searchString = StringUtility.EmptyToNull(renderingContext.Control.ValidSearchStringRegex) ?? "\\S*";
      string searchStringForDropDown = StringUtility.EmptyToNull(renderingContext.Control.ValidSearchStringForDropDownRegex) ?? searchString;
      string searchStringForDropDownDoesNotMatchRegexMessage =
          StringUtility.EmptyToNull(renderingContext.Control.SearchStringForDropDownDoesNotMatchRegexMessage)
          ?? GetResourceManager(renderingContext)
              .GetText(ResourceIdentifier.SearchStringForDropDownDoesNotMatchRegexMessage)
              .ToString(WebStringEncoding.HtmlWithTransformedLineBreaks);

      var jsonBuilder = new StringBuilder(1000);

      jsonBuilder.Append("{ ");
      jsonBuilder.Append("ValidSearchStringRegex : ");
      AppendStringValueOrNullToScript(jsonBuilder, searchString);
      jsonBuilder.Append(", ");
      jsonBuilder.Append("ValidSearchStringForDropDownRegex : ");
      AppendStringValueOrNullToScript(jsonBuilder, searchStringForDropDown);
      jsonBuilder.Append(", ");
      jsonBuilder.Append("SearchStringForDropDownDoesNotMatchRegexMessage : ");
      AppendStringValueOrNullToScript(jsonBuilder, searchStringForDropDownDoesNotMatchRegexMessage);
      jsonBuilder.Append(", ");
      jsonBuilder.Append("IgnoreSearchStringForDropDownUponValidInput : ");
      AppendBooleanValueToScript(jsonBuilder, renderingContext.Control.IgnoreSearchStringForDropDownUponValidInput);
      jsonBuilder.Append(" }");

      return jsonBuilder.ToString();
    }

    private string GetResourcesAsJson (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      var resourceManager = GetResourceManager(renderingContext);
      var jsonBuilder = new StringBuilder(1000);

      jsonBuilder.Append("{ ");
      jsonBuilder.Append("LoadIconFailedErrorMessage : ");
      AppendStringValueOrNullToScript(jsonBuilder, resourceManager.GetText(ResourceIdentifier.LoadIconFailedErrorMessage));
      jsonBuilder.Append(", ");
      jsonBuilder.Append("LoadDataFailedErrorMessage : ");
      AppendStringValueOrNullToScript(jsonBuilder, resourceManager.GetText(ResourceIdentifier.LoadDataFailedErrorMessage));
      jsonBuilder.Append(", ");
      jsonBuilder.Append("NoDataFoundMessage : ");
      AppendStringValueOrNullToScript(jsonBuilder, resourceManager.GetText(ResourceIdentifier.NoDataFoundMessage));
      jsonBuilder.Append(", ");
      jsonBuilder.Append("LoadingMoreMatchesMessage : ");
      AppendStringValueOrNullToScript(jsonBuilder, resourceManager.GetText(ResourceIdentifier.LoadingMoreMatchesMessage));
      jsonBuilder.Append(", ");
      jsonBuilder.Append("LoadedMoreMatchesMessage : ");
      AppendStringValueOrNullToScript(jsonBuilder, resourceManager.GetText(ResourceIdentifier.LoadedMoreMatchesMessage));
      jsonBuilder.Append(" }");

      return jsonBuilder.ToString();
    }

    protected virtual IResourceManager GetResourceManager (BocRenderingContext<IBocAutoCompleteReferenceValue> renderingContext)
    {
      return GetResourceManager(typeof(ResourceIdentifier), renderingContext.Control.GetResourceManager());
    }

    protected sealed override string GetAriaHasPopupForCombobox ()
    {
      return HtmlAriaHasPopupAttributeValue.Listbox;
    }

    protected sealed override string GetAriaRoleDescriptionForComboboxReadOnly (BocRenderingContext<IBocAutoCompleteReferenceValue> renderingContext)
    {
      var resourceManager = GetResourceManager(renderingContext);
      return resourceManager.GetString(ResourceIdentifier.ScreenReaderLabelForComboboxReadOnly);
    }

    protected sealed override void RenderEditModeValue (BocRenderingContext<IBocAutoCompleteReferenceValue> renderingContext)
    {
      TextBox textBox = GetTextBox(renderingContext);
      var validationErrors = GetValidationErrorsToRender(renderingContext).ToArray();
      var validationErrorsID = GetValidationErrorsID(renderingContext);

      ValidationErrorRenderer.SetValidationErrorsReferenceOnControl(textBox, validationErrorsID, validationErrors);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassInput + " " + CssClassThemed);

      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);

      bool autoPostBack = textBox.AutoPostBack;
      textBox.AutoPostBack = false;
      textBox.RenderControl(renderingContext.Writer);
      textBox.AutoPostBack = autoPostBack;
      renderingContext.Writer.RenderEndTag();

      RenderDropdownButton(renderingContext);

      var hiddenField = GetHiddenField(renderingContext);
      if (autoPostBack)
      {
        PostBackOptions options = new PostBackOptions(textBox, string.Empty);
        if (textBox.CausesValidation)
        {
            options.PerformValidation = true;
            options.ValidationGroup = textBox.ValidationGroup;
        }
        if (renderingContext.Control.Page!.Form != null)
            options.AutoPostBack = true;
        var postBackEventReference = renderingContext.Control.Page.ClientScript.GetPostBackEventReference(options, true);
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Onchange, postBackEventReference);
      }
      hiddenField.RenderControl(renderingContext.Writer);

      ValidationErrorRenderer.RenderValidationErrors(renderingContext.Writer, validationErrorsID, validationErrors);
    }

    private void RenderDropdownButton (BocRenderingContext<IBocAutoCompleteReferenceValue> renderingContext)
    {
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, GetDropDownButtonName(renderingContext));
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassButton);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.AriaHidden, HtmlAriaHiddenAttributeValue.True);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Tabindex, "-1");
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      IconInfo.CreateSpacer(ResourceUrlFactory).Render(renderingContext.Writer, renderingContext.Control);
      renderingContext.Writer.RenderEndTag();
    }

    private HiddenField GetHiddenField (BocRenderingContext<IBocAutoCompleteReferenceValue> renderingContext)
    {
      return new HiddenField
      {
        ClientIDMode = ClientIDMode.Static,
        ID = renderingContext.Control.GetKeyValueName(),
        Page = renderingContext.Control.Page!.WrappedInstance,
        EnableViewState = true,
        Value = renderingContext.Control.BusinessObjectUniqueIdentifier ?? renderingContext.Control.NullValueString
      };
    }

    private TextBox GetTextBox (BocRenderingContext<IBocAutoCompleteReferenceValue> renderingContext)
    {
      var textBox = _textBoxFactory();
      textBox.ClientIDMode = ClientIDMode.Static;
      textBox.ID = renderingContext.Control.GetTextValueName();
      textBox.Text = renderingContext.Control.GetLabelText();
      textBox.Enabled = renderingContext.Control.Enabled;
      textBox.EnableViewState = false;
      textBox.Page = renderingContext.Control.Page!.WrappedInstance;
      textBox.ApplyStyle(renderingContext.Control.CommonStyle);
      renderingContext.Control.TextBoxStyle.ApplyStyle(textBox);

      textBox.Attributes.Add(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Combobox);
      textBox.Attributes.Add(HtmlTextWriterAttribute2.AriaAutoComplete, HtmlAriaAutoCompleteAttributeValue.Both);
      textBox.Attributes.Add(HtmlTextWriterAttribute2.AriaExpanded, HtmlAriaExpandedAttributeValue.False);
      textBox.Attributes.Add(HtmlTextWriterAttribute2.AriaHasPopup, GetAriaHasPopupForCombobox());
      textBox.Attributes.Add(HtmlTextWriterAttribute2.AriaActiveDescendant, "");

      var labelIDs = renderingContext.Control.GetLabelIDs().ToArray();
      LabelReferenceRenderer.SetLabelReferenceOnControl(textBox, labelIDs);

      textBox.Attributes.Add("autocomplete", "off");

      if (renderingContext.Control.IsRequired)
        textBox.Attributes.Add(HtmlTextWriterAttribute2.AriaRequired, HtmlAriaRequiredAttributeValue.True);

      return textBox;
    }

    public override string GetCssClassBase (IBocAutoCompleteReferenceValue control)
    {
      return "bocAutoCompleteReferenceValue";
    }

    private string CssClassButton
    {
      get { return "bocAutoCompleteReferenceValueButton"; }
    }

    private string CssClassInput
    {
      get { return "bocAutoCompleteReferenceValueInput"; }
    }
  }
}
