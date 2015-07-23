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
using Remotion.Utilities;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;
using Remotion.Web.UI.Controls.DropDownMenuImplementation.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.Web.Legacy.UI.Controls.Rendering
{
  /// <summary>
  /// Responsible for rendering a <see cref="DropDownMenu"/> control in quirks mode.
  /// <seealso cref="IDropDownMenu"/>
  /// </summary>
  public class DropDownMenuQuirksModeRenderer : QuirksModeRendererBase<IDropDownMenu>, IDropDownMenuRenderer
  {
    private const string c_dropDownIcon = "DropDownMenuArrow.gif";

    public DropDownMenuQuirksModeRenderer (IResourceUrlFactory resourceUrlFactory) 
      : base(resourceUrlFactory)
    { 
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull ("htmlHeadAppender", htmlHeadAppender);

      htmlHeadAppender.RegisterUtilitiesJavaScriptInclude ();

      string key = typeof (DropDownMenuQuirksModeRenderer).FullName + "_Script";
      if (!htmlHeadAppender.IsRegistered (key))
      {
        var scriptUrl = ResourceUrlFactory.CreateResourceUrl (typeof (DropDownMenuQuirksModeRenderer), ResourceType.Html, "DropDownMenu.js");
        htmlHeadAppender.RegisterJavaScriptInclude (key, scriptUrl);
      }

      key = typeof (DropDownMenuQuirksModeRenderer).FullName + "_Style";
      if (!htmlHeadAppender.IsRegistered (key))
      {
        var styleUrl = ResourceUrlFactory.CreateResourceUrl (typeof (DropDownMenuQuirksModeRenderer), ResourceType.Html, "DropDownMenu.css");
        htmlHeadAppender.RegisterStylesheetLink (key, styleUrl, HtmlHeadAppender.Priority.Library);
      }
    }

    public void RenderAsContextMenu (DropDownMenuRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      RegisterMenuItems (renderingContext);

      RegisterEventHandlerScripts (renderingContext);
    }

    public void Render (DropDownMenuRenderingContext renderingContext)
    {
      ArgumentUtility.CheckNotNull ("renderingContext", renderingContext);

      RegisterMenuItems (renderingContext);

      RegisterEventHandlerScripts (renderingContext);

      AddStandardAttributesToRender (renderingContext);
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Display, "inline-block");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div);

      //  Menu-Div filling the control's div is required to apply internal css attributes
      //  for position, width and height. This allows the Head and th popup-div to align themselves
      renderingContext.Writer.AddStyleAttribute ("position", "relative");
      renderingContext.Writer.AddAttribute ("id", renderingContext.Control.MenuHeadClientID);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Menu-Div

      RenderHead (renderingContext);

      renderingContext.Writer.RenderEndTag (); // End Menu-Div
      renderingContext.Writer.RenderEndTag (); // End outer div
    }

    private void RenderHead (DropDownMenuRenderingContext renderingContext)
    {
      //  Head-Div is used to group the title and the button, providing a single point of reference
      //  for the popup-div.
      renderingContext.Writer.AddStyleAttribute ("position", "relative");
      renderingContext.Writer.AddAttribute ("id", renderingContext.Control.ClientID + "_HeadDiv");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHead);
      renderingContext.Writer.AddAttribute ("OnMouseOver", "DropDownMenu_OnHeadMouseOver (this)");
      renderingContext.Writer.AddAttribute ("OnMouseOut", "DropDownMenu_OnHeadMouseOut (this)");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Div); // Begin Drop Down Head-Div

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellspacing, "0");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Cellpadding, "0");
      renderingContext.Writer.AddStyleAttribute ("display", "inline");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Table); // Begin Drop Down Button table
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);

      RenderHeadTitle (renderingContext);
      RenderHeadButton (renderingContext);

      renderingContext.Writer.RenderEndTag ();
      renderingContext.Writer.RenderEndTag (); // End Drop Down Button table

      renderingContext.Writer.RenderEndTag (); // End Drop Down Head-Div
    }

    private void RenderHeadTitle (DropDownMenuRenderingContext renderingContext)
    {
      bool hasHeadTitleContents = true;
      if (renderingContext.Control.RenderHeadTitleMethod == null)
      {
        bool hasTitleText = !string.IsNullOrEmpty (renderingContext.Control.TitleText);
        bool hasTitleIcon = renderingContext.Control.TitleIcon != null && !string.IsNullOrEmpty (renderingContext.Control.TitleIcon.Url);
        hasHeadTitleContents = hasTitleText || hasTitleIcon;

        if (hasHeadTitleContents)
        {
          renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1%"); //"100%");
          renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadTitle);
          renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

          if (renderingContext.Control.Enabled)
          {
            renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.A); // Begin title tag
          }
          else
          {
            renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Color, "GrayText");
            renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Span); // Begin title tag
          }
          RenderIcon (renderingContext, renderingContext.Control.TitleIcon);
          renderingContext.Writer.Write (renderingContext.Control.TitleText);
          renderingContext.Writer.RenderEndTag (); // End title tag

          renderingContext.Writer.RenderEndTag (); // End td
        }
      }
      else
        renderingContext.Control.RenderHeadTitleMethod (renderingContext.Writer);

      if (hasHeadTitleContents)
      {
        renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
        renderingContext.Writer.AddStyleAttribute ("padding-right", "0.3em");
        renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); //  Begin td
        renderingContext.Writer.RenderEndTag ();
      }
    }

    private void RenderIcon (DropDownMenuRenderingContext renderingContext, IconInfo icon)
    {
      if (icon == null || string.IsNullOrEmpty (icon.Url))
        return;

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Src, icon.Url);
      if (!icon.Width.IsEmpty && !icon.Height.IsEmpty)
      {
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Width, icon.Width.ToString ());
        renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Height, icon.Height.ToString ());
      }
      renderingContext.Writer.AddStyleAttribute ("vertical-align", "middle");
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
      renderingContext.Writer.AddStyleAttribute ("margin-right", "0.3em");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Img);
      renderingContext.Writer.RenderEndTag ();
    }

    private void RenderHeadButton (DropDownMenuRenderingContext renderingContext)
    {
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "0%");
      renderingContext.Writer.AddStyleAttribute ("text-align", "center");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Class, CssClassHeadButton);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Td); // Begin td

      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Id, renderingContext.Control.ClientID + "_DropDownMenuButton");
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.Width, "1em");
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Href, "#");
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.A); // Begin anchor

      renderingContext.Writer.AddStyleAttribute ("vertical-align", "middle");
      renderingContext.Writer.AddStyleAttribute (HtmlTextWriterStyle.BorderStyle, "none");
      string url = ResourceUrlFactory.CreateResourceUrl (typeof (DropDownMenuQuirksModeRenderer), ResourceType.Image, c_dropDownIcon).GetUrl();
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Src, url);
      renderingContext.Writer.AddAttribute (HtmlTextWriterAttribute.Alt, string.Empty);
      renderingContext.Writer.RenderBeginTag (HtmlTextWriterTag.Img);
      renderingContext.Writer.RenderEndTag (); // End img

      renderingContext.Writer.RenderEndTag (); // End anchor

      renderingContext.Writer.RenderEndTag (); // End td
    }

    private void RegisterEventHandlerScripts (DropDownMenuRenderingContext renderingContext)
    {
      string key = typeof (DropDownMenuQuirksModeRenderer).FullName + "_Startup";

      if (!renderingContext.Control.Page.ClientScript.IsStartupScriptRegistered (typeof (DropDownMenuQuirksModeRenderer), key))
      {
        string styleSheetUrl = 
            ResourceUrlFactory.CreateResourceUrl (typeof (DropDownMenuQuirksModeRenderer), ResourceType.Html, "DropDownMenu.css").GetUrl();
        string script = string.Format ("DropDownMenu_InitializeGlobals ('{0}');", styleSheetUrl);
        renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (renderingContext.Control, typeof (DropDownMenuQuirksModeRenderer), key, script);
      }

      if (renderingContext.Control.Enabled && renderingContext.Control.Visible && renderingContext.Control.Mode == MenuMode.DropDownMenu)
      {
        key = renderingContext.Control.ClientID + "_ClickEventHandlerBindScript";
        if (!renderingContext.Control.Page.ClientScript.IsStartupScriptRegistered (typeof (DropDownMenuQuirksModeRenderer), key))
        {
          string elementReference = string.Format ("document.getElementById('{0}')", renderingContext.Control.MenuHeadClientID);
          string menuIDReference = string.Format ("'{0}'", renderingContext.Control.ClientID);
          string script = renderingContext.Control.GetBindOpenEventScript (elementReference, menuIDReference, false);
          renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (renderingContext.Control, typeof (DropDownMenuQuirksModeRenderer), key, script);
        }
      }
    }

    private void RegisterMenuItems (DropDownMenuRenderingContext renderingContext)
    {
      string key = renderingContext.Control.UniqueID;
      if (renderingContext.Control.Enabled && !renderingContext.Control.Page.ClientScript.IsStartupScriptRegistered (typeof (DropDownMenuQuirksModeRenderer), key))
      {
        StringBuilder script = new StringBuilder ();
        script.Append ("DropDownMenu_AddMenuInfo" + " (\r\n\t");
        script.AppendFormat ("new DropDownMenu_MenuInfo ('{0}', new Array (\r\n", renderingContext.Control.ClientID);
        bool isFirstItem = true;

        WebMenuItem[] menuItems;
        if (renderingContext.Control.EnableGrouping)
          menuItems = renderingContext.Control.MenuItems.GroupMenuItems (true);
        else
          menuItems = renderingContext.Control.MenuItems.ToArray ();

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
          if (!menuItem.EvaluateVisible ())
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
        renderingContext.Control.Page.ClientScript.RegisterStartupScriptBlock (renderingContext.Control, typeof (DropDownMenuQuirksModeRenderer), key, script.ToString ());
      }
    }

    private void AppendMenuItem (DropDownMenuRenderingContext renderingContext, StringBuilder stringBuilder, WebMenuItem menuItem, int menuItemIndex)
    {
      string href = "null";
      string target = "null";

      bool isCommandEnabled = true;
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
            string argument = menuItemIndex.ToString ();
            href = renderingContext.Control.Page.ClientScript.GetPostBackClientHyperlink (renderingContext.Control, argument);
            href = ScriptUtility.EscapeClientScript (href);
            href = "'" + href + "'";
          }
          else if (menuItem.Command.Type == CommandType.Href)
          {
            href = menuItem.Command.HrefCommand.FormatHref (menuItemIndex.ToString (), menuItem.ItemID);
            href = "'" + renderingContext.Control.ResolveClientUrl (href) + "'";
            target = "'" + menuItem.Command.HrefCommand.Target + "'";
          }
        }
      }

      bool showIcon = menuItem.Style == WebMenuItemStyle.Icon || menuItem.Style == WebMenuItemStyle.IconAndText;
      bool showText = menuItem.Style == WebMenuItemStyle.Text || menuItem.Style == WebMenuItemStyle.IconAndText;

      string icon = GetIconUrl (renderingContext, menuItem, showIcon);
      string disabledIcon = GetDisabledIconUrl (renderingContext, menuItem, showIcon);
      string text = showText ? "'" + menuItem.Text + "'" : "null";

      bool isDisabled = !menuItem.EvaluateEnabled () || !isCommandEnabled;
      stringBuilder.AppendFormat (
          "\t\tnew DropDownMenu_ItemInfo ('{0}', '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8})",
          menuItemIndex,
          menuItem.Category,
          text,
          icon,
          disabledIcon,
          (int) menuItem.RequiredSelection,
          isDisabled ? "true" : "false",
          href,
          target);
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

    protected virtual string CssClassHead
    {
      get { return "dropDownMenuHead"; }
    }

    protected virtual string CssClassHeadFocus
    {
      get { return "dropDownMenuHeadFocus"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="DropDownMenu"/>'s title. </summary>
    /// <remarks> Class: <c></c> </remarks>
    protected virtual string CssClassHeadTitle
    {
      get { return "dropDownMenuHeadTitle"; }
    }

    protected virtual string CssClassHeadTitleFocus
    {
      get { return "dropDownMenuHeadTitleFocus"; }
    }

    protected virtual string CssClassHeadButton
    {
      get { return "dropDownMenuHeadButton"; }
    }

    protected virtual string CssClassMenuButtonFocus
    {
      get { return "dropDownMenuButtonFocus"; }
    }

    protected virtual string CssClassPopUp
    {
      get { return "dropDownMenuPopUp"; }
    }

    protected virtual string CssClassItem
    {
      get { return "dropDownMenuItem"; }
    }

    protected virtual string CssClassItemFocus
    {
      get { return "dropDownMenuItemFocus"; }
    }

    protected virtual string CssClassItemTextPane
    {
      get { return "dropDownMenuItemTextPane"; }
    }

    protected virtual string CssClassItemIconPane
    {
      get { return "dropDownMenuItemIconPane"; }
    }
  }
}