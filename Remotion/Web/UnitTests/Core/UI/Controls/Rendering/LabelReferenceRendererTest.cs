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
using NUnit.Framework;
using Remotion.Web.Contracts.DiagnosticMetadata;
using Remotion.Web.UI.Controls.Rendering;

namespace Remotion.Web.UnitTests.Core.UI.Controls.Rendering
{
  [TestFixture]
  public class LabelReferenceRendererTest
  {
    private HtmlHelper _html;

    [SetUp]
    public void SetUp ()
    {
      _html = new HtmlHelper();
    }

    [Test]
    public void SetLabelReferenceOnControl_WithLabelIDs ()
    {
      var labelReferenceRenderer = new LabelReferenceRenderer(new DefaultRenderingFeatures());
      var labelControl = new Label();

      labelReferenceRenderer.SetLabelsReferenceOnControl(
          labelControl,
          new[] { "Label1", "Label2" },
          new string[0]);

      Assert.That(labelControl.Attributes.Count, Is.EqualTo(1));
      Assert.That(labelControl.Attributes["aria-labelledby"], Is.EqualTo("Label1 Label2"));
    }

    [Test]
    public void SetLabelReferenceOnControl_WithLabelIDs_AndAccessibilityAnnotationIDs ()
    {
      var labelReferenceRenderer = new LabelReferenceRenderer(new DefaultRenderingFeatures());
      var labelControl = new Label();

      labelReferenceRenderer.SetLabelsReferenceOnControl(
          labelControl,
          new[] { "Label1", "Label2" },
          new[] { "AccessibilityAnnotation1", "AccessibilityAnnotation2" });

      Assert.That(labelControl.Attributes.Count, Is.EqualTo(1));
      Assert.That(labelControl.Attributes["aria-labelledby"], Is.EqualTo("Label1 Label2 AccessibilityAnnotation1 AccessibilityAnnotation2"));
    }

    [Test]
    public void SetLabelReferenceOnControl_WithAccessibilityAnnotationIDs ()
    {
      var labelReferenceRenderer = new LabelReferenceRenderer(new DefaultRenderingFeatures());
      var labelControl = new Label();

      labelReferenceRenderer.SetLabelsReferenceOnControl(
          labelControl,
          new string[0],
          new[] { "AccessibilityAnnotation1", "AccessibilityAnnotation2" });

      Assert.That(labelControl.Attributes.Count, Is.EqualTo(1));
      Assert.That(labelControl.Attributes["aria-labelledby"], Is.EqualTo("AccessibilityAnnotation1 AccessibilityAnnotation2"));
    }

    [Test]
    public void SetLabelReferenceOnControl_WithDiagnosticMetadataEnabled ()
    {
      var labelReferenceRenderer = new LabelReferenceRenderer(new WithDiagnosticMetadataRenderingFeatures());
      var labelControl = new Label();

      labelReferenceRenderer.SetLabelsReferenceOnControl(
          labelControl,
          new[] { "Label1", "Label2" },
          new string[0]);

      Assert.That(labelControl.Attributes.Count, Is.EqualTo(2));
      Assert.That(labelControl.Attributes["aria-labelledby"], Is.EqualTo("Label1 Label2"));
      Assert.That(labelControl.Attributes[DiagnosticMetadataAttributes.LabelIDIndex], Is.EqualTo("0 1"));
    }

    [Test]
    public void SetLabelReferenceOnControl_WithDiagnosticMetadataEnabled_AndAccessibilityAnnotationIDs ()
    {
      var labelReferenceRenderer = new LabelReferenceRenderer(new WithDiagnosticMetadataRenderingFeatures());
      var labelControl = new Label();

      labelReferenceRenderer.SetLabelsReferenceOnControl(
          labelControl,
          new[] { "Label1", "Label2" },
          new[] { "AccessibilityAnnotation1", "AccessibilityAnnotation2" });

      Assert.That(labelControl.Attributes.Count, Is.EqualTo(2));
      Assert.That(labelControl.Attributes["aria-labelledby"], Is.EqualTo("Label1 Label2 AccessibilityAnnotation1 AccessibilityAnnotation2"));
      Assert.That(labelControl.Attributes[DiagnosticMetadataAttributes.LabelIDIndex], Is.EqualTo("0 1"));
    }

    [Test]
    public void SetLabelReferenceOnControl_WithoutLabelIDs ()
    {
      var labelReferenceRenderer = new LabelReferenceRenderer(new DefaultRenderingFeatures());
      var labelControl = new Label();

      labelReferenceRenderer.SetLabelsReferenceOnControl(
          labelControl,
          new string[0],
          new string[0]);

      Assert.That(labelControl.Attributes.Count, Is.EqualTo(0));
    }

    [Test]
    public void RenderLabelReference_WithLabelIDs ()
    {
      var labelReferenceRenderer = new LabelReferenceRenderer(new DefaultRenderingFeatures());

      labelReferenceRenderer.AddLabelsReference(_html.Writer, new[] { "Label1", "Label2" }, new string[0]);
      _html.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      _html.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var span = _html.GetAssertedChildElement(document, "span", 0);
      _html.AssertAttribute(span, "aria-labelledby", "Label1 Label2");
      _html.AssertNoAttribute(span, DiagnosticMetadataAttributes.LabelIDIndex);
    }

    [Test]
    public void RenderLabelReference_WithAccessibilityAnnotationIDs ()
    {
      var labelReferenceRenderer = new LabelReferenceRenderer(new DefaultRenderingFeatures());

      labelReferenceRenderer.AddLabelsReference(
          _html.Writer,
          new[] { "Label1", "Label2" },
          new[] { "AccessibilityAnnotation1", "AccessibilityAnnotation2" });

      _html.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      _html.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var span = _html.GetAssertedChildElement(document, "span", 0);
      _html.AssertAttribute(span, "aria-labelledby", "Label1 Label2 AccessibilityAnnotation1 AccessibilityAnnotation2");
      _html.AssertNoAttribute(span, DiagnosticMetadataAttributes.LabelIDIndex);
    }

    [Test]
    public void RenderLabelReference_WithOnlyAccessibilityAnnotationIDs ()
    {
      var labelReferenceRenderer = new LabelReferenceRenderer(new DefaultRenderingFeatures());

      labelReferenceRenderer.AddLabelsReference(
          _html.Writer,
          new string[0],
          new[] { "AccessibilityAnnotation1", "AccessibilityAnnotation2" });

      _html.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      _html.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var span = _html.GetAssertedChildElement(document, "span", 0);
      _html.AssertAttribute(span, "aria-labelledby", "AccessibilityAnnotation1 AccessibilityAnnotation2");
      _html.AssertNoAttribute(span, DiagnosticMetadataAttributes.LabelIDIndex);
    }

    [Test]
    public void RenderLabelReference_WithDiagnosticMetadataEnabled ()
    {
      var labelReferenceRenderer = new LabelReferenceRenderer(new WithDiagnosticMetadataRenderingFeatures());

      labelReferenceRenderer.AddLabelsReference(
          _html.Writer,
          new[] { "Label1", "Label2" },
          new string[0]);

      _html.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      _html.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var span = _html.GetAssertedChildElement(document, "span", 0);
      _html.AssertAttribute(span, "aria-labelledby", "Label1 Label2");
      _html.AssertAttribute(span, DiagnosticMetadataAttributes.LabelIDIndex, "0 1");
    }

    [Test]
    public void SetLabelReferenceOnControl_WithDiagnosticMetadataEnabled_AndOnlyAccessibilityAnnotationIDs ()
    {
      var labelReferenceRenderer = new LabelReferenceRenderer(new WithDiagnosticMetadataRenderingFeatures());
      var labelControl = new Label();

      labelReferenceRenderer.SetLabelsReferenceOnControl(
          labelControl,
          new string[0],
          new[] { "AccessibilityAnnotation1", "AccessibilityAnnotation2" });

      Assert.That(labelControl.Attributes.Count, Is.EqualTo(2));
      Assert.That(labelControl.Attributes["aria-labelledby"], Is.EqualTo("AccessibilityAnnotation1 AccessibilityAnnotation2"));
      Assert.That(labelControl.Attributes[DiagnosticMetadataAttributes.LabelIDIndex], Is.EqualTo(""));
    }

    [Test]
    public void RenderLabelReference_WithDiagnosticMetadataEnabled_AndAccessibilityAnnotationIDs ()
    {
      var labelReferenceRenderer = new LabelReferenceRenderer(new WithDiagnosticMetadataRenderingFeatures());

      labelReferenceRenderer.AddLabelsReference(
          _html.Writer,
          new[] { "Label1", "Label2" },
          new[] { "AccessibilityAnnotation1", "AccessibilityAnnotation2" });

      _html.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      _html.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var span = _html.GetAssertedChildElement(document, "span", 0);
      _html.AssertAttribute(span, "aria-labelledby", "Label1 Label2 AccessibilityAnnotation1 AccessibilityAnnotation2");
      _html.AssertAttribute(span, DiagnosticMetadataAttributes.LabelIDIndex, "0 1");
    }

    [Test]
    public void RenderLabelReference_WithDiagnosticMetadataEnabled_AndOnlyAccessibilityAnnotationIDs ()
    {
      var labelReferenceRenderer = new LabelReferenceRenderer(new WithDiagnosticMetadataRenderingFeatures());

      labelReferenceRenderer.AddLabelsReference(
          _html.Writer,
          new string[0],
          new[] { "AccessibilityAnnotation1", "AccessibilityAnnotation2" });

      _html.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      _html.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var span = _html.GetAssertedChildElement(document, "span", 0);
      _html.AssertAttribute(span, "aria-labelledby", "AccessibilityAnnotation1 AccessibilityAnnotation2");
      _html.AssertAttribute(span, DiagnosticMetadataAttributes.LabelIDIndex, "");
    }

    [Test]
    public void RenderLabelReference_WithoutLabelIDs ()
    {
      var labelReferenceRenderer = new LabelReferenceRenderer(new DefaultRenderingFeatures());

      labelReferenceRenderer.AddLabelsReference(
          _html.Writer,
          new string[0],
          new string[0]);

      _html.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      _html.Writer.RenderEndTag();

      var document = _html.GetResultDocument();
      var span = _html.GetAssertedChildElement(document, "span", 0);
      _html.AssertNoAttribute(span, "aria-labelledby");
      _html.AssertNoAttribute(span, DiagnosticMetadataAttributes.LabelIDIndex);
    }
  }
}
