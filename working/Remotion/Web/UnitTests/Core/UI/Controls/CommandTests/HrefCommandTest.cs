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
using System.Web.UI;
using System.Web.UI.WebControls;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Core.UI.Controls.CommandTests
{
  [TestFixture]
  public class HrefCommandTest : BaseTest
  {
    private CommandTestHelper _testHelper;

    [SetUp]
    public virtual void SetUp ()
    {
      _testHelper = new CommandTestHelper ();
      HttpContextHelper.SetCurrent (_testHelper.HttpContext);
    }

    [Test]
    public void HasAccess_WithoutSeucrityProvider ()
    {
      Command command = _testHelper.CreateHrefCommand ();
      _testHelper.ReplayAll ();

      bool hasAccess = command.HasAccess (null);

      _testHelper.VerifyAll ();
      Assert.That (hasAccess, Is.True);
    }

    [Test]
    public void Render_WithAccessGranted ()
    {
      Command command = _testHelper.CreateHrefCommandAsPartialMock ();
      string[] parameters = new string[] { "Value1", "Value2" };

      NameValueCollection additionalUrlParameters = new NameValueCollection ();
      additionalUrlParameters.Add ("Parameter3", "Value3");

      string expectedHref = command.HrefCommand.FormatHref (parameters);
      expectedHref = UrlUtility.AddParameter (expectedHref, additionalUrlParameters.GetKey (0), additionalUrlParameters.Get (0));
      string expectedOnClick = _testHelper.OnClick;

      _testHelper.ExpectOnceOnHasAccess (command, true);
      _testHelper.ReplayAll ();

      command.RenderBegin (
          _testHelper.HtmlWriter, 
          RenderingFeatures.Default,
          _testHelper.PostBackEvent, 
          parameters, 
          _testHelper.OnClick, 
          _testHelper.SecurableObject, 
          additionalUrlParameters, 
          true, 
          new Style ());

      _testHelper.VerifyAll ();

      Assert.IsNotNull (_testHelper.HtmlWriter.Tag, "Missing Tag");
      Assert.AreEqual (HtmlTextWriterTag.A, _testHelper.HtmlWriter.Tag, "Wrong Tag");

      Assert.IsNotNull (_testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Href], "Missing Href");
      Assert.AreEqual (expectedHref, _testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Href], "Wrong Href");

      Assert.IsNotNull (_testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Onclick], "Missing OnClick");
      Assert.AreEqual (expectedOnClick, _testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Onclick], "Wrong OnClick");

      Assert.IsNotNull (_testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Title], "Missing Title");
      Assert.AreEqual (_testHelper.ToolTip, _testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Title], "Wrong Title");

      Assert.IsNotNull (_testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Target], "Missing Target");
      Assert.AreEqual (_testHelper.Target, _testHelper.HtmlWriter.Attributes[HtmlTextWriterAttribute.Target], "Wrong Target");
    }

    [Test]
    public void Render_WithAccessDenied ()
    {
      Command command = _testHelper.CreateHrefCommandAsPartialMock ();
      string[] parameters = new string[] { "Value1", "Value2" };
      NameValueCollection additionalUrlParameters = new NameValueCollection ();
      additionalUrlParameters.Add ("Parameter3", "Value3");
      _testHelper.ExpectOnceOnHasAccess (command, false);
      _testHelper.ReplayAll ();

      command.RenderBegin (
          _testHelper.HtmlWriter, 
          RenderingFeatures.Default,
          _testHelper.PostBackEvent, 
          parameters, 
          _testHelper.OnClick, 
          _testHelper.SecurableObject, 
          additionalUrlParameters, 
          true, 
          new Style ());

      _testHelper.VerifyAll ();
      Assert.IsNotNull (_testHelper.HtmlWriter.Tag, "Missing Tag");
      Assert.AreEqual (HtmlTextWriterTag.A, _testHelper.HtmlWriter.Tag, "Wrong Tag");
      Assert.AreEqual (0, _testHelper.HtmlWriter.Attributes.Count, "Has Attributes");
    }
  }
}
