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
// using NUnit.Framework;
//
using System;
using System.Web.UI;
using System.Xml;
using JetBrains.Annotations;
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{
  [TestFixture]
  public class LazyEvaluatedValidationMessageControlTest
  {
    private HtmlHelper _html;

    [SetUp]
    public void Setup ()
    {
      _html = new HtmlHelper();
    }

    private class TestableOpenLazyEvaluatedValidationMessageControl : LazyEvaluatedValidationMessageControl
    {
      public TestableOpenLazyEvaluatedValidationMessageControl (string tag, [CanBeNull] IValidator validatorStub)
          : base(tag, validatorStub)
      {
      }

      public new void Render (HtmlTextWriter writer)
      {
        base.Render(writer);
      }
    }

    [Test]
    public void Render_RendersLazilyGeneratedMessage ()
    {
      var errorMessage = "Unexpected error message";

      var validatorStub = new Mock<IValidator>();
      validatorStub.Setup(_ => _.IsValid).Returns(false);
      validatorStub.Setup(_ => _.ErrorMessage).Returns(() => errorMessage);

      var lazyValidationControl = new TestableOpenLazyEvaluatedValidationMessageControl("span", validatorStub.Object);

      errorMessage = "A lazy error.";

      lazyValidationControl.Render(_html.Writer);

      var document = _html.GetResultDocument();

      var span = _html.GetAssertedChildElement(document, "span", 0);
      Assert.That(span.InnerText, Is.EqualTo("A lazy error."));
    }
  }
}
