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
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.Services;
using Remotion.Mixins;
using Remotion.Reflection;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering
{
  /// <summary>
  /// Provides a common base class for Standards Mode renderers or <see cref="BocReferenceValue"/> and <see cref="BocAutoCompleteReferenceValue"/>.
  /// </summary>
  public abstract class BocReferenceValueRendererBase<TControl> : BocRendererBase<TControl>
      where TControl: IBocReferenceValueBase
  {
    private readonly ILabelReferenceRenderer _labelReferenceRenderer;
    private readonly IValidationErrorRenderer _validationErrorRenderer;

    protected BocReferenceValueRendererBase (
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

    protected ILabelReferenceRenderer LabelReferenceRenderer
    {
      get { return _labelReferenceRenderer; }
    }

    protected IValidationErrorRenderer ValidationErrorRenderer
    {
      get { return _validationErrorRenderer; }
    }

    protected abstract void RenderEditModeValue (BocRenderingContext<TControl> renderingContext);

    protected abstract string GetAriaHasPopupForCombobox ();

    protected abstract string GetAriaRoleDescriptionForComboboxReadOnly (BocRenderingContext<TControl> renderingContext);

    protected virtual void RegisterJavaScriptFiles (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterWebClientScriptInclude();

      string scriptKey = typeof(BocReferenceValueRendererBase<>).GetFullNameChecked() + "_Script";
      htmlHeadAppender.RegisterJavaScriptInclude(
          scriptKey,
          ResourceUrlFactory.CreateResourceUrl(typeof(BocReferenceValueRendererBase<>), ResourceType.Html, "BocReferenceValueBase.js"));
    }

    [Obsolete("Use Render (BocReferenceValueBaseRenderingContext<>) instead. (Version 1.21.3)", false)]
    protected void Render (BocRenderingContext<TControl> renderingContext)
    {
      throw new NotSupportedException("Use Render (BocReferenceValueBaseRenderingContext<>) instead. (Version 1.21.3)");
    }

    protected void Render (BocReferenceValueBaseRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      const string widthCssProperty = "--width";
      var width = Unit.Empty;
      if (!renderingContext.Control.Width.IsEmpty)
        width = renderingContext.Control.Width;
      else if (renderingContext.Control.Style[HtmlTextWriterStyle.Width] != null)
        width = Unit.Parse(renderingContext.Control.Style[HtmlTextWriterStyle.Width]!);
      if (!width.IsEmpty)
        renderingContext.Control.Style[widthCssProperty] = width.Type == UnitType.Percentage ? "100%" : width.ToString();

      AddAttributesToRender(renderingContext);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);

      RenderContents(renderingContext);

      renderingContext.Writer.RenderEndTag();

      RegisterInitializationScript(renderingContext);

      if (!string.IsNullOrEmpty(renderingContext.Control.ControlServicePath))
      {
        CheckScriptManager(
            renderingContext.Control,
            "{0} '{1}' requires that the page contains a ScriptManager because the ControlServicePath is set.",
            renderingContext.Control.GetType().Name,
            renderingContext.Control.ID);
      }

      if (!width.IsEmpty)
        renderingContext.Control.Style[widthCssProperty] = null;
    }

    private void RegisterInitializationScript (BocRenderingContext<TControl> renderingContext)
    {
      string key = typeof(BocReferenceValueRendererBase<>).GetFullNameChecked() + "_InitializeGlobals";

      if (renderingContext.Control.Page!.ClientScript.IsClientScriptBlockRegistered(typeof(BocReferenceValueRendererBase<>), key))
        return;

      var nullIcon = IconInfo.CreateSpacer(ResourceUrlFactory);

      var script = new StringBuilder(1000);
      script.Append("BocReferenceValueBase.InitializeGlobals(");
      script.AppendFormat("'{0}'", nullIcon.Url);
      script.Append(");");

      renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock(
          renderingContext.Control, typeof(BocReferenceValueRendererBase<>), key, script.ToString());
    }

    protected string? GetControlServicePath (RenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      var controlControlServicePath = renderingContext.Control.ControlServicePath;

      if (string.IsNullOrEmpty(controlControlServicePath))
        return null;
      return renderingContext.Control.ResolveClientUrl(controlControlServicePath);
    }

    protected string GetIconContextAsJson (BocReferenceValueBaseRenderingContext<TControl> renderingContext)
    {
      var jsonBuilder = new StringBuilder(1000);
      // See also BocReferenceValueBase.GetBusinessObjectClass
      var businessObjectClass = renderingContext.Control.Property?.ReferenceClass?.Identifier
                                ?? renderingContext.BusinessObjectWebServiceContext.BusinessObjectClass;

      jsonBuilder.Append("{ ");
      jsonBuilder.Append("businessObjectClass : ");
      AppendStringValueOrNullToScript(jsonBuilder, businessObjectClass);
      jsonBuilder.Append(", ");
      jsonBuilder.Append("arguments : ");
      AppendStringValueOrNullToScript(jsonBuilder, renderingContext.BusinessObjectWebServiceContext.Arguments);
      jsonBuilder.Append(" }");

      return jsonBuilder.ToString();
    }

    [Obsolete("Use RenderContents (BocReferenceValueBaseRenderingContext<>) instead. (Version 1.21.3)", false)]
    protected void RenderContents (BocRenderingContext<TControl> renderingContext)
    {
      throw new NotSupportedException("Use RenderContents (BocReferenceValueBaseRenderingContext<>) instead. (Version 1.21.3)");
    }

    protected virtual void RenderContents (BocReferenceValueBaseRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);

      RenderIcon(renderingContext);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, GetInnerContentID(renderingContext));
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, GetCssClassInnerContent(renderingContext));
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);

      if (renderingContext.Control.IsReadOnly)
      {
        RenderReadOnlyValue(renderingContext);
      }
      else
      {
        RenderEditModeValue(renderingContext);
      }

      renderingContext.Writer.RenderEndTag();

      bool hasOptionsMenu = renderingContext.Control.HasOptionsMenu;
      if (hasOptionsMenu)
      {
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassOptionsMenu);
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);

        if (!string.IsNullOrEmpty(renderingContext.Control.ControlServicePath))
        {
          var stringValueParametersDictionary = new Dictionary<string, string?>();
          stringValueParametersDictionary.Add("controlID", renderingContext.Control.ID);
          stringValueParametersDictionary.Add(
              "controlType",
              TypeUtility.GetPartialAssemblyQualifiedName(MixinTypeUtility.GetUnderlyingTargetType(renderingContext.Control.GetType())));
          stringValueParametersDictionary.Add("businessObjectClass", renderingContext.BusinessObjectWebServiceContext.BusinessObjectClass);
          stringValueParametersDictionary.Add("businessObjectProperty", renderingContext.BusinessObjectWebServiceContext.BusinessObjectProperty);
          stringValueParametersDictionary.Add("businessObject", renderingContext.BusinessObjectWebServiceContext.BusinessObjectIdentifier);
          stringValueParametersDictionary.Add("arguments", renderingContext.BusinessObjectWebServiceContext.Arguments);

          renderingContext.Control.OptionsMenu.SetLoadMenuItemStatus(
              renderingContext.Control.ControlServicePath,
              nameof(IBocReferenceValueWebService.GetMenuItemStatusForOptionsMenu),
              stringValueParametersDictionary);
        }

        renderingContext.Control.OptionsMenu.Width = renderingContext.Control.OptionsMenuWidth;
        renderingContext.Control.OptionsMenu.RenderControl(renderingContext.Writer);

        renderingContext.Writer.RenderEndTag();
      }

      renderingContext.Writer.RenderEndTag();
    }

    private void RenderIcon (BocRenderingContext<TControl> renderingContext)
    {
      var icon = GetIcon(renderingContext);

      if (icon == null && renderingContext.Control.IsReadOnly)
        return;

      if (icon == null)
        icon = IconInfo.CreateSpacer(ResourceUrlFactory);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassIcon);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      icon.Render(renderingContext.Writer, renderingContext.Control);
      renderingContext.Writer.RenderEndTag();
    }

    private void RenderReadOnlyValue (BocRenderingContext<TControl> renderingContext)
    {
      var textBox = new RenderOnlyTextBox() { ClientIDMode = ClientIDMode.Static };
      textBox.ID = renderingContext.Control.ClientID + "_TextValue";
      textBox.EnableViewState = false;
      textBox.Enabled = renderingContext.Control.Enabled;
      textBox.ReadOnly = true;
      textBox.Text = renderingContext.Control.GetLabelText();
      textBox.CssClass = CssClassDefinition.ScreenReaderText;
      if (renderingContext.Control.IsRequired)
        textBox.Attributes.Add(HtmlTextWriterAttribute2.AriaRequired, HtmlAriaRequiredAttributeValue.True);
      textBox.Attributes.Add(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Combobox);
      textBox.Attributes.Add(HtmlTextWriterAttribute2.AriaExpanded, HtmlAriaExpandedAttributeValue.False);
      textBox.Attributes.Add(HtmlTextWriterAttribute2.AriaHasPopup, GetAriaHasPopupForCombobox());
      textBox.Attributes.Add(HtmlTextWriterAttribute2.AriaRoleDescription, GetAriaRoleDescriptionForComboboxReadOnly(renderingContext));
      textBox.Attributes.Add("data-value", renderingContext.Control.BusinessObjectUniqueIdentifier ?? renderingContext.Control.NullValueString);
      var labelIDs = renderingContext.Control.GetLabelIDs().ToArray();
      _labelReferenceRenderer.SetLabelReferenceOnControl(textBox, labelIDs);

      var validationErrors = GetValidationErrorsToRender(renderingContext).ToArray();
      var validationErrorsID = GetValidationErrorsID(renderingContext);

      ValidationErrorRenderer.SetValidationErrorsReferenceOnControl(textBox, validationErrorsID, validationErrors);

      var label = new Label { ClientIDMode = ClientIDMode.Static };
      label.EnableViewState = false;
      label.Height = Unit.Empty;
      label.Width = Unit.Empty;
      label.ApplyStyle(renderingContext.Control.CommonStyle);
      label.ApplyStyle(renderingContext.Control.LabelStyle);
      label.Attributes.Add(HtmlTextWriterAttribute2.AriaHidden, HtmlAriaHiddenAttributeValue.True);
      label.Text = PlainTextString.CreateFromText(renderingContext.Control.GetLabelText()).ToString(WebStringEncoding.Html);

      textBox.RenderControl(renderingContext.Writer);
      label.RenderControl(renderingContext.Writer);
      ValidationErrorRenderer.RenderValidationErrors(renderingContext.Writer, validationErrorsID, validationErrors);
    }

    [CanBeNull]
    private IconInfo? GetIcon (BocRenderingContext<TControl> renderingContext)
    {
      if (!renderingContext.Control.IsIconEnabled())
        return null;

      var icon = renderingContext.Control.GetIcon();
      if (icon == null)
        return null;

      return icon;
    }

    protected IEnumerable<PlainTextString> GetValidationErrorsToRender (BocRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      return renderingContext.Control.GetValidationErrors();
    }

    protected string GetValidationErrorsID (BocRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      return renderingContext.Control.ClientID + "_ValidationErrors";
    }

    private string GetInnerContentID (BocRenderingContext<TControl> renderingContext)
    {
      return renderingContext.Control.ClientID + "_Content";
    }

    protected override string GetAdditionalCssClass (TControl control)
    {
      ArgumentUtility.CheckNotNull("control", control);

      var additionalCssClass = base.GetAdditionalCssClass(control);
      if (control.HasOptionsMenu && control.ReserveOptionsMenuWidth)
        additionalCssClass += " " + CssClassReserveOptionsMenuWidth;

      return additionalCssClass;
    }

    private string GetCssClassInnerContent (BocRenderingContext<TControl> renderingContext)
    {
      string cssClass = CssClassInnerContent + " " + CssClassThemed;

      if (GetIcon(renderingContext) != null)
        cssClass += " " + CssClassHasIcon;

      if (!renderingContext.Control.HasOptionsMenu)
        cssClass += " " + CssClassWithoutOptionsMenu;
      else
        cssClass += " " + CssClassHasOptionsMenu;

      return cssClass;
    }

    protected string CssClassContent
    {
      get { return "body"; }
    }

    protected string CssClassInnerContent
    {
      get { return "content"; }
    }

    private string CssClassHasOptionsMenu
    {
      get { return "hasOptionsMenu"; }
    }

    private string CssClassWithoutOptionsMenu
    {
      get { return "withoutOptionsMenu"; }
    }

    private string CssClassOptionsMenu
    {
      get { return "optionsMenu"; }
    }

    private string CssClassReserveOptionsMenuWidth
    {
      get { return "reserveOptionsMenuWidth"; }
    }

    private string CssClassHasIcon
    {
      get { return "hasIcon"; }
    }

    protected string CssClassIcon
    {
      get { return "icon"; }
    }
  }
}
