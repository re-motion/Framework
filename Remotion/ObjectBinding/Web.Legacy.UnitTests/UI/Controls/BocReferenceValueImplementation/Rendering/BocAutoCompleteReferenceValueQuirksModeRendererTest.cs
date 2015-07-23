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
using System.Web.UI.WebControls;
using System.Xml;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.Resources;
using Remotion.Development.Web.UnitTesting.UI.Controls.Rendering;
using Remotion.ObjectBinding.Web.Legacy.UnitTests.Domain;
using Remotion.ObjectBinding.Web.Services;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.Web;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.Legacy.UnitTests.UI.Controls.BocReferenceValueImplementation.Rendering
{
  [TestFixture]
  public class BocAutoCompleteReferenceValueQuirksModeRendererTest : RendererTestBase
  {
    private const string c_clientID = "MyReferenceValue";
    private const string c_textValueName = "MyReferenceValue_TextValue";
    private const string c_keyValueName = "MyReferenceValue_Boc_HiddenValue";
    private static readonly Unit s_width = Unit.Pixel (250);
    private static readonly Unit s_height = Unit.Point (12);

    private BusinessObjectReferenceDataSource _dataSource;
    private IBusinessObjectProvider _provider;
    private IResourceUrlFactory _resourceUrlFactory;
    private IBocAutoCompleteReferenceValue Control { get; set; }
    private DropDownMenu OptionsMenu { get; set; }
    private IClientScriptManager ClientScriptManagerMock { get; set; }
    private TypeWithReference BusinessObject { get; set; }
    private StubTextBox TextBox { get; set; }
    

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      OptionsMenu = new StubDropDownMenu ();
      TextBox = new StubTextBox();

      Control = MockRepository.GenerateStub<IBocAutoCompleteReferenceValue> ();
      Control.Stub (stub => stub.ClientID).Return (c_clientID);
      Control.Stub (stub => stub.GetTextValueName()).Return (c_textValueName);
      Control.Stub (stub => stub.GetKeyValueName()).Return (c_keyValueName);
      Control.Stub (stub => stub.Command).Return (new BocCommand ());
      Control.Command.Type = CommandType.Event;
      Control.Command.Show = CommandShow.Always;
      Control.Stub (stub => stub.SearchServicePath).Return ("~/SearchService.asmx");

      Control.Stub (stub => stub.OptionsMenu).Return (OptionsMenu);

      IPage pageStub = MockRepository.GenerateStub<IPage> ();
      Control.Stub (stub => stub.Page).Return (pageStub);

      ClientScriptManagerMock = MockRepository.GenerateMock<IClientScriptManager> ();
      pageStub.Stub (stub => stub.ClientScript).Return (ClientScriptManagerMock);

      BusinessObject = TypeWithReference.Create ("MyBusinessObject");
      BusinessObject.ReferenceList = new[]
                                     {
                                         TypeWithReference.Create ("ReferencedObject 0"),
                                         TypeWithReference.Create ("ReferencedObject 1"),
                                         TypeWithReference.Create ("ReferencedObject 2")
                                     };
      _dataSource = new BusinessObjectReferenceDataSource ();
      _dataSource.BusinessObject = (IBusinessObject) BusinessObject;

      _provider = ((IBusinessObject) BusinessObject).BusinessObjectClass.BusinessObjectProvider;
      _provider.AddService<IBusinessObjectWebUIService> (new ReflectionBusinessObjectWebUIService ());

      StateBag stateBag = new StateBag ();
      Control.Stub (mock => mock.Attributes).Return (new AttributeCollection (stateBag));
      Control.Stub (mock => mock.Style).Return (Control.Attributes.CssStyle);
      Control.Stub (mock => mock.CommonStyle).Return (new Style (stateBag));
      Control.Stub (mock => mock.LabelStyle).Return (new Style (stateBag));
      Control.Stub (mock => mock.TextBoxStyle).Return (new SingleRowTextBoxStyle());
      Control.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));

      Control.Stub (stub => stub.GetLabelText()).Return ("MyText");

      _resourceUrlFactory = new FakeResourceUrlFactory();
    }

    [TearDown]
    public void TearDown ()
    {
      ClientScriptManagerMock.VerifyAllExpectations ();
    }

    protected override void Initialize ()
    {
      base.Initialize ();
      HttpResponseBase response = MockRepository.GenerateMock<HttpResponseBase> ();
      HttpContext.Stub (mock => mock.Response).Return (response);
      response.Stub (mock => mock.ContentType).Return ("text/html");

      HttpBrowserCapabilities browser = new HttpBrowserCapabilities ();
      browser.Capabilities = new Hashtable ();
      browser.Capabilities.Add ("browser", "IE");
      browser.Capabilities.Add ("majorversion", "7");

      var request = MockRepository.GenerateStub<HttpRequestBase> ();
      request.Stub (stub => stub.Browser).Return (new HttpBrowserCapabilitiesWrapper (browser));

      HttpContext.Stub (stub => stub.Request).Return (request);
    }

    [Test]
    [Ignore ("Assertions for embedded menu are incorrect: COMMONS-2431")]
    public void RenderReferenceValueAutoPostback ()
    {
      TextBox.AutoPostBack = false;
      Control.Stub (stub => stub.Enabled).Return (true);
      SetUpClientScriptExpectations ();
      SetValue ();

      Control.Stub (stub => stub.TextBoxStyle).Return (new SingleRowTextBoxStyle ());
      Control.TextBoxStyle.AutoPostBack = true;

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, false, false);
      Assert.That (TextBox.AutoPostBack, Is.True);
    }

    [Test]
    public void RenderNullReferenceValue ()
    {
      SetUpClientScriptExpectations ();

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, false, false);
    }

    [Test]
    public void RenderNullReferenceValueWithOptionsMenu ()
    {
      SetUpClientScriptExpectations ();
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, false, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithEmbeddedOptionsMenu ()
    {
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode div = GetAssertedDiv (0, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (OptionsMenu.Style["width"], Is.EqualTo ("150pt"));
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithEmbeddedOptionsMenuAndStyle ()
    {
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle ();

      XmlNode div = GetAssertedDiv (0, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (OptionsMenu.Style["width"], Is.EqualTo ("100%"));
      Assert.That (OptionsMenu.Style["height"], Is.EqualTo ("100%"));
    }

    [Test]
    public void RenderNullReferenceValueReadOnly ()
    {
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, true, false, false);
    }

    [Test]
    public void RenderNullReferenceValueReadOnlyWithStyle ()
    {
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);
      AddStyle ();

      XmlNode div = GetAssertedDiv (1, true);
      XmlNode table = GetAssertedTable (div, true);
      AssertRow (table, true, false, false);
    }

    [Test]
    [Ignore ("Assertions for embedded menu are incorrect: COMMONS-2431")]
    public void RenderNullReferenceValueReadOnlyWithOptionsMenu ()
    {
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode div = GetAssertedDiv (1, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (OptionsMenu.Style["width"], Is.EqualTo ("0%"));
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithStyle ()
    {
      AddStyle ();

      XmlNode div = GetAssertedDiv (1, true);
      XmlNode table = GetAssertedTable (div, true);
      AssertRow (table, false, false, false);
    }

    [Test]
    public void RenderNullReferenceValueWithOptionsAndStyle ()
    {
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle ();

      XmlNode div = GetAssertedDiv (1, true);
      XmlNode table = GetAssertedTable (div, true);
      AssertRow (table, false, false, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithIcon ()
    {
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.Property).Return (
          (IBusinessObjectReferenceProperty) ((IBusinessObject) BusinessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue"));
      SetUpGetIconExpectations ();

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, true, false);
    }

    [Test]
    public void RenderReferenceValue ()
    {
      SetUpClientScriptExpectations ();
      SetValue ();
      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, false, false);
    }

    private void SetValue ()
    {
      BusinessObject.ReferenceValue = BusinessObject.ReferenceList[0];
      Control.Stub (stub => stub.Value).Return ((IBusinessObjectWithIdentity) BusinessObject.ReferenceValue);
    }

    [Test]
    public void RenderReferenceValueWithOptionsMenu ()
    {
      SetUpClientScriptExpectations ();
      SetValue ();
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, false, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithEmbeddedOptionsMenu ()
    {
      SetValue ();
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode div = GetAssertedDiv (0, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (OptionsMenu.Style["width"], Is.EqualTo ("150pt"));
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithEmbeddedOptionsMenuAndStyle ()
    {
      SetValue ();
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle ();

      XmlNode div = GetAssertedDiv (0, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (OptionsMenu.Style["width"], Is.EqualTo ("100%"));
      Assert.That (OptionsMenu.Style["height"], Is.EqualTo ("100%"));
    }

    [Test]
    public void RenderReferenceValueReadOnly ()
    {
      SetValue ();
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, true, false, false);
    }

    [Test]
    public void RenderReferenceValueReadOnlyWithStyle ()
    {
      SetValue ();
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);
      AddStyle ();

      XmlNode div = GetAssertedDiv (1, true);
      XmlNode table = GetAssertedTable (div, true);
      AssertRow (table, true, false, false);
    }

    [Test]
    [Ignore ("Assertions for embedded menu are incorrect: COMMONS-2431")]
    public void RenderReferenceValueReadOnlyWithOptionsMenu ()
    {
      SetValue ();
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode div = GetAssertedDiv (0, false);
      div.AssertTextNode ("DropDownMenu", 0);

      Assert.That (OptionsMenu.Style["width"], Is.EqualTo ("0%"));
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithStyle ()
    {
      SetValue ();
      AddStyle ();

      XmlNode div = GetAssertedDiv (1, true);
      XmlNode table = GetAssertedTable (div, true);
      AssertRow (table, false, false, false);
    }

    [Test]
    public void RenderReferenceValueWithOptionsAndStyle ()
    {
      SetValue ();
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle ();

      XmlNode div = GetAssertedDiv (1, true);
      XmlNode table = GetAssertedTable (div, true);
      AssertRow (table, false, false, false);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithIcon ()
    {
      SetValue ();
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.Property).Return (
          (IBusinessObjectReferenceProperty) ((IBusinessObject) BusinessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue"));
      SetUpGetIconExpectations ();

      XmlNode div = GetAssertedDiv (1, false);
      XmlNode table = GetAssertedTable (div, false);
      AssertRow (table, false, true, false);
    }

    [Test]
    public void RenderOptions ()
    {
      var renderer = new TestableBocAutoCompleteReferenceValueQuirksModeRenderer (_resourceUrlFactory, () => new StubTextBox());
      Html.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      renderer.RenderOptionsMenuTitle (CreateRenderingContext());
      Html.Writer.RenderEndTag ();
      
      var document = Html.GetResultDocument ();
      AssertRow (document, false, false, false);
    }

    [Test]
    public void RenderOptionsReadOnly ()
    {
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      var renderer = new TestableBocAutoCompleteReferenceValueQuirksModeRenderer (_resourceUrlFactory, () => new StubTextBox());
      Html.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      renderer.RenderOptionsMenuTitle (CreateRenderingContext());
      Html.Writer.RenderEndTag ();


      var document = Html.GetResultDocument ();
      AssertRow (document, true, false, false);
    }

    [Test]
    public void RenderOptionsReadOnlyWithStyle ()
    {
      AddStyle ();
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      var renderer = new TestableBocAutoCompleteReferenceValueQuirksModeRenderer (_resourceUrlFactory);
      Html.Writer.RenderBeginTag (HtmlTextWriterTag.Tr);
      renderer.RenderOptionsMenuTitle (CreateRenderingContext());
      Html.Writer.RenderEndTag ();

      var document = Html.GetResultDocument ();
      AssertRow (document, true, false, true);
    }

    private XmlNode GetAssertedDiv (int expectedChildElements, bool withStyle)
    {
      var renderer = new TestableBocAutoCompleteReferenceValueQuirksModeRenderer (_resourceUrlFactory, () =>TextBox);

      Assert.That (TextBox.ID, Is.Null);
      renderer.Render (CreateRenderingContext());
      if (!Control.IsReadOnly)
        Assert.That (TextBox.ID, Is.EqualTo (Control.GetTextValueName ()));

      var document = Html.GetResultDocument ();
      var div = document.GetAssertedChildElement ("div", 0);
      div.AssertAttributeValueEquals ("id", "MyReferenceValue");
      div.AssertAttributeValueContains ("class", "bocAutoCompleteReferenceValue");
      if (Control.IsReadOnly)
        div.AssertAttributeValueContains ("class", "readOnly");

      div.AssertStyleAttribute ("display", "inline");
      if (withStyle)
      {
        div.AssertStyleAttribute ("height", s_height.ToString ());
        div.AssertStyleAttribute ("width", s_width.ToString ());
      }

      div.AssertChildElementCount (expectedChildElements);
      return div;
    }

    private XmlNode GetAssertedTable (XmlNode div, bool withStyle)
    {
      var table = div.GetAssertedChildElement ("table", 0);
      table.AssertAttributeValueEquals ("cellspacing", "0");
      table.AssertAttributeValueEquals ("cellpadding", "0");
      table.AssertAttributeValueEquals ("border", "0");

      table.AssertStyleAttribute ("display", "inline");

      if (withStyle)
      {
        table.AssertStyleAttribute ("width", "100%");
        if (!Control.IsReadOnly)
          table.AssertStyleAttribute ("height", "100%");
      }
      else if (!Control.IsReadOnly)
        table.AssertStyleAttribute ("width", "150pt");

      table.AssertChildElementCount (1);
      return table;
    }

    private void AssertRow (XmlNode table, bool hasLabel, bool hasIcon, bool hasDummyCell)
    {
      var row = table.GetAssertedChildElement ("tr", 0);

      int cellCount = 1;
      if (Control.HasOptionsMenu)
        cellCount++;
      if (hasIcon)
        cellCount++;
      if (hasDummyCell)
        cellCount++;

      row.AssertChildElementCount (cellCount);

      if (hasIcon)
        AssertIconCell (row);

      AssertValueCell (row, hasLabel, hasIcon ? 1 : 0);

      if (Control.HasOptionsMenu)
        AssertMenuCell (row);

      if (hasDummyCell)
      {
        var cell = row.GetAssertedChildElement ("td", cellCount - 1);
        cell.AssertStyleAttribute ("width", "1%");
        cell.AssertChildElementCount (0);
      }
    }

    private void AssertIconCell (XmlNode row)
    {
      var iconCell = row.GetAssertedChildElement ("td", 0);
      iconCell.AssertAttributeValueEquals ("class", "bocAutoCompleteReferenceValueContent");
      iconCell.AssertStyleAttribute ("width", "0%");
      iconCell.AssertStyleAttribute ("padding-right", "0.3em");
      iconCell.AssertChildElementCount (1);

      XmlNode iconParent;
      if (Control.IsCommandEnabled ())
        iconParent = iconCell.GetAssertedChildElement ("a", 0);
      else
        iconParent = iconCell.GetAssertedChildElement ("span", 0);

      AssertIcon (iconParent, false);
    }

    private void AssertValueCell (XmlNode row, bool hasLabel, int index)
    {
      var valueCell = row.GetAssertedChildElement ("td", index);
      valueCell.AssertAttributeValueEquals ("class", "bocAutoCompleteReferenceValueContent");
      if (Control.IsReadOnly)
        valueCell.AssertStyleAttribute ("width", "auto");
      else
        valueCell.AssertStyleAttribute ("width", "100%");

      valueCell.AssertChildElementCount (1);
      if (hasLabel)
      {
        if (Control.IsCommandEnabled ())
        {
          var link = valueCell.GetAssertedChildElement ("a", 0);
          link.AssertAttributeValueEquals ("href", "#");
          link.AssertAttributeValueEquals ("onclick", "");
          link.AssertChildElementCount (1);

          var label = link.GetAssertedChildElement ("span", 0);
          label.AssertTextNode (Control.Value.DisplayName, 0);
        }
        else
        {
          var span = valueCell.GetAssertedChildElement ("span", 0);
          var label = span.GetAssertedChildElement ("span", 0);
          label.AssertTextNode ("MyText", 0);
        }
      }
      else
        valueCell.AssertTextNode ("TextBox", 0);
    }

    private void AssertMenuCell (XmlNode row)
    {
      var menuCell = row.GetAssertedChildElement ("td", 1);
      menuCell.AssertStyleAttribute ("padding-left", "0.3em");
      menuCell.AssertStyleAttribute ("width", "0%");
      menuCell.AssertChildElementCount (0);
      menuCell.AssertTextNode ("DropDownMenu", 0);
    }

    protected void AddStyle ()
    {
      Control.Height = s_height;
      Control.Width = s_width;
      Control.Style["height"] = Control.Height.ToString ();
      Control.Style["width"] = Control.Width.ToString ();
    }

    protected void SetUpGetIconExpectations ()
    {
      Control.Expect (mock => mock.GetIcon ()).IgnoreArguments ().Return (new IconInfo ("~/Images/NullIcon.gif"));
    }

    protected void SetUpClientScriptExpectations ()
    {
      ClientScriptManagerMock.Expect (mock => mock.GetPostBackEventReference (Control, BocReferenceValueBase.CommandArgumentName))
                             .Return ("PostBackEventReference");
    }

    protected void AssertIcon (XmlNode parent, bool wrapNonCommandIcon)
    {
      if (Control.IsCommandEnabled ())
      {
        var link = parent.GetAssertedChildElement ("a", 0);
        link.AssertAttributeValueEquals ("class", "bocAutoCompleteReferenceValueCommand");
        link.AssertAttributeValueEquals ("href", "#");
        link.AssertAttributeValueEquals ("onclick", "");
        link.AssertChildElementCount (1);

        var icon = link.GetAssertedChildElement ("img", 0);
        icon.AssertAttributeValueEquals ("src", "~/Images/Remotion.ObjectBinding.UnitTests.Web.Domain.TypeWithReference.gif");
        icon.AssertStyleAttribute ("border-style", "none");
      }
      else
      {
        var iconParent = parent;
        if (wrapNonCommandIcon)
        {
          var span = parent.GetAssertedChildElement ("span", 0);
          span.AssertAttributeValueEquals ("class", "bocAutoCompleteReferenceValueCommand");

          iconParent = span;
        }

        var icon = iconParent.GetAssertedChildElement ("img", 0);
        icon.AssertAttributeValueEquals ("src", "~/Images/NullIcon.gif");
        icon.AssertStyleAttribute ("border-style", "none");
      }
    }
 
    private BocAutoCompleteReferenceValueRenderingContext CreateRenderingContext ()
    {
      return new BocAutoCompleteReferenceValueRenderingContext (
          HttpContext,
          Html.Writer,
          Control,
          SearchAvailableObjectWebServiceContext.Create (Control.DataSource, Control.Property, "SearchArgs"),
          BusinessObjectIconWebServiceContext.Create (null, "IconArgs"));
    }
  }
}