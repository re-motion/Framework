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
using NUnit.Framework;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{
  [TestFixture]
  public class TitleTagTest
  {
    private HtmlHelper _htmlHelper;

    [SetUp]
    public void SetUp ()
    {
      _htmlHelper = new HtmlHelper();
    }

    [Test]
    public void Render ()
    {
      var titleTag = new TitleTag ("My Title");

      titleTag.Render (_htmlHelper.Writer);

      var document = _htmlHelper.GetResultDocument();
      var titleElement = _htmlHelper.GetAssertedChildElement (document, "title", 0);
      _htmlHelper.AssertTextNode (titleElement, "My Title", 0);
    }

    [Test]
    public void Render_UsesHtmlEncoding ()
    {
      var titleTag = new TitleTag ("My <Title>");

      titleTag.Render (_htmlHelper.Writer);

      var documentText = _htmlHelper.GetDocumentText();
      Assert.That (documentText, Is.StringContaining("My &lt;Title&gt;"));
    }
  }
}