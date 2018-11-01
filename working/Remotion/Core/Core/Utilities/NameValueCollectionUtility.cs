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
using System.Collections.Specialized;

namespace Remotion.Utilities
{
  /// <summary>
  ///   Utility class for <see cref="NameValueCollection"/>
  /// </summary>
  public static class NameValueCollectionUtility
  {
    public static NameValueCollection Clone (this NameValueCollection collection)
    {
      return new NameValueCollection (collection);
    }

    /// <summary>
    ///   Adds the second dictionary to the first. If a key occurs in both dictionaries, the value of the second
    ///   dictionaries is taken.
    /// </summary>
    /// <param name="first"> Must not be <see langword="null"/>. </param>
    /// <param name="second"> Must not be <see langword="null"/>. </param>
    public static void Append (NameValueCollection first, NameValueCollection second)
    {
      ArgumentUtility.CheckNotNull ("first", first);
      
      if (second != null)
      {
        for (int i = 0; i < second.Count; i++)
          first.Set (second.GetKey(i), second.Get(i));
      }
    }

    /// <summary>
    ///   Merges two collections. If a key occurs in both collections, the value of the second collections is taken.
    /// </summary>
    public static NameValueCollection Merge (NameValueCollection first, NameValueCollection second)
    {
      if (first == null && second == null)
        return null;
      else if (first != null && second == null)
        return Clone (first);
      else if (first == null && second != null)
        return Clone (second);

      NameValueCollection result = Clone (first);
      Append (result, second);
      return result;
    }
  }
}
