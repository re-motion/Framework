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
using System.Web.UI;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.DropDownMenuImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering <see cref="DropDownMenu"/> controls in standard mode.
  /// <seealso cref="IDropDownMenu"/>
  /// </summary>
  [ImplementationFor (typeof (IDropDownMenuRenderer), Lifetime = LifetimeKind.Singleton)]
  public class DropDownMenuRenderer : RendererBase<IDropDownMenu>, IDropDownMenuRenderer
  {
    private const string c_whiteSpace = "&nbsp;";

    public DropDownMenuRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures)
        : base (resourceUrlFactory, globalizationService, renderingFeatures)
    {
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude();
      htmlHeadAppender.RegisterJQueryIFrameShimJavaScriptInclude();

      string scriptKey = typeof (DropDownMenuRenderer).FullName + "_Script";
      var scriptUrl = ResourceUrlFactory.CreateResourceUrl (typeof (DropDownMenuRenderer), ResourceType.Html, "DropDownMenu.js");
      htmlHeadAppender.RegisterJavaScriptInclude (scriptKey, scriptUrl);

      string styleSheetKey = typeof (DropDownMenuRenderer).FullName + "_Style";
      var styleSheetUrl = ResourceUrlFactory.CreateThemedResourceUrl (typeof (DropDownMenuRenderer), ResourceType.Html, "DropDownMenu.css");
      htmlHeadAppender.RegisterStylesheetLink (styleSheetKey, styleSheetUrl, HtmlHeadAppender.Priority.Library);
    }

    public void Render (DropDownMenuRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      RegisterMenuItems (renderingContext);

      RegisterEventHandlerScripts (renderingContext);

      AddAttributesToRender (renderingContext);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      RenderHead (renderingContext);

      renderingContext.Writer.RenderEndTag();
    }

    public void RenderAsContextMenu (DropDownMenuRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      RegisterMenuItems (renderingContext);

      RegisterEventHandlerScripts (renderingContext);
    }

    private void RenderHead (DropDownMenuRenderingContext renderingContext)
    {
      string cssClass = CssClassHead;
      if (!renderingContext.Control.Enabled)
        cssClass += " " + CssClassDisabled;
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, cssClass);
      if (HasCustomTitle (renderingContext) && HasTitleText (renderingContext))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Title, renderingContext.Control.TitleText);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span);

      if (HasCustomTitle (renderingContext))
        renderingContext.Control.RenderHeadTitleMethod (renderingContext.Writer);
      else if (HasDefaultTitle (renderingContext))
        RenderDefaultTitle (renderingContext);

      RenderDropdownButton (renderingContext);

      renderingContext.Writer.RenderEndTag();
    }

    private void RenderDefaultTitle (DropDownMenuRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassDropDownLabel);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID + "_DropDownMenuLabel");
      if (renderingContext.Control.Enabled)
      {
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, "return false;");
      }
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.A);

      if (HasTitleIcon (renderingContext))
        renderingContext.Control.TitleIcon.Render (renderingContext.Writer, renderingContext.Control);

      if (HasTitleText (renderingContext))
      {
        renderingContext.Writer.Write (renderingContext.Control.TitleText);
        renderingContext.Writer.Write (c_whiteSpace);
      }
      renderingContext.Writer.RenderEndTag();
    }

    private void RenderDropdownButton (DropDownMenuRenderingContext renderingContext)
    {
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassDropDownButton);
      if (!HasDefaultTitle (renderingContext) || HasCustomTitle (renderingContext))
      {
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID + "_DropDownMenuButton");
        if (renderingContext.Control.Enabled)
        {
          renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
          renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Onclick, "return false;");
        }
      }

      if (HasCustomTitle (renderingContext) && HasTitleText (renderingContext))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Title, renderingContext.Control.TitleText);

      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.A);

      IconInfo.CreateSpacer (ResourceUrlFactory).Render (renderingContext.Writer, renderingContext.Control);

      renderingContext.Writer.RenderEndTag();
    }

    private bool HasCustomTitle (DropDownMenuRenderingContext renderingContext)
    {
      return renderingContext.Control.RenderHeadTitleMethod != null;
    }

    private bool HasDefaultTitle (DropDownMenuRenderingContext renderingContext)
    {
      return HasTitleIcon (renderingContext) || HasTitleText (renderingContext);
    }

    private bool HasTitleText (DropDownMenuRenderingContext renderingContext)
    {
      return !string.IsNullOrEmpty (renderingContext.Control.TitleText);
    }

    private bool HasTitleIcon (DropDownMenuRenderingContext renderingContext)
    {
      return renderingContext.Control.TitleIcon != null && !string.IsNullOrEmpty (renderingContext.Control.TitleIcon.Url);
    }

    protected string CssClassDropDownLabel
    {
      get { return "DropDownMenuLabel"; }
    }

    protected string CssClassDropDownButton
    {
      get { return "DropDownMenuButton"; }
    }

    private void AddAttributesToRender (DropDownMenuRenderingContext renderingContext)
    {
      AddStandardAttributesToRender (renderingContext);
      if (string.IsNullOrEmpty (renderingContext.Control.CssClass) && string.IsNullOrEmpty (renderingContext.Control.Attributes["class"]))
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassBase);

      if (renderingContext.Control.ControlStyle.Width.IsEmpty)
      {
        if (!renderingContext.Control.Width.IsEmpty)
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, renderingContext.Control.Width.ToString());
      }
    }

    protected override void AddDiagnosticMetadataAttributes (RenderingContext<IDropDownMenu> renderingContext)
    {
      base.AddDiagnosticMetadataAttributes (renderingContext);

      renderingContext.Writer.AddAttribute (DiagnosticMetadataAttributes.Content, HtmlUtility.StripHtmlTags (renderingContext.Control.TitleText));
    }

    private void RegisterEventHandlerScripts (DropDownMenuRenderingContext renderingContext)
    {
      if (!renderingContext.Control.Enabled)
        return;

      string key = renderingContext.Control.ClientID + "_KeyDownEventHandlerBindScript";
      string getSelectionCount = (string.IsNullOrEmpty (renderingContext.Control.GetSelectionCount)
          ? "null"
          : renderingContext.Control.GetSelectionCount);
      string script = string.Format (
          "$('#{0}').keydown( function(event){{ DropDownMenu_OnKeyDown(event, document.getElementById('{0}'), {1}); }} );",
          renderingContext.Control.ClientID,
          getSelectionCount);

      renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (renderingContext.Control, typeof (ClientScriptBehavior), key, script);

      if (renderingContext.Control.Enabled && renderingContext.Control.Visible && renderingContext.Control.Mode == MenuMode.DropDownMenu)
      {
        key = renderingContext.Control.ClientID + "_ClickEventHandlerBindScript";
        string elementReference = string.Format ("$('#{0}')", renderingContext.Control.ClientID);
        string menuIDReference = string.Format ("'{0}'", renderingContext.Control.ClientID);
        script = renderingContext.Control.GetBindOpenEventScript (elementReference, menuIDReference, false);
        renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (renderingContext.Control, typeof (ClientScriptBehavior), key, script);
      }
    }

    private void RegisterMenuItems (DropDownMenuRenderingContext renderingContext)
    {
      string key = renderingContext.Control.UniqueID;
      if (renderingContext.Control.Enabled
          && !renderingContext.Control.Page.ClientScript.IsStartupScriptRegistered (typeof (DropDownMenuRenderer), key))
      {
        StringBuilder script = new StringBuilder();
        script.Append ("DropDownMenu_AddMenuInfo" + " (\r\n\t");
        script.AppendFormat ("new DropDownMenu_MenuInfo ('{0}', new Array (\r\n", renderingContext.Control.ClientID);
        bool isFirstItem = true;

        WebMenuItem[] menuItems;
        if (renderingContext.Control.EnableGrouping)
          menuItems = renderingContext.Control.MenuItems.GroupMenuItems (true);
        else
          menuItems = renderingContext.Control.MenuItems.ToArray();

        string category = null;
        bool isCategoryVisible = false;
        for (int i = 0; i < menuItems.Length; i++)
        {
          WebMenuItem menuItem = menuItems[i];
          if (renderingContext.Control.EnableGrouping && category != menuItem.Category)
          {
            category = menuItem.Category;
            isCategoryVisible = false;
          }
          if (!menuItem.EvaluateVisible())
            continue;
          if (renderingContext.Control.EnableGrouping && menuItem.IsSeparator && !isCategoryVisible)
            continue;
          if (renderingContext.Control.EnableGrouping)
            isCategoryVisible = true;
          if (isFirstItem)
            isFirstItem = false;
          else
            script.AppendFormat (",\r\n");
          AppendMenuItem (renderingContext, script, menuItem, renderingContext.Control.MenuItems.IndexOf (menuItem));
        }
        script.Append (" )"); // Close Array
        script.Append (" )"); // Close new MenuInfo
        script.Append (" );"); // Close AddMenuInfo
        renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (
            renderingContext.Control,
            typeof (DropDownMenuRenderer),
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
            string argument = menuItemIndex.ToString();
            href = renderingContext.Control.Page.ClientScript.GetPostBackClientHyperlink (renderingContext.Control, argument);
            href = ScriptUtility.EscapeClientScript (href);
            href = "'" + href + "'";

            diagnosticMetadataTriggersPostBack = true;
          }
          else if (menuItem.Command.Type == CommandType.Href)
          {
            href = menuItem.Command.HrefCommand.FormatHref (menuItemIndex.ToString(), menuItem.ItemID);
            href = "'" + renderingContext.Control.ResolveClientUrl (href) + "'";
            target = "'" + menuItem.Command.HrefCommand.Target + "'";

            diagnosticMetadataTriggersNavigation = true;
          }
        }
      }

      bool showIcon = menuItem.Style == WebMenuItemStyle.Icon || menuItem.Style == WebMenuItemStyle.IconAndText;
      bool showText = menuItem.Style == WebMenuItemStyle.Text || menuItem.Style == WebMenuItemStyle.IconAndText;

      string icon = GetIconUrl (renderingContext, menuItem, showIcon);
      string disabledIcon = GetDisabledIconUrl (renderingContext, menuItem, showIcon);
      string text = showText ? "'" + menuItem.Text + "'" : "null";
      string diagnosticMetadataText = showText ? menuItem.Text : "";

      var diagnosticMetadataJson = "null";
      if (IsDiagnosticMetadataRenderingEnabled)
      {
        var htmlID = renderingContext.Control.ClientID + "_" + menuItemIndex;
        // Note: the output of diagnosticMetadataText is enclosed by single quotes, as it may contain double quotes.
        diagnosticMetadataJson = string.Format (
            "{{\"{0}\":\"{1}\", \"{2}\":\"{3}\", \"{4}\":\"{5}\", \"{6}\":\"{7}\", \"{8}\":'{9}'}}",
            HtmlTextWriterAttribute.Id,
            htmlID,
            DiagnosticMetadataAttributes.TriggersNavigation,
            diagnosticMetadataTriggersNavigation.ToString().ToLower(),
            DiagnosticMetadataAttributes.TriggersPostBack,
            diagnosticMetadataTriggersPostBack.ToString().ToLower(),
            DiagnosticMetadataAttributes.ItemID,
            menuItem.ItemID,
            DiagnosticMetadataAttributes.Content,
            HtmlUtility.StripHtmlTags (diagnosticMetadataText ?? ""));
      }

      bool isDisabled = !menuItem.EvaluateEnabled() || !isCommandEnabled;
      stringBuilder.AppendFormat (
          "\t\tnew DropDownMenu_ItemInfo ('{0}', '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9})",
          menuItemIndex,
          menuItem.Category,
          text,
          icon,
          disabledIcon,
          (int) menuItem.RequiredSelection,
          isDisabled ? "true" : "false",
          href,
          target,
          diagnosticMetadataJson);
    }

    protected virtual string GetIconUrl (DropDownMenuRenderingContext renderingContext, WebMenuItem menuItem, bool showIcon)
    {
      string icon = "null";

      if (showIcon && menuItem.Icon.HasRenderingInformation)
      {
        string url = menuItem.Icon.Url;
        icon = "'" + renderingContext.Control.ResolveClientUrl (url) + "'";
      }
      return icon;
    }

    protected virtual string GetDisabledIconUrl (DropDownMenuRenderingContext renderingContext, WebMenuItem menuItem, bool showIcon)
    {
      string disabledIcon = "null";
      if (showIcon && menuItem.DisabledIcon.HasRenderingInformation)
      {
        string url = menuItem.DisabledIcon.Url;
        disabledIcon = "'" + renderingContext.Control.ResolveClientUrl (url) + "'";
      }
      return disabledIcon;
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
      get { return "disabled"; }
    }
  }
}