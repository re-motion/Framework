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
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.Contracts.DiagnosticMetadata;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation;
using Remotion.Web.UI.Controls.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocDateTimeValueImplementation.Rendering
{
  [TestFixture]
  [SetCulture("")]
  public class BocDateTimeValueRendererTest : RendererTestBase
  {
    private const string c_timeString = "17:45";
    private const string c_dateString = "12.07.2013";
    private const string c_formatedDateString = "2013-07-12";
    private const string c_formatedTimeString = "17:45:00";
    private const string c_dateValueName = "MyDateTimeValue_DateValue";
    private const string c_timeValueName = "MyDateTimeValue_TimeValue";
    private const string c_dateValueID = "MyDateTimeValue";
    private IBocDateTimeValue _control;
    private SingleRowTextBoxStyle _dateStyle;
    private SingleRowTextBoxStyle _timeStyle;
    private StubTextBox _dateTextBox;
    private StubTextBox _timeTextBox;
    private BocDateTimeValueRenderingContext _renderingContext;
    private DateTime _dateValue = new DateTime(2013, 07, 12);
    private DateTime _dateTimeValue = new DateTime (2013, 07, 12, 17, 45, 0);


    [SetUp]
    public void SetUp ()
    {
      Initialize();
      _control = MockRepository.GenerateStub<IBocDateTimeValue>();
      _control.Stub (stub => stub.ClientID).Return (c_dateValueID);
      _control.Stub (stub => stub.ControlType).Return ("BocDateTimeValue");
      _control.Stub (stub => stub.GetDateValueName()).Return (c_dateValueName);
      _control.Stub (stub => stub.GetTimeValueName()).Return (c_timeValueName);

      _dateStyle = new SingleRowTextBoxStyle();
      _timeStyle = new SingleRowTextBoxStyle();
      _control.Stub (stub => stub.DateTextBoxStyle).Return (_dateStyle);
      _control.Stub (stub => stub.TimeTextBoxStyle).Return (_timeStyle);

      var pageStub = MockRepository.GenerateStub<IPage>();
      pageStub.Stub (stub => stub.WrappedInstance).Return (new PageMock());
      pageStub.Stub (stub => stub.ClientScript).Return (MockRepository.GenerateStub<IClientScriptManager>());
      _control.Stub (stub => stub.Page).Return (pageStub);

      var datePickerButton = MockRepository.GenerateStub<IDatePickerButton> ();
      datePickerButton.Stub (stub => stub.EnableClientScript).Return (true);
      datePickerButton.Stub (stub => stub.RenderControl (Html.Writer)).WhenCalled (invocation => Html.Writer.WriteLine ("DatePicker"));
      _control.Stub (stub => stub.DatePickerButton).Return (datePickerButton);

      StateBag stateBag = new StateBag ();
      _control.Stub (stub => stub.Attributes).Return (new AttributeCollection (stateBag));
      _control.Stub (stub => stub.Style).Return (_control.Attributes.CssStyle);
      _control.Stub (stub => stub.DateTextBoxStyle).Return (new TextBoxStyle ());
      _control.Stub (stub => stub.TimeTextBoxStyle).Return (new TextBoxStyle ());
      _control.Stub (stub => stub.DateTimeTextBoxStyle).Return (new TextBoxStyle ());
      _control.Stub (stub => stub.ControlStyle).Return (new Style (stateBag));

      _control.Stub (stub => stub.ProvideMaxLength).Return (true);

      var dateTimeFormatter = new DateTimeFormatter();
      _control.Stub (stub => stub.DateTimeFormatter).Return (dateTimeFormatter);

      _dateTextBox = new StubTextBox();
      _timeTextBox = new StubTextBox();

      _renderingContext = new BocDateTimeValueRenderingContext (HttpContext, Html.Writer, _control);
    }

    [Test]
    public void RenderDateValue ()
    {
      _control.Stub (stub => stub.DateString).Return (c_dateString);
      _control.Stub (stub => stub.ActualValueType).Return (BocDateTimeValueType.Date);
      _control.Stub (stub => stub.Enabled).Return (true);

      BocDateTimeValueRenderer renderer;
      XmlNode container = GetAssertedContainer (out renderer, true);

      AssertDate (container, renderer);

      container.AssertTextNode ("DatePicker", 1);
    }

    [Test]
    public void RenderDateValue_ReadOnlyMode ()
    {
      _control.Stub (stub => stub.DateString).Return (c_dateString);
      _control.Stub (stub => stub.ActualValueType).Return (BocDateTimeValueType.Date);
      _control.Stub (stub => stub.Value).Return (_dateValue);
      _control.Stub (stub => stub.Enabled).Return (true);
      _control.Stub (stub => stub.IsReadOnly).Return (true);
      
      BocDateTimeValueRenderer renderer;
      XmlNode container = GetAssertedContainer (out renderer, true);

      AssertDate (container, renderer);
    }

    [Test]
    public void RenderDateTimeValue ()
    {
      _control.Stub (stub => stub.DateString).Return (c_dateString);
      _control.Stub (stub => stub.TimeString).Return (c_timeString);
      _control.Stub (stub => stub.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _control.Stub (stub => stub.Enabled).Return (true);

      BocDateTimeValueRenderer renderer;
      XmlNode container = GetAssertedContainer (out renderer, false);

      AssertDate (container, renderer);
      AssertTime (container, renderer);

      container.AssertTextNode ("DatePicker", 1);
    }

    [Test]
    public void RenderDateTimeValue_ReadOnlyMode ()
    {
      _control.Stub (stub => stub.DateString).Return (c_dateString);
      _control.Stub (stub => stub.TimeString).Return (c_timeString);
      _control.Stub (stub => stub.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _control.Stub (stub => stub.Value).Return (_dateTimeValue);
      _control.Stub (stub => stub.Enabled).Return (true);
      _control.Stub (stub => stub.IsReadOnly).Return (true);
      
      BocDateTimeValueRenderer renderer;
      XmlNode container = GetAssertedContainer (out renderer, false);

      AssertTime (container, renderer);
    }

    [Test]
    public void RenderIDs ()
    {
      _control.Stub (stub => stub.DateString).Return (c_dateString);
      _control.Stub (stub => stub.TimeString).Return (c_timeString);
      _control.Stub (stub => stub.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _control.Stub (stub => stub.Enabled).Return (true);

      var renderer = new BocDateTimeValueRenderer (new FakeResourceUrlFactory (), GlobalizationService, RenderingFeatures.Default);
      renderer.Render (new BocDateTimeValueRenderingContext (HttpContext, Html.Writer, _control));
      var document = Html.GetResultDocument ();
      var container = document.GetAssertedChildElement ("span", 0);
      var dateInput = container.GetAssertedChildElement("span", 0).GetAssertedChildElement("input", 0);
      var timeInput = container.GetAssertedChildElement ("span", 2).GetAssertedChildElement ("input", 0);
      dateInput.AssertAttributeValueEquals ("id", c_dateValueName);
      dateInput.AssertAttributeValueEquals ("name", c_dateValueName);
      timeInput.AssertAttributeValueEquals ("id", c_timeValueName);
      timeInput.AssertAttributeValueEquals ("name", c_timeValueName);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      _control.Stub (stub => stub.DateString).Return (c_dateString);
      _control.Stub (stub => stub.TimeString).Return (c_timeString);
      _control.Stub (stub => stub.ShowSeconds).Return (true);
      _control.Stub (stub => stub.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateStyle.AutoPostBack = true;
      _timeStyle.AutoPostBack = false;

      var renderer = new BocDateTimeValueRenderer (new FakeResourceUrlFactory (), GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);
      renderer.Render (new BocDateTimeValueRenderingContext (HttpContext, Html.Writer, _control));
      
      var document = Html.GetResultDocument();
      var container = document.GetAssertedChildElement ("span", 0);
      Html.AssertAttribute (container, DiagnosticMetadataAttributes.ControlType, "BocDateTimeValue");
      Html.AssertAttribute (container, DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueHasTimeField, "true");

      var dateInput = container.GetAssertedChildElement("span", 0).GetAssertedChildElement("input", 0);
      Html.AssertAttribute (dateInput, DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueDateField, "true");
      Html.AssertAttribute (dateInput, DiagnosticMetadataAttributes.TriggersPostBack, "true");

      var timeInput = container.GetAssertedChildElement ("span", 2).GetAssertedChildElement ("input", 0);
      Html.AssertAttribute (timeInput, DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueTimeField, "true");
      Html.AssertAttribute (timeInput, DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueTimeFieldHasSeconds, "true");
      Html.AssertAttribute (timeInput, DiagnosticMetadataAttributes.TriggersPostBack, "false");
    }

    private void AssertTime (XmlNode container, BocDateTimeValueRenderer renderer)
    {
      if (!_control.IsReadOnly)
      {
        var timeInputWrapper = container.GetAssertedChildElement ("span", 2);
        timeInputWrapper.AssertAttributeValueContains ("class", renderer.CssClassTimeInputWrapper);
        timeInputWrapper.AssertAttributeValueContains (
            "class", renderer.GetPositioningCssClass (_renderingContext, BocDateTimeValueRenderer.DateTimeValuePart.Time));
        timeInputWrapper.AssertChildElementCount (0);
        timeInputWrapper.AssertTextNode ("TextBox", 0);

        Assert.That (_timeTextBox.ID, Is.EqualTo (c_timeValueName));
        Assert.That (_timeTextBox.CssClass, Is.EqualTo (renderer.CssClassTime));
        Assert.That (_timeTextBox.Text, Is.EqualTo (c_timeString));
        Assert.That (_timeTextBox.MaxLength, Is.EqualTo (5));
      }
      else
      {
        var dateLabel = container.GetAssertedChildElement ("span", 0);
        dateLabel.AssertAttributeValueEquals ("data-value", c_formatedDateString);
        var timeLabel = container.GetAssertedChildElement ("span", 1);
        timeLabel.AssertAttributeValueEquals ("data-value", c_formatedTimeString);
      }
    }

    private void AssertDate (XmlNode container, BocDateTimeValueRenderer renderer)
    {
      var dateInputWrapper = container.GetAssertedChildElement ("span", 0);
      if (!_control.IsReadOnly)
      {
        dateInputWrapper.AssertAttributeValueContains ("class", renderer.CssClassDateInputWrapper);
        dateInputWrapper.AssertAttributeValueContains (
            "class", renderer.GetPositioningCssClass (_renderingContext, BocDateTimeValueRenderer.DateTimeValuePart.Date));
        dateInputWrapper.AssertChildElementCount (0);

        dateInputWrapper.AssertTextNode ("TextBox", 0);
        Assert.That (_dateTextBox.ID, Is.EqualTo (c_dateValueName));
        Assert.That (_dateTextBox.CssClass, Is.EqualTo (renderer.CssClassDate));
        Assert.That (_dateTextBox.Text, Is.EqualTo (c_dateString));
        Assert.That (_dateTextBox.MaxLength, Is.EqualTo (10));
      }
      else
      {
        dateInputWrapper.AssertAttributeValueEquals ("data-value", _dateValue.ToString ("yyyy-MM-dd"));
      }
    }

    private XmlNode GetAssertedContainer (out BocDateTimeValueRenderer renderer, bool isDateOnly)
    {
      renderer = new TestableBocDateTimeValueRenderer (
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.Default,
          _dateTextBox,
          _timeTextBox);
      renderer.Render (new BocDateTimeValueRenderingContext(HttpContext, Html.Writer, _control));

      var document = Html.GetResultDocument ();
      var container = document.GetAssertedChildElement ("span", 0);
      container.AssertAttributeValueEquals ("id", c_dateValueID);
      container.AssertAttributeValueContains ("class", isDateOnly ? renderer.CssClassDateOnly : renderer.CssClassDateTime);
      container.AssertChildElementCount (isDateOnly ? 1 : 2);
      return container;
    }
  }
}