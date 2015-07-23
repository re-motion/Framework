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
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation.Rendering;
using Remotion.Web.UI.Controls.Rendering;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Controls.DatePickerButtonImplementation.Rendering
{
  [TestFixture]
  public class DatePickerButtonRendererTest : RendererTestBase
  {
    private IDatePickerButton _datePickerButton;
    private HttpContextBase _httpContext;
    private HtmlHelper _htmlHelper;

    [SetUp]
    public void SetUp ()
    {
      _htmlHelper = new HtmlHelper ();
      _httpContext = MockRepository.GenerateStub<HttpContextBase> ();

      _datePickerButton = MockRepository.GenerateStub<IDatePickerButton>();
      _datePickerButton.ID = "_Boc_DatePickerButton";
      _datePickerButton.Stub (mock => mock.ContainerControlID).Return ("Container");
      _datePickerButton.Stub (mock => mock.TargetControlID).Return ("Target");
      _datePickerButton.Stub (mock => mock.ClientID).Return (_datePickerButton.ID);
    }

    [Test]
    public void RenderButton ()
    {
      _datePickerButton.Stub (mock => mock.Enabled).Return (true);
      _datePickerButton.Stub (mock => mock.EnableClientScript).Return (true);

      AssertDateTimePickerButton (false, true);
    }

    [Test]
    public void RenderButtonNoClientScript ()
    {
      _datePickerButton.Stub (mock => mock.Enabled).Return (true);

      AssertDateTimePickerButton (false, false);
    }

    [Test]
    public void RenderButtonDisabled ()
    {
      _datePickerButton.Stub (mock => mock.EnableClientScript).Return (true);

      AssertDateTimePickerButton (true, true);
    }

    [Test]
    public void RenderButtonDisabledNoClientScript ()
    {
      AssertDateTimePickerButton (true, false);
    }

    private void AssertDateTimePickerButton (bool isDisabled, bool hasClientScript)
    {
      var renderer = new DatePickerButtonRenderer (new FakeResourceUrlFactory(), GlobalizationService, RenderingFeatures.Default);
      renderer.Render (new DatePickerButtonRenderingContext (_httpContext, _htmlHelper.Writer, _datePickerButton));
      var buttonDocument = _htmlHelper.GetResultDocument();

      var button = _htmlHelper.GetAssertedChildElement (buttonDocument, "a", 0);
      _htmlHelper.AssertAttribute (button, "id", "_Boc_DatePickerButton");
      string script = string.Format (
          "DatePicker_ShowDatePicker(this, document.getElementById ('{0}'), " +
          "document.getElementById ('{1}'), '{2}', '{3}', '{4}');return false;",
          _datePickerButton.ContainerControlID,
          _datePickerButton.TargetControlID,
          "/fake/Remotion.Web/Themes/Fake/UI/DatePickerForm.aspx",
          "14em",
          "16em"
          );

      if (isDisabled)
      {
        script = "return false;";
        _htmlHelper.AssertAttribute (button, "disabled", "disabled");
      }
      if (hasClientScript)
      {
        _htmlHelper.AssertAttribute (button, "onclick", script);
        _htmlHelper.AssertAttribute (button, "href", "#");
      }

      if (hasClientScript)
      {
        var image = _htmlHelper.GetAssertedChildElement (button, "img", 0);
        _htmlHelper.AssertAttribute (image, "alt", _datePickerButton.AlternateText);
        _htmlHelper.AssertAttribute (image, "src", renderer.GetResolvedImageUrl().GetUrl());
      }
    }
  }
}