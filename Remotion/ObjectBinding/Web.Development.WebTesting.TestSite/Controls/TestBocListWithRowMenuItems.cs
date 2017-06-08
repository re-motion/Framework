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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public class TestBocListWithRowMenuItems : BocList
  {
    protected override WebMenuItem[] InitializeRowMenuItems (IBusinessObject businessObject, int listIndex)
    {
      var baseRowMenuItems = base.InitializeRowMenuItems (businessObject, listIndex);

      var rowMenuItems = new WebMenuItem[2];
      rowMenuItems[0] = new WebMenuItem { ItemID = "RowMenuItemCmd1", Text = "Row menu 1" };
      rowMenuItems[1] = new WebMenuItem { ItemID = "RowMenuItemCmd2", Text = "Row menu 2" };

      return ArrayUtility.Combine (baseRowMenuItems, rowMenuItems);
    }

    protected override void OnRowMenuItemEventCommandClick (WebMenuItem menuItem, IBusinessObject businessObject, int listIndex)
    {
      var command = menuItem.ItemID + "|" + menuItem.Text;
      TestOutput.SetActionPerformed (ID, listIndex, "RowContextMenuClick", command);
    }

    private IBocListUserControlBaseTestOutput TestOutput
    {
      get { return (IBocListUserControlBaseTestOutput) ((Layout) Page.Master).GetTestOutputControl(); }
    }
  }
}