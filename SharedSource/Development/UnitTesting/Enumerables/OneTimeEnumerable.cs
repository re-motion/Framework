// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Collections;
using System.Collections.Generic;
using Remotion.Utilities;

#nullable enable
// ReSharper disable once CheckNamespace
namespace Remotion.Development.UnitTesting.Enumerables
{
  /// <summary>
  /// A decorator for <see cref="IEnumerable{T}"/> instances ensuring that they are iterated only once.
  /// </summary>
  /// <typeparam name="T">The element type of the <see cref="IEnumerable{T}"/>.</typeparam>
  public partial class OneTimeEnumerable<T> : IEnumerable<T>
  {
    private class OneTimeEnumerator : IEnumerator<T>
    {
      private readonly IEnumerator<T> _enumerator;

      public OneTimeEnumerator (IEnumerator<T> enumerator)
      {
        ArgumentUtility.CheckNotNull("enumerator", enumerator);
        _enumerator = enumerator;
      }

      public T Current
      {
        get { return _enumerator.Current; }
      }

      object? IEnumerator.Current
      {
        get { return Current; }
      }

      public void Dispose ()
      {
        _enumerator.Dispose();
      }

      public bool MoveNext ()
      {
        return _enumerator.MoveNext();
      }

      public void Reset ()
      {
        throw new NotSupportedException("OneTimeEnumerator does not support Reset().");
      }
    }

    private readonly IEnumerable<T> _enumerable;
    private bool _isUsed = false;

    public OneTimeEnumerable (IEnumerable<T> enumerable)
    {
      ArgumentUtility.CheckNotNull("enumerable", enumerable);
      _enumerable = enumerable;
    }

    /// <inheritdoc />
    public IEnumerator<T> GetEnumerator ()
    {
      if (_isUsed)
        throw new InvalidOperationException("OneTimeEnumerable can only be iterated once.");
      _isUsed = true;

      return new OneTimeEnumerator(_enumerable.GetEnumerator());
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }
  }
}
