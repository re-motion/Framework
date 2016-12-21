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

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  [TestFixture]
  public class HasRelationChangedTest : DatabaseTest
  {
    public const int TestSetSize = 1000;
    public const int TestRepititions = 100;

    [Test]
    public void AskChanged ()
    {
      Console.WriteLine ("Expected average duration of HasRelationChangedTest on reference system: ~8 ms");

      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var objects = TestDomainObjectMother.PrepareDatabaseObjectsWithRelationProperties (TestSetSize);
        bool changed = ClientTransaction.Current.HasChanged();

        Assert.That (changed, Is.False);

        var stopwatch = new Stopwatch();
        stopwatch.Start();
        for (int i = 0; i < TestRepititions; i++)
          changed ^= ClientTransaction.Current.HasChanged();
        stopwatch.Stop();

        Console.WriteLine (changed);

        double averageMilliSeconds = stopwatch.Elapsed.TotalMilliseconds / TestRepititions;
        Console.WriteLine (
            "HasRelationChangedTest (executed {0} x ClientTransaction.Current.HasChanged ({2} objects - total {3} objects in CTx)): Average duration: {1} ms",
            TestRepititions,
            averageMilliSeconds.ToString ("n"),
            objects.Length,
            ClientTransaction.Current.EnlistedDomainObjectCount);
      }
    }
  }
}