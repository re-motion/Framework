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
using System.Web;
using NUnit.Framework;
using Remotion.Development.Web.UnitTesting.AspNetFramework;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Remotion.Web.UnitTests.Core.UI.Controls.TabbedMultiViewImplementation;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{
  [TestFixture]
  public class TabbedMultiViewTest : WebControlTest
  {
    private HttpContext _currentHttpContext;

    private TabbedMultiView _tabbedMultiView;

    private TabView _tabView1;

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

      _tabbedMultiView = new TabbedMultiViewMock();
      _tabbedMultiView.ID = "TabbedMultiVIew";

      _tabView1 = new TabView { ID = "Tab1", Title = WebString.CreateFromText("Tab 1") };
      _tabbedMultiView.Views.Add(_tabView1);
    }

    [Test]
    public void StylesheetRegistrationIntegrationTest ()
    {
      NamingContainer.Controls.Add(_tabbedMultiView);

      Assert.That(HtmlHeadAppender.Current, Is.Not.Null);

      NamingContainerInvoker.InitRecursive();

      var htmlHeadAppender = HtmlHeadAppender.Current;
      var registeredStyleSheetBlock = htmlHeadAppender.GetHtmlHeadElements().OfType<StyleSheetBlock>().SingleOrDefault();
      Assert.That(registeredStyleSheetBlock, Is.Not.Null);

      Assert.That(registeredStyleSheetBlock.StyleSheetElements.Count, Is.EqualTo(3));
      Assert.That(((StyleSheetImportRule)registeredStyleSheetBlock.StyleSheetElements[0]).ResourceUrl.GetUrl(), Does.EndWith("Common.css"));
      Assert.That(((StyleSheetImportRule)registeredStyleSheetBlock.StyleSheetElements[1]).ResourceUrl.GetUrl(), Does.EndWith("TabStrip.css"));
      Assert.That(((StyleSheetImportRule)registeredStyleSheetBlock.StyleSheetElements[2]).ResourceUrl.GetUrl(), Does.EndWith("TabbedMultiView.css"));
    }
  }
}
