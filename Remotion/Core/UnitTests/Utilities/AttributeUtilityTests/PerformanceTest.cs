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
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests
{
  [TestFixture]
  [Explicit]
  public class PerformanceTest
  {
    private Type[] _types;
    private PropertyInfo[] _properties;

    [SetUp]
    public void SetUp ()
    {
      _types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).ToArray();
      _types = _types.Concat(_types).Concat(_types).Concat(_types).Concat(_types).ToArray();

      var bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
      _properties = _types.SelectMany(t => t.GetProperties(bindingFlags)).ToArray();
      _properties = _properties.Concat(_properties).Concat(_properties).Concat(_properties).Concat(_properties).ToArray();
    }

    [Test]
    public void GetCustomAttributes_Unfiltered_Types ()
    {
      PerformMeasurement(
          _types,
          items =>
          {
            var counter = 0;
            for (int i = 0; i < items.Length; ++i)
              counter += AttributeUtility.GetCustomAttributes(items[i], typeof(Attribute), true).Length;
            return counter;
          },
          items =>
          {
            var counter = 0;
            for (int i = 0; i < items.Length; ++i)
              counter += Attribute.GetCustomAttributes(items[i], true).Length;
            return counter;
          });
    }

    [Test]
    public void GetCustomAttributes_Filtered_Types ()
    {
      PerformMeasurement(
          _types,
          items =>
          {
            var counter = 0;
            for (int i = 0; i < items.Length; ++i)
              counter += AttributeUtility.GetCustomAttributes(items[i], typeof(DataContractAttribute), true).Length;
            return counter;
          },
          items =>
          {
            var counter = 0;
            for (int i = 0; i < items.Length; ++i)
              counter += Attribute.GetCustomAttributes(items[i], typeof(DataContractAttribute), true).Length;
            return counter;
          });
    }

    [Test]
    public void GetCustomAttributes_Unfiltered_Properties ()
    {
      PerformMeasurement(
          _properties,
          items =>
          {
            var counter = 0;
            for (int i = 0; i < items.Length; ++i)
              try
              {
                counter += AttributeUtility.GetCustomAttributes(items[i], typeof(Attribute), true).Length;
              }
              catch (AmbiguousMatchException)
              {
              }
            return counter;
          },
          items =>
          {
            var counter = 0;
            for (int i = 0; i < items.Length; ++i)
              try
              {
                counter += Attribute.GetCustomAttributes(items[i], true).Length;
              }
              catch (AmbiguousMatchException)
              {
              }
            return counter;
          });
    }

    [Test]
    public void GetCustomAttributes_Filtered_Properties ()
    {
      PerformMeasurement(
          _properties,
          items =>
          {
            var counter = 0;
            for (int i = 0; i < items.Length; ++i)
              try
              {
                counter += AttributeUtility.GetCustomAttributes(items[i], typeof(DataMemberAttribute), true).Length;
              }
              catch (AmbiguousMatchException)
              {
                // Ignore
              }
            return counter;
          },
          items =>
          {
            var counter = 0;
            for (int i = 0; i < items.Length; ++i)
              try
              {
                counter += Attribute.GetCustomAttributes(items[i], typeof(DataMemberAttribute), true).Length;
              }
              catch (AmbiguousMatchException)
              {
              }
            return counter;
          });
    }

    private static TimeSpan MeasureTime (string description, MemberInfo[] members, Func<MemberInfo[], int> func)
    {
      // Warm up:
      func(members);

      var sw = Stopwatch.StartNew();
      var result = func(members);
      var elapsed = sw.Elapsed;
      Console.WriteLine(
          "{0}: {1} for {2} items; i.e., {3:n} ns per item; i.e., {4:n} item per second; result: {5}",
          description,
          elapsed,
          members.Length,
          elapsed.TotalMilliseconds / members.Length * 1000.0 * 1000.0,
          members.Length / elapsed.TotalSeconds,
          result);
      return elapsed;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void PerformMeasurement (MemberInfo[] members, Func<MemberInfo[], int> utilityFunc, Func<MemberInfo[], int> referenceFunc)
    {
      var callingMethod = new StackFrame(1).GetMethod().Name;
      var utilityTime = MeasureTime(
          callingMethod + " - AttributeUtility",
          members,
          utilityFunc);
      var referenceTime = MeasureTime(
          callingMethod + " - Attribute class ",
          members,
          referenceFunc);
      Console.WriteLine("Factor: " + utilityTime.TotalMilliseconds / referenceTime.TotalMilliseconds);
    }
  }
}
