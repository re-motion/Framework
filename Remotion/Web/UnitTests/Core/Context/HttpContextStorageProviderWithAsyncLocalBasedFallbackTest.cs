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
using System.Threading;
using System.Web;
using NUnit.Framework;
using Remotion.Context;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Web.Context;

namespace Remotion.Web.UnitTests.Core.Context
{
  [TestFixture]
  public class HttpContextStorageProviderWithAsyncLocalBasedFallbackTest
  {
    private HttpContextStorageProviderWithAsyncLocalBasedFallback _provider;
    private HttpContext _testContext;

    [SetUp]
    public void SetUp ()
    {
      _testContext = HttpContextHelper.CreateHttpContext("x", "y", "z");
      HttpContext.Current = _testContext;
      _provider = new HttpContextStorageProviderWithAsyncLocalBasedFallback();
    }

    [TearDown]
    public void TearDown ()
    {
      HttpContext.Current = null;
    }

    [Test]
    public void DataRestoredAfterSafeContextBoundaryDisposed ()
    {
      _provider.SetData("test", 123);
      using (_provider.OpenSafeContextBoundary())
      {
        var safeContextResult = _provider.GetData("test");
        Assert.That(safeContextResult, Is.Null);
      }

      var result = _provider.GetData("test");
      Assert.That(result, Is.EqualTo(123));
    }

    [Test]
    public void SafeContextBoundaryDataUnavailableAfterDispose ()
    {
      using (_provider.OpenSafeContextBoundary())
      {
        _provider.SetData("test", 123);
        var safeContextResult = _provider.GetData("test");
        Assert.That(safeContextResult, Is.EqualTo(123));
      }

      var result = _provider.GetData("test");
      Assert.That(result, Is.Null);
    }

    [Test]
    public void SafeContextBoundaryCreationClearsData ()
    {
      _provider.SetData("test", 123);

      using var _ = _provider.OpenSafeContextBoundary();

      Assert.That(HttpContext.Current.Items["Rm.CtxP"], Is.Null);
    }

    [Test]
    public void HttpContextSwitchOnSameThreadResetsData ()
    {
      _provider.SetData("test", 123);

      HttpContext.Current = HttpContextHelper.CreateHttpContext("a", "b", "c");

      var result = _provider.GetData("test");
      Assert.That(result, Is.Null);
    }

    [Test]
    public void HttpContextSharedToNewThreadContainsSameData ()
    {
      _provider.SetData("test", 123);

      var context = HttpContext.Current;
      int? threadResult = null;
#pragma warning disable RMCORE0001
      var t = new Thread(
          () =>
          {
            HttpContext.Current = context;
            threadResult = (int)_provider.GetData("test");
          });
      t.Start();
      t.Join();
#pragma warning restore RMCORE0001
      var result = _provider.GetData("test");
      Assert.That(result, Is.EqualTo(123));
      Assert.That(threadResult, Is.EqualTo(123));
    }

    [Test]
    public void NewThreadCannotAccessExistingData ()
    {
      _provider.SetData("test", 123);

      int? threadResult = null;
#pragma warning disable RMCORE0001
      var t = new Thread(
          () =>
          {
            threadResult = (int?)_provider.GetData("test");
          });
      t.Start();
      t.Join();
#pragma warning restore RMCORE0001
      var result = _provider.GetData("test");
      Assert.That(result, Is.EqualTo(123));
      Assert.That(threadResult, Is.Null);
    }

    [Test]
    public void DataIsCollectedAfterRemovalFromHttpContextItems ()
    {
      var valueWeakReference = SetDataAndReturnItsWeakReference();

      Assert.That(valueWeakReference.IsAlive, Is.True);

      HttpContext.Current.Items["Rm.CtxP"] = null;

      GC.Collect();
      GC.WaitForPendingFinalizers();
      GC.Collect();

      Assert.That(valueWeakReference.IsAlive, Is.False);

      WeakReference SetDataAndReturnItsWeakReference ()
      {
        var value = new object();
        _provider.SetData("test", value);

        return new(value);
      }
    }

    [Test]
    public void FallbackToAsyncLocal_IfNoCurrentHttpContext ()
    {
      HttpContext.Current = null;

      var fallbackProvider = (AsyncLocalStorageProvider)PrivateInvoke.GetNonPublicField(_provider, "_fallbackProvider");

      _provider.SetData("Foo", 123);
      Assert.That(_provider.GetData("Foo"), Is.EqualTo(123));
      Assert.That(fallbackProvider.GetData("Foo"), Is.EqualTo(123));

      _provider.FreeData("Foo");
      Assert.That(fallbackProvider.GetData("Foo"), Is.Null);
    }
  }
}
