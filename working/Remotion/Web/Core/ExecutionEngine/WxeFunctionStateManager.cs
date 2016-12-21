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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Remotion.Context;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Web.ExecutionEngine
{
  /// <threadsafety static="true" instance="true" />
  public class WxeFunctionStateManager
  {
    /// <threadsafety static="true" instance="true" />
    [Serializable]
    public class WxeFunctionStateMetaData : Tuple<string, int, DateTime>
    {
      public WxeFunctionStateMetaData (string functionToken, int lifetime, DateTime lastAccess)
          : base (functionToken, lifetime, lastAccess)
      {
      }

      public string FunctionToken
      {
        get { return Item1; }
      }

      public int Lifetime
      {
        get { return Item2; }
      }

      public DateTime LastAccess
      {
        get { return Item3; }
      }
    }

    private static readonly ILog s_log = LogManager.GetLogger (typeof (WxeFunctionStateManager));

    private static readonly string s_key = typeof (WxeFunctionStateManager).AssemblyQualifiedName;
    private static readonly string s_sessionKeyForFunctionStates = s_key + "|WxeFunctionStates";

    private static readonly SafeContextSingleton<WxeFunctionStateManager> s_functionStateManager =
        new SafeContextSingleton<WxeFunctionStateManager> (s_key, CreateWxeFunctionStateManager);

    private static WxeFunctionStateManager CreateWxeFunctionStateManager ()
    {
      var session = HttpContext.Current.Session;
      var sessionWrapper = new HttpSessionStateWrapper (session);
      Assertion.IsTrue (ReferenceEquals (session.SyncRoot, sessionWrapper.SyncRoot));
      return new WxeFunctionStateManager (sessionWrapper);
    }

    public static WxeFunctionStateManager Current
    {
      get { return s_functionStateManager.Current; }
    }

    public static bool HasSession
    {
      get { return HttpContext.Current.Session[s_sessionKeyForFunctionStates] != null; }
    }

    private readonly Dictionary<string, WxeFunctionStateMetaData> _functionStates;
    private readonly HttpSessionStateBase _session;
    private readonly object _lockObject;

    public WxeFunctionStateManager (HttpSessionStateBase session)
    {
      ArgumentUtility.CheckNotNull ("session", session);
      _session = session;

      _functionStates = (Dictionary<string, WxeFunctionStateMetaData>) _session[s_sessionKeyForFunctionStates];
      if (_functionStates == null)
      {
        _functionStates = new Dictionary<string, WxeFunctionStateMetaData> ();
        _session[s_sessionKeyForFunctionStates] = _functionStates;
      }
      // WxeFunctionStateManager must be synchronized accross access from multiple requests in case the WxeFunctionStateManager is accessed from a ReadOnlySession.
      // Note that this will not guard against access from ReadOnlySessions that use serialization, but this is not an issue as a ReadOnlySession 
      // does include a write-back.
      _lockObject = _session.SyncRoot;
    }

    /// <summary> Cleans up expired <see cref="WxeFunctionState"/> objects in the collection. </summary>
    /// <returns>The remaining (i.e. not expired) function states.</returns>
    /// <remarks> Removes and aborts expired function states. </remarks>
    public int CleanUpExpired ()
    {
      lock (_lockObject)
      {
        foreach (string functionToken in _functionStates.Keys.ToArray())
        {
          if (IsExpired (functionToken))
          {
            WxeFunctionState functionState = GetItem (functionToken);
            Abort (functionState);
          }
        }
        return _functionStates.Count;
      }
    }

    /// <summary> Adds the <paramref name="functionState"/> to the collection. </summary>
    /// <param name="functionState"> 
    ///   The <see cref="WxeFunctionState"/> to be added. Must not be <see langword="null"/> or aborted.
    /// </param>
    public void Add (WxeFunctionState functionState)
    {
      ArgumentUtility.CheckNotNull ("functionState", functionState);
      if (functionState.IsAborted)
        throw new ArgumentException ("An aborted WxeFunctionState cannot be added to the collection.", "functionState");

      lock (_lockObject)
      {
        _functionStates.Add (
            functionState.FunctionToken,
            new WxeFunctionStateMetaData (functionState.FunctionToken, functionState.Lifetime, DateTime.Now));
        _session.Add (GetSessionKeyForFunctionState (functionState.FunctionToken), functionState);
      }
    }

    /// <summary> Gets the <see cref="WxeFunctionState"/> for the specifed <paramref name="functionToken"/>. </summary>
    /// <param name="functionToken"> 
    ///   The token to look-up the <see cref="WxeFunctionState"/>. Must not be <see langword="null"/> or empty.
    /// </param>
    /// <returns> The <see cref="WxeFunctionState"/> for the specified <paramref name="functionToken"/>. </returns>
    public WxeFunctionState GetItem (string functionToken)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("functionToken", functionToken);
      
      Stopwatch stopwatch = null;
      bool hasOutOfProcessSession = _session.Mode != SessionStateMode.Off && _session.Mode != SessionStateMode.InProc;
      if (hasOutOfProcessSession)
      {
        stopwatch = new Stopwatch();
        stopwatch.Start();
      }

      WxeFunctionState functionState;
      lock (_lockObject)
      {
        functionState = (WxeFunctionState) _session[GetSessionKeyForFunctionState (functionToken)];
      }

      if (hasOutOfProcessSession)
      {
        stopwatch.Stop();
        s_log.DebugFormat ("Deserialized WxeFunctionState {0} in {1} ms.", functionToken, stopwatch.ElapsedMilliseconds);
      }

      return functionState;
    }

    /// <summary> Removes the <paramref name="functionToken"/> from the collection. </summary>
    /// <param name="functionToken"> 
    ///   The <see cref="WxeFunctionState"/> to be removed. Must not be <see langword="null"/> or empty.
    /// </param>
    protected void Remove (string functionToken)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("functionToken", functionToken);

      lock (_lockObject)
      {
        _session.Remove (GetSessionKeyForFunctionState (functionToken));
        _functionStates.Remove (functionToken);
      }
    }

    /// <summary> Removes and aborts the <paramref name="functionState"/> from the collection. </summary>
    /// <param name="functionState"> 
    ///   The <see cref="WxeFunctionState"/> to be removed. Must not be <see langword="null"/>.
    /// </param>
    public void Abort (WxeFunctionState functionState)
    {
      ArgumentUtility.CheckNotNull ("functionState", functionState);

      lock (_lockObject)
      {
        Remove (functionState.FunctionToken);
        // WxeFunctionState is not threadsafe, thus locking the abort at this point is good practice
        functionState.Abort();
      }
    }

    public bool IsExpired (string functionToken)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("functionToken", functionToken);

      lock (_lockObject)
      {
        WxeFunctionStateMetaData functionStateMetaData;
        if (_functionStates.TryGetValue (functionToken, out functionStateMetaData))
          return functionStateMetaData.LastAccess.AddMinutes (functionStateMetaData.Lifetime) < DateTime.Now;

        return true;
      }
    }

    public DateTime GetLastAccess (string functionToken)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("functionToken", functionToken);
      lock (_lockObject)
      {
        CheckFunctionTokenExists (functionToken);

        return _functionStates[functionToken].LastAccess;
      }
    }

    public void Touch (string functionToken)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("functionToken", functionToken);
      lock (_lockObject)
      {
        CheckFunctionTokenExists (functionToken);

        s_log.Debug (string.Format ("Refreshing WxeFunctionState {0}.", functionToken));
        WxeFunctionStateMetaData old = _functionStates[functionToken];
        _functionStates[functionToken] = new WxeFunctionStateMetaData (old.FunctionToken, old.Lifetime, DateTime.Now);
      }
    }

    private void CheckFunctionTokenExists (string functionToken)
    {
      // No lock in method, will be locked from the outside.
      if (!_functionStates.ContainsKey (functionToken))
      {
        throw new ArgumentException (
            string.Format ("WxeFunctionState '{0}' is not registered with the WxeFunctionStateManager.", functionToken), "functionToken");
      }
    }

    private string GetSessionKeyForFunctionState (string functionToken)
    {
      return s_key + "|WxeFunctionState|" + functionToken;
    }
  }
}
