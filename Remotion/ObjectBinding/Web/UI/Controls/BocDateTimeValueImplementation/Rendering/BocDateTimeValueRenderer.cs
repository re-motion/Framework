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
using Remotion.Web.Globalization;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Rendering
{
  [ImplementationFor(typeof(IBocDateTimeValueRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocDateTimeValueRenderer : BocRendererBase<IBocDateTimeValue>, IBocDateTimeValueRenderer
  {
    public enum DateTimeValuePart
    {
      Date,
      Time,
      Picker
    }

    private readonly ILabelReferenceRenderer _labelReferenceRenderer;
    private readonly IValidationErrorRenderer _validationErrorRenderer;

    private readonly Func<TextBox> _dateTextBoxFactory;
    private readonly Func<TextBox> _timeTextBoxFactory;

    public BocDateTimeValueRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures,
        ILabelReferenceRenderer labelReferenceRenderer,
        IValidationErrorRenderer validationErrorRenderer)
        : this(
            resourceUrlFactory,
            globalizationService,
            renderingFeatures,
            labelReferenceRenderer,
            validationErrorRenderer,
            () => new RenderOnlyTextBox(),
            () => new RenderOnlyTextBox())
    {
    }

    protected BocDateTimeValueRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures,
        ILabelReferenceRenderer labelReferenceRenderer,
        IValidationErrorRenderer validationErrorRenderer,
        Func<TextBox> dateTextBoxFactory,
        Func<TextBox> timeTextBoxFactory)
        : base(resourceUrlFactory, globalizationService, renderingFeatures)
    {
      ArgumentUtility.CheckNotNull("labelReferenceRenderer", labelReferenceRenderer);
      ArgumentUtility.CheckNotNull("validationErrorRenderer", validationErrorRenderer);
      ArgumentUtility.CheckNotNull("dateTextBoxFactory", dateTextBoxFactory);
      ArgumentUtility.CheckNotNull("timeTextBoxFactory", timeTextBoxFactory);

      _labelReferenceRenderer = labelReferenceRenderer;
      _validationErrorRenderer = validationErrorRenderer;
      _dateTextBoxFactory = dateTextBoxFactory;
      _timeTextBoxFactory = timeTextBoxFactory;

    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterCommonStyleSheet();

      string styleKey = typeof(BocDateTimeValueRenderer).GetFullNameChecked() + "_Style";
      var styleFile = ResourceUrlFactory.CreateThemedResourceUrl(typeof(BocDateTimeValueRenderer), ResourceType.Html, "BocDateTimeValue.css");
      htmlHeadAppender.RegisterStylesheetLink(styleKey, styleFile, HtmlHeadAppender.Priority.Library);
    }

    public void Render (BocDateTimeValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      AddAttributesToRender(renderingContext);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);

      var formatter = renderingContext.Control.DateTimeFormatter;
      var resourceManager = renderingContext.Control.GetResourceManager();

      var dateTextBoxLabelID = renderingContext.Control.ClientID + "_DateLabel";
      var timeTextBoxLabelID = renderingContext.Control.ClientID + "_TimeLabel";

      var dateValueValidationErrors = GetDateValueValidationErrorsToRender(renderingContext).ToArray();
      var timeValueValidationErrors = GetTimeValueValidationErrorsToRender(renderingContext).ToArray();

      var dateValueValidationErrorsID = GetDateValueValidationErrorsID(renderingContext);
      var timeValueValidationErrorsID = GetTimeValueValidationErrorsID(renderingContext);

      bool hasDateField;
      bool hasTimeField;
      EvaluateActualValueType(renderingContext, out hasDateField, out hasTimeField);

      var dateTextBox = _dateTextBoxFactory();
      dateTextBox.ID = renderingContext.Control.GetDateValueName();
      dateTextBox.CssClass = CssClassDate;
      InitializeTextBox(
          renderingContext,
          dateTextBox,
          renderingContext.Control.DateTextBoxStyle,
          formatter.GetDateMaxLength());
      dateTextBox.Text = renderingContext.Control.DateString;
      dateTextBox.Page = renderingContext.Control.Page!.WrappedInstance;

      var controlDateLabelIDs = renderingContext.Control.GetLabelIDs().ToArray();
      var accessibilityAnnotationIDsCollectionForDateTextBox = new List<string>();

      if (hasTimeField)
        accessibilityAnnotationIDsCollectionForDateTextBox.Add(dateTextBoxLabelID);

      _labelReferenceRenderer.SetLabelsReferenceOnControl(
          dateTextBox,
          controlDateLabelIDs,
          accessibilityAnnotationIDsCollectionForDateTextBox);

      if (renderingContext.Control.IsRequired)
        dateTextBox.Attributes.Add(HtmlTextWriterAttribute2.AriaRequired, HtmlAriaRequiredAttributeValue.True);

      var timeTextBox = _timeTextBoxFactory();
      timeTextBox.ID = renderingContext.Control.GetTimeValueName();
      timeTextBox.CssClass = CssClassTime;
      InitializeTextBox(
          renderingContext,
          timeTextBox,
          renderingContext.Control.TimeTextBoxStyle,
          formatter.GetTimeMaxLength(renderingContext.Control.ShowSeconds));
      timeTextBox.Text = renderingContext.Control.TimeString;
      timeTextBox.Page = renderingContext.Control.Page.WrappedInstance;

      var controlTimeLabelIDs = renderingContext.Control.GetLabelIDs().ToArray();
      var accessibilityAnnotationIDsCollectionForTimeTextBox = new List<string>();

      if (hasDateField)
        accessibilityAnnotationIDsCollectionForTimeTextBox.Add(timeTextBoxLabelID);

      _labelReferenceRenderer.SetLabelsReferenceOnControl(
          timeTextBox,
          controlTimeLabelIDs,
          accessibilityAnnotationIDsCollectionForTimeTextBox);

      if (renderingContext.Control.IsRequired)
        timeTextBox.Attributes.Add(HtmlTextWriterAttribute2.AriaRequired, HtmlAriaRequiredAttributeValue.True);

      var datePickerButton = renderingContext.Control.DatePickerButton;
      datePickerButton.AlternateText = renderingContext.Control.GetDatePickerText();

      if(IsDiagnosticMetadataRenderingEnabled)
      {
        dateTextBox.Attributes[DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueDateField] = "true";
        dateTextBox.Attributes[DiagnosticMetadataAttributes.TriggersPostBack] = dateTextBox.AutoPostBack.ToString().ToLower();

        timeTextBox.Attributes[DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueTimeField] = "true";
        var showSeconds = renderingContext.Control.ShowSeconds;
        timeTextBox.Attributes[DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueTimeFieldHasSeconds] = showSeconds.ToString().ToLower();
        timeTextBox.Attributes[DiagnosticMetadataAttributes.TriggersPostBack] = timeTextBox.AutoPostBack.ToString().ToLower();
      }

      if (hasDateField)
      {
        renderingContext.Writer.AddAttribute(
            HtmlTextWriterAttribute.Class, CssClassDateInputWrapper + " " + CssClassThemed + " " + GetPositioningCssClass(renderingContext, DateTimeValuePart.Date));
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);

        _validationErrorRenderer.SetValidationErrorsReferenceOnControl(dateTextBox, dateValueValidationErrorsID, dateValueValidationErrors);

        dateTextBox.RenderControl(renderingContext.Writer);
        if (renderingContext.Control.IsReadOnly)
          RenderReadOnlyValueLabel(renderingContext.Control.DateString, renderingContext);

        if (hasTimeField)
        {
          renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, dateTextBoxLabelID);
          renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Hidden, HtmlHiddenAttributeValue.Hidden);
          renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
          resourceManager.GetText(BocDateTimeValue.ResourceIdentifier.DateLabelText).WriteTo(renderingContext.Writer);
          renderingContext.Writer.RenderEndTag();
        }

        _validationErrorRenderer.RenderValidationErrors(renderingContext.Writer, dateValueValidationErrorsID, dateValueValidationErrors);

        renderingContext.Writer.RenderEndTag();

        if (!renderingContext.Control.IsReadOnly)
        {
          datePickerButton.CssClass = GetPositioningCssClass(renderingContext, DateTimeValuePart.Picker);
          datePickerButton.RenderControl(renderingContext.Writer);
        }

        if (renderingContext.Control.IsReadOnly)
          renderingContext.Writer.Write(' ');
      }

      if (hasTimeField)
      {
        renderingContext.Writer.AddAttribute(
            HtmlTextWriterAttribute.Class, CssClassTimeInputWrapper + " " + CssClassThemed + " " + GetPositioningCssClass(renderingContext, DateTimeValuePart.Time));
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);

        _validationErrorRenderer.SetValidationErrorsReferenceOnControl(timeTextBox, timeValueValidationErrorsID, timeValueValidationErrors);

        timeTextBox.RenderControl(renderingContext.Writer);
        if (renderingContext.Control.IsReadOnly)
          RenderReadOnlyValueLabel(renderingContext.Control.TimeString, renderingContext);

        if (hasDateField)
        {
          renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, timeTextBoxLabelID);
          renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Hidden, HtmlHiddenAttributeValue.Hidden);
          renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
          resourceManager.GetText(BocDateTimeValue.ResourceIdentifier.TimeLabelText).WriteTo(renderingContext.Writer);
          renderingContext.Writer.RenderEndTag();
        }

        _validationErrorRenderer.RenderValidationErrors(renderingContext.Writer, timeValueValidationErrorsID, timeValueValidationErrors);

        renderingContext.Writer.RenderEndTag();
      }

      renderingContext.Writer.RenderEndTag();
    }

    protected override void AddDiagnosticMetadataAttributes (RenderingContext<IBocDateTimeValue> renderingContext)
    {
      base.AddDiagnosticMetadataAttributes(renderingContext);

      var hasTimeField = renderingContext.Control.ActualValueType != BocDateTimeValueType.Date;
      renderingContext.Writer.AddAttribute(
          DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueHasTimeField,
          hasTimeField.ToString().ToLower());
    }

    private void EvaluateActualValueType (BocDateTimeValueRenderingContext renderingContext, out bool hasDateField, out bool hasTimeField)
    {
      hasDateField = false;
      hasTimeField = false;
      switch (renderingContext.Control.ActualValueType)
      {
        case BocDateTimeValueType.Date:
          hasDateField = true;
          break;
        case BocDateTimeValueType.DateTime:
        case BocDateTimeValueType.Undefined:
          hasDateField = true;
          hasTimeField = true;
          break;
      }
    }

    public override string GetCssClassBase (IBocDateTimeValue control)
    {
      switch (control.ActualValueType)
      {
        case BocDateTimeValueType.DateTime:
          return CssClassDateTime;
        case BocDateTimeValueType.Date:
          return CssClassDateOnly;
        default:
          return CssClassDateTime;
      }
    }

    public string CssClassDateTime
    {
      get { return "bocDateTimeValue"; }
    }

    public string CssClassDateOnly
    {
      get { return "bocDateValue"; }
    }

    public string CssClassDateInputWrapper
    {
      get { return "bocDateInputWrapper"; }
    }

    public string CssClassTimeInputWrapper
    {
      get { return "bocTimeInputWrapper"; }
    }

    public string CssClassDate
    {
      get { return "bocDateTimeDate"; }
    }

    public string CssClassTime
    {
      get { return "bocDateTimeTime"; }
    }

    public string GetPositioningCssClass (BocDateTimeValueRenderingContext renderingContext, DateTimeValuePart part)
    {
      var hasTimeFieldWithSeconds = renderingContext.Control.ShowSeconds && renderingContext.Control.ActualValueType != BocDateTimeValueType.Date;
      var formatter = renderingContext.Control.DateTimeFormatter;

      return string.Format(
          "boc{0}{1}Hours{2}",
          part,
          formatter.Is12HourTimeFormat() ? 12 : 24,
          hasTimeFieldWithSeconds ? "WithSeconds" : string.Empty);
    }

    private void InitializeTextBox (BocDateTimeValueRenderingContext renderingContext, TextBox textBox, SingleRowTextBoxStyle textBoxStyle, int maxLength)
    {
      textBox.Enabled = renderingContext.Control.Enabled;
      textBox.ReadOnly = renderingContext.Control.IsReadOnly || !renderingContext.Control.Enabled;
      textBox.Width = Unit.Empty;
      textBox.Height = Unit.Empty;
      textBox.ApplyStyle(renderingContext.Control.CommonStyle);
      renderingContext.Control.DateTimeTextBoxStyle.ApplyStyle(textBox);
      textBoxStyle.ApplyStyle(textBox);

      if (renderingContext.Control.ProvideMaxLength)
        textBox.MaxLength = maxLength;

      if (renderingContext.Control.IsReadOnly)
        textBox.CssClass = CssClassDefinition.ScreenReaderText;
    }

    private void RenderReadOnlyValueLabel (string? text, BocDateTimeValueRenderingContext renderingContext)
    {
      var label = new Label { ClientIDMode = ClientIDMode.Static };
      label.Text = text;
      label.Attributes.Add(HtmlTextWriterAttribute2.AriaHidden, HtmlAriaHiddenAttributeValue.True);

      label.Height = Unit.Empty;
      label.Width = Unit.Empty;
      label.ApplyStyle(renderingContext.Control.CommonStyle);
      label.ApplyStyle(renderingContext.Control.LabelStyle);

      bool isControlHeightEmpty = renderingContext.Control.Height.IsEmpty && string.IsNullOrEmpty(renderingContext.Control.Style[HtmlTextWriterStyle.Height]);
      bool isLabelHeightEmpty = label.Height.IsEmpty && string.IsNullOrEmpty(label.Style[HtmlTextWriterStyle.Height]);
      if (!isControlHeightEmpty && isLabelHeightEmpty)
        label.Style.Add(HtmlTextWriterStyle.Height, "100%");

      label.RenderControl(renderingContext.Writer);
    }

    private IEnumerable<PlainTextString> GetDateValueValidationErrorsToRender (BocRenderingContext<IBocDateTimeValue> renderingContext)
    {
      return renderingContext.Control.GetDateValueValidationErrors();
    }

    private string GetDateValueValidationErrorsID (BocRenderingContext<IBocDateTimeValue> renderingContext)
    {
      return renderingContext.Control.ClientID + "_DateValueValidationErrors";
    }

    private IEnumerable<PlainTextString> GetTimeValueValidationErrorsToRender (BocRenderingContext<IBocDateTimeValue> renderingContext)
    {
      return renderingContext.Control.GetTimeValueValidationErrors();
    }

    private string GetTimeValueValidationErrorsID (BocRenderingContext<IBocDateTimeValue> renderingContext)
    {
      return renderingContext.Control.ClientID + "_TimeValueValidationErrors";
    }
  }
}
