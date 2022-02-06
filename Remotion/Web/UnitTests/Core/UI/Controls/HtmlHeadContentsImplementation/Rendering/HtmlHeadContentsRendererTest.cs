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
using System.Web;
using Moq;
using NUnit.Framework;
using Remotion.Web.Resources;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.HtmlHeadContentsImplementation;
using Remotion.Web.UI.Controls.HtmlHeadContentsImplementation.Rendering;

namespace Remotion.Web.UnitTests.Core.UI.Controls.HtmlHeadContentsImplementation.Rendering
{
  [TestFixture]
  public class HtmlHeadContentsRendererTest
  {
    [Test]
    public void Render_SortsWellknownElementsByType ()
    {
      var htmlHeadElements = new List<HtmlHeadElement>
                             {
                                 new StubStyleSheetElement("stylesheet-1"),
                                 new JavaScriptInclude(new StaticResourceUrl("javscript-1")),
                                 new TitleTag(WebString.CreateFromText("title")),
                                 new StubHtmlHeadElement("head-1"),
                                 new StubStyleSheetElement("stylesheet-2"),
                                 new JavaScriptInclude(new StaticResourceUrl("javscript-2")),
                                 new StubHtmlHeadElement("head-2"),
                             };
      var renderer = new HtmlHeadContentsRenderer();
      var htmlHelper = new HtmlHelper();

      var htmlHeadContentsRenderingContext = new HtmlHeadContentsRenderingContext(
          new Mock<HttpContextBase>().Object,
          htmlHelper.Writer,
          new Mock<IHtmlHeadContents>().Object,
          htmlHeadElements);

      renderer.Render(htmlHeadContentsRenderingContext);

      var content = htmlHelper.GetDocumentText();

      Assert.That(content, Is.EqualTo(@"<title>
	title
</title>
<script src=""javscript-1"" type=""text/javascript""></script>
<script src=""javscript-2"" type=""text/javascript""></script>
stylesheet-1
stylesheet-2
head-1
head-2
"));
    }

    [Test]
    public void Render_TakesOnlyFirstTitleElement ()
    {
      var htmlHeadElements = new List<HtmlHeadElement>
                             {
                                 new TitleTag(WebString.CreateFromText("title-1")),
                                 new TitleTag(WebString.CreateFromText("title-2")),
                             };
      var renderer = new HtmlHeadContentsRenderer();
      var htmlHelper = new HtmlHelper();

      var htmlHeadContentsRenderingContext = new HtmlHeadContentsRenderingContext(
          new Mock<HttpContextBase>().Object,
          htmlHelper.Writer,
          new Mock<IHtmlHeadContents>().Object,
          htmlHeadElements);

      renderer.Render(htmlHeadContentsRenderingContext);

      var content = htmlHelper.GetDocumentText();

      Assert.That(content, Is.EqualTo(@"<title>
	title-1
</title>
"));
    }
  }
}
