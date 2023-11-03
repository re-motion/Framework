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
using Moq;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates;
using Remotion.Web.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute;
using Remotion.Web.ExecutionEngine.UrlMapping;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Core.ExecutionEngine.Infrastructure.WxePageStepExecutionStates.Execute
{
  [TestFixture]
  public class PreparingRedirectToSubFunctionStateTest : TestBase
  {
    public override void SetUp ()
    {
      base.SetUp();

      UrlMappingConfiguration.SetCurrent(UrlMappingConfiguration.CreateUrlMappingConfiguration(@"Res\UrlMapping.xml"));
      UrlMappingConfiguration.Current.Mappings.Add(new UrlMappingEntry(RootFunction.GetType(), "~/root.wxe"));
      UrlMappingConfiguration.Current.Mappings.Add(new UrlMappingEntry(SubFunction.Object.GetType(), "~/sub.wxe"));

      Uri uri = new Uri("http://localhost/AppDir/root.wxe");

      ResponseMock.Setup(stub => stub.ContentEncoding).Returns(Encoding.UTF8);

      RequestMock.Setup(stub => stub.Url).Returns(uri);
      RequestMock.Setup(stub => stub.ApplicationPath).Returns("/AppDir");
      RequestMock.Setup(stub => stub.ContentEncoding).Returns(Encoding.UTF8);
    }

    [Test]
    public void IsExecuting ()
    {
      IExecutionState executionState = CreateExecutionState(new WxePermaUrlOptions());
      Assert.That(executionState.IsExecuting, Is.True);
    }

    [Test]
    public void ExecuteSubFunction_WithPermaUrl_GoesToExecutingSubFunction ()
    {
      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions();
      IExecutionState executionState = CreateExecutionState(permaUrlOptions);

      ExecutionStateContextMock
          .Setup(mock => mock.SetExecutionState(It.IsNotNull<RedirectingToSubFunctionState>()))
          .Callback(
              (IExecutionState executionState) =>
              {
                var nextState = CheckExecutionState((RedirectingToSubFunctionState)executionState);
                Assert.That(
                    nextState.Parameters.DestinationUrl,
                    Is.EqualTo("/AppDir/sub.wxe?Parameter1=OtherValue&WxeFunctionToken=" + WxeContext.FunctionToken));
                Assert.That(nextState.Parameters.ResumeUrl, Is.EqualTo("/AppDir/root.wxe?WxeFunctionToken=" + WxeContext.FunctionToken));
              })
          .Verifiable();

      executionState.ExecuteSubFunction(WxeContext);

      ExecutionStateContextMock.Verify();
    }

    [Test]
    public void ExecuteSubFunction_WithPermaUrl_WithCustumUrlParamters_GoesToExecutingSubFunction ()
    {
      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions(false, new NameValueCollection { { "Key", "Value" } });
      IExecutionState executionState = CreateExecutionState(permaUrlOptions);

      ExecutionStateContextMock
          .Setup(mock => mock.SetExecutionState(It.IsNotNull<RedirectingToSubFunctionState>()))
          .Callback(
              (IExecutionState executionState) =>
              {
                var nextState = CheckExecutionState((RedirectingToSubFunctionState)executionState);
                Assert.That(
                    nextState.Parameters.DestinationUrl,
                    Is.EqualTo("/AppDir/sub.wxe?Key=Value&WxeFunctionToken=" + WxeContext.FunctionToken));
                Assert.That(nextState.Parameters.ResumeUrl, Is.EqualTo("/AppDir/root.wxe?WxeFunctionToken=" + WxeContext.FunctionToken));
              })
          .Verifiable();

      executionState.ExecuteSubFunction(WxeContext);

      ExecutionStateContextMock.Verify();
    }

    [Test]
    public void ExecuteSubFunction_WithPermaUrl_WithParentPermaUrl_GoesToExecutingSubFunction ()
    {
      WxeContext.QueryString.Add("Key", "Value");

      WxePermaUrlOptions permaUrlOptions = new WxePermaUrlOptions(true);
      IExecutionState executionState = CreateExecutionState(permaUrlOptions);

      ExecutionStateContextMock
          .Setup(mock => mock.SetExecutionState(It.IsNotNull<RedirectingToSubFunctionState>()))
          .Callback(
              (IExecutionState executionState) =>
              {
                var nextState = CheckExecutionState((RedirectingToSubFunctionState)executionState);

                string destinationUrl = UrlUtility.AddParameters(
                    "/AppDir/sub.wxe",
                    new NameValueCollection
                    {
                        { "Parameter1", "OtherValue" },
                        { WxeHandler.Parameters.WxeFunctionToken, WxeContext.FunctionToken },
                        { WxeHandler.Parameters.ReturnUrl, "/AppDir/root.wxe?Key=Value" }
                    },
                    Encoding.UTF8);
                Assert.That(nextState.Parameters.DestinationUrl, Is.EqualTo(destinationUrl));

                Assert.That(nextState.Parameters.ResumeUrl, Is.EqualTo("/AppDir/root.wxe?Key=Value&WxeFunctionToken=" + WxeContext.FunctionToken));
              })
          .Verifiable();

      executionState.ExecuteSubFunction(WxeContext);

      ExecutionStateContextMock.Verify();
    }

    [Test]
    public void Initialize_WithoutPermaUrl ()
    {
      Assert.That(
          () => CreateExecutionState(WxePermaUrlOptions.Null),
          Throws.ArgumentException
              .With.ArgumentExceptionMessageEqualTo(
                  "The 'PreparingRedirectToSubFunctionState' type only supports WxePermaUrlOptions with the UsePermaUrl-flag set to true.",
                  "parameters"));
    }

    private PreparingRedirectToSubFunctionState CreateExecutionState (WxePermaUrlOptions permaUrlOptions)
    {
      return new PreparingRedirectToSubFunctionState(
          ExecutionStateContextMock.Object, new PreparingRedirectToSubFunctionStateParameters(SubFunction.Object, PostBackCollection, permaUrlOptions));
    }
  }
}
