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
using Remotion.Utilities;

namespace Remotion.ExtensibleEnums.Infrastructure
{
  /// <summary>
  /// Compares two <see cref="IExtensibleEnumInfo"/> instances based on the <see cref="IExtensibleEnum.ID"/> and 
  /// <see cref="IExtensibleEnumInfo.PositionalKey"/>. This class is used by <see cref="ExtensibleEnumDefinition{T}"/>
  /// for sorting the values returned by <see cref="ExtensibleEnumDefinition{T}.GetValueInfos"/>.
  /// </summary>
  /// <typeparam name="T">The concrete <see cref="IExtensibleEnumInfo"/> implementation.</typeparam>
  public class ExtensibleEnumInfoComparer<T> : IComparer<T>
      where T : IExtensibleEnumInfo
  {
    public static readonly ExtensibleEnumInfoComparer<T> Instance = new ExtensibleEnumInfoComparer<T> ();

    private ExtensibleEnumInfoComparer ()
    {
    }

    public int Compare (T x, T y)
    {
      ArgumentUtility.CheckNotNull ("x", x);
      ArgumentUtility.CheckNotNull ("y", y);

      if (x.PositionalKey != y.PositionalKey)
        return x.PositionalKey.CompareTo (y.PositionalKey);
      else 
        return x.Value.ID.CompareTo (y.Value.ID);
    }
  }
}