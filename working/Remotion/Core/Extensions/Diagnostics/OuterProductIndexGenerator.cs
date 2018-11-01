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
  /// Allows a class implementing the IOuterProductProcessor interface to visit each member of an outer product of a variable number of independently sized tuples.
  /// From a programmer's view the class supplies "variable number of nested for loops"-functionality.
  /// For convenience derive you processor class from <see cref="OuterProductProcessorBase"/> (see examples below).
  /// </summary>
  /// <include file='..\doc\include\Diagnostics\OuterProduct.xml' path='OuterProductIndexGenerator/ClassExample1/*' />
  /// <include file='..\doc\include\Diagnostics\OuterProduct.xml' path='OuterProductIndexGenerator/ClassExample2/*' />
  public class OuterProductIndexGenerator 
  {
    private int _numberElementsProcessed;
    private int[] _numberElementsPerDimension;
    private int _numberElementsOverall;
    private int[] _currentDimensionIndices;


    ///<overloads>
    ///OuterProductIndexGenerator can be initialized in a general way by passing the number of elements
    ///along each dimension in an integer array, or specialized by passing a rectangular array whose
    ///dimensions shall be used by the outer product.
    ///</overloads>
    /// <summary>
    /// Initializes OuterProductIndexGenerator from an integer array, where each array entry gives the number of elements along its
    /// corresponding dimension. In programers terms: The number of times each nested for-loop will loop.
    /// </summary>
    /// <param name="numberElementsPerDimension">"Number of loops for each for"-loop array</param>
    public OuterProductIndexGenerator (int[] numberElementsPerDimension)
    {
      Init ((int[]) numberElementsPerDimension.Clone ());
    }

    /// <summary>
    /// Initializes OuterProductIndexGenerator from a (rectangular) array. Use to iterate over a rectangular array and access
    /// its members with <c>rectangularArray.GetValue(ProcessingState.DimensionIndices)</c> in the
    /// <see cref="IOuterProductProcessor"/> implementation.
    /// </summary>
    /// <param name="array">Array from whose dimensions the dimensions of the outer product will be initialized.</param>
    public OuterProductIndexGenerator (Array array)
    {
      Init (array);
    }


    /// <summary>
    /// The total number of outer product elements that have been visited by the processor.
    /// </summary>
    public int NumberElementsProcessed
    {
      get { return _numberElementsProcessed; }
    }

    /// <summary>
    /// The number of elements in each outer product dimension.
    /// </summary>
    public int[] NumberElementsPerDimension
    {
      get { return _numberElementsPerDimension; }
    }

    /// <summary>
    /// The total number of elements in the outer product (= the product of all NumberElementsPerDimension entries).
    /// </summary>
    public int NumberElementsOverall
    {
      get { return _numberElementsOverall; }
    }

    /// <summary>
    /// The dimension indices representing the current outer product permutation.
    /// </summary>
    public int[] DimensionIndices
    {
      get { return _currentDimensionIndices; }
    }

    /// <summary>
    /// The total number of combinations in the outer product.
    /// </summary>
    public int Length { get { return NumberElementsOverall; } }


    //TODO: to static, use from ctor-cascading
    private void Init (Array array)
    {
      int numberDimensions = array.Rank;
      int[] numberElementsPerDimension = new int[numberDimensions];
      for (int dimensionIndex = 0; dimensionIndex < numberDimensions; ++dimensionIndex)
      {
        numberElementsPerDimension[dimensionIndex] = array.GetLength (dimensionIndex);
      }
      Init (numberElementsPerDimension);
    }

    private void Init (int[] numberElementsPerDimension)
    {
      _numberElementsPerDimension = numberElementsPerDimension;
      InitProcessing();
    }

    //TODO: inline
    private void InitProcessing ()
    {
      int rank = _numberElementsPerDimension.Length;
      _currentDimensionIndices = new int[rank];
      _numberElementsOverall = CalculateOuterProductNumberElementsOverall (_numberElementsPerDimension);
      _numberElementsProcessed = 0;
    }


    /// <summary>
    /// Calcs the number of elements in an outer product. 
    /// </summary>
    /// <param name="numberElementsPerDimension">The array giving the number of elements along each dimension of the outer product.</param>
    /// <returns>The product of the numbers in the passed array of integers.</returns>
    public static int CalculateOuterProductNumberElementsOverall (int[] numberElementsPerDimension)
    {
      if (numberElementsPerDimension.Length <= 0)
      {
        return 0;
      }
      else
      {
        int numberStateCombinations = 1;
        foreach (var numberElements in numberElementsPerDimension)
        {
          numberStateCombinations *= numberElements;
        }
        return numberStateCombinations;
      }
    }

 
    /// <summary>
    /// The recursive method which implements the variable number of for-loops together with processing callbacks to the outerProductProcessor.
    /// </summary>
    /// <param name="dimensionIndex"></param>
    /// <param name="outerProductProcessor"></param>
    private void ProcessOuterProductRecursive (int dimensionIndex, IOuterProductProcessor outerProductProcessor)
    {
      if (dimensionIndex >= _numberElementsPerDimension.Length)
      {
        return;
      }

      var processingState = new OuterProductProcessingState (this, dimensionIndex);


      for (int currentDimensionIndex = 0; currentDimensionIndex < _numberElementsPerDimension[dimensionIndex]; ++currentDimensionIndex)
      {
        DimensionIndices[dimensionIndex] = currentDimensionIndex;
        
        outerProductProcessor.SetProcessingState (processingState);
        bool continueProcessingBeforeLoop = outerProductProcessor.DoBeforeLoop ();
        if (!continueProcessingBeforeLoop)
        {
          break;
        }
        
        ProcessOuterProductRecursive (dimensionIndex + 1, outerProductProcessor);

        outerProductProcessor.SetProcessingState (processingState);
        bool continueProcessingAfterLoop = outerProductProcessor.DoAfterLoop ();
        if (!continueProcessingAfterLoop)
        {
          break;
        }

        ++_numberElementsProcessed;
      }
    }


    /// <summary>
    /// Call to start the processing of each OuterProductIndexGenerator-element.
    /// </summary>
    /// <param name="outerProductProcessor">An OuterProductIndexGenerator-processor which needs to implement the IOuterProductProcessor interface.</param>
    public void ProcessOuterProduct (IOuterProductProcessor outerProductProcessor)
    {
      //Init (_numberElementsPerDimension);
      //TODO: NO reset in type, builders are one way
      InitProcessing();
      ProcessOuterProductRecursive (0, outerProductProcessor);
    }


  }
}
