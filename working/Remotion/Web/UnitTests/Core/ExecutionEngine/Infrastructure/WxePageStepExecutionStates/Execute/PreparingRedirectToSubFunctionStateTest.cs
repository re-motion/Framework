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
using System.Text;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute
{
  [TestFixture]
  public class PreparingRedirectToSubFunctionStateTest : TestBase
  {
    public override void SetUp ()
    {
      base.SetUp();

      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (RootFunction.GetType(), "~/root.wxe"));
      UrlMappingConfiguration.Current.Mappings.Add (new UrlMappingEntry (SubFunction.GetType(), "~/sub.wxe"));

      Uri uri = new Uri ("http://localhost/root.wxe");

      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("~/sub.wxe")).Return ("/session/sub.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("/session/sub.wxe")).Return ("/session/sub.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("~/root.wxe")).Return ("/session/root.wxe").Repeat.Any ();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("/root.wxe")).Return ("/session/root.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ApplyAppPathModifier ("/session/root.wxe")).Return ("/session/root.wxe").Repeat.Any();
      ResponseMock.Stub (stub => stub.ContentEncoding).Return (Encoding.Default).Repeat.Any();

      RequestMock.Stub (stub => stub.Url).Return (uri).Repeat.Any();
      RequestMock.Stub (stub => stub.ContentEncoding).Return (Encoding.Default).Repeat.Any();
    }

    [Test]
    public void IsExecuting ()
    {
      IExecutionState executionState = CreateExecutionState (new WxePermaUrlOptions());
      Assert.That (executionState.IsExecuting, Is.True);
    }

    [Test]
    public void ExecuteSubFunction_WithPermaUrl_GoesToExecutingSubFunction ()
    {
      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      IExecutionState executionState = CreateExecutionState (permaUrlOptions);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<RedirectingToSubFunctionState>.Is.NotNull))
          .WhenCalled (
          invocation =>
          {
            var nextState = CheckExecutionState ((RedirectingToSubFunctionState) invocation.Arguments[0]);
            Assert.That (
                nextState.Parameters.DestinationUrl,
                Is.EqualTo ("/session/sub.wxe?Parameter1=OtherValue&WxeFunctionToken=" + WxeContext.FunctionToken));
            Assert.That (nextState.Parameters.ResumeUrl, Is.EqualTo ("/session/root.wxe?WxeFunctionToken=" + WxeContext.FunctionToken));
          });

      MockRepository.ReplayAll();

      executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_WithPermaUrl_WithCustumUrlParamters_GoesToExecutingSubFunction ()
    {
      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions (false, new NameValueCollection { { "Key", "Value" } });
      IExecutionState executionState = CreateExecutionState (permaUrlOptions);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<RedirectingToSubFunctionState>.Is.NotNull))
          .WhenCalled (
          invocation =>
          {
            var nextState = CheckExecutionState ((RedirectingToSubFunctionState) invocation.Arguments[0]);
            Assert.That (
                nextState.Parameters.DestinationUrl,
                Is.EqualTo ("/session/sub.wxe?Key=Value&WxeFunctionToken=" + WxeContext.FunctionToken));
            Assert.That (nextState.Parameters.ResumeUrl, Is.EqualTo ("/session/root.wxe?WxeFunctionToken=" + WxeContext.FunctionToken));
          });

      MockRepository.ReplayAll();

      executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }

    [Test]
    public void ExecuteSubFunction_WithPermaUrl_WithParentPermaUrl_GoesToExecutingSubFunction ()
    {
      WxeContext.QueryString.Add ("Key", "Value");

      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions (true);
      IExecutionState executionState = CreateExecutionState (permaUrlOptions);

      ExecutionStateContextMock.Expect (mock => mock.SetExecutionState (Arg<RedirectingToSubFunctionState>.Is.NotNull))
          .WhenCalled (
          invocation =>
          {
            var nextState = CheckExecutionState ((RedirectingToSubFunctionState) invocation.Arguments[0]);

            string destinationUrl = UrlUtility.AddParameters (
                "/session/sub.wxe",
                new NameValueCollection
                {
                    { "Parameter1", "OtherValue" },
                    { WxeHandler.Parameters.WxeFunctionToken, WxeContext.FunctionToken },
                    { WxeHandler.Parameters.ReturnUrl, "/root.wxe?Key=Value" }
                },
                Encoding.Default);
            Assert.That (nextState.Parameters.DestinationUrl, Is.EqualTo (destinationUrl));

            Assert.That (nextState.Parameters.ResumeUrl, Is.EqualTo ("/session/root.wxe?Key=Value&WxeFunctionToken=" + WxeContext.FunctionToken));
          });

      MockRepository.ReplayAll();

      executionState.ExecuteSubFunction (WxeContext);

      MockRepository.VerifyAll();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The 'PreparingRedirectToSubFunctionState' type only supports WxePermaUrlOptions with the UsePermaUrl-flag set to true.\r\n"
        + "Parameter name: parameters",
        MatchType = MessageMatch.Contains)]
    public void Initialize_WithoutPermaUrl ()
    {
      CreateExecutionState (WxePermaUrlOptions.Null);
    }

    private PreparingRedirectToSubFunctionState CreateExecutionState (WxePermaUrlOptions permaUrlOptions)
    {
      return new PreparingRedirectToSubFunctionState (
          ExecutionStateContextMock, new PreparingRedirectToSubFunctionStateParameters (SubFunction, PostBackCollection, permaUrlOptions));
    }
  }
}
