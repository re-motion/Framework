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
using System.Web;
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation.Rendering;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.Web.UnitTests.Core.UI.Controls.DatePickerButtonImplementation.Rendering
{
  [TestFixture]
  public class DatePickerButtonRendererTest : RendererTestBase
  {
    private Mock<IDatePickerButton> _datePickerButton;
    private Mock<HttpContextBase> _httpContext;
    private HtmlHelper _htmlHelper;
    private CultureScope _cultureScope;

    [SetUp]
    public void SetUp ()
    {
      _htmlHelper = new HtmlHelper();
      _httpContext = new Mock<HttpContextBase>();

      _datePickerButton = new Mock<IDatePickerButton>();
      _datePickerButton.SetupProperty(_ => _.ID);
      _datePickerButton.Object.ID = "_Boc_DatePickerButton";
      _datePickerButton.Setup(mock => mock.ContainerControlID).Returns("Container");
      _datePickerButton.Setup(mock => mock.TargetControlID).Returns("Target");
      _datePickerButton.Setup(mock => mock.ClientID).Returns(_datePickerButton.Object.ID);

      _cultureScope = new CultureScope("de-DE", "de-CH");
    }

    [TearDown]
    public void TearDown ()
    {
      _cultureScope.Dispose();
    }

    [Test]
    public void RenderButton ()
    {
      _datePickerButton.Setup(mock => mock.Enabled).Returns(true);
      _datePickerButton.Setup(mock => mock.EnableClientScript).Returns(true);

      AssertDateTimePickerButton(false, true);
    }

    [Test]
    public void RenderButtonNoClientScript ()
    {
      _datePickerButton.Setup(mock => mock.Enabled).Returns(true);

      AssertDateTimePickerButton(false, false);
    }

    [Test]
    public void RenderButtonDisabled ()
    {
      _datePickerButton.Setup(mock => mock.EnableClientScript).Returns(true);

      AssertDateTimePickerButton(true, true);
    }

    [Test]
    public void RenderButtonDisabledNoClientScript ()
    {
      AssertDateTimePickerButton(true, false);
    }

    private void AssertDateTimePickerButton (bool isDisabled, bool hasClientScript)
    {
      var renderer = new DatePickerButtonRenderer(new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.Default, new FakeFallbackNavigationUrlProvider());
      renderer.Render(new DatePickerButtonRenderingContext(_httpContext.Object, _htmlHelper.Writer, _datePickerButton.Object));
      var buttonDocument = _htmlHelper.GetResultDocument();

      var button = _htmlHelper.GetAssertedChildElement(buttonDocument, "a", 0);
      _htmlHelper.AssertAttribute(button, "id", "_Boc_DatePickerButton");
      string script = string.Format(
          "DatePicker.ShowDatePicker(this, document.getElementById ('{0}'), " +
          "document.getElementById ('{1}'), '{2}', '{3}', '{4}');return false;",
          _datePickerButton.Object.ContainerControlID,
          _datePickerButton.Object.TargetControlID,
          "/fake/Remotion.Web/Themes/Fake/UI/DatePickerForm.aspx?Culture=de-DE&UICulture=de-CH",
          "14em",
          "16em"
          );

      if (isDisabled)
      {
        script = "return false;";
        _htmlHelper.AssertAttribute(button, "disabled", "disabled");
      }

      _htmlHelper.AssertAttribute(button, "onclick", script);
      _htmlHelper.AssertAttribute(button, "href", "fakeFallbackUrl");
      _htmlHelper.AssertAttribute(button, "tabindex", "-1");

      if (hasClientScript)
      {
        var image = _htmlHelper.GetAssertedChildElement(button, "img", 0);
        _htmlHelper.AssertAttribute(image, "alt", _datePickerButton.Object.AlternateText);
        _htmlHelper.AssertAttribute(image, "src", renderer.GetResolvedImageUrl().GetUrl());
      }
    }
  }
}
