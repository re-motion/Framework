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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.PerformanceTests.TestDomain;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  [TestFixture]
  public class CommitTest : DatabaseTest
  {
    public const int TestSetSize = 1000;
    public const int TestRepititions = 20;

    [Test]
    public void CommitSubTransaction_Relations ()
    {
      Console.WriteLine ("Expected average duration of CommitSubTransaction_Relations on reference system: ~150 ms");

      using (ClientTransaction.CreateRootTransaction ().CreateSubTransaction().EnterDiscardingScope ())
      {
        ClassWithRelationProperties[] objects = TestDomainObjectMother.PrepareDatabaseObjectsWithRelationProperties (TestSetSize);

        Assert.That (ClientTransaction.Current.HasChanged (), Is.False);

        // warm up
        ChangeObjectsWithRelations (objects);
        ClientTransaction.Current.Commit ();

        var stopwatch = new Stopwatch();

        for (int i = 0; i < TestRepititions; i++)
        {
          ChangeObjectsWithRelations (objects);
          stopwatch.Start ();
          ClientTransaction.Current.Commit ();
          stopwatch.Stop ();
        }

        Assert.That (ClientTransaction.Current.HasChanged (), Is.False);

        double averageMilliSeconds = stopwatch.Elapsed.TotalMilliseconds / TestRepititions;
        Console.WriteLine (
            "CommitSubTransaction_Relations (executed {0} x Commit ({2} objects - total {3} objects in CTx)): Average duration: {1} ms",
            TestRepititions,
            averageMilliSeconds.ToString ("n"),
            objects.Length,
            ClientTransaction.Current.EnlistedDomainObjectCount);
      }
    }

    [Test]
    public void CommitSubTransaction_ValueProperties()
    {
      Console.WriteLine ("Expected average duration of CommitSubTransaction_ValueProperties on reference system: ~120 ms");

      using (ClientTransaction.CreateRootTransaction ().CreateSubTransaction ().EnterDiscardingScope ())
      {
        ClassWithValueProperties[] objects = TestDomainObjectMother.PrepareDatabaseObjectsWithValueProperties (TestSetSize);
        
        // Create 3000 unchanged objects in ClientTransaction in order to make test set more similar to relation test
        for (int i = 0; i < 3000; ++i)
          TestDomainObjectMother.CreateAndFillValuePropertyObject ();

        // warm up
        ChangeObjectsWithValueProperties (objects);
        ClientTransaction.Current.Commit ();

        var stopwatch = new Stopwatch ();

        for (int i = 0; i < TestRepititions; i++)
        {
          ChangeObjectsWithValueProperties (objects);
          stopwatch.Start ();
          ClientTransaction.Current.Commit ();
          stopwatch.Stop ();
        }

        Assert.That (ClientTransaction.Current.HasChanged (), Is.False);

        double averageMilliSeconds = stopwatch.Elapsed.TotalMilliseconds / TestRepititions;
        Console.WriteLine (
            "CommitSubTransaction_ValueProperties (executed {0} x Commit ({2} objects - total {3} objects in CTx)): Average duration: {1} ms",
            TestRepititions,
            averageMilliSeconds.ToString ("n"),
            objects.Length,
            ClientTransaction.Current.EnlistedDomainObjectCount);
      }
    }

    [Test]
    public void CommitTransactionToDatabase_NewProperties ()
    {
      Console.WriteLine ("Expected average duration of CommitTransactionToDatabase_NewProperties on reference system: ~3.5 ms");

      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        TestDomainObjectMother.PrepareDatabaseObjectsWithValueProperties (TestSetSize);
        ClientTransaction.Current.Commit();
      }

      var stopwatch = new Stopwatch();

      for (int i = 0; i < TestRepititions; i++)
      {
        using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
        {
          TestDomainObjectMother.PrepareDatabaseObjectsWithValueProperties (TestSetSize);
          stopwatch.Start();
          ClientTransaction.Current.Commit();
          stopwatch.Stop();
        }
      }

      double averageMilliSeconds = stopwatch.Elapsed.TotalMilliseconds / TestRepititions;
      Console.WriteLine (
          "CommitTransactionToDatabase_NewProperties (executed {0} x Commit ({2} objects)): Average duration: {1} ms",
          TestRepititions,
          averageMilliSeconds.ToString ("n"),
          TestSetSize);
    }

    private void ChangeObjectsWithRelations (ClassWithRelationProperties[] objects)
    {
      for (int i = 0; i < objects.Length; i++)
      {
        var currentObject = objects[i];
        var nextObject = objects[(i + 1) % objects.Length];

        switch (i % 4)
        {
          case 0:
            Swap (currentObject, nextObject, "Virtual1");
            Swap (currentObject, nextObject, "Virtual3");
            Swap (currentObject, nextObject, "Virtual5");
            break;
          case 2:
            Swap (currentObject, nextObject, "Real1");
            Swap (currentObject, nextObject, "Real3");
            Swap (currentObject, nextObject, "Real5");
            break;
        }
      }
    }

    private void ChangeObjectsWithValueProperties (ClassWithValueProperties[] objects)
    {
      for (int i = 0; i < objects.Length; i++)
      {
        var currentObject = objects[i];

        switch (i % 2)
        {
          case 0:
            ++currentObject.IntProperty1;
            ++currentObject.IntProperty2;
            ++currentObject.IntProperty3;
            break;
          case 1:
            currentObject.BoolProperty1 = !currentObject.BoolProperty1;
            currentObject.BoolProperty2 = !currentObject.BoolProperty2;
            currentObject.BoolProperty3 = !currentObject.BoolProperty3;
            break;
        }
      }
    }

    private void Swap (ClassWithRelationProperties one, ClassWithRelationProperties two, string shortPropertyName)
    {
      var propertiesOne = new PropertyIndexer (one);
      var propertiesTwo  = new PropertyIndexer (two);
      var accessorOne = propertiesOne[typeof (ClassWithRelationProperties), shortPropertyName];
      var accessorTwo = propertiesTwo[typeof (ClassWithRelationProperties), shortPropertyName];

      var oldValue = accessorOne.GetValueWithoutTypeCheck();
      accessorOne.SetValueWithoutTypeCheck (accessorTwo.GetValueWithoutTypeCheck());
      accessorTwo.SetValueWithoutTypeCheck (oldValue);
    }
  }
}