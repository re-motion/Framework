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
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.DomainObjects.PerformanceTests.TestDomain;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ServiceLocation;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  [TestFixture]
  public class PropertyTest
  {
    public const int TestRepititions = 1000 * 1000;

    private ServiceLocatorScope _serviceLocatorScope;
    private bool _disableAccessChecksBackup;

    [SetUp]
    public void SetUp ()
    {
      var bindablePropertyReadAccessStrategy =
          new CompundBindablePropertyReadAccessStrategy(
              new IBindablePropertyReadAccessStrategy[] { new BindableDomainObjectPropertyReadAccessStrategy() });

      var bindablePropertyWriteAccessStrategy =
          new CompundBindablePropertyWriteAccessStrategy(
              new IBindablePropertyWriteAccessStrategy[] { new BindableDomainObjectPropertyWriteAccessStrategy() });

      var storageSettings = SafeServiceLocator.Current.GetInstance<IStorageSettings>();

      var serviceLocator = DefaultServiceLocator.CreateWithBootstrappedServices();
      serviceLocator.RegisterSingle<IBindablePropertyReadAccessStrategy>(() => bindablePropertyReadAccessStrategy);
      serviceLocator.RegisterSingle<IBindablePropertyWriteAccessStrategy>(() => bindablePropertyWriteAccessStrategy);
      serviceLocator.RegisterSingle(() => storageSettings);
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);

      ClientTransaction.CreateRootTransaction().EnterDiscardingScope();

      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);
    }

    [TearDown]
    public void TearDown ()
    {
      BusinessObjectProvider.SetProvider(typeof(BindableDomainObjectProviderAttribute), null);

      ClientTransactionScope.ResetActiveScope();
      _serviceLocatorScope.Dispose();
    }

    [Test]
    public void String_GetProperty ()
    {
      Console.WriteLine(
          "Expected average duration of PropertyTest for String_GetProperty on reference system: ~0.8 µs (release build), ~2.0 µs (debug build)");

      var obj = ClassWithFewValueProperties.NewObject();
      obj.StringProperty1 = "A";
      var comparisonValue = "B";

      bool value = false;

      Assert.That(obj.StringProperty1, Is.Not.Null);

      var gcCounter = new GCCounter();
      gcCounter.BeginCount();
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < TestRepititions; i++)
        value ^= obj.StringProperty1 == comparisonValue;

      stopwatch.Stop();
      gcCounter.EndCount();

      Trace.WriteLine(value);

      double averageMilliSeconds = ((double)stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine("String_GetProperty ((executed {0:N0}x): Average duration: {1:N} µs", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount(Console.Out);

      Console.WriteLine();
    }

    [Test]
    public void Unidirectional_GetProperty ()
    {
      Console.WriteLine(
          "Expected average duration of PropertyTest for Unidirectional_GetProperty on reference system: ~1.1 µs (release build), ~3.3 µs (debug build)");

      var obj = ClassWithRelationProperties.NewObject();
      obj.Unary1 = OppositeClassWithAnonymousRelationProperties.NewObject();
      var comparisonValue = OppositeClassWithAnonymousRelationProperties.NewObject();

      bool value = false;

      Assert.That(obj.Unary1, Is.Not.Null);

      var gcCounter = new GCCounter();
      gcCounter.BeginCount();
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < TestRepititions; i++)
        value ^= obj.Unary1 == comparisonValue;

      stopwatch.Stop();
      gcCounter.EndCount();

      Trace.WriteLine(value);

      double averageMilliSeconds = ((double)stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine("Unidirectional_GetProperty ((executed {0:N0}x): Average duration: {1:N} µs", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount(Console.Out);

      Console.WriteLine();
    }

    [Test]
    public void Bidirectional_OneToOne_RealEndPoint_GetProperty ()
    {
      Console.WriteLine(
          "Expected average duration of PropertyTest for Bidirectional_OneToOne_RealEndPoint_GetProperty on reference system: ~1.1 µs (release build), ~3.3 µs (debug build)");

      var obj = ClassWithRelationProperties.NewObject();
      obj.Real1 = OppositeClassWithVirtualRelationProperties.NewObject();
      var comparisonValue = OppositeClassWithVirtualRelationProperties.NewObject();

      bool value = false;

      Assert.That(obj.Real1, Is.Not.Null);

      var gcCounter = new GCCounter();
      gcCounter.BeginCount();
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < TestRepititions; i++)
        value ^= obj.Real1 == comparisonValue;

      stopwatch.Stop();
      gcCounter.EndCount();

      Trace.WriteLine(value);

      double averageMilliSeconds = ((double)stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine("Bidirectional_OneToOne_RealEndPoint_GetProperty ((executed {0:N0}x): Average duration: {1:N} µs", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount(Console.Out);

      Console.WriteLine();
    }

    [Test]
    public void Bidirectional_OneToOne_VirtualEndPoint_GetProperty ()
    {
      Console.WriteLine(
          "Expected average duration of PropertyTest for Bidirectional_OneToOne_VirtualEndPoint_GetProperty on reference system: ~0.9 µs (release build), ~2.5 µs (debug build)");

      var obj = ClassWithRelationProperties.NewObject();
      obj.Virtual1 = OppositeClassWithRealRelationProperties.NewObject();
      var comparisonValue = OppositeClassWithRealRelationProperties.NewObject();

      bool value = false;

      Assert.That(obj.Virtual1, Is.Not.Null);

      var gcCounter = new GCCounter();
      gcCounter.BeginCount();
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < TestRepititions; i++)
        value ^= obj.Virtual1 == comparisonValue;

      stopwatch.Stop();
      gcCounter.EndCount();

      Trace.WriteLine(value);

      double averageMilliSeconds = ((double)stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine("Bidirectional_OneToOne_VirtualEndPoint_GetProperty ((executed {0:N0}x): Average duration: {1:N} µs", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount(Console.Out);

      Console.WriteLine();
    }

    [Test]
    public void Bidirectional_OneToMany_RealEndPoint_GetProperty ()
    {
      Console.WriteLine(
          "Expected average duration of PropertyTest for Bidirectional_OneToMany_RealEndPoint_GetProperty on reference system: ~1.1 µs (release build), ~3.0 µs (debug build)");

      var collection = ClassWithRelationProperties.NewObject();
      collection.Collection.Add(OppositeClassWithCollectionRelationProperties.NewObject());

      var obj = OppositeClassWithCollectionRelationProperties.NewObject();
      obj.EndOfCollection = collection;

      bool value = false;

      Assert.That(obj.EndOfCollection, Is.Not.Null);

      var gcCounter = new GCCounter();
      gcCounter.BeginCount();
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < TestRepititions; i++)
        value ^= obj.EndOfCollection == null;

      stopwatch.Stop();
      gcCounter.EndCount();

      Trace.WriteLine(value);

      double averageMilliSeconds = ((double)stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine("Bidirectional_OneToMany_RealEndPoint_GetProperty ((executed {0:N0}x): Average duration: {1:N} µs", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount(Console.Out);

      Console.WriteLine();
    }

    [Test]
    public void Bidirectional_OneToMany_CollectionEndPoint_GetProperty ()
    {
      Console.WriteLine(
          "Expected average duration of PropertyTest for Bidirectional_OneToMany_CollectionEndPoint_GetProperty on reference system: ~1.0 µs (release build), ~3.3 µs (debug build)");

      var obj = ClassWithRelationProperties.NewObject();
      obj.Collection.Add(OppositeClassWithCollectionRelationProperties.NewObject());
      obj.Collection.Add(OppositeClassWithCollectionRelationProperties.NewObject());

      bool value = false;

      Assert.That(obj.Collection, Is.Not.Null);

      var gcCounter = new GCCounter();
      gcCounter.BeginCount();
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < TestRepititions; i++)
        value ^= obj.Collection[1] == null;

      stopwatch.Stop();
      gcCounter.EndCount();

      Trace.WriteLine(value);

      double averageMilliSeconds = ((double)stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine("Bidirectional_OneToMany_CollectionEndPoint_GetProperty ((executed {0:N0}x): Average duration: {1:N} µs", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount(Console.Out);

      Console.WriteLine();
    }

    [Test]
    public void String_SetProperty ()
    {
      Console.WriteLine(
          "Expected average duration of PropertyTest for String_SetProperty on reference system: ~1.3 µs (release build), ~3.0 µs (debug build)");

      var obj = ClassWithFewValueProperties.NewObject();
      obj.StringProperty1 = "A";
      var value = new[] { "B", "C" };

       var gcCounter = new GCCounter();
      gcCounter.BeginCount();
     var stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < TestRepititions; i++)
        obj.StringProperty1 = value[i & 1];

      stopwatch.Stop();
      gcCounter.EndCount();

      Trace.WriteLine(obj.StringProperty1);

      double averageMilliSeconds = ((double)stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine("String_SetProperty ((executed {0:N0}x): Average duration: {1:N} µs", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount(Console.Out);

      Console.WriteLine();
    }

    [Test]
    public void Unidirectional_SetProperty ()
    {
      Console.WriteLine(
          "Expected average duration of PropertyTest for Unidirectional_SetProperty on reference system: ~4.0 µs (release build), ~9.0 µs (debug build)");

      var obj = ClassWithRelationProperties.NewObject();
      obj.Unary1 = OppositeClassWithAnonymousRelationProperties.NewObject();
      var value = new[] { OppositeClassWithAnonymousRelationProperties.NewObject(), OppositeClassWithAnonymousRelationProperties.NewObject() };

       var gcCounter = new GCCounter();
      gcCounter.BeginCount();
     var stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < TestRepititions; i++)
        obj.Unary1 = value[i & 1];

      stopwatch.Stop();
      gcCounter.EndCount();

      Trace.WriteLine(obj.Unary1);

      double averageMilliSeconds = ((double)stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine("Unidirectional_SetProperty ((executed {0:N0}x): Average duration: {1:N} µs", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount(Console.Out);

      Console.WriteLine();
    }

    [Test]
    public void Bidirectional_OneToOne_RealEndPoint_SetProperty ()
    {
      Console.WriteLine(
          "Expected average duration of PropertyTest for Bidirectional_OneToOne_RealEndPoint_SetProperty on reference system: ~10 µs (release build), ~21 µs (debug build)");

      var obj = ClassWithRelationProperties.NewObject();
      obj.Real1 = OppositeClassWithVirtualRelationProperties.NewObject();
      var value = new[] { OppositeClassWithVirtualRelationProperties.NewObject(), OppositeClassWithVirtualRelationProperties.NewObject() };

       var gcCounter = new GCCounter();
      gcCounter.BeginCount();
     var stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < TestRepititions; i++)
        obj.Real1 = value[i & 1];

      stopwatch.Stop();
      gcCounter.EndCount();

      Trace.WriteLine(obj.Real1);

      double averageMilliSeconds = ((double)stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine("Bidirectional_OneToOne_RealEndPoint_SetProperty ((executed {0:N0}x): Average duration: {1:N} µs", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount(Console.Out);

      Console.WriteLine();
    }

    [Test]
    public void Bidirectional_OneToOne_VirtualEndPoint_SetProperty ()
    {
      Console.WriteLine(
          "Expected average duration of PropertyTest for Bidirectional_OneToOne_VirtualEndPoint_SetProperty on reference system: ~11 µs (release build), ~22 µs (debug build)");

      var obj = ClassWithRelationProperties.NewObject();
      obj.Virtual1 = OppositeClassWithRealRelationProperties.NewObject();
      var value = new[] { OppositeClassWithRealRelationProperties.NewObject(), OppositeClassWithRealRelationProperties.NewObject() };

       var gcCounter = new GCCounter();
      gcCounter.BeginCount();
     var stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < TestRepititions; i++)
        obj.Virtual1 = value[i & 1];

      stopwatch.Stop();
      gcCounter.EndCount();

      Trace.WriteLine(obj.Virtual1);

      double averageMilliSeconds = ((double)stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine("Bidirectional_OneToOne_VirtualEndPoint_SetProperty ((executed {0:N0}x): Average duration: {1:N} µs", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount(Console.Out);

      Console.WriteLine();
    }

    [Test]
    public void Bidirectional_OneToMany_RealEndPoint_SetProperty ()
    {
      Console.WriteLine(
          "Expected average duration of PropertyTest for Bidirectional_OneToMany_RealEndPoint_SetProperty on reference system: ~13 µs (release build), ~27 µs (debug build)");

      var obj = OppositeClassWithCollectionRelationProperties.NewObject();
      obj.EndOfCollection = ClassWithRelationProperties.NewObject();
      var value = new[] { ClassWithRelationProperties.NewObject(), ClassWithRelationProperties.NewObject() };

       var gcCounter = new GCCounter();
      gcCounter.BeginCount();
     var stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < TestRepititions; i++)
        obj.EndOfCollection = value[i & 1];

      stopwatch.Stop();
      gcCounter.EndCount();

      Trace.WriteLine(obj.EndOfCollection);

      double averageMilliSeconds = ((double)stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine("Bidirectional_OneToMany_RealEndPoint_SetProperty ((executed {0:N0}x): Average duration: {1:N} µs", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount(Console.Out);

      Console.WriteLine();
    }

    [Test]
    public void Bidirectional_OneToMany_CollectionEndPoint_SetProperty ()
    {
      Console.WriteLine(
          "Expected average duration of PropertyTest for Bidirectional_OneToMany_CollectionEndPoint_SetProperty on reference system: ~28 µs (release build), ~55 µs (debug build)");

      var obj = ClassWithRelationProperties.NewObject();
      obj.Collection.Add(OppositeClassWithCollectionRelationProperties.NewObject());
      var value = new[] { OppositeClassWithCollectionRelationProperties.NewObject(), OppositeClassWithCollectionRelationProperties.NewObject() };

       var gcCounter = new GCCounter();
      gcCounter.BeginCount();
     var stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < TestRepititions; i++)
      {
        obj.Collection.Clear();
        obj.Collection.Add(value[i & 1]);
      }

      stopwatch.Stop();
      gcCounter.EndCount();

      Trace.WriteLine(obj.Collection);

      double averageMilliSeconds = ((double)stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine("Bidirectional_OneToMany_CollectionEndPoint_SetProperty ((executed {0:N0}x): Average duration: {1:N} µs", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount(Console.Out);

      Console.WriteLine();
    }
  }
}
