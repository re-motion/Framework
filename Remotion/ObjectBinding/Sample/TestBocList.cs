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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Sample
{
  public class TestBocList: BocList
  {
    public EventHandler<BocListItemEventArgs> RowMenuItemClick;
    protected override string GetSelectionChangedHandlerScript ()
    {
      var baseScript = base.GetSelectionChangedHandlerScript ();
      var extensionScript = "if (window.console && window.console.log) console.log ('OnSelectionChanged: ' + bocList.id + ', isInitializing: ' + isInitializing);";
      return string.Format ("function (bocList, isInitializing) {{ var base = {0}; base (bocList, isInitializing); {1}; }}", baseScript, extensionScript);
    }

    protected override WebMenuItem[] InitializeRowMenuItems(IBusinessObject businessObject, int listIndex)
    {
      WebMenuItem[] baseMenuItems = base.InitializeRowMenuItems (businessObject, listIndex);

      WebMenuItem[] menuItems = new WebMenuItem[3];
      var menuItem0 = new WebMenuItem();
      menuItem0.ItemID = listIndex.ToString() + "_0";
      menuItem0.Text = menuItem0.ItemID;
      menuItems[0] = menuItem0;

      var menuItem1 = new TestBocMenuItem (businessObject);
      menuItem1.ItemID = listIndex.ToString() + "_1";
      menuItem1.Text = menuItem1.ItemID;
      menuItems[1] = menuItem1;

      var menuItem2 = new WebMenuItem();
      menuItem2.ItemID = listIndex.ToString() + "_2";
      menuItem2.Text =  menuItem2.ItemID;
      menuItems[2] = menuItem2;

      return ArrayUtility.Combine (baseMenuItems, menuItems);
    }

    protected override void PreRenderRowMenuItems(WebMenuItemCollection menuItems, IBusinessObject businessObject, int listIndex)
    {
      base.PreRenderRowMenuItems (menuItems, businessObject,  listIndex);
      if (listIndex == 1)
        ((WebMenuItem)menuItems[2]).IsVisible = false;
      else if (listIndex == 2)
        ((WebMenuItem)menuItems[2]).IsDisabled = true;

      // In case the menu item is a dumb menu item
      // Set Text and Icon
      // Set IsVisible
      // Set isDisabled
    }

    protected override void OnRowMenuItemEventCommandClick (WebMenuItem menuItem, IBusinessObject businessObject, int listIndex)
    {
      base.OnRowMenuItemEventCommandClick (menuItem, businessObject, listIndex);
      if (RowMenuItemClick != null)
        RowMenuItemClick (menuItem, new BocListItemEventArgs (listIndex, businessObject));
    }

    public new void SetPageIndex (int pageIndex)
    {
      base.SetPageIndex (pageIndex);
    }
  }

  public class TestBocMenuItem: BocMenuItem
  {
    private IBusinessObject _businessObject;

    public TestBocMenuItem (IBusinessObject businessObject)
    {
      _businessObject = businessObject;
    }

    public IBusinessObject BusinessObject
    {
      get { return _businessObject; }
    }

    protected override void OnClick()
    {
      base.OnClick ();
      System.Diagnostics.Debug.WriteLine ("Clicked menu item '" + ItemID + "' for BusinessObject '" + _businessObject.ToString() + "'.");
      // handle the click
      base.OwnerControl.LoadValue (true);
    }

    protected override void PreRender()
    {
      base.PreRender ();
      // Set Text and Icon
    }

    public override bool EvaluateEnabled()
    {
      return base.EvaluateEnabled ();
      // if (base.EvaluateDisabled ())
      //   return true;
      // else
      //   do your own stuff
    }

    public override bool EvaluateVisible()
    {
      return base.EvaluateVisible ();
      // if (! base.EvaluateVisible ())
      //   return false;
      // else
      //   do your own stuff
    }
  }
}
