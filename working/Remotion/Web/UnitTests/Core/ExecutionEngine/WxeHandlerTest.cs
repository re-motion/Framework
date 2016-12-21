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
using System.Threading;
using System.Web;
using NUnit.Framework;
using Remotion.Context;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.UnitTests.Core.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine
{
  [TestFixture]
  public class WxeHandlerTest : WxeTest
  {
    private WxeHandlerMock _wxeHandler;

    protected const string c_functionTokenForFunctionStateWithEnabledCleanUp = "00000000-Enabled-CleanUp";
    protected const string c_functionTokenForFunctionStateWithDisabledCleanUp = "00000000-Disabled-CleanUp";
    protected const string c_functionTokenForFunctionStateWithMissingFunction = "00000000-Missing-Function";
    protected const string c_functionTokenForMissingFunctionState = "00000000-Missing-FunctionState";
    protected const string c_functionTokenForAbortedFunctionState = "00000000-Aborted";
    protected const string c_functionTokenForExpiredFunctionState = "00000000-Expired";
    protected const string c_functionTokenForNewFunctionState = "00000000-New";
    protected const string c_functionTokenForFunctionStateWithChildFunction = "00000000-Has-ChildFunction";

    private WxeFunctionStateMock _functionStateWithEnabledCleanUp;
    private WxeFunctionStateMock _functionStateWithDisabledCleanUp;
    private WxeFunctionStateMock _functionStateWithMissingFunction;
    private WxeFunctionStateMock _functionStateAborted;
    private WxeFunctionStateMock _functionStateExpired;
    private WxeFunctionStateMock _functionStateWithChildFunction;

    private Type _functionType;
    private string _functionTypeName;
    private string _invalidFunctionTypeName;

    private string _returnUrl = "newReturnUrl.html";

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();

      _wxeHandler = new WxeHandlerMock();

      _functionStateWithEnabledCleanUp = new WxeFunctionStateMock (new TestFunction(), 10, true, c_functionTokenForFunctionStateWithEnabledCleanUp);
      WxeFunctionStateManager.Current.Add (_functionStateWithEnabledCleanUp);

      _functionStateWithDisabledCleanUp = new WxeFunctionStateMock (new TestFunction(), 10, false, c_functionTokenForFunctionStateWithDisabledCleanUp);
      WxeFunctionStateManager.Current.Add (_functionStateWithDisabledCleanUp);

      _functionStateWithMissingFunction = new WxeFunctionStateMock (new TestFunction(), 10, false, c_functionTokenForFunctionStateWithMissingFunction);
      _functionStateWithMissingFunction.Function = null;
      WxeFunctionStateManager.Current.Add (_functionStateWithMissingFunction);

      _functionStateAborted = new WxeFunctionStateMock (new TestFunction(), 10, true, c_functionTokenForAbortedFunctionState);
      WxeFunctionStateManager.Current.Add (_functionStateAborted);
      _functionStateAborted.Abort();

      _functionStateExpired = new WxeFunctionStateMock (new TestFunction(), 0, true, c_functionTokenForExpiredFunctionState);
      WxeFunctionStateManager.Current.Add (_functionStateExpired);

      TestFunction rootFunction = new TestFunction();
      TestFunction childFunction = new TestFunction();
      rootFunction.Add (childFunction);
      _functionStateWithChildFunction = new WxeFunctionStateMock (childFunction, 10, true, c_functionTokenForFunctionStateWithChildFunction);
      WxeFunctionStateManager.Current.Add (_functionStateWithChildFunction);

      _functionType = typeof (TestFunction);
      _functionTypeName = _functionType.AssemblyQualifiedName;
      _invalidFunctionTypeName = "Remotion.Web.UnitTests::ExecutionEngine.InvalidFunction";

      Thread.Sleep (20);
      Remotion.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);
    }

    [TearDown]
    public override void TearDown ()
    {
      WxeFunctionStateManager.Current.Abort (_functionStateWithEnabledCleanUp);
      WxeFunctionStateManager.Current.Abort (_functionStateWithDisabledCleanUp);
      WxeFunctionStateManager.Current.Abort (_functionStateWithMissingFunction);
      WxeFunctionStateManager.Current.Abort (_functionStateAborted);
      WxeFunctionStateManager.Current.Abort (_functionStateExpired);
      WxeFunctionStateManager.Current.Abort (_functionStateWithChildFunction);

      Remotion.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);
      SafeContext.Instance.FreeData (typeof (WxeFunctionStateManager).AssemblyQualifiedName);
      base.TearDown ();
    }

    [Test]
    public void CreateNewFunctionStateState ()
    {
      WxeFunctionState functionState = _wxeHandler.CreateNewFunctionState (CurrentHttpContext, _functionType);

      Assert.That (functionState, Is.Not.Null);
      Assert.That (functionState.FunctionToken, Is.Not.Null);
      Assert.That (functionState.Function, Is.Not.Null);
      Assert.That (functionState.Function.GetType(), Is.EqualTo (_functionType));
      Assert.That (functionState.Function.ReturnUrl, Is.EqualTo (TestFunction.ReturnUrlValue));

      WxeFunctionState expiredFunctionState = WxeFunctionStateManager.Current.GetItem (c_functionTokenForExpiredFunctionState);
      Assert.That (expiredFunctionState, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (WxeException))]
    public void GetFunctionTypeWithInvalidTypeName ()
    {
      _wxeHandler.GetTypeByTypeName (_invalidFunctionTypeName);
    }

    [Test]
    public void GetFunctionTypeByTypeName ()
    {
      Type type = _wxeHandler.GetTypeByTypeName (_functionTypeName);
      Assert.That (type, Is.Not.Null);
      Assert.That (type, Is.EqualTo (_functionType));
    }

    [Test]
    public void GetFunctionTypeByPath ()
    {
      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (_functionType, "~/Test.wxe"));

      Type type = _wxeHandler.GetTypeByPath (@"/Test.wxe");

      Assert.That (type, Is.Not.Null);
      Assert.That (type, Is.EqualTo (_functionType));
    }

    [Test]
    [ExpectedException (typeof (WxeException))]
    public void GetFunctionTypeByPathWithoutMapping ()
    {
      _wxeHandler.GetTypeByPath (@"/Test1.wxe");
    }

    [Test]
    public void CreateNewFunctionStateStateWithReturnUrl ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set (WxeHandler.Parameters.ReturnUrl, _returnUrl);
      HttpContextHelper.SetQueryString (CurrentHttpContext, queryString);

      WxeFunctionState functionState = _wxeHandler.CreateNewFunctionState (CurrentHttpContext, _functionType);

      Assert.That (functionState, Is.Not.Null);
      Assert.That (functionState.FunctionToken, Is.Not.Null);
      Assert.That (functionState.Function, Is.Not.Null);
      Assert.That (functionState.Function.GetType(), Is.EqualTo (_functionType));
      Assert.That (functionState.Function.ReturnUrl, Is.EqualTo (_returnUrl));

      WxeFunctionState expiredFunctionState = WxeFunctionStateManager.Current.GetItem (c_functionTokenForExpiredFunctionState);
      Assert.That (expiredFunctionState, Is.Null);
    }

    [Test]
    public void CreateNewFunctionStateStateWithArgument ()
    {
      string agrumentValue = "True";
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set (TestFunction.Parameter1Name, agrumentValue);
      HttpContextHelper.SetQueryString (CurrentHttpContext, queryString);

      WxeFunctionState functionState = _wxeHandler.CreateNewFunctionState (CurrentHttpContext, _functionType);

      Assert.That (functionState, Is.Not.Null);
      Assert.That (functionState.FunctionToken, Is.Not.Null);
      Assert.That (functionState.Function, Is.Not.Null);
      Assert.That (functionState.Function.GetType(), Is.EqualTo (_functionType));
      TestFunction testFunction = (TestFunction) functionState.Function;
      Assert.That (testFunction.Parameter1, Is.EqualTo (agrumentValue));

      WxeFunctionState expiredFunctionState = WxeFunctionStateManager.Current.GetItem (c_functionTokenForExpiredFunctionState);
      Assert.That (expiredFunctionState, Is.Null);
    }

    [Test]
    public void CreateNewFunctionStateStateWithReturnUrlAndReturnToSelf ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set (WxeHandler.Parameters.ReturnUrl, _returnUrl);
      queryString.Set (WxeHandler.Parameters.WxeReturnToSelf, true.ToString());
      HttpContextHelper.SetQueryString (CurrentHttpContext, queryString);

      WxeFunctionState functionState = _wxeHandler.CreateNewFunctionState (CurrentHttpContext, _functionType);

      Assert.That (functionState, Is.Not.Null);
      Assert.That (functionState.FunctionToken, Is.Not.Null);
      Assert.That (functionState.Function, Is.Not.Null);
      Assert.That (functionState.Function.GetType(), Is.EqualTo (_functionType));
      Assert.That (functionState.Function.ReturnUrl, Is.EqualTo (_returnUrl));

      WxeFunctionState expiredFunctionState = WxeFunctionStateManager.Current.GetItem (c_functionTokenForExpiredFunctionState);
      Assert.That (expiredFunctionState, Is.Null);
    }

    [Test]
    public void CreateNewFunctionStateStateWithReturnToSelf ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set (WxeHandler.Parameters.WxeReturnToSelf, "True");
      HttpContextHelper.SetQueryString (CurrentHttpContext, queryString);

      WxeFunctionState functionState = _wxeHandler.CreateNewFunctionState (CurrentHttpContext, _functionType);

      Assert.That (functionState, Is.Not.Null);
      Assert.That (functionState.FunctionToken, Is.Not.Null);
      Assert.That (functionState.Function, Is.Not.Null);
      Assert.That (functionState.Function.GetType(), Is.EqualTo (_functionType));
      Assert.That (functionState.Function.ReturnUrl, Is.EqualTo (CurrentHttpContext.Request.RawUrl));

      WxeFunctionState expiredFunctionState = WxeFunctionStateManager.Current.GetItem (c_functionTokenForExpiredFunctionState);
      Assert.That (expiredFunctionState, Is.Null);
    }

    [Test]
    public void RetrieveExistingFunctionState ()
    {
      DateTime timeBeforeRefresh = DateTime.Now;
      Thread.Sleep (20);

      WxeFunctionState functionState = _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForFunctionStateWithEnabledCleanUp);

      Assert.That (functionState, Is.SameAs (_functionStateWithEnabledCleanUp));
      Assert.That (WxeFunctionStateManager.Current.GetLastAccess (c_functionTokenForFunctionStateWithEnabledCleanUp) > timeBeforeRefresh, Is.True);
      Assert.That (functionState.IsAborted, Is.False);
      Assert.That (WxeFunctionStateManager.Current.IsExpired (c_functionTokenForFunctionStateWithEnabledCleanUp), Is.False);

      WxeFunctionState expiredFunctionState = WxeFunctionStateManager.Current.GetItem (c_functionTokenForExpiredFunctionState);
      Assert.That (expiredFunctionState, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (WxeTimeoutException))]
    public void RetrieveMissingFunctionStateWithNoType ()
    {
      NameValueCollection form = new NameValueCollection();
      form.Set (WxeHandler.Parameters.WxeFunctionToken, c_functionTokenForMissingFunctionState);
      HttpContextHelper.SetForm (CurrentHttpContext, form);

      _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForMissingFunctionState);
    }

    [Test]
    public void RetrieveMissingFunctionStateWithTypeFromMapping ()
    {
      HttpContext context = HttpContextHelper.CreateHttpContext ("GET", "Test.wxe", null);

      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (_functionType, "~/Test.wxe"));

      WxeFunctionState functionState = _wxeHandler.ResumeExistingFunctionState (context, c_functionTokenForMissingFunctionState);

      Assert.That (functionState, Is.Not.Null);
      Assert.That (functionState.Function.GetType(), Is.EqualTo (_functionType));
    }

    [Test]
    [ExpectedException (typeof (WxeTimeoutException))]
    public void RetrieveMissingFunctionStateWithTypeFromMappingAndGetRequestWithPostBackAction ()
    {
      HttpContext context = HttpContextHelper.CreateHttpContext ("GET", "Test.wxe", null);
      NameValueCollection queryString = new NameValueCollection();
      queryString.Add (WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Refresh);
      HttpContextHelper.SetQueryString (context, queryString);

      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (typeof (TestFunction), "~/Test.wxe"));

      _wxeHandler.ResumeExistingFunctionState (context, c_functionTokenForMissingFunctionState);
    }

    [Test]
    [ExpectedException (typeof (WxeTimeoutException))]
    public void RetrieveMissingFunctionStateWithTypeFromMappingAndPostRequest ()
    {
      HttpContext context = HttpContextHelper.CreateHttpContext ("POST", "Test.wxe", null);
      NameValueCollection form = new NameValueCollection();
      form.Add (WxeHandler.Parameters.WxeFunctionToken, c_functionTokenForMissingFunctionState);
      HttpContextHelper.SetForm (context, form);

      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (typeof (TestFunction), "~/Test.wxe"));

      _wxeHandler.ResumeExistingFunctionState (context, c_functionTokenForMissingFunctionState);
    }

    [Test]
    public void RetrieveMissingFunctionStateWithTypeFromQueryString ()
    {
      HttpContext context = HttpContextHelper.CreateHttpContext ("GET", "Test.wxe", null);
      NameValueCollection queryString = new NameValueCollection();
      queryString.Add (WxeHandler.Parameters.WxeFunctionType, _functionTypeName);
      HttpContextHelper.SetQueryString (context, queryString);

      Remotion.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);

      WxeFunctionState functionState = _wxeHandler.ResumeExistingFunctionState (context, c_functionTokenForMissingFunctionState);
      Assert.That (functionState, Is.Not.Null);
      Assert.That (functionState.Function.GetType(), Is.EqualTo (typeof (TestFunction)));
    }

    [Test]
    [ExpectedException (typeof (WxeTimeoutException))]
    public void RetrieveMissingFunctionStateWithTypeFromQueryStringAndGetRequestWithPostBackAction ()
    {
      HttpContext context = HttpContextHelper.CreateHttpContext ("GET", "Test.wxe", null);
      NameValueCollection queryString = new NameValueCollection();
      queryString.Add (WxeHandler.Parameters.WxeFunctionType, _functionTypeName);
      queryString.Add (WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Refresh);
      HttpContextHelper.SetQueryString (context, queryString);

      Remotion.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);

      _wxeHandler.ResumeExistingFunctionState (context, c_functionTokenForMissingFunctionState);
    }

    [Test]
    [ExpectedException (typeof (WxeTimeoutException))]
    public void RetrieveMissingFunctionStateWithTypeFromQueryStringAndPostRequest ()
    {
      HttpContext context = HttpContextHelper.CreateHttpContext ("POST", "Test.wxe", null);
      NameValueCollection queryString = new NameValueCollection();
      queryString.Add (WxeHandler.Parameters.WxeFunctionType, _functionTypeName);
      HttpContextHelper.SetQueryString (context, queryString);

      Remotion.Web.ExecutionEngine.UrlMapping.UrlMappingConfiguration.SetCurrent (null);

      _wxeHandler.ResumeExistingFunctionState (context, c_functionTokenForMissingFunctionState);
    }

    [Test]
    [ExpectedException (typeof (WxeException),
        ExpectedMessage = "Function missing in WxeFunctionState " + c_functionTokenForFunctionStateWithMissingFunction + ".")]
    public void RetrieveFunctionStateWithMissingFunction ()
    {
      _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForFunctionStateWithMissingFunction);
    }

    [Test]
    [ExpectedException (typeof (WxeTimeoutException))]
    public void RetrieveExpiredFunctionState ()
    {
      NameValueCollection form = new NameValueCollection();
      form.Set (WxeHandler.Parameters.WxeFunctionToken, c_functionTokenForExpiredFunctionState);
      HttpContextHelper.SetForm (CurrentHttpContext, form);

      _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForExpiredFunctionState);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void RetrieveAbortedFunctionState ()
    {
      NameValueCollection form = new NameValueCollection();
      form.Set (WxeHandler.Parameters.WxeFunctionToken, c_functionTokenForAbortedFunctionState);
      HttpContextHelper.SetForm (CurrentHttpContext, form);

      _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForAbortedFunctionState);
    }

    [Test]
    public void RefreshExistingFunctionState ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set (WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Refresh);
      HttpContextHelper.SetQueryString (CurrentHttpContext, queryString);

      DateTime timeBeforeRefresh = DateTime.Now;
      Thread.Sleep (20);

      WxeFunctionState functionState = _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForFunctionStateWithEnabledCleanUp);

      Assert.That (functionState, Is.Null);
      Assert.That (WxeFunctionStateManager.Current.GetLastAccess (c_functionTokenForFunctionStateWithEnabledCleanUp) > timeBeforeRefresh, Is.True);
      Assert.That (_functionStateWithEnabledCleanUp.IsAborted, Is.False);
      Assert.That (WxeFunctionStateManager.Current.IsExpired (c_functionTokenForFunctionStateWithEnabledCleanUp), Is.False);
    }

    [Test]
    public void RefreshExistingFunctionStateWithMissingFunction ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set (WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Refresh);
      HttpContextHelper.SetQueryString (CurrentHttpContext, queryString);

      DateTime timeBeforeRefresh = DateTime.Now;
      Thread.Sleep (20);

      WxeFunctionState functionState =
          _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForFunctionStateWithMissingFunction);

      Assert.That (functionState, Is.Null);
      Assert.That (WxeFunctionStateManager.Current.GetLastAccess (c_functionTokenForFunctionStateWithMissingFunction) > timeBeforeRefresh, Is.True);
      Assert.That (_functionStateWithMissingFunction.IsAborted, Is.False);
      Assert.That (WxeFunctionStateManager.Current.IsExpired (c_functionTokenForFunctionStateWithMissingFunction), Is.False);
    }

    [Test]
    public void AbortExistingFunctionState ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set (WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Abort);
      HttpContextHelper.SetQueryString (CurrentHttpContext, queryString);

      WxeFunctionState functionState = _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForFunctionStateWithEnabledCleanUp);

      Assert.That (functionState, Is.Null);
      Assert.That (_functionStateWithEnabledCleanUp.IsAborted, Is.True);

      WxeFunctionState expiredFunctionState = WxeFunctionStateManager.Current.GetItem (c_functionTokenForExpiredFunctionState);
      Assert.That (expiredFunctionState, Is.Null);
    }

    [Test]
    public void AbortExistingFunctionStateMissingFunction ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set (WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Abort);
      HttpContextHelper.SetQueryString (CurrentHttpContext, queryString);

      WxeFunctionState functionState =
          _wxeHandler.ResumeExistingFunctionState (CurrentHttpContext, c_functionTokenForFunctionStateWithMissingFunction);

      Assert.That (functionState, Is.Null);
      Assert.That (_functionStateWithMissingFunction.IsAborted, Is.True);
    }

    [Test]
    public void CleanUpFunctionStateWithEnabledCleanUp ()
    {
      _wxeHandler.CleanUpFunctionState (_functionStateWithEnabledCleanUp);
      Assert.That (_functionStateWithEnabledCleanUp.IsAborted, Is.True);
    }

    [Test]
    public void CleanUpFunctionStateWithDisabledCleanUp ()
    {
      _wxeHandler.CleanUpFunctionState (_functionStateWithDisabledCleanUp);
      Assert.That (_functionStateWithEnabledCleanUp.IsAborted, Is.False);
    }

    [Test]
    public void CleanUpFunctionStateWithChildFunction ()
    {
      _wxeHandler.CleanUpFunctionState (_functionStateWithChildFunction);
      Assert.That (_functionStateWithChildFunction.IsAborted, Is.False);
    }

    [Test]
    public void ExecuteFunctionState ()
    {
      CurrentHttpContext.Items["Test"] = new object();
      _wxeHandler.ExecuteFunctionState (CurrentHttpContext, _functionStateWithEnabledCleanUp, true);
      TestFunction function = (TestFunction) _functionStateWithEnabledCleanUp.Function;

      WxeContext wxeContext = function.TestStep.WxeContext;
      Assert.That (wxeContext, Is.SameAs (WxeContext.Current));
      Assert.That (wxeContext.HttpContext.Items["Test"], Is.SameAs (CurrentHttpContext.Items["Test"]));
      Assert.That (wxeContext.FunctionToken, Is.EqualTo (_functionStateWithEnabledCleanUp.FunctionToken));
      Assert.That (function.LastExecutedStepID, Is.EqualTo ("4"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void ExecuteAbortedFunctionState ()
    {
      _wxeHandler.ExecuteFunctionState (CurrentHttpContext, _functionStateAborted, true);
    }

    [Test]
    public void ExecuteFunction ()
    {
      TestFunction function = (TestFunction) _functionStateWithEnabledCleanUp.Function;
      _wxeHandler.ExecuteFunction (function, CurrentWxeContext, true);

      WxeContext wxeContext = function.TestStep.WxeContext;
      Assert.That (wxeContext, Is.SameAs (WxeContext.Current));

      Type[] catchExceptionTypes = function.ExceptionHandler.GetCatchExceptionTypes();
      Assert.That (catchExceptionTypes.Length, Is.EqualTo (1));
      Assert.That (catchExceptionTypes[0], Is.SameAs (typeof (WxeUserCancelException)));

      Assert.That (function.LastExecutedStepID, Is.EqualTo ("4"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void ExecuteAbortedFunction ()
    {
      TestFunction function = (TestFunction) _functionStateAborted.Function;
      _wxeHandler.ExecuteFunction (function, CurrentWxeContext, true);
    }
  }
}
