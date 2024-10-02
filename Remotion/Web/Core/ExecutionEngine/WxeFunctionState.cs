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
using Microsoft.Extensions.Logging;
using Remotion.Logging;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  /// <summary>
  ///   Stores the session state for a single function token.
  /// </summary>
  /// <threadsafety static="true" instance="false" />
  public class WxeFunctionState
  {
    private static ILogger s_logger = LazyLoggerFactory.CreateLogger<WxeFunctionState>();

    private WxeFunction _function;
    private int _lifetime;
    private string _functionToken;
    private bool _isAborted;
    private bool _isCleanUpEnabled;
    private int _postBackID;

    public WxeFunctionState (WxeFunction function, bool enableCleanUp)
        : this(
            function,
            SafeServiceLocator.Current.GetInstance<IWxeLifetimeManagementSettings>().FunctionTimeout,
            enableCleanUp)
    {
    }

    public WxeFunctionState (
        WxeFunction function, int lifetime, bool enableCleanUp)
    {
      ArgumentUtility.CheckNotNull("function", function);
      _lifetime = lifetime;
      _functionToken = Guid.NewGuid().ToString();
      _function = function;
      _function.SetFunctionToken(_functionToken);
      _isCleanUpEnabled = enableCleanUp;
      _postBackID = 0;
      s_logger.LogDebug(string.Format("Created WxeFunctionState {0}.", _functionToken));
    }

    public WxeFunction Function
    {
      get { return _function; }
    }

    public int Lifetime
    {
      get { return _lifetime; }
    }

    public string FunctionToken
    {
      get { return _functionToken; }
    }

    /// <summary> 
    ///   Gets a flag that determines whether to automatically clean-up (i.e. abort) the function state after 
    ///   its function has executed.
    /// </summary>
    public bool IsCleanUpEnabled
    {
      get { return _isCleanUpEnabled; }
    }

    protected internal int PostBackID
    {
      get { return _postBackID; }
      set { _postBackID = value; }
    }

    public bool IsAborted
    {
      get { return _isAborted; }
    }

    /// <summary> Aborts the <b>WxeFunctionState</b> by calling <see cref="AbortRecursive"/>. </summary>
    /// <remarks> 
    ///   Use the <see cref="WxeFunctionStateManager.Abort">WxeFunctionStateCollection.Abort</see> method to abort
    ///   a <b>WxeFunctionState</b>.
    /// </remarks>
    protected internal void Abort ()
    {
      if (! _isAborted)
      {
        s_logger.LogDebug(string.Format("Aborting WxeFunctionState {0}.", _functionToken));
        AbortRecursive();
        _isAborted = true;
      }
    }

    /// <summary> Aborts the <b>WxeFunctionState</b>. </summary>
    protected virtual void AbortRecursive ()
    {
      if (_function != null)
        _function.Abort();
    }
  }
}
