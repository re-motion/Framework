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
using System.ComponentModel;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.Logging;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.UI.Controls.WebTabStripImplementation;
using Remotion.Web.UI.Controls.WebTabStripImplementation.Rendering;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{
  [ToolboxData("<{0}:WebTabStrip runat=server></{0}:WebTabStrip>")]
  public class WebTabStrip
      :
          WebControl,
          IWebTabStrip,
          IPostBackDataHandler,
          IPostBackEventHandler,
          IResourceDispatchTarget
  {
    //  constants
    /// <summary> The key identifying a tab resource entry. </summary>
    private const string c_resourceKeyTabs = "Tabs";

    // statics
    private static readonly ILog s_log = LogManager.GetLogger(MethodBase.GetCurrentMethod()!.DeclaringType!);
    private static readonly object s_selectedIndexChangedEvent = new object();
    private static readonly object s_clickEvent = new object();

    // types

    // fields
    private readonly WebTabCollection _tabs;
    private WebTab? _selectedTab;
    private string? _selectedItemID;
    private string? _tabToBeSelected;
    private bool _hasTabsRestored;
    private bool _isRestoringTabs;
    private object? _tabsControlState;
    private bool _enableSelectedTab;
    private readonly WebTabStyle _tabStyle;
    private readonly WebTabStyle _selectedTabStyle;
    private readonly WebTabStyle _disabledTabStyle;

    public WebTabStrip (WebTabCollection tabCollection)
    {
      ArgumentUtility.CheckNotNull("tabCollection", tabCollection);
      _tabs = tabCollection;
      _tabs.SetTabStrip(this);
      _tabStyle = new WebTabStyle();
      _selectedTabStyle = new WebTabStyle();
      _disabledTabStyle = new WebTabStyle();
    }

    public WebTabStrip (IControl? ownerControl, Type[] supportedTabTypes)
        : this(new WebTabCollection(ownerControl, supportedTabTypes))
    {
    }

    public WebTabStrip (IControl? ownerControl)
        : this(ownerControl, new[] { typeof(WebTab) })
    {
    }

    public WebTabStrip ()
        : this(null, new[] { typeof(WebTab) })
    {
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit(e);

      RegisterHtmlHeadContents(Page!.Context!, HtmlHeadAppender.Current); // TODO RM-8118: not null assertions

      Page.RegisterRequiresControlState(this);
      Page.RegisterRequiresPostBack(this);
    }

    public void RegisterHtmlHeadContents (HttpContextBase context, HtmlHeadAppender htmlHeadAppender)
    {
      var renderer = CreateRenderer();
      renderer.RegisterHtmlHeadContents(htmlHeadAppender, this);
    }

    bool IPostBackDataHandler.LoadPostData (string postDataKey, NameValueCollection postCollection)
    {
      //  Is PostBack caused by this tab strip ?
      if (postCollection[ControlHelper.PostEventSourceID] == UniqueID)
      {
        _tabToBeSelected = postCollection[ControlHelper.PostEventArgumentID];
        ArgumentUtility.CheckNotNullOrEmpty("postCollection[\"__EVENTARGUMENT\"]", _tabToBeSelected!);
        if (_tabToBeSelected != _selectedItemID)
          return true;
      }
      return false;
    }

    void IPostBackDataHandler.RaisePostDataChangedEvent ()
    {
      EnsureTabsRestored();
      HandleSelectionChangeEvent(_tabToBeSelected!); // TODO RM-8118: debug not null
    }

    /// <summary> Handles the click event for a tab. </summary>
    /// <param name="itemID"> The id of the tab. </param>
    private void HandleSelectionChangeEvent (string itemID)
    {
      SetSelectedTab(itemID);
      OnSelectedIndexChanged();
    }

    protected virtual void OnSelectedIndexChanged ()
    {
      EventHandler? handler = (EventHandler?)Events[s_selectedIndexChangedEvent];
      if (handler != null)
        handler(this, EventArgs.Empty);
    }

    void IPostBackEventHandler.RaisePostBackEvent (string eventArgument)
    {
      EnsureTabsRestored();
      HandleClickEvent(eventArgument);
    }

    private void HandleClickEvent (string eventArgument)
    {
      ArgumentUtility.CheckNotNullOrEmpty("eventArgument", eventArgument);
      WebTab? tab = Tabs.Find(eventArgument);
      if (tab != null)
        OnClick(tab);
    }

    protected virtual void OnClick (WebTab tab)
    {
      ArgumentUtility.CheckNotNull("tab", tab);
      tab.OnClick();
      WebTabClickEventHandler? handler = (WebTabClickEventHandler?)Events[s_clickEvent];
      if (handler != null)
      {
        WebTabClickEventArgs e = new WebTabClickEventArgs(tab);
        handler(this, e);
      }
    }

    private void EnsureTabsRestored ()
    {
      if (_hasTabsRestored)
        return;

      _isRestoringTabs = true;
      if (_tabsControlState != null)
      {
        LoadTabsControlState(_tabsControlState, _tabs);
        _hasTabsRestored = true;
      }
      _isRestoringTabs = false;
    }

    protected override void LoadControlState (object? savedState)
    {
      if (savedState != null)
      {
        object?[] values = (object?[])savedState;
        base.LoadControlState(values[0]);
        _tabsControlState = values[1];
        _selectedItemID = (string?)values[2];
      }
    }

    protected override object? SaveControlState ()
    {
      object?[] values = new object?[3];
      values[0] = base.SaveControlState();
      values[1] = SaveTabsControlState(_tabs);
      values[2] = _selectedItemID;
      return values;
    }

    /// <summary> Loads the settings of the <paramref name="tabs"/> from <paramref name="tabsControlState"/>. </summary>
    private void LoadTabsControlState (object tabsControlState, WebTabCollection tabs)
    {
      ((IControlStateManager)tabs).LoadControlState(tabsControlState);
    }

    /// <summary> Saves the settings of the  <paramref name="tabs"/> and returns this view state </summary>
    private object? SaveTabsControlState (WebTabCollection tabs)
    {
      EnsureTabsRestored();
      return ((IControlStateManager)tabs).SaveControlState();
    }

    protected override void OnPreRender (EventArgs e)
    {
      EnsureTabsRestored();

      base.OnPreRender(e);

      var resourceManager = ResourceManagerUtility.GetResourceManager(this, true);
      var globalizationService = SafeServiceLocator.Current.GetInstance<IGlobalizationService>();

      LoadResources(resourceManager, globalizationService);
    }

    protected override void Render (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      var renderer = CreateRenderer();
      renderer.Render(CreateRenderingContext(writer));
    }

    protected virtual IWebTabStripRenderer CreateRenderer ()
    {
      return SafeServiceLocator.Current.GetInstance<IWebTabStripRenderer>();
    }

    protected virtual WebTabStripRenderingContext CreateRenderingContext (HtmlTextWriter writer)
    {
      ArgumentUtility.CheckNotNull("writer", writer);

      var builder = new WebTabRendererAdapterArrayBuilder(GetVisibleTabs().ToArray(), TabStyle, SelectedTabStyle);
      builder.EnableSelectedTab = EnableSelectedTab;

      var renderers = builder.GetWebTabRenderers();

      return new WebTabStripRenderingContext(Page!.Context!, writer, this, renderers); // TODO RM-8118: not null assertion
    }

    private List<WebTab> GetVisibleTabs ()
    {
      WebTabCollection tabs = Tabs;

      var visibleTabs = new List<WebTab>();
      foreach (WebTab tab in tabs)
      {
        if (tab.EvaluateVisible())
          visibleTabs.Add(tab);
      }

      return visibleTabs;
    }

    IList<IWebTab> IWebTabStrip.GetVisibleTabs ()
    {
      return GetVisibleTabs().ConvertAll<IWebTab>(tab => tab);
    }

    /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
    /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
    void IResourceDispatchTarget.Dispatch (IDictionary<string, WebString> values)
    {
      ArgumentUtility.CheckNotNull("values", values);
      Dispatch(values);
    }

    /// <summary> Dispatches the resources passed in <paramref name="values"/> to the control's properties. </summary>
    /// <param name="values"> An <c>IDictonary</c>: &lt;string key, string value&gt;. </param>
    protected virtual void Dispatch (IDictionary<string, WebString> values)
    {
      var tabValues = new Dictionary<string, IDictionary<string, WebString>>();
      var propertyValues = new Dictionary<string, WebString>();

      //  Parse the values

      foreach (var entry in values)
      {
        string key = entry.Key;
        string[] keyParts = key.Split(new[] { ':' }, 3);

        //  Is a property/value entry?
        if (keyParts.Length == 1)
        {
          string property = keyParts[0];
          propertyValues.Add(property, entry.Value);
        }
            //  Is collection entry?
        else if (keyParts.Length == 3)
        {
          //  Compound key: "collectionID:elementID:property"
          string collectionID = keyParts[0];
          string elementID = keyParts[1];
          string property = keyParts[2];

          Dictionary<string,IDictionary<string,WebString>>? currentCollection = null;

          //  Switch to the right collection
          switch (collectionID)
          {
            case c_resourceKeyTabs:
            {
              currentCollection = tabValues;
              break;
            }
            default:
            {
              //  Invalid collection property
              s_log.Warn(
                  "WebTabStrip '" + ID + "' in naming container '" + NamingContainer.GetType().GetFullNameSafe() + "' on page '" + Page
                  + "' does not contain a collection property named '" + collectionID + "'.");
              break;
            }
          }

          //  Add the property/value pair to the collection
          if (currentCollection != null)
          {
            //  Get the dictonary for the current element
            //  If no dictonary exists, create it and insert it into the elements hashtable.
            if (!currentCollection.TryGetValue(elementID, out var elementValues))
            {
              elementValues = new Dictionary<string, WebString>();
              currentCollection[elementID] = elementValues;
            }

            //  Insert the argument and resource's value into the dictonary for the specified element.
            elementValues.Add(property, entry.Value);
          }
        }
        else
        {
          //  Not supported format or invalid property
          s_log.Warn(
              "WebTabStrip '" + ID + "' in naming container '" + NamingContainer.GetType().GetFullNameSafe() + "' on page '" + Page
              + "' received a resource with an invalid or unknown key '" + key
              + "'. Required format: 'property' or 'collectionID:elementID:property'.");
        }
      }

      //  Dispatch simple properties
      ResourceDispatcher.DispatchGeneric(this, propertyValues);

      //  Dispatch to collections
      Tabs.Dispatch(tabValues, this, "Tabs");
    }

    /// <summary> Loads the resources into the control's properties. </summary>
    protected virtual void LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull("globalizationService", globalizationService);

      Tabs.LoadResources(resourceManager, globalizationService);
    }

    /// <summary> Sets the selected tab. </summary>
    internal void SetSelectedTabInternal (WebTab? tab)
    {
      if (! _isRestoringTabs)
        EnsureTabsRestored();

      if (tab != null && tab.TabStrip != this)
        throw new InvalidOperationException("Only tabs that are part of this tab strip can be selected.");
      if (_selectedTab != tab)
      {
        if ((_selectedTab != null) && _selectedTab.IsSelected)
          _selectedTab.SetSelected(false);
        _selectedTab = tab;
        if ((_selectedTab != null) && ! _selectedTab.IsSelected)
          _selectedTab.SetSelected(true);

        if (_selectedTab == null)
          _selectedItemID = null;
        else
          _selectedItemID = _selectedTab.ItemID;

        if (_selectedTab != null)
          _selectedTab.OnSelectionChangedInternal();
      }
    }

    private void SetSelectedTab (string itemID)
    {
      ArgumentUtility.CheckNotNullOrEmpty("itemID", itemID);
      if (_selectedTab == null || _selectedTab.ItemID != itemID)
      {
        WebTab? tab = Tabs.Find(itemID);
        if (tab != _selectedTab)
          SetSelectedTabInternal(tab);
      }
    }

    /// <summary> Gets the currently selected tab. </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [Browsable(false)]
    public WebTab? SelectedTab
    {
      get
      {
        if (Tabs.Count > 0)
        {
          EnsureTabsRestored();
          if (! string.IsNullOrEmpty(_tabToBeSelected))
            SetSelectedTab(_tabToBeSelected);
        }
        return _selectedTab;
      }
    }

    /// <summary> Gets the tabs displayed by this tab strip. </summary>
    [PersistenceMode(PersistenceMode.InnerProperty)]
    [ListBindable(false)]
    [MergableProperty(false)]
    //  Default category
    [Description("The tabs displayed by this tab strip.")]
    [DefaultValue((string?)null)]
    public WebTabCollection Tabs
    {
      get { return _tabs; }
    }

    [Description("Determines whether to enable the selected tab.")]
    [DefaultValue(false)]
    public bool EnableSelectedTab
    {
      get { return _enableSelectedTab; }
      set { _enableSelectedTab = value; }
    }

    /// <summary> Occurs when a node is clicked. </summary>
    [Category("Action")]
    [Description("Occurs when the selected tab has been changed.")]
    public event EventHandler SelectedIndexChanged
    {
      add { Events.AddHandler(s_selectedIndexChangedEvent, value); }
      remove { Events.RemoveHandler(s_selectedIndexChangedEvent, value); }
    }

    /// <summary> Is raised when a tab is clicked. </summary>
    [Category("Action")]
    [Description("Is raised when a tab is clicked.")]
    public event WebTabClickEventHandler Click
    {
      add { Events.AddHandler(s_clickEvent, value); }
      remove { Events.RemoveHandler(s_clickEvent, value); }
    }

    [Category("Style")]
    [Description("The style that you want to apply to a tab that is not selected.")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public WebTabStyle TabStyle
    {
      get { return _tabStyle; }
    }

    [Category("Style")]
    [Description("The style that you want to apply to the selected tab.")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public WebTabStyle SelectedTabStyle
    {
      get { return _selectedTabStyle; }
    }

    [Category("Style")]
    [Description("The style that you want to apply to a disabled tab.")]
    [NotifyParentProperty(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [PersistenceMode(PersistenceMode.InnerProperty)]
    public WebTabStyle DisabledTabStyle
    {
      get { return _disabledTabStyle; }
    }

    public new IPage? Page
    {
      get { return PageWrapper.CastOrCreate(base.Page); }
    }

    string IControlWithDiagnosticMetadata.ControlType
    {
      get { return "WebTabStrip"; }
    }

    #region protected virtual string CssClass...

    /// <summary> Gets the CSS-Class applied to the <see cref="WebTabStrip"/> itself. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStrip</c>. </para>
    ///   <para> Applied only if the <see cref="WebControl.CssClass"/> is not set. </para>
    /// </remarks>
    protected virtual string CssClassBase
    {
      get { return "tabStrip"; }
    }

    /// <summary> Gets the CSS-Class applied to the pane of <see cref="WebTab"/> items. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTabsPane</c>. </para>
    /// </remarks>
    protected virtual string CssClassTabsPane
    {
      get { return "tabStripTabsPane"; }
    }

    /// <summary> Gets the CSS-Class applied to a pane of <see cref="WebTab"/> items if no items are present. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTabsPane</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>div.tabStripTabsPane.readOnly</c> as a selector. </para>
    /// </remarks>
    protected virtual string CssClassTabsPaneEmpty
    {
      get { return "empty"; }
    }

    /// <summary> Gets the CSS-Class applied to a <see cref="WebTab"/>. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTab</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="P:Control.TabStyle"/>. </para>
    /// </remarks>
    protected virtual string CssClassTab
    {
      get { return "tabStripTab"; }
    }

    /// <summary> Gets the CSS-Class applied to a <see cref="WebTab"/> if it is selected. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTabSelected</c>. </para>
    ///   <para> Applied only if the <see cref="Style.CssClass"/> is not set for the <see cref="P:Control.SelectedTabStyle"/>. </para>
    /// </remarks>
    protected virtual string CssClassTabSelected
    {
      get { return "tabStripTabSelected"; }
    }

    /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for formatting the inside of the anchor element. </summary>
    /// <remarks> 
    ///   <para> Class: <c>anchorBody</c>. </para>
    /// </remarks>
    protected virtual string CssClassTabAnchorBody
    {
      get { return "anchorBody"; }
    }

    /// <summary> Gets the CSS-Class applied to a <c>span</c> intended for clearing the space after the last tab. </summary>
    /// <remarks> 
    ///   <para> Class: <c>last</c>. </para>
    /// </remarks>
    protected virtual string CssClassTabLast
    {
      get { return "last"; }
    }

    /// <summary> Gets the CSS-Class applied to a separator. </summary>
    /// <remarks> 
    ///   <para> Class: <c>tabStripTabSeparator</c>. </para>
    /// </remarks>
    protected virtual string CssClassSeparator
    {
      get { return "tabStripTabSeparator"; }
    }

    /// <summary> Gets the CSS-Class applied to the <see cref="WebTab"/> when it is displayed disabled. </summary>
    /// <remarks> 
    ///   <para> Class: <c>disabled</c>. </para>
    ///   <para> Applied in addition to the regular CSS-Class. Use <c>.tabStripTab.disabled</c> as a selector.</para>
    /// </remarks>
    protected virtual string CssClassDisabled
    {
      get { return CssClassDefinition.Disabled; }
    }

    #endregion
  }
}
