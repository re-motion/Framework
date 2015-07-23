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
using System.Linq;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocListUserControl : DataEditUserControl
  {
    public override IBusinessObjectDataSourceControl DataSource
    {
      get { return CurrentObject; }
    }

    protected override void OnInit (EventArgs e)
    {
      base.OnInit (e);

      var dummyPersonWithNoJobs = Person.CreateObject();
      dummyPersonWithNoJobs.Jobs = new Job[0];
      EmptyObject.BusinessObject = (IBusinessObject) dummyPersonWithNoJobs;
      EmptyObject.LoadValues (false);

      var view1 = new BocListView { ItemID = "ViewCmd1", Title = "View 1" };
      var view2 = new BocListView { ItemID = "ViewCmd2", Title = "View 2" };
      JobList_Normal.AvailableViews.AddRange (view1, view2);
      JobList_Normal.SelectedView = view2;
      JobList_ReadOnly.AvailableViews.AddRange (view1, view2);

      JobList_Normal.MenuItemClick += MenuItemClickHandler;
      JobList_ReadOnly.MenuItemClick += MenuItemClickHandler;

      JobList_Normal.SortingOrderChanged += SortingOrderChangedHandler;
      JobList_ReadOnly.SortingOrderChanged += SortingOrderChangedHandler;

      JobList_Normal.EditableRowChangesSaved += EditableRowChangedSavedHandler;
      JobList_ReadOnly.EditableRowChangesSaved += EditableRowChangedSavedHandler;

      JobList_Normal.EditableRowChangesCanceled += EditableRowChangesCanceledHandler;
      JobList_ReadOnly.EditableRowChangesCanceled += EditableRowChangesCanceledHandler;

      JobList_Normal.ListItemCommandClick += ListItemCommandClickHandler;
      JobList_ReadOnly.ListItemCommandClick += ListItemCommandClickHandler;

      JobList_Normal.CustomCellClick += CustomCellClickHandler;
      JobList_ReadOnly.CustomCellClick += CustomCellClickHandler;
    }

    protected override void OnPreRender (EventArgs e)
    {
      base.OnPreRender (e);
      SetTestOutput();
    }

    private void MenuItemClickHandler (object sender, WebMenuItemClickEventArgs e)
    {
      var bocList = (BocList) sender;
      var command = e.Item.ItemID + "|" + e.Item.Text;
      TestOutput.SetActionPerformed (bocList.ID, -1, "ListMenuOrOptionsClick", command);
    }

    private void SortingOrderChangedHandler (object sender, BocListSortingOrderChangeEventArgs bocListSortingOrderChangeEventArgs)
    {
      var bocList = (BocList) sender;
      TestOutput.SetActionPerformed (
          bocList.ID,
          -1,
          "SortingOrderChanged",
          string.Join (", ", bocListSortingOrderChangeEventArgs.NewSortingOrder.Select (nso => nso.Column.ItemID + "-" + nso.Direction.ToString()))
          );
    }

    private void EditableRowChangedSavedHandler (object sender, BocListItemEventArgs bocListItemEventArgs)
    {
      var bocList = (BocList) sender;
      TestOutput.SetActionPerformed (bocList.ID, bocListItemEventArgs.ListIndex, "InLineEdit", "Saved");
    }

    private void EditableRowChangesCanceledHandler (object sender, BocListItemEventArgs bocListItemEventArgs)
    {
      var bocList = (BocList) sender;
      TestOutput.SetActionPerformed (bocList.ID, bocListItemEventArgs.ListIndex, "InLineEdit", "Canceled");
    }

    private void ListItemCommandClickHandler (object sender, BocListItemCommandClickEventArgs bocListItemCommandClickEventArgs)
    {
      var bocList = (BocList) sender;
      var cell = bocListItemCommandClickEventArgs.Column.ItemID;
      TestOutput.SetActionPerformed (
          bocList.ID,
          bocListItemCommandClickEventArgs.ListIndex,
          "CellCommandClick",
          cell);
    }

    private void CustomCellClickHandler (object sender, BocCustomCellClickEventArgs bocCustomCellClickEventArgs)
    {
      var bocList = (BocList) sender;
      var cell = bocCustomCellClickEventArgs.Column.ItemID + "|" + bocCustomCellClickEventArgs.Column.ColumnTitleDisplayValue;
      TestOutput.SetActionPerformed (bocList.ID, -1, "CustomCellClick", cell);
    }

    private void SetTestOutput ()
    {
      TestOutput.SetInfoForNormalBocList (JobList_Normal);
    }

    private BocListUserControlTestOutput TestOutput
    {
      get { return (BocListUserControlTestOutput) ((Layout) Page.Master).GetTestOutputControl(); }
    }
  }
}