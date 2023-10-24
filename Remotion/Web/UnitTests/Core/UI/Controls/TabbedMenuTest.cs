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
using System.Linq;
using System.Web;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Development.Web.UnitTesting.Configuration;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{

[TestFixture]
public class TabbedMenuTest: WebControlTest
{
  private HttpContext _currentHttpContext;

  private TabbedMenu _tabbedMenu;

  private MainMenuTab _mainMenuTab1;
  private MainMenuTab _mainMenuTab2;
  private MainMenuTab _mainMenuTab3;

  private SubMenuTab _subMenuTab11;
  private SubMenuTab _subMenuTab12;
  private SubMenuTab _subMenuTab13;

  private SubMenuTab _subMenuTab21;
  private SubMenuTab _subMenuTab22;
  private SubMenuTab _subMenuTab23;

  protected override void SetUpContext ()
  {
    base.SetUpContext();

    _currentHttpContext = HttpContextHelper.CreateHttpContext("GET", "default.html", null);
    _currentHttpContext.Response.ContentEncoding = System.Text.Encoding.UTF8;
    HttpContextHelper.SetCurrent(_currentHttpContext);
  }

  protected override void SetUpPage ()
  {
    base.SetUpPage();

    _tabbedMenu = new TabbedMenu();
    _tabbedMenu.ID = "TabbedMenu";

    _mainMenuTab1 = new MainMenuTab("MainMenuTab1", WebString.CreateFromText("Main 1"));
    _mainMenuTab2 = new MainMenuTab("MainMenuTab2", WebString.CreateFromText("Main 2"));
    _mainMenuTab3 = new MainMenuTab("MainMenuTab3", WebString.CreateFromText("Main 3"));

    _subMenuTab11 = new SubMenuTab("SubMenuTab11", WebString.CreateFromText("Sub 1.1"));
    _subMenuTab12 = new SubMenuTab("SubMenuTab12", WebString.CreateFromText("Sub 1.2"));
    _subMenuTab13 = new SubMenuTab("SubMenuTab13", WebString.CreateFromText("Sub 1.3"));

    _subMenuTab21 = new SubMenuTab("SubMenuTab21", WebString.CreateFromText("Sub 2.1"));
    _subMenuTab22 = new SubMenuTab("SubMenuTab22", WebString.CreateFromText("Sub 2.2"));
    _subMenuTab23 = new SubMenuTab("SubMenuTab23", WebString.CreateFromText("Sub 2.3"));

    _mainMenuTab1.SubMenuTabs.Add(_subMenuTab11);
    _mainMenuTab1.SubMenuTabs.Add(_subMenuTab12);
    _mainMenuTab1.SubMenuTabs.Add(_subMenuTab13);

    _mainMenuTab2.SubMenuTabs.Add(_subMenuTab21);
    _mainMenuTab2.SubMenuTabs.Add(_subMenuTab22);
    _mainMenuTab2.SubMenuTabs.Add(_subMenuTab23);

    _tabbedMenu.Tabs.Add(_mainMenuTab1);
    _tabbedMenu.Tabs.Add(_mainMenuTab2);
    _tabbedMenu.Tabs.Add(_mainMenuTab3);
  }

  [Test]
  public void GetUrlParametersForMainMenuTab ()
  {
    string expectedParameterValue = _mainMenuTab2.ItemID;

    NameValueCollection parameters = _tabbedMenu.GetUrlParameters(_mainMenuTab2);

    Assert.That(parameters, Is.Not.Null);
    Assert.That(parameters.Count, Is.EqualTo(1));
    Assert.That(parameters.GetKey(0), Is.Not.Null);
    Assert.That(parameters.GetKey(0), Is.EqualTo(_tabbedMenu.SelectionID));
    Assert.That(parameters.Get(0), Is.Not.Null);
    Assert.That(parameters.Get(0), Is.EqualTo(expectedParameterValue));
  }

  [Test]
  public void GetUrlParametersForSubMenuTab ()
  {
    string expectedParameterValue = string.Format("{0},{1}", _subMenuTab22.Parent.ItemID, _subMenuTab22.ItemID);

    NameValueCollection parameters = _tabbedMenu.GetUrlParameters(_subMenuTab22);

    Assert.That(parameters, Is.Not.Null);
    Assert.That(parameters.Count, Is.EqualTo(1));
    Assert.That(parameters.GetKey(0), Is.Not.Null);
    Assert.That(parameters.GetKey(0), Is.EqualTo(_tabbedMenu.SelectionID));
    Assert.That(parameters.Get(0), Is.Not.Null);
    Assert.That(parameters.Get(0), Is.EqualTo(expectedParameterValue));
  }

  [Test]
  public void FormatUrlForMainMenuTab ()
  {
    string url = "/AppDir/page.aspx";
    string expectedParameterValue = _mainMenuTab2.ItemID;
    string expectedUrl = UrlUtility.AddParameter(url, _tabbedMenu.SelectionID, expectedParameterValue);

    string value = _tabbedMenu.FormatUrl(url, _mainMenuTab2);

    Assert.That(value, Is.Not.Null);
    Assert.That(value, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void FormatUrlForSubMenuTab ()
  {
    string url = "/AppDir/page.aspx";
    string expectedParameterValue = string.Format("{0},{1}", _subMenuTab22.Parent.ItemID, _subMenuTab22.ItemID);
    string expectedUrl = UrlUtility.AddParameter(url, _tabbedMenu.SelectionID, expectedParameterValue);

    string value = _tabbedMenu.FormatUrl(url, _subMenuTab22);

    Assert.That(value, Is.Not.Null);
    Assert.That(value, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void FormatUrlForSelectedMainMenuTab ()
  {
    string url = "/AppDir/page.aspx";
    _mainMenuTab3.IsSelected = true;
    string expectedParameterValue = _mainMenuTab3.ItemID;
    string expectedUrl = UrlUtility.AddParameter(url, _tabbedMenu.SelectionID, expectedParameterValue);

    string value = _tabbedMenu.FormatUrl(url);

    Assert.That(value, Is.Not.Null);
    Assert.That(value, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void FormatUrlForSelectedSubMenuTab ()
  {
    string url = "/AppDir/page.aspx";
    _subMenuTab12.IsSelected = true;
    string expectedParameterValue = string.Format("{0},{1}", _subMenuTab12.Parent.ItemID, _subMenuTab12.ItemID);
    string expectedUrl = UrlUtility.AddParameter(url, _tabbedMenu.SelectionID, expectedParameterValue);

    string value = _tabbedMenu.FormatUrl(url);

    Assert.That(value, Is.Not.Null);
    Assert.That(value, Is.EqualTo(expectedUrl));
  }

  [Test]
  public void StylesheetRegistrationIntegrationTest ()
  {
    NamingContainer.Controls.Add(_tabbedMenu);

    Assert.That(HtmlHeadAppender.Current, Is.Not.Null);

    NamingContainerInvoker.InitRecursive();

    var htmlHeadAppender = HtmlHeadAppender.Current;
    var registeredStyleSheetBlock = htmlHeadAppender.GetHtmlHeadElements().OfType<StyleSheetBlock>().SingleOrDefault();
    Assert.That(registeredStyleSheetBlock, Is.Not.Null);

    Assert.That(registeredStyleSheetBlock.StyleSheetElements.Count, Is.EqualTo(3));
    Assert.That(((StyleSheetImportRule)registeredStyleSheetBlock.StyleSheetElements[0]).ResourceUrl.GetUrl(), Does.EndWith("Common.css"));
    Assert.That(((StyleSheetImportRule)registeredStyleSheetBlock.StyleSheetElements[1]).ResourceUrl.GetUrl(), Does.EndWith("TabStrip.css"));
    Assert.That(((StyleSheetImportRule)registeredStyleSheetBlock.StyleSheetElements[2]).ResourceUrl.GetUrl(), Does.EndWith("TabbedMenu.css"));
  }
}

}
