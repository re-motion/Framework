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

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  /// <summary>
  /// The <see cref="IWxeFunctionExecutionListener"/> interface defines hooks for interacting with a <see cref="WxeFunction"/>'s execution,
  /// including it's re-entry model.
  /// </summary>
  public interface IWxeFunctionExecutionListener : INullObject
  {
    /// <summary>Play is invoked when the function's <see cref="WxeFunction.Execute(WxeContext)"/> method is invoked (first and subsequent calls).</summary>
    void OnExecutionPlay (WxeContext context);

    /// <summary>Stop is invoked when the function's <see cref="WxeFunction.Execute(WxeContext)"/> method is completed successfully.</summary>
    void OnExecutionStop (WxeContext context);

    /// <summary>
    /// Play is invoked when the function's <see cref="WxeFunction.Execute(WxeContext)"/> method is exited by a <see cref="ThreadAbortException"/>,
    /// i.e. the execution is paused.
    /// </summary>
    void OnExecutionPause (WxeContext context);

    /// <summary>Play is invoked when the function's <see cref="WxeFunction.Execute(WxeContext)"/> method fails.</summary>
    void OnExecutionFail (WxeContext context, Exception exception);
  }
}
