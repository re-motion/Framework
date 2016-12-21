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
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UI.Controls.BocDateTimeValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocDateTimeValueImplementation.Rendering;
using Remotion.Web;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.DatePickerButtonImplementation;
using Rhino.Mocks;
using AttributeCollection = System.Web.UI.AttributeCollection;

namespace Remotion.ObjectBinding.Web.Legacy.UnitTests.UI.Controls.BocDateTimeValueImplementation.Rendering
{
  [TestFixture]
  public class BocDateTimeValueQuirksModeRendererTest : RendererTestBase
  {
    private const string c_defaultControlWidth = "150pt";
    private const string c_dateValueID = "MyDateTimeValue";
    private const string c_dateValueName = "MyDateTimeValue_DateValue";
    private const string c_timeValueName = "MyDateTimeValue_TimeValue";

    private IBocDateTimeValue _dateTimeValue;
    private BocDateTimeValueQuirksModeRenderer _renderer;
    private IResourceUrlFactory _resourceUrlFactory;
    
    [SetUp]
    public void SetUp ()
    {
      Initialize();

      var datePickerButton = MockRepository.GenerateStub<IDatePickerButton>();
      datePickerButton.Stub (stub => stub.EnableClientScript).Return (true);

      _dateTimeValue = MockRepository.GenerateStub<IBocDateTimeValue>();
      _dateTimeValue.Stub (mock => mock.ClientID).Return (c_dateValueID);
      _dateTimeValue.Stub (mock => mock.GetDateValueName ()).Return (c_dateValueName);
      _dateTimeValue.Stub (mock => mock.GetTimeValueName ()).Return (c_timeValueName);
      _dateTimeValue.Stub (mock => mock.DatePickerButton).Return (datePickerButton);
      _dateTimeValue.DatePickerButton.AlternateText = "DatePickerButton";

      _dateTimeValue.Stub (mock => mock.ProvideMaxLength).Return (true);

      var pageStub = MockRepository.GenerateStub<IPage>();
      pageStub.Stub (stub => stub.WrappedInstance).Return (new PageMock());
      _dateTimeValue.Stub (stub => stub.Page).Return (pageStub);

      StateBag stateBag = new StateBag();
      _dateTimeValue.Stub (mock => mock.Attributes).Return (new AttributeCollection (stateBag));
      _dateTimeValue.Stub (mock => mock.Style).Return (_dateTimeValue.Attributes.CssStyle);
      _dateTimeValue.Stub (mock => mock.DateTextBoxStyle).Return (new TextBoxStyle());
      _dateTimeValue.Stub (mock => mock.TimeTextBoxStyle).Return (new TextBoxStyle());
      _dateTimeValue.Stub (mock => mock.DateTimeTextBoxStyle).Return (new TextBoxStyle());
      _dateTimeValue.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));
      
      var dateTimeFormatter = new DateTimeFormatter();
      _dateTimeValue.Stub (stub => stub.DateTimeFormatter).Return (dateTimeFormatter);

      _resourceUrlFactory = new FakeResourceUrlFactory();
    }

    [Test]
    public void RenderUndefined ()
    {
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (false, false, false);
    }

    [Test]
    public void RenderDateTimeWithSeconds ()
    {
      _dateTimeValue.Stub (mock => mock.ShowSeconds).Return (true);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (false, false, false);
    }

    [Test]
    public void RenderDateTime ()
    {
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (false, false, false);
    }

    [Test]
    public void RenderDateTimeAutoPostback ()
    {
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);
      _dateTimeValue.Stub (mock => mock.DateTextBoxStyle).Return (new SingleRowTextBoxStyle());
      _dateTimeValue.Stub (mock => mock.TimeTextBoxStyle).Return (new SingleRowTextBoxStyle ());
      _dateTimeValue.DateTextBoxStyle.AutoPostBack = true;
      _dateTimeValue.TimeTextBoxStyle.AutoPostBack = true;

      AssertDocument (false, false, false);
    }

    [Test]
    public void RenderDate ()
    {
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (false, false, false);
    }

    [Test]
    public void RenderUndefinedDisabled ()
    {
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      AssertDocument (false, true, false);
    }

    [Test]
    public void RenderDateTimeDisabled ()
    {
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      AssertDocument (false, true, false);
    }

    [Test]
    public void RenderDateDisabled ()
    {
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      AssertDocument (false, true, false);
    }

    [Test]
    public void RenderUndefinedReadOnly ()
    {
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);

      AssertDocument (true, false, false);
    }

    [Test]
    public void RenderDateTimeReadOnly ()
    {
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);

      AssertDocument (true, false, false);
    }

    [Test]
    public void RenderDateReadOnly ()
    {
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);

      AssertDocument (true, false, false);
    }

    [Test]
    public void RenderUndefinedWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderDateTimeWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderDateWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderUndefinedDisabledWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderDateTimeDisabledWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderDateDisabledWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderUndefinedReadOnlyWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderDateTimeReadOnlyWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderDateReadOnlyWithStyle ()
    {
      SetStyle (false);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderUndefinedWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderDateTimeWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderDateWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (false, false, true);
    }

    [Test]
    public void RenderUndefinedDisabledWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderDateTimeDisabledWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderDateDisabledWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);

      AssertDocument (false, true, true);
    }

    [Test]
    public void RenderUndefinedReadOnlyWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderDateTimeReadOnlyWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderDateReadOnlyWithStyleInAttributes ()
    {
      SetStyle (true);
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.Date);
      _dateTimeValue.Stub (mock => mock.Value).Return (DateTime.Today);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (true, false, true);
    }

    [Test]
    public void RenderEmptyDateTime ()
    {
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (false, false, false);
    }

    [Test]
    public void RenderEmptyDateTimeReadOnly ()
    {
      _dateTimeValue.Stub (mock => mock.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (mock => mock.IsReadOnly).Return (true);
      _dateTimeValue.Stub (mock => mock.Enabled).Return (true);

      AssertDocument (true, false, false);
    }

    [Test]
    public void RenderIDs ()
    {
      _dateTimeValue.Stub (stub => stub.ActualValueType).Return (BocDateTimeValueType.DateTime);
      _dateTimeValue.Stub (stub => stub.Enabled).Return (true);

      IClientScriptBehavior clientScriptBehaviorStub = MockRepository.GenerateStub<IClientScriptBehavior> ();
      clientScriptBehaviorStub.Stub (stub => stub.IsBrowserCapableOfScripting (HttpContext, _dateTimeValue)).Return (true);
      var renderer = new BocDateTimeValueQuirksModeRenderer (clientScriptBehaviorStub, new FakeResourceUrlFactory ());
      renderer.Render (new BocDateTimeValueRenderingContext (HttpContext, Html.Writer, _dateTimeValue));
      var document = Html.GetResultDocument ();
      var tableRow = document.GetAssertedChildElement ("div", 0).GetAssertedChildElement ("table", 0).GetAssertedChildElement ("tr", 0);
      var dateInput = tableRow.GetAssertedChildElement ("td", 0).GetAssertedChildElement ("input", 0);
      var timeInput = tableRow.GetAssertedChildElement ("td", 2).GetAssertedChildElement ("input", 0);
      dateInput.AssertAttributeValueEquals ("id", c_dateValueName);
      dateInput.AssertAttributeValueEquals ("name", c_dateValueName);
      timeInput.AssertAttributeValueEquals ("id", c_timeValueName);
      timeInput.AssertAttributeValueEquals ("name", c_timeValueName);
    }

    private void SetStyle (bool inAttributes)
    {
      if (inAttributes)
      {
        _dateTimeValue.Style["width"] = "213pt";
        _dateTimeValue.Style["height"] = "23pt";
        _dateTimeValue.Attributes["class"] = "CssClass";
      }
      else
      {
        _dateTimeValue.Width = Unit.Point (213);
        _dateTimeValue.Height = Unit.Point (23);
        _dateTimeValue.CssClass = "CssClass";
        _dateTimeValue.ControlStyle.Width = _dateTimeValue.Width;
        _dateTimeValue.ControlStyle.Height = _dateTimeValue.Height;
      }
    }

    private void AssertDocument (bool isReadOnly, bool isDisabled, bool withStyle)
    {
      var siteStub = MockRepository.GenerateStub<ISite> ();
      siteStub.Stub (stub => stub.DesignMode).Return (true);
      _dateTimeValue.Site = siteStub;

      IClientScriptBehavior clientScriptBehaviorStub = MockRepository.GenerateStub<IClientScriptBehavior>();
      clientScriptBehaviorStub.Stub (stub => stub.IsBrowserCapableOfScripting(HttpContext, _dateTimeValue)).Return (true);
      _renderer = new BocDateTimeValueQuirksModeRenderer (clientScriptBehaviorStub, _resourceUrlFactory);
      _renderer.Render (new BocDateTimeValueRenderingContext(HttpContext, Html.Writer, _dateTimeValue));

      var document = Html.GetResultDocument();
      var div = GetAssertedDiv (document, isReadOnly, isDisabled, withStyle);

      if (isReadOnly)
        AssertSpan (div);
      else
        AssertTable (div, isDisabled, withStyle);
    }

    private void AssertTable (XmlNode div, bool isDisabled, bool withStyle)
    {
      var table = GetAssertedTable (div, withStyle);
      var tr = Html.GetAssertedChildElement (table, "tr", 0);

      XmlNode dateBoxCell = GetAssertedDateBoxCell (tr);
      AssertDateTextBox (dateBoxCell, false, withStyle);

      XmlNode buttonCell = GetAssertedButtonCell (tr);
      Html.AssertChildElementCount (buttonCell, 0);

      if (_dateTimeValue.ActualValueType != BocDateTimeValueType.Date)
      {
        XmlNode timeBoxCell = GetAssertedTimeBoxCell (tr);
        AssertTimeTextBox (timeBoxCell, isDisabled, withStyle);
      }
    }

    private void AssertSpan (XmlNode div)
    {
      var span = Html.GetAssertedChildElement (div, "span", 0);
      string formatString = "d";
      if (_dateTimeValue.ActualValueType == BocDateTimeValueType.DateTime)
        formatString = "g";
      else if (_dateTimeValue.ActualValueType == BocDateTimeValueType.Undefined)
        formatString = "G";

      Html.AssertTextNode (
          span,
          _dateTimeValue.Value.HasValue ? _dateTimeValue.Value.Value.ToString (formatString) : HtmlHelper.WhiteSpace,
          0);
    }

    private XmlNode GetAssertedTimeBoxCell (XmlNode tr)
    {
      var timeBoxCell = Html.GetAssertedChildElement (tr, "td", 2);
      Html.AssertStyleAttribute (timeBoxCell, "width", _dateTimeValue.ShowSeconds ? "45%" : "40%");
      Html.AssertStyleAttribute (timeBoxCell, "padding-left", "0.3em");
      return timeBoxCell;
    }

    private XmlNode GetAssertedDateBoxCell (XmlNode tr)
    {
      var width = "100%";
      if (_dateTimeValue.ActualValueType != BocDateTimeValueType.Date)
      {
        if (_dateTimeValue.ShowSeconds)
          width = "55%";
        else
          width = "60%";
      }

      var dateBoxCell = Html.GetAssertedChildElement (tr, "td", 0);
      Html.AssertStyleAttribute (dateBoxCell, "width", width);
      return dateBoxCell;
    }

    private XmlNode GetAssertedButtonCell (XmlNode tr)
    {
      var buttonCell = Html.GetAssertedChildElement (tr, "td", 1);
      Html.AssertStyleAttribute (buttonCell, "width", "0%");
      Html.AssertStyleAttribute (buttonCell, "padding-left", "0.3em");
      return buttonCell;
    }

    private void AssertTimeTextBox (XmlNode timeBoxCell, bool isDisabled, bool withStyle)
    {
      var timeBox = Html.GetAssertedChildElement (timeBoxCell, "input", 0);
      string timeFormat = "t";

      if (_dateTimeValue.ShowSeconds)
      {
        timeFormat = "T";
      }
      int maxLength = new DateTime (2009, 12, 31, 12, 30, 30).ToString (timeFormat).Length;

      AssertTextBox (timeBox, _dateTimeValue.GetTimeValueName(), maxLength, isDisabled, withStyle, _dateTimeValue.DateTextBoxStyle.AutoPostBack==true);
      if (_dateTimeValue.Value.HasValue)
        Html.AssertAttribute (timeBox, "value", _dateTimeValue.Value.Value.ToString (timeFormat));
      else
        Html.AssertNoAttribute (timeBox, "value");
    }

    private void AssertDateTextBox (XmlNode dateBoxCell, bool isDisabled, bool withStyle)
    {
      var dateBox = Html.GetAssertedChildElement (dateBoxCell, "input", 0);
      AssertTextBox (dateBox, _dateTimeValue.GetDateValueName(), 10, isDisabled, withStyle, _dateTimeValue.TimeTextBoxStyle.AutoPostBack==true);
      if (_dateTimeValue.Value.HasValue)
        Html.AssertAttribute (dateBox, "value", _dateTimeValue.Value.Value.ToString ("d"));
      else
        Html.AssertNoAttribute (dateBox, "value");
    }

    private void AssertTextBox (XmlNode textBox, string id, int maxLength, bool isDisabled, bool withStyle, bool autoPostBack)
    {
      Html.AssertAttribute (textBox, "type", "text");
      Html.AssertAttribute (textBox, "id", id);
      Html.AssertAttribute (textBox, "name", id);
      Html.AssertAttribute (textBox, "maxlength", maxLength.ToString());
      Html.AssertStyleAttribute (textBox, "width", "100%");

      if (autoPostBack)
      {
        Html.AssertAttribute (textBox, "onchange", string.Format ("javascript:__doPostBack('{0}','')", id));
      }

      if (isDisabled)
      {
        Html.AssertAttribute (textBox, "disabled", "disabled");
        Html.AssertAttribute (textBox, "readonly", "readonly");
      }

      if (withStyle)
        Html.AssertStyleAttribute (textBox, "height", "100%");
    }

    private XmlNode GetAssertedTable (XmlNode div, bool withStyle)
    {
      var table = Html.GetAssertedChildElement (div, "table", 0);
      Html.AssertAttribute (table, "cellspacing", "0");
      Html.AssertAttribute (table, "cellpadding", "0");
      Html.AssertAttribute (table, "border", "0");

      string width = c_defaultControlWidth;
      if (withStyle)
        width = _dateTimeValue.Width.IsEmpty ? _dateTimeValue.Style["width"] : _dateTimeValue.Width.ToString();

      Html.AssertStyleAttribute (table, "width", width);
      Html.AssertStyleAttribute (table, "display", "inline");
      return table;
    }

    private XmlNode GetAssertedDiv (XmlDocument document, bool isReadOnly, bool isDisabled, bool withStyle)
    {
      var div = Html.GetAssertedChildElement (document, "div", 0);
      
      Html.AssertAttribute (div, "id", c_dateValueID);

      Html.AssertAttribute (
          div,
          "class",
          withStyle ? _dateTimeValue.CssClass : _renderer.CssClassBase,
          HtmlHelperBase.AttributeValueCompareMode.Contains);

      if (isDisabled)
        Html.AssertAttribute (div, "class", _renderer.CssClassDisabled, HtmlHelperBase.AttributeValueCompareMode.Contains);

      if (isReadOnly)
        Html.AssertAttribute (div, "class", _renderer.CssClassReadOnly, HtmlHelperBase.AttributeValueCompareMode.Contains);

      Html.AssertStyleAttribute (div, "width", "auto");
      Html.AssertStyleAttribute (div, "display", "inline");

      if (withStyle)
        Html.AssertStyleAttribute (div, "height", _dateTimeValue.Height.IsEmpty ? _dateTimeValue.Style["height"] : _dateTimeValue.Height.ToString());

      return div;
    }
  }
}