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
using System.IO;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting
{
  /// <summary>
  /// Counts the number of garbage collections that occured between <see cref="BeginCount"/> and <see cref="EndCount"/> 
  /// and prints the result using <see cref="PrintCount"/>.
  /// </summary>
  public class GCCounter
  {
    private static readonly int s_generationCount = GC.MaxGeneration + 1;

    private readonly int[] _totalCollections = new int[s_generationCount];
    private int[] _oldCollections = new int[s_generationCount];
    
    public GCCounter ()
    {
    }

    public void BeginCount()
    {
      GC.Collect();
      for (int i = 0; i < s_generationCount; i++)
        _oldCollections[i] = GC.CollectionCount (i);
    }

    public void EndCount ()
    {
      var newCollections = new int[s_generationCount];
      for (int i = 0; i < s_generationCount; i++)
        newCollections[i] = GC.CollectionCount (i);

      for (int i = 0; i < s_generationCount; i++)
       _totalCollections[i] = newCollections[i] - _oldCollections[i];

      _oldCollections = new int[s_generationCount];
    }

    public void PrintCount (TextWriter outputWriter)
    {
      ArgumentUtility.CheckNotNull ("outputWriter", outputWriter);

      var output = new string[s_generationCount];
      for (int i = 0; i < s_generationCount; i++)
        output[i] = string.Format ("GC Gen {0}: {1}x", i, _totalCollections[i]);

      var combinedOutput = string.Join (", ", output);
      
      outputWriter.WriteLine (combinedOutput);
    }
  }
}