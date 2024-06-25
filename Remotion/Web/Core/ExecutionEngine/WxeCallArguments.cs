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
using System.Web.UI.WebControls;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  /// Use an instance <see cref="WxeCallArguments"/> type to control the execution of a <see cref="WxeFunction"/>. This type always requires a 
  /// <b>sender</b>-object and can be parameterized with <see cref="WxeCallOptions"/> to specify if the <see cref="WxeFunction"/> should execute
  /// within the same window, as a new root-function, with or without a perma-URL and so on.
  /// </summary>
  public sealed class WxeCallArguments : IWxeCallArguments
  {
    /// <summary>
    /// The default arguments. Use this instance to execute a <see cref="WxeFunction"/> as a sub-function within the same window.
    /// </summary>
    /// <remarks>
    /// <note type="caution">
    /// The <see cref="Default"/> arguments are not compatible with the <see cref="ImageButton"/> control and various native ASP.NET input controls.
    /// This manifests itself by the execution engine not re-raising a postback event upon the returning postback when the sub-function has completed.
    /// </note>
    /// </remarks>
    public static readonly IWxeCallArguments Default = new WxeCallArgumentsWithoutSender(WxePermaUrlOptions.Null);

    [NotNull]
    private readonly Control _sender;

    private readonly IWxeCallOptions _options;

    public WxeCallArguments ([NotNull] Control sender, [NotNull] IWxeCallOptions options)
    {
      ArgumentUtility.CheckNotNull("sender", sender);
      ArgumentUtility.CheckNotNull("options", options);

      _sender = sender;
      _options = options;
    }

    [PublicAPI]
    public Control Sender
    {
      get { return _sender; }
    }

    [PublicAPI]
    public IWxeCallOptions Options
    {
      get { return _options; }
    }

    void IWxeCallArguments.Dispatch (IWxeExecutor executor, WxeFunction function)
    {
      ArgumentUtility.CheckNotNull("executor", executor);
      ArgumentUtility.CheckNotNull("function", function);

      _options.Dispatch(executor, function, _sender);
    }
  }
}
