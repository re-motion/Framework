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
using NUnit.Framework;
using Remotion.Web.Development.WebTesting.PageObjects;

namespace Remotion.Web.Development.WebTesting.IntegrationTests
{
  [TestFixture]
  public class ErrorPageDetectionTest : IntegrationTest
  {
    [Test]
    public void ThrowOnErrorPage_WithNoErrorPage_DoesNotThrow ()
    {
      var htmlPageObject = Start<HtmlPageObject>("Empty.aspx");

      Assert.That(() => AspNetErrorPageDetection.ThrowOnErrorPage(htmlPageObject), Throws.Nothing);
    }

    [Test]
    public void ThrowOnErrorPage_WithErrorPage_Throws ()
    {
      var htmlPageObject = Start<HtmlPageObject>("Error.aspx");

      var exception = Assert.Throws<AspNetErrorPageException>(() => AspNetErrorPageDetection.ThrowOnErrorPage(htmlPageObject));
      Assert.That(exception.Message, Is.EqualTo("This page always produces an error."));
      Assert.That(exception.Details, Contains.Substring("Description: An unhandled exception occurred during the execution of the current web request. Please review the stack trace for more information about the error and where it originated in the code."));
    }
  }
}
