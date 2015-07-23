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

namespace Remotion.Diagnostics
{
  /// <summary>
  /// The current state of the outer product / nested for loops traversal.
  /// DimensionIndices supplies the current permutation of indices (array with an entry for each for-loop).
  /// DimensionIndex is the currently running for-loop; ElementIndex (=DimensionIndices[DimensionIndex]) is the value of the loop-variable of the currently running for-loop.
  /// IsInnermostLoop, IsOutermostLoop can be queried to treat the innermost and outermost loop differently, if so required.
  /// </summary>
  public struct OuterProductProcessingState
  {
    /// <summary>
    /// Initializes a ProcessingState with an OuterProductIndexGenerator reference and the current dimension index 
    /// (= nested-for-loop loop-variable index).
    /// </summary>
    public OuterProductProcessingState (OuterProductIndexGenerator outerProductIndexGenerator, int dimensionIndex)
    {
      _outerProductIndexGenerator = outerProductIndexGenerator;
      _dimensionIndex = dimensionIndex;
    }

    private readonly OuterProductIndexGenerator _outerProductIndexGenerator;
    private readonly int _dimensionIndex;

    /// <summary>
    /// The outer product dimension which is currently processed (i.e. the index of the currently running for-loop).
    /// </summary>
    public int DimensionIndex
    {
      get { return _dimensionIndex; }
    }

    /// <summary>
    /// Integer array containing the number of elements in each outer product dimension.
    /// </summary>
    public int[] NumberElementsPerDimension
    {
      get { return _outerProductIndexGenerator.NumberElementsPerDimension; }
    }

    /// <summary>
    /// Integer array containing the current permutation of outer product indices (i.e. each array entry is the current value of each for-loop variable;
    /// <see cref="ElementIndex"/>).
    /// </summary>
    public int[] DimensionIndices
    {
      get { return _outerProductIndexGenerator.DimensionIndices; }
    }

    /// <summary>
    /// The overall number of elements in the outer product.
    /// </summary>
    public int NumberElementsOverall
    {
      get { return _outerProductIndexGenerator.NumberElementsOverall; }
    }

    /// <summary>
    /// ElementIndex (=DimensionIndices[DimensionIndex]) is the value of the loop-variable of the currently running for-loop.
    /// </summary>
    public int ElementIndex
    {
      get { return _outerProductIndexGenerator.DimensionIndices[_dimensionIndex]; }
    }

    /// <summary>
    /// Whether the element is the first element in the current for-loop.
    /// </summary>
    public bool IsFirstLoopElement
    {
      get { return ElementIndex == 0; }
    }

    /// <summary>
    /// Whether the element is the last element in the current for-loop.
    /// </summary>
    public bool IsLastLoopElement
    {
      get { return ElementIndex == (NumberElementsPerDimension[DimensionIndex] - 1); }
    }

    /// <summary>
    /// Whether the current for-loop is the innermost loop.
    /// </summary>
    public bool IsInnermostLoop
    {
      get { return DimensionIndex == (NumberElementsPerDimension.Length - 1); }
    }

    /// <summary>
    /// Whether the current for-loop is the outermost loop.
    /// </summary>
    public bool IsOutermostLoop
    {
      get { return DimensionIndex == 0; }
    }

    /// <summary>
    /// The overall elements of the outer product which have already been processed.
    /// </summary>
    public int NumberElementsProcessed
    {
      get { return _outerProductIndexGenerator.NumberElementsProcessed; }
    }

    /// <summary>
    /// Returns a copy of the current <see cref="DimensionIndices"/>-array.
    /// Use if you want to e.g. store the generated dimension indices permutations in your own collection.
    /// </summary>
    public int[] GetDimensionIndicesCopy ()
    {
      return (int[]) DimensionIndices.Clone();
    }
  }
}
