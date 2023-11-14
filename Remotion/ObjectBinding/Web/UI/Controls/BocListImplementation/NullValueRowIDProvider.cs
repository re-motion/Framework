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

namespace Remotion.ObjectBinding.Web.UI.Controls.BocListImplementation
{
  /// <summary>
  /// Implementation of the <see cref="IRowIDProvider"/> interface used when the <see cref="BocList"/> does not have a value set.
  /// All operations throw <see cref="NotSupportedException"/>.
  /// </summary>
  [Serializable]
  public class NullValueRowIDProvider : IRowIDProvider
  {
    public NullValueRowIDProvider ()
    {
    }

    public string GetControlRowID (BocListRow row)
    {
      throw new NotSupportedException("The operation is not supported because the value is not set.");
    }

    public string GetItemRowID (BocListRow row)
    {
      throw new NotSupportedException("The operation is not supported because the value is not set.");
    }

    public BocListRow? GetRowFromItemRowID (IReadOnlyList<IBusinessObject> rows, string rowID)
    {
      throw new NotSupportedException("The operation is not supported because the value is not set.");
    }

    public void AddRow (BocListRow row)
    {
      throw new NotSupportedException("The operation is not supported because the value is not set.");
    }

    public void RemoveRow (BocListRow row)
    {
      throw new NotSupportedException("The operation is not supported because the value is not set.");
    }
  }
}
