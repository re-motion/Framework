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
using System.Collections;
using System.Web;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Web.UI;
using Remotion.Web.UI.SmartPageImplementation;
using Remotion.Web.Utilities;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.SmartPageImplementation
{
  [TestFixture]
  public class SmartPageAsyncPostBackErrorHandlerTest
  {
    [Test]
    public void HandleError_IsCustomErrorEnabledFalse_SetsAspNetDetailedErrorMessage ()
    {
      var contextStub = MockRepository.GenerateStub<HttpContextBase>();
      contextStub.Stub (_ => _.Items).Return (new Hashtable());
      contextStub.Stub (_ => _.IsCustomErrorEnabled).Return (false);

      var handler = new SmartPageAsyncPostBackErrorHandler (contextStub);

      Assert.That (() => handler.HandleError (new ApplicationException ("The error")), Throws.TypeOf<AsyncUnhandledException>());

      var message = contextStub.Items[ControlHelper.AsyncPostBackErrorMessageKey];
      Assert.That (message, Is.StringStarting (@"

            <span><H1>"));
      Assert.That (message, Is.StringContaining ("[ApplicationException: The error]"));
      Assert.That (message, Is.StringEnding (@"<br>

    "));
    }

    [Test]
    public void HandleError_IsCustomErrorEnabledTrue_SetsNonSensitiveErrorMessage ()
    {
      var contextStub = MockRepository.GenerateStub<HttpContextBase>();
      contextStub.Stub (_ => _.Items).Return (new Hashtable());
      contextStub.Stub (_ => _.IsCustomErrorEnabled).Return (true);

      var requstStub = MockRepository.GenerateStub<HttpRequestBase>();
      requstStub.Stub (_ => _.ApplicationPath).Return ("Application/Path");
      contextStub.Stub (_ => _.Request).Return (requstStub);

      var handler = new SmartPageAsyncPostBackErrorHandler (contextStub);
      var exceptionMessage = "The error";

      Assert.That (() => handler.HandleError (new ApplicationException (exceptionMessage)), Throws.TypeOf<AsyncUnhandledException>());

      var message = contextStub.Items[ControlHelper.AsyncPostBackErrorMessageKey];
      Assert.That (message, Is.StringStarting (@"

            <span><h1>"));
      Assert.That (message, Is.Not.StringContaining (exceptionMessage));
      Assert.That (message, Is.StringContaining ("Application/Path"));
      Assert.That (message, Is.StringEnding (@"<br/>

    "));
    }

    [Test]
    public void HandleError_SetsAsyncErrorInformation ()
    {
      var contextStub = MockRepository.GenerateStub<HttpContextBase>();
      contextStub.Stub (_ => _.Items).Return (new Hashtable());

      var handler = new SmartPageAsyncPostBackErrorHandler (contextStub);

      Assert.That (() => handler.HandleError (new ApplicationException ("The error")), Throws.TypeOf<AsyncUnhandledException>());

      Assert.That (contextStub.Items[ControlHelper.AsyncPostBackErrorKey], Is.True);
      Assert.That (contextStub.Items[ControlHelper.AsyncPostBackErrorHttpCodeKey], Is.EqualTo (500));
      Assert.That (contextStub.Items[ControlHelper.AsyncPostBackErrorMessageKey], Is.Not.Empty);
    }

    [Test]
    public void HandleError_IntegrationTest ()
    {
      var expectedException = new ApplicationException ("Test exception");

      var page = new FakePageForAsyncPostBack();
      page.Load += delegate { throw expectedException; };

      page.ScriptManager.AsyncPostBackError += (sender, e) => { ((ScriptManager) sender).AsyncPostBackErrorMessage = e.Exception.Message; };

      Assert.That (() => page.ProcessRequest (), Throws.TypeOf<HttpUnhandledException>().With.InnerException.SameAs (expectedException));
      Assert.That (page.Context.Items[ControlHelper.AsyncPostBackErrorKey], Is.True);
      Assert.That (page.Context.Items[ControlHelper.AsyncPostBackErrorHttpCodeKey], Is.EqualTo (500));
      Assert.That (page.Context.Items[ControlHelper.AsyncPostBackErrorMessageKey], Is.EqualTo (expectedException.Message));
    }
  }
}