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
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocListImplementation
{
  public class FakeRowIDProvider : IRowIDProvider
  {
    public FakeRowIDProvider ()
    {
    }

    public Action<BocListRow> NotifyAddRow { private get; set; }
    public Action<BocListRow> NotifyRemoveRow { private get; set; }

    public string GetControlRowID (BocListRow row)
    {
      return row.Index.ToString();
    }

    public string GetItemRowID (BocListRow row)
    {
      return row.Index.ToString();
    }

    public BocListRow GetRowFromItemRowID (IReadOnlyList<IBusinessObject> rows, string rowID)
    {
      var rowIndex = int.Parse(rowID);
      if (rowIndex >= rows.Count)
        return null;
      var obj = rows[rowIndex];
      return new BocListRow(rowIndex, obj);
    }

    public void AddRow (BocListRow row)
    {
      if (NotifyAddRow != null)
        NotifyAddRow(row);
    }

    public void RemoveRow (BocListRow row)
    {
      if (NotifyRemoveRow != null)
        NotifyRemoveRow(row);
    }
  }
}
