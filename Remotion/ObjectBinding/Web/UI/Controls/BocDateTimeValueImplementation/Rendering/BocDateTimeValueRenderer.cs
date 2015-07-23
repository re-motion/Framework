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
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Rendering
{
  [ImplementationFor (typeof (IBocDateTimeValueRenderer), Lifetime = LifetimeKind.Singleton)]
  public class BocDateTimeValueRenderer : BocRendererBase<IBocDateTimeValue>, IBocDateTimeValueRenderer
  {
    public enum DateTimeValuePart
    {
      Date,
      Time,
      Picker
    }

    /// <summary> Text displayed when control is displayed in desinger and is read-only has no contents. </summary>
    private const string c_designModeEmptyLabelContents = "##";

    private readonly TextBox _dateTextBox;
    private readonly TextBox _timeTextBox;

    public BocDateTimeValueRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures)
        : this (resourceUrlFactory, globalizationService, renderingFeatures, new RenderOnlyTextBox(), new RenderOnlyTextBox())
    {
    }

    protected BocDateTimeValueRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures,
        TextBox dateTextBox,
        TextBox timeTextBox)
        : base (resourceUrlFactory, globalizationService, renderingFeatures)
    {
      ArgumentUtility.CheckNotNull ("dateTextBox", dateTextBox);
      ArgumentUtility.CheckNotNull ("timeTextBox", timeTextBox);

      _dateTextBox = dateTextBox;
      _timeTextBox = timeTextBox;
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      RegisterBrowserCompatibilityScript (htmlHeadAppender);

      string styleKey = typeof (BocDateTimeValueRenderer).FullName + "_Style";
      var styleFile = ResourceUrlFactory.CreateThemedResourceUrl (typeof (BocDateTimeValueRenderer), ResourceType.Html, "BocDateTimeValue.css");
      htmlHeadAppender.RegisterStylesheetLink (styleKey, styleFile, HtmlHeadAppender.Priority.Library);
    }

    public void Render (BocDateTimeValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      AddAttributesToRender (renderingContext);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      if (renderingContext.Control.IsReadOnly)
        RenderReadOnlyValue (renderingContext);
      else
        RenderEditModeControls (renderingContext);

      renderingContext.Writer.RenderEndTag();
    }

    protected override void AddDiagnosticMetadataAttributes (RenderingContext<IBocDateTimeValue> renderingContext)
    {
      base.AddDiagnosticMetadataAttributes (renderingContext);

      var hasTimeField = renderingContext.Control.ActualValueType != BocDateTimeValueType.Date;
      renderingContext.Writer.AddAttribute (
          DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueHasTimeField,
          hasTimeField.ToString().ToLower());
    }

    private void RenderEditModeControls (BocDateTimeValueRenderingContext renderingContext)
    {
      var formatter = renderingContext.Control.DateTimeFormatter;

      var dateTextBox = _dateTextBox;
      dateTextBox.ID = renderingContext.Control.GetDateValueName();
      dateTextBox.CssClass = CssClassDate;
      Initialize (
          renderingContext,
          dateTextBox,
          renderingContext.Control.DateTextBoxStyle,
          formatter.GetDateMaxLength());
      dateTextBox.Text = renderingContext.Control.DateString;
      dateTextBox.Page = renderingContext.Control.Page.WrappedInstance;

      var timeTextBox = _timeTextBox;
      timeTextBox.ID = renderingContext.Control.GetTimeValueName();
      timeTextBox.CssClass = CssClassTime;
      Initialize (
          renderingContext,
          timeTextBox,
          renderingContext.Control.TimeTextBoxStyle,
          formatter.GetTimeMaxLength (renderingContext.Control.ShowSeconds));
      timeTextBox.Text = renderingContext.Control.TimeString;
      timeTextBox.Page = renderingContext.Control.Page.WrappedInstance;

      var datePickerButton = renderingContext.Control.DatePickerButton;
      datePickerButton.AlternateText = renderingContext.Control.GetDatePickerText();
      datePickerButton.IsDesignMode = renderingContext.Control.IsDesignMode;

      if(IsDiagnosticMetadataRenderingEnabled)
      {
        dateTextBox.Attributes[DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueDateField] = "true";
        dateTextBox.Attributes[DiagnosticMetadataAttributes.TriggersPostBack] = dateTextBox.AutoPostBack.ToString().ToLower();

        timeTextBox.Attributes[DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueTimeField] = "true";
        var showSeconds = renderingContext.Control.ShowSeconds;
        timeTextBox.Attributes[DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueTimeFieldHasSeconds] = showSeconds.ToString().ToLower();
        timeTextBox.Attributes[DiagnosticMetadataAttributes.TriggersPostBack] = timeTextBox.AutoPostBack.ToString().ToLower();
      }

      bool hasDateField = false;
      bool hasTimeField = false;
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

      if (hasDateField)
      {
        renderingContext.Writer.AddAttribute (
            HtmlTextWriterAttribute.Class, CssClassDateInputWrapper + " " + GetPositioningCssClass (renderingContext, DateTimeValuePart.Date));
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
        dateTextBox.RenderControl (renderingContext.Writer);
        renderingContext.Writer.RenderEndTag();

        datePickerButton.CssClass = GetPositioningCssClass (renderingContext, DateTimeValuePart.Picker);
        datePickerButton.RenderControl (renderingContext.Writer);
      }

      if (hasTimeField)
      {
        renderingContext.Writer.AddAttribute (
            HtmlTextWriterAttribute.Class, CssClassTimeInputWrapper + " " + GetPositioningCssClass (renderingContext, DateTimeValuePart.Time));
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
        timeTextBox.RenderControl (renderingContext.Writer);
        renderingContext.Writer.RenderEndTag();
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

      return string.Format (
          "boc{0}{1}Hours{2}",
          part,
          formatter.Is12HourTimeFormat() ? 12 : 24,
          hasTimeFieldWithSeconds ? "WithSeconds" : string.Empty);
    }

    private void Initialize (BocDateTimeValueRenderingContext renderingContext, TextBox textBox, SingleRowTextBoxStyle textBoxStyle, int maxLength)
    {
      textBox.Enabled = renderingContext.Control.Enabled;
      textBox.ReadOnly = !renderingContext.Control.Enabled;
      textBox.Width = Unit.Empty;
      textBox.Height = Unit.Empty;
      textBox.ApplyStyle (renderingContext.Control.CommonStyle);
      renderingContext.Control.DateTimeTextBoxStyle.ApplyStyle (textBox);
      textBoxStyle.ApplyStyle (textBox);

      if (renderingContext.Control.ProvideMaxLength)
        textBox.MaxLength = maxLength;
    }

    private void RenderReadOnlyValue (BocDateTimeValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      var dateLabel = new Label();
      var timeLabel = new Label();

      if (renderingContext.Control.IsDesignMode && !renderingContext.Control.Value.HasValue)
      {
        dateLabel.Text = c_designModeEmptyLabelContents;
        //  Too long, can't resize in designer to less than the content's width
        //  Control.label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
      }
      else
      {
        if (renderingContext.Control.Value.HasValue)
        {
          var formatter = renderingContext.Control.DateTimeFormatter;

          var dateTime = renderingContext.Control.Value.Value;

          dateLabel.Text = formatter.FormatDateValue (dateTime);
          dateLabel.Attributes.Add ("data-value", dateTime.ToString ("yyyy-MM-dd"));

          if (renderingContext.Control.ActualValueType == BocDateTimeValueType.DateTime)
          {
            timeLabel.Text = formatter.FormatTimeValue (dateTime, renderingContext.Control.ShowSeconds);
            timeLabel.Attributes.Add ("data-value", dateTime.ToString ("HH:mm:ss"));
          }
        }
      }

      RenderLabel (dateLabel, renderingContext);
      if (renderingContext.Control.ActualValueType == BocDateTimeValueType.DateTime)
      {
        renderingContext.Writer.Write (' ');
        RenderLabel (timeLabel, renderingContext);
      }
    }

    private static void RenderLabel (Label label, BocDateTimeValueRenderingContext renderingContext)
    {
      label.Height = Unit.Empty;
      label.Width = Unit.Empty;
      label.ApplyStyle (renderingContext.Control.CommonStyle);
      label.ApplyStyle (renderingContext.Control.LabelStyle);

      bool isControlHeightEmpty = renderingContext.Control.Height.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["height"]);
      bool isLabelHeightEmpty = label.Height.IsEmpty && string.IsNullOrEmpty (label.Style["height"]);
      if (!isControlHeightEmpty && isLabelHeightEmpty)
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      bool isControlWidthEmpty = renderingContext.Control.Width.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["width"]);
      bool isLabelWidthEmpty = label.Width.IsEmpty && string.IsNullOrEmpty (label.Style["width"]);
      if (!isControlWidthEmpty && isLabelWidthEmpty)
      {
        if (!renderingContext.Control.Width.IsEmpty)
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, renderingContext.Control.Width.ToString ());
        else
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, renderingContext.Control.Style["width"]);
      }

      label.RenderControl (renderingContext.Writer);
    }
  }
}