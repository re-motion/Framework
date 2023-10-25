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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.UI;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Globalization;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.DropDownMenuImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="DropDownMenu"/> controls in standard mode.
  /// <seealso cref="IDropDownMenu"/>
  /// </summary>
  [ImplementationFor(typeof(IDropDownMenuRenderer), Lifetime = LifetimeKind.Singleton)]
  public class DropDownMenuRenderer : RendererBase<IDropDownMenu>, IDropDownMenuRenderer
  {
    /// <summary> A list of control specific resources. </summary>
    /// <remarks> 
    ///   Resources will be accessed using 
    ///   <see cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)">IResourceManager.GetString(Enum)</see>. 
    ///   See the documentation of <b>GetString</b> for further details.
    /// </remarks>
    [ResourceIdentifiers]
    [MultiLingualResources("Remotion.Web.Globalization.DropDownMenuRenderer")]
    public enum ResourceIdentifier
    {
      /// <summary> The error message displayed when the menu items could not be loaded from the server. </summary>
      LoadFailedErrorMessage,
      /// <summary> The status message displayed while the menu items are loaded from the server. </summary>
      LoadingStatusMessage,
    }

    private readonly ILabelReferenceRenderer _labelReferenceRenderer;
    private readonly IFallbackNavigationUrlProvider _fallbackNavigationUrlProvider;

    private const string c_whiteSpace = "&nbsp;";

    public DropDownMenuRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures,
        ILabelReferenceRenderer labelReferenceRenderer,
        IFallbackNavigationUrlProvider fallbackNavigationUrlProvider)
        : base(resourceUrlFactory, globalizationService, renderingFeatures)
    {
      ArgumentUtility.CheckNotNull("labelReferenceRenderer", labelReferenceRenderer);
      ArgumentUtility.CheckNotNull("fallbackNavigationUrlProvider", fallbackNavigationUrlProvider);

      _labelReferenceRenderer = labelReferenceRenderer;
      _fallbackNavigationUrlProvider = fallbackNavigationUrlProvider;
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterWebClientScriptInclude();
      htmlHeadAppender.RegisterCommonStyleSheet();

      string styleSheetKey = typeof(DropDownMenuRenderer).GetFullNameChecked() + "_Style";
      var styleSheetUrl = ResourceUrlFactory.CreateThemedResourceUrl(typeof(DropDownMenuRenderer), ResourceType.Html, "DropDownMenu.css");
      htmlHeadAppender.RegisterStylesheetLink(styleSheetKey, styleSheetUrl, HtmlHeadAppender.Priority.Library);
    }

    public void Render (DropDownMenuRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      RegisterMenuItems(renderingContext);

      RegisterEventHandlerScripts(renderingContext);

      AddAttributesToRender(renderingContext);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);

      RenderHead(renderingContext);

      renderingContext.Writer.RenderEndTag();
    }

    public void RenderAsContextMenu (DropDownMenuRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      RegisterMenuItems(renderingContext);

      RegisterEventHandlerScripts(renderingContext);
    }

    private void RenderHead (DropDownMenuRenderingContext renderingContext)
    {
      string cssClass = CssClassHead;
      if (!renderingContext.Control.Enabled)
        cssClass += " " + CssClassDisabled;
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);

      if (HasVisibleTitle(renderingContext))
        RenderVisibleTitle(renderingContext);
      else
        RenderScreenReaderOnlyTitle(renderingContext);

      RenderDropdownButton(renderingContext);

      renderingContext.Writer.RenderEndTag();
    }

    private void RenderVisibleTitle (DropDownMenuRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassDropDownLabel);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID + "_DropDownMenuLabel");
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Button);
      if (renderingContext.Control.Enabled)
      {
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Href, _fallbackNavigationUrlProvider.GetURL());
      }
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.A);

      if (HasTitleIcon(renderingContext))
        renderingContext.Control.TitleIcon!.Render(renderingContext.Writer, renderingContext.Control);

      if (HasTitleText(renderingContext))
      {
        renderingContext.Control.TitleText.WriteTo(renderingContext.Writer);
      }

      renderingContext.Writer.RenderEndTag();
    }

    private void RenderScreenReaderOnlyTitle (DropDownMenuRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID + "_DropDownMenuLabel");
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Hidden, HtmlHiddenAttributeValue.Hidden);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);

      if (HasTitleText(renderingContext))
      {
        renderingContext.Control.TitleText.WriteTo(renderingContext.Writer);
      }
      else if (HasTitleIcon(renderingContext))
      {
        var titleIconDescription = WebString.CreateFromText(renderingContext.Control.TitleIcon!.AlternateText);
        titleIconDescription.WriteTo(renderingContext.Writer);
      }

      renderingContext.Writer.RenderEndTag();
    }

    private void RenderDropdownButton (DropDownMenuRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassDropDownButton);
      if (!HasVisibleTitle(renderingContext))
      {
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID + "_DropDownMenuButton");
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Button);

        var labelID = renderingContext.Control.ClientID + "_DropDownMenuLabel";
        _labelReferenceRenderer.AddLabelsReference(renderingContext.Writer, new[] { labelID });

        if (renderingContext.Control.Enabled)
        {
          renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Href, _fallbackNavigationUrlProvider.GetURL());
        }
      }
      else
      {
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.AriaHidden, HtmlAriaHiddenAttributeValue.True);
      }

      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.A);

      IconInfo.CreateSpacer(ResourceUrlFactory).Render(renderingContext.Writer, renderingContext.Control);

      renderingContext.Writer.RenderEndTag();
    }

    private bool HasVisibleTitle (DropDownMenuRenderingContext renderingContext)
    {
      return renderingContext.Control.ShowTitle && (HasTitleIcon(renderingContext) || HasTitleText(renderingContext));
    }

    private bool HasTitleText (DropDownMenuRenderingContext renderingContext)
    {
      return !renderingContext.Control.TitleText.IsEmpty;
    }

    private bool HasTitleIcon (DropDownMenuRenderingContext renderingContext)
    {
      return renderingContext.Control.TitleIcon != null && !string.IsNullOrEmpty(renderingContext.Control.TitleIcon.Url);
    }

    protected string CssClassDropDownLabel
    {
      get { return "DropDownMenuLabel"; }
    }

    protected string CssClassDropDownButton
    {
      get { return "DropDownMenuButton"; }
    }

    protected string CssClassDropDownButtonPrimary
    {
      get { return CssClassDefinition.ButtonTypePrimary; }
    }

    protected string CssClassDropDownButtonSupplemental
    {
      get { return CssClassDefinition.ButtonTypeSupplemental; }
    }

    private void AddAttributesToRender (DropDownMenuRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      OverrideCssClass(renderingContext, out var backUpCssClass, out var backUpAttributeCssClass);

      AddStandardAttributesToRender(renderingContext);

      RestoreClass(renderingContext, backUpCssClass, backUpAttributeCssClass);

      AddAdditionalAttributes(renderingContext);
    }

    /// <summary>
    /// Called after all attributes have been added by <see cref="AddAttributesToRender"/>.
    /// Use this to render style attributes without putting them into the control's <see cref="IStyledControl.ControlStyle"/> property.
    /// </summary>
    protected virtual void AddAdditionalAttributes (DropDownMenuRenderingContext renderingContext)
    {
      if (renderingContext.Control.ControlStyle.Width.IsEmpty)
      {
        if (!renderingContext.Control.Width.IsEmpty)
          renderingContext.Writer.AddStyleAttribute(HtmlTextWriterStyle.Width, renderingContext.Control.Width.ToString());
      }
    }

    protected virtual string GetAdditionalCssClass (IDropDownMenu dropDownMenu)
    {
      var additionalCssClass = string.Empty;

      if (dropDownMenu.ButtonType == ButtonType.Primary)
        additionalCssClass += " " + CssClassDropDownButtonPrimary;

      if (dropDownMenu.ButtonType == ButtonType.Supplemental)
        additionalCssClass += " " + CssClassDropDownButtonSupplemental;

      return additionalCssClass;
    }

    protected override void AddDiagnosticMetadataAttributes (RenderingContext<IDropDownMenu> renderingContext)
    {
      base.AddDiagnosticMetadataAttributes(renderingContext);

      HtmlUtility.ExtractPlainText(renderingContext.Control.TitleText).AddAttributeTo(renderingContext.Writer, DiagnosticMetadataAttributes.Content);
      renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributes.IsDisabled, (!renderingContext.Control.Enabled).ToString().ToLower());
      renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributes.ButtonType, renderingContext.Control.ButtonType.ToString());
    }

    private void OverrideCssClass (DropDownMenuRenderingContext renderingContext, out string backUpCssClass, out string? backUpAttributeCssClass)
    {
      backUpCssClass = renderingContext.Control.CssClass;
      var hasCssClass = !string.IsNullOrEmpty(backUpCssClass);
      if (hasCssClass)
        renderingContext.Control.CssClass += GetAdditionalCssClass(renderingContext.Control);

      backUpAttributeCssClass = renderingContext.Control.Attributes["class"];
      var hasClassAttribute = !string.IsNullOrEmpty(backUpAttributeCssClass);
      if (hasClassAttribute)
        renderingContext.Control.Attributes["class"] += GetAdditionalCssClass(renderingContext.Control);

      if (!hasCssClass && !hasClassAttribute)
        renderingContext.Control.CssClass = CssClassBase + GetAdditionalCssClass(renderingContext.Control);
    }

    private void RestoreClass (DropDownMenuRenderingContext renderingContext, string backUpCssClass, string? backUpAttributeCssClass)
    {
      renderingContext.Control.CssClass = backUpCssClass;
      renderingContext.Control.Attributes["class"] = backUpAttributeCssClass;
    }

    private void RegisterEventHandlerScripts (DropDownMenuRenderingContext renderingContext)
    {
      if (!renderingContext.Control.Enabled)
        return;

      string key = renderingContext.Control.ClientID + "_KeyDownEventHandlerBindScript";
      string getSelectionCount = (string.IsNullOrEmpty(renderingContext.Control.GetSelectionCount)
          ? "null"
          : renderingContext.Control.GetSelectionCount);
      string hasDedicatedDropDownMenuElement = renderingContext.Control.Mode == MenuMode.DropDownMenu ? "true" : "false";
      string script = string.Format(
          "document.getElementById ('{0}').addEventListener ('keydown', function(event){{ DropDownMenu.OnKeyDown(event, '#{0}', {1}, {2}); }} );",
          renderingContext.Control.ClientID,
          getSelectionCount,
          hasDedicatedDropDownMenuElement);

      renderingContext.Control.Page!.ClientScript.RegisterStartupScriptBlock(renderingContext.Control, typeof(ClientScriptBehavior), key, script);

      if (renderingContext.Control.Enabled && renderingContext.Control.Visible && renderingContext.Control.Mode == MenuMode.DropDownMenu)
      {
        key = renderingContext.Control.ClientID + "_ClickEventHandlerBindScript";
        string elementReference = string.Format("'#{0}'", renderingContext.Control.ClientID);
        string menuIDReference = string.Format("'{0}'", renderingContext.Control.ClientID);
        script = renderingContext.Control.GetBindOpenEventScript(elementReference, menuIDReference, false);
        renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock(renderingContext.Control, typeof(ClientScriptBehavior), key, script);
      }
    }

    private void RegisterMenuItems (DropDownMenuRenderingContext renderingContext)
    {
      string key = renderingContext.Control.UniqueID;
      if (renderingContext.Control.Enabled
          && !renderingContext.Control.Page!.ClientScript.IsStartupScriptRegistered(typeof(DropDownMenuRenderer), key))
      {
        StringBuilder script = new StringBuilder();
        script.Append("DropDownMenu.AddMenuInfo" + " (").AppendLine();
        script.AppendFormat("  new DropDownMenu_MenuInfo ('{0}', ", renderingContext.Control.ClientID);
        script.Append("function(onSuccess, onError) {").AppendLine();
        script.Append("    const unfilteredMenuItems = [").AppendLine();

        WebMenuItem[] menuItems;
        if (renderingContext.Control.EnableGrouping)
          menuItems = renderingContext.Control.MenuItems.GroupMenuItems(true).Where(mi => mi.EvaluateVisible()).ToArray();
        else
          menuItems = renderingContext.Control.MenuItems.Cast<WebMenuItem>().Where(mi => mi.EvaluateVisible()).ToArray();

        var menuItemsToBeRendered = new List<WebMenuItem>(menuItems.Length);
        for (int i = 0; i < menuItems.Length; i++)
        {
          var menuItem = menuItems[i];

          // Skip leading separators
          if (menuItem.IsSeparator && menuItemsToBeRendered.Count == 0)
            continue;

          // Skip duplicate separators
          if (menuItem.IsSeparator && (i + 1) < menuItems.Length - 1 && menuItems[i + 1].IsSeparator)
            continue;

          // Skip trailing separator
          if (menuItem.IsSeparator && i == menuItems.Length - 1)
            continue;

          menuItemsToBeRendered.Add(menuItem);
        }

        bool isFirstItem = true;
        foreach (var menuItem in menuItemsToBeRendered)
        {
          if (isFirstItem)
            isFirstItem = false;
          else
            script.AppendFormat(",").AppendLine();
          AppendMenuItem(renderingContext, script, menuItem, renderingContext.Control.MenuItems.IndexOf(menuItem));
        }
        script.Append("];").AppendLine(); // Close Array

        var loadMenuItemStatus = renderingContext.Control.LoadMenuItemStatus;
        script.Append("    const loadMenuItemStatus = ").Append(string.IsNullOrEmpty(loadMenuItemStatus) ? "null" : loadMenuItemStatus).Append(";").AppendLine();
        script.Append("    DropDownMenu.LoadFilteredMenuItems (unfilteredMenuItems, loadMenuItemStatus, onSuccess, onError);").AppendLine();
        script.Append("  },"); // Close function body
        script.Append(GetResourcesAsJson(renderingContext));
        script.Append(" )"); // Close new MenuInfo
        script.Append(" );"); // Close AddMenuInfo
        renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock(
            renderingContext.Control,
            typeof(DropDownMenuRenderer),
            key,
            script.ToString());
      }
    }

    private void AppendMenuItem (DropDownMenuRenderingContext renderingContext, StringBuilder stringBuilder, WebMenuItem menuItem, int menuItemIndex)
    {
      string href = "null";
      string target = "null";

      bool isCommandEnabled = true;
      var diagnosticMetadataTriggersNavigation = false;
      var diagnosticMetadataTriggersPostBack = false;
      if (menuItem.Command != null)
      {
        bool isActive = menuItem.Command.Show == CommandShow.Always
                        || renderingContext.Control.IsReadOnly && menuItem.Command.Show == CommandShow.ReadOnly
                        || !renderingContext.Control.IsReadOnly && menuItem.Command.Show == CommandShow.EditMode;

        isCommandEnabled = isActive && menuItem.Command.Type != CommandType.None;
        if (isCommandEnabled)
        {
          bool isPostBackCommand = menuItem.Command.Type == CommandType.Event
                                   || menuItem.Command.Type == CommandType.WxeFunction;
          if (isPostBackCommand)
          {
            // Clientside script creates an anchor with href="#" and onclick=function
            // The function will be executed using eval(...) and must therefor not end with a return statement.
            string argument = menuItemIndex.ToString();
            href = renderingContext.Control.Page!.ClientScript.GetPostBackClientHyperlink(renderingContext.Control, argument);
            href = ScriptUtility.EscapeClientScript(href);
            href = "'" + href + "'";

            diagnosticMetadataTriggersPostBack = true;
          }
          else if (menuItem.Command.Type == CommandType.Href)
          {
            href = menuItem.Command.HrefCommand.FormatHref(menuItemIndex.ToString(), menuItem.ItemID);
            href = "'" + renderingContext.Control.ResolveClientUrl(href) + "'";
            target = "'" + menuItem.Command.HrefCommand.Target + "'";

            diagnosticMetadataTriggersNavigation = true;
          }
        }
      }

      bool showIcon = menuItem.Style == WebMenuItemStyle.Icon || menuItem.Style == WebMenuItemStyle.IconAndText;
      bool showText = menuItem.Style == WebMenuItemStyle.Text || menuItem.Style == WebMenuItemStyle.IconAndText;

      string icon = GetIconUrl(renderingContext, menuItem, showIcon);
      string disabledIcon = GetDisabledIconUrl(renderingContext, menuItem, showIcon);
      string text = showText
          ? "'" + ScriptUtility.EscapeClientScript(menuItem.Text) + "'"
          : "null";
      WebString diagnosticMetadataText = showText ? menuItem.Text : default;

      bool isDisabled = !menuItem.EvaluateEnabled() || !isCommandEnabled;
      var fallbackNavigationUrl = ScriptUtility.EscapeClientScript(_fallbackNavigationUrlProvider.GetURL());
      stringBuilder.AppendFormat(
          "\t\tnew DropDownMenu_ItemInfo ({0}, '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, '{9}', ",
          string.IsNullOrEmpty(menuItem.ItemID) ? "null" : "'" + menuItem.ItemID + "'",
          ScriptUtility.EscapeClientScript(menuItem.Category),
          text,
          icon,
          disabledIcon,
          (int)menuItem.RequiredSelection,
          isDisabled ? "true" : "false",
          href,
          target,
          fallbackNavigationUrl);

      if (IsDiagnosticMetadataRenderingEnabled)
      {
        var htmlID = renderingContext.Control.ClientID + "_" + menuItemIndex;
        var diagnosticMetadataDictionary = new Dictionary<string, string?>();
        diagnosticMetadataDictionary.Add(HtmlTextWriterAttribute.Id.ToString(), htmlID);
        diagnosticMetadataDictionary.Add(DiagnosticMetadataAttributes.ItemID, menuItem.ItemID);
        diagnosticMetadataDictionary.Add(DiagnosticMetadataAttributes.Content, HtmlUtility.ExtractPlainText(diagnosticMetadataText).GetValue());

        stringBuilder.WriteDictionaryAsJson(diagnosticMetadataDictionary);

        stringBuilder.Append(", ");

        var diagnosticMetadataDictionaryForCommand = new Dictionary<string, string?>();
        diagnosticMetadataDictionaryForCommand.Add(DiagnosticMetadataAttributes.IsDisabled, isDisabled.ToString().ToLower());
        diagnosticMetadataDictionaryForCommand.Add(DiagnosticMetadataAttributes.TriggersNavigation, diagnosticMetadataTriggersNavigation.ToString().ToLower());
        diagnosticMetadataDictionaryForCommand.Add(DiagnosticMetadataAttributes.TriggersPostBack, diagnosticMetadataTriggersPostBack.ToString().ToLower());
        stringBuilder.WriteDictionaryAsJson(diagnosticMetadataDictionaryForCommand);
      }
      else
      {
        stringBuilder.Append("null, null");
      }

      stringBuilder.Append(")");
    }

    protected virtual string GetIconUrl (DropDownMenuRenderingContext renderingContext, WebMenuItem menuItem, bool showIcon)
    {
      string icon = "null";

      if (showIcon && menuItem.Icon.HasRenderingInformation)
      {
        string url = menuItem.Icon.Url;
        icon = "'" + renderingContext.Control.ResolveClientUrl(url) + "'";
      }
      return icon;
    }

    protected virtual string GetDisabledIconUrl (DropDownMenuRenderingContext renderingContext, WebMenuItem menuItem, bool showIcon)
    {
      string disabledIcon = "null";
      if (showIcon && menuItem.DisabledIcon.HasRenderingInformation)
      {
        string url = menuItem.DisabledIcon.Url;
        disabledIcon = "'" + renderingContext.Control.ResolveClientUrl(url) + "'";
      }
      return disabledIcon;
    }

    private string GetResourcesAsJson (DropDownMenuRenderingContext renderingContext)
    {
      var resourceManager = GetResourceManager(renderingContext);
      var jsonBuilder = new StringBuilder(1000);

      jsonBuilder.Append("{ ");
      jsonBuilder.Append("LoadFailedErrorMessage : ");
      AppendStringValueOrNullToScript(jsonBuilder, resourceManager.GetText(ResourceIdentifier.LoadFailedErrorMessage));
      jsonBuilder.Append(", ");
      jsonBuilder.Append("LoadingStatusMessage : ");
      AppendStringValueOrNullToScript(jsonBuilder, resourceManager.GetText(ResourceIdentifier.LoadingStatusMessage));
      jsonBuilder.Append(", ");
      jsonBuilder.Append(" }");

      return jsonBuilder.ToString();
    }

    protected virtual IResourceManager GetResourceManager (DropDownMenuRenderingContext renderingContext)
    {
      return GetResourceManager(typeof(ResourceIdentifier), renderingContext.Control.GetResourceManager());
    }

    protected string CssClassBase
    {
      get { return "DropDownMenuContainer"; }
    }

    protected string CssClassHead
    {
      get { return "DropDownMenuSelect"; }
    }

    protected string CssClassList
    {
      get { return "DropDownMenuOptions"; }
    }

    protected string CssClassDisabled
    {
      get { return CssClassDefinition.Disabled; }
    }
  }
}
