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
using System.Diagnostics;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  public class PerformanceTestHelper
  {
    public static void TimeAndOutput (long testRuns, string testName, Func<bool> operation)
    {
      Console.WriteLine ("Executing {0}...", testName);
      var result = GetResult (testRuns, operation);
      double averageMilliSeconds = result.TotalMilliseconds / testRuns;
      Console.WriteLine (string.Format ("{0} (executed {1}x): Average duration: {2} ms", testName, testRuns, averageMilliSeconds.ToString ("n")));
    }

    public static TimeSpan GetResult (long testRuns, Func<bool> operation)
    {
      var stopwatch = Stopwatch.StartNew();
      var result = true;
      for (long i = 0; i < testRuns; ++i)
	    {
	      result &= operation();
	    }
      stopwatch.Stop();
	
	    Console.WriteLine (result);
	
	    var elapsed = stopwatch.Elapsed;
	    return elapsed;
    }

  }
}