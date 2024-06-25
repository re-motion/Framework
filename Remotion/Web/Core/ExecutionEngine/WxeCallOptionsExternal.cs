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
  /// Use the <see cref="WxeCallOptionsExternal"/> type if you whish to execute a <see cref="WxeFunction"/> as a new root function, 
  /// typically in a new window. The <see cref="WxeFunction"/> will be initialized on the server and then opened via a Javascript call.
  /// </summary>
  public sealed class WxeCallOptionsExternal : WxeCallOptionsBase
  {
    private readonly string _target;
    private readonly string? _features;
    private readonly bool _returningPostback;

    public WxeCallOptionsExternal (string target)
        : this(target, null, true, WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptionsExternal (string target, string features)
        : this(target, features, true, WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptionsExternal (string target, string? features, bool returningPostback)
        : this(target, features, returningPostback, WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptionsExternal (string target, string? features, bool returningPostback, WxePermaUrlOptions permaUrlOptions)
        : base(permaUrlOptions)
    {
      ArgumentUtility.CheckNotNullOrEmpty("target", target);

      _target = target;
      _features = features;
      _returningPostback = returningPostback;
    }

    public override void Dispatch (IWxeExecutor executor, WxeFunction function, Control sender)
    {
      ArgumentUtility.CheckNotNull("executor", executor);
      ArgumentUtility.CheckNotNull("function", function);
      ArgumentUtility.CheckNotNull("sender", sender);

      executor.ExecuteFunctionExternal(function, sender, this);

      throw new WxeCallExternalException();
    }

    public string Target
    {
      get { return _target; }
    }

    public string? Features
    {
      get { return _features; }
    }

    public bool ReturningPostback
    {
      get { return _returningPostback; }
    }
  }
}
