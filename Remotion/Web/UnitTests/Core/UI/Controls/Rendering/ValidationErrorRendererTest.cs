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
  public class ValidationErrorRendererTest
  {
    private HtmlHelper _html;

    [SetUp]
    public void SetUp ()
    {
      _html = new HtmlHelper();
    }

    [Test]
    public void SetValidationErrorReferenceOnControl ()
    {
      var validationErrorRenderer = new ValidationErrorRenderer(new DefaultRenderingFeatures());
      var labelControl = new Label();

      validationErrorRenderer.SetValidationErrorsReferenceOnControl(labelControl, "ValidationErrorID", new[] { "ValidationError1"});

      Assert.That(labelControl.Attributes.Count, Is.EqualTo(2));
      Assert.That(labelControl.Attributes["aria-describedby"], Is.EqualTo("ValidationErrorID"));
      Assert.That(labelControl.Attributes["aria-invalid"], Is.EqualTo("true"));
    }

    [Test]
    public void SetValidationErrorReferenceOnControl_WithNoValidationErrors ()
    {
      var validationErrorRenderer = new ValidationErrorRenderer(new DefaultRenderingFeatures());
      var labelControl = new Label();

      validationErrorRenderer.SetValidationErrorsReferenceOnControl(labelControl, "ValidationErrorID", new string[0]);

      Assert.That(labelControl.Attributes.Count, Is.EqualTo(0));
    }

    [Test]
    public void SetValidationErrorReferenceOnControl_WithDescribedByAlreadySet ()
    {
      var validationErrorRenderer = new ValidationErrorRenderer(new DefaultRenderingFeatures());
      var labelControl = new Label();

      labelControl.Attributes["aria-describedby"] = "DescribedByValue";

      validationErrorRenderer.SetValidationErrorsReferenceOnControl(labelControl, "ValidationErrorID", new[] { "ValidationError1"});

      Assert.That(labelControl.Attributes.Count, Is.EqualTo(2));
      Assert.That(labelControl.Attributes["aria-describedby"], Is.EqualTo("DescribedByValue ValidationErrorID"));
      Assert.That(labelControl.Attributes["aria-invalid"], Is.EqualTo("true"));
    }

    [Test]
    public void SetValidationErrorReferenceOnControl_WithDiagnosticMetadata ()
    {
      var validationErrorRenderer = new ValidationErrorRenderer(new WithDiagnosticMetadataRenderingFeatures());
      var labelControl = new Label();

      validationErrorRenderer.SetValidationErrorsReferenceOnControl(labelControl, "ValidationErrorID", new[] { "ValidationError1"});

      Assert.That(labelControl.Attributes.Count, Is.EqualTo(3));
      Assert.That(labelControl.Attributes["aria-describedby"], Is.EqualTo("ValidationErrorID"));
      Assert.That(labelControl.Attributes["aria-invalid"], Is.EqualTo("true"));
      Assert.That(labelControl.Attributes[DiagnosticMetadataAttributes.ValidationErrorIDIndex], Is.EqualTo("0"));
    }

    [Test]
    public void SetValidationErrorReferenceOnControl_WithDiagnosticMetadata_AndMultipleDescribeBys ()
    {
      var validationErrorRenderer = new ValidationErrorRenderer(new WithDiagnosticMetadataRenderingFeatures());
      var labelControl = new Label();

      labelControl.Attributes["aria-describedby"] = "DescribedByValue1 DescribedByValue2";


      validationErrorRenderer.SetValidationErrorsReferenceOnControl(labelControl, "ValidationErrorID", new[] { "ValidationError1"});

      Assert.That(labelControl.Attributes.Count, Is.EqualTo(3));
      Assert.That(labelControl.Attributes["aria-describedby"], Is.EqualTo("DescribedByValue1 DescribedByValue2 ValidationErrorID"));
      Assert.That(labelControl.Attributes["aria-invalid"], Is.EqualTo("true"));
      Assert.That(labelControl.Attributes[DiagnosticMetadataAttributes.ValidationErrorIDIndex], Is.EqualTo("2"));
    }

    [Test]
    public void AddValidationErrorReference ()
    {
      var validationErrorRenderer = new ValidationErrorRenderer(new DefaultRenderingFeatures());
      var labelControl = new Label();

      validationErrorRenderer.AddValidationErrorsReference(labelControl.Attributes, "ValidationErrorID", new[] { "ValidationError1"});

      Assert.That(labelControl.Attributes.Count, Is.EqualTo(2));
      Assert.That(labelControl.Attributes["aria-describedby"], Is.EqualTo("ValidationErrorID"));
      Assert.That(labelControl.Attributes["aria-invalid"], Is.EqualTo("true"));
    }

    [Test]
    public void AddValidationErrorReference_WithNoValidationErrors ()
    {
      var validationErrorRenderer = new ValidationErrorRenderer(new DefaultRenderingFeatures());
      var labelControl = new Label();

      validationErrorRenderer.AddValidationErrorsReference(labelControl.Attributes, "ValidationErrorID", new string[0]);

      Assert.That(labelControl.Attributes.Count, Is.EqualTo(0));
    }

    [Test]
    public void AddValidationErrorReference_WithDescribedByAlreadySet ()
    {
      var validationErrorRenderer = new ValidationErrorRenderer(new DefaultRenderingFeatures());
      var labelControl = new Label();

      labelControl.Attributes["aria-describedby"] = "DescribedByValue";

      validationErrorRenderer.AddValidationErrorsReference(labelControl.Attributes, "ValidationErrorID", new[] { "ValidationError1"});

      Assert.That(labelControl.Attributes.Count, Is.EqualTo(2));
      Assert.That(labelControl.Attributes["aria-describedby"], Is.EqualTo("DescribedByValue ValidationErrorID"));
      Assert.That(labelControl.Attributes["aria-invalid"], Is.EqualTo("true"));
    }

    [Test]
    public void AddValidationErrorReference_WithDiagnosticMetadata ()
    {
      var validationErrorRenderer = new ValidationErrorRenderer(new WithDiagnosticMetadataRenderingFeatures());
      var labelControl = new Label();

      validationErrorRenderer.AddValidationErrorsReference(labelControl.Attributes, "ValidationErrorID", new[] { "ValidationError1"});

      Assert.That(labelControl.Attributes.Count, Is.EqualTo(3));
      Assert.That(labelControl.Attributes["aria-describedby"], Is.EqualTo("ValidationErrorID"));
      Assert.That(labelControl.Attributes["aria-invalid"], Is.EqualTo("true"));
      Assert.That(labelControl.Attributes[DiagnosticMetadataAttributes.ValidationErrorIDIndex], Is.EqualTo("0"));
    }

    [Test]
    public void AddValidationErrorReference_WithDiagnosticMetadata_AndMultipleDescribeBys ()
    {
      var validationErrorRenderer = new ValidationErrorRenderer(new WithDiagnosticMetadataRenderingFeatures());
      var labelControl = new Label();

      labelControl.Attributes["aria-describedby"] = "DescribedByValue1 DescribedByValue2";


      validationErrorRenderer.AddValidationErrorsReference(labelControl.Attributes, "ValidationErrorID", new[] { "ValidationError1"});

      Assert.That(labelControl.Attributes.Count, Is.EqualTo(3));
      Assert.That(labelControl.Attributes["aria-describedby"], Is.EqualTo("DescribedByValue1 DescribedByValue2 ValidationErrorID"));
      Assert.That(labelControl.Attributes["aria-invalid"], Is.EqualTo("true"));
      Assert.That(labelControl.Attributes[DiagnosticMetadataAttributes.ValidationErrorIDIndex], Is.EqualTo("2"));
    }

    [Test]
    public void RenderValidationError ()
    {
      var validationErrorRenderer = new ValidationErrorRenderer(new DefaultRenderingFeatures());

      validationErrorRenderer.RenderValidationErrors(
          _html.Writer,
          "ValidationErrorID",
          new[] { "ValidationError1", "ValidationError2" });

      var document = _html.GetResultDocument();
      var span = _html.GetAssertedChildElement(document, "span", 0);

      
      _html.AssertAttribute(span, "id", "ValidationErrorID");
      _html.AssertAttribute(span, "hidden", "hidden");
      Assert.That(span.InnerXml, Is.EqualTo("ValidationError1<br />ValidationError2<br />"));
    }

    [Test]
    public void RenderValidationError_NoValidationErrors ()
    {
      var validationErrorRenderer = new ValidationErrorRenderer(new DefaultRenderingFeatures());

      validationErrorRenderer.RenderValidationErrors(
          _html.Writer,
          "ValidationErrorID",
          new string[0]);

      // We cannot assert that no element got rendered, because _html.GetResultDocument() would throw an exception.
      // Therefore we render a single span and assert that only that span exists.
      _html.Writer.RenderBeginTag(HtmlTextWriterTag.Span);
      _html.Writer.RenderEndTag();

      var document = _html.GetDocumentText();
      Assert.That(document, Is.EqualTo("<span></span>"));
    }

  }
}
