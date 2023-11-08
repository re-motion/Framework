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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using JetBrains.Annotations;
using Remotion.Mixins;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.Web;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.Services;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DropDownMenuImplementation;
using Remotion.Web.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{

[ToolboxData("<{0}:BocDropDownMenu runat=server></{0}:BocDropDownMenu>")]
public class BocDropDownMenu : BusinessObjectBoundWebControl, IBocMenuItemContainer
{
  // constants

  // types

  // static members
  private static readonly Type[] s_supportedPropertyInterfaces = new Type[] {
      typeof(IBusinessObjectReferenceProperty) };
  private static readonly object s_menuItemClickEvent = new object();

  // member fields

  private DropDownMenu _dropDownMenu;
  private IBusinessObject? _value;
  private bool _enableIcon = true;
  private string[]? _hiddenMenuItems;
  private string? _controlServicePath;
  private string? _controlServiceArguments;

  // contruction and destruction

  protected IWebServiceFactory WebServiceFactory { get; }

  public BocDropDownMenu ()
      : this(SafeServiceLocator.Current.GetInstance<IWebServiceFactory>())
  {
  }

  protected BocDropDownMenu ([NotNull] IWebServiceFactory webServiceFactory)
  {
    ArgumentUtility.CheckNotNull("webServiceFactory", webServiceFactory);

    _dropDownMenu = new DropDownMenu(this);
    WebServiceFactory = webServiceFactory;
  }

  // methods and properties

  protected override void OnInit (EventArgs e)
  {
    base.OnInit(e);
    InitializeMenusItems();
  }

  protected override void CreateChildControls ()
  {
    base.CreateChildControls();
    _dropDownMenu.ID = ID + "_Boc_DropDownMenu";
    Controls.Add(_dropDownMenu);
    _dropDownMenu.EventCommandClick += new WebMenuItemClickEventHandler(DropDownMenu_EventCommandClick);
    _dropDownMenu.WxeFunctionCommandClick += new WebMenuItemClickEventHandler(DropDownMenu_WxeFunctionCommandClick);
  }

  protected override void OnPreRender (EventArgs e)
  {
    base.OnPreRender(e);
    PreRenderMenuItems();

    PreRenderDropDownMenu();

    CheckControlService();
  }

  private void CheckControlService ()
  {
    if (string.IsNullOrEmpty(ControlServicePath))
      return;

    var virtualServicePath = VirtualPathUtility.GetVirtualPath(this, ControlServicePath);
    WebServiceFactory.CreateJsonService<IBocDropDownMenuWebService>(virtualServicePath);
  }

  protected virtual void InitializeMenusItems ()
  {
  }

  protected virtual void PreRenderMenuItems ()
  {
    if (_hiddenMenuItems == null)
      return;

    BocDropDownMenu.HideMenuItems(MenuItems, _hiddenMenuItems);
  }

  public static void HideMenuItems (WebMenuItemCollection menuItems, string[] hiddenItems)
  {
    ArgumentUtility.CheckNotNull("menuItems", menuItems);
    ArgumentUtility.CheckNotNull("hiddenItems", hiddenItems);

    for (int idxHiddenItems = 0; idxHiddenItems < hiddenItems.Length; idxHiddenItems++)
    {
      string itemID = hiddenItems[idxHiddenItems].Trim();
      if (itemID.Length == 0)
        continue;

      bool isSuffix = itemID.IndexOf(".") == -1;
      string? itemIDSuffix = null;
      if (isSuffix)
        itemIDSuffix = "." + itemID;

      for (int idxItems = 0; idxItems < menuItems.Count; idxItems++)
      {
        WebMenuItem menuItem = (WebMenuItem)menuItems[idxItems];
        if (! menuItem.IsVisible)
          continue;
        if (string.IsNullOrEmpty(menuItem.ItemID))
          continue;

        if (isSuffix)
        {
          if (menuItem.ItemID.Length == itemID.Length)
          {
            if (menuItem.ItemID == itemID)
              menuItem.IsVisible = false;
          }
          else
          {
            if (menuItem.ItemID.EndsWith(itemIDSuffix!))
              menuItem.IsVisible = false;
          }
        }
        else
        {
          if (menuItem.ItemID == itemID)
            menuItem.IsVisible = false;
        }
      }
    }
  }

  private void PreRenderDropDownMenu ()
  {
    _dropDownMenu.IsReadOnly = true;
    _dropDownMenu.Enabled = Enabled;
    _dropDownMenu.Width = Width;
    _dropDownMenu.Height = Height;
    _dropDownMenu.Style.Clear();
    foreach (string key in Style.Keys)
      _dropDownMenu.Style[key] = Style[key];

    var businessObject = Value;
    if (businessObject != null)
    {
      _dropDownMenu.GetSelectionCount = "function() { return 1; }";
      _dropDownMenu.TitleText = GetTitleText(businessObject);

     if (_enableIcon)
     {
       _dropDownMenu.TitleIcon = BusinessObjectBoundWebControl.GetIcon(businessObject, businessObject.BusinessObjectClass.BusinessObjectProvider);
      }
    }
    else
    {
      _dropDownMenu.GetSelectionCount = "function() { return 0; }";
    }
  }

  protected virtual WebString GetTitleText (IBusinessObject businessObject)
  {
    ArgumentUtility.CheckNotNull("businessObject", businessObject);

    var titleText = businessObject is IBusinessObjectWithIdentity businessObjectWithIdentity
        ? businessObjectWithIdentity.GetAccessibleDisplayName()
        : businessObject.ToString();

    return WebString.CreateFromText(titleText);
  }

  protected override void Render (HtmlTextWriter writer)
  {

    if (!string.IsNullOrEmpty(ControlServicePath))
    {
      var businessObjectWebServiceContext = CreateBusinessObjectWebServiceContext();

      var stringValueParametersDictionary = new Dictionary<string, string?>();
      stringValueParametersDictionary.Add("controlID", ID);
      stringValueParametersDictionary.Add(
          "controlType",
          TypeUtility.GetPartialAssemblyQualifiedName(MixinTypeUtility.GetUnderlyingTargetType(GetType())));
      stringValueParametersDictionary.Add("businessObjectClass", businessObjectWebServiceContext.BusinessObjectClass);
      stringValueParametersDictionary.Add("businessObjectProperty", businessObjectWebServiceContext.BusinessObjectProperty);
      stringValueParametersDictionary.Add("businessObject", businessObjectWebServiceContext.BusinessObjectIdentifier);
      stringValueParametersDictionary.Add("arguments", businessObjectWebServiceContext.Arguments);

      _dropDownMenu.SetLoadMenuItemStatus(
          ControlServicePath,
          nameof(IBocDropDownMenuWebService.GetMenuItemStatus),
          stringValueParametersDictionary);
    }

    _dropDownMenu.RenderControl(writer);
  }

  private BusinessObjectWebServiceContext CreateBusinessObjectWebServiceContext ()
  {
    return BusinessObjectWebServiceContext.Create(
        DataSource,
        Property,
        ControlServiceArguments);
  }

  /// <summary> Loads the <see cref="Value"/> from the bound <see cref="IBusinessObject"/>. </summary>
  /// <include file='..\..\doc\include\UI\Controls\BocDropDownMenu.xml' path='BocDropDownMenu/LoadValue/*' />
  public override void LoadValue (bool interim)
  {
    if (DataSource == null)
      return;

    IBusinessObject? value = null;
    if (Property == null)
      value = DataSource.BusinessObject;
    else if (DataSource.BusinessObject != null)
      value = (IBusinessObject?)DataSource.BusinessObject.GetProperty(Property);

    LoadValueInternal(value, interim);
  }

  /// <summary> Populates the <see cref="Value"/> with the unbound <paramref name="value"/>. </summary>
  /// <param name="value"> 
  ///   The object implementing <see cref="IBusinessObject"/> to load, or <see langword="null"/>. 
  /// </param>
  /// <param name="interim"> Not used. </param>
  public void LoadUnboundValue (IBusinessObject? value, bool interim)
  {
    LoadValueInternal(value, interim);
  }

  /// <summary> Performs the actual loading for <see cref="LoadValue"/> and <see cref="LoadUnboundValue"/>. </summary>
  protected virtual void LoadValueInternal (IBusinessObject? value, bool interim)
  {
    Value = value;
  }

  /// <summary> Gets or sets the current value. </summary>
  /// <value> An object implementing <see cref="IBusinessObject"/>. </value>
  [Browsable(false)]
  public new IBusinessObject? Value
  {
    get { return _value; }
    set { _value = value; }
  }

  /// <summary> Gets or sets the current value when <see cref="Value"/> through polymorphism. </summary>
  /// <value> The value must be of type <see cref="IList"/> or <see cref="IBusinessObject"/>. </value>
  protected override sealed object? ValueImplementation
  {
    get { return Value; }
    set { Value = (IBusinessObject?)value; }
  }

  /// <summary>Gets a flag indicating whether the <see cref="BocDropDownMenu"/> contains a value. </summary>
  public override bool HasValue
  {
    get { return _value != null; }
  }

  /// <summary>
  ///   Gets or sets the <see cref="IBusinessObjectReferenceProperty"/> used for accessing the data to be loaded into 
  ///   <see cref="Value"/>.
  /// </summary>
  /// <value> 
  ///   An <see cref="IBusinessObjectReferenceProperty"/> that is part of the bound 
  ///   <see cref="IBusinessObject"/>'s <see cref="IBusinessObjectClass"/>. If no property is assigned, 
  ///   the <see cref="BusinessObjectBoundWebControl.DataSource"/>'s <see cref="IBusinessObject"/> 
  ///   itself will be used as the control's value.
  /// </value>
  [Browsable(false)]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public new IBusinessObjectReferenceProperty? Property
  {
    get { return (IBusinessObjectReferenceProperty?)base.Property; }
    set { base.Property = (IBusinessObjectReferenceProperty?)value; }
  }

  /// <summary> The <see cref="BocDropDownMenu"/> supports only scalar properties. </summary>
  /// <returns> <see langword="true"/> if <paramref name="isList"/> is <see langword="false"/>. </returns>
  /// <seealso cref="BusinessObjectBoundWebControl.SupportsPropertyMultiplicity"/>
  protected override bool SupportsPropertyMultiplicity (bool isList)
  {
    return ! isList;
  }

  /// <summary>
  ///   The <see cref="BocDropDownMenu"/> supports properties of types <see cref="IBusinessObjectReferenceProperty"/>.
  /// </summary>
  /// <seealso cref="BusinessObjectBoundWebControl.SupportedPropertyInterfaces"/>
  protected override Type[] SupportedPropertyInterfaces
  {
    get { return s_supportedPropertyInterfaces; }
  }

  /// <summary>
  ///   Gets a flag that determines whether it is valid to generate HTML &lt;label&gt; tags referencing the
  ///   <see cref="TargetControl"/>.
  /// </summary>
  /// <value> Returns always <see langword="false"/>. </value>
  public override bool UseLabel
  {
    get { return false; }
  }

  /// <summary>
  ///   Gets the input control that can be referenced by HTML tags like &lt;label for=...&gt; using its 
  ///   <see cref="Control.ClientID"/>.
  /// </summary>
  public override Control TargetControl
  {
    get { return (Control)_dropDownMenu; }
  }

  [PersistenceMode(PersistenceMode.InnerProperty)]
  [ListBindable(false)]
  [Category("Menu")]
  [Description("The menu items displayed by the menu.")]
  [DefaultValue((string?)null)]
  public WebMenuItemCollection MenuItems
  {
    get { return _dropDownMenu.MenuItems; }
  }

  [DefaultValue(true)]
  public bool EnableGrouping
  {
    get { return _dropDownMenu.EnableGrouping; }
    set { _dropDownMenu.EnableGrouping = value; }
  }

  [Category("Behavior")]
  [DefaultValue("")]
  public string? ControlServicePath
  {
    get { return _controlServicePath; }
    set { _controlServicePath = value ?? string.Empty; }
  }

  [Category("Behavior")]
  [DefaultValue("")]
  [Description("Additional arguments passed to the control service.")]
  public string? ControlServiceArguments
  {
    get { return _controlServiceArguments; }
    set { _controlServiceArguments = StringUtility.EmptyToNull(value); }
  }

  /// <summary> 
  ///   Handles the <see cref="Remotion.Web.UI.Controls.MenuBase.EventCommandClick"/> event of the 
  ///   <see cref="DropDownMenu"/>.
  /// </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="WebMenuItemClickEventArgs"/> object that contains the event data. </param>
  private void DropDownMenu_EventCommandClick (object sender, WebMenuItemClickEventArgs e)
  {
    OnMenuItemEventCommandClick(e.Item);
  }

  /// <summary> 
  ///   Calls the <see cref="BocMenuItemCommand.OnClick(BocMenuItem)"/> method of the <paramref name="menuItem"/>'s 
  ///   <see cref="BocMenuItem.Command"/> and raises <see cref="MenuItemClick"/> event. 
  /// </summary>
  /// <param name="menuItem"> The <see cref="BocMenuItem"/> that has been clicked. </param>
  /// <remarks> Only called for commands of type <see cref="CommandType.Event"/>. </remarks>
  protected virtual void OnMenuItemEventCommandClick (WebMenuItem menuItem)
  {
    WebMenuItemClickEventHandler? menuItemClickHandler = (WebMenuItemClickEventHandler?)Events[s_menuItemClickEvent];
    if (menuItem.Command != null)
    {
      if (menuItem is BocMenuItem)
        ((BocMenuItemCommand)menuItem.Command).OnClick((BocMenuItem)menuItem);
      else
        menuItem.Command.OnClick();
    }
    if (menuItemClickHandler != null)
    {
      WebMenuItemClickEventArgs e = new WebMenuItemClickEventArgs(menuItem);
      menuItemClickHandler(this, e);
    }
  }

  /// <summary> Is raised when a menu item with a command of type <see cref="CommandType.Event"/> is clicked. </summary>
  [Category("Action")]
  [Description("Is raised when a menu item with a command of type Event is clicked.")]
  public event WebMenuItemClickEventHandler MenuItemClick
  {
    add { Events.AddHandler(s_menuItemClickEvent, value); }
    remove { Events.RemoveHandler(s_menuItemClickEvent, value); }
  }

  /// <summary> 
  ///   Handles the <see cref="Remotion.Web.UI.Controls.MenuBase.WxeFunctionCommandClick"/> event of the 
  ///   <see cref="DropDownMenu"/>. 
  /// </summary>
  /// <param name="sender"> The source of the event. </param>
  /// <param name="e"> An <see cref="WebMenuItemClickEventArgs"/> object that contains the event data. </param>
  /// <remarks> Only called for commands of type <see cref="CommandType.Event"/>. </remarks>
  private void DropDownMenu_WxeFunctionCommandClick (object sender, WebMenuItemClickEventArgs e)
  {
    OnMenuItemWxeFunctionCommandClick(e.Item);
  }

  /// <summary> 
  ///   Calls the <see cref="BocMenuItemCommand.ExecuteWxeFunction(IWxePage,int[],IBusinessObject[])"/> method of the <paramref name="menuItem"/>'s 
  ///   <see cref="BocMenuItem.Command"/>.
  /// </summary>
  /// <param name="menuItem"> The <see cref="BocMenuItem"/> that has been clicked. </param>
  /// <remarks> Only called for commands of type <see cref="CommandType.WxeFunction"/>. </remarks>
  protected virtual void OnMenuItemWxeFunctionCommandClick (WebMenuItem menuItem)
  {
    if (menuItem != null && menuItem.Command != null)
    {
      if (menuItem is BocMenuItem)
      {
        int[] indices = new int[0];
        IBusinessObject[] businessObjects;
        if (Value != null)
          businessObjects = new IBusinessObject[] { Value };
        else
          businessObjects = new IBusinessObject[0];

        BocMenuItemCommand command = (BocMenuItemCommand)menuItem.Command;
        if (Page is IWxePage)
          command.ExecuteWxeFunction((IWxePage)Page, indices, businessObjects);
        //else
        //  command.ExecuteWxeFunction (Page, indices, businessObjects);
      }
      else
      {
        Command command = menuItem.Command;
        if (Page is IWxePage)
          command.ExecuteWxeFunction((IWxePage)Page, null);
        //else
        //  command.ExecuteWxeFunction (Page, null, new NameValueCollection (0));
      }
    }
  }
  /// <summary> 
  ///   Gets or sets a flag that determines whether the <see cref="Remotion.Web.UI.Controls.DropDownMenu.TitleIcon"/> 
  ///   is shown in front of the <see cref="Value"/>.
  /// </summary>
  /// <value> 
  ///   <see langword="true"/> to show the <see cref="Remotion.Web.UI.Controls.DropDownMenu.TitleIcon"/>. 
  ///   The default value is <see langword="true"/>. 
  /// </value>
  /// <remarks> 
  ///   An icon is only shown if the <see cref="Property"/>'s 
  ///   <see cref="IBusinessObjectClass.BusinessObjectProvider">ReferenceClass.BusinessObjectProvider</see>
  ///   provides an instance of type <see cref="IBusinessObjectWebUIService"/> and 
  ///   <see cref="IBusinessObjectWebUIService.GetIcon"/> returns not <see langword="null"/>.
  /// </remarks>
  [PersistenceMode(PersistenceMode.Attribute)]
  [Category("Appearance")]
  [Description("Flag that determines whether to show the icon in front of the value.")]
  [DefaultValue(true)]
  public bool EnableIcon
  {
    get { return _enableIcon; }
    set { _enableIcon = value; }
  }

  /// <summary> Gets or sets the list of menu items to be hidden. </summary>
  /// <value> The <see cref="WebMenuItem.ItemID"/> values of the menu items to hide. </value>
  [Category("Menu")]
  [Description("The list of menu items to be hidden, identified by their ItemIDs.")]
  [DefaultValue((string?)null)]
  [PersistenceMode(PersistenceMode.Attribute)]
  public string[] HiddenMenuItems
  {
    get
    {
      if (_hiddenMenuItems == null)
        return new string[0];
      return _hiddenMenuItems;
    }
    set {_hiddenMenuItems = value;}
  }
  /// <summary> Gets the encapsulated <see cref="DropDownMenu"/>. </summary>
  protected DropDownMenu DropDownMenu
  {
    get { return _dropDownMenu; }
  }

  bool IBocMenuItemContainer.IsReadOnly
  {
    get
    {
      if (DataSource == null)
        return false;
      if (Property == null)
        return (DataSource.Mode == DataSourceMode.Read) ? true : false;
      else
        return true;
    }
  }

  bool IBocMenuItemContainer.IsSelectionEnabled
  {
    get { return true; }
  }

  IBusinessObject[] IBocMenuItemContainer.GetSelectedBusinessObjects ()
  {
    if (Value == null)
      return new IBusinessObject[0];
    else
      return new IBusinessObject[] {Value};
  }

  void IBocMenuItemContainer.RemoveBusinessObjects (IBusinessObject[] businessObjects)
  {
    throw new NotSupportedException("BocDropDownMenu is a read-only control, even though the bound object might be modifiable.");
  }

  void IBocMenuItemContainer.InsertBusinessObjects (IBusinessObject[] businessObjects)
  {
    throw new NotSupportedException("BocDropDownMenu is a read-only control, even though the bound object might be modifiable.");
  }
}

}
