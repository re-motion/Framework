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
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine.Infrastructure
{
  /// <summary>
  /// The <see cref="WxeRepostOptions"/> determine if and how the calling page is notified after a <see cref="WxeFunction"/> executed with the 
  /// <see cref="IWxeExecutor"/>.<see cref="IWxeExecutor.ExecuteFunction"/> or <see cref="IWxeExecutor"/>.<see cref="IWxeExecutor.ExecuteFunctionNoRepost"/> 
  /// methods has returned to the caller.
  /// </summary>
  public class WxeRepostOptions
  {
    public static WxeRepostOptions SuppressRepost ([NotNull] Control sender, bool usesEventTarget)
    {
      return new WxeRepostOptions(sender, usesEventTarget);
    }

    public static WxeRepostOptions DoRepost ([CanBeNull] Control? sender)
    {
      return new WxeRepostOptions(sender);
    }

    private readonly Control? _sender;
    private readonly bool _usesEventTarget;
    private readonly bool _suppressesRepost;

    private WxeRepostOptions (Control sender, bool usesEventTarget)
    {
      ArgumentUtility.CheckNotNull("sender", sender);

      if (!usesEventTarget && !(sender is IPostBackEventHandler || sender is IPostBackDataHandler))
      {
        throw new ArgumentException(
            "The 'sender' must implement either IPostBackEventHandler or IPostBackDataHandler. Provide the control that raised the post back event.");
      }

      _sender = sender;
      _usesEventTarget = usesEventTarget;
      _suppressesRepost = true;
    }

    private WxeRepostOptions (Control? sender)
    {
      _sender = sender;
      _usesEventTarget = false;
      _suppressesRepost = false;
    }

    public Control? Sender
    {
      get { return _sender; }
    }

    public bool UsesEventTarget
    {
      get { return _usesEventTarget; }
    }

    public bool SuppressesRepost
    {
      get { return _suppressesRepost; }
    }
  }
}
