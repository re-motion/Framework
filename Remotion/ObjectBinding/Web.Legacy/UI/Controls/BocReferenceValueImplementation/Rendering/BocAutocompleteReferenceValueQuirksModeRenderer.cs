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
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.Legacy.UI.Controls;
using Remotion.Web.Legacy.UI.Controls.Rendering;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocReferenceValueImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="BocReferenceValue"/> controls in Quirks Mode.
  /// </summary>
  /// <remarks>
  /// <para>During edit mode, the control is displayed using a <see cref="System.Web.UI.WebControls.DropDownList"/>.</para>
  /// <para>During read-only mode, the control's value is displayed using a <see cref="System.Web.UI.WebControls.Label"/>.</para>
  /// </remarks>
  public class BocAutoCompleteReferenceValueQuirksModeRenderer : 
      BocReferenceValueQuirksModeRendererBase<IBocAutoCompleteReferenceValue, BocAutoCompleteReferenceValueRenderingContext>, IBocAutoCompleteReferenceValueRenderer
  {
    private const string c_defaultControlWidth = "150pt";

    public BocAutoCompleteReferenceValueQuirksModeRenderer (IResourceUrlFactory resourceUrlFactory)
        : this (resourceUrlFactory, () => new RenderOnlyTextBox())
    {
    }

    protected BocAutoCompleteReferenceValueQuirksModeRenderer (IResourceUrlFactory resourceUrlFactory, Func<TextBox> textBoxFactory) 
      : base(resourceUrlFactory)
    {
      ArgumentUtility.CheckNotNull ("textBoxFactory", textBoxFactory);
      TextBoxFactory = textBoxFactory;
    }

    private Func<TextBox> TextBoxFactory { get; set; }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      RegisterJavaScriptFiles (htmlHeadAppender);
      RegisterStylesheets (htmlHeadAppender);
    }

    protected sealed override void RegisterJavaScriptFiles (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      base.RegisterJavaScriptFiles (htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude ();
      htmlHeadAppender.RegisterJQueryIFrameShimJavaScriptInclude ();

      string jqueryAutocompleteScriptKey = typeof (BocAutoCompleteReferenceValueQuirksModeRenderer).FullName + "_JQueryAutoCompleteScript";
      var jqueryScriptUrl = ResourceUrlFactory.CreateResourceUrl (typeof (BocAutoCompleteReferenceValueQuirksModeRenderer), ResourceType.Html, 
        "BocAutoCompleteReferenceValue.jquery.js");
      htmlHeadAppender.RegisterJavaScriptInclude (jqueryAutocompleteScriptKey, jqueryScriptUrl);

      string scriptKey = typeof (BocAutoCompleteReferenceValueQuirksModeRenderer).FullName + "_Script";
      var scriptUrl = ResourceUrlFactory.CreateResourceUrl (typeof (BocAutoCompleteReferenceValueQuirksModeRenderer), ResourceType.Html, 
        "BocAutoCompleteReferenceValue.js");
      htmlHeadAppender.RegisterJavaScriptInclude (scriptKey, scriptUrl);
    }

    private void RegisterStylesheets (HtmlHeadAppender htmlHeadAppender)
    {
      string styleKey = typeof (BocAutoCompleteReferenceValueQuirksModeRenderer).FullName + "_Style";
      var styleUrl = ResourceUrlFactory.CreateResourceUrl (typeof (BocAutoCompleteReferenceValueQuirksModeRenderer), ResourceType.Html, 
        "BocAutoCompleteReferenceValue.css");
      htmlHeadAppender.RegisterStylesheetLink (styleKey, styleUrl, HtmlHeadAppender.Priority.Library);

      string jqueryAutocompleteStyleKey = typeof (BocAutoCompleteReferenceValueQuirksModeRenderer).FullName + "_JQueryAutoCompleteStyle";
      var jqueryStyleUrl = ResourceUrlFactory.CreateResourceUrl (typeof (BocAutoCompleteReferenceValueQuirksModeRenderer), ResourceType.Html, 
        "BocAutoCompleteReferenceValue.jquery.css");
      htmlHeadAppender.RegisterStylesheetLink (jqueryAutocompleteStyleKey, jqueryStyleUrl, HtmlHeadAppender.Priority.Library);
    }

    public new void Render (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      base.Render (renderingContext);

      RegisterInitializationScript (renderingContext);

      CheckScriptManager (
          renderingContext.Control,
          "{0} '{1}' requires that the page contains a ScriptManager.",
          renderingContext.Control.GetType().Name,
          renderingContext.Control.ID);
    }

    protected override void RenderContents (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      TextBox textBox = GetTextbox (renderingContext);
      textBox.Page = renderingContext.Control.Page.WrappedInstance;
      HiddenField hiddenField = GetHiddenField (renderingContext);
      Label label = GetLabel (renderingContext);
      Image icon = GetIcon (renderingContext);

      if (IsEmbedInOptionsMenu (renderingContext))
        RenderContentsWithIntegratedOptionsMenu (renderingContext, textBox, label);
      else
        RenderContentsWithSeparateOptionsMenu (renderingContext, textBox, hiddenField, label, icon);
    }

    private void RegisterInitializationScript (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      if (renderingContext.Control.IsReadOnly)
        return;

      if (!renderingContext.Control.Enabled)
        return;

      string key = renderingContext.Control.UniqueID + "_InitializationScript";

      var script = new StringBuilder (1000);
      script.Append ("$(document).ready( function() { BocAutoCompleteReferenceValue.Initialize(");
      script.AppendFormat ("$('#{0}'), ", renderingContext.Control.GetTextValueName());
      script.AppendFormat ("$('#{0}'), ", renderingContext.Control.GetKeyValueName());
      script.AppendFormat ("$('#{0}'),", GetDropDownButtonName(renderingContext));

      if (renderingContext.Control.IsIconEnabled())
        script.AppendFormat ("$('#{0} .{1}'), ", renderingContext.Control.ClientID, CssClassCommand);
      else
        script.Append ("null, ");

      script.AppendFormat ("'{0}', ", renderingContext.Control.ResolveClientUrl (renderingContext.Control.SearchServicePath));

      script.AppendFormat ("{0}, ", renderingContext.Control.CompletionSetCount);
      script.AppendFormat ("{0}, ", renderingContext.Control.DropDownDisplayDelay);
      script.AppendFormat ("{0}, ", renderingContext.Control.DropDownRefreshDelay);
      script.AppendFormat ("{0}, ", renderingContext.Control.SelectionUpdateDelay);

      script.AppendFormat ("'{0}', ", renderingContext.Control.NullValueString);
      AppendBooleanValueToScript (script, renderingContext.Control.TextBoxStyle.AutoPostBack ?? false);
      script.Append (", ");
      script.Append (GetSearchContextAsJson (renderingContext.SearchAvailableObjectWebServiceContext));
      script.Append (", ");
      AppendStringValueOrNullToScript (script, GetIconServicePath (renderingContext));
      script.Append (", ");
      script.Append (GetIconContextAsJson (renderingContext.IconWebServiceContext) ?? "null");
      script.Append (", ");
      script.Append (GetCommandInfoAsJson (renderingContext) ?? "null");

      script.Append ("); } );");

      renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (
          renderingContext.Control, typeof (IBocAutoCompleteReferenceValue), key, script.ToString ());
    }

    private string GetSearchContextAsJson (SearchAvailableObjectWebServiceContext searchContext)
    {
      var jsonBuilder = new StringBuilder (1000);

      jsonBuilder.Append ("{ ");
      jsonBuilder.Append ("businessObjectClass : ");
      AppendStringValueOrNullToScript (jsonBuilder, searchContext.BusinessObjectClass);
      jsonBuilder.Append (", ");
      jsonBuilder.Append ("businessObjectProperty : ");
      AppendStringValueOrNullToScript (jsonBuilder, searchContext.BusinessObjectProperty);
      jsonBuilder.Append (", ");
      jsonBuilder.Append ("businessObject : ");
      AppendStringValueOrNullToScript (jsonBuilder, searchContext.BusinessObjectIdentifier);
      jsonBuilder.Append (", ");
      jsonBuilder.Append ("args : ");
      AppendStringValueOrNullToScript (jsonBuilder, searchContext.Args);
      jsonBuilder.Append (" }");

      return jsonBuilder.ToString ();
    }

    private TextBox GetTextbox (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      var textBox = TextBoxFactory();
      textBox.ID = renderingContext.Control.GetTextValueName();
      textBox.EnableViewState = false;
      textBox.Text = renderingContext.Control.GetLabelText ();

      textBox.Enabled = renderingContext.Control.Enabled;
      textBox.Height = Unit.Empty;
      textBox.Width = Unit.Empty;
      textBox.ApplyStyle (renderingContext.Control.CommonStyle);
      renderingContext.Control.TextBoxStyle.ApplyStyle (textBox);

      return textBox;
    }

    private HiddenField GetHiddenField (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      var hiddenField = new HiddenField();
      hiddenField.ID = renderingContext.Control.GetKeyValueName();
      hiddenField.Value = renderingContext.Control.BusinessObjectUniqueIdentifier ?? renderingContext.Control.NullValueString;

      return hiddenField;
    }

    private Label GetLabel (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      var label = new Label { EnableViewState = false, Height = Unit.Empty, Width = Unit.Empty };
      label.ApplyStyle (renderingContext.Control.CommonStyle);
      label.ApplyStyle (renderingContext.Control.LabelStyle);
      label.Text = HttpUtility.HtmlEncode (renderingContext.Control.GetLabelText ());
      return label;
    }

    private Image GetIcon (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      var icon = new Image { EnableViewState = false, Visible = false };
      if (renderingContext.Control.IsIconEnabled())
      {
        IconInfo iconInfo = renderingContext.Control.GetIcon ();

        if (iconInfo != null)
        {
          icon.ImageUrl = iconInfo.Url;
          icon.Width = iconInfo.Width;
          icon.Height = iconInfo.Height;

          icon.Visible = true;
          icon.Style["vertical-align"] = "middle";
          icon.Style["border-style"] = "none";

          if (renderingContext.Control.IsCommandEnabled ())
          {
            if (string.IsNullOrEmpty (iconInfo.AlternateText))
              icon.AlternateText = renderingContext.Control.GetLabelText ();
            else
              icon.AlternateText = iconInfo.AlternateText;
          }
        }
      }
      return icon;
    }

    protected override void AddAdditionalAttributes (RenderingContext<IBocAutoCompleteReferenceValue> renderingContext)
    {
      base.AddAdditionalAttributes(renderingContext);
      renderingContext.Writer.AddStyleAttribute ("display", "inline");
    }

    public override string CssClassBase
    {
      get { return "bocAutoCompleteReferenceValue"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="BocReferenceValue"/>'s value. </summary>
    /// <remarks> Class: <c>bocReferenceValueContent</c> </remarks>
    public virtual string CssClassContent
    {
      get { return "bocAutoCompleteReferenceValueContent"; }
    }

    private string CssClassCommand
    {
      get { return "command"; }
    }

    private void RenderContentsWithSeparateOptionsMenu (BocAutoCompleteReferenceValueRenderingContext renderingContext, TextBox textBox, HiddenField hiddenField, Label label, Image icon)
    {
      bool isReadOnly = renderingContext.Control.IsReadOnly;

      bool isControlHeightEmpty = renderingContext.Control.Height.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["height"]);
      bool isTextboxHeightEmpty = textBox.Height.IsEmpty
                                  && string.IsNullOrEmpty (textBox.Style["height"]);
      bool isControlWidthEmpty = renderingContext.Control.Width.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["width"]);
      bool isLabelWidthEmpty = label.Width.IsEmpty
                               && string.IsNullOrEmpty (label.Style["width"]);
      bool isTextboxWidthEmpty = textBox.Width.IsEmpty
                                 && string.IsNullOrEmpty (textBox.Style["width"]);
      if (isReadOnly)
      {
        if (isLabelWidthEmpty && !isControlWidthEmpty)
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      }
      else
      {
        if (!isControlHeightEmpty && isTextboxHeightEmpty)
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");

        if (isTextboxWidthEmpty)
        {
          if (isControlWidthEmpty)
            renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, c_defaultControlWidth);
          else
            renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
        }
      }

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Border, "0");
      renderingContext.Writer.AddStyleAttribute ("display", "inline");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Table); // Begin table
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr); //  Begin tr

      bool isCommandEnabled = renderingContext.Control.BusinessObjectUniqueIdentifier != null && renderingContext.Control.IsCommandEnabled ();

      string postBackEvent = GetPostBackEvent (renderingContext);
      string objectID = renderingContext.Control.BusinessObjectUniqueIdentifier ?? string.Empty;

      if (isReadOnly)
        RenderReadOnlyValue (renderingContext, icon, label, isCommandEnabled, postBackEvent, string.Empty, objectID);
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (renderingContext, icon, isCommandEnabled, postBackEvent, string.Empty, objectID);
        RenderEditModeValue (renderingContext, textBox, hiddenField, isControlHeightEmpty, isTextboxHeightEmpty, isTextboxWidthEmpty);
      }

      bool hasOptionsMenu = renderingContext.Control.HasOptionsMenu;
      if (hasOptionsMenu)
      {
        renderingContext.Writer.AddStyleAttribute ("padding-left", "0.3em");
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        //writer.AddAttribute ("align", "right");
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
        renderingContext.Control.OptionsMenu.Width = renderingContext.Control.OptionsMenuWidth;
        renderingContext.Control.OptionsMenu.RenderControl (renderingContext.Writer);
        renderingContext.Writer.RenderEndTag (); //  End td
      }

      //HACK: Opera has problems with inline tables and may collapse contents unless a cell with width 0% is present
      if (!renderingContext.Control.IsDesignMode && !isReadOnly && !hasOptionsMenu && !icon.Visible
          && renderingContext.HttpContext.Request.Browser.Browser == "Opera")
      {
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td
        renderingContext.Writer.Write ("&nbsp;");
        renderingContext.Writer.RenderEndTag (); // End td
      }

      renderingContext.Writer.RenderEndTag ();
      renderingContext.Writer.RenderEndTag ();
    }

    private void RenderContentsWithIntegratedOptionsMenu (BocAutoCompleteReferenceValueRenderingContext renderingContext, TextBox textBox, Label label)
    {
      bool isReadOnly = renderingContext.Control.IsReadOnly;

      bool isControlHeightEmpty = renderingContext.Control.Height.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["height"]);
      bool isTextboxHeightEmpty = string.IsNullOrEmpty (textBox.Style["height"]);
      bool isControlWidthEmpty = renderingContext.Control.Width.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["width"]);
      bool isLabelWidthEmpty = string.IsNullOrEmpty (label.Style["width"]);
      bool isTextBoxWidthEmpty = string.IsNullOrEmpty (textBox.Style["width"]);

      if (isReadOnly)
      {
        if (isLabelWidthEmpty && !isControlWidthEmpty)
          renderingContext.Control.OptionsMenu.Style["width"] = "100%";
        else
          renderingContext.Control.OptionsMenu.Style["width"] = "0%";
      }
      else
      {
        if (!isControlHeightEmpty && isTextboxHeightEmpty)
          renderingContext.Control.OptionsMenu.Style["height"] = "100%";

        if (isTextBoxWidthEmpty)
        {
          if (isControlWidthEmpty)
            renderingContext.Control.OptionsMenu.Style["width"] = c_defaultControlWidth;
          else
            renderingContext.Control.OptionsMenu.Style["width"] = "100%";
        }
      }

      renderingContext.Control.OptionsMenu.SetRenderHeadTitleMethodDelegate ((writer)=> RenderOptionsMenuTitle(renderingContext));
      renderingContext.Control.OptionsMenu.RenderControl (renderingContext.Writer);
      renderingContext.Control.OptionsMenu.SetRenderHeadTitleMethodDelegate (null);
    }

    public void RenderOptionsMenuTitle (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      var textbox = GetTextbox (renderingContext);
      var hiddenField = GetHiddenField (renderingContext);
      textbox.Page = renderingContext.Control.Page.WrappedInstance;
      hiddenField.Page = renderingContext.Control.Page.WrappedInstance;
      Image icon = GetIcon (renderingContext);
      Label label = GetLabel (renderingContext);
      bool isReadOnly = renderingContext.Control.IsReadOnly;

      bool isControlHeightEmpty = renderingContext.Control.Height.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["height"]);
      bool isTextboxHeightEmpty = string.IsNullOrEmpty (textbox.Style["height"]);
      bool isControlWidthEmpty = renderingContext.Control.Width.IsEmpty && string.IsNullOrEmpty (renderingContext.Control.Style["width"]);
      bool isTextboxWidthEmpty = string.IsNullOrEmpty (textbox.Style["width"]);

      bool isCommandEnabled = renderingContext.Control.BusinessObjectUniqueIdentifier != null && renderingContext.Control.IsCommandEnabled ();

      string postBackEvent = GetPostBackEvent (renderingContext);
      string objectID = renderingContext.Control.BusinessObjectUniqueIdentifier ?? string.Empty;

      if (isReadOnly)
      {
        RenderReadOnlyValue (renderingContext, icon, label, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        if (!isControlWidthEmpty)
        {
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1%");
          renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
          renderingContext.Writer.RenderEndTag();
        }
      }
      else
      {
        if (icon.Visible)
          RenderSeparateIcon (renderingContext, icon, isCommandEnabled, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        RenderEditModeValue (renderingContext, textbox, hiddenField, isControlHeightEmpty, isTextboxHeightEmpty, isTextboxWidthEmpty);
      }
    }

    private void RenderSeparateIcon (BocAutoCompleteReferenceValueRenderingContext renderingContext, Image icon, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      renderingContext.Writer.AddStyleAttribute ("padding-right", "0.3em");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassCommand);
      if (isCommandEnabled)
      {
        renderingContext.Control.Command.ItemID = "Command";
        renderingContext.Control.Command.RenderBegin (renderingContext.Writer, LegacyRenderingFeatures.ForLegacy, postBackEvent, onClick, objectID, null);
        if (!string.IsNullOrEmpty (renderingContext.Control.Command.ToolTip))
          icon.ToolTip = renderingContext.Control.Command.ToolTip;
      }
      else
      {
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID + "_Command");
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
      icon.RenderControl (renderingContext.Writer);
      if (isCommandEnabled)
        renderingContext.Control.Command.RenderEnd (renderingContext.Writer);
      else
        renderingContext.Writer.RenderEndTag();

      renderingContext.Writer.RenderEndTag (); //  End td
    }

    private void RenderReadOnlyValue (BocAutoCompleteReferenceValueRenderingContext renderingContext, Image icon, Label label, bool isCommandEnabled, string postBackEvent, string onClick, string objectID)
    {
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "auto");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassCommand);
      if (isCommandEnabled)
      {
        renderingContext.Control.Command.ItemID = "Command";
        renderingContext.Control.Command.RenderBegin (renderingContext.Writer, LegacyRenderingFeatures.ForLegacy, postBackEvent, onClick, objectID, null);
      }
      else
      {
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      }
      if (icon.Visible)
      {
        icon.RenderControl (renderingContext.Writer);
        renderingContext.Writer.Write ("&nbsp;");
      }
      label.RenderControl (renderingContext.Writer);
      if (isCommandEnabled)
        renderingContext.Control.Command.RenderEnd (renderingContext.Writer);
      else
        renderingContext.Writer.RenderEndTag ();

      renderingContext.Writer.RenderEndTag(); //  End td
    }

    private void RenderEditModeValue (
        BocAutoCompleteReferenceValueRenderingContext renderingContext,
        TextBox textBox,
        HiddenField hiddenField,
        bool isControlHeightEmpty,
        bool isDropDownListHeightEmpty,
        bool isDropDownListWidthEmpty)
    {
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td

      if (!isControlHeightEmpty && isDropDownListHeightEmpty)
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Height, "100%");
      if (isDropDownListWidthEmpty)
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      
      bool autoPostBack = textBox.AutoPostBack;
      textBox.AutoPostBack = false;
      textBox.RenderControl (renderingContext.Writer);
      textBox.AutoPostBack = autoPostBack;

      if (autoPostBack)
      {
        PostBackOptions options = new PostBackOptions (textBox, string.Empty);
        if (textBox.CausesValidation)
        {
          options.PerformValidation = true;
          options.ValidationGroup = textBox.ValidationGroup;
        }
        if (renderingContext.Control.Page.Form != null)
          options.AutoPostBack = true;
        var postBackEventReference = renderingContext.Control.Page.ClientScript.GetPostBackEventReference (options, true);
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onchange, postBackEventReference);
      }
      hiddenField.RenderControl (renderingContext.Writer);

      renderingContext.Writer.RenderEndTag(); //  End td

      RenderEditModeValueExtension (renderingContext);
    }

    /// <summary> Called after the edit mode value's cell is rendered. </summary>
    /// <remarks> Render a table cell: &lt;td style="width:0%"&gt;Your contents goes here&lt;/td&gt;</remarks>
    protected virtual void RenderEditModeValueExtension (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      if (!renderingContext.Control.Enabled)
        return;

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td);
      RenderDropdownButton (renderingContext);
      renderingContext.Writer.RenderEndTag ();
    }

    private void RenderDropdownButton (BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, GetDropDownButtonName(renderingContext));
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassButton);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      IconInfo.CreateSpacer (ResourceUrlFactory).Render (renderingContext.Writer, renderingContext.Control);
      renderingContext.Writer.RenderEndTag ();
    }

    protected string CssClassButton
    {
      get { return "bocAutoCompleteReferenceValueButton"; }
    }

    private bool IsEmbedInOptionsMenu(BocAutoCompleteReferenceValueRenderingContext renderingContext)
    {
      return renderingContext.Control.HasValueEmbeddedInsideOptionsMenu == true && renderingContext.Control.HasOptionsMenu
               || renderingContext.Control.HasValueEmbeddedInsideOptionsMenu == null && renderingContext.Control.IsReadOnly && renderingContext.Control.HasOptionsMenu;
    }
  }
}