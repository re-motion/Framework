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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// A collection of <see cref="IScreenshotTransformation{T}"/>s that can be applied as a <see cref="IScreenshotTransformation{T}"/>.
  /// </summary>
  public class ScreenshotTransformationCollection<T> : ICollection<IScreenshotTransformation<T>>, IScreenshotTransformation<T>
      where T : notnull
  {
    private class TransformationComparer : IComparer<int>
    {
      public int Compare (int x, int y)
      {
        var result = x.CompareTo (y);

        if (result == 0)
          return 1;
        return result;
      }
    }

    public static readonly ScreenshotTransformationCollection<T> EmptyCollection = new ScreenshotTransformationCollection<T> (true);

    private readonly SortedList<int, IScreenshotTransformation<T>> _transformations;
    private readonly HashSet<IScreenshotTransformation<T>> _transformationsSet;

    private bool _isReadOnly;

    public ScreenshotTransformationCollection ()
        : this (false)
    {
    }

    private ScreenshotTransformationCollection (bool isReadOnly)
    {
      _transformations = new SortedList<int, IScreenshotTransformation<T>> (new TransformationComparer());
      _transformationsSet = new HashSet<IScreenshotTransformation<T>>();
      _isReadOnly = isReadOnly;
    }

    /// <inheritdoc />
    public int Count
    {
      get { return _transformations.Count; }
    }

    /// <inheritdoc />
    public bool IsReadOnly
    {
      get { return _isReadOnly; }
    }

    /// <inheritdoc />
    public int ZIndex
    {
      get { return 0; }
    }

    /// <inheritdoc />
    public ScreenshotTransformationContext<T> BeginApply (ScreenshotTransformationContext<T> context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      return _transformations.Aggregate (context, (current, transformation) => transformation.Value.BeginApply (current));
    }

    /// <inheritdoc />
    public void EndApply (ScreenshotTransformationContext<T> context)
    {
      ArgumentUtility.CheckNotNull ("context", context);

      foreach (var transformation in _transformations.Reverse())
        transformation.Value.EndApply (context);
    }

    /// <inheritdoc />
    public IEnumerator<IScreenshotTransformation<T>> GetEnumerator ()
    {
      return _transformations.Select (e => e.Value).GetEnumerator();
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    /// <inheritdoc />
    public void Add ([NotNull] IScreenshotTransformation<T> item)
    {
      ArgumentUtility.CheckNotNull ("item", item);

      if (_isReadOnly)
        throw new InvalidOperationException ("The collection can not be changed as it is read-only.");

      if (_transformationsSet.Contains (item))
        throw new InvalidOperationException ("The specified item is already in the collection.");

      _transformations.Add (item.ZIndex, item);
      _transformationsSet.Add (item);
    }

    /// <inheritdoc />
    public void Clear ()
    {
      if (_isReadOnly)
        throw new InvalidOperationException ("The collection can not be changed as it is read-only.");

      _transformations.Clear();
      _transformationsSet.Clear();
    }

    /// <inheritdoc />
    public bool Contains ([NotNull] IScreenshotTransformation<T> item)
    {
      ArgumentUtility.CheckNotNull ("item", item);

      return _transformationsSet.Contains (item);
    }

    /// <inheritdoc />
    public void CopyTo (IScreenshotTransformation<T>[] array, int arrayIndex)
    {
      ArgumentUtility.CheckNotNull ("array", array);

      _transformations.Select (i => i.Value).ToArray().CopyTo (array, arrayIndex);
    }

    /// <summary>
    /// Locks the <see cref="ScreenshotTransformationCollection{T}"/> for editing.
    /// </summary>
    public void Lock ()
    {
      _isReadOnly = true;
    }

    /// <inheritdoc />
    public bool Remove ([NotNull] IScreenshotTransformation<T> item)
    {
      ArgumentUtility.CheckNotNull ("item", item);

      if (_isReadOnly)
        throw new InvalidOperationException ("The collection can not be changed as it is read-only.");

      var index = _transformations.IndexOfValue (item);
      if (index == -1)
        return false;

      _transformations.RemoveAt (index);
      _transformationsSet.Remove (item);
      return true;
    }
  }
}