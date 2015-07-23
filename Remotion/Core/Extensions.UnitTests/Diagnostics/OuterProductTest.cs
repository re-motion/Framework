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
using System.Text;
using NUnit.Framework;
using Remotion.Diagnostics;

namespace Remotion.Extensions.UnitTests.Diagnostics
{
  [TestFixture]
  public class OuterProductTest
  {
    /// <summary>
    /// OuterProductIndexGenerator.IOuterProductProcessor implementations
    /// </summary>
    private abstract class OuterProductProcessorBase : Remotion.Diagnostics.OuterProductProcessorBase
    {
      protected readonly Array rectangularArray;
      protected readonly StringBuilder result = new StringBuilder();

      protected OuterProductProcessorBase (Array rectangularArray)
      {
        this.rectangularArray = rectangularArray;
      }

      public virtual String GetResult ()
      {
        return result.ToString();
      }
    }


    private class OuterProductProcessorOneLineString : OuterProductProcessorBase
    {
      public OuterProductProcessorOneLineString (Array rectangularArray)
          : base (rectangularArray)
      {
      }

      public override bool DoBeforeLoop ()
      {
        if (ProcessingState.IsInnermostLoop)
        {
          result.Append (ProcessingState.IsFirstLoopElement ? "" : ",");
          result.Append (CollectionToSequenceString (ProcessingState.DimensionIndices));
        }
        else
        {
          result.Append (ProcessingState.IsFirstLoopElement ? "" : ","); // Seperator only if not first element
          result.Append ("{");
        }
        return true;
      }

      public override bool DoAfterLoop ()
      {
        if (!ProcessingState.IsInnermostLoop)
          result.Append ("}");
        return true;
      }
    }


    private class OuterProductProcessorPrettyPrinter : OuterProductProcessorBase
    {
      public OuterProductProcessorPrettyPrinter (Array rectangularArray)
          : base (rectangularArray)
      {
      }

      protected void AppendIndentedLine (string text)
      {
        result.AppendLine();
        result.Append (' ', ProcessingState.DimensionIndex);
        result.Append (text);
      }

      protected virtual void AppendInnermostLoop ()
      {
        AppendIndentedLine (CollectionToSequenceString (ProcessingState.DimensionIndices));
      }

      public override bool DoBeforeLoop ()
      {
        if (ProcessingState.IsInnermostLoop)
          AppendInnermostLoop();
        else
          AppendIndentedLine ("{");
        return true;
      }

      public override bool DoAfterLoop ()
      {
        if (!ProcessingState.IsInnermostLoop)
          AppendIndentedLine ("}");
        return true;
      }
    }


    private class OuterProductProcessorArrayPrettyPrinter : OuterProductProcessorPrettyPrinter
    {
      public OuterProductProcessorArrayPrettyPrinter (Array rectangularArray)
          : base (rectangularArray)
      {
      }

      protected override void AppendInnermostLoop ()
      {
        AppendIndentedLine (rectangularArray.GetValue (ProcessingState.DimensionIndices).ToString());
      }
    }


    private class OuterProductProcessorArrayPrinter : OuterProductProcessorBase
    {
      public int NumberElementsToOutputInnermost { get; set; }
      public int NumberElementsToOutputAllButInnermost { get; set; }
      public int NumberElementsToOutputOverall { get; set; }

      public OuterProductProcessorArrayPrinter (Array rectangularArray)
          : base (rectangularArray)
      {
        NumberElementsToOutputInnermost = -1;
        NumberElementsToOutputAllButInnermost = -1;
        NumberElementsToOutputOverall = -1;
      }

      protected virtual void AppendInnermostLoop ()
      {
        result.Append (rectangularArray.GetValue (ProcessingState.DimensionIndices).ToString());
      }

      public override bool DoBeforeLoop ()
      {
        if (ProcessingState.IsInnermostLoop)
        {
          result.Append (ProcessingState.IsFirstLoopElement ? "" : ",");
          if (NumberElementsToOutputInnermost > 0 && ProcessingState.ElementIndex >= NumberElementsToOutputInnermost)
          {
            result.Append ("...");
            return false;
          }
          AppendInnermostLoop();
        }
        else
        {
          result.Append (ProcessingState.IsFirstLoopElement ? "" : ","); // Seperator only if not first element
          if (NumberElementsToOutputAllButInnermost > 0 && ProcessingState.ElementIndex >= NumberElementsToOutputAllButInnermost)
          {
            result.Append ("...");
            return false;
          }
          result.Append ("{");
        }
        return true;
      }

      public override bool DoAfterLoop ()
      {
        if (!ProcessingState.IsInnermostLoop)
          result.Append ("}");
        if (NumberElementsToOutputOverall > 0 && ProcessingState.NumberElementsProcessed >= NumberElementsToOutputOverall)
        {
          result.Append (",...");
          return false;
        }
        return true;
      }
    }

    /// <summary>
    /// OuterProductIndexGenerator.IOuterProductProcessor used in OuterProductIndexGenerator "pretty print rectangular arrays of arbitrary dimensions" code sample
    /// </summary>
    public class RectangularArrayToString : Remotion.Diagnostics.OuterProductProcessorBase
    {
      private readonly Array RectangularArray;
      public readonly StringBuilder Result = new StringBuilder(); // To keep sample concise

      public RectangularArrayToString (Array rectangularArray)
      {
        RectangularArray = rectangularArray;
      }

      public override bool DoBeforeLoop ()
      {
        if (ProcessingState.IsInnermostLoop)
        {
          Result.Append (ProcessingState.IsFirstLoopElement ? "" : ",");
          Result.Append (RectangularArray.GetValue (ProcessingState.DimensionIndices).ToString());
        }
        else
        {
          Result.Append (ProcessingState.IsFirstLoopElement ? "" : ",");
          Result.Append ("{");
        }
        return true;
      }

      public override bool DoAfterLoop ()
      {
        if (!ProcessingState.IsInnermostLoop)
          Result.Append ("}");
        return true;
      }
    }


    /// <summary>
    /// OuterProductIndexGenerator.IOuterProductProcessor used in OuterProductIndexGenerator "create outer prodcut permutations" code sample
    /// </summary>
    public class OuterProductPermutations : Remotion.Diagnostics.OuterProductProcessorBase
    {
      public readonly List<int[]> outerProductPermutations = new List<int[]>(); // To keep sample concise

      public override bool DoBeforeLoop ()
      {
        if (ProcessingState.IsInnermostLoop)
        {
          //Log (CollectionToSequenceString (ProcessingState.DimensionIndices));
          outerProductPermutations.Add (ProcessingState.GetDimensionIndicesCopy());
        }
        return true;
      }
    }


    /// <summary>
    /// Helper function to convert a collection into a human-readable string; use To.Text-facility instead as soon as it is fully implemented.
    /// </summary>
    private static string CollectionToSequenceString (IEnumerable collection, string start, string seperator, string end)
    {
      var sb = new StringBuilder();

      sb.Append (start);
      bool insertSeperator = false; // no seperator before first element
      foreach (Object element in collection)
      {
        if (insertSeperator)
          sb.Append (seperator);
        else
          insertSeperator = true;

        sb.Append (element.ToString());
      }
      sb.Append (end);
      return sb.ToString();
    }


    private static string CollectionToSequenceString (IEnumerable collection)
    {
      return CollectionToSequenceString (collection, "(", ",", ")");
    }


    /// <summary>
    /// OuterProductIndexGenerator tests be here...
    /// </summary>
    [Test]
    public void NumberElementsPerDimensionCtorTest ()
    {
      int[] arrayDimensions = new int[] { 5, 7, 11 };
      var outerProduct = new OuterProductIndexGenerator (arrayDimensions);
      Assert.That (outerProduct.Length, Is.EqualTo (5*7*11));
    }


    [Test]
    public void ArrayCtorTest ()
    {
      String[,] rectangularArray = new string[,] { { "A1", "A2" }, { "B1", "B2" }, { "C1", "C2" } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      Assert.That (outerProduct.Length, Is.EqualTo (3*2));
    }


    [Test]
    public void NestedForLoopsTest ()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append ("{");
      for (int i0 = 0; i0 < 2; ++i0)
      {
        sb.Append (i0 == 0 ? "" : ",");
        sb.Append ("{");
        for (int i1 = 0; i1 < 3; ++i1)
        {
          sb.Append (i1 != 0 ? "," : "");
          sb.Append ("(" + i0 + "," + i1 + ")");
        }
        sb.Append ("}");
      }
      sb.Append ("}");
      string s = sb.ToString();
      Assert.That (s, Is.EqualTo ("{{(0,0),(0,1),(0,2)},{(1,0),(1,1),(1,2)}}"));
    }


    [Test]
    public void VisitorNestedForTest ()
    {
      String[,] rectangularArray = new string[,] { { null, null, null }, { null, null, null } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      var processor = new OuterProductProcessorOneLineString (rectangularArray);
      outerProduct.ProcessOuterProduct (processor);
      string s = processor.GetResult();
      Assert.That (s, Is.EqualTo ("{(0,0),(0,1),(0,2)},{(1,0),(1,1),(1,2)}"));
    }


    [Test]
    public void PermutationVisitorTest ()
    {
      var dimensionArray = new int[] { 2, 3, 2 };
      var outerProduct = new OuterProductIndexGenerator (dimensionArray);
      var processor = new OuterProductProcessorOneLineString (null);
      outerProduct.ProcessOuterProduct (processor);
      string s = processor.GetResult();
      Assert.That (s, Is.EqualTo ("{{(0,0,0),(0,0,1)},{(0,1,0),(0,1,1)},{(0,2,0),(0,2,1)}},{{(1,0,0),(1,0,1)},{(1,1,0),(1,1,1)},{(1,2,0),(1,2,1)}}"));
    }


    [Test]
    public void VisitorNestedForTest2 ()
    {
      var rectangularArray = new string[,,] { { { null, null }, { null, null }, { null, null } }, { { null, null }, { null, null }, { null, null } } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      var processor = new OuterProductProcessorOneLineString (rectangularArray);
      outerProduct.ProcessOuterProduct (processor);
      string s = processor.GetResult();
      Assert.That (s, Is.EqualTo ("{{(0,0,0),(0,0,1)},{(0,1,0),(0,1,1)},{(0,2,0),(0,2,1)}},{{(1,0,0),(1,0,1)},{(1,1,0),(1,1,1)},{(1,2,0),(1,2,1)}}"));
    }

    [Test]
    public void VisitorNestedForOuterProductProcessorPrettyPrinterTest ()
    {
      var rectangularArray = new string[,,] { { { null, null }, { null, null }, { null, null } }, { { null, null }, { null, null }, { null, null } } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      var processor = new OuterProductProcessorPrettyPrinter (rectangularArray);
      outerProduct.ProcessOuterProduct (processor);
      string s = processor.GetResult();
      const string resultExpected =
          @"
{
 {
  (0,0,0)
  (0,0,1)
 }
 {
  (0,1,0)
  (0,1,1)
 }
 {
  (0,2,0)
  (0,2,1)
 }
}
{
 {
  (1,0,0)
  (1,0,1)
 }
 {
  (1,1,0)
  (1,1,1)
 }
 {
  (1,2,0)
  (1,2,1)
 }
}";
      Assert.That (s, Is.EqualTo (resultExpected));
    }


    [Test]
    public void VisitorNestedForOuterProductProcessorArrayPrettyPrinterTest ()
    {
      var rectangularArray = new string[,,] { { { "A0", "A1" }, { "B0", "B1" }, { "C0", "C1" } }, { { "D0", "D1" }, { "E0", "E1" }, { "F0", "F1" } } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      var processor = new OuterProductProcessorArrayPrettyPrinter (rectangularArray);
      outerProduct.ProcessOuterProduct (processor);
      string s = processor.GetResult();
      const string resultExpected =
          @"
{
 {
  A0
  A1
 }
 {
  B0
  B1
 }
 {
  C0
  C1
 }
}
{
 {
  D0
  D1
 }
 {
  E0
  E1
 }
 {
  F0
  F1
 }
}";
      Assert.That (s, Is.EqualTo (resultExpected));
    }


    [Test]
    public void RectangularArrayVisitorTest ()
    {
      String[,] rectangularArray = new string[,] { { "A1", "A2", "A3" }, { "B1", "B2", "B3" }, { "C1", "C2", "C3" } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      var processor = new OuterProductProcessorArrayPrinter (rectangularArray);
      outerProduct.ProcessOuterProduct (processor);
      string result = processor.GetResult();
      Assert.That (result, Is.EqualTo ("{A1,A2,A3},{B1,B2,B3},{C1,C2,C3}"));
    }

    [Test]
    public void RectangularArrayTerminatingVisitorTest ()
    {
      String[,] rectangularArray = new string[,] { { "A1", "A2", "A3" }, { "B1", "B2", "B3" }, { "C1", "C2", "C3" } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      var processor = new OuterProductProcessorArrayPrinter (rectangularArray);
      processor.NumberElementsToOutputInnermost = 2;
      outerProduct.ProcessOuterProduct (processor);
      string result = processor.GetResult();
      Assert.That (result, Is.EqualTo ("{A1,A2,...},{B1,B2,...},{C1,C2,...}"));
    }


    [Test]
    public void RectangularArrayTerminatingVisitorTest2 ()
    {
      String[,] rectangularArray = new string[,] { { "A1", "A2", "A3" }, { "B1", "B2", "B3" }, { "C1", "C2", "C3" }, { "D1", "D2", "D3" } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      var processor = new OuterProductProcessorArrayPrinter (rectangularArray);
      processor.NumberElementsToOutputInnermost = 2;
      processor.NumberElementsToOutputAllButInnermost = 3;
      outerProduct.ProcessOuterProduct (processor);
      string result = processor.GetResult();
      Assert.That (result, Is.EqualTo ("{A1,A2,...},{B1,B2,...},{C1,C2,...},..."));
    }

    [Test]
    public void RectangularArrayTerminatingVisitorTest3 ()
    {
      String[,] rectangularArray = new string[,] { { "A1", "A2", "A3" }, { "B1", "B2", "B3" }, { "C1", "C2", "C3" }, { "D1", "D2", "D3" } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      var processor = new OuterProductProcessorArrayPrinter (rectangularArray);
      processor.NumberElementsToOutputInnermost = 2;
      processor.NumberElementsToOutputAllButInnermost = 3;
      processor.NumberElementsToOutputOverall = 5;
      outerProduct.ProcessOuterProduct (processor);
      string result = processor.GetResult();
      Assert.That (result, Is.EqualTo ("{A1,A2,...},{B1,B2,...},..."));
    }


    [Test]
    public void ProcessSameOuterProductMultipleTimesTest ()
    {
      String[,] rectangularArray = new string[,] { { "A1", "A2", "A3" }, { "B1", "B2", "B3" }, { "C1", "C2", "C3" }, { "D1", "D2", "D3" } };
      var outerProduct = new OuterProductIndexGenerator (rectangularArray);
      OuterProductProcessorArrayPrinter processor = null;
      int i = 0;
      for (; i < 3; ++i)
      {
        processor = new OuterProductProcessorArrayPrinter (rectangularArray);
        outerProduct.ProcessOuterProduct (processor);
      }
      Assert.That (i, Is.EqualTo (3));
      string result = processor.GetResult();
      Assert.That (result, Is.EqualTo ("{A1,A2,A3},{B1,B2,B3},{C1,C2,C3},{D1,D2,D3}"));
    }


    [Test]
    public void SampleRectangularArrayToStringTest ()
    {
      List<String> resultStrings = new List<string> { "A1,A2,A3", "{A1,A2,A3},{B1,B2,B3},{C1,C2,C3}", "{{A1,A2},{B1,B2}},{{C1,C2},{D1,D2}}" };

      Array rectangularArray1D = new string[] { "A1", "A2", "A3" };
      Array rectangularArray2D = new string[,] { { "A1", "A2", "A3" }, { "B1", "B2", "B3" }, { "C1", "C2", "C3" } };
      Array rectangularArray3D = new string[,,] { { { "A1", "A2" }, { "B1", "B2" } }, { { "C1", "C2" }, { "D1", "D2" } } };
      var arrays = new List<Array>() { rectangularArray1D, rectangularArray2D, rectangularArray3D };
      foreach (var array in arrays)
      {
        var outerProduct = new OuterProductIndexGenerator (array);
        var processor = new RectangularArrayToString (array);
        outerProduct.ProcessOuterProduct (processor);
        //System.Console.WriteLine (processor.Result.ToString());

        string result = processor.Result.ToString();
        Assert.That (new List<String> { result }, Is.SubsetOf (resultStrings));
      }
    }


    [Test]
    public void SamplePermutationVisitorTest ()
    {
      var dimensionArray = new int[] { 2, 3, 2 };
      var outerProduct = new OuterProductIndexGenerator (dimensionArray);
      var processor = new OuterProductPermutations();
      outerProduct.ProcessOuterProduct (processor);
      var result = processor.outerProductPermutations;

      var resultExpected = new int[][]
                           {
                               new int[] { 0, 0, 0 }, new int[] { 0, 0, 1 }, new int[] { 0, 1, 0 }, new int[] { 0, 1, 1 }, new int[] { 0, 2, 0 },
                               new int[] { 0, 2, 1 }, new int[] { 1, 0, 0 }, new int[] { 1, 0, 1 }, new int[] { 1, 1, 0 }, new int[] { 1, 1, 1 },
                               new int[] { 1, 2, 0 }, new int[] { 1, 2, 1 }
                           };
      Assert.That (result.ToArray(), Is.EqualTo (resultExpected));
    }
  }
}
