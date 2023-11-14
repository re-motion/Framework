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
using System.Web.UI.WebControls;
using Remotion.Utilities;
using Remotion.Web.Infrastructure;

namespace Remotion.Web.UI.Controls
{
  public abstract class MenuBase : WebControl, IControl, IPostBackEventHandler
  {
    private readonly WebMenuItemCollection _menuItems;

    private readonly IControl _ownerControl;
    private bool _isReadOnly;

    protected MenuBase (IControl? ownerControl, Type[] supportedMenuItemTypes)
    {
      if (ownerControl == null)
        ownerControl = this;

      _ownerControl = ownerControl;
      _menuItems = new WebMenuItemCollection(ownerControl, supportedMenuItemTypes);
    }

    public IControl OwnerControl
    {
      get { return _ownerControl; }
    }

    [PersistenceMode(PersistenceMode.InnerProperty)]
    [ListBindable(false)]
    [Category("Behavior")]
    [Description("The menu items displayed by this drop down menu.")]
    [DefaultValue((string?)null)]
    public WebMenuItemCollection MenuItems
    {
      get { return _menuItems; }
    }

    [DefaultValue(false)]
    public bool IsReadOnly
    {
      get { return _isReadOnly; }
      set { _isReadOnly = value; }
    }

    /// <summary> Implements interface <see cref="IPostBackEventHandler"/>. </summary>
    /// <param name="eventArgument"> &lt;index&gt; </param>
    void IPostBackEventHandler.RaisePostBackEvent (string eventArgument)
    {
      ArgumentUtility.CheckNotNullOrEmpty("eventArgument", eventArgument);

      //  First part: index
      int index;
      try
      {
        if (eventArgument.Length == 0)
          throw new FormatException();
        index = int.Parse(eventArgument);
      }
      catch (FormatException)
      {
        throw new ArgumentException("First part of argument 'eventArgument' must be an integer. Expected format: '<index>'.");
      }

      if (index >= _menuItems.Count)
      {
        throw new ArgumentOutOfRangeException(
            eventArgument,
            "Index of argument 'eventargument' was out of the range of valid values. Index must be less than the number of displayed menu items.'");
      }

      WebMenuItem item = _menuItems[index];
      if (item.Command == null)
      {
        throw new ArgumentOutOfRangeException(
            eventArgument, "The DropDownMenu '" + ID + "' does not have a command associated with menu item " + index + ".");
      }

      switch (item.Command.Type)
      {
        case CommandType.Event:
        {
          OnEventCommandClick(item);
          break;
        }
        case CommandType.WxeFunction:
        {
          OnWxeFunctionCommandClick(item);
          break;
        }
        default:
        {
          break;
        }
      }
    }

    /// <summary> Occurs when a command of type <see cref="CommandType.Event"/> is clicked. </summary>
    [Category("Action")]
    [Description("Occurs when a command of type Event is clicked.")]
    public virtual event WebMenuItemClickEventHandler? EventCommandClick;

    /// <summary> Occurs when a command of type <see cref="CommandType.WxeFunction"/> is clicked. </summary>
    [Category("Action")]
    [Description("Occurs when a command of type WxeFunction is clicked.")]
    public virtual event WebMenuItemClickEventHandler? WxeFunctionCommandClick;

    /// <summary> Fires the <see cref="MenuBase.EventCommandClick"/> event. </summary>
    protected virtual void OnEventCommandClick (WebMenuItem item)
    {
      ArgumentUtility.CheckNotNull("item", item);

      if (item.Command != null)
        item.Command.OnClick();

      if (EventCommandClick != null)
      {
        WebMenuItemClickEventArgs e = new WebMenuItemClickEventArgs(item);
        EventCommandClick(this, e);
      }
    }

    /// <summary> Fires the <see cref="MenuBase.WxeFunctionCommandClick"/> event. </summary>
    protected virtual void OnWxeFunctionCommandClick (WebMenuItem item)
    {
      if (WxeFunctionCommandClick != null)
      {
        WebMenuItemClickEventArgs e = new WebMenuItemClickEventArgs(item);
        WxeFunctionCommandClick(this, e);
      }
    }

    public new IPage? Page
    {
      get { return PageWrapper.CastOrCreate(base.Page); }
    }
  }
}
