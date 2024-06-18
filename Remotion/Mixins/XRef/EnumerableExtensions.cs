// This file is part of the MixinXRef project
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
// 
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
// 
using System;
using System.Collections.Generic;
using System.Linq;

namespace Remotion.Mixins.XRef
{
  public static class EnumerableExtensions
  {
    public static IEnumerable<T> DistinctBy<T, TIdentity> (this IEnumerable<T> enumerable, Func<T, TIdentity> identity)
    {
      return enumerable.Distinct(new DelegateComparer<T, TIdentity>(identity));
    }

    private class DelegateComparer<T, TIdentity> : IEqualityComparer<T>
    {
      private readonly Func<T, TIdentity> _identity;

      public DelegateComparer (Func<T, TIdentity> identity)
      {
        _identity = identity;
      }

      public bool Equals (T? x, T? y)
      {
        return Equals(_identity(x), _identity(y));
      }

      public int GetHashCode (T obj)
      {
        return _identity(obj).GetHashCode();
      }
    }
  }
}
