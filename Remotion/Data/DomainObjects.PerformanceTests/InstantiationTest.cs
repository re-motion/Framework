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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.PerformanceTests.TestDomain;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  [TestFixture]
  public class InstantiationTest : DatabaseTest
  {
    public const int TestRepititions = 100000;

    [Test]
    public void GetObjectReference ()
    {
      Console.WriteLine ("Expected average duration of GetObjectReference on reference system: ~15 µs (release build), ~20 µs (debug build)");

      bool found = true;
      var stopwatch = new Stopwatch ();
      
      var transaction = ClientTransaction.CreateRootTransaction ();
      LifetimeService.GetObjectReference (transaction, new ObjectID(typeof (Person), Guid.NewGuid ()));

      stopwatch.Start ();
      for (int i = 0; i < TestRepititions; i++)
        found &= LifetimeService.GetObjectReference (transaction, new ObjectID(typeof (Person), Guid.NewGuid())) != null;
      stopwatch.Stop ();

      Console.WriteLine (found);

      double averageMicroseconds = stopwatch.Elapsed.TotalMilliseconds * 1000.0 / TestRepititions;
      Console.WriteLine (
          "GetObjectReference (executed {0}x): Average duration: {1} µs",
          TestRepititions,
          averageMicroseconds.ToString ("n"));
    }
  }
}