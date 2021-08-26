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
using Remotion.FunctionalProgramming;
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
  [ImplementationFor (typeof (IBocEnumValueRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocEnumValueRenderer : BocRendererBase<IBocEnumValue>, IBocEnumValueRenderer
  {
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
        : base (resourceUrlFactory, globalizationService, renderingFeatures)
    {
      ArgumentUtility.CheckNotNull ("internalControlMemberCaller", internalControlMemberCaller);
      ArgumentUtility.CheckNotNull ("labelReferenceRenderer", labelReferenceRenderer);
      ArgumentUtility.CheckNotNull ("validationErrorRenderer", validationErrorRenderer);
      
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
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterCommonStyleSheet();

      string key = typeof (BocEnumValueRenderer).GetFullNameChecked() + "_Style";
      var url = ResourceUrlFactory.CreateThemedResourceUrl (typeof (BocEnumValueRenderer), ResourceType.Html, "BocEnumValue.css");
      htmlHeadAppender.RegisterStylesheetLink (key, url, HtmlHeadAppender.Priority.Library);
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
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      AddAttributesToRender (renderingContext);
      var tag = renderingContext.Control.ListControlStyle.ControlType == ListControlType.RadioButtonList
          ? HtmlTextWriterTag.Div
          : HtmlTextWriterTag.Span;
      renderingContext.Writer.RenderBeginTag (tag);

      var validationErrors = GetValidationErrorsToRender (renderingContext).ToArray();
      var validationErrorsID = GetValidationErrorsID (renderingContext);

      bool isControlHeightEmpty = renderingContext.Control.Height.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["height"]);
      bool isControlWidthEmpty = renderingContext.Control.Width.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["width"]);
      Label label = GetLabel (renderingContext);
      ListControl listControl = GetListControl (renderingContext);
      
      WebControl innerControl = renderingContext.Control.IsReadOnly ? (WebControl) label : listControl;
      innerControl.Page = renderingContext.Control.Page.WrappedInstance;

      bool isInnerControlHeightEmpty = innerControl.Height.IsEmpty && string.IsNullOrEmpty (innerControl.Style["height"]);
      if (!isControlHeightEmpty && isInnerControlHeightEmpty)
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      bool isInnerControlWidthEmpty = innerControl.Width.IsEmpty && string.IsNullOrEmpty (innerControl.Style["width"]);

      if (isInnerControlWidthEmpty)
      {
        if (!isControlWidthEmpty)
        {
          if (renderingContext.Control.IsReadOnly)
          {
            if (!renderingContext.Control.Width.IsEmpty)
              renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, renderingContext.Control.Width.ToString());
            else
              renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, renderingContext.Control.Style["width"]);
          }
        }
      }

      //if (listControl.Enabled && !listControl.IsDisabled & listControl.SupportsDisabledAttribute)
      //  renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
      //if (listControl.SupportsDisabledAttribute && !listControl.Enabled)
      //if (!listControl.Enabled && WebControl.DisabledCssClass.Equals ("aspNetDisabled"))
      //if (innerControl.ClientID.Contains("body_DataEditControl_DisabledUnboundMarriageStatusField") && !listControl.Enabled)
      //if (!listControl.Enabled && WebControl.DisabledCssClass.Equals("aspNetDisabled"))
      //if (!listControl.Enabled)
      //{
      //    listControl.
      //    listControl.Attributes.Add("disabled", "disabled");
      //}


      _validationErrorRenderer.SetValidationErrorsReferenceOnControl (innerControl, validationErrorsID, validationErrors);

      innerControl.RenderControl (renderingContext.Writer);

      _validationErrorRenderer.RenderValidationErrors (renderingContext.Writer, validationErrorsID, validationErrors);

      renderingContext.Writer.RenderEndTag();
    }

    protected override void AddDiagnosticMetadataAttributes (RenderingContext<IBocEnumValue> renderingContext)
    {
      base.AddDiagnosticMetadataAttributes (renderingContext);

      var hasAutoPostBack = renderingContext.Control.ListControlStyle.AutoPostBack.HasValue
                            && renderingContext.Control.ListControlStyle.AutoPostBack.Value;
      renderingContext.Writer.AddAttribute (DiagnosticMetadataAttributes.TriggersPostBack, hasAutoPostBack.ToString().ToLower());

      renderingContext.Writer.AddAttribute (
          DiagnosticMetadataAttributesForObjectBinding.BocEnumValueStyle,
          renderingContext.Control.ListControlStyle.ControlType.ToString());

      renderingContext.Writer.AddAttribute (DiagnosticMetadataAttributesForObjectBinding.NullIdentifier, renderingContext.Control.NullIdentifier);
    }
        
    private ListControl GetListControl (BocEnumValueRenderingContext renderingContext)
    {
      ListControl listControl = renderingContext.Control.ListControlStyle.Create (false);
      listControl.ClientIDMode = ClientIDMode.Static;
      listControl.ID = renderingContext.Control.GetValueName();
      listControl.Enabled = renderingContext.Control.Enabled;

      listControl.Width = Unit.Empty;
      listControl.Height = Unit.Empty;
      listControl.ApplyStyle (renderingContext.Control.CommonStyle);
      renderingContext.Control.ListControlStyle.ApplyStyle (listControl);

      var isRadioButtonList = renderingContext.Control.ListControlStyle.ControlType == ListControlType.RadioButtonList;
      var isDropDownList = renderingContext.Control.ListControlStyle.ControlType == ListControlType.DropDownList;
      var isListBox = renderingContext.Control.ListControlStyle.ControlType == ListControlType.ListBox;

      if (isRadioButtonList)
        listControl.Attributes.Add (HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.RadioGroup);
      
      if (isListBox && !listControl.Enabled)
        listControl.Attributes.Add ("disabled", "disabled");

      var labelIDs = renderingContext.Control.GetLabelIDs().ToArray();
      _labelReferenceRenderer.SetLabelReferenceOnControl (listControl, labelIDs);

      var oneBasedIndex = 1;

      var isNullItemVisible = IsNullItemVisible (renderingContext);
      if (isNullItemVisible)
      {
        var nullItem = CreateNullItem (renderingContext);
        var nullItemText = nullItem.Text;
        if (isDropDownList && !renderingContext.Control.ListControlStyle.DropDownListNullValueTextVisible)
        {
          nullItem.Attributes[HtmlTextWriterAttribute2.AriaLabel] = nullItemText;
          // By setting the label to a single whitespace, we can convince the HTML validator that the element is valid,
          // while preventing text from being displayed in the UI.
          nullItem.Attributes[HtmlTextWriterAttribute2.Label] = " ";
          nullItem.Text = string.Empty;
        }

        if (IsDiagnosticMetadataRenderingEnabled)
        {
          nullItem.Attributes[DiagnosticMetadataAttributes.ItemID] = nullItem.Value;
          nullItem.Attributes[DiagnosticMetadataAttributes.IndexInCollection] = oneBasedIndex.ToString();
          nullItem.Attributes[DiagnosticMetadataAttributes.Content] = HtmlUtility.StripHtmlTags (nullItemText);
        }

        listControl.Items.Add (nullItem);
        oneBasedIndex++;
      }

      IEnumerationValueInfo[] valueInfos = renderingContext.Control.GetEnabledValues();

      for (int i = 0; i < valueInfos.Length; i++, oneBasedIndex++)
      {
        IEnumerationValueInfo valueInfo = valueInfos[i];
        ListItem item = new ListItem (valueInfo.DisplayName, valueInfo.Identifier);
        if (valueInfo.Value.Equals (renderingContext.Control.Value))
          item.Selected = true;

        if (IsDiagnosticMetadataRenderingEnabled)
        {
          item.Attributes[DiagnosticMetadataAttributes.ItemID] = item.Value;
          item.Attributes[DiagnosticMetadataAttributes.IndexInCollection] = oneBasedIndex.ToString();
          item.Attributes[DiagnosticMetadataAttributes.Content] = HtmlUtility.StripHtmlTags (item.Text);
        }

        listControl.Items.Add (item);
      }

      if (renderingContext.Control.IsRequired && !isRadioButtonList)
      {
        listControl.Attributes.Add (HtmlTextWriterAttribute2.AriaRequired, HtmlAriaRequiredAttributeValue.True);
      }
      else if (renderingContext.Control.IsRequired && isRadioButtonList)
      {
        var radioButton = _internalControlMemberCaller.GetControlToRepeat ((RadioButtonList) listControl);
        radioButton.InputAttributes.Add (HtmlTextWriterAttribute2.AriaRequired, HtmlAriaRequiredAttributeValue.True);
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
      ListItem emptyItem = new ListItem (renderingContext.Control.GetNullItemText(), renderingContext.Control.NullIdentifier);
      if (renderingContext.Control.Value == null)
      {
        if (renderingContext.Control.Value == null)
          emptyItem.Selected = true;
      }

      return emptyItem;
    }

    private Label GetLabel (BocEnumValueRenderingContext renderingContext)
    {
      Label label = new Label { ID = renderingContext.Control.GetValueName(), ClientIDMode = ClientIDMode.Static };
      string text;
      if (renderingContext.Control.EnumerationValueInfo == null)
      {
        text = null;
        label.Attributes.Add ("data-value", renderingContext.Control.NullIdentifier);
      }
      else
      {
        text = renderingContext.Control.EnumerationValueInfo.DisplayName;
        label.Attributes.Add ("data-value", renderingContext.Control.EnumerationValueInfo.Identifier);
      }

      label.Text = text;

      label.Width = Unit.Empty;
      label.Height = Unit.Empty;
      label.ApplyStyle (renderingContext.Control.CommonStyle);
      label.ApplyStyle (renderingContext.Control.LabelStyle);

      var labelIDs = renderingContext.Control.GetLabelIDs().ToArray();
      _labelReferenceRenderer.SetLabelsReferenceOnControl (label, labelIDs, new[] { label.ClientID });

      label.Attributes.Add ("tabindex", "0");
      // Screenreaders (JAWS v18) will not read the contents of a span with role=combobox, etc,
      // therefor we have to emulate the reading of the label + contents. Missing from this is "readonly" after the label is read.
      //switch (renderingContext.Control.ListControlStyle.ControlType)
      //{
      //  case ListControlType.DropDownList:
      //    label.Attributes.Add (HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Combobox);
      //    break;
      //  case ListControlType.ListBox:
      //    label.Attributes.Add (HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Listbox);
      //    break;
      //  case ListControlType.RadioButtonList:
      //    label.Attributes.Add (HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Radio);
      //    break;
      //  default:
      //    throw new NotImplementedException();
      //}
      //label.Attributes.Add (HtmlTextWriterAttribute2.AriaReadOnly, HtmlAriaReadOnlyAttributeValue.True);

      return label;
    }

    private IEnumerable<string> GetValidationErrorsToRender (BocRenderingContext<IBocEnumValue> renderingContext)
    {
      return renderingContext.Control.GetValidationErrors();
    }

    private string GetValidationErrorsID (BocRenderingContext<IBocEnumValue> renderingContext)
    {
      return renderingContext.Control.ClientID + "_ValidationErrors";
    }

    public override string GetCssClassBase (IBocEnumValue control)
    {
      return "bocEnumValue";
    }
  }
}
