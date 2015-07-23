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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Remotion.Web.Resources;
using Remotion.Web.UI;
using Remotion.Web.UI.Controls;
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI
{
  [TestFixture]
  public class HtmlHeadAppenderTest
  {
    private HtmlHeadAppender _htmlHeadAppender;

    [SetUp]
    public void SetUp ()
    {
      _htmlHeadAppender = (HtmlHeadAppender) Activator.CreateInstance (typeof (HtmlHeadAppender), nonPublic: true);
    }

    [Test]
    public void SetTitle ()
    {
      _htmlHeadAppender.SetTitle ("The Title");

      var htmlHeadElements = _htmlHeadAppender.GetHtmlHeadElements().ToArray();

      Assert.That (htmlHeadElements.Length, Is.EqualTo (1));

      Assert.That (htmlHeadElements[0], Is.InstanceOf (typeof (TitleTag)));
      var titleTag = (TitleTag) htmlHeadElements[0];
      Assert.That (titleTag.Title, Is.EqualTo ("The Title"));
    }

    [Test]
    public void SetTitle_Twice ()
    {
      _htmlHeadAppender.SetTitle ("The Title1");
      _htmlHeadAppender.SetTitle ("The Title2");

      var htmlHeadElements = _htmlHeadAppender.GetHtmlHeadElements().ToArray();

      Assert.That (htmlHeadElements.Length, Is.EqualTo (1));

      var titleTag = (TitleTag) htmlHeadElements[0];
      Assert.That (titleTag.Title, Is.EqualTo ("The Title2"));
    }

    [Test]
    public void RegisterJavaScriptInclude_WithResourceUrl ()
    {
      IResourceUrl resourceUrl = new StaticResourceUrl ("url.js");
      _htmlHeadAppender.RegisterJavaScriptInclude ("key", resourceUrl);

      var htmlHeadElements = _htmlHeadAppender.GetHtmlHeadElements().ToArray();

      Assert.That (htmlHeadElements.Length, Is.EqualTo (1));
      Assert.That (htmlHeadElements[0], Is.InstanceOf (typeof (JavaScriptInclude)));
      Assert.That (((JavaScriptInclude) htmlHeadElements[0]).ResourceUrl, Is.SameAs (resourceUrl));
    }

    [Test]
    public void RegisterJavaScriptInclude_WithString ()
    {
      _htmlHeadAppender.RegisterJavaScriptInclude ("key", "url.js");

      var htmlHeadElements = _htmlHeadAppender.GetHtmlHeadElements().ToArray();

      Assert.That (htmlHeadElements.Length, Is.EqualTo (1));
      Assert.That (htmlHeadElements[0], Is.InstanceOf (typeof (JavaScriptInclude)));
      Assert.That (((JavaScriptInclude) htmlHeadElements[0]).ResourceUrl.GetUrl(), Is.EqualTo ("url.js"));
    }

    [Test]
    public void RegisterStylesheetLink_WithResourceUrl ()
    {
      IResourceUrl resourceUrl = new StaticResourceUrl ("url.css");
      _htmlHeadAppender.RegisterStylesheetLink ("key", resourceUrl);

      var htmlHeadElements = _htmlHeadAppender.GetHtmlHeadElements().ToArray();

      Assert.That (htmlHeadElements.Length, Is.EqualTo (1));
      Assert.That (htmlHeadElements[0], Is.InstanceOf (typeof (StyleSheetBlock)));
      var styleSheetBlock = (StyleSheetBlock) htmlHeadElements[0];
      Assert.That (styleSheetBlock.StyleSheetElements.Count, Is.EqualTo (1));
      Assert.That (styleSheetBlock.StyleSheetElements[0], Is.InstanceOf (typeof (StyleSheetImportRule)));
      Assert.That (((StyleSheetImportRule) styleSheetBlock.StyleSheetElements[0]).ResourceUrl, Is.SameAs (resourceUrl));
    }

    [Test]
    public void RegisterStylesheetLink_WithString ()
    {
      _htmlHeadAppender.RegisterStylesheetLink ("key", "url.css");

      var htmlHeadElements = _htmlHeadAppender.GetHtmlHeadElements().ToArray();

      Assert.That (htmlHeadElements.Length, Is.EqualTo (1));
      Assert.That (htmlHeadElements[0], Is.InstanceOf (typeof (StyleSheetBlock)));
      var styleSheetBlock = (StyleSheetBlock) htmlHeadElements[0];
      Assert.That (styleSheetBlock.StyleSheetElements.Count, Is.EqualTo (1));
      Assert.That (styleSheetBlock.StyleSheetElements[0], Is.InstanceOf (typeof (StyleSheetImportRule)));
      Assert.That (((StyleSheetImportRule) styleSheetBlock.StyleSheetElements[0]).ResourceUrl.GetUrl(), Is.EqualTo ("url.css"));
    }

    [Test]
    public void RegisterStylesheetLink_WithResourceUrl_AndPriority ()
    {
      IResourceUrl resourceUrl2 = new StaticResourceUrl ("url2.css");
      _htmlHeadAppender.RegisterStylesheetLink ("key2", resourceUrl2, HtmlHeadAppender.Priority.UserControl);
      IResourceUrl resourceUrl1 = new StaticResourceUrl ("url1.css");
      _htmlHeadAppender.RegisterStylesheetLink ("key1", resourceUrl1, HtmlHeadAppender.Priority.Library);

      var htmlHeadElements = _htmlHeadAppender.GetHtmlHeadElements().ToArray();

      Assert.That (htmlHeadElements.Length, Is.EqualTo (2));

      var styleSheetBlock1 = (StyleSheetBlock) htmlHeadElements[0];
      Assert.That (styleSheetBlock1.StyleSheetElements.Count, Is.EqualTo (1));
      Assert.That (((StyleSheetImportRule) styleSheetBlock1.StyleSheetElements[0]).ResourceUrl, Is.SameAs (resourceUrl1));

      var styleSheetBlock2 = (StyleSheetBlock) htmlHeadElements[1];
      Assert.That (styleSheetBlock2.StyleSheetElements.Count, Is.EqualTo (1));
      Assert.That (((StyleSheetImportRule) styleSheetBlock2.StyleSheetElements[0]).ResourceUrl, Is.SameAs (resourceUrl2));
    }

    [Test]
    public void RegisterStylesheetLink_WithString_AndPriority ()
    {
      _htmlHeadAppender.RegisterStylesheetLink ("key2", "url2.css", HtmlHeadAppender.Priority.UserControl);
      _htmlHeadAppender.RegisterStylesheetLink ("key1", "url1.css", HtmlHeadAppender.Priority.Library);

      var htmlHeadElements = _htmlHeadAppender.GetHtmlHeadElements().ToArray();

      Assert.That (htmlHeadElements.Length, Is.EqualTo (2));

      var styleSheetBlock1 = (StyleSheetBlock) htmlHeadElements[0];
      Assert.That (((StyleSheetImportRule) styleSheetBlock1.StyleSheetElements[0]).ResourceUrl.GetUrl(), Is.EqualTo ("url1.css"));

      var styleSheetBlock2 = (StyleSheetBlock) htmlHeadElements[1];
      Assert.That (((StyleSheetImportRule) styleSheetBlock2.StyleSheetElements[0]).ResourceUrl.GetUrl(), Is.EqualTo ("url2.css"));
    }

    [Test]
    public void GetHtmlHeadElements_GroupsStyleSheetsIntoBlocksOf31ImportRules ()
    {
      List<IResourceUrl> libraryUrls = new List<IResourceUrl>();
      List<IResourceUrl> userControlUrls = new List<IResourceUrl>();
      for (int i = 0; i < 40; i++)
      {
        IResourceUrl libraryUrl = new StaticResourceUrl (string.Format ("Library{0}.css", i));
        _htmlHeadAppender.RegisterStylesheetLink (string.Format ("Library{0}.css", i), libraryUrl, HtmlHeadAppender.Priority.Library);
        libraryUrls.Add (libraryUrl);

        IResourceUrl userControlUrl = new StaticResourceUrl (string.Format ("UserControl{0}.css", i));
        _htmlHeadAppender.RegisterStylesheetLink (string.Format ("UserControl{0}.css", i), userControlUrl, HtmlHeadAppender.Priority.UserControl);
        userControlUrls.Add (userControlUrl);
      }

      var htmlHeadElements = _htmlHeadAppender.GetHtmlHeadElements().ToArray();

      Assert.That (htmlHeadElements.Length, Is.EqualTo (4));

      var libraryBlock1 = (StyleSheetBlock) htmlHeadElements[0];
      Assert.That (libraryBlock1.StyleSheetElements.Count, Is.EqualTo (31));
      Assert.That (((StyleSheetImportRule) libraryBlock1.StyleSheetElements[0]).ResourceUrl, Is.SameAs (libraryUrls[0]));
      Assert.That (((StyleSheetImportRule) libraryBlock1.StyleSheetElements[30]).ResourceUrl, Is.SameAs (libraryUrls[30]));

      var libraryBlock2 = (StyleSheetBlock) htmlHeadElements[1];
      Assert.That (libraryBlock2.StyleSheetElements.Count, Is.EqualTo (9));
      Assert.That (((StyleSheetImportRule) libraryBlock2.StyleSheetElements[0]).ResourceUrl, Is.SameAs (libraryUrls[31]));
      Assert.That (((StyleSheetImportRule) libraryBlock2.StyleSheetElements[8]).ResourceUrl, Is.SameAs (libraryUrls[39]));

      var userControlBlock1 = (StyleSheetBlock) htmlHeadElements[2];
      Assert.That (userControlBlock1.StyleSheetElements.Count, Is.EqualTo (31));
      Assert.That (((StyleSheetImportRule) userControlBlock1.StyleSheetElements[0]).ResourceUrl, Is.SameAs (userControlUrls[0]));
      Assert.That (((StyleSheetImportRule) userControlBlock1.StyleSheetElements[30]).ResourceUrl, Is.SameAs (userControlUrls[30]));

      var userControlBlock2 = (StyleSheetBlock) htmlHeadElements[3];
      Assert.That (userControlBlock2.StyleSheetElements.Count, Is.EqualTo (9));
      Assert.That (((StyleSheetImportRule) userControlBlock2.StyleSheetElements[0]).ResourceUrl, Is.SameAs (userControlUrls[31]));
      Assert.That (((StyleSheetImportRule) userControlBlock2.StyleSheetElements[8]).ResourceUrl, Is.SameAs (userControlUrls[39]));
    }

    [Test]
    public void GetHtmlHeadElements_PlacesStyleSheetImportRulesBeforeStyleSheetElements ()
    {
      var libraryElement1 = MockRepository.GenerateStub<StyleSheetElement>();
      _htmlHeadAppender.RegisterHeadElement ("libraryElement1", libraryElement1, HtmlHeadAppender.Priority.Library);

      var libraryRule1 = new StyleSheetImportRule (new StaticResourceUrl ("url.css"));
      _htmlHeadAppender.RegisterHeadElement ("libraryRule1", libraryRule1, HtmlHeadAppender.Priority.Library);

      var libraryElement2 = MockRepository.GenerateStub<StyleSheetElement>();
      _htmlHeadAppender.RegisterHeadElement ("libraryElement2", libraryElement2, HtmlHeadAppender.Priority.Library);

      var libraryRule2 = new StyleSheetImportRule (new StaticResourceUrl ("url.css"));
      _htmlHeadAppender.RegisterHeadElement ("libraryRule2", libraryRule2, HtmlHeadAppender.Priority.Library);

      var libraryElement3 = MockRepository.GenerateStub<StyleSheetElement>();
      _htmlHeadAppender.RegisterHeadElement ("libraryElement3", libraryElement3, HtmlHeadAppender.Priority.Library);

      var userControlRule1 = new StyleSheetImportRule (new StaticResourceUrl ("url.css"));
      _htmlHeadAppender.RegisterHeadElement ("userControlRule1", userControlRule1, HtmlHeadAppender.Priority.UserControl);

      var userControlElement1 = MockRepository.GenerateStub<StyleSheetElement>();
      _htmlHeadAppender.RegisterHeadElement ("userControlElement1", userControlElement1, HtmlHeadAppender.Priority.UserControl);

      var htmlHeadElements = _htmlHeadAppender.GetHtmlHeadElements().ToArray();

      Assert.That (htmlHeadElements.Length, Is.EqualTo (4));

      var libraryBlock1 = (StyleSheetBlock) htmlHeadElements[0];
      Assert.That (libraryBlock1.StyleSheetElements.Count, Is.EqualTo (2));
      Assert.That (libraryBlock1.StyleSheetElements[0], Is.SameAs (libraryRule1));
      Assert.That (libraryBlock1.StyleSheetElements[1], Is.SameAs (libraryRule2));

      var libraryBlock2 = (StyleSheetBlock) htmlHeadElements[1];
      Assert.That (libraryBlock2.StyleSheetElements.Count, Is.EqualTo (3));
      Assert.That (libraryBlock2.StyleSheetElements[0], Is.SameAs (libraryElement1));
      Assert.That (libraryBlock2.StyleSheetElements[1], Is.SameAs (libraryElement2));
      Assert.That (libraryBlock2.StyleSheetElements[2], Is.SameAs (libraryElement3));

      var userControlBlock1 = (StyleSheetBlock) htmlHeadElements[2];
      Assert.That (userControlBlock1.StyleSheetElements.Count, Is.EqualTo (1));
      Assert.That (userControlBlock1.StyleSheetElements[0], Is.SameAs (userControlRule1));

      var userControlBlock2 = (StyleSheetBlock) htmlHeadElements[3];
      Assert.That (userControlBlock2.StyleSheetElements.Count, Is.EqualTo (1));
      Assert.That (userControlBlock2.StyleSheetElements[0], Is.SameAs (userControlElement1));
    }

    [Test]
    public void GetHtmlHeadElements_PlacesTitleFirst ()
    {
      var element1 = MockRepository.GenerateStub<HtmlHeadElement>();
      _htmlHeadAppender.RegisterHeadElement ("element1", element1, HtmlHeadAppender.Priority.Library);

      _htmlHeadAppender.SetTitle ("The Title");

      var element2 = MockRepository.GenerateStub<HtmlHeadElement>();
      _htmlHeadAppender.RegisterHeadElement ("element2", element2, HtmlHeadAppender.Priority.Library);

      var htmlHeadElements = _htmlHeadAppender.GetHtmlHeadElements().ToArray();

      Assert.That (htmlHeadElements.Length, Is.EqualTo (3));

      Assert.That (htmlHeadElements[0], Is.InstanceOf (typeof (TitleTag)));
      Assert.That (htmlHeadElements[1], Is.SameAs (element1));
      Assert.That (htmlHeadElements[2], Is.SameAs (element2));
    }

    [Test]
    public void RegisterHeadElement ()
    {
      var userControlElement = MockRepository.GenerateStub<HtmlHeadElement>();
      _htmlHeadAppender.RegisterHeadElement ("userControl", userControlElement, HtmlHeadAppender.Priority.UserControl);

      var libraryElement = MockRepository.GenerateStub<HtmlHeadElement>();
      _htmlHeadAppender.RegisterHeadElement ("library", libraryElement, HtmlHeadAppender.Priority.Library);

      var htmlHeadElements = _htmlHeadAppender.GetHtmlHeadElements().ToArray();

      Assert.That (htmlHeadElements.Length, Is.EqualTo (2));

      Assert.That (htmlHeadElements[0], Is.SameAs (libraryElement));
      Assert.That (htmlHeadElements[1], Is.SameAs (userControlElement));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "RegisterHeadElement must not be called after SetAppended has been called.")]
    public void RegisterHeadElement_AfterHasAppended ()
    {
      _htmlHeadAppender.SetAppended();
      var userControlElement = MockRepository.GenerateStub<HtmlHeadElement>();

      _htmlHeadAppender.RegisterHeadElement ("userControl", userControlElement, HtmlHeadAppender.Priority.UserControl);
    }
  }
}