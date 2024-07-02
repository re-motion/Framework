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
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Use the <see cref="WxeCallOptionsNoRepost"/> type if you whish to execute a <see cref="WxeFunction"/> as a sub function and suppressing 
  /// the re-post to the postback-handler after the execution has returned to the caller function.
  /// </summary>
  public sealed class WxeCallOptionsNoRepost : WxeCallOptionsBase
  {
    private readonly bool? _usesEventTarget;

    public WxeCallOptionsNoRepost ()
        : this(null, WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptionsNoRepost (bool? usesEventTarget)
        : this(usesEventTarget, WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptionsNoRepost (WxePermaUrlOptions permaUrlOptions)
        : this(null, permaUrlOptions)
    {
    }

    public WxeCallOptionsNoRepost (bool? usesEventTarget, WxePermaUrlOptions permaUrlOptions)
        : base(permaUrlOptions)
    {
      _usesEventTarget = usesEventTarget;
    }

    public override void Dispatch (IWxeExecutor executor, WxeFunction function, Control sender)
    {
      ArgumentUtility.CheckNotNull("executor", executor);
      ArgumentUtility.CheckNotNull("function", function);
      ArgumentUtility.CheckNotNull("sender", sender);

      executor.ExecuteFunctionNoRepost(function, sender, this);
    }

    public bool? UsesEventTarget
    {
      get { return _usesEventTarget; }
    }
  }
}
