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
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls.ListMenuImplementation.Rendering
{
  /// <summary>
  /// Responsible for rendering a <see cref="ListMenu"/> control in standard mode.
  /// <seealso cref="IListMenu"/>
  /// </summary>
  [ImplementationFor(typeof(IListMenuRenderer), Lifetime = LifetimeKind.Singleton)]
  public class ListMenuRenderer : RendererBase<IListMenu>, IListMenuRenderer
  {
    private readonly IFallbackNavigationUrlProvider _fallbackNavigationUrlProvider;
    private const string c_whiteSpace = "&nbsp;";

    public ListMenuRenderer (
        IResourceUrlFactory resourceUrlFactory,
        IGlobalizationService globalizationService,
        IRenderingFeatures renderingFeatures,
        IFallbackNavigationUrlProvider fallbackNavigationUrlProvider)
        : base(resourceUrlFactory, globalizationService, renderingFeatures)
    {
      ArgumentUtility.CheckNotNull("fallbackNavigationUrlProvider", fallbackNavigationUrlProvider);
      _fallbackNavigationUrlProvider = fallbackNavigationUrlProvider;
    }

    public string CssClassListMenu
    {
      get { return "listMenu"; }
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterWebClientScriptInclude();
      htmlHeadAppender.RegisterCommonStyleSheet();

      string styleSheetKey = typeof(ListMenuRenderer).GetFullNameChecked() + "_Style";
      var styleSheetUrl = ResourceUrlFactory.CreateThemedResourceUrl(typeof(ListMenuRenderer), ResourceType.Html, "ListMenu.css");
      htmlHeadAppender.RegisterStylesheetLink(styleSheetKey, styleSheetUrl, HtmlHeadAppender.Priority.Library);
    }

    public void Render (ListMenuRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull("renderingContext", renderingContext);

      RegisterMenuItems(renderingContext);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassListMenu);
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Region);

      if (IsDiagnosticMetadataRenderingEnabled)
        AddDiagnosticMetadataAttributes(renderingContext);

      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Div);

      var headingID = renderingContext.Control.ClientID + "_Heading";
      var hasHeading = !renderingContext.Control.Heading.IsEmpty;

      if (hasHeading)
      {
        var heading = renderingContext.Control.Heading;
        var tag = GetTagFromHeadingLevel(renderingContext.Control.HeadingLevel);
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, headingID);
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassDefinition.ScreenReaderText);
        renderingContext.Writer.RenderBeginTag(tag);
        heading.WriteTo(renderingContext.Writer);
        renderingContext.Writer.RenderEndTag();
      }

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.Menu);
      if(hasHeading)
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.AriaLabelledBy, headingID);

      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Table);

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.None);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Tbody);

      var isFirst = true;
      var groupedMenuItems = GetVisibleMenuItemsInGroups(renderingContext);
      foreach (var menuItemsInGroup in groupedMenuItems)
      {
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.None);
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Tr);
        renderingContext.Writer.AddStyleAttribute(HtmlTextWriterStyle.Width, "100%");
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, "listMenuRow");
        renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.None);
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Td);

        foreach (var menuItem in menuItemsInGroup)
        {
          RenderListMenuItem(renderingContext, menuItem, renderingContext.Control.MenuItems.IndexOf(menuItem), isFirst);
          isFirst = false;
        }

        renderingContext.Writer.RenderEndTag();
        renderingContext.Writer.RenderEndTag();
      }
      //WebMenuItem[] groupedListMenuItems = renderingContext.Control.MenuItems.GroupMenuItems (false).Where (mi=>mi.EvaluateVisible()).ToArray();
      //bool isFirstItem = true;
      //for (int idxItems = 0; idxItems < groupedListMenuItems.Length; idxItems++)
      //{
      //  WebMenuItem currentItem = groupedListMenuItems[idxItems];

      //  bool isLastItem = (idxItems == (groupedListMenuItems.Length - 1));
      //  bool isFirstCategoryItem = (isFirstItem || (groupedListMenuItems[idxItems - 1].Category != currentItem.Category));
      //  bool isLastCategoryItem = (isLastItem || (groupedListMenuItems[idxItems + 1].Category != currentItem.Category));
      //  bool hasAlwaysLineBreaks = (renderingContext.Control.LineBreaks == ListMenuLineBreaks.All);
      //  bool hasCategoryLineBreaks = (renderingContext.Control.LineBreaks == ListMenuLineBreaks.BetweenGroups);

      //  if (hasAlwaysLineBreaks || (hasCategoryLineBreaks && isFirstCategoryItem) || isFirstItem)
      //  {
      //    renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      //    renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "100%");
      //    renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, "listMenuRow");
      //    renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td);
      //  }
      //  RenderListMenuItem (renderingContext, currentItem, renderingContext.Control.MenuItems.IndexOf (currentItem));
      //  if (hasAlwaysLineBreaks || (hasCategoryLineBreaks && isLastCategoryItem) || isLastItem)
      //  {
      //    renderingContext.Writer.RenderEndTag ();
      //    renderingContext.Writer.RenderEndTag ();
      //  }

      //  if (isFirstItem)
      //    isFirstItem = false;
      //}
      renderingContext.Writer.RenderEndTag(); //tbody
      renderingContext.Writer.RenderEndTag(); //table
      renderingContext.Writer.RenderEndTag(); //div
    }

    protected override void AddDiagnosticMetadataAttributes (RenderingContext<IListMenu> renderingContext)
    {
      base.AddDiagnosticMetadataAttributes(renderingContext);

      renderingContext.Writer.AddAttribute(DiagnosticMetadataAttributes.IsDisabled, (!renderingContext.Control.Enabled).ToString().ToLower());
    }

    private HtmlTextWriterTag GetTagFromHeadingLevel (HeadingLevel? headingLevel) =>
        headingLevel switch
        {
            HeadingLevel.H1 => HtmlTextWriterTag.H1,
            HeadingLevel.H2 => HtmlTextWriterTag.H2,
            HeadingLevel.H3 => HtmlTextWriterTag.H3,
            HeadingLevel.H4 => HtmlTextWriterTag.H4,
            HeadingLevel.H5 => HtmlTextWriterTag.H5,
            _ => HtmlTextWriterTag.Span,
        };

    private IEnumerable<IEnumerable<WebMenuItem>> GetVisibleMenuItemsInGroups (ListMenuRenderingContext renderingContext)
    {
      var menuItems = renderingContext.Control.MenuItems.GroupMenuItems(false).Where(mi => mi.EvaluateVisible());
      var lineBreaks = renderingContext.Control.LineBreaks;
      switch (lineBreaks)
      {
        case ListMenuLineBreaks.All:
          return menuItems.Select(mi => new[] { mi }.AsEnumerable());

        case ListMenuLineBreaks.BetweenGroups:
          return menuItems.GroupBy(mi => mi.Category).Select(g => g.AsEnumerable());

        case ListMenuLineBreaks.None:
          return new[] { menuItems };

        default:
          throw new InvalidOperationException(string.Format("'{0}' is not a valid option for ListMenuLineBreaks enum.", lineBreaks));
      }
    }

    private void RenderListMenuItem (ListMenuRenderingContext renderingContext, WebMenuItem menuItem, int index, bool isFirst)
    {
      bool showIcon = menuItem.Style == WebMenuItemStyle.Icon || menuItem.Style == WebMenuItemStyle.IconAndText;
      bool showText = menuItem.Style == WebMenuItemStyle.Text || menuItem.Style == WebMenuItemStyle.IconAndText;

      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Id, GetMenuItemClientID(renderingContext, index));
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute.Class, "listMenuItem");
      renderingContext.Writer.AddAttribute(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.None);
      renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);

      var attributes = new NameValueCollection();
      attributes.Add(HtmlTextWriterAttribute2.Role, HtmlRoleAttributeValue.MenuItem);
      if (renderingContext.Control.Enabled)
        attributes.Add("tabindex", isFirst ? "0" : "-1");

      var command = !menuItem.IsDisabled
          ? Assertion.IsNotNull(menuItem.Command, "Command must not be null for an enabled menu item.")
          : new Command(CommandType.None) { OwnerControl = menuItem.OwnerControl };

      if (string.IsNullOrEmpty(command.ItemID))
        command.ItemID = "MenuItem_" + index + "_Command";

      command.RenderBegin(
          renderingContext.Writer,
          RenderingFeatures,
          postBackEvent: "",
          parameters: new[] { index.ToString() },
          onClick: null,
          securableObject: null,
          additionalUrlParameters: new NameValueCollection(),
          includeNavigationUrlParameters: true,
          style: new Style(),
          attributes: attributes);

      if (showIcon && menuItem.Icon.HasRenderingInformation)
      {
        menuItem.Icon.Render(renderingContext.Writer, renderingContext.Control);
      }

      if (showText)
      {
        renderingContext.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
        menuItem.Text.WriteTo(renderingContext.Writer);
        renderingContext.Writer.RenderEndTag();
      }

      command.RenderEnd(renderingContext.Writer);
      renderingContext.Writer.RenderEndTag();
    }

    private void RegisterMenuItems (ListMenuRenderingContext renderingContext)
    {
      if (!renderingContext.Control.HasClientScript)
        return;

      WebMenuItem[] groupedListMenuItems = renderingContext.Control.MenuItems.GroupMenuItems(false);

      string key = renderingContext.Control.UniqueID + "_MenuItems";
      if (!renderingContext.Control.Page!.ClientScript.IsStartupScriptRegistered(typeof(ListMenuRenderer), key))
      {
        StringBuilder script = new StringBuilder();
        script.AppendFormat("ListMenu.Initialize ('#{0}');", renderingContext.Control.ClientID).AppendLine();
        script.AppendFormat("ListMenu.AddMenuInfo ('#{0}', \r\n\t", renderingContext.Control.ClientID);
        script.AppendFormat("new ListMenu_MenuInfo ('{0}', new Array (\r\n", renderingContext.Control.ClientID);
        bool isFirstItemInGroup = true;

        for (int idxItems = 0; idxItems < groupedListMenuItems.Length; idxItems++)
        {
          WebMenuItem currentItem = groupedListMenuItems[idxItems];
          if (!currentItem.EvaluateVisible())
            continue;

          if (isFirstItemInGroup)
            isFirstItemInGroup = false;
          else
            script.AppendFormat(",\r\n");
          AppendListMenuItem(renderingContext, script, currentItem);
        }
        script.Append(" )"); // Close Array
        script.Append(" )"); // Close new MenuInfo
        script.Append(" );\r\n"); // Close AddMenuInfo

        script.Append(renderingContext.Control.GetUpdateScriptReference("null"));

        renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock(
            renderingContext.Control,
            typeof(ListMenuRenderer),
            key,
            script.ToString());
      }
    }

    private void AppendListMenuItem (ListMenuRenderingContext renderingContext, StringBuilder stringBuilder, WebMenuItem menuItem)
    {
      int menuItemIndex = renderingContext.Control.MenuItems.IndexOf(menuItem);
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
      string icon = "null";
      if (showIcon && menuItem.Icon.HasRenderingInformation)
        icon = "'" + renderingContext.Control.ResolveClientUrl(menuItem.Icon.Url) + "'";
      string disabledIcon = "null";
      if (showIcon && menuItem.DisabledIcon.HasRenderingInformation)
        disabledIcon = "'" + renderingContext.Control.ResolveClientUrl(menuItem.DisabledIcon.Url) + "'";
      string text = showText
          ? "'" + ScriptUtility.EscapeClientScript(menuItem.Text) + "'"
          : "null";

      bool isDisabled = !renderingContext.Control.Enabled
                        || !menuItem.EvaluateEnabled()
                        || !isCommandEnabled;
      var fallbackNavigationUrl = ScriptUtility.EscapeClientScript(_fallbackNavigationUrlProvider.GetURL());
      stringBuilder.AppendFormat(
          "\t\tnew ListMenuItemInfo ('{0}', '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, '{9}', ",
          GetMenuItemClientID(renderingContext, menuItemIndex),
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
        var diagnosticMetadataDictionary = new Dictionary<string, string?>();

        if (!string.IsNullOrEmpty(menuItem.ItemID))
          diagnosticMetadataDictionary.Add(DiagnosticMetadataAttributes.ItemID, menuItem.ItemID);

        if (!menuItem.Text.IsEmpty)
          diagnosticMetadataDictionary.Add(DiagnosticMetadataAttributes.Content, HtmlUtility.ExtractPlainText(menuItem.Text).GetValue());

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

    private string GetMenuItemClientID (ListMenuRenderingContext renderingContext, int menuItemIndex)
    {
      return renderingContext.Control.ClientID + "_" + menuItemIndex;
    }
  }
}
