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
using System.Linq;
using Remotion.Utilities;

namespace Remotion.Collections
{
  /// <summary>
  /// Wrapper around an <see cref="IEnumerable{T}"/> which supplies element-wise <see cref="Object.Equals(object)"/> and
  /// <see cref="Object.GetHashCode"/> semantics. Use in conjunction with <see cref="CompoundValueEqualityComparer{T}"/>
  /// to get value based semantics for container class members.
  /// </summary>
  /// <typeparam name="TElement"></typeparam>
  public class ComponentwiseEqualsAndHashcodeWrapper<TElement> : IEnumerable<TElement>
  {
    private readonly IEnumerable<TElement> _enumerable;

    public ComponentwiseEqualsAndHashcodeWrapper (IEnumerable<TElement> enumerable)
    {
      _enumerable = enumerable;
    }

    public IEnumerable<TElement> Enumerable
    {
      get { return _enumerable; }
    }


    /// <summary>
    /// Compares the elements of the <see cref="ComponentwiseEqualsAndHashcodeWrapper{TElement}"/> for equality, if the passed <see cref="object"/> 
    /// is an <see cref="ComponentwiseEqualsAndHashcodeWrapper{TElement}"/> .
    /// </summary>
    public override bool Equals (object obj)
    {
      if (ReferenceEquals (null, obj))
      {
        return false;
      }
      else if (ReferenceEquals (this, obj))
      {
        return true;
      }
      else if (obj is ComponentwiseEqualsAndHashcodeWrapper<TElement>)
      {
        var enumerableEqualsWrapper = (ComponentwiseEqualsAndHashcodeWrapper<TElement>) obj;
        return enumerableEqualsWrapper.Enumerable.Cast<object> ().SequenceEqual (Enumerable.Cast<object> ());
      }
      else
      {
        return false;
      }
    }


    public IEnumerator<TElement> GetEnumerator ()
    {
      return _enumerable.GetEnumerator ();
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator ();
    }

    /// <summary>
    /// Returns a hash code based on the members of the <see cref="IEnumerable{T}"/>.
    /// </summary>
    public override int GetHashCode ()
    {
      return EqualityUtility.GetRotatedHashCode (_enumerable);
    }
  }

  /// <summary>
  /// ComponentwiseEqualsAndHashcodeWrapper-factory: ComponentwiseEqualsAndHashcodeWrapper.New(<see cref="IEnumerable{T}"/>).
  /// </summary>
  public static class ComponentwiseEqualsAndHashcodeWrapper 
  {
    public static ComponentwiseEqualsAndHashcodeWrapper<T> New<T> (IEnumerable<T> elements)
    {
      return new ComponentwiseEqualsAndHashcodeWrapper<T> (elements);
    }
  }
}
