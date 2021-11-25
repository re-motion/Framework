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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Moq;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.Mixins.Validation;
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
    private const string c_labelID = "Label";
    private const string c_dateValidationErrors = "DateValidationError";
    private const string c_timeValidationErrors = "TimeValidationError";

    private Mock<IBocDateTimeValue> _control;
    private SingleRowTextBoxStyle _dateStyle;
    private SingleRowTextBoxStyle _timeStyle;
    private StubTextBox _dateTextBox;
    private StubTextBox _timeTextBox;
    private BocDateTimeValueRenderingContext _renderingContext;
    private DateTime _dateValue = new DateTime(2013, 07, 12);
    private DateTime _dateTimeValue = new DateTime(2013, 07, 12, 17, 45, 0);
    private IResourceManager _resourceManager;


    [SetUp]
    public void SetUp ()
    {
      Initialize();
      _control = new Mock<IBocDateTimeValue>();
      _control.SetupProperty(_ => _.CssClass);
      _control.Setup(stub => stub.ClientID).Returns(c_dateValueID);
      _control.Setup(stub => stub.ControlType).Returns("BocDateTimeValue");
      _control.Setup(stub => stub.GetDateValueName()).Returns(c_dateValueName);
      _control.Setup(stub => stub.GetTimeValueName()).Returns(c_timeValueName);
      _control.Setup(mock => mock.GetLabelIDs()).Returns(EnumerableUtility.Singleton(c_labelID));
      _control.Setup(mock => mock.GetDateValueValidationErrors()).Returns(EnumerableUtility.Singleton(c_dateValidationErrors));
      _control.Setup(mock => mock.GetTimeValueValidationErrors()).Returns(EnumerableUtility.Singleton(c_timeValidationErrors));

      _dateStyle = new SingleRowTextBoxStyle();
      _timeStyle = new SingleRowTextBoxStyle();
      _control.Setup(stub => stub.DateTextBoxStyle).Returns(_dateStyle);
      _control.Setup(stub => stub.TimeTextBoxStyle).Returns(_timeStyle);

      var pageStub = new Mock<IPage>();
      pageStub.Setup(stub => stub.WrappedInstance).Returns(new PageMock());
      pageStub.Setup(stub => stub.ClientScript).Returns(new Mock<IClientScriptManager>().Object);
      _control.Setup(stub => stub.Page).Returns(pageStub.Object);

      var datePickerButton = new Mock<IDatePickerButton>();
      datePickerButton.Setup(stub => stub.EnableClientScript).Returns(true);
      datePickerButton.Setup(stub => stub.RenderControl(Html.Writer)).Callback((HtmlTextWriter writer) => Html.Writer.WriteLine("DatePicker"));
      _control.Setup(stub => stub.DatePickerButton).Returns(datePickerButton.Object);

      StateBag stateBag = new StateBag();
      _control.Setup(stub => stub.Attributes).Returns(new AttributeCollection(stateBag));
      _control.Setup(stub => stub.Style).Returns(_control.Object.Attributes.CssStyle);
      _control.Setup(stub => stub.DateTimeTextBoxStyle).Returns(new TextBoxStyle());
      _control.Setup(stub => stub.ControlStyle).Returns(new Style(stateBag));

      _control.Setup(stub => stub.ProvideMaxLength).Returns(true);

      var dateTimeFormatter = new DateTimeFormatter();
      _control.Setup(stub => stub.DateTimeFormatter).Returns(dateTimeFormatter);

      _resourceManager = GlobalizationService.GetResourceManager(typeof(BocDateTimeValue.ResourceIdentifier));
      _control.Setup(list => list.GetResourceManager()).Returns(_resourceManager);

      _dateTextBox = new StubTextBox();
      _timeTextBox = new StubTextBox();

      _renderingContext = new BocDateTimeValueRenderingContext(HttpContext, Html.Writer, _control.Object);
    }

    [Test]
    public void RenderDateValue ()
    {
      _control.Setup(stub => stub.DateString).Returns(c_dateString);
      _control.Setup(stub => stub.ActualValueType).Returns(BocDateTimeValueType.Date);
      _control.Setup(stub => stub.Enabled).Returns(true);

      BocDateTimeValueRenderer renderer;
      XmlNode container = GetAssertedContainer(out renderer, isDateOnly: true,  isReadOnly: false);

      AssertDate(container, renderer, isDateOnly: true);

      container.AssertTextNode("DatePicker", 1);
    }

    [Test]
    public void RenderDateValue_ReadOnlyMode ()
    {
      _control.Setup(stub => stub.DateString).Returns(c_dateString);
      _control.Setup(stub => stub.ActualValueType).Returns(BocDateTimeValueType.Date);
      _control.Setup(stub => stub.Value).Returns(_dateValue);
      _control.Setup(stub => stub.Enabled).Returns(true);
      _control.Setup(stub => stub.IsReadOnly).Returns(true);
      
      BocDateTimeValueRenderer renderer;
      XmlNode container = GetAssertedContainer(out renderer, isDateOnly: true, isReadOnly: true);

      AssertDate(container, renderer, isDateOnly: true);
    }

    [Test]
    public void RenderDateTimeValue ()
    {
      _control.Setup(stub => stub.DateString).Returns(c_dateString);
      _control.Setup(stub => stub.TimeString).Returns(c_timeString);
      _control.Setup(stub => stub.ActualValueType).Returns(BocDateTimeValueType.DateTime);
      _control.Setup(stub => stub.Enabled).Returns(true);

      BocDateTimeValueRenderer renderer;
      XmlNode container = GetAssertedContainer(out renderer, isDateOnly: false, isReadOnly: false);

      AssertDate(container, renderer, isDateOnly: false);
      AssertTime(container, renderer);

      container.AssertTextNode("DatePicker", 1);
    }

    [Test]
    public void RenderDateTimeValue_ReadOnlyMode ()
    {
      _control.Setup(stub => stub.DateString).Returns(c_dateString);
      _control.Setup(stub => stub.TimeString).Returns(c_timeString);
      _control.Setup(stub => stub.ActualValueType).Returns(BocDateTimeValueType.DateTime);
      _control.Setup(stub => stub.Value).Returns(_dateTimeValue);
      _control.Setup(stub => stub.Enabled).Returns(true);
      _control.Setup(stub => stub.IsReadOnly).Returns(true);
      
      BocDateTimeValueRenderer renderer;
      XmlNode container = GetAssertedContainer(out renderer, isDateOnly: false, isReadOnly: true);

      AssertTime(container, renderer);
    }

    [Test]
    public void RenderIDs ()
    {
      _control.Setup(stub => stub.DateString).Returns(c_dateString);
      _control.Setup(stub => stub.TimeString).Returns(c_timeString);
      _control.Setup(stub => stub.ActualValueType).Returns(BocDateTimeValueType.DateTime);
      _control.Setup(stub => stub.Enabled).Returns(true);

      var renderer = new BocDateTimeValueRenderer(
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.Default,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      renderer.Render(new BocDateTimeValueRenderingContext(HttpContext, Html.Writer, _control.Object));
      var document = Html.GetResultDocument();
      var container = document.GetAssertedChildElement("span", 0);
      var dateInput = container.GetAssertedChildElement("span", 0).GetAssertedChildElement("input", 0);
      var dateLabel = container.GetAssertedChildElement("span", 0).GetAssertedChildElement("span", 1);
      var timeInput = container.GetAssertedChildElement("span", 2).GetAssertedChildElement("input", 0);
      var timeLabel = container.GetAssertedChildElement("span", 2).GetAssertedChildElement("span", 1);
      dateInput.AssertAttributeValueEquals("id", c_dateValueName);
      dateInput.AssertAttributeValueEquals("name", c_dateValueName);
      dateLabel.AssertAttributeValueEquals("id", c_dateValueID + "_DateLabel");
      timeInput.AssertAttributeValueEquals("id", c_timeValueName);
      timeInput.AssertAttributeValueEquals("name", c_timeValueName);
      timeLabel.AssertAttributeValueEquals("id",  c_dateValueID + "_TimeLabel");
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      _control.Setup(stub => stub.DateString).Returns(c_dateString);
      _control.Setup(stub => stub.TimeString).Returns(c_timeString);
      _control.Setup(stub => stub.ShowSeconds).Returns(true);
      _control.Setup(stub => stub.ActualValueType).Returns(BocDateTimeValueType.DateTime);
      _dateStyle.AutoPostBack = true;
      _timeStyle.AutoPostBack = false;

      var renderer = new BocDateTimeValueRenderer(
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.WithDiagnosticMetadata,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer());
      renderer.Render(new BocDateTimeValueRenderingContext(HttpContext, Html.Writer, _control.Object));
      
      var document = Html.GetResultDocument();
      var container = document.GetAssertedChildElement("span", 0);
      Html.AssertAttribute(container, DiagnosticMetadataAttributes.ControlType, "BocDateTimeValue");
      Html.AssertAttribute(container, DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueHasTimeField, "true");

      var dateInput = container.GetAssertedChildElement("span", 0).GetAssertedChildElement("input", 0);
      Html.AssertAttribute(dateInput, DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueDateField, "true");
      Html.AssertAttribute(dateInput, DiagnosticMetadataAttributes.TriggersPostBack, "true");

      var timeInput = container.GetAssertedChildElement("span", 2).GetAssertedChildElement("input", 0);
      Html.AssertAttribute(timeInput, DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueTimeField, "true");
      Html.AssertAttribute(timeInput, DiagnosticMetadataAttributesForObjectBinding.BocDateTimeValueTimeFieldHasSeconds, "true");
      Html.AssertAttribute(timeInput, DiagnosticMetadataAttributes.TriggersPostBack, "false");
    }

    private void AssertTime (XmlNode container, BocDateTimeValueRenderer renderer)
    {
      if (!_control.Object.IsReadOnly)
      {
        var timeInputWrapper = container.GetAssertedChildElement("span", 2);
        timeInputWrapper.AssertAttributeValueContains("class", renderer.CssClassTimeInputWrapper);
        timeInputWrapper.AssertAttributeValueContains(
            "class", renderer.GetPositioningCssClass(_renderingContext, BocDateTimeValueRenderer.DateTimeValuePart.Time));
        timeInputWrapper.AssertChildElementCount(3);

        var inputField = timeInputWrapper.GetAssertedChildElement("input", 0);
        inputField.AssertAttributeValueEquals("type", "stub");

        var timeDescriptionLabel = timeInputWrapper.GetAssertedChildElement("span", 1);
        timeDescriptionLabel.AssertAttributeValueEquals("id", c_dateValueID + "_TimeLabel");

        Assert.That(_timeTextBox.ID, Is.EqualTo(c_timeValueName));
        Assert.That(_timeTextBox.CssClass, Is.EqualTo(renderer.CssClassTime));
        Assert.That(_timeTextBox.Text, Is.EqualTo(c_timeString));
        Assert.That(_timeTextBox.MaxLength, Is.EqualTo(5));
        Assert.That(_timeTextBox.Attributes[StubLabelReferenceRenderer.LabelReferenceAttribute], Is.EqualTo(c_labelID));
        Assert.That(_timeTextBox.Attributes[StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute], Is.EqualTo(c_dateValueID + "_TimeLabel"));
        Assert.That(_timeTextBox.Attributes[StubValidationErrorRenderer.ValidationErrorsIDAttribute], Is.EqualTo(c_dateValueID + "_TimeValueValidationErrors"));
        Assert.That(_timeTextBox.Attributes[StubValidationErrorRenderer.ValidationErrorsAttribute], Is.EqualTo(c_timeValidationErrors));

        var validationErrors = container.GetAssertedChildElement("span", 2).GetAssertedChildElement("fake", 2);
        validationErrors.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_dateValueID + "_TimeValueValidationErrors");
        validationErrors.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsAttribute, c_timeValidationErrors);
      }
      else
      {
        container.AssertChildElementCount(6);

        var timeValueLabel = container.GetAssertedChildElement("span", 3);
        timeValueLabel.AssertAttributeValueEquals("id", c_dateValueID + "_TimeValue");
        timeValueLabel.AssertAttributeValueEquals("data-value", c_formatedTimeString);
        timeValueLabel.AssertAttributeValueEquals(StubLabelReferenceRenderer.LabelReferenceAttribute, c_labelID);
        timeValueLabel.AssertAttributeValueEquals(StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, c_dateValueID + "_TimeLabel "+ c_dateValueID + "_TimeValue");
        timeValueLabel.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_dateValueID + "_TimeValueValidationErrors");
        timeValueLabel.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsAttribute, c_timeValidationErrors);

        var timeDescriptionLabel = container.GetAssertedChildElement("span", 4);
        timeDescriptionLabel.AssertAttributeValueEquals("id", c_dateValueID + "_TimeLabel");

        var validationErrors = container.GetAssertedChildElement("fake", 5);
        validationErrors.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_dateValueID + "_TimeValueValidationErrors");
        validationErrors.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsAttribute, c_timeValidationErrors);
      }
    }

    private void AssertDate (XmlNode container, BocDateTimeValueRenderer renderer, bool isDateOnly)
    {
      if (!_control.Object.IsReadOnly)
      {
        var dateInputWrapper = container.GetAssertedChildElement("span", 0);
        dateInputWrapper.AssertAttributeValueContains("class", renderer.CssClassDateInputWrapper);
        dateInputWrapper.AssertAttributeValueContains(
            "class",
            renderer.GetPositioningCssClass(_renderingContext, BocDateTimeValueRenderer.DateTimeValuePart.Date));
        dateInputWrapper.AssertChildElementCount(isDateOnly ? 2 : 3);

        var inputField = dateInputWrapper.GetAssertedChildElement("input", 0);
        inputField.AssertAttributeValueEquals("type", "stub");

        if (!isDateOnly)
        {
          var dateDescriptionLabel = dateInputWrapper.GetAssertedChildElement("span", 1);
          dateDescriptionLabel.AssertAttributeValueEquals("id", c_dateValueID + "_DateLabel");
        }

        Assert.That(_dateTextBox.ID, Is.EqualTo(c_dateValueName));
        Assert.That(_dateTextBox.CssClass, Is.EqualTo(renderer.CssClassDate));
        Assert.That(_dateTextBox.Text, Is.EqualTo(c_dateString));
        Assert.That(_dateTextBox.MaxLength, Is.EqualTo(10));
        Assert.That(_dateTextBox.Attributes[StubLabelReferenceRenderer.LabelReferenceAttribute], Is.EqualTo(c_labelID));
        Assert.That(_dateTextBox.Attributes[StubValidationErrorRenderer.ValidationErrorsIDAttribute], Is.EqualTo(c_dateValueID + "_DateValueValidationErrors"));
        Assert.That(_dateTextBox.Attributes[StubValidationErrorRenderer.ValidationErrorsAttribute], Is.EqualTo(c_dateValidationErrors));
        
        if (isDateOnly)
        {
		  Assert.That(_dateTextBox.Attributes[StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute], Is.EqualTo(""));

          var validationErrors = container.GetAssertedChildElement("span", 0).GetAssertedChildElement("fake", 1);
          validationErrors.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsAttribute, c_dateValidationErrors);
        }
        else
        {
          Assert.That(
              _dateTextBox.Attributes[StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute],
              Is.EqualTo(c_dateValueID + "_DateLabel"));
          var validationErrors = container.GetAssertedChildElement("span", 0).GetAssertedChildElement("fake", 2);
          validationErrors.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_dateValueID + "_DateValueValidationErrors");
          validationErrors.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsAttribute, c_dateValidationErrors);
        }
      }
      else
      {
        container.AssertChildElementCount(isDateOnly ? 2 : 4);
        var dateValueLabel = container.GetAssertedChildElement("span", 0);
        dateValueLabel.AssertAttributeValueEquals("id", c_dateValueID + "_DateValue");
        dateValueLabel.AssertAttributeValueEquals("data-value", _dateValue.ToString("yyyy-MM-dd"));
        dateValueLabel.AssertAttributeValueEquals(StubLabelReferenceRenderer.LabelReferenceAttribute, c_labelID);
        dateValueLabel.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsIDAttribute, c_dateValueID + "_DateValueValidationErrors");
        dateValueLabel.AssertAttributeValueEquals(StubValidationErrorRenderer.ValidationErrorsAttribute, c_dateValidationErrors);

        if (isDateOnly)
          dateValueLabel.AssertAttributeValueEquals(StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, c_dateValueID + "_DateValue");
        else
          dateValueLabel.AssertAttributeValueEquals(StubLabelReferenceRenderer.AccessibilityAnnotationsAttribute, c_dateValueID + "_DateLabel " + c_dateValueID + "_DateValue");

        if (!isDateOnly)
        {
          var dateDescriptionLabel = container.GetAssertedChildElement("span", 1);
          dateDescriptionLabel.AssertAttributeValueEquals("id", c_dateValueID + "_DateLabel");
        }
      }
    }

    private XmlNode GetAssertedContainer (out BocDateTimeValueRenderer renderer, bool isDateOnly, bool isReadOnly)
    {
      renderer = new TestableBocDateTimeValueRenderer(
          new FakeResourceUrlFactory(),
          GlobalizationService,
          RenderingFeatures.Default,
          new StubLabelReferenceRenderer(),
          new StubValidationErrorRenderer(),
          _dateTextBox,
          _timeTextBox);
      renderer.Render(new BocDateTimeValueRenderingContext(HttpContext, Html.Writer, _control.Object));

      var document = Html.GetResultDocument();
      var container = document.GetAssertedChildElement("span", 0);
      container.AssertAttributeValueEquals("id", c_dateValueID);
      container.AssertAttributeValueContains("class", isDateOnly ? renderer.CssClassDateOnly : renderer.CssClassDateTime);
      if (!isReadOnly)
        container.AssertChildElementCount(isDateOnly ? 1 : 2);
      else
        container.AssertChildElementCount(isDateOnly ? 2 : 6);
      return container;
    }
  }
}