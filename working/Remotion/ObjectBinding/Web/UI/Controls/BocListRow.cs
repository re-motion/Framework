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
using Remotion.Utilities;

namespace Remotion.ObjectBinding.Web.UI.Controls
{
  public sealed class BocListRow : IEquatable<BocListRow>
  {
    public static bool operator== (BocListRow row1, BocListRow row2)
    {
      return Equals (row1, row2);
    }

    public static bool operator!= (BocListRow row1, BocListRow row2)
    {
      return !(row1 == row2);
    }

    private readonly int _index;
    private readonly IBusinessObject _businessObject;

    public BocListRow (int index, IBusinessObject businessObject)
    {
      if (index < 0)
        throw new ArgumentOutOfRangeException ("index", index, "Negative indices are not allowed.");
      ArgumentUtility.CheckNotNull ("businessObject", businessObject);

      _index = index;
      _businessObject = businessObject;
    }

    public int Index
    {
      get { return _index; }
    }

    public IBusinessObject BusinessObject
    {
      get { return _businessObject; }
    }

    public bool Equals (BocListRow other)
    {
      if (other == null)
        return false;

      return this._index == other._index && this._businessObject.Equals (other._businessObject);
    }

    public override bool Equals (object obj)
    {
      return Equals (obj as BocListRow);
    }

    public override int GetHashCode ()
    {
      return _index.GetHashCode();
    }
  }
}