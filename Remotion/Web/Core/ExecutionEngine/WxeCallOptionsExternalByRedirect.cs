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
using System.Collections.Specialized;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Use the <see cref="WxeCallOptionsExternalByRedirect"/> type if you whish to execute a <see cref="WxeFunction"/> as a new root function in the
  /// same window.The <see cref="WxeFunction"/> will be initialized on the server and then opened via a HTTP-redirect request.
  /// </summary>
  public sealed class WxeCallOptionsExternalByRedirect : WxeCallOptionsBase
  {
    private readonly bool _returnToCaller;
    private readonly NameValueCollection? _callerUrlParameters;

    public WxeCallOptionsExternalByRedirect ()
        : this(new WxePermaUrlOptions(false, null), true, null)
    {
    }

    public WxeCallOptionsExternalByRedirect (NameValueCollection urlParameters)
        : this(new WxePermaUrlOptions(false, urlParameters), true, null)
    {
    }

    public WxeCallOptionsExternalByRedirect (WxePermaUrlOptions permaUrlOptions, bool returnToCaller, NameValueCollection? callerUrlParameters)
        : base(permaUrlOptions)
    {
      _returnToCaller = returnToCaller;
      _callerUrlParameters = callerUrlParameters;
    }

    public override void Dispatch (IWxeExecutor executor, WxeFunction function, Control sender)
    {
      ArgumentUtility.CheckNotNull("executor", executor);
      ArgumentUtility.CheckNotNull("function", function);
      ArgumentUtility.CheckNotNull("sender", sender);

      executor.ExecuteFunctionExternalByRedirect(function, sender, this);

      throw new WxeCallExternalException();
    }

    public bool ReturnToCaller
    {
      get { return _returnToCaller; }
    }

    public NameValueCollection? CallerUrlParameters
    {
      get { return _callerUrlParameters; }
    }
  }
}
