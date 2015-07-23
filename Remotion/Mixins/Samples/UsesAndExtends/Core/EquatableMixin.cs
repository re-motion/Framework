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
using System.Reflection;
using Remotion.Utilities;

namespace Remotion.Mixins.Samples.UsesAndExtends.Core
{
  public class EquatableMixin<[BindToTargetType]T> : Mixin<T>, IEquatable<T>
     where T : class
  {
    private static readonly FieldInfo[] s_targetFields;

    static EquatableMixin()
    {
      s_targetFields = typeof (T).GetFields (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    }

    bool IEquatable<T>.Equals (T other)
    {
      if (other == null)
        return false;

      for (int i = 0; i < s_targetFields.Length; i++)
      {
        object thisFieldValue = s_targetFields[i].GetValue (Target);
        object otherFieldValue = s_targetFields[i].GetValue (other);
        
        if (!Equals (thisFieldValue, otherFieldValue))
          return false;
      }

      return true;
    }

    [OverrideTarget]
    protected new bool Equals (object other)
    {
      return ((IEquatable<T>)this).Equals (other as T);
    }

    [OverrideTarget]
    protected new int GetHashCode ()
    {
      var fieldValues = new object[s_targetFields.Length];
      for (int i = 0; i < fieldValues.Length; ++i)
        fieldValues[i] = s_targetFields[i].GetValue (Target);
      
      return EqualityUtility.GetRotatedHashCode (fieldValues);
    }
  }
}
