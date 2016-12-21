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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Rendering;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation.Rendering;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocDateTimeValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocDateTimeValue"/> controls, but not for the included <see cref="IDatePickerButton"/>.
  /// For that, see <see cref="DatePickerButtonRenderer"/>.
  /// <seealso cref="IBocDateTimeValue"/>
  /// </summary>
  /// <include file='..\..\..\..\doc\include\UI\Controls\BocDateTimeValueRenderer.xml' path='BocDateTimeValueRenderer/Class/*'/>
  public class BocDateTimeValueQuirksModeRenderer : BocQuirksModeRendererBase<IBocDateTimeValue>, IBocDateTimeValueRenderer
  {
    /// <summary> Text displayed when control is displayed in desinger and is read-only has no contents. </summary>
    private const string c_designModeEmptyLabelContents = "##";
    private const string c_defaultControlWidth = "150pt";
    private readonly IClientScriptBehavior _clientScriptBehavior;

    public BocDateTimeValueQuirksModeRenderer (IClientScriptBehavior clientScriptBehavior, IResourceUrlFactory resourceUrlFactory)
      : base (resourceUrlFactory)
    {
      ArgumentUtility.CheckNotNull ("clientScriptBehavior", clientScriptBehavior);

      _clientScriptBehavior = clientScriptBehavior;
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);
      
      string styleKey = typeof (BocDateTimeValueQuirksModeRenderer).FullName + "_Style";
      var styleFile = ResourceUrlFactory.CreateResourceUrl (typeof (BocDateTimeValueQuirksModeRenderer), ResourceType.Html, "BocDateTimeValue.css");
      htmlHeadAppender.RegisterStylesheetLink (styleKey, styleFile, HtmlHeadAppender.Priority.Library);
    }

    /// <summary>
    /// Renders an inline table consisting of one row with up to three cells, depending on <see cref="IBocDateTimeValue.ActualValueType"/>.
    /// The first one for the date textbox, second for the <see cref="DatePickerButton"/> and third for the time textbox.
    /// The text boxes are rendered directly, the date picker is responsible for rendering itself.
    /// </summary>
    public void Render (BocDateTimeValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      AddAttributesToRender (renderingContext, true);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      if (renderingContext.Control.IsReadOnly)
        RenderReadOnlyValue (renderingContext);
      else
        RenderEditModeControls (renderingContext);

      renderingContext.Writer.RenderEndTag ();
    }

    private void RenderEditModeControls (BocDateTimeValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      var formatter = renderingContext.Control.DateTimeFormatter;
      var dateTextBox = new RenderOnlyTextBox { ID = renderingContext.Control.GetDateValueName(), ClientIDMode = ClientIDMode.Static };
      Initialize (renderingContext, dateTextBox, renderingContext.Control.DateTextBoxStyle, formatter.GetDateMaxLength ());
      dateTextBox.Text = renderingContext.Control.Value.HasValue ? formatter.FormatDateValue (renderingContext.Control.Value.Value) : renderingContext.Control.DateString;
      dateTextBox.Page = renderingContext.Control.Page.WrappedInstance;

      var timeTextBox = new RenderOnlyTextBox { ID = renderingContext.Control.GetTimeValueName(), ClientIDMode = ClientIDMode.Static };
      var showSeconds = renderingContext.Control.ShowSeconds;
      Initialize (renderingContext, timeTextBox, renderingContext.Control.TimeTextBoxStyle, formatter.GetTimeMaxLength (showSeconds));
      timeTextBox.Text = renderingContext.Control.Value.HasValue ? formatter.FormatTimeValue (renderingContext.Control.Value.Value, showSeconds) : renderingContext.Control.TimeString;
      timeTextBox.Page = renderingContext.Control.Page.WrappedInstance;

      var datePickerButton = renderingContext.Control.DatePickerButton;
      datePickerButton.AlternateText = renderingContext.Control.GetDatePickerText ();
      datePickerButton.IsDesignMode = renderingContext.Control.IsDesignMode;

      RenderTableBeginTag (renderingContext, dateTextBox, timeTextBox); // Begin table
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr); // Begin tr

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
      bool canScript = DetermineClientScriptLevel(datePickerButton, renderingContext);
      bool hasDatePicker = hasDateField && canScript;

      int dateTextBoxWidthPercentage = GetDateTextBoxWidthPercentage (renderingContext, hasDateField, hasTimeField);
      string dateTextBoxSize = GetDateTextBoxSize (renderingContext, dateTextBoxWidthPercentage);
      string timeTextBoxSize = GetTimeTextBoxSize (renderingContext, 100 - dateTextBoxWidthPercentage);

      RenderDateCell (renderingContext, hasDateField, dateTextBox, dateTextBoxSize);
      RenderDatePickerCell (renderingContext, hasDatePicker, datePickerButton);

      //HACK: Opera has problems with inline tables and may collapse contents unless a cell with width 0% is present
      InsertDummyCellForOpera (renderingContext, hasDatePicker);

      RenderTimeCell (renderingContext, hasDateField, hasTimeField, timeTextBox, timeTextBoxSize);

      renderingContext.Writer.RenderEndTag (); // End tr
      renderingContext.Writer.RenderEndTag (); // End table
    }

    private void RenderTimeCell (BocDateTimeValueRenderingContext renderingContext, bool hasDateField, bool hasTimeField, TextBox timeTextBox, string timeTextBoxSize)
    {
      if (!hasTimeField)
        return;

      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, timeTextBoxSize);

      if (hasDateField)
        renderingContext.Writer.AddStyleAttribute ("padding-left", "0.3em");

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

      if (!IsControlHeightEmpty (renderingContext.Control) && IsControlHeightEmpty (timeTextBox))
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      timeTextBox.RenderControl (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag (); // End td
    }

    private void RenderDatePickerCell (BocDateTimeValueRenderingContext renderingContext, bool hasDatePicker, IDatePickerButton datePickerButton)
    {
      if (!hasDatePicker)
        return;

      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      renderingContext.Writer.AddStyleAttribute ("padding-left", "0.3em");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td
      datePickerButton.RenderControl (renderingContext.Writer);
      renderingContext.Writer.RenderEndTag (); // End td
    }

    private void InsertDummyCellForOpera (BocDateTimeValueRenderingContext renderingContext, bool hasDatePicker)
    {
      if (hasDatePicker || renderingContext.HttpContext.Request.Browser.Browser != "Opera")
        return;

      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td
      renderingContext.Writer.Write ("&nbsp;");
      renderingContext.Writer.RenderEndTag (); // End td
    }

    private void RenderDateCell (BocDateTimeValueRenderingContext renderingContext, bool hasDateField, TextBox dateTextBox, string dateTextBoxSize)
    {
      if (!hasDateField)
        return;

      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, dateTextBoxSize);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

      if (!IsControlHeightEmpty (renderingContext.Control) && IsControlHeightEmpty (dateTextBox))
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      dateTextBox.RenderControl (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag (); // End td
    }

    private void RenderTableBeginTag (BocDateTimeValueRenderingContext renderingContext, TextBox dateTextBox, TextBox timeTextBox)
    {
      if (!IsControlHeightEmpty (renderingContext.Control) && IsControlHeightEmpty (dateTextBox) && IsControlHeightEmpty (timeTextBox))
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

      if (IsControlWidthEmpty (dateTextBox) && IsControlWidthEmpty (timeTextBox))
      {
        if (IsControlWidthEmpty (renderingContext.Control))
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
        else
        {
          if (!renderingContext.Control.Width.IsEmpty)
            renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, renderingContext.Control.Width.ToString ());
          else
            renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, renderingContext.Control.Style["width"]);
        }
      }

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      renderingContext.Writer.AddStyleAttribute ("display", "inline");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Table); // Begin table
    }

    private bool IsControlWidthEmpty (WebControl control)
    {
      return control.Width.IsEmpty && string.IsNullOrEmpty (control.Style["width"]);
    }

    private bool IsControlWidthEmpty (IBocDateTimeValue control)
    {
      return control.Width.IsEmpty && string.IsNullOrEmpty (control.Style["width"]);
    }

    private bool IsControlHeightEmpty (WebControl control)
    {
      return control.Height.IsEmpty && string.IsNullOrEmpty (control.Style["height"]);
    }

    private bool IsControlHeightEmpty (IBocDateTimeValue control)
    {
      return control.Height.IsEmpty && string.IsNullOrEmpty (control.Style["height"]);
    }

    protected override void AddAdditionalAttributes (RenderingContext<IBocDateTimeValue> renderingContext)
    {
      base.AddAdditionalAttributes (renderingContext);
      renderingContext.Writer.AddStyleAttribute ("display", "inline");
    }

    private bool DetermineClientScriptLevel (IDatePickerButton datePickerButton, BocDateTimeValueRenderingContext renderingContext)
    {
      if (!datePickerButton.EnableClientScript)
        return false;

      return _clientScriptBehavior.IsBrowserCapableOfScripting(renderingContext.HttpContext, renderingContext.Control); 
    }

    public override string CssClassBase
    {
      get { return "bocDateTimeValue"; }
    }

    private string GetDateTextBoxSize (BocDateTimeValueRenderingContext renderingContext, int dateTextBoxWidthPercentage)
    {
      string dateTextBoxSize;
      if (!renderingContext.Control.DateTextBoxStyle.Width.IsEmpty)
        dateTextBoxSize = renderingContext.Control.DateTextBoxStyle.Width.ToString ();
      else
        dateTextBoxSize = dateTextBoxWidthPercentage + "%";
      return dateTextBoxSize;
    }

    private string GetTimeTextBoxSize (BocDateTimeValueRenderingContext renderingContext, int timeTextBoxWidthPercentage)
    {
      string timeTextBoxSize;
      if (!renderingContext.Control.TimeTextBoxStyle.Width.IsEmpty)
        timeTextBoxSize = renderingContext.Control.TimeTextBoxStyle.Width.ToString ();
      else
        timeTextBoxSize = timeTextBoxWidthPercentage + "%";
      return timeTextBoxSize;
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

    private int GetDateTextBoxWidthPercentage (BocDateTimeValueRenderingContext renderingContext, bool hasDateField, bool hasTimeField)
    {
      int dateTextBoxWidthPercentage = 0;
      if (hasDateField && hasTimeField && renderingContext.Control.ShowSeconds)
        dateTextBoxWidthPercentage = 55;
      else if (hasDateField && hasTimeField)
        dateTextBoxWidthPercentage = 60;
      else if (hasDateField)
        dateTextBoxWidthPercentage = 100;
      return dateTextBoxWidthPercentage;
    }

    private void RenderReadOnlyValue (BocDateTimeValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      Label label = new Label();

      if (renderingContext.Control.IsDesignMode && string.IsNullOrEmpty (label.Text))
      {
        label.Text = c_designModeEmptyLabelContents;
        //  Too long, can't resize in designer to less than the content's width
        //  Control.label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
      }
      else
      {
        if (renderingContext.Control.Value.HasValue)
        {
          var formatter = renderingContext.Control.DateTimeFormatter;
          DateTime dateTime = renderingContext.Control.Value.Value;

          if (renderingContext.Control.ActualValueType == BocDateTimeValueType.DateTime)
            label.Text = formatter.FormatDateValue (dateTime) + ' ' + formatter.FormatTimeValue (dateTime, renderingContext.Control.ShowSeconds);
          else if (renderingContext.Control.ActualValueType == BocDateTimeValueType.Date)
            label.Text = formatter.FormatDateValue (dateTime);
          else
            label.Text = dateTime.ToString ();
        }
        else
          label.Text = "&nbsp;";
      }

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