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
using System.Threading;
using System.Web;
using NUnit.Framework;
using Remotion.Context;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine
{
  [TestFixture]
  public class WxeFunctionStateManagerTest
  {
    private HttpSessionStateBase _session;
    private WxeFunctionState _functionState;

    [SetUp]
    public void SetUp ()
    {
      _session = new FakeHttpSessionStateBase();

      _functionState = new WxeFunctionState(new TestFunction(), 1, true);
    }

    [TearDown]
    public void TearDown ()
    {
      HttpContextHelper.SetCurrent(null);
      SafeContext.Instance.FreeData(typeof(WxeFunctionStateManager).AssemblyQualifiedName);
    }

    [Test]
    public void InitializeFromExistingSession ()
    {
      DateTime lastAccess = DateTime.UtcNow;
      var functionStateMetaData = new WxeFunctionStateManager.WxeFunctionStateMetaData(Guid.NewGuid().ToString(), 1, lastAccess);
      var functionStates = new Dictionary<string, WxeFunctionStateManager.WxeFunctionStateMetaData>();
      functionStates.Add(functionStateMetaData.FunctionToken, functionStateMetaData);
      _session[GetSessionKeyForFunctionStates()] = functionStates;

      var functionStateManager = new WxeFunctionStateManager(_session);

      Assert.That(functionStateManager.GetLastAccessUtc(functionStateMetaData.FunctionToken), Is.EqualTo(lastAccess));
    }

    [Test]
    public void Add ()
    {
      var functionStateManager = new WxeFunctionStateManager(_session);
      functionStateManager.Add(_functionState);

      Assert.That(_session[GetSessionKeyForFunctionState()], Is.SameAs(_functionState));
    }

    [Test]
    public void GetItem ()
    {
      _session.Add(GetSessionKeyForFunctionState(), _functionState);

      var functionStateManager = new WxeFunctionStateManager(_session);
      WxeFunctionState actual = functionStateManager.GetItem(_functionState.FunctionToken);

      Assert.That(actual, Is.SameAs(_functionState));
    }

    [Test]
    public void Abort ()
    {
      _session.Add(GetSessionKeyForFunctionState(), _functionState);

      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager(_session);
      functionStateManager.Abort(_functionState);

      Assert.That(_session[GetSessionKeyForFunctionState()], Is.Null);
    }

    [Test]
    public void Touch ()
    {
      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager(_session);
      functionStateManager.Add(_functionState);
      DateTime lastAccess = functionStateManager.GetLastAccessUtc(_functionState.FunctionToken);
      Thread.Sleep(1000);
      functionStateManager.Touch(_functionState.FunctionToken);
      Assert.Greater(functionStateManager.GetLastAccessUtc(_functionState.FunctionToken), lastAccess);
    }

    [Test]
    [Category("LongRunning")]
    public void IsExpired_DelaysForOneMinute ()
    {
      WxeFunctionState functionState = new WxeFunctionState(new TestFunction(), 1, true);
      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager(_session);
      functionStateManager.Add(functionState);
      Assert.That(functionStateManager.IsExpired(functionState.FunctionToken), Is.False);
      Thread.Sleep(61000);
      Assert.That(functionStateManager.IsExpired(functionState.FunctionToken), Is.True);
    }

    [Test]
    public void IsExpired_WithUnknownFunctionToken ()
    {
      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager(_session);
      Assert.That(functionStateManager.IsExpired(Guid.NewGuid().ToString()), Is.True);
    }

    [Test]
    public void TryGetLiveValue_WithKnownFunctionTokenBeforeExpiration_ReturnsTrueAndExistingFunctionState ()
    {
      var functionState = new WxeFunctionState(new TestFunction(), 1, true);
      var functionStateManager = new WxeFunctionStateManager(_session);
      functionStateManager.Add(functionState);

      WxeFunctionState resultFunctionState;
      var result = functionStateManager.TryGetLiveValue(functionState.FunctionToken, out resultFunctionState);

      Assert.That(result, Is.True);
      Assert.That(resultFunctionState, Is.SameAs(functionState));
    }

    [Test]
    [Category("LongRunning")]
    public void TryGetLiveValue_WithExpiredFunctionToken_ReturnsFalseAndNullForFunctionState ()
    {
      var functionState = new WxeFunctionState(new TestFunction(), 1, true);
      var functionStateManager = new WxeFunctionStateManager(_session);
      functionStateManager.Add(functionState);
      WxeFunctionState resultFunctionStateBeforeExpiration;
      var resultBeforeExpiration = functionStateManager.TryGetLiveValue(functionState.FunctionToken, out resultFunctionStateBeforeExpiration);

      Assert.That(resultBeforeExpiration, Is.True);
      Assert.That(resultFunctionStateBeforeExpiration, Is.SameAs(functionState));

      Thread.Sleep(61000);

      WxeFunctionState resultFunctionStateAfterExpiration;
      var resultAfterExpiration = functionStateManager.TryGetLiveValue(functionState.FunctionToken, out resultFunctionStateAfterExpiration);

      Assert.That(resultAfterExpiration, Is.False);
      Assert.That(resultFunctionStateAfterExpiration, Is.Null);
    }

    [Test]
    public void TryGetLiveValue_WithUnknownFunctionToken_ReturnsFalseAndNullForFunctionState ()
    {
      var functionStateManager = new WxeFunctionStateManager(_session);
      WxeFunctionState resultFunctionState;
      var result = functionStateManager.TryGetLiveValue(Guid.NewGuid().ToString(), out resultFunctionState);

      Assert.That(result, Is.False);
      Assert.That(resultFunctionState, Is.Null);
    }

    [Test]
    [Category("LongRunning")]
    public void CleanupExpired_DelaysForOneMinute ()
    {
      WxeFunctionState functionStateExpired = new WxeFunctionState(new TestFunction(), 1, true);
      WxeFunctionState functionStateNotExpired = new WxeFunctionState(new TestFunction(), 10, true);
      WxeFunctionStateManager functionStateManager = new WxeFunctionStateManager(_session);
      functionStateManager.Add(functionStateExpired);
      functionStateManager.Add(functionStateNotExpired);

      var resultBeforeExpiration = functionStateManager.CleanUpExpired();
      Assert.That(resultBeforeExpiration, Is.EqualTo(2));

      Thread.Sleep(61000);

      Assert.That(functionStateManager.IsExpired(functionStateExpired.FunctionToken), Is.True);
      Assert.That(functionStateManager.IsExpired(functionStateNotExpired.FunctionToken), Is.False);

      var resultAfterExpiration = functionStateManager.CleanUpExpired();

      Assert.That(resultAfterExpiration, Is.EqualTo(1));
      Assert.That(functionStateManager.IsExpired(functionStateNotExpired.FunctionToken), Is.False);
      Assert.That(_session[GetSessionKeyForFunctionState(functionStateExpired.FunctionToken)], Is.Null);
    }

    [Test]
    public void HasSessionAndGetCurrent ()
    {
      HttpContextHelper.SetCurrent(HttpContextHelper.CreateHttpContext("get", "default.aspx", string.Empty));
      Assert.That(WxeFunctionStateManager.HasSession, Is.False);
      Assert.That(WxeFunctionStateManager.Current, Is.Not.Null);
      Assert.That(WxeFunctionStateManager.HasSession, Is.True);
    }

    [Test]
    public void GetCurrent_SameInstanceTwice ()
    {
      HttpContextHelper.SetCurrent(HttpContextHelper.CreateHttpContext("get", "default.aspx", string.Empty));
      Assert.That(WxeFunctionStateManager.Current, Is.SameAs(WxeFunctionStateManager.Current));
    }

    [Test]
    public void HasSessionAndGetCurrentInSeparateThreads ()
    {
      HttpContextHelper.SetCurrent(HttpContextHelper.CreateHttpContext("get", "default.aspx", string.Empty));
      Assert.That(WxeFunctionStateManager.HasSession, Is.False);
      Assert.That(WxeFunctionStateManager.Current, Is.Not.Null);
      Assert.That(WxeFunctionStateManager.HasSession, Is.True);
      ThreadRunner.Run(
          delegate
          {
            using (SafeContext.Instance.OpenSafeContextBoundary())
            {
              HttpContextHelper.SetCurrent(HttpContextHelper.CreateHttpContext("get", "default.aspx", string.Empty));
              Assert.That(WxeFunctionStateManager.HasSession, Is.False);
              Assert.That(WxeFunctionStateManager.Current, Is.Not.Null);
              Assert.That(WxeFunctionStateManager.HasSession, Is.True);
            }
          });
    }


    private string GetSessionKeyForFunctionState ()
    {
      string functionToken = _functionState.FunctionToken;
      return GetSessionKeyForFunctionState(functionToken);
    }

    private string GetSessionKeyForFunctionState (string functionToken)
    {
      return typeof(WxeFunctionStateManager).AssemblyQualifiedName + "|WxeFunctionState|" + functionToken;
    }

    private string GetSessionKeyForFunctionStates ()
    {
      return typeof(WxeFunctionStateManager).AssemblyQualifiedName + "|WxeFunctionStates";
    }
  }
}
