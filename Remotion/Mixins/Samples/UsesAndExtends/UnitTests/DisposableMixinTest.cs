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
using Remotion.Mixins.Samples.UsesAndExtends.Core;
using Remotion.TypePipe;

namespace Remotion.Mixins.Samples.UsesAndExtends.UnitTests
{
  [TestFixture]
  public class DisposableMixinTest
  {
    public class Data
    {
      public bool ManagedCalled = false;
      public bool UnmanagedCalled = false;
    }

    [Uses(typeof(DisposableMixin))]
    public class C
    {
      public Data Data = new Data();

      [OverrideMixin]
      public void CleanupManagedResources ()
      {
        Data.ManagedCalled = true;
      }

      [OverrideMixin]
      public void CleanupUnmanagedResources ()
      {
        Data.UnmanagedCalled = true;
      }
    }

    [Test]
    public void DisposeCallsAllCleanupMethods ()
    {
      C c = ObjectFactory.Create<C>(ParamList.Empty);
      Data data = c.Data;

      Assert.That(data.ManagedCalled, Is.False);
      Assert.That(data.UnmanagedCalled, Is.False);

      using ((IDisposable)c)
      {
        Assert.That(data.ManagedCalled, Is.False);
        Assert.That(data.UnmanagedCalled, Is.False);
      }
      Assert.That(data.ManagedCalled, Is.True);
      Assert.That(data.UnmanagedCalled, Is.True);
      GC.KeepAlive(c);
    }

    [Test]
    public void GCCallsAllUnmanagedCleanup ()
    {
      Data GetDataObject ()
      {
        C c = ObjectFactory.Create<C>(ParamList.Empty);

        Assert.That(c.Data.ManagedCalled, Is.False);
        Assert.That(c.Data.UnmanagedCalled, Is.False);

        GC.KeepAlive(c);
        return c.Data;
      }

      // The object must be created in a separate method: The x64 JITter in .NET 4.7.2 (DEBUG builds only) keeps the reference alive until the variable is out of scope.
      var data = GetDataObject();

      GC.Collect();
      GC.WaitForPendingFinalizers();

      Assert.That(data.ManagedCalled, Is.False);
      Assert.That(data.UnmanagedCalled, Is.True);
    }
  }
}
