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

      _functionStateWithEnabledCleanUp = new WxeFunctionStateMock(new TestFunction(), 10, true, c_functionTokenForFunctionStateWithEnabledCleanUp);
      WxeFunctionStateManager.Current.Add(_functionStateWithEnabledCleanUp);

      _functionStateWithDisabledCleanUp = new WxeFunctionStateMock(new TestFunction(), 10, false, c_functionTokenForFunctionStateWithDisabledCleanUp);
      WxeFunctionStateManager.Current.Add(_functionStateWithDisabledCleanUp);

      _functionStateWithMissingFunction = new WxeFunctionStateMock(new TestFunction(), 10, false, c_functionTokenForFunctionStateWithMissingFunction);
      _functionStateWithMissingFunction.Function = null;
      WxeFunctionStateManager.Current.Add(_functionStateWithMissingFunction);

      _functionStateAborted = new WxeFunctionStateMock(new TestFunction(), 10, true, c_functionTokenForAbortedFunctionState);
      WxeFunctionStateManager.Current.Add(_functionStateAborted);
      _functionStateAborted.Abort();

      _functionStateExpired = new WxeFunctionStateMock(new TestFunction(), 0, true, c_functionTokenForExpiredFunctionState);
      WxeFunctionStateManager.Current.Add(_functionStateExpired);

      TestFunction rootFunction = new TestFunction();
      TestFunction childFunction = new TestFunction();
      rootFunction.Add(childFunction);
      _functionStateWithChildFunction = new WxeFunctionStateMock(childFunction, 10, true, c_functionTokenForFunctionStateWithChildFunction);
      WxeFunctionStateManager.Current.Add(_functionStateWithChildFunction);

      _functionType = typeof(TestFunction);
      _functionTypeName = _functionType.AssemblyQualifiedName;
      _invalidFunctionTypeName = "Remotion.Web.UnitTests::ExecutionEngine.InvalidFunction";

      Thread.Sleep(20);
      UrlMappingConfiguration.SetCurrent(null);
    }

    [TearDown]
    public override void TearDown ()
    {
      WxeFunctionStateManager.Current.Abort(_functionStateWithEnabledCleanUp);
      WxeFunctionStateManager.Current.Abort(_functionStateWithDisabledCleanUp);
      WxeFunctionStateManager.Current.Abort(_functionStateWithMissingFunction);
      WxeFunctionStateManager.Current.Abort(_functionStateAborted);
      WxeFunctionStateManager.Current.Abort(_functionStateExpired);
      WxeFunctionStateManager.Current.Abort(_functionStateWithChildFunction);

      UrlMappingConfiguration.SetCurrent(null);
      SafeContext.Instance.FreeData(typeof(WxeFunctionStateManager).AssemblyQualifiedName);
      base.TearDown();
    }

    [Test]
    public void CreateNewFunctionStateState ()
    {
      WxeFunctionState functionState = _wxeHandler.CreateNewFunctionState(CurrentHttpContext, _functionType);

      Assert.That(functionState, Is.Not.Null);
      Assert.That(functionState.FunctionToken, Is.Not.Null);
      Assert.That(functionState.Function, Is.Not.Null);
      Assert.That(functionState.Function.GetType(), Is.EqualTo(_functionType));
      Assert.That(functionState.Function.ReturnUrl, Is.EqualTo(TestFunction.ReturnUrlValue));

      WxeFunctionState expiredFunctionState = WxeFunctionStateManager.Current.GetItem(c_functionTokenForExpiredFunctionState);
      Assert.That(expiredFunctionState, Is.Null);
    }

    [Test]
    public void GetFunctionTypeWithInvalidTypeName ()
    {
      Assert.That(
          () => _wxeHandler.GetTypeByTypeName(_invalidFunctionTypeName),
          Throws.InstanceOf<WxeException>());
    }

    [Test]
    public void GetFunctionTypeByTypeName ()
    {
      Type type = _wxeHandler.GetTypeByTypeName(_functionTypeName);
      Assert.That(type, Is.Not.Null);
      Assert.That(type, Is.EqualTo(_functionType));
    }

    [Test]
    public void GetFunctionTypeByPath ()
    {
      UrlMappingConfiguration.SetCurrent(UrlMappingConfiguration.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml"));
      UrlMappingConfiguration.Current.Mappings.Add(new UrlMappingEntry(_functionType, "~/Test.wxe"));

      Type type = _wxeHandler.GetTypeByPath(@"/Test.wxe");

      Assert.That(type, Is.Not.Null);
      Assert.That(type, Is.EqualTo(_functionType));
    }

    [Test]
    public void GetFunctionTypeByPathWithoutMapping ()
    {
      UrlMappingConfiguration.SetCurrent(UrlMappingConfiguration.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml"));

      Assert.That(
          () => _wxeHandler.GetTypeByPath(@"/Test1.wxe"),
          Throws.InstanceOf<WxeException>());
    }

    [Test]
    public void CreateNewFunctionStateStateWithReturnUrl ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set(WxeHandler.Parameters.ReturnUrl, _returnUrl);
      HttpContextHelper.SetQueryString(CurrentHttpContext, queryString);

      WxeFunctionState functionState = _wxeHandler.CreateNewFunctionState(CurrentHttpContext, _functionType);

      Assert.That(functionState, Is.Not.Null);
      Assert.That(functionState.FunctionToken, Is.Not.Null);
      Assert.That(functionState.Function, Is.Not.Null);
      Assert.That(functionState.Function.GetType(), Is.EqualTo(_functionType));
      Assert.That(functionState.Function.ReturnUrl, Is.EqualTo(_returnUrl));

      WxeFunctionState expiredFunctionState = WxeFunctionStateManager.Current.GetItem(c_functionTokenForExpiredFunctionState);
      Assert.That(expiredFunctionState, Is.Null);
    }

    [Test]
    public void CreateNewFunctionStateStateWithArgument ()
    {
      string agrumentValue = "True";
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set(TestFunction.Parameter1Name, agrumentValue);
      HttpContextHelper.SetQueryString(CurrentHttpContext, queryString);

      WxeFunctionState functionState = _wxeHandler.CreateNewFunctionState(CurrentHttpContext, _functionType);

      Assert.That(functionState, Is.Not.Null);
      Assert.That(functionState.FunctionToken, Is.Not.Null);
      Assert.That(functionState.Function, Is.Not.Null);
      Assert.That(functionState.Function.GetType(), Is.EqualTo(_functionType));
      TestFunction testFunction = (TestFunction)functionState.Function;
      Assert.That(testFunction.Parameter1, Is.EqualTo(agrumentValue));

      WxeFunctionState expiredFunctionState = WxeFunctionStateManager.Current.GetItem(c_functionTokenForExpiredFunctionState);
      Assert.That(expiredFunctionState, Is.Null);
    }

    [Test]
    public void CreateNewFunctionStateStateWithReturnUrlAndReturnToSelf ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set(WxeHandler.Parameters.ReturnUrl, _returnUrl);
      queryString.Set(WxeHandler.Parameters.WxeReturnToSelf, true.ToString());
      HttpContextHelper.SetQueryString(CurrentHttpContext, queryString);

      WxeFunctionState functionState = _wxeHandler.CreateNewFunctionState(CurrentHttpContext, _functionType);

      Assert.That(functionState, Is.Not.Null);
      Assert.That(functionState.FunctionToken, Is.Not.Null);
      Assert.That(functionState.Function, Is.Not.Null);
      Assert.That(functionState.Function.GetType(), Is.EqualTo(_functionType));
      Assert.That(functionState.Function.ReturnUrl, Is.EqualTo(_returnUrl));

      WxeFunctionState expiredFunctionState = WxeFunctionStateManager.Current.GetItem(c_functionTokenForExpiredFunctionState);
      Assert.That(expiredFunctionState, Is.Null);
    }

    [Test]
    public void CreateNewFunctionStateStateWithReturnToSelf ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set(WxeHandler.Parameters.WxeReturnToSelf, "True");
      HttpContextHelper.SetQueryString(CurrentHttpContext, queryString);

      WxeFunctionState functionState = _wxeHandler.CreateNewFunctionState(CurrentHttpContext, _functionType);

      Assert.That(functionState, Is.Not.Null);
      Assert.That(functionState.FunctionToken, Is.Not.Null);
      Assert.That(functionState.Function, Is.Not.Null);
      Assert.That(functionState.Function.GetType(), Is.EqualTo(_functionType));
      Assert.That(functionState.Function.ReturnUrl, Is.EqualTo(CurrentHttpContext.Request.RawUrl));

      WxeFunctionState expiredFunctionState = WxeFunctionStateManager.Current.GetItem(c_functionTokenForExpiredFunctionState);
      Assert.That(expiredFunctionState, Is.Null);
    }

    [Test]
    public void RetrieveExistingFunctionState ()
    {
      DateTime timeBeforeRefresh = DateTime.UtcNow;
      Thread.Sleep(20);

      WxeFunctionState functionState = _wxeHandler.ResumeExistingFunctionState(CurrentHttpContext, c_functionTokenForFunctionStateWithEnabledCleanUp);

      Assert.That(functionState, Is.SameAs(_functionStateWithEnabledCleanUp));
      Assert.That(WxeFunctionStateManager.Current.GetLastAccessUtc(c_functionTokenForFunctionStateWithEnabledCleanUp) > timeBeforeRefresh, Is.True);
      Assert.That(functionState.IsAborted, Is.False);
      Assert.That(WxeFunctionStateManager.Current.IsExpired(c_functionTokenForFunctionStateWithEnabledCleanUp), Is.False);

      WxeFunctionState expiredFunctionState = WxeFunctionStateManager.Current.GetItem(c_functionTokenForExpiredFunctionState);
      Assert.That(expiredFunctionState, Is.Null);
    }

    [Test]
    public void RetrieveMissingFunctionStateWithNoType ()
    {
      UrlMappingConfiguration.SetCurrent(UrlMappingConfiguration.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml"));

      NameValueCollection form = new NameValueCollection();
      form.Set(WxeHandler.Parameters.WxeFunctionToken, c_functionTokenForMissingFunctionState);
      HttpContextHelper.SetForm(CurrentHttpContext, form);
      Assert.That(
          () => _wxeHandler.ResumeExistingFunctionState(CurrentHttpContext, c_functionTokenForMissingFunctionState),
          Throws.InstanceOf<WxeTimeoutException>());
    }

    [Test]
    public void RetrieveMissingFunctionStateWithTypeFromMapping ()
    {
      HttpContext context = HttpContextHelper.CreateHttpContext("GET", "Test.wxe", null);
      UrlMappingConfiguration.SetCurrent(UrlMappingConfiguration.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml"));
      UrlMappingConfiguration.Current.Mappings.Add(new UrlMappingEntry(_functionType, "~/Test.wxe"));

      WxeFunctionState functionState = _wxeHandler.ResumeExistingFunctionState(context, c_functionTokenForMissingFunctionState);

      Assert.That(functionState, Is.Not.Null);
      Assert.That(functionState.Function.GetType(), Is.EqualTo(_functionType));
    }

    [Test]
    public void RetrieveMissingFunctionStateWithTypeFromMappingAndGetRequestWithPostBackAction_FailsWithStatusCode500 ()
    {
      HttpContext context = HttpContextHelper.CreateHttpContext("GET", "Test.wxe", null);
      NameValueCollection queryString = new NameValueCollection();
      queryString.Add(WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Refresh);
      HttpContextHelper.SetQueryString(context, queryString);
      UrlMappingConfiguration.SetCurrent(UrlMappingConfiguration.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml"));
      UrlMappingConfiguration.Current.Mappings.Add(new UrlMappingEntry(typeof(TestFunction), "~/Test.wxe"));

      _wxeHandler.ResumeExistingFunctionState(context, c_functionTokenForMissingFunctionState);

      Assert.That(context.Response.StatusCode, Is.EqualTo(409));
      Assert.That(context.Response.StatusDescription, Is.EqualTo("Function Timeout."));
    }

    [Test]
    public void RetrieveMissingFunctionStateWithTypeFromMappingAndPostRequest ()
    {
      HttpContext context = HttpContextHelper.CreateHttpContext("POST", "Test.wxe", null);
      NameValueCollection form = new NameValueCollection();
      form.Add(WxeHandler.Parameters.WxeFunctionToken, c_functionTokenForMissingFunctionState);
      HttpContextHelper.SetForm(context, form);

      UrlMappingConfiguration.SetCurrent(UrlMappingConfiguration.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml"));
      UrlMappingConfiguration.Current.Mappings.Add(new UrlMappingEntry(typeof(TestFunction), "~/Test.wxe"));
      Assert.That(
          () => _wxeHandler.ResumeExistingFunctionState(context, c_functionTokenForMissingFunctionState),
          Throws.InstanceOf<WxeTimeoutException>());
    }

    [Test]
    public void RetrieveMissingFunctionStateWithTypeFromQueryString ()
    {
      HttpContext context = HttpContextHelper.CreateHttpContext("GET", "Test.wxe", null);
      NameValueCollection queryString = new NameValueCollection();
      queryString.Add(WxeHandler.Parameters.WxeFunctionType, _functionTypeName);
      HttpContextHelper.SetQueryString(context, queryString);

      UrlMappingConfiguration.SetCurrent(null);

      WxeFunctionState functionState = _wxeHandler.ResumeExistingFunctionState(context, c_functionTokenForMissingFunctionState);
      Assert.That(functionState, Is.Not.Null);
      Assert.That(functionState.Function.GetType(), Is.EqualTo(typeof(TestFunction)));
    }

    [Test]
    public void RetrieveMissingFunctionStateWithTypeFromQueryStringAndGetRequestWithPostBackAction_FailsWithStatusCode500 ()
    {
      HttpContext context = HttpContextHelper.CreateHttpContext("GET", "Test.wxe", null);
      NameValueCollection queryString = new NameValueCollection();
      queryString.Add(WxeHandler.Parameters.WxeFunctionType, _functionTypeName);
      queryString.Add(WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Refresh);
      HttpContextHelper.SetQueryString(context, queryString);

      UrlMappingConfiguration.SetCurrent(null);

      _wxeHandler.ResumeExistingFunctionState(context, c_functionTokenForMissingFunctionState);

      Assert.That(context.Response.StatusCode, Is.EqualTo(409));
      Assert.That(context.Response.StatusDescription, Is.EqualTo("Function Timeout."));
    }

    [Test]
    public void RetrieveMissingFunctionStateWithTypeFromQueryStringAndPostRequest ()
    {
      HttpContext context = HttpContextHelper.CreateHttpContext("POST", "Test.wxe", null);
      NameValueCollection queryString = new NameValueCollection();
      queryString.Add(WxeHandler.Parameters.WxeFunctionType, _functionTypeName);
      HttpContextHelper.SetQueryString(context, queryString);

      UrlMappingConfiguration.SetCurrent(null);
      Assert.That(
          () => _wxeHandler.ResumeExistingFunctionState(context, c_functionTokenForMissingFunctionState),
          Throws.InstanceOf<WxeTimeoutException>());
    }

    [Test]
    public void RetrieveFunctionStateWithMissingFunction ()
    {
      Assert.That(
          () => _wxeHandler.ResumeExistingFunctionState(CurrentHttpContext, c_functionTokenForFunctionStateWithMissingFunction),
          Throws.InstanceOf<WxeException>()
              .With.Message.EqualTo(
                  "Function missing in WxeFunctionState " + c_functionTokenForFunctionStateWithMissingFunction + "."));
    }

    [Test]
    public void RetrieveExpiredFunctionState ()
    {
      UrlMappingConfiguration.SetCurrent(UrlMappingConfiguration.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml"));

      NameValueCollection form = new NameValueCollection();
      form.Set(WxeHandler.Parameters.WxeFunctionToken, c_functionTokenForExpiredFunctionState);
      HttpContextHelper.SetForm(CurrentHttpContext, form);
      Assert.That(
          () => _wxeHandler.ResumeExistingFunctionState(CurrentHttpContext, c_functionTokenForExpiredFunctionState),
          Throws.InstanceOf<WxeTimeoutException>());
    }

    [Test]
    public void RetrieveAbortedFunctionState ()
    {
      NameValueCollection form = new NameValueCollection();
      form.Set(WxeHandler.Parameters.WxeFunctionToken, c_functionTokenForAbortedFunctionState);
      HttpContextHelper.SetForm(CurrentHttpContext, form);
      Assert.That(
          () => _wxeHandler.ResumeExistingFunctionState(CurrentHttpContext, c_functionTokenForAbortedFunctionState),
          Throws.InvalidOperationException);
    }

    [Test]
    public void RefreshExistingFunctionState_Succeeds ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set(WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Refresh);
      HttpContextHelper.SetQueryString(CurrentHttpContext, queryString);

      DateTime timeBeforeRefresh = DateTime.UtcNow;
      Thread.Sleep(20);

      WxeFunctionState functionState = _wxeHandler.ResumeExistingFunctionState(CurrentHttpContext, c_functionTokenForFunctionStateWithEnabledCleanUp);

      Assert.That(functionState, Is.Null);
      Assert.That(WxeFunctionStateManager.Current.GetLastAccessUtc(c_functionTokenForFunctionStateWithEnabledCleanUp) > timeBeforeRefresh, Is.True);
      Assert.That(_functionStateWithEnabledCleanUp.IsAborted, Is.False);
      Assert.That(WxeFunctionStateManager.Current.IsExpired(c_functionTokenForFunctionStateWithEnabledCleanUp), Is.False);

      Assert.That(CurrentHttpContext.Response.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public void RefreshExistingFunctionStateWithMissingFunction_Succeeds ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set(WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Refresh);
      HttpContextHelper.SetQueryString(CurrentHttpContext, queryString);

      DateTime timeBeforeRefresh = DateTime.UtcNow;
      Thread.Sleep(20);

      WxeFunctionState functionState =
          _wxeHandler.ResumeExistingFunctionState(CurrentHttpContext, c_functionTokenForFunctionStateWithMissingFunction);

      Assert.That(functionState, Is.Null);
      Assert.That(WxeFunctionStateManager.Current.GetLastAccessUtc(c_functionTokenForFunctionStateWithMissingFunction) > timeBeforeRefresh, Is.True);
      Assert.That(_functionStateWithMissingFunction.IsAborted, Is.False);
      Assert.That(WxeFunctionStateManager.Current.IsExpired(c_functionTokenForFunctionStateWithMissingFunction), Is.False);

      Assert.That(CurrentHttpContext.Response.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public void RefreshExpiredFunctionState_FailsWithStatusCode500 ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set(WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Refresh);
      HttpContextHelper.SetQueryString(CurrentHttpContext, queryString);

      _wxeHandler.ResumeExistingFunctionState(CurrentHttpContext, c_functionTokenForExpiredFunctionState);

      Assert.That(CurrentHttpContext.Response.StatusCode, Is.EqualTo(409));
      Assert.That(CurrentHttpContext.Response.StatusDescription, Is.EqualTo("Function Timeout."));
    }

    [Test]
    public void RefreshWithoutSession_FailsWithStatusCode500 ()
    {
      CurrentHttpContext.Session.Clear();
      Assert.That(WxeFunctionStateManager.HasSession, Is.False);

      NameValueCollection queryString = new NameValueCollection();
      queryString.Set(WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Refresh);
      HttpContextHelper.SetQueryString(CurrentHttpContext, queryString);

      _wxeHandler.ResumeExistingFunctionState(CurrentHttpContext, c_functionTokenForExpiredFunctionState);

      Assert.That(CurrentHttpContext.Response.StatusCode, Is.EqualTo(409));
      Assert.That(CurrentHttpContext.Response.StatusDescription, Is.EqualTo("Session Timeout."));
    }

    [Test]
    public void AbortExistingFunctionState_Succeeds ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set(WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Abort);
      HttpContextHelper.SetQueryString(CurrentHttpContext, queryString);

      WxeFunctionState functionState = _wxeHandler.ResumeExistingFunctionState(CurrentHttpContext, c_functionTokenForFunctionStateWithEnabledCleanUp);

      Assert.That(functionState, Is.Null);
      Assert.That(_functionStateWithEnabledCleanUp.IsAborted, Is.True);

      WxeFunctionState expiredFunctionState = WxeFunctionStateManager.Current.GetItem(c_functionTokenForExpiredFunctionState);
      Assert.That(expiredFunctionState, Is.Null);

      Assert.That(CurrentHttpContext.Response.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public void AbortExistingFunctionStateMissingFunction_Succeeds ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set(WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Abort);
      HttpContextHelper.SetQueryString(CurrentHttpContext, queryString);

      WxeFunctionState functionState =
          _wxeHandler.ResumeExistingFunctionState(CurrentHttpContext, c_functionTokenForFunctionStateWithMissingFunction);

      Assert.That(functionState, Is.Null);
      Assert.That(_functionStateWithMissingFunction.IsAborted, Is.True);

      Assert.That(CurrentHttpContext.Response.StatusCode, Is.EqualTo(200));
    }

    [Test]
    public void AbortExpiredFunctionState_FailsWithStatusCode500 ()
    {
      NameValueCollection queryString = new NameValueCollection();
      queryString.Set(WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Abort);
      HttpContextHelper.SetQueryString(CurrentHttpContext, queryString);

      _wxeHandler.ResumeExistingFunctionState(CurrentHttpContext, c_functionTokenForExpiredFunctionState);

      Assert.That(CurrentHttpContext.Response.StatusCode, Is.EqualTo(409));
      Assert.That(CurrentHttpContext.Response.StatusDescription, Is.EqualTo("Function Timeout."));
    }

    [Test]
    public void AbortWithoutSession_FailsWithStatusCode500 ()
    {
      CurrentHttpContext.Session.Clear();
      Assert.That(WxeFunctionStateManager.HasSession, Is.False);

      NameValueCollection queryString = new NameValueCollection();
      queryString.Set(WxeHandler.Parameters.WxeAction, WxeHandler.Actions.Abort);
      HttpContextHelper.SetQueryString(CurrentHttpContext, queryString);

      _wxeHandler.ResumeExistingFunctionState(CurrentHttpContext, c_functionTokenForExpiredFunctionState);

      Assert.That(CurrentHttpContext.Response.StatusCode, Is.EqualTo(409));
      Assert.That(CurrentHttpContext.Response.StatusDescription, Is.EqualTo("Session Timeout."));
    }

    [Test]
    public void CleanUpFunctionStateWithEnabledCleanUp ()
    {
      _wxeHandler.CleanUpFunctionState(_functionStateWithEnabledCleanUp);
      Assert.That(_functionStateWithEnabledCleanUp.IsAborted, Is.True);
    }

    [Test]
    public void CleanUpFunctionStateWithDisabledCleanUp ()
    {
      _wxeHandler.CleanUpFunctionState(_functionStateWithDisabledCleanUp);
      Assert.That(_functionStateWithEnabledCleanUp.IsAborted, Is.False);
    }

    [Test]
    public void CleanUpFunctionStateWithChildFunction ()
    {
      _wxeHandler.CleanUpFunctionState(_functionStateWithChildFunction);
      Assert.That(_functionStateWithChildFunction.IsAborted, Is.False);
    }

    [Test]
    public void ExecuteFunctionState ()
    {
      CurrentHttpContext.Items["Test"] = new object();
      _wxeHandler.ExecuteFunctionState(CurrentHttpContext, _functionStateWithEnabledCleanUp, true);
      TestFunction function = (TestFunction)_functionStateWithEnabledCleanUp.Function;

      WxeContext wxeContext = function.TestStep.WxeContext;
      Assert.That(wxeContext, Is.SameAs(WxeContext.Current));
      Assert.That(wxeContext.HttpContext.Items["Test"], Is.SameAs(CurrentHttpContext.Items["Test"]));
      Assert.That(wxeContext.FunctionToken, Is.EqualTo(_functionStateWithEnabledCleanUp.FunctionToken));
      Assert.That(function.LastExecutedStepID, Is.EqualTo("4"));
    }

    [Test]
    public void ExecuteAbortedFunctionState ()
    {
      Assert.That(
          () => _wxeHandler.ExecuteFunctionState(CurrentHttpContext, _functionStateAborted, true),
          Throws.ArgumentException);
    }

    [Test]
    public void ExecuteFunction ()
    {
      TestFunction function = (TestFunction)_functionStateWithEnabledCleanUp.Function;
      _wxeHandler.ExecuteFunction(function, CurrentWxeContext, true);

      WxeContext wxeContext = function.TestStep.WxeContext;
      Assert.That(wxeContext, Is.SameAs(WxeContext.Current));

      Type[] catchExceptionTypes = function.ExceptionHandler.GetCatchExceptionTypes();
      Assert.That(catchExceptionTypes.Length, Is.EqualTo(1));
      Assert.That(catchExceptionTypes[0], Is.SameAs(typeof(WxeUserCancelException)));

      Assert.That(function.LastExecutedStepID, Is.EqualTo("4"));
    }

    [Test]
    public void ExecuteAbortedFunction ()
    {
      TestFunction function = (TestFunction)_functionStateAborted.Function;
      Assert.That(
          () => _wxeHandler.ExecuteFunction(function, CurrentWxeContext, true),
          Throws.ArgumentException);
    }
  }
}
