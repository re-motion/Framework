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
    /// <summary> The text displayed when control is displayed in desinger, is read-only, and has no contents. </summary>
    private const string c_designModeEmptyLabelContents = "##";

    public BocEnumValueRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures)
        : base (resourceUrlFactory, globalizationService, renderingFeatures)
    {
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      string key = typeof (BocEnumValueRenderer).FullName + "_Style";
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

      innerControl.RenderControl (renderingContext.Writer);

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

      var oneBasedIndex = 1;

      bool needsNullValueItem = (renderingContext.Control.Value == null)
                                && (renderingContext.Control.ListControlStyle.ControlType != ListControlType.RadioButtonList);
      if (!renderingContext.Control.IsRequired || needsNullValueItem)
      {
        var nullItem = CreateNullItem (renderingContext);

        if (IsDiagnosticMetadataRenderingEnabled)
        {
          nullItem.Attributes[DiagnosticMetadataAttributes.ItemID] = "==null==";
          nullItem.Attributes[DiagnosticMetadataAttributes.IndexInCollection] = oneBasedIndex.ToString();
          nullItem.Attributes[DiagnosticMetadataAttributes.Content] = HtmlUtility.StripHtmlTags (renderingContext.Control.GetNullItemText());
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
          item.Attributes[DiagnosticMetadataAttributes.ItemID] = valueInfo.Identifier;
          item.Attributes[DiagnosticMetadataAttributes.IndexInCollection] = oneBasedIndex.ToString();
          item.Attributes[DiagnosticMetadataAttributes.Content] = HtmlUtility.StripHtmlTags (valueInfo.DisplayName);
        }

        listControl.Items.Add (item);
      }

      return listControl;
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
      if (renderingContext.Control.IsDesignMode && renderingContext.Control.EnumerationValueInfo == null)
      {
        text = c_designModeEmptyLabelContents;
        //  Too long, can't resize in designer to less than the content's width
        //  label.Text = "[ " + this.GetType().Name + " \"" + this.ID + "\" ]";
      }
      else if (renderingContext.Control.EnumerationValueInfo != null)
      {
        text = renderingContext.Control.EnumerationValueInfo.DisplayName;
        label.Attributes.Add ("data-value", renderingContext.Control.EnumerationValueInfo.Identifier);
      }
      else
        text = null;

      label.Text = text;

      label.Width = Unit.Empty;
      label.Height = Unit.Empty;
      label.ApplyStyle (renderingContext.Control.CommonStyle);
      label.ApplyStyle (renderingContext.Control.LabelStyle);
      return label;
    }

    public override string GetCssClassBase (IBocEnumValue control)
    {
      return "bocEnumValue";
    }
  }
}