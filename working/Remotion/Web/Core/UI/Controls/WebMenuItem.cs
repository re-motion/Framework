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
using Remotion.Security;
using Remotion.Utilities;
using Remotion.Web.UI.Globalization;
using Remotion.Web.Utilities;

namespace Remotion.Web.UI.Controls
{
  [TypeConverter (typeof (ExpandableObjectConverter))]
  public class WebMenuItem : IControlItem
  {
    private const string c_separator = "-";

    public static WebMenuItem GetSeparator ()
    {
      return new WebMenuItem (
          null, null, c_separator, new IconInfo(), new IconInfo(), WebMenuItemStyle.IconAndText, RequiredSelection.Any, false, null);
    }

    private string _itemID = string.Empty;
    private string _category = string.Empty;
    private string _text = string.Empty;
    private IconInfo _icon;
    private IconInfo _disabledIcon;
    private WebMenuItemStyle _style = WebMenuItemStyle.IconAndText;
    private RequiredSelection _requiredSelection = RequiredSelection.Any;
    private bool _isDisabled;
    private bool _isVisible = true;
    private MissingPermissionBehavior _missingPermissionBehavior;
    private ISecurableObject _securableObject;

    /// <summary> The command rendered for this menu item. </summary>
    private readonly SingleControlItemCollection _command;

    /// <summary> The control to which this object belongs. </summary>
    private IControl _ownerControl;

    private readonly CommandClickEventHandler _commandClick;

    public WebMenuItem (
        string itemID,
        string category,
        string text,
        IconInfo icon,
        IconInfo disabledIcon,
        WebMenuItemStyle style,
        RequiredSelection requiredSelection,
        bool isDisabled,
        Command command)
    {
      _itemID = itemID;
      _category = category;
      _text = text;
      Icon = icon;
      DisabledIcon = disabledIcon;
      _style = style;
      _requiredSelection = requiredSelection;
      _isDisabled = isDisabled;
      _command = new SingleControlItemCollection (command, new[] { typeof (Command) });

      _commandClick = new CommandClickEventHandler (Command_Click);
      if (_command.ControlItem != null)
        ((Command) _command.ControlItem).Click += _commandClick;
    }

    public WebMenuItem ()
        : this (
            null,
            null,
            null,
            new IconInfo(),
            new IconInfo(),
            WebMenuItemStyle.IconAndText,
            RequiredSelection.Any,
            false,
            new Command (CommandType.Event))
    {
    }

    /// <summary> Is called when the value of <see cref="OwnerControl"/> has changed. </summary>
    protected virtual void OnOwnerControlChanged ()
    {
      if (Command != null)
        Command.OwnerControl = OwnerControl;
    }

    private void OwnerControl_PreRender (object sender, EventArgs e)
    {
      if (ControlHelper.IsDesignMode (_ownerControl))
        return;
      PreRender();
    }

    /// <summary> Is called when the <see cref="OwnerControl"/> is Pre-Rendered. </summary>
    protected virtual void PreRender ()
    {
    }

    private void Command_Click (object sender, CommandClickEventArgs e)
    {
      OnClick();
    }

    /// <summary> This mehtod is called when a menu item is clicked on the client side. </summary>
    protected virtual void OnClick ()
    {
    }

    /// <summary> Returns a <see cref="string"/> that represents this <see cref="WebMenuItem"/>. </summary>
    /// <returns> Returns the <see cref="Text"/>, followed by the class name of the instance. </returns>
    public override string ToString ()
    {
      string displayName = ItemID;
      if (string.IsNullOrEmpty (displayName))
        displayName = Text;
      if (string.IsNullOrEmpty (displayName))
        return DisplayedTypeName;
      else
        return string.Format ("{0}: {1}", displayName, DisplayedTypeName);
    }

    /// <summary> Gets the human readable name of this type. </summary>
    protected virtual string DisplayedTypeName
    {
      get { return "WebMenuItem"; }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Description ("The ID of this menu item.")]
    [NotifyParentProperty (true)]
    [ParenthesizePropertyName (true)]
    [DefaultValue ("")]
    public string ItemID
    {
      get { return _itemID; }
      set { _itemID = value ?? string.Empty; }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The category to which this menu item belongs. Items of the same category will be grouped in the UI.")]
    [NotifyParentProperty (true)]
    [DefaultValue ("")]
    public string Category
    {
      get { return _category; }
      set { _category = value ?? string.Empty; }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The text displayed in this menu item. Use '-' for a separator menu item. The value will not be HTML encoded.")]
    [NotifyParentProperty (true)]
    [DefaultValue ("")]
    public string Text
    {
      get { return _text; }
      set { _text = value ?? string.Empty; }
    }

    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public bool IsSeparator
    {
      get { return _text == c_separator; }
    }

    /// <summary> 
    ///   Gets or sets the image representing the menu item in the rendered page. Must not be <see langword="null"/>. 
    /// </summary>
    /// <value> An <see cref="IconInfo"/> representing the menu item. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Category ("Appearance")]
    [Description ("The image representing the menu item in the rendered page.")]
    [NotifyParentProperty (true)]
    public IconInfo Icon
    {
      get { return _icon; }
      set
      {
        ArgumentUtility.CheckNotNull ("Icon", value);
        _icon = value;
      }
    }

    private bool ShouldSerializeIcon ()
    {
      return IconInfo.ShouldSerialize (_icon);
    }

    private void ResetIcon ()
    {
      _icon.Reset();
    }

    /// <summary> 
    ///   Gets or sets the image representing the disabled menu item in the rendered page. Must not be <see langword="null"/>. 
    /// </summary>
    /// <value> An <see cref="IconInfo"/> representing the disabled menu item. </value>
    [PersistenceMode (PersistenceMode.Attribute)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Content)]
    [Category ("Appearance")]
    [Description ("The image representing the disabled menu item in the rendered page.")]
    [NotifyParentProperty (true)]
    public IconInfo DisabledIcon
    {
      get { return _disabledIcon; }
      set
      {
        ArgumentUtility.CheckNotNull ("DisabledIcon", value);
        _disabledIcon = value;
      }
    }

    private bool ShouldSerializeDisabledIcon ()
    {
      return IconInfo.ShouldSerialize (_disabledIcon);
    }

    private void ResetDisabledIcon ()
    {
      _disabledIcon.Reset();
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("The selection state of a connected control that is required for enabling this menu item.")]
    [NotifyParentProperty (true)]
    [DefaultValue (RequiredSelection.Any)]
    public RequiredSelection RequiredSelection
    {
      get { return _requiredSelection; }
      set { _requiredSelection = value; }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("True to manually disable the menu item.")]
    [NotifyParentProperty (true)]
    [DefaultValue (false)]
    public bool IsDisabled
    {
      get { return _isDisabled; }
      set { _isDisabled = value; }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [Description ("False to hide the menu item.")]
    [NotifyParentProperty (true)]
    [DefaultValue (true)]
    public bool IsVisible
    {
      get { return _isVisible; }
      set { _isVisible = value; }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Behavior")]
    [NotifyParentProperty (true)]
    [DefaultValue (MissingPermissionBehavior.Invisible)]
    public MissingPermissionBehavior MissingPermissionBehavior
    {
      get { return _missingPermissionBehavior; }
      set { _missingPermissionBehavior = value; }
    }

    [Browsable (false)]
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    public ISecurableObject SecurableObject
    {
      get { return _securableObject; }
      set { _securableObject = value; }
    }

    /// <summary> Gets or sets the <see cref="Command"/> rendered for this menu item. </summary>
    /// <value> A <see cref="Command"/>. </value>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Category ("Behavior")]
    [Description ("The command rendered for this menu item.")]
    [NotifyParentProperty (true)]
    public virtual Command Command
    {
      get { return (Command) _command.ControlItem; }
      set
      {
        if (Command != null)
          Command.Click -= _commandClick;
        _command.ControlItem = value;
        if (Command != null)
          Command.Click += _commandClick;
      }
    }

    [PersistenceMode (PersistenceMode.Attribute)]
    [Category ("Appearance")]
    [Description ("The style of this menu item.")]
    [NotifyParentProperty (true)]
    [DefaultValue (WebMenuItemStyle.IconAndText)]
    public WebMenuItemStyle Style
    {
      get { return _style; }
      set { _style = value; }
    }

    protected bool ShouldSerializeCommand ()
    {
      if (Command == null)
        return false;

      if (Command.IsDefaultType)
        return false;
      else
        return true;
    }

    /// <summary> Sets the <see cref="Command"/> to its default value. </summary>
    /// <remarks> 
    ///   The default value is a <see cref="Command"/> object with a <c>Command.Type</c> set to 
    ///   <see cref="CommandType.None"/>.
    /// </remarks>
    protected void ResetCommand ()
    {
      if (Command != null)
      {
        Command = (Command) Activator.CreateInstance (Command.GetType());
        Command.Type = CommandType.None;
      }
    }

    [PersistenceMode (PersistenceMode.InnerProperty)]
    [Browsable (false)]
    public SingleControlItemCollection PersistedCommand
    {
      get { return _command; }
    }

    /// <summary> Controls the persisting of the <see cref="Command"/>. </summary>
    /// <remarks> 
    ///   Does not persist <see cref="Command"/> objects with a <c>Command.Type</c> set to 
    ///   <see cref="CommandType.None"/>.
    /// </remarks>
    protected bool ShouldSerializePersistedCommand ()
    {
      return ShouldSerializeCommand();
    }

    /// <summary> Gets or sets the control to which this object belongs. </summary>
    [DesignerSerializationVisibility (DesignerSerializationVisibility.Hidden)]
    [Browsable (false)]
    public IControl OwnerControl
    {
      get { return OwnerControlImplementation; }
      set { OwnerControlImplementation = value; }
    }

    protected virtual IControl OwnerControlImplementation
    {
      get { return _ownerControl; }
      set
      {
        if (_ownerControl != value)
        {
          if (_ownerControl != null)
            _ownerControl.PreRender -= OwnerControl_PreRender;
          _ownerControl = value;
          if (_ownerControl != null)
            _ownerControl.PreRender += OwnerControl_PreRender;
          _command.OwnerControl = value;
          OnOwnerControlChanged();
        }
      }
    }

    public virtual bool EvaluateVisible ()
    {
      if (!IsVisible)
        return false;

      if (Command != null)
      {
        if (MissingPermissionBehavior == MissingPermissionBehavior.Invisible)
          return Command.HasAccess (_securableObject);
      }

      return true;
    }

    public virtual bool EvaluateEnabled ()
    {
      if (IsDisabled)
        return false;

      if (Command != null)
      {
        if (MissingPermissionBehavior == MissingPermissionBehavior.Disabled)
          return Command.HasAccess (_securableObject);
      }

      return true;
    }

    public virtual void LoadResources (IResourceManager resourceManager, IGlobalizationService globalizationService)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      ArgumentUtility.CheckNotNull ("globalizationService", globalizationService);
      
      string key = ResourceManagerUtility.GetGlobalResourceKey (Category);
      if (!string.IsNullOrEmpty (key))
        Category = resourceManager.GetString (key);

      key = ResourceManagerUtility.GetGlobalResourceKey (Text);
      if (!string.IsNullOrEmpty (key))
        Text = resourceManager.GetString (key);

      Icon.LoadResources (resourceManager);
      DisabledIcon.LoadResources (resourceManager);

      if (Command != null)
        Command.LoadResources (resourceManager, globalizationService);
    }
  }

  public enum RequiredSelection
  {
    Any = 0,
    ExactlyOne = 1,
    OneOrMore = 2
  }

  public enum WebMenuItemStyle
  {
    IconAndText,
    Icon,
    Text
  }

  /// <summary>
  ///   Represents the method that handles the <c>Click</c> event raised when clicking on a menu item.
  /// </summary>
  public delegate void WebMenuItemClickEventHandler (object sender, WebMenuItemClickEventArgs e);

  /// <summary>
  ///   Provides data for the <c>Click</c> event.
  /// </summary>
  public class WebMenuItemClickEventArgs : EventArgs
  {
    /// <summary> The <see cref="WebMenuItem"/> that was clicked. </summary>
    private readonly WebMenuItem _item;

    /// <summary> Initializes an instance. </summary>
    public WebMenuItemClickEventArgs (WebMenuItem item)
    {
      ArgumentUtility.CheckNotNull ("item", item);
      _item = item;
    }

    /// <summary> The <see cref="Command"/> that caused the event. </summary>
    public Command Command
    {
      get { return _item.Command; }
    }

    /// <summary> The <see cref="WebMenuItem"/> that was clicked. </summary>
    public WebMenuItem Item
    {
      get { return _item; }
    }
  }
}
