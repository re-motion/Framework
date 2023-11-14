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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocEnumValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocEnumValue"/> controls.
  /// <seealso cref="IBocEnumValue"/>
  /// </summary>
  [ImplementationFor(typeof(IBocEnumValueRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocEnumValueRenderer : BocRendererBase<IBocEnumValue>, IBocEnumValueRenderer
  {

    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.ObjectBinding.Web.Globalization.BocEnumValueRenderer")]
    public enum ResourceIdentifier
    {
      /// <summary> The aria-role description for the drop-down list as a read-only element. </summary>
      ScreenReaderLabelForComboboxReadOnly,
      /// <summary> The aria-role description for the radio-button as a read-only element. </summary>
      ScreenReaderLabelForRadioButtonReadOnly
    }

    private readonly IInternalControlMemberCaller _internalControlMemberCaller;
    private readonly ILabelReferenceRenderer _labelReferenceRenderer;
    private readonly IValidationErrorRenderer _validationErrorRenderer;

    public BocEnumValueRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures,
        IInternalControlMemberCaller internalControlMemberCaller,
        ILabelReferenceRenderer labelReferenceRenderer,
        IValidationErrorRenderer validationErrorRenderer)
        : base(resourceUrlFactory, globalizationService, renderingFeatures)
    {
      ArgumentUtility.CheckNotNull("internalControlMemberCaller", internalControlMemberCaller);
      ArgumentUtility.CheckNotNull("labelReferenceRenderer", labelReferenceRenderer);
      ArgumentUtility.CheckNotNull("validationErrorRenderer", validationErrorRenderer);

      _internalControlMemberCaller = internalControlMemberCaller;
      _labelReferenceRenderer = labelReferenceRenderer;
      _validationErrorRenderer = validationErrorRenderer;
    }

    /// <inheritdoc />
    protected override bool UseThemingContext
    {
      get { return true; }
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterCommonStyleSheet();

      string key = typeof(BocEnumValueRenderer).GetFullNameChecked() + "_Style";
      var url = ResourceUrlFactory.CreateThemedResourceUrl(typeof(BocEnumValueRenderer), ResourceType.Html, "BocEnumValue.css");
      htmlHeadAppender.RegisterStylesheetLink(key, url, HtmlHeadAppender.Priority.Library);
    }

    /// <summary>
    /// Renders the concrete <see cref="ListControl"/> control as obtained from <see cref="IBocEnumValue.ListControlStyle"/>,
    /// wrapped in a &lt;div&gt;
    /// <seealso cref="ListControlType"/>
    /// </summary>
    /// <remarks>The <see cref="ISmartControl.IsRequired"/> attribute determines if a "null item" is inserted. In addition,
    /// as long as no value has been selected, <see cref="DropDownList"/> and <see cref="ListBox"/> have a "null item" inserted
    /// even when <see cref="ISmartControl.IsRequired"/> is <see langword="true"/>.
    /// </remarks>
    public void Render (BocEnumValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);
      ArgumentUtility.CheckNotNull("renderingContext.Control.Page", renderingContext.Control.Page!);

      AddAttributesToRender(renderingContext);
      var tag = renderingContext.Control.ListControlStyle.ControlType == ListControlType.RadioButtonList
          ? HtmlTextWriterTag.Div
          : HtmlTextWriterTag.Span;
      renderingContext.Writer.RenderBeginTag(tag);

      var validationErrors = GetValidationErrorsToRender(renderingContext).ToArray();
      var validationErrorsID = GetValidationErrorsID(renderingContext);

      var label = GetLabel(renderingContext);
      label.Page = renderingContext.Control.Page.WrappedInstance;
      var dataControl = renderingContext.Control.IsReadOnly ? GetReadOnlyDataControl(renderingContext) : GetListControl(renderingContext);
      dataControl.Page = renderingContext.Control.Page.WrappedInstance;

      _validationErrorRenderer.SetValidationErrorsReferenceOnControl(dataControl, validationErrorsID, validationErrors);

      if (!IsControlHeightEmpty(renderingContext.Control))
      {
        if (IsWebControlHeightEmpty(dataControl))
          dataControl.Style.Add(HtmlTextWriterStyle.Height, "100%");

        if (IsWebControlHeightEmpty(label))
          label.Style.Add(HtmlTextWriterStyle.Height, "100%");
      }

      if (!IsControlWidthEmpty(renderingContext.Control))
      {
        if (IsWebControlWidthEmpty(label))
          label.Style.Add(HtmlTextWriterStyle.Width, renderingContext.Control.Width.ToString()); // TODO: 100% ?
      }

      dataControl.RenderControl(renderingContext.Writer);
      if (renderingContext.Control.IsReadOnly)
        label.RenderControl(renderingContext.Writer);

      _validationErrorRenderer.RenderValidationErrors(renderingContext.Writer, validationErrorsID, validationErrors);

      renderingContext.Writer.RenderEndTag();

      static bool IsControlHeightEmpty (IBocEnumValue control) => control.Height.IsEmpty && string.IsNullOrEmpty(control.Style[HtmlTextWriterStyle.Height]);
      static bool IsControlWidthEmpty (IBocEnumValue control) => control.Width.IsEmpty && string.IsNullOrEmpty(control.Style[HtmlTextWriterStyle.Width]);
      static bool IsWebControlHeightEmpty (WebControl control) => control.Height.IsEmpty && string.IsNullOrEmpty(control.Style[HtmlTextWriterStyle.Height]);
      static bool IsWebControlWidthEmpty (WebControl control) => control.Width.IsEmpty && string.IsNullOrEmpty(control.Style[HtmlTextWriterStyle.Width]);
    }

    protected override void AddDiagnosticMetadataAttributes (RenderingContext<IBocEnumValue> renderingContext)
    {
      base.AddDiagnosticMetadataAttributes(renderingContext);

      var hasAutoPostBack = renderingContext.Control.ListControlStyle.AutoPostBack.HasValue
                            && renderingContext.Control.ListControlStyle.AutoPostBack.Value;
      renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributes.TriggersPostBack, hasAutoPostBack.ToString().ToLower());

      renderingContext.Writer.AddAttribute(
          DiagnosticMetadataAttributesForObjectBinding.BocEnumValueStyle,
          renderingContext.Control.ListControlStyle.ControlType.ToString());

      renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributesForObjectBinding.NullIdentifier, renderingContext.Control.NullIdentifier);
    }

    protected virtual IResourceManager GetResourceManager (BocRenderingContext<IBocEnumValue> renderingContext)
    {
      return GetResourceManager(typeof(ResourceIdentifier), renderingContext.Control.GetResourceManager());
    }

    private ListControl GetListControl (BocEnumValueRenderingContext renderingContext)
    {
      ListControl listControl = renderingContext.Control.ListControlStyle.Create(false);
      listControl.ClientIDMode = ClientIDMode.Static;
      listControl.ID = renderingContext.Control.GetValueName();
      listControl.Enabled = renderingContext.Control.Enabled;

      listControl.Width = Unit.Empty;
      listControl.Height = Unit.Empty;
      listControl.ApplyStyle(renderingContext.Control.CommonStyle);
      renderingContext.Control.ListControlStyle.ApplyStyle(listControl);

      var isRadioButtonList = renderingContext.Control.ListControlStyle.ControlType == ListControlType.RadioButtonList;
      var isDropDownList = renderingContext.Control.ListControlStyle.ControlType == ListControlType.DropDownList;

      if (isRadioButtonList)
        listControl.Attributes.Add(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.RadioGroup);

      var labelIDs = renderingContext.Control.GetLabelIDs().ToArray();
      _labelReferenceRenderer.SetLabelReferenceOnControl(listControl, labelIDs);

      var oneBasedIndex = 1;

      var isNullItemVisible = IsNullItemVisible(renderingContext);
      if (isNullItemVisible)
      {
        var nullItem = CreateNullItem(renderingContext);
        var nullItemText = nullItem.Text;
        if (isDropDownList && !renderingContext.Control.ListControlStyle.DropDownListNullValueTextVisible)
        {
          nullItem.Attributes[HtmlTextWriterAttribute2.AriaLabel] = nullItemText;
          char nbsp = (char)160;
          // By setting the label to a single whitespace, we can convince the HTML validator that the element is valid,
          // while preventing text from being displayed in the UI. the NBSP is required in Firefox to prevent the dropdown icon from minimzing.
          nullItem.Attributes[HtmlTextWriterAttribute2.Label] = new string(nbsp, 1);
          nullItem.Text = string.Empty;
        }

        if (IsDiagnosticMetadataRenderingEnabled)
        {
          nullItem.Attributes[DiagnosticMetadataAttributes.ItemID] = nullItem.Value;
          nullItem.Attributes[DiagnosticMetadataAttributes.IndexInCollection] = oneBasedIndex.ToString();
          nullItem.Attributes[DiagnosticMetadataAttributes.Content] = HtmlUtility.ExtractPlainText(PlainTextString.CreateFromText(nullItemText)).GetValue();
        }

        listControl.Items.Add(nullItem);
        oneBasedIndex++;
      }

      IEnumerationValueInfo[] valueInfos = renderingContext.Control.GetEnabledValues();

      for (int i = 0; i < valueInfos.Length; i++, oneBasedIndex++)
      {
        IEnumerationValueInfo valueInfo = valueInfos[i];
        ListItem item = new ListItem(valueInfo.DisplayName, valueInfo.Identifier);
        if (valueInfo.Value.Equals(renderingContext.Control.Value))
          item.Selected = true;

        if (IsDiagnosticMetadataRenderingEnabled)
        {
          item.Attributes[DiagnosticMetadataAttributes.ItemID] = item.Value;
          item.Attributes[DiagnosticMetadataAttributes.IndexInCollection] = oneBasedIndex.ToString();
          item.Attributes[DiagnosticMetadataAttributes.Content] = HtmlUtility.ExtractPlainText(PlainTextString.CreateFromText(item.Text)).GetValue();
        }

        listControl.Items.Add(item);
      }

      if (renderingContext.Control.IsRequired && !isRadioButtonList)
      {
        listControl.Attributes.Add(HtmlTextWriterAttribute2.AriaRequired, HtmlAriaRequiredAttributeValue.True);
      }
      else if (renderingContext.Control.IsRequired && isRadioButtonList)
      {
        var radioButton = _internalControlMemberCaller.GetControlToRepeat((RadioButtonList)listControl);
        radioButton.InputAttributes.Add(HtmlTextWriterAttribute2.AriaRequired, HtmlAriaRequiredAttributeValue.True);
      }

      return listControl;
    }

    private bool IsNullItemVisible (BocEnumValueRenderingContext renderingContext)
    {
      var isRequired = renderingContext.Control.IsRequired;
      var isNullValueSelected = renderingContext.Control.Value == null;
      var isRadioButtonList = renderingContext.Control.ListControlStyle.ControlType == ListControlType.RadioButtonList;

      if (isRadioButtonList)
      {
        if (isRequired)
          return false;

        if (!renderingContext.Control.ListControlStyle.RadioButtonListNullValueVisible)
          return false;

        return true;
      }
      else
      {
        if (isRequired)
          return isNullValueSelected;

        return true;
      }
    }

    /// <summary> Creates the <see cref="ListItem"/> symbolizing the undefined selection. </summary>
    /// <returns> A <see cref="ListItem"/>. </returns>
    private ListItem CreateNullItem (BocEnumValueRenderingContext renderingContext)
    {
      ListItem emptyItem = new ListItem(
          renderingContext.Control.GetNullItemText().GetValue(),
          renderingContext.Control.NullIdentifier);

      if (renderingContext.Control.Value == null)
      {
        if (renderingContext.Control.Value == null)
          emptyItem.Selected = true;
      }

      return emptyItem;
    }

    private WebControl GetReadOnlyDataControl (BocEnumValueRenderingContext renderingContext)
    {
      var resourceManager = GetResourceManager(renderingContext);
      var labelIDs = renderingContext.Control.GetLabelIDs().ToArray();

      var dataText = renderingContext.Control.EnumerationValueInfo == null
          ? renderingContext.Control.GetNullItemText()
          : PlainTextString.CreateFromText(renderingContext.Control.EnumerationValueInfo.DisplayName);

      var dataValue = renderingContext.Control.EnumerationValueInfo == null
          ? renderingContext.Control.NullIdentifier
          : renderingContext.Control.EnumerationValueInfo.Identifier;

      var textControlID = renderingContext.Control.ClientID + "_TextValue";

      if (renderingContext.Control.ListControlStyle.ControlType == ListControlType.RadioButtonList)
      {
        var radioButtonList = new Label() { ClientIDMode = ClientIDMode.Static };
        radioButtonList.ID = textControlID;
        radioButtonList.EnableViewState = false;
        radioButtonList.CssClass = CssClassDefinition.ScreenReaderText;
        radioButtonList.Attributes.Add(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.RadioGroup);
        _labelReferenceRenderer.SetLabelReferenceOnControl(radioButtonList, labelIDs);

        var radioButton = new Label() { ClientIDMode = ClientIDMode.Static };
        radioButton.Attributes.Add("data-value", dataValue);
        if (renderingContext.Control.IsRequired)
          radioButton.Attributes.Add(HtmlTextWriterAttribute2.AriaRequired, HtmlAriaRequiredAttributeValue.True);
        radioButton.Attributes.Add(HtmlTextWriterAttribute2.Tabindex, "0");
        radioButton.Attributes.Add(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Radio);
        radioButton.Attributes.Add(HtmlTextWriterAttribute2.AriaChecked, HtmlAriaCheckedAttributeValue.True);
        radioButton.Attributes.Add(HtmlTextWriterAttribute2.AriaRoleDescription, resourceManager.GetString(ResourceIdentifier.ScreenReaderLabelForRadioButtonReadOnly));
        radioButton.Attributes.Add(HtmlTextWriterAttribute2.AriaLabel, dataText.GetValue());

        radioButtonList.Controls.Add(radioButton);

        return radioButtonList;
      }
      else
      {
        var textBox = new RenderOnlyTextBox() { ClientIDMode = ClientIDMode.Static };
        textBox.ID = textControlID;
        textBox.EnableViewState = false;
        textBox.CssClass = CssClassDefinition.ScreenReaderText;
        textBox.Attributes.Add("data-value", dataValue);
        if (renderingContext.Control.IsRequired)
          textBox.Attributes.Add(HtmlTextWriterAttribute2.AriaRequired, HtmlAriaRequiredAttributeValue.True);
        textBox.Enabled = renderingContext.Control.Enabled;
        textBox.ReadOnly = true;
        textBox.Attributes.Add(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Combobox);
        textBox.Attributes.Add(HtmlTextWriterAttribute2.AriaExpanded, HtmlAriaExpandedAttributeValue.False);
        textBox.Attributes.Add(HtmlTextWriterAttribute2.AriaHasPopup, HtmlAriaHasPopupAttributeValue.Menu);
        textBox.Attributes.Add(HtmlTextWriterAttribute2.AriaRoleDescription, resourceManager.GetString(ResourceIdentifier.ScreenReaderLabelForComboboxReadOnly));
        textBox.Text = dataText.GetValue();
        _labelReferenceRenderer.SetLabelReferenceOnControl(textBox, labelIDs);

        return textBox;
      }

    }

    private Label GetLabel (BocEnumValueRenderingContext renderingContext)
    {
      PlainTextString text;
      if (renderingContext.Control.EnumerationValueInfo == null)
        text = PlainTextString.Empty;
      else
        text = PlainTextString.CreateFromText(renderingContext.Control.EnumerationValueInfo.DisplayName);

      var label = new Label { ClientIDMode = ClientIDMode.Static };
      label.EnableViewState = false;
      label.Height = Unit.Empty;
      label.Width = Unit.Empty;
      label.ApplyStyle(renderingContext.Control.CommonStyle);
      label.ApplyStyle(renderingContext.Control.LabelStyle);
      label.Attributes.Add(HtmlTextWriterAttribute2.AriaHidden, HtmlAriaHiddenAttributeValue.True);
      label.Text = text.ToString(WebStringEncoding.Html);

      return label;
    }

    private IEnumerable<PlainTextString> GetValidationErrorsToRender (BocRenderingContext<IBocEnumValue> renderingContext)
    {
      return renderingContext.Control.GetValidationErrors();
    }

    private string GetValidationErrorsID (BocRenderingContext<IBocEnumValue> renderingContext)
    {
      return renderingContext.Control.ClientID + "_ValidationErrors";
    }

    public override string GetCssClassBase (IBocEnumValue control)
    {
      const string cssClassBase = "bocEnumValue";
      if (control.IsReadOnly)
        return cssClassBase;

      switch (control.ListControlStyle.ControlType)
      {
        case ListControlType.DropDownList:
          return cssClassBase + " dropDownList";
        case ListControlType.ListBox:
          return cssClassBase + " listBox";
        case ListControlType.RadioButtonList:
          return cssClassBase + " radioButtonList";
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}
