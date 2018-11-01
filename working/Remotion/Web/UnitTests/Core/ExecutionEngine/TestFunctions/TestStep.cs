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
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions
{
  public class TestStep: WxeStep
  {
    public static new T GetStepByType<T> (WxeStep step)
        where T:WxeStep
    {
      return WxeStep.GetStepByType<T> (step);
    }

    private bool _isExecuteCalled;
    private bool _isAbortRecursiveCalled;
    private WxeContext _wxeContext;

    public TestStep()
    {
    }

    public override void Execute(WxeContext context)
    {
      _isExecuteCalled = true;
      _wxeContext = context;
    }

    protected override void AbortRecursive()
    {
      base.AbortRecursive ();
      _isAbortRecursiveCalled = true;
    }

    public bool IsExecuteCalled
    {
      get { return _isExecuteCalled; }
    }

    public bool IsAbortRecursiveCalled
    {
      get { return _isAbortRecursiveCalled; }
    }

    public WxeContext WxeContext
    {
      get { return _wxeContext; }
    }
  }
}
