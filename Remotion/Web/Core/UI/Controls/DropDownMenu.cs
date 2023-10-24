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
using System.ComponentModel;
using System.Web.UI;
using Remotion.Globalization;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;
using Remotion.Web.UI.Controls.DropDownMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.Web.UI.Controls
{
  public class DropDownMenu : MenuBase, IDropDownMenu
  {
    #region Obsolete

    [Obsolete("This feature has removed. (Version 3.0.0)", true)]
    public void SetRenderHeadTitleMethodDelegate (Action<HtmlTextWriter> renderHeadTitleMethod)
    {
      throw new NotSupportedException("This feature has removed. (Version 3.0.0)");
    }

    #endregion

    private ButtonType _buttonType;
    private bool _showTitle = true;
    private WebString _titleText;
    private IconInfo? _titleIcon;
    private bool _enableGrouping = true;
    private bool _isBrowserCapableOfScripting;

    private string _getSelectionCount = "";
    private string _loadMenuItemStatus = "";

    public DropDownMenu (IControl? ownerControl, Type[] supportedMenuItemTypes)
      :base(ownerControl, supportedMenuItemTypes)
    {
      Mode = MenuMode.DropDownMenu;
    }

    public DropDownMenu (IControl? ownerControl)
        : this(ownerControl, new[] { typeof(WebMenuItem) })
    {
    }

    public DropDownMenu ()
        : this(null, new[] { typeof(WebMenuItem) })
    {
    }

    protected override void OnInit (EventArgs e)
    {
      var clientScriptBahavior = SafeServiceLocator.Current.GetInstance<IClientScriptBehavior>();
      _isBrowserCapableOfScripting = clientScriptBahavior.IsBrowserCapableOfScripting(Page!.Context, this);
      RegisterHtmlHeadContents(HtmlHeadAppender.Current);
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents(htmlHeadAppender);
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender(e);

      for (int i = 0; i < MenuItems.Count; i++)
      {
        WebMenuItem menuItem = MenuItems[i];
        if (menuItem.Command != null)
        {
          menuItem.Command.RegisterForSynchronousPostBackOnDemand(
              this, i.ToString(), string.Format("DropDownMenu '{0}', MenuItem '{1}'", ID, menuItem.ItemID));
        }
      }
    }

    public void RenderAsContextMenu (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      var renderer = CreateRenderer();
      renderer.RenderAsContextMenu(CreateRenderingContext(writer));
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      var renderer = CreateRenderer();
      renderer.Render(CreateRenderingContext(writer));
    }

    protected virtual IDropDownMenuRenderer CreateRenderer ()
    {
      return SafeServiceLocator.Current.GetInstance<IDropDownMenuRenderer>();
    }

    protected virtual DropDownMenuRenderingContext CreateRenderingContext (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      return new DropDownMenuRenderingContext(Page!.Context!, writer, this); // TODO RM-8118: Not null assertion
    }

    public string GetBindOpenEventScript (string elementReference, string menuIDReference, bool moveToMousePosition)
    {
      ArgumentUtility.CheckNotNullOrEmpty("elementReference", elementReference);
      ArgumentUtility.CheckNotNullOrEmpty("menuIDReference", menuIDReference);

      return string.Format(
          "DropDownMenu.BindOpenEvent({0}, {1}, '{2}', {3}, {4});",
          elementReference,
          menuIDReference,
          GetEventType(),
          string.IsNullOrEmpty(GetSelectionCount) ? "null" : GetSelectionCount,
          moveToMousePosition ? "true" : "false"
          );
    }

    private string GetEventType ()
    {
      switch (Mode)
      {
        case MenuMode.DropDownMenu:
          return  "click";
        case MenuMode.ContextMenu:
          return "contextmenu";
        default:
          throw new InvalidOperationException();
      }
    }

    IResourceManager IDropDownMenu.GetResourceManager ()
    {
      return GetResourceManager();
    }

    protected virtual IResourceManager GetResourceManager ()
    {
      return NullResourceManager.Instance;
    }

    /// <summary>
    /// Gets or sets the button type that determines how the <see cref="DropDownMenu"/>'s button is displayed on the page.
    /// </summary>
    [Description("Determines how the button is displayed on the page.")]
    [Category("Appearance")]
    [DefaultValue(ButtonType.Standard)]
    public ButtonType ButtonType
    {
      get { return _buttonType; }
      set { _buttonType = value; }
    }

    [Description("Set false to remove the title from the DropDownMenu's button when it is rendered.")]
    [DefaultValue(true)]
    public bool ShowTitle
    {
      get { return _showTitle; }
      set { _showTitle = value; }
    }

    public WebString TitleText
    {
      get { return _titleText; }
      set { _titleText = value; }
    }

    string IDropDownMenu.MenuHeadClientID
    {
      get { return ClientID + "_MenuDiv"; }
    }

    public IconInfo? TitleIcon
    {
      get { return _titleIcon; }
      set { _titleIcon = value; }
    }

    public bool IsBrowserCapableOfScripting
    {
      get { return _isBrowserCapableOfScripting; }
    }

    [DefaultValue(true)]
    public bool EnableGrouping
    {
      get { return _enableGrouping; }
      set { _enableGrouping = value; }
    }

    public MenuMode Mode { get; set; }

    [DefaultValue("")]
    public string GetSelectionCount
    {
      get { return _getSelectionCount; }
      set { _getSelectionCount = StringUtility.NullToEmpty(value); }
    }

    public string LoadMenuItemStatus
    {
      get { return _loadMenuItemStatus; }
    }

    public void SetLoadMenuItemStatus (string value)
    {
      ArgumentUtility.CheckNotEmpty("value", value);

      _loadMenuItemStatus = value;
    }

    string IControlWithDiagnosticMetadata.ControlType
    {
      get { return "DropDownMenu"; }
    }
  }
}
