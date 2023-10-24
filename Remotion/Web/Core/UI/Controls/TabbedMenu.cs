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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Globalization;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.TabbedMenuImplementation;
using Remotion.Web.UI.Controls.TabbedMenuImplementation.Rendering;
using Remotion.Web.UI.Controls.WebTabStripImplementation;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{

  /// <summary>
  ///   The <b>TabbedMenu</b> can be used to provide a navigation menu.
  /// </summary>
  public class TabbedMenu : WebControl, INavigationControl, ITabbedMenu
  {
    // constants

    // statics
    private static readonly object s_eventCommandClickEvent = new object();

    // types


    // fields
    private readonly Style _statusStyle;
    private readonly WebTabStrip _mainMenuTabStrip;
    private readonly WebTabStrip _subMenuTabStrip;
    private WebString _statusText;
    private bool _isSubMenuTabStripRefreshed;
    private bool _isPastInitialization;
    private Color _subMenuBackgroundColor;
    private ResourceManagerSet? _cachedResourceManager;

    // construction and destruction
    public TabbedMenu ()
    {
      _mainMenuTabStrip = new WebTabStrip(new MainMenuTabCollection(this, new[] { typeof(MainMenuTab) }));
      _subMenuTabStrip = new WebTabStrip(this, new[] { typeof(SubMenuTab) });
      _statusStyle = new Style();
      _subMenuBackgroundColor = new Color();
    }

    // methods and properties

    /// <summary> Overrides the <see cref="Control.OnInit"/> method. </summary>
    protected override void OnInit (EventArgs e)
    {
      EnsureChildControls();
      base.OnInit(e);

      _isPastInitialization = true;
      _mainMenuTabStrip.EnableSelectedTab = true;
      _mainMenuTabStrip.EnableViewState = false;
      _mainMenuTabStrip.Click += MainMenuTabStrip_Click;
      _subMenuTabStrip.EnableSelectedTab = true;
      _subMenuTabStrip.EnableViewState = false;
      _subMenuTabStrip.Click += SubMenuTabStrip_Click;

      if (Page is ISmartNavigablePage)
      {
        ((ISmartNavigablePage)Page).RegisterNavigationControl(this);
      }
      LoadSelection();

      RegisterHtmlHeadContents(HtmlHeadAppender.Current);
    }

    public void RegisterHtmlHeadContents (HtmlHeadAppender htmlHeadAppender)
    {
      ArgumentUtility.CheckNotNull("htmlHeadAppender", htmlHeadAppender);

      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents(htmlHeadAppender);
    }

    protected virtual ITabbedMenuRenderer CreateRenderer ()
    {
      return SafeServiceLocator.Current.GetInstance<ITabbedMenuRenderer>();
    }

    protected virtual TabbedMenuRenderingContext CreateRenderingContext (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      return new TabbedMenuRenderingContext(Page!.Context!, writer, this); //TODO RM-8118: not null assertion
    }

    /// <summary> Overrides the <see cref="Control.CreateChildControls"/> method. </summary>
    protected override void CreateChildControls ()
    {
      _mainMenuTabStrip.ID = ID + "_MainMenuTabStrip";
      Controls.Add(_mainMenuTabStrip);

      _subMenuTabStrip.ID = ID + "_SubMenuTabStrip";
      Controls.Add(_subMenuTabStrip);
    }

    /// <summary> Ensures that the <see cref="SubMenuTabStrip"/> is populated with the tabs from the selected 
    ///   <see cref="MainMenuTab"/>'s <see cref="MainMenuTab.SubMenuTabs"/>. 
    /// </summary>
    private void EnsureSubMenuTabStripPopulated ()
    {
      if (_isSubMenuTabStripRefreshed)
        return;
      PopulateSubMenuTabStrip();
    }

    /// <summary> 
    ///   Refreshes the <see cref="SubMenuTabStrip"/> with the tabs from the selected <see cref="MainMenuTab"/>'s
    ///   <see cref="MainMenuTab.SubMenuTabs"/>. 
    /// </summary>
    internal void RefreshSubMenuTabStrip ()
    {
      _subMenuTabStrip.Tabs.Clear();
      PopulateSubMenuTabStrip();
    }

    /// <summary> 
    ///   Populates the <see cref="SubMenuTabStrip"/> with the tabs from the selected <see cref="MainMenuTab"/>'s
    ///   <see cref="MainMenuTab.SubMenuTabs"/>. 
    /// </summary>
    private void PopulateSubMenuTabStrip ()
    {
      _isSubMenuTabStripRefreshed = true;
      MainMenuTab? selectedMainMenuItem = SelectedMainMenuTab;
      if (selectedMainMenuItem != null)
        _subMenuTabStrip.Tabs.AddRange(selectedMainMenuItem.SubMenuTabs);
      if (_subMenuTabStrip.SelectedTab == null && _subMenuTabStrip.Tabs.Count > 0)
        _subMenuTabStrip.SetSelectedTabInternal(_subMenuTabStrip.Tabs[0]);
    }

    /// <summary> Overrides the <see cref="Control.OnPreRender"/> method. </summary>
    protected override void OnPreRender (EventArgs e)
    {
      EnsureSubMenuTabStripPopulated();

      foreach (MenuTab menuTab in Tabs)
      {
        if (menuTab.Command != null && menuTab.Command.Type == CommandType.Event)
        {
          menuTab.Command.RegisterForSynchronousPostBackOnDemand(
              _mainMenuTabStrip, menuTab.ItemID, string.Format("TabbedMenu '{0}', MenuTab '{1}'", ID, menuTab.ItemID));
        }
      }

      if (SelectedMainMenuTab != null)
      {
        foreach (MenuTab menuTab in SelectedMainMenuTab.SubMenuTabs)
        {
          if (menuTab.Command != null && menuTab.Command.Type == CommandType.Event)
          {
            menuTab.Command.RegisterForSynchronousPostBackOnDemand(
                _subMenuTabStrip,
                menuTab.ItemID,
                string.Format("TabbedMenu '{0}', MainMenuTab '{1}', SubMenuTab '{2}'", ID, SelectedMainMenuTab.ItemID, menuTab.ItemID));
          }
        }
      }

      base.OnPreRender(e);
      SaveSelection();

      IResourceManager resourceManager = ResourceManagerUtility.GetResourceManager(this, true) ?? NullResourceManager.Instance;
      LoadResources(resourceManager);
    }

    /// <summary> Overrides the <see cref="WebControl.TagKey"/> property. </summary>
    /// <value> Returns a <see cref="HtmlTextWriterTag.Table"/> tab. </value>
    protected override HtmlTextWriterTag TagKey
    {
      get { return HtmlTextWriterTag.Table; }
    }

    /// <summary> Overrides the <see cref="WebControl.AddAttributesToRender"/> method. </summary>
    protected override void AddAttributesToRender (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      base.AddAttributesToRender(writer);
      if (string.IsNullOrEmpty(CssClass) && string.IsNullOrEmpty(Attributes["class"]))
        writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClassBase);
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);
      EnsureChildControls();

      var renderer = CreateRenderer();
      renderer.Render(CreateRenderingContext(writer));
    }

    /// <summary> Overrides the <see cref="Control.Controls"/> property. </summary>
    public override ControlCollection Controls
    {
      get
      {
        EnsureChildControls();
        return base.Controls;
      }
    }


    /// <summary> Gets the ID used for reading and persisting the selected tab IDs. </summary>
    /// <remarks> Value: <c>TabbedMenuSelection</c>. </remarks>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public virtual string SelectionID
    {
      get { return "TabbedMenuSelection"; }
    }

    /// <summary> 
    ///   Creates a string array from the IDs of the provided <paramref name="mainMenuTab"/> and 
    ///   <paramref name="subMenuTab"/>.
    /// </summary>
    /// <param name="mainMenuTab"> 
    ///   The <see cref="MainMenuTab"/>. If <see langword="null"/>, an empty array will be returned.
    /// </param>
    /// <param name="subMenuTab"> The <see cref="SubMenuTab"/>. </param>
    /// <returns> A string array. </returns>
    private string[] ConvertTabIDsToArray (MainMenuTab? mainMenuTab, SubMenuTab? subMenuTab)
    {
      string[] tabIDs;
      if (mainMenuTab == null)
      {
        tabIDs = new string[0];
      }
      else if (subMenuTab == null)
      {
        tabIDs = new string[1];
        tabIDs[0] = mainMenuTab.ItemID;
      }
      else
      {
        tabIDs = new string[2];
        tabIDs[0] = mainMenuTab.ItemID;
        tabIDs[1] = subMenuTab.ItemID;
      }
      return tabIDs;
    }

    /// <summary> Loads the selected tabs from the window state or the query string. </summary>
    private void LoadSelection ()
    {
      string[] selectedTabIDs = GetSelectionFromWindowState();
      if (selectedTabIDs.Length == 0)
        selectedTabIDs = GetSelectionFromQueryString();

      if (selectedTabIDs.Length > 0)
      {
        string selectedMainMenuItemID = selectedTabIDs[0];
        WebTab selectedMainMenuItem = _mainMenuTabStrip.Tabs.Find(selectedMainMenuItemID)!;
        if (selectedMainMenuItem.IsVisible)
          selectedMainMenuItem.IsSelected = true;
      }
      RefreshSubMenuTabStrip();
      if (selectedTabIDs.Length > 1)
      {
        string selectedSubMenuItemID = selectedTabIDs[1];
        WebTab selectedSubMenuItem = _subMenuTabStrip.Tabs.Find(selectedSubMenuItemID)!;
        if (selectedSubMenuItem.IsVisible)
          selectedSubMenuItem.IsSelected = true;
      }
    }

    /// <summary> Gets the IDs of the tabs to be selected from the query string. </summary>
    /// <returns> 
    ///   A string array containing the ID of the <see cref="MainMenuTab"/> at index 0 and the ID of the 
    ///   <see cref="SubMenuTab"/> at index 1. If no selected tab could be found, the array is empty.
    /// </returns>
    private string[] GetSelectionFromQueryString ()
    {
      string[]? selection = null;

      string? value;
      if (Page is IWxePage)
        value = WxeContext.Current!.QueryString[SelectionID]; // TODO RM-8118: not null assertion
      else
        value = Context.Request.QueryString[SelectionID];
      if (value != null)
        selection = (string[]?)TypeConversionProvider.Convert(typeof(string), typeof(string[]), value);

      if (selection == null)
        selection = new string[0];
      return selection;
    }

    /// <summary> Gets the IDs of the tabs to be selected from the <see cref="IWindowStateManager"/>. </summary>
    /// <returns> 
    ///   A string array containing the ID of the <see cref="MainMenuTab"/> at index 0 and the ID of the 
    ///   <see cref="SubMenuTab"/> at index 1. If no selected tab could be found, the array is empty.
    /// </returns>
    private string[] GetSelectionFromWindowState ()
    {
      string[]? selection = null;

      IWindowStateManager? windowStateManager = Page as IWindowStateManager;
      if (windowStateManager != null)
        selection = (string[]?)windowStateManager.GetData(SelectionID);

      if (selection == null)
        selection = new string[0];
      return selection;
    }

    /// <summary> Saves the selected tabs into the window state. </summary>
    private void SaveSelection ()
    {
      IWindowStateManager? windowStateManager = Page as IWindowStateManager;
      if (windowStateManager == null)
        return;

      string[] tabIDs = ConvertTabIDsToArray(SelectedMainMenuTab, SelectedSubMenuTab);
      windowStateManager.SetData(SelectionID, tabIDs);
    }

    /// <summary> 
    ///   Provides the URL parameters containing the navigation information for this control (e.g. the selected tab).
    /// </summary>
    /// <returns> 
    ///   A <see cref="NameValueCollection"/> containing the URL parameters required by this 
    ///   <see cref="INavigationControl"/> to restore its navigation state when using hyperlinks.
    /// </returns>
    NameValueCollection INavigationControl.GetNavigationUrlParameters ()
    {
      if (_subMenuTabStrip.SelectedTab != null)
        return GetUrlParameters(SelectedSubMenuTab!);
      else if (_mainMenuTabStrip.SelectedTab != null)
        return GetUrlParameters(SelectedMainMenuTab!);
      return new NameValueCollection();
    }

    /// <summary> Gets the parameters required for selecting the <paramref name="menuTab"/>. </summary>
    /// <param name="menuTab"> 
    ///   The <see cref="MenuTab"/> that should be selected by the when using the returned URL parameters. 
    ///   Must not be <see langword="null"/>.
    /// </param>
    /// <returns> 
    ///   A <see cref="NameValueCollection"/> that contains the URL parameters parameters required by this 
    ///   <see cref="TabbedMenu"/>.
    /// </returns>
    public virtual NameValueCollection GetUrlParameters (IMenuTab menuTab)
    {
      ArgumentUtility.CheckNotNull("menuTab", menuTab);

      MainMenuTab? mainMenuTab = menuTab as MainMenuTab;
      SubMenuTab? subMenuTab = menuTab as SubMenuTab;

      string[] tabIDs;
      if (mainMenuTab != null)
        tabIDs = ConvertTabIDsToArray(mainMenuTab, null);
      else if (subMenuTab != null)
        tabIDs = ConvertTabIDsToArray(subMenuTab.Parent, subMenuTab);
      else
        throw new NotSupportedException(string.Format("menuTab is of unsupported type '{0}'.", menuTab.GetType().GetFullNameSafe()));

      string? value = (string?)TypeConversionProvider.Convert(typeof(string[]), typeof(string), tabIDs);

      NameValueCollection urlParameters = new NameValueCollection();
      urlParameters.Add(SelectionID, value);
      return urlParameters;
    }

    /// <summary> 
    ///   Adds parameters required for re-selecting the currently selected <see cref="MenuTab"/> to the 
    ///   <paramref name="url"/>. 
    /// </summary>
    /// <param name="url"> The URL. Must not be <see langword="null"/> or empty. </param>
    /// <returns> The <paramref name="url"/> extended with the parameters required by this <see cref="TabbedMenu"/>. </returns>
    public string FormatUrl (string url)
    {
      ArgumentUtility.CheckNotNullOrEmpty("url", url);

      if (_subMenuTabStrip.SelectedTab != null)
        return FormatUrl(url, SelectedSubMenuTab!);
      else if (_mainMenuTabStrip.SelectedTab != null)
        return FormatUrl(url, SelectedMainMenuTab!);
      else
        return url;
    }

    /// <summary> 
    ///   Adds the parameters required for selecting the <paramref name="menuTab"/> to the <paramref name="url"/>.
    /// </summary>
    /// <param name="url"> The URL. Must not be <see langword="null"/> or empty. </param>
    /// <param name="menuTab"> 
    ///   The <see cref="MenuTab"/> that should be selected by the <paramref name="url"/>. 
    ///   Must not be <see langword="null"/>.
    /// </param>
    /// <returns> The <paramref name="url"/> extended with the parameters required by this <see cref="TabbedMenu"/>. </returns>
    public string FormatUrl (string url, MenuTab menuTab)
    {
      ArgumentUtility.CheckNotNullOrEmpty("url", url);

      NameValueCollection urlParameters = GetUrlParameters(menuTab);
      url = UrlUtility.AddParameters(url, urlParameters);
      return url;
    }


    /// <summary> 
    ///   Event handler for the <see cref="WebTabStrip.Click"/> of the <see cref="MainMenuTabStrip"/>.
    /// </summary>
    private void MainMenuTabStrip_Click (object sender, WebTabClickEventArgs e)
    {
      HandleTabStripClick((MenuTab)e.Tab);
    }

    /// <summary> 
    ///   Event handler for the <see cref="WebTabStrip.Click"/> of the <see cref="SubMenuTabStrip"/>.
    /// </summary>
    private void SubMenuTabStrip_Click (object sender, WebTabClickEventArgs e)
    {
      HandleTabStripClick((MenuTab)e.Tab);
    }

    /// <summary> 
    ///   Handles the click events of the <see cref="MainMenuTabStrip"/> and the <see cref="SubMenuTabStrip"/>.
    /// </summary>
    /// <param name="tab"> The <see cref="MenuTab"/> whose command was clicked. </param>
    private void HandleTabStripClick (MenuTab tab)
    {
      if (tab != null && tab.Command != null)
      {
        if (tab.Command.Type == CommandType.Event)
        {
          OnEventCommandClick(tab);
        }
        else if (tab.Command.Type == CommandType.WxeFunction)
        {
          throw new InvalidOperationException("MenuTab commands of CommandType WxeFunction must always execute on client side.");
        }
      }
    }

    /// <summary> Fires the <see cref="EventCommandClick"/> event. </summary>
    /// <param name="tab"> The <see cref="MenuTab"/> whose command was clicked. </param>
    protected virtual void OnEventCommandClick (MenuTab? tab)
    {
      if (tab != null && tab.Command != null)
        tab.Command.OnClick();

      MenuTabClickEventHandler? handler = (MenuTabClickEventHandler?)Events[s_eventCommandClickEvent];
      if (handler != null)
      {
        MenuTabClickEventArgs e = new MenuTabClickEventArgs(tab!); // TODO RM-8118: not null assertion
        handler(this, e);
      }
    }


    /// <summary> Find the <see cref="IResourceManager"/> for this control. </summary>
    protected virtual IResourceManager GetResourceManager ()
    {
      return GetResourceManager(null!); // TODO RM-8118: What's happening here?
    }

    /// <summary> Find the <see cref="IResourceManager"/> for this control. </summary>
    /// <param name="localResourcesType"> 
    ///   A type with the <see cref="MultiLingualResourcesAttribute"/> applied to it.
    ///   Typically an <b>enum</b> or the derived class itself.
    /// </param>
    protected IResourceManager GetResourceManager (Type localResourcesType)
    {
      //Remotion.Utilities.ArgumentUtility.CheckNotNull ("localResourcesType", localResourcesType);

      //  Provider has already been identified.
      if (_cachedResourceManager != null)
        return _cachedResourceManager;

      //  Get the resource managers

      var localResourceManager = GlobalizationService.GetResourceManager(localResourcesType);
      var namingContainerResourceManager = ResourceManagerUtility.GetResourceManager(NamingContainer, true);

      _cachedResourceManager = ResourceManagerSet.Create(namingContainerResourceManager, localResourceManager);

      return _cachedResourceManager;
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected virtual void LoadResources (IResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull("resourceManager", resourceManager);

      string? key = ResourceManagerUtility.GetGlobalResourceKey(StatusText.GetValue());
      if (!string.IsNullOrEmpty(key))
        StatusText = resourceManager.GetWebString(key, StatusText.Type);
    }

    protected IGlobalizationService GlobalizationService
    {
      get { return SafeServiceLocator.Current.GetInstance<IGlobalizationService>(); }
    }

    private ITypeConversionProvider TypeConversionProvider
    {
      get { return SafeServiceLocator.Current.GetInstance<ITypeConversionProvider>(); }
    }

    /// <summary> Gets the collection of <see cref="MainMenuTab"/> objects. </summary>
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [ListBindable(false)]
    [Description("")]
    [DefaultValue((string?)null)]
    public MainMenuTabCollection Tabs
    {
      get
      {
        if (_isPastInitialization)
          EnsureSubMenuTabStripPopulated();
        return (MainMenuTabCollection)_mainMenuTabStrip.Tabs;
      }
    }

    /// <summary> Gets or sets the text displayed in the status area. </summary>
    /// <remarks>
    ///   The value will not be HTML encoded.
    /// </remarks>
    [Description("The text displayed in the status area.")]
    [DefaultValue(typeof(WebString), "")]
    public WebString StatusText
    {
      get { return _statusText; }
      set { _statusText = value; }
    }

    /// <summary> Is raised when a tab with a command of type <see cref="CommandType.Event"/> is clicked. </summary>
    [Category("Action")]
    [Description("Is raised when a tab with a command of type Event is clicked.")]
    public event MenuTabClickEventHandler EventCommandClick
    {
      add { Events.AddHandler(s_eventCommandClickEvent, value); }
      remove { Events.RemoveHandler(s_eventCommandClickEvent, value); }
    }

    /// <summary> Gets the selected <see cref="MainMenuTab"/>. </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public MainMenuTab? SelectedMainMenuTab
    {
      get
      {
        return (MainMenuTab?)_mainMenuTabStrip.SelectedTab;
      }
    }

    /// <summary> Gets the selected <see cref="SubMenuTab"/>. </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SubMenuTab? SelectedSubMenuTab
    {
      get
      {
        return (SubMenuTab?)_subMenuTabStrip.SelectedTab;
      }
    }

    /// <summary> Gets the <see cref="WebTabStrip"/> used for the main menu. </summary>
    protected WebTabStrip MainMenuTabStrip
    {
      get { return _mainMenuTabStrip; }
    }

    /// <summary> Gets the <see cref="WebTabStrip"/> used for the sub menu. </summary>
    protected WebTabStrip SubMenuTabStrip
    {
      get { return _subMenuTabStrip; }
    }

    /// <summary> Gets the style applied to the status area. </summary>
    [Category("Style")]
    [Description("The style applied to the status area.")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public Style StatusStyle
    {
      get { return _statusStyle; }
    }

    [Category("Style")]
    [Description("The style that you want to apply to a main menu tab that is not selected.")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public WebTabStyle MainMenuTabStyle
    {
      get { return _mainMenuTabStrip.TabStyle; }
    }

    [Category("Style")]
    [Description("The style that you want to apply to the selected main menu tab.")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public WebTabStyle MainMenuSelectedTabStyle
    {
      get { return _mainMenuTabStrip.SelectedTabStyle; }
    }

    [Category("Style")]
    [Description("The style that you want to apply to a disabled main menu tab.")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public WebTabStyle MainMenuDisabledTabStyle
    {
      get { return _mainMenuTabStrip.DisabledTabStyle; }
    }

    [Category("Style")]
    [Description("The style that you want to apply to a sub menu tab that is not selected.")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public WebTabStyle SubMenuTabStyle
    {
      get { return _subMenuTabStrip.TabStyle; }
    }

    [Category("Style")]
    [Description("The style that you want to apply to the selected sub menu tab.")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public WebTabStyle SubMenuSelectedTabStyle
    {
      get { return _subMenuTabStrip.SelectedTabStyle; }
    }

    [Category("Style")]
    [Description("The style that you want to apply to a disabled sub menu tab.")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public WebTabStyle SubMenuDisabledTabStyle
    {
      get { return _subMenuTabStrip.DisabledTabStyle; }
    }

    [Category("Style")]
    [Description("The background color that you want to apply to the sub menu area.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [NotifyParentProperty(true)]
    [TypeConverter(typeof(WebColorConverter))]
    [DefaultValue(typeof(Color), "")]
    public Color SubMenuBackgroundColor
    {
      get { return _subMenuBackgroundColor; }
      set { _subMenuBackgroundColor = value; }
    }

    IWebTabStrip ITabbedMenu.MainMenuTabStrip
    {
      get { return MainMenuTabStrip; }
    }

    IWebTabStrip ITabbedMenu.SubMenuTabStrip
    {
      get { return SubMenuTabStrip; }
    }

    public new IPage? Page
    {
      get { return PageWrapper.CastOrCreate(base.Page); }
    }

    string IControlWithDiagnosticMetadata.ControlType
    {
      get { return "TabbedMenu"; }
    }

    #region protected virtual string CssClass...
    /// <summary> Gets the CSS-Class applied to the <see cref="WebTabStrip"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStrip</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    protected virtual string CssClassBase
    {
      get { return "tabbedMenu"; }
    }

    /// <summary> Gets the CSS-Class applied to the main menu's tab strip. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMainMenu</c>. </para>
    /// </remarks>
    protected virtual string CssClassMainMenu
    {
      get { return "tabbedMainMenu"; }
    }

    /// <summary> Gets the CSS-Class applied to the sub menu's tab strip. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedSubMenu</c>. </para>
    /// </remarks>
    protected virtual string CssClassSubMenu
    {
      get { return "tabbedSubMenu"; }
    }

    /// <summary> Gets the CSS-Class applied to the main menu cell. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMainMenuCell</c>. </para>
    /// </remarks>
    protected virtual string CssClassMainMenuCell
    {
      get { return "tabbedMainMenuCell"; }
    }

    /// <summary> Gets the CSS-Class applied to the sub menu cell. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedSubMenuCell</c>. </para>
    /// </remarks>
    protected virtual string CssClassSubMenuCell
    {
      get { return "tabbedSubMenuCell"; }
    }

    /// <summary> Gets the CSS-Class applied to the status cell. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabbedMenuStatusCell</c>. </para>
    /// </remarks>
    protected virtual string CssClassStatusCell
    {
      get { return "tabbedMenuStatusCell"; }
    }

    #endregion
  }
}
