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
using System.IO;
using System.Web.UI;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.Web.UnitTesting.UI.Controls;
using Remotion.ServiceLocation;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI.Controls;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.Web.UnitTests.Core.UI.Controls.WebButtonTests
{
  [TestFixture]
  public class WebButtonDiagnosticMetadataTest : BaseTest
  {
    private ServiceLocatorScope _serviceLocatorScope;

    public override void SetUp ()
    {
      base.SetUp();

      var serviceLocator = DefaultServiceLocator.Create();
      serviceLocator.RegisterSingle<IRenderingFeatures> (() => RenderingFeatures.WithDiagnosticMetadata);
      _serviceLocatorScope = new ServiceLocatorScope (serviceLocator);
    }

    public override void TearDown ()
    {
      base.TearDown();
      _serviceLocatorScope.Dispose();
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      var webButton = new TestWebButton { ID = "WebButton", Text = "My Button" };

      var renderedText = RenderControl (webButton);

      Assert.That (renderedText, Is.StringContaining (DiagnosticMetadataAttributes.ControlType + "=\"WebButton\""));
      Assert.That (renderedText, Is.StringContaining (DiagnosticMetadataAttributes.ItemID + "=\"" + webButton.ID + "\""));
      Assert.That (renderedText, Is.StringContaining (DiagnosticMetadataAttributes.Content + "=\"" + webButton.Text + "\""));
      Assert.That (renderedText, Is.Not.StringContaining (DiagnosticMetadataAttributes.CommandName));
      Assert.That (renderedText, Is.StringContaining (DiagnosticMetadataAttributes.TriggersPostBack + "=\"true\""));
    }

    [Test]
    public void RenderDiagnosticMetadataAttributesWithCommand ()
    {
      var webButton = new TestWebButton { ID = "WebButton", CommandName = "MyCommand" };

      var renderedText = RenderControl (webButton);

      Assert.That (renderedText, Is.StringContaining (DiagnosticMetadataAttributes.ControlType + "=\"WebButton\""));
      Assert.That (renderedText, Is.StringContaining (DiagnosticMetadataAttributes.CommandName + "=\"MyCommand\""));
      Assert.That (renderedText, Is.StringContaining (DiagnosticMetadataAttributes.TriggersPostBack + "=\"true\""));
    }

    private string RenderControl (Control control)
    {
      var page = new Page();
      page.Controls.Add (control);

      var ci = new ControlInvoker (page);
      ci.InitRecursive();
      ci.LoadRecursive();
      ci.PreRenderRecursive();
      var stringWriter = new StringWriter();
      control.RenderControl (new HtmlTextWriter (stringWriter));

      return stringWriter.ToString();
    }
  }
}