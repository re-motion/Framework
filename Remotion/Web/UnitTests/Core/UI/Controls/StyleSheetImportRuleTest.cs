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
using Rhino.Mocks;

namespace Remotion.Web.UnitTests.Core.UI.Controls
{
  [TestFixture]
  public class StyleSheetImportRuleTest
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
      IResourceUrl resourceUrl = MockRepository.GenerateStub<IResourceUrl>();
      resourceUrl.Stub (stub => stub.GetUrl()).Return ("myStylesheetUrl.js");

      var javaScriptInclude = new StyleSheetImportRule (resourceUrl);

      javaScriptInclude.Render (_htmlHelper.Writer);

      Assert.That (_htmlHelper.GetDocumentText(), Is.EqualTo ("@import url(\"myStylesheetUrl.js\");\r\n"));
    }
  }
}