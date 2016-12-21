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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Mixins.UnitTests.Core.TestDomain;

namespace Remotion.Mixins.UnitTests.Core
{
  [TestFixture]
  [Explicit ("Performance tests")]
  public class MixinTypeUtilityPerformanceTest
  {
    private Type _unmixedType;
    private Type _targetType;
    private Type _concreteType;

    [TestFixtureSetUp]
    public void TestFixtureSetUp ()
    {
      Console.WriteLine (
          "{0}\t{1}\t{2}\t{3}\t{4}",
          "Operation",
          "Tested Type",
          "Runs",
          "Elapsed total",
          "Elapsed ns/run");
    }

    [SetUp]
    public void SetUp ()
    {
      _unmixedType = typeof (object);
      _targetType = typeof (BaseType3);
      _concreteType = MixinTypeUtility.GetConcreteMixedType (typeof (BaseType3));
    }

    [Test]
    public void IsGeneratedConcreteMixedType ()
    {
      TimeThis (MethodBase.GetCurrentMethod (), (type, runs) =>
      {
        bool acc = false;
        for (long l = 0; l < runs; ++l)
        {
          acc ^= MixinTypeUtility.IsGeneratedConcreteMixedType (type);
        }
        return acc;
      });
    }

    [Test]
    public void IsGeneratedByMixinEngine ()
    {
      TimeThis (MethodBase.GetCurrentMethod (), (type, runs) =>
      {
        bool acc = false;
        for (long l = 0; l < runs; ++l)
        {
          acc ^= MixinTypeUtility.IsGeneratedByMixinEngine (type);
        }
        return acc;
      });
    }

    [Test]
    public void GetConcreteMixedType ()
    {
      TimeThis (MethodBase.GetCurrentMethod (), (type, runs) =>
      {
        bool acc = false;
        for (long l = 0; l < runs; ++l)
        {
          acc ^= MixinTypeUtility.GetConcreteMixedType (type) != null;
        }
        return acc;
      });
    }

    [Test]
    public void IsAssignableFrom ()
    {
      TimeThis (MethodBase.GetCurrentMethod (), (type, runs) =>
      {
        bool acc = false;
        for (long l = 0; l < runs; ++l)
        {
          acc ^= MixinTypeUtility.IsAssignableFrom (typeof (IComparable), type);
        }
        return acc;
      });
    }

    [Test]
    public void HasMixins ()
    {
      TimeThis (MethodBase.GetCurrentMethod (), (type, runs) =>
      {
        bool acc = false;
        for (long l = 0; l < runs; ++l)
        {
          acc ^= MixinTypeUtility.HasMixins (type);
        }
        return acc;
      });
    }

    [Test]
    public void GetAscribableMixinOnUnmixedTypes ()
    {
      TimeThis (MethodBase.GetCurrentMethod (), (type, runs) =>
      {
        bool acc = false;
        for (long l = 0; l < runs; ++l)
        {
          acc ^= MixinTypeUtility.GetAscribableMixinType (type, typeof (BT1Mixin1)) != null;
        }
        return acc;
      });
    }

    [Test]
    public void HasAscribableMixin ()
    {
      TimeThis (MethodBase.GetCurrentMethod (), (type, runs) =>
      {
        bool acc = false;
        for (long l = 0; l < runs; ++l)
        {
          acc ^= MixinTypeUtility.HasAscribableMixin (type, typeof (BT3Mixin3<,>));
        }
        return acc;
      });
    }

    [Test]
    public void GetMixinTypes ()
    {
      TimeThis (MethodBase.GetCurrentMethod (), (type, runs) =>
      {
        bool acc = false;
        for (long l = 0; l < runs; ++l)
        {
          acc ^= MixinTypeUtility.GetMixinTypes (type).Count() != 0;
        }
        return acc;
      });
    }

    [Test]
    public void GetMixinTypesExact ()
    {
      TimeThis (MethodBase.GetCurrentMethod (), (type, runs) =>
      {
        bool acc = false;
        for (long l = 0; l < runs; ++l)
        {
          acc ^= MixinTypeUtility.GetMixinTypesExact (type) != null;
        }
        return acc;
      });
    }

    [Test]
    public void CreateInstance ()
    {
      TimeThis (MethodBase.GetCurrentMethod (), (type, runs) =>
      {
        bool acc = false;
        for (long l = 0; l < runs; ++l)
        {
          acc ^= MixinTypeUtility.CreateInstance (type) != null;
        }
        return acc;
      });
    }

    [Test]
    public void GetUnderlyingTargetType ()
    {
      TimeThis (MethodBase.GetCurrentMethod (), (type, runs) =>
      {
        bool acc = false;
        for (long l = 0; l < runs; ++l)
        {
          acc ^= MixinTypeUtility.GetUnderlyingTargetType (type) != null;
        }
        return acc;
      });
    }

    private void TimeThis<T> (MethodBase callingMethod, Func<Type, long, T> testLoop, long runs = 100000, Tuple<Type, string>[] types = null)
    {
      types = types ?? new[] { Tuple.Create (_unmixedType, "unmixed type"), Tuple.Create (_targetType, "target type"), Tuple.Create (_concreteType, "concrete type") };
      foreach (var tuple in types)
      {
        TimeThis (testLoop, runs, callingMethod, tuple.Item1, tuple.Item2);
      }
    }

    private void TimeThis<T> (Func<Type, long, T> testLoop, long runs, MethodBase callingMethod, Type testedType, string testedTypeDescription)
    {
      // Warm up
      testLoop (testedType, 1);

      GC.Collect (2, GCCollectionMode.Forced);
      GC.WaitForPendingFinalizers();
      GC.Collect (2, GCCollectionMode.Forced);

      var sw = Stopwatch.StartNew ();
      var result = testLoop (testedType, runs);
      var elapsed = sw.Elapsed;

      Console.WriteLine (
          "{0}\t{1}\t{2:n0}\t{3}\t{4:n}",
          callingMethod.Name,
          testedTypeDescription,
          runs,
          elapsed,
          elapsed.TotalMilliseconds / runs * 1000.0 * 1000.0,
          result);
    }
  }
}
