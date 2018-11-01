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
using Remotion.Data.DomainObjects.PerformanceTests.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  public class BindableObjectTestBase
  {
    public const int TestRepititions = 1000 * 1000;

    public virtual void BusinessObject_Property_IsAccessible ()
    {
      var obj = (IBusinessObject) ObjectWithSecurity.NewObject();
      var property = obj.BusinessObjectClass.GetPropertyDefinition ("TheProperty");

      bool value = true;

      foreach (var propertyDefinition in obj.BusinessObjectClass.GetPropertyDefinitions())
        Assert.That (propertyDefinition.IsAccessible (obj));

      var gcCounter = new GCCounter();
      gcCounter.BeginCount();
      var stopwatch = new Stopwatch();

      stopwatch.Start();
      for (int i = 0; i < TestRepititions; i++)
        value ^= property.IsAccessible (obj);
      stopwatch.Stop();
      gcCounter.EndCount();

      Trace.WriteLine (value);

      double averageMilliSeconds = ((double) stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine ("BusinessObject_Property_IsAccessible (executed {0:N0}x): Average duration: {1:N} 탎", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount (Console.Out);
    }

    public virtual void BusinessObject_GetProperty ()
    {
      var obj = (IBusinessObject) ObjectWithSecurity.NewObject();
      ((ObjectWithSecurity) obj).TheProperty = -1;
      var property = obj.BusinessObjectClass.GetPropertyDefinition ("TheProperty");

      bool value = false;

      Assert.That (obj.GetProperty (property), Is.Not.Null);

      var gcCounter = new GCCounter();
      gcCounter.BeginCount();
      var stopwatch = new Stopwatch();
      stopwatch.Start();
      for (int i = 0; i < TestRepititions; i++)
        value ^= obj.GetProperty (property) == null;
      stopwatch.Stop();
      gcCounter.EndCount();

      Trace.WriteLine (value);

      double averageMilliSeconds = ((double) stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine ("BusinessObject_GetProperty (executed {0:N0}x): Average duration: {1:N} 탎", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount (Console.Out);
    }

    public virtual void DynamicMethod_GetProperty ()
    {
      var obj = (IBusinessObject) ObjectWithSecurity.NewObject();
      ((ObjectWithSecurity) obj).TheProperty = -1;
      var property = (PropertyBase) obj.BusinessObjectClass.GetPropertyDefinition ("TheProperty");

      bool value = false;

      var propertyInfo = property.PropertyInfo;
      var dynamicMethod = propertyInfo.GetGetMethod (false).GetFastInvoker<Func<object, object>>();
      Assert.That (dynamicMethod (obj), Is.Not.Null);

      var gcCounter = new GCCounter();
      gcCounter.BeginCount();
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < TestRepititions; i++)
        value ^= dynamicMethod (obj) == null;

      stopwatch.Stop();
      gcCounter.EndCount();

      Trace.WriteLine (value);

      double averageMilliSeconds = ((double) stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine ("DynamicMethod_GetProperty (executed {0:N0}x): Average duration: {1:N} 탎", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount (Console.Out);
    }

    public virtual void DomainObject_GetProperty ()
    {
      var obj = ObjectWithSecurity.NewObject();
      obj.TheProperty = -1;

      bool value = false;

      Assert.That (obj.TheProperty, Is.Not.Null);

      var gcCounter = new GCCounter();
      gcCounter.BeginCount();
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < TestRepititions; i++)
        value ^= obj.TheProperty == 0;

      stopwatch.Stop();
      gcCounter.EndCount();

      Trace.WriteLine (value);

      double averageMilliSeconds = ((double) stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine ("DomainObject_GetProperty ((executed {0:N0}x): Average duration: {1:N} 탎", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount (Console.Out);
    }

    public virtual void BusinessObject_SetProperty ()
    {
      var obj = (IBusinessObject) ObjectWithSecurity.NewObject();
      var property = obj.BusinessObjectClass.GetPropertyDefinition ("TheProperty");

      obj.SetProperty (property, -1);

      var gcCounter = new GCCounter();
      gcCounter.BeginCount();
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      for (int i = 0; i < TestRepititions; i++)
        obj.SetProperty (property, i);

      stopwatch.Stop();
      gcCounter.EndCount();

      double averageMilliSeconds = ((double) stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine ("BusinessObject_SetProperty (executed {0:N0}x): Average duration: {1:N} 탎", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount (Console.Out);
    }

    public virtual void DomainObject_SetProperty ()
    {
      var obj = ObjectWithSecurity.NewObject ();
      obj.TheProperty = -1;

       var gcCounter = new GCCounter();
      gcCounter.BeginCount();
     var stopwatch = new Stopwatch ();
      stopwatch.Start ();

      for (int i = 0; i < TestRepititions; i++)
        obj.TheProperty = i;

      stopwatch.Stop ();
      gcCounter.EndCount();

      Trace.WriteLine (obj.TheProperty);

      double averageMilliSeconds = ((double) stopwatch.ElapsedMilliseconds / TestRepititions) * 1000;
      Console.WriteLine ("DomainObject_SetProperty ((executed {0:N0}x): Average duration: {1:N} 탎", TestRepititions, averageMilliSeconds);
      gcCounter.PrintCount (Console.Out);
    }
  }
}