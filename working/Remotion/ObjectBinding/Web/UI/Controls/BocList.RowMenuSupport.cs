﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Linq;
using System.Web.UI.WebControls;
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public partial class BocList
  {
    private bool _rowMenusInitialized;

    private BocListRowMenu[] _rowMenus = new BocListRowMenu[0];

    private readonly PlaceHolder _rowMenusPlaceHolder = new PlaceHolder();

    private void CreateChildControlsForRowMenus ()
    {
      Controls.Add (_rowMenusPlaceHolder);
    }

    private void ResetRowMenus ()
    {
      _rowMenus = new BocListRowMenu[0];
      _rowMenusPlaceHolder.Controls.Clear();
      _rowMenusInitialized = false;
    }

    private void EnsureRowMenusInitialized ()
    {
      if (_rowMenusInitialized)
        return;

      _rowMenusInitialized = true;

      if (! AreRowMenusEnabled)
        return;
      if (IsDesignMode)
        return;
      if (!HasValue)
        return;
      EnsureChildControls();

      Assertion.IsTrue (_rowMenus.Length == 0);
      Assertion.IsTrue (_rowMenusPlaceHolder.Controls.Count == 0);

      var rowMenus = new List<BocListRowMenu>();
      foreach (var row in EnsureBocListRowsForCurrentPageGot())
      {
        var rowMenu = new BocListRowMenu (this, row.ValueRow);
        rowMenu.ID = GetRowMenuID (row.ValueRow);
        rowMenu.EventCommandClick += (sender, e) => HandleRowMenuItemClick (sender, e, OnRowMenuItemEventCommandClick);
        rowMenu.WxeFunctionCommandClick += (sender, e) => HandleRowMenuItemClick (sender, e, OnRowMenuItemWxeFunctionCommandClick);

        _rowMenusPlaceHolder.Controls.Add (rowMenu);
        WebMenuItem[] menuItems = InitializeRowMenuItems (row.ValueRow.BusinessObject, row.ValueRow.Index);
        rowMenu.MenuItems.AddRange (menuItems);

        rowMenus.Add (rowMenu);
      }

      _rowMenus = rowMenus.ToArray();
    }

    /// <summary> Creates the menu items for a data row. </summary>
    /// <param name="businessObject"> 
    ///   The <see cref="IBusinessObject"/> of the row for which the menu items are being generated. 
    /// </param>
    /// <param name="listIndex"> The position of the <paramref name="businessObject"/> in the list of values. </param>
    /// <returns> A <see cref="WebMenuItem"/> array with the menu items generated by the implementation. </returns>
    protected virtual WebMenuItem[] InitializeRowMenuItems (IBusinessObject businessObject, int listIndex)
    {
      return new WebMenuItem[0];
    }

    /// <summary> PreRenders the menu items for all row menus. </summary>
    private void PreRenderRowMenusItems ()
    {
      foreach (var rowMenu in _rowMenus)
      {
        PreRenderRowMenuItems (rowMenu.MenuItems, rowMenu.Row.BusinessObject, rowMenu.Row.Index);
        rowMenu.Visible = rowMenu.MenuItems.Cast<WebMenuItem>().Any (item => item.EvaluateVisible());
      }
    }

    /// <summary> PreRenders the menu items for a data row. </summary>
    /// <param name="menuItems"> The menu items to be displayed for the row. </param>
    /// <param name="businessObject"> 
    ///   The <see cref="IBusinessObject"/> of the row for which the menu items are being generated. 
    /// </param>
    /// <param name="listIndex"> The position of the <paramref name="businessObject"/> in the list of values. </param>
    protected virtual void PreRenderRowMenuItems (WebMenuItemCollection menuItems, IBusinessObject businessObject, int listIndex)
    {
    }

    private void HandleRowMenuItemClick (object sender, WebMenuItemClickEventArgs e, Action<WebMenuItem, IBusinessObject, int> handler)
    {
      var rowMenu = ArgumentUtility.CheckNotNullAndType<BocListRowMenu> ("sender", sender);

      int listIndex;
      if (!HasValue)
        listIndex = -1;
      else if (rowMenu.Row.Index >= Value.Count)
        listIndex = -1;
      else if (Value[rowMenu.Row.Index].Equals (rowMenu.Row.BusinessObject))
        listIndex = rowMenu.Row.Index;
      else
        listIndex = Value.IndexOf (rowMenu.Row.BusinessObject);

      var businessObject = rowMenu.Row.BusinessObject;

      handler (e.Item, businessObject, listIndex);
    }

    /// <summary> Handles the click on an Event command of a row menu. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/OnRowMenuItemEventCommandClick/*' />
    protected virtual void OnRowMenuItemEventCommandClick (WebMenuItem menuItem, IBusinessObject businessObject, int listIndex)
    {
      if (menuItem != null && menuItem.Command != null)
      {
        if (menuItem is BocMenuItem)
          ((BocMenuItemCommand) menuItem.Command).OnClick ((BocMenuItem) menuItem);
        else
          menuItem.Command.OnClick();
      }
    }

    /// <summary> Handles the click to a WXE function command or a row menu. </summary>
    /// <include file='..\..\doc\include\UI\Controls\BocList.xml' path='BocList/OnRowMenuItemWxeFunctionCommandClick/*' />
    protected virtual void OnRowMenuItemWxeFunctionCommandClick (WebMenuItem menuItem, IBusinessObject businessObject, int listIndex)
    {
      if (menuItem != null && menuItem.Command != null)
      {
        if (menuItem is BocMenuItem)
        {
          BocMenuItemCommand command = (BocMenuItemCommand) menuItem.Command;
          if (Page is IWxePage)
            command.ExecuteWxeFunction ((IWxePage) Page, new[] { listIndex }, new[] { businessObject });
          //else
          //  command.ExecuteWxeFunction (Page, new int[1] {listIndex}, new IBusinessObject[1] {businessObject});
        }
        else
        {
          Command command = menuItem.Command;
          if (Page is IWxePage)
            command.ExecuteWxeFunction ((IWxePage) Page, null);
          //else
          //  command.ExecuteWxeFunction (Page, null, new NameValueCollection (0));
        }
      }
    }
    
    /// <summary> 
    ///   Creates a <see cref="BocDropDownMenuColumnDefinition"/> if <see cref="RowMenuDisplay"/> is set to
    ///   <see cref="Controls.RowMenuDisplay.Automatic"/>.
    /// </summary>
    /// <returns> A <see cref="BocDropDownMenuColumnDefinition"/> instance or <see langword="null"/>. </returns>
    private BocDropDownMenuColumnDefinition GetRowMenuColumn ()
    {
      if (_rowMenuDisplay == RowMenuDisplay.Automatic)
      {
        BocDropDownMenuColumnDefinition dropDownMenuColumn = new BocDropDownMenuColumnDefinition();
        dropDownMenuColumn.Width = Unit.Percentage (0);
        dropDownMenuColumn.MenuTitleText = GetResourceManager().GetString (ResourceIdentifier.RowMenuTitle);
        return dropDownMenuColumn;
      }
      return null;
    }

    /// <summary> 
    ///   Tests that the <paramref name="columnDefinitions"/> array holds exactly one
    ///   <see cref="BocDropDownMenuColumnDefinition"/> if the <see cref="RowMenuDisplay"/> is set to 
    ///   <see cref="Controls.RowMenuDisplay.Automatic"/> or <see cref="Controls.RowMenuDisplay.Manual"/>.
    /// </summary>
    private void CheckRowMenuColumns (BocColumnDefinition[] columnDefinitions)
    {
      bool isFound = false;
      for (int i = 0; i < columnDefinitions.Length; i++)
      {
        if (columnDefinitions[i] is BocDropDownMenuColumnDefinition)
        {
          if (isFound)
            throw new InvalidOperationException ("Only a single BocDropDownMenuColumnDefinition is allowed in the BocList '" + ID + "'.");
          isFound = true;
        }
      }
      if (RowMenuDisplay == RowMenuDisplay.Manual && ! isFound)
      {
        throw new InvalidOperationException (
            "No BocDropDownMenuColumnDefinition was found in the BocList '" + ID + "' but the RowMenuDisplay was set to manual.");
      }
    }

    /// <summary> 
    ///   Gets a flag describing whether a <see cref="DropDownMenu"/> is shown in the 
    ///   <see cref="BocDropDownMenuColumnDefinition"/>.
    /// </summary>
    protected virtual bool AreRowMenusEnabled
    {
      get
      {
        if (WcagHelper.Instance.IsWaiConformanceLevelARequired())
          return false;
        if (_rowMenuDisplay == RowMenuDisplay.Undefined
            || _rowMenuDisplay == RowMenuDisplay.Disabled)
          return false;
        return true;
      }
    }

    private string GetRowMenuID (BocListRow row)
    {
      return ID + c_rowMenuIDPrefix + RowIDProvider.GetControlRowID (row);
    }
  }
}