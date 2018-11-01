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
using System.Runtime.Remoting.Messaging;
using System.Web;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Web.Context;

namespace Remotion.Web.UnitTests.Core.Context
{
  [TestFixture]
  public class HttpContextStorageProviderTest
  {
    private HttpContextStorageProvider _provider;
    private HttpContext _testContext;

    [SetUp]
    public void SetUp ()
    {
      _testContext = HttpContextHelper.CreateHttpContext ("x", "y", "z");
      HttpContext.Current = _testContext;
      _provider = new HttpContextStorageProvider ();
    }

    [Test]
    public void GetData_WithoutValue ()
    {
      Assert.That (_provider.GetData ("Foo"), Is.Null);
    }

    [Test]
    public void SetData ()
    {
      _provider.SetData ("Foo", 45);
      Assert.That (_provider.GetData ("Foo"), Is.EqualTo (45));
      Assert.That (_testContext.Items["Foo"], Is.EqualTo (45));
    }

    [Test]
    public void SetData_Null ()
    {
      _provider.SetData ("Foo", 45);
      _provider.SetData ("Foo", null);
      Assert.That (_provider.GetData ("Foo"), Is.Null);
      Assert.That (_testContext.Items["Foo"], Is.Null);
    }

    [Test]
    public void FreeData ()
    {
      _provider.SetData ("Foo", 45);
      _provider.FreeData ("Foo");
      Assert.That (_provider.GetData ("Foo"), Is.Null);
      Assert.That (_testContext.Items.Contains ("Foo"), Is.False);
    }

    [Test]
    public void FreeData_WithoutValue ()
    {
      _provider.FreeData ("Foo");
      Assert.That (_provider.GetData ("Foo"), Is.Null);
      Assert.That (_testContext.Items.Contains ("Foo"), Is.False);
    }

    [Test]
    public void FallbackToCallContext_IfNoCurrentHttpContext ()
    {
      HttpContext.Current = null;
      
      _provider.SetData ("Foo", 123);
      Assert.That (_provider.GetData ("Foo"), Is.EqualTo (123));
      Assert.That (CallContext.GetData ("Foo"), Is.EqualTo (123));
      
      _provider.FreeData ("Foo");
      Assert.That (CallContext.GetData ("Foo"), Is.Null);
    }
  }
}
