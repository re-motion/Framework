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
  /// The <see cref="WxeCallOptions"/> type represents the most generic of options for executing a <see cref="WxeFunction"/>, namely it only controls
  /// whether the useer should be presented with a perma-URL to the <see cref="WxeFunction"/>.
  /// Use the derived types if you require additional control over the <see cref="WxeFunction"/>'s execution.
  /// </summary>
  public sealed class WxeCallOptions : WxeCallOptionsBase
  {
    public WxeCallOptions ()
        : this(WxePermaUrlOptions.Null)
    {
    }

    public WxeCallOptions (WxePermaUrlOptions permaUrlOptions)
        : base(permaUrlOptions)
    {
    }

    public override void Dispatch (IWxeExecutor executor, WxeFunction function, Control sender)
    {
      ArgumentUtility.CheckNotNull("executor", executor);
      ArgumentUtility.CheckNotNull("function", function);
      ArgumentUtility.CheckNotNull("sender", sender);

      executor.ExecuteFunction(function, sender, this);
    }
  }
}
