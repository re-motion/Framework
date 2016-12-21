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
using NUnit.Framework;
using Remotion.Context;
using Remotion.Development.UnitTesting;

namespace Remotion.UnitTests.Context
{
  [TestFixture]
  public class SafeContextSingletonTest
  {
    [SetUp]
    public void SetUp ()
    {
      SafeContext.Instance.FreeData ("test");
      SafeContext.Instance.FreeData ("test1");
      SafeContext.Instance.FreeData ("test2");
    }

    [TearDown]
    public void TearDown ()
    {
      SafeContext.Instance.FreeData ("test");
      SafeContext.Instance.FreeData ("test1");
      SafeContext.Instance.FreeData ("test2");
    }

    [Test]
    public void UsesSafeContext_WithGivenKey ()
    {
      object instance = new object();
      SafeContextSingleton<object> singleton = new SafeContextSingleton<object> ("test", delegate { return null; });
      singleton.SetCurrent (instance);

      Assert.That (SafeContext.Instance.GetData ("test"), Is.SameAs (instance));
    }

    [Test]
    public void SingleInstance_CreatedOnDemand ()
    {
      object instance = null;
      SafeContextSingleton<object> singleton = new SafeContextSingleton<object> ("test", delegate { return (instance = new object ()); });

      Assert.That (instance, Is.Null);
      object current = singleton.Current;
      Assert.That (current, Is.Not.Null);
      Assert.That (instance, Is.Not.Null);
      Assert.That (current, Is.SameAs (instance));

      Assert.That (singleton.Current, Is.SameAs (current));
      Assert.That (singleton.Current, Is.SameAs (current));
      Assert.That (singleton.Current, Is.SameAs (current));
    }

    [Test]
    public void DifferentSingletons ()
    {
      object instance1 = new object ();
      object instance2 = new object ();

      SafeContextSingleton<object> singleton1 = new SafeContextSingleton<object> ("test1", delegate { return instance1; });
      SafeContextSingleton<object> singleton2 = new SafeContextSingleton<object> ("test2", delegate { return instance2; });

      Assert.That (singleton1.Current, Is.SameAs (instance1));
      Assert.That (singleton2.Current, Is.SameAs (instance2));
      Assert.That (singleton1.Current, Is.SameAs (instance1));
      Assert.That (singleton2.Current, Is.SameAs (instance2));
    }

    [Test]
    public void HasCurrent ()
    {
      SafeContextSingleton<object> singleton = new SafeContextSingleton<object> ("test", delegate { return ( new object ()); });
      Assert.That (singleton.HasCurrent, Is.False);
      Dev.Null = singleton.Current;
      Assert.That (singleton.HasCurrent, Is.True);
      singleton.SetCurrent (null);
      Assert.That (singleton.HasCurrent, Is.False);
    }

    [Test]
    public void SetCurrent ()
    {
      object instance = new object();
      SafeContextSingleton<object> singleton = new SafeContextSingleton<object> ("test", delegate { return new object (); });
      Assert.That (singleton.Current, Is.Not.SameAs (instance));
      singleton.SetCurrent (instance);
      Assert.That (singleton.Current, Is.SameAs (instance));
      singleton.SetCurrent (new object());
      Assert.That (singleton.Current, Is.Not.SameAs (instance));
    }
  }
}
