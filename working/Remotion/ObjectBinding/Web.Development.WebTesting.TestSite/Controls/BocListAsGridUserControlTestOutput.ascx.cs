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
using System.Web.UI;
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public partial class BocListAsGridUserControlTestOutput : UserControl, IBocListUserControlBaseTestOutput
  {
    public void SetInfoForNormalBocList (BocList bocList)
    {
      SelectedIndicesLabel.Text = GetSelectedRowIndicesAsString (bocList);
      SelectedViewLabel.Text = bocList.SelectedView.ItemID;
      EditModeLabel.Text = bocList.IsRowEditModeActive.ToString();
    }

    private string GetSelectedRowIndicesAsString (BocList bocList)
    {
      var selectedRows = string.Join (", ", bocList.GetSelectedRows());
      if (string.IsNullOrEmpty (selectedRows))
        selectedRows = "NoneSelected";
      return selectedRows;
    }

    public void SetActionPerformed (string bocListId, int rowIndex, string action, string parameter)
    {
      ActionPerformedSenderLabel.Text = bocListId;
      ActionPerformedSenderRowLabel.Text = rowIndex.ToString();
      ActionPerformedLabel.Text = action;
      ActionPerformedParameterLabel.Text = parameter;
    }
  }
}