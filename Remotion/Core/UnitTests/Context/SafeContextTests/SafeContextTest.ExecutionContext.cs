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
using NUnit.Framework;
using Remotion.Context;

namespace Remotion.UnitTests.Context.SafeContextTests
{
  [TestFixture]
  public class SafeContextExecutionContextTest : SafeContextTestBase
  {
    [Test]
    public void Run_WithImplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;
      using (SetupImplicitSafeContextStorageProvider(safeContextStorageProvider))
      {
        RunExecutionContextTest((callback, state) => SafeContext.ExecutionContext.Run(ExecutionContext.Capture(), callback, state));
      }
    }

    [Test]
    public void Run_WithExplicitSafeContextStorageProvider_OpensSafeContextBoundary ()
    {
      var safeContextStorageProvider = SafeContextProviderMock.Object;
      RunExecutionContextTest((callback, state) => SafeContext.ExecutionContext.Run(safeContextStorageProvider, ExecutionContext.Capture(), callback, state));
    }

    private void RunExecutionContextTest (Action<ContextCallback, object> executionContextRun)
    {
      var delegateExecuted = false;

      var shouldState = new object();
      executionContextRun(ContextCallback, shouldState);
      Assert.That(delegateExecuted, Is.True);

      void ContextCallback (object state)
      {
        Assert.That(state, Is.SameAs(shouldState));
        SafeContextProviderMock.Verify(e => e.OpenSafeContextBoundary());
        delegateExecuted = true;
      }
    }
  }
}
