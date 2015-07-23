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
using Remotion.Globalization;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation;
using Remotion.ObjectBinding.Web.UI.Controls.BocReferenceValueImplementation.Rendering;
using Remotion.ObjectBinding.Web.UnitTests.Domain;
using Remotion.Web;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.Infrastructure;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;
using Rhino.Mocks;

namespace Remotion.ObjectBinding.Web.UnitTests.UI.Controls.BocReferenceValueImplementation.Rendering
{
  [TestFixture]
  public class BocReferenceValueRendererTest : RendererTestBase
  {
    private const string c_clientID = "MyReferenceValue";
    private const string c_valueName = "MyReferenceValue_SelectedValue";
    private const string c_uniqueIdentifier = "uniqueidentifiert";

    private enum OptionMenuConfiguration
    {
      NoOptionsMenu,
      SeparateOptionsMenu,
      EmbeddedOptionsMenu
    }

    private IBusinessObjectProvider _provider;
    private BusinessObjectReferenceDataSource _dataSource;
    protected static readonly Unit Width = Unit.Pixel (250);
    protected static readonly Unit Height = Unit.Point (12);
    private IResourceUrlFactory _resourceUrlFactoryStub;
    
    public IClientScriptManager ClientScriptManagerMock { get; set; }
    public IBocReferenceValue Control { get; set; }
    public TypeWithReference BusinessObject { get; set; }
    public StubDropDownMenu OptionsMenu { get; set; }
    public StubDropDownList DropDownList { get; set; }

    [SetUp]
    public void SetUp ()
    {
      Initialize();

      OptionsMenu = new StubDropDownMenu();
      DropDownList = new StubDropDownList();

      Control = MockRepository.GenerateStub<IBocReferenceValue>();
      Control.Stub (stub => stub.ClientID).Return (c_clientID);
      Control.Stub (stub => stub.ControlType).Return ("BocReferenceValue");
      Control.Stub (stub => stub.Command).Return (new BocCommand());
      Control.Stub (stub => stub.BusinessObjectUniqueIdentifier).Return (c_uniqueIdentifier);
      Control.Command.Type = CommandType.Event;
      Control.Command.Show = CommandShow.Always;

      Control.Stub (stub => stub.OptionsMenu).Return (OptionsMenu);

      IPage pageStub = MockRepository.GenerateStub<IPage>();
      pageStub.Stub (stub => stub.WrappedInstance).Return (new PageMock());
      Control.Stub (stub => stub.Page).Return (pageStub);

      ClientScriptManagerMock = MockRepository.GenerateMock<IClientScriptManager>();
      pageStub.Stub (stub => stub.ClientScript).Return (ClientScriptManagerMock);

      BusinessObject = TypeWithReference.Create ("MyBusinessObject");
      BusinessObject.ReferenceList = new[]
                                     {
                                         TypeWithReference.Create ("ReferencedObject 0"),
                                         TypeWithReference.Create ("ReferencedObject 1"),
                                         TypeWithReference.Create ("ReferencedObject 2")
                                     };
      _dataSource = new BusinessObjectReferenceDataSource();
      _dataSource.BusinessObject = (IBusinessObject) BusinessObject;

      _provider = ((IBusinessObject) BusinessObject).BusinessObjectClass.BusinessObjectProvider;
      _provider.AddService<IBusinessObjectWebUIService> (new ReflectionBusinessObjectWebUIService());

      StateBag stateBag = new StateBag();
      Control.Stub (mock => mock.Attributes).Return (new AttributeCollection (stateBag));
      Control.Stub (mock => mock.Style).Return (Control.Attributes.CssStyle);
      Control.Stub (mock => mock.LabelStyle).Return (new Style (stateBag));
      Control.Stub (mock => mock.DropDownListStyle).Return (new DropDownListStyle());
      Control.Stub (mock => mock.ControlStyle).Return (new Style (stateBag));
      Control.Stub (stub => stub.GetValueName ()).Return (c_valueName);
      Control.Stub (stub => stub.PopulateDropDownList (Arg<DropDownList>.Is.NotNull))
          .WhenCalled (
              invocation =>
              {
                foreach (var item in BusinessObject.ReferenceList)
                  ((DropDownList) invocation.Arguments[0]).Items.Add (new ListItem (item.DisplayName, item.UniqueIdentifier));
              });

      Control.Stub (stub => stub.GetLabelText()).Return ("MyText");
      Control.Stub (stub => stub.ResolveClientUrl (null)).IgnoreArguments().Do ((Func<string, string>) (url => url.TrimStart ('~')));
      Control.Stub (stub => stub.GetResourceManager()).Return (NullResourceManager.Instance);

      _resourceUrlFactoryStub = new FakeResourceUrlFactory();
    }

    [TearDown]
    public void TearDown ()
    {
      ClientScriptManagerMock.VerifyAllExpectations();
    }

    [Test]
    public void RenderNullReferenceValue ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);

      XmlNode containerDiv = GetAssertedContainerSpan (false);
      AssertControl (containerDiv, OptionMenuConfiguration.NoOptionsMenu);
    }

    [Test]
    public void RenderNullReferenceValueWithOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetUpClientScriptExpectations();
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode containerDiv = GetAssertedContainerSpan (false);
      AssertControl (containerDiv, OptionMenuConfiguration.SeparateOptionsMenu);
    }

    [Test]
    [Ignore ("Assertions for embedded menu are incorrect: COMMONS-2431")]
    public void RenderNullReferenceValueWithEmbeddedOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, OptionMenuConfiguration.EmbeddedOptionsMenu);
    }

    [Test]
    [Ignore ("Assertions for embedded menu are incorrect: COMMONS-2431")]
    public void RenderNullReferenceValueWithEmbeddedOptionsMenuAndStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, OptionMenuConfiguration.EmbeddedOptionsMenu);
    }

    [Test]
    public void RenderNullReferenceValueReadOnly ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertReadOnlyContent (span);
    }

    [Test]
    public void RenderNullReferenceValueReadOnlyWithStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);
      AddStyle();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertReadOnlyContent (span);
    }

    [Test]
    [Ignore ("Assertions for embedded menu are incorrect: COMMONS-2431")]
    public void RenderNullReferenceValueReadOnlyWithOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertReadOnlyContent (span);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      AddStyle();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, OptionMenuConfiguration.NoOptionsMenu);
    }

    [Test]
    public void RenderNullReferenceValueWithOptionsAndStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, OptionMenuConfiguration.SeparateOptionsMenu);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderNullReferenceValueWithIcon ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.Property).Return (
          (IBusinessObjectReferenceProperty) ((IBusinessObject) BusinessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue"));
      SetUpGetIconExpectations();

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, OptionMenuConfiguration.NoOptionsMenu);
    }

    [Test]
    public void RenderReferenceValue ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetUpClientScriptExpectations();
      SetValue();
      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, OptionMenuConfiguration.NoOptionsMenu);
    }

    [Test]
    public void RenderReferenceValueAutoPostback ()
    {
      DropDownList.AutoPostBack = false;
      Control.Stub (stub => stub.Enabled).Return (true);
      SetUpClientScriptExpectations();
      SetValue();

      Control.Stub (stub => stub.DropDownListStyle).Return (new DropDownListStyle());
      Control.DropDownListStyle.AutoPostBack = true;

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, OptionMenuConfiguration.NoOptionsMenu);
      Assert.That (DropDownList.AutoPostBack, Is.True);
    }

    private void SetValue ()
    {
      BusinessObject.ReferenceValue = BusinessObject.ReferenceList[0];
      Control.Stub (stub => stub.Value).Return ((IBusinessObjectWithIdentity) BusinessObject.ReferenceValue);
    }

    [Test]
    public void RenderReferenceValueWithOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetUpClientScriptExpectations();
      SetValue();
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, OptionMenuConfiguration.SeparateOptionsMenu);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    [Ignore ("Assertions for embedded menu are incorrect: COMMONS-2431")]
    public void RenderReferenceValueWithEmbeddedOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue();
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, OptionMenuConfiguration.EmbeddedOptionsMenu);
    }

    [Test]
    [Ignore ("Assertions for embedded menu are incorrect: COMMONS-2431")]
    public void RenderReferenceValueWithEmbeddedOptionsMenuAndStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue();
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle();

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, OptionMenuConfiguration.EmbeddedOptionsMenu);
    }

    [Test]
    public void RenderReferenceValueReadOnly ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue();
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertReadOnlyContent (span);
    }

    [Test]
    public void RenderReferenceValueReadOnlyWithStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue();
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);
      AddStyle();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertReadOnlyContent (span);
    }

    [Test]
    [Ignore ("Assertions for embedded menu are incorrect: COMMONS-2431")]
    public void RenderReferenceValueReadOnlyWithOptionsMenu ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue();
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      XmlNode span = GetAssertedContainerSpan (false);
      AssertReadOnlyContent (span);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue();
      AddStyle();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, OptionMenuConfiguration.NoOptionsMenu);
    }

    [Test]
    public void RenderReferenceValueWithOptionsAndStyle ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue();
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);
      AddStyle();

      XmlNode span = GetAssertedContainerSpan (true);
      AssertControl (span, OptionMenuConfiguration.SeparateOptionsMenu);

      Assert.That (OptionsMenu.Style["width"], Is.Null);
      Assert.That (OptionsMenu.Style["height"], Is.Null);
    }

    [Test]
    public void RenderReferenceValueWithIcon ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);
      SetValue();
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.Property).Return (
          (IBusinessObjectReferenceProperty) ((IBusinessObject) BusinessObject).BusinessObjectClass.GetPropertyDefinition ("ReferenceValue"));
      SetUpGetIconExpectations();

      XmlNode span = GetAssertedContainerSpan (false);
      AssertControl (span, OptionMenuConfiguration.NoOptionsMenu);
    }

    [Test]
    [Ignore ("Assertions for embedded menu are incorrect: COMMONS-2431")]
    public void RenderOptions ()
    {
      var renderer = new TestableBocReferenceValueRenderer (
          _resourceUrlFactoryStub,
          GlobalizationService,
          RenderingFeatures.Default,
          () => new StubDropDownList());
      Control.Stub (stub => stub.HasValueEmbeddedInsideOptionsMenu).Return (true);
      Control.Stub (stub => stub.HasOptionsMenu).Return (true);

      Html.Writer.AddAttribute (HtmlTextWriterAttribute.Class, "body");
      Html.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      renderer.RenderOptionsMenuTitle (CreateRenderingContext());
      Html.Writer.RenderEndTag();

      var document = Html.GetResultDocument();
      AssertControl (document, OptionMenuConfiguration.EmbeddedOptionsMenu);
    }

    [Test]
    public void RenderOptionsReadOnly ()
    {
      Control.Stub (stub => stub.IsIconEnabled()).Return (true);
      Control.Stub (stub => stub.IsReadOnly).Return (true);

      var renderer = new TestableBocReferenceValueRenderer (
          _resourceUrlFactoryStub,
          GlobalizationService,
          RenderingFeatures.Default,
          () => new StubDropDownList());
      Html.Writer.AddAttribute (HtmlTextWriterAttribute.Class, "body");
      Html.Writer.RenderBeginTag (HtmlTextWriterTag.Span);
      renderer.RenderOptionsMenuTitle (CreateRenderingContext());
      Html.Writer.RenderEndTag();
      
      var document = Html.GetResultDocument();
      AssertReadOnlyContent (document);
    }

    [Test]
    public void RenderIDs ()
    {
      Control.Stub (stub => stub.Enabled).Return (true);

      var renderer = new BocReferenceValueRenderer (_resourceUrlFactoryStub, GlobalizationService, RenderingFeatures.Default);
      renderer.Render (CreateRenderingContext ());
      var document = Html.GetResultDocument ();
      var select = document.GetAssertedChildElement ("span", 0).GetAssertedChildElement ("span", 0).GetAssertedChildElement ("span", 1).GetAssertedChildElement ("select", 0);
      select.AssertAttributeValueEquals ("id", c_valueName);
      select.AssertAttributeValueEquals ("name", c_valueName);
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes()
    {
      Control.DropDownListStyle.AutoPostBack = true;

      var renderer = new BocReferenceValueRenderer (_resourceUrlFactoryStub, GlobalizationService, RenderingFeatures.WithDiagnosticMetadata);
      renderer.Render (CreateRenderingContext ());

      var document = Html.GetResultDocument();
      var control = document.DocumentElement;
      control.AssertAttributeValueEquals (DiagnosticMetadataAttributes.ControlType, "BocReferenceValue");
      control.AssertAttributeValueEquals (DiagnosticMetadataAttributes.TriggersPostBack, "true");
    }

    private void AssertReadOnlyContent (XmlNode parent)
    {
      var span = parent.GetAssertedChildElement ("span", 0);
      span.AssertAttributeValueEquals ("class", "body");
      span.AssertChildElementCount (Control.HasOptionsMenu ? 2 : 1);

      var commandLink = span.GetAssertedChildElement ("a", 0);
      commandLink.AssertAttributeValueEquals ("id", Control.ClientID + "_Command");
      commandLink.AssertAttributeValueEquals ("class", "command");
      commandLink.AssertChildElementCount (1);

      var contentSpan = commandLink.GetAssertedChildElement ("span", 0);
      contentSpan.AssertAttributeValueEquals ("class", "content withoutOptionsMenu");
      contentSpan.AssertChildElementCount (1);

      var innerSpan = contentSpan.GetAssertedChildElement ("span", 0);
      innerSpan.AssertAttributeValueEquals ("id", c_clientID + "_Value");
      innerSpan.AssertAttributeValueEquals ("data-value", c_uniqueIdentifier);
      innerSpan.AssertChildElementCount (0);
      innerSpan.AssertTextNode ("MyText", 0);

      if (Control.HasOptionsMenu)
      {
        var wrapperSpan = span.GetAssertedChildElement ("span", 1);
        wrapperSpan.AssertAttributeValueEquals ("class", "optionsMenu");
        wrapperSpan.AssertChildElementCount (0);
        wrapperSpan.AssertTextNode ("DropDownMenu", 0);
      }
    }

    private void AssertControl (XmlNode containerDiv, OptionMenuConfiguration optionMenuConfiguration)
    {
      var contentDiv = containerDiv.GetAssertedChildElement ("span", 0);
      contentDiv.AssertAttributeValueEquals ("class", "body");

      AssertIcon (contentDiv, true);

      var contentSpan = contentDiv.GetAssertedChildElement ("span", 1);
      switch (optionMenuConfiguration)
      {
        case OptionMenuConfiguration.NoOptionsMenu:
          contentSpan.AssertAttributeValueEquals ("class", "content withoutOptionsMenu");
          break;
        case OptionMenuConfiguration.SeparateOptionsMenu:
          contentSpan.AssertAttributeValueEquals ("class", "content separateOptionsMenu");
          break;
        case OptionMenuConfiguration.EmbeddedOptionsMenu:
          contentSpan.AssertAttributeValueEquals ("class", "content embeddedOptionsMenu");
          break;
      }

      contentSpan.AssertTextNode ("DropDownList", 0);

      if (optionMenuConfiguration == OptionMenuConfiguration.SeparateOptionsMenu)
      {
        var optionsMenuDiv = contentDiv.GetAssertedChildElement ("span", 2);
        optionsMenuDiv.AssertAttributeValueEquals ("class", "optionsMenu");
        optionsMenuDiv.AssertTextNode ("DropDownMenu", 0);
      }
    }

    private XmlNode GetAssertedContainerSpan (bool withStyle)
    {
      var renderer = new TestableBocReferenceValueRenderer (
          _resourceUrlFactoryStub,
          GlobalizationService,
          RenderingFeatures.Default,
          () => DropDownList);
      renderer.Render (CreateRenderingContext());
      
      var document = Html.GetResultDocument();
      var containerDiv = document.GetAssertedChildElement ("span", 0);

      containerDiv.AssertAttributeValueEquals ("id", "MyReferenceValue");
      containerDiv.AssertAttributeValueContains ("class", "bocReferenceValue");
      if (Control.IsReadOnly)
        containerDiv.AssertAttributeValueContains ("class", "readOnly");
      if (!Control.Enabled)
        containerDiv.AssertAttributeValueContains ("class", "disabled");

      if (withStyle)
      {
        containerDiv.AssertStyleAttribute ("width", Width.ToString());
        containerDiv.AssertStyleAttribute ("height", Height.ToString());
      }

      return containerDiv;
    }

    protected void AddStyle ()
    {
      Control.Height = Height;
      Control.Width = Width;
      Control.Style["height"] = Control.Height.ToString();
      Control.Style["width"] = Control.Width.ToString();
    }

    protected void SetUpGetIconExpectations ()
    {
      Control.Expect (mock => mock.GetIcon ()).Return (new IconInfo ("~/Images/NullIcon.gif"));
    }

    protected void SetUpClientScriptExpectations ()
    {
      ClientScriptManagerMock.Expect (mock => mock.GetPostBackEventReference (Control, BocReferenceValueBase.CommandArgumentName))
                             .Return ("PostBackEventReference");
    }

    protected void AssertIcon (XmlNode parent, bool wrapNonCommandIcon)
    {
      var isIconEnabled = Control.IsIconEnabled();
      var commandClass = "command";
      if (isIconEnabled)
        commandClass += " hasIcon";

      if (Control.IsCommandEnabled())
      {
        var link = parent.GetAssertedChildElement ("a", 0);
        link.AssertAttributeValueEquals ("id", Control.ClientID + "_Command");
        link.AssertAttributeValueEquals ("class", commandClass);
        link.AssertAttributeValueEquals ("href", "#");
        link.AssertAttributeValueEquals ("onclick", "");
        if (isIconEnabled)
        {
          link.AssertChildElementCount (1);

          var icon = link.GetAssertedChildElement ("img", 0);
          icon.AssertAttributeValueEquals ("src", "/Images/Remotion.ObjectBinding.UnitTests.Web.Domain.TypeWithReference.gif");
        }
        else
        {
          link.AssertChildElementCount (0);
        }
      }
      else
      {
        var iconParent = parent;
        if (wrapNonCommandIcon)
        {
          var anchor = parent.GetAssertedChildElement ("a", 0);
          anchor.AssertAttributeValueEquals ("id", Control.ClientID + "_Command");
          anchor.AssertAttributeValueEquals ("class", commandClass);

          iconParent = anchor;
        }

        if (isIconEnabled)
        {
          iconParent.AssertChildElementCount (1);
          var icon = iconParent.GetAssertedChildElement ("img", 0);
          icon.AssertAttributeValueEquals ("src", "/Images/NullIcon.gif");
        }
        else
        {
          iconParent.AssertChildElementCount (0);
        }
      }
    }

    private BocReferenceValueRenderingContext CreateRenderingContext ()
    {
      return new BocReferenceValueRenderingContext (HttpContext, Html.Writer, Control, null);
    }
  }
}