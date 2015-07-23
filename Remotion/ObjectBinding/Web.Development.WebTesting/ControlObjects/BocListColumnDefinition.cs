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
using Remotion.Web.Development.WebTesting;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlObjects
{
  /// <summary>
  /// Defines a list's column.
  /// </summary>
  public class BocListColumnDefinition<TRowControlObject, TCellControlObject>
      where TRowControlObject : ControlObject
      where TCellControlObject : ControlObject
  {
    private readonly string _itemID;
    private readonly int _index;
    private readonly string _title;
    private readonly bool _hasDiagnosticMetadata;

    public BocListColumnDefinition (string itemID, int index, string title, bool hasDiagnosticMetadata)
    {
      _itemID = itemID;
      _index = index;
      _title = title;
      _hasDiagnosticMetadata = hasDiagnosticMetadata;
    }

    public string ItemID
    {
      get { return _itemID; }
    }

    public int Index
    {
      get { return _index; }
    }

    public string Title
    {
      get { return _title; }
    }

    public bool HasDiagnosticMetadata
    {
      get { return _hasDiagnosticMetadata; }
    }
  }
}