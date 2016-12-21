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
using System.Collections;
using System.Collections.Generic;

namespace Remotion.Mixins.Utilities
{
  /// <summary>
  /// Wraps an enumerable object so that the wrapped object cannot be cast back to its original type.
  /// </summary>
  /// <typeparam name="T">The type returned by the wrapped enumerable object.</typeparam>
  /// <remarks>Use this class when returning an enumerable object from a method to prevent that the object can be cast to its original type.
  /// That way, it will be ensured that the returned object only supports the methods exposed by the <see cref="IEnumerable{T}"/> interface.</remarks>
  public sealed class UncastableEnumerableWrapper<T> : IEnumerable<T>
  {
    private IEnumerable<T> _wrapped;

    public UncastableEnumerableWrapper (IEnumerable<T> wrapped)
    {
      _wrapped = wrapped;
    }

    public IEnumerator<T> GetEnumerator ()
    {
      return _wrapped.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}
