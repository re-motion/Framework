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
      serviceLocator.RegisterSingle<IRenderingFeatures>(() => RenderingFeatures.WithDiagnosticMetadata);
      _serviceLocatorScope = new ServiceLocatorScope(serviceLocator);
    }

    public override void TearDown ()
    {
      base.TearDown();
      _serviceLocatorScope.Dispose();
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes ()
    {
      var webButton = new WebButton { ID = "WebButton", Text = WebString.CreateFromText("My\nButton") };

      var renderedText = RenderControl(webButton);

      Assert.That(renderedText, Does.Contain(DiagnosticMetadataAttributes.ControlType + "=\"WebButton\""));
      Assert.That(renderedText, Does.Contain(DiagnosticMetadataAttributes.ItemID + "=\"" + webButton.ID + "\""));
      Assert.That(renderedText, Does.Contain(DiagnosticMetadataAttributes.Content + "=\"My\nButton\""));
      Assert.That(renderedText, Does.Not.Contains(DiagnosticMetadataAttributes.CommandName));
      Assert.That(renderedText, Does.Contain(DiagnosticMetadataAttributes.TriggersPostBack + "=\"true\""));
    }

    [Test]
    public void RenderDiagnosticMetadataAttributes_WithHtmlText_StripsTagsInContent ()
    {
      var webButton = new WebButton { ID = "WebButton", Text = WebString.CreateFromHtml("<p>My Button</p>") };

      var renderedText = RenderControl(webButton);

      Assert.That(renderedText, Does.Contain(DiagnosticMetadataAttributes.Content + "=\"My Button\""));
    }

    [Test]
    public void RenderDiagnosticMetadataAttributesWithCommand ()
    {
      var webButton = new WebButton { ID = "WebButton", CommandName = "MyCommand" };

      var renderedText = RenderControl(webButton);

      Assert.That(renderedText, Does.Contain(DiagnosticMetadataAttributes.ControlType + "=\"WebButton\""));
      Assert.That(renderedText, Does.Contain(DiagnosticMetadataAttributes.CommandName + "=\"MyCommand\""));
      Assert.That(renderedText, Does.Contain(DiagnosticMetadataAttributes.TriggersPostBack + "=\"true\""));
    }

    private string RenderControl (Control control)
    {
      var page = new Page();
      page.Controls.Add(control);

      var ci = new ControlInvoker(page);
      ci.InitRecursive();
      ci.LoadRecursive();
      ci.PreRenderRecursive();
      var stringWriter = new StringWriter();
      control.RenderControl(new HtmlTextWriter(stringWriter));

      return stringWriter.ToString();
    }
  }
}
