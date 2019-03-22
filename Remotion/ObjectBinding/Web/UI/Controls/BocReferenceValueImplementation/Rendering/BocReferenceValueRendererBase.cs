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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.Services;
using Remotion.Mixins;
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
        : base (resourceUrlFactory, globalizationService, renderingFeatures)
    {
      ArgumentUtility.CheckNotNull ("labelReferenceRenderer", labelReferenceRenderer);
      ArgumentUtility.CheckNotNull ("validationErrorRenderer", validationErrorRenderer);

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

    protected abstract void RenderEditModeValueWithSeparateOptionsMenu (BocRenderingContext<TControl> renderingContext);
    protected abstract void RenderEditModeValueWithIntegratedOptionsMenu (BocRenderingContext<TControl> renderingContext);

    protected virtual void RegisterJavaScriptFiles (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude();

      string scriptKey = typeof (BocReferenceValueRendererBase<>).FullName + "_Script";
      htmlHeadAppender.RegisterJavaScriptInclude (
          scriptKey,
          ResourceUrlFactory.CreateResourceUrl (typeof (BocReferenceValueRendererBase<>), ResourceType.Html, "BocReferenceValueBase.js"));
    }

    [Obsolete ("Use Render (BocReferenceValueBaseRenderingContext<>) instead. (Version 1.21.3)", false)]
    protected void Render (BocRenderingContext<TControl> renderingContext)
    {
      throw new NotSupportedException ("Use Render (BocReferenceValueBaseRenderingContext<>) instead. (Version 1.21.3)");
    }

    protected void Render (BocReferenceValueBaseRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      AddAttributesToRender (renderingContext);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      RenderContents (renderingContext);

      renderingContext.Writer.RenderEndTag();

      RegisterInitializationScript (renderingContext);

      if (!string.IsNullOrEmpty (renderingContext.Control.IconServicePath))
      {
        CheckScriptManager (
            renderingContext.Control,
            "{0} '{1}' requires that the page contains a ScriptManager because the IconServicePath is set.",
            renderingContext.Control.GetType().Name,
            renderingContext.Control.ID);
      }
    }

    private void RegisterInitializationScript (BocRenderingContext<TControl> renderingContext)
    {
      string key = typeof (BocReferenceValueRendererBase<>).FullName + "_InitializeGlobals";

      if (renderingContext.Control.Page.ClientScript.IsClientScriptBlockRegistered (typeof (BocReferenceValueRendererBase<>), key))
        return;

      var nullIcon = IconInfo.CreateSpacer (ResourceUrlFactory);

      var script = new StringBuilder (1000);
      script.Append ("BocReferenceValueBase.InitializeGlobals(");
      script.AppendFormat ("'{0}'", nullIcon.Url);
      script.Append (");");

      renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (
          renderingContext.Control, typeof (BocReferenceValueRendererBase<>), key, script.ToString());
    }

    protected string GetIconServicePath (RenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      if (!renderingContext.Control.IsIconEnabled())
        return null;

      var iconServicePath = renderingContext.Control.IconServicePath;

      if (string.IsNullOrEmpty (iconServicePath))
        return null;
      return renderingContext.Control.ResolveClientUrl (iconServicePath);
    }

    protected string GetIconContextAsJson (BusinessObjectIconWebServiceContext iconServiceContext)
    {
      if (iconServiceContext == null)
        return null;

      var jsonBuilder = new StringBuilder (1000);

      jsonBuilder.Append ("{ ");
      jsonBuilder.Append ("businessObjectClass : ");
      AppendStringValueOrNullToScript (jsonBuilder, iconServiceContext.BusinessObjectClass);
      jsonBuilder.Append (", ");
      jsonBuilder.Append ("arguments : ");
      AppendStringValueOrNullToScript (jsonBuilder, iconServiceContext.Arguments);
      jsonBuilder.Append (" }");

      return jsonBuilder.ToString();
    }

    [Obsolete ("This feature has been deprecated and will be removed in version 1.22.0. (Version 1.21.3)", false)]
    protected string GetCommandInfoAsJson (BocRenderingContext<TControl> renderingContext)
    {
      var command = renderingContext.Control.Command;
      if (command == null)
        return null;

      if (command.Show == CommandShow.ReadOnly)
        return null;

      var postBackEvent = GetPostBackEvent (renderingContext);
      var commandInfo = command.GetCommandInfo (postBackEvent, new[] { "-0-" }, "", null, new NameValueCollection(), true);

      var jsonBuilder = new StringBuilder (1000);

      jsonBuilder.Append ("{ ");

      jsonBuilder.Append ("href : ");
      string href;
      if (commandInfo.Href != null)
        href = commandInfo.Href.Replace ("-0-", "{0}");
      else
        href = null;
      AppendStringValueOrNullToScript (jsonBuilder, href);

      jsonBuilder.Append (", ");

      jsonBuilder.Append ("target : ");
      string target = commandInfo.Target;
      AppendStringValueOrNullToScript (jsonBuilder, target);

      jsonBuilder.Append (", ");

      jsonBuilder.Append ("onClick : ");
      string onClick = commandInfo.OnClick;
      AppendStringValueOrNullToScript (jsonBuilder, onClick);

      jsonBuilder.Append (", ");

      jsonBuilder.Append ("title : ");
      string title = commandInfo.Title;
      AppendStringValueOrNullToScript (jsonBuilder, title);

      jsonBuilder.Append (" }");

      return jsonBuilder.ToString();
    }

    [Obsolete ("Use RenderContents (BocReferenceValueBaseRenderingContext<>) instead. (Version 1.21.3)", false)]
    protected void RenderContents (BocRenderingContext<TControl> renderingContext)
    {
      throw new NotSupportedException ("Use RenderContents (BocReferenceValueBaseRenderingContext<>) instead. (Version 1.21.3)");
    }

    protected virtual void RenderContents (BocReferenceValueBaseRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

#pragma warning disable 618
      if (IsEmbedInOptionsMenu (renderingContext))
        RenderContentsWithIntegratedOptionsMenu (renderingContext);
#pragma warning restore 618
      else
        RenderContentsWithSeparateOptionsMenu (renderingContext);
    }

    [Obsolete ("This feature has been deprecated and will be removed in version 1.22.0. (Version 1.21.3)", false)]
    private void RenderContentsWithIntegratedOptionsMenu (BocReferenceValueBaseRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      if (!string.IsNullOrEmpty (renderingContext.Control.ControlServicePath))
      {
        var stringValueParametersDictionary = new Dictionary<string, string>();
        stringValueParametersDictionary.Add ("controlID", renderingContext.Control.ID);
        stringValueParametersDictionary.Add (
            "controlType",
            TypeUtility.GetPartialAssemblyQualifiedName (MixinTypeUtility.GetUnderlyingTargetType (renderingContext.Control.GetType())));
        stringValueParametersDictionary.Add ("businessObjectClass", renderingContext.BusinessObjectWebServiceContext.BusinessObjectClass);
        stringValueParametersDictionary.Add ("businessObjectProperty", renderingContext.BusinessObjectWebServiceContext.BusinessObjectProperty);
        stringValueParametersDictionary.Add ("businessObject", renderingContext.BusinessObjectWebServiceContext.BusinessObjectIdentifier);
        stringValueParametersDictionary.Add ("arguments", renderingContext.BusinessObjectWebServiceContext.Arguments);

        renderingContext.Control.OptionsMenu.SetLoadMenuItemStatus (
            renderingContext.Control.ControlServicePath,
            nameof (IBocReferenceValueWebService.GetMenuItemStatusForOptionsMenu),
            stringValueParametersDictionary);
      }

      renderingContext.Control.OptionsMenu.SetRenderHeadTitleMethodDelegate (writer => RenderOptionsMenuTitle (renderingContext));
      renderingContext.Control.OptionsMenu.RenderControl (renderingContext.Writer);
      renderingContext.Control.OptionsMenu.SetRenderHeadTitleMethodDelegate (null);
    }

    private void RenderContentsWithSeparateOptionsMenu (BocReferenceValueBaseRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassContent);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      string postBackEvent = GetPostBackEvent (renderingContext);
      string objectID = renderingContext.Control.BusinessObjectUniqueIdentifier ?? string.Empty;

      if (renderingContext.Control.IsReadOnly)
        RenderReadOnlyValue (renderingContext, postBackEvent, string.Empty, objectID);
      else
      {
        RenderSeparateIcon (renderingContext, postBackEvent, string.Empty, objectID);
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, GetCssClassInnerContent (renderingContext));
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

        RenderEditModeValueWithSeparateOptionsMenu (renderingContext);

        renderingContext.Writer.RenderEndTag();
      }

      bool hasOptionsMenu = renderingContext.Control.HasOptionsMenu;
      if (hasOptionsMenu)
      {
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassOptionsMenu);
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

        if (!string.IsNullOrEmpty (renderingContext.Control.ControlServicePath))
        {
          var stringValueParametersDictionary = new Dictionary<string, string>();
          stringValueParametersDictionary.Add ("controlID", renderingContext.Control.ID);
          stringValueParametersDictionary.Add (
              "controlType",
              TypeUtility.GetPartialAssemblyQualifiedName (MixinTypeUtility.GetUnderlyingTargetType (renderingContext.Control.GetType())));
          stringValueParametersDictionary.Add ("businessObjectClass", renderingContext.BusinessObjectWebServiceContext.BusinessObjectClass);
          stringValueParametersDictionary.Add ("businessObjectProperty", renderingContext.BusinessObjectWebServiceContext.BusinessObjectProperty);
          stringValueParametersDictionary.Add ("businessObject", renderingContext.BusinessObjectWebServiceContext.BusinessObjectIdentifier);
          stringValueParametersDictionary.Add ("arguments", renderingContext.BusinessObjectWebServiceContext.Arguments);

          renderingContext.Control.OptionsMenu.SetLoadMenuItemStatus (
              renderingContext.Control.ControlServicePath,
              nameof (IBocReferenceValueWebService.GetMenuItemStatusForOptionsMenu),
              stringValueParametersDictionary);
        }

        renderingContext.Control.OptionsMenu.Width = renderingContext.Control.OptionsMenuWidth;
        renderingContext.Control.OptionsMenu.RenderControl (renderingContext.Writer);

        renderingContext.Writer.RenderEndTag();
      }

      renderingContext.Writer.RenderEndTag();
    }

    public void RenderOptionsMenuTitle (BocRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      string postBackEvent = GetPostBackEvent (renderingContext);
      string objectID = renderingContext.Control.BusinessObjectUniqueIdentifier ?? string.Empty;

      if (renderingContext.Control.IsReadOnly)
        RenderReadOnlyValue (renderingContext, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
      else
      {
        RenderSeparateIcon (renderingContext, postBackEvent, DropDownMenu.OnHeadTitleClickScript, objectID);
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, GetCssClassInnerContent (renderingContext));
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

        RenderEditModeValueWithIntegratedOptionsMenu (renderingContext);

        renderingContext.Writer.RenderEndTag();
      }
    }

    private string GetPostBackEvent (BocRenderingContext<TControl> renderingContext)
    {
      if (renderingContext.Control.IsDesignMode)
        return "";

      string argument = BocReferenceValueBase.CommandArgumentName;
      return renderingContext.Control.Page.ClientScript.GetPostBackEventReference (renderingContext.Control, argument) + ";";
    }

    private void RenderSeparateIcon (BocRenderingContext<TControl> renderingContext, string postBackEvent, string onClick, string objectID)
    {
      IconInfo icon = null;
      var isIconEnabled = renderingContext.Control.IsIconEnabled();
      if (isIconEnabled)
        icon = GetIcon (renderingContext);

      var anchorClass = CssClassCommand;
      if (icon != null)
        anchorClass += " " + CssClassHasIcon;
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, anchorClass);

#pragma warning disable 618
      var isCommandEnabled = isIconEnabled && IsCommandEnabled (renderingContext);
#pragma warning restore 618

      var labelIDs = renderingContext.Control.GetLabelIDs().ToArray();
      LabelReferenceRenderer.AddLabelsReference (renderingContext.Writer, labelIDs);

#pragma warning disable 618
      var command = GetCommand (renderingContext, isCommandEnabled);
#pragma warning restore 618
      command.RenderBegin (renderingContext.Writer, RenderingFeatures, postBackEvent, onClick, objectID, null);

      if (isIconEnabled)
        icon = icon ?? IconInfo.CreateSpacer (ResourceUrlFactory);

      if (icon != null)
      {
#pragma warning disable CS0618 // Type or member is obsolete
        if (!string.IsNullOrEmpty (command.ToolTip))
          icon.ToolTip = renderingContext.Control.Command.ToolTip;
#pragma warning restore CS0618 // Type or member is obsolete
        icon.Render (renderingContext.Writer, renderingContext.Control);
      }

#pragma warning disable 618
      renderingContext.Control.Command.RenderEnd (renderingContext.Writer);
#pragma warning restore 618
    }

    private void RenderReadOnlyValue (BocRenderingContext<TControl> renderingContext, string postBackEvent, string onClick, string objectID)
    {
      var validationErrors = GetValidationErrorsToRender (renderingContext).ToArray();
      var validationErrorsID = GetValidationErrorsID (renderingContext);

      Label label = GetLabel (renderingContext);
      IconInfo icon = null;
      var isIconEnabled = renderingContext.Control.IsIconEnabled();
      if (isIconEnabled)
        icon = GetIcon (renderingContext);

#pragma warning disable 618
      var isCommandEnabled = IsCommandEnabled (renderingContext);
#pragma warning restore 618

      var anchorClass = CssClassCommand;
      if (icon != null)
        anchorClass += " " + CssClassHasIcon;
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, anchorClass);

      if (!isCommandEnabled)
      {
        renderingContext.Writer.AddAttribute ("tabindex", "0");
        // Screenreaders (JAWS v18) will not read the contents of a span with role=combobox (at least in browse-mode),
        // therefor we have to emulate the reading of the label + contents. Missing from this is "readonly" after the label is read.
        //renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Combobox);
        //renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute2.AriaReadOnly, HtmlAriaReadOnlyAttributeValue.True);
      }

      var labelIDs = renderingContext.Control.GetLabelIDs();
      LabelReferenceRenderer.AddLabelsReference (renderingContext.Writer, labelIDs.ToArray(), new[] { label.ClientID });
      
      var attributeCollection = new AttributeCollection (new StateBag());
      ValidationErrorRenderer.AddValidationErrorsReference (attributeCollection, validationErrorsID, validationErrors);
      attributeCollection.AddAttributes (renderingContext.Writer);

#pragma warning disable 618
      var command = GetCommand (renderingContext, isCommandEnabled);
#pragma warning restore 618
      command.RenderBegin (renderingContext.Writer, RenderingFeatures, postBackEvent, onClick, objectID, null);

      if (icon != null)
        icon.Render (renderingContext.Writer, renderingContext.Control);

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, GetCssClassInnerContent (renderingContext));

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      label.RenderControl (renderingContext.Writer);
      renderingContext.Writer.RenderEndTag();
#pragma warning disable 618
      renderingContext.Control.Command.RenderEnd (renderingContext.Writer);
#pragma warning restore 618

      ValidationErrorRenderer.RenderValidationErrors (renderingContext.Writer, validationErrorsID, validationErrors);
    }

    [Obsolete ("This feature has been deprecated and will be removed in version 1.22.0. (Version 1.21.3)", false)]
    private bool IsCommandEnabled (BocRenderingContext<TControl> renderingContext)
    {
      return renderingContext.Control.BusinessObjectUniqueIdentifier != null && renderingContext.Control.IsCommandEnabled();
    }

    [Obsolete ("This feature has been deprecated and will be removed in version 1.22.0. (Version 1.21.3)", false)]
    private BocCommand GetCommand (BocRenderingContext<TControl> renderingContext, bool isCommandEnabled)
    {
      var command = isCommandEnabled
                        ? renderingContext.Control.Command
                        : new BocCommand (CommandType.None) { OwnerControl = renderingContext.Control };
      command.ItemID = "Command";
      return command;
    }

    private IconInfo GetIcon (BocRenderingContext<TControl> renderingContext)
    {
      var iconInfo = renderingContext.Control.GetIcon();

#pragma warning disable 618
      if (iconInfo != null && renderingContext.Control.IsCommandEnabled())
      {
        if (string.IsNullOrEmpty (iconInfo.AlternateText))
          iconInfo.AlternateText = renderingContext.Control.GetLabelText();
      }
#pragma warning restore 618

      return iconInfo;
    }

    private Label GetLabel (BocRenderingContext<TControl> renderingContext)
    {
      var label = new Label
                  {
                      ID = renderingContext.Control.ClientID + "_Value",
                      ClientIDMode = ClientIDMode.Static,
                      EnableViewState = false,
                      Height = Unit.Empty,
                      Width = Unit.Empty
                  };
      label.ApplyStyle (renderingContext.Control.CommonStyle);
      label.ApplyStyle (renderingContext.Control.LabelStyle);
      label.Text = HttpUtility.HtmlEncode (renderingContext.Control.GetLabelText());
      label.Attributes.Add ("data-value", renderingContext.Control.BusinessObjectUniqueIdentifier ?? renderingContext.Control.NullValueString);
      return label;
    }

    [Obsolete ("This feature has been deprecated and will be removed in version 1.22.0. (Version 1.21.3)", false)]
    private bool IsEmbedInOptionsMenu (BocRenderingContext<TControl> renderingContext)
    {
      return renderingContext.Control.HasValueEmbeddedInsideOptionsMenu == true && renderingContext.Control.HasOptionsMenu
             || renderingContext.Control.HasValueEmbeddedInsideOptionsMenu == null && renderingContext.Control.IsReadOnly
             && renderingContext.Control.HasOptionsMenu;
    }

    protected IEnumerable<string> GetValidationErrorsToRender (BocRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      return renderingContext.Control.GetValidationErrors();
    }

    protected string GetValidationErrorsID (BocRenderingContext<TControl> renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      return renderingContext.Control.ClientID + "_ValidationErrors";
    }

    private string GetCssClassInnerContent (BocRenderingContext<TControl> renderingContext)
    {
      string cssClass = CssClassInnerContent;

      if (!renderingContext.Control.HasOptionsMenu)
        cssClass += " " + CssClassWithoutOptionsMenu;
#pragma warning disable 618
      else if (IsEmbedInOptionsMenu (renderingContext))
        cssClass += " " + CssClassEmbeddedOptionsMenu;
#pragma warning restore 618
      else
        cssClass += " " + CssClassSeparateOptionsMenu;

      return cssClass;
    }

    protected string CssClassContent
    {
      get { return "body"; }
    }

    private string CssClassInnerContent
    {
      get { return "content"; }
    }

    private string CssClassSeparateOptionsMenu
    {
      get { return "separateOptionsMenu"; }
    }

    [Obsolete ("This feature has been deprecated and will be removed in version 1.22.0. (Version 1.21.3)", false)]
    private string CssClassEmbeddedOptionsMenu
    {
      get { return "embeddedOptionsMenu"; }
    }

    private string CssClassWithoutOptionsMenu
    {
      get { return "withoutOptionsMenu"; }
    }

    private string CssClassOptionsMenu
    {
      get { return "optionsMenu"; }
    }

    protected string CssClassCommand
    {
      get { return "command"; }
    }

    private string CssClassHasIcon
    {
      get { return "hasIcon"; }
    }
  }
}