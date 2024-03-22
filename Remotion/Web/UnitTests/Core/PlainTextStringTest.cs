using System;
using System.IO;
using System.Web.UI;
using NUnit.Framework;

namespace Remotion.Web.UnitTests.Core
{
  [TestFixture]
  public class PlainTextStringTest
  {
    [Test]
    public void CreateFromText ()
    {
      var stringValue = "aoe   \" & ' < > é \r \n \r\n";
      var plainTextString = PlainTextString.CreateFromText(stringValue);

      Assert.That(plainTextString.GetValue(), Is.EqualTo(stringValue));
    }

    [Test]
    public void ExplicitConversion_FromString ()
    {
      var stringValue = "aoe   \" & ' < > é \r \n \r\n";
      var plainTextString = PlainTextString.CreateFromText(stringValue);

      Assert.That(plainTextString.GetValue(), Is.EqualTo(stringValue));
    }

    [Test]
    public void ImplicitConversion_ToWebString ()
    {
      var stringValue = "aoe   \" & ' < > é \r \n \r\n";
      var plainTextString = PlainTextString.CreateFromText(stringValue);
      WebString webString = plainTextString;

      Assert.That(webString.Type, Is.EqualTo(WebStringType.PlainText));
      Assert.That(webString.GetValue(), Is.EqualTo(stringValue));
    }

    [Test]
    public void Empty ()
    {
      Assert.That(PlainTextString.Empty.IsEmpty, Is.True);
      Assert.That(PlainTextString.Empty.GetValue(), Is.EqualTo(string.Empty));
    }

    [Test]
    public void Equality_WithNullValue_IsSameAsEmptyString ()
    {
      Assert.That(PlainTextString.CreateFromText(null), Is.EqualTo(PlainTextString.Empty));
    }

    [Test]
    public void IsEmpty ()
    {
      Assert.That(new PlainTextString().IsEmpty, Is.True);
      Assert.That(PlainTextString.CreateFromText("").IsEmpty, Is.True);
      Assert.That(PlainTextString.CreateFromText(" ").IsEmpty, Is.False);
      Assert.That(PlainTextString.CreateFromText("test").IsEmpty, Is.False);
    }

    [Test]
    public void Write_WithNullValue_RendersEmptyOutput ()
    {
      var plainTextString = PlainTextString.CreateFromText(null);

      var renderedString = ExecuteWithHtmlTextWriter(plainTextString.WriteTo);
      Assert.That(renderedString, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Write_WithEmptyValue_RendersEmptyOutput ()
    {
      var plainTextString = PlainTextString.Empty;

      var renderedString = ExecuteWithHtmlTextWriter(plainTextString.WriteTo);
      Assert.That(renderedString, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Write_RendersEncodedOutputWithTransformedLineBreaks ()
    {
      var stringValue = "aoe   \" & ' < > é \r \n \r\n";
      var plainTextString = PlainTextString.CreateFromText(stringValue);

      var renderedString = ExecuteWithHtmlTextWriter(plainTextString.WriteTo);
      Assert.That(renderedString, Is.EqualTo("aoe &#160; &quot; &amp; &#39; &lt; &gt; &#233; <br /> <br /> <br />"));
    }

    [Test]
    public void AddAttribute_NullValueWithAttributeEnum_RendersEmptyAttribute ()
    {
      var plainTextString = PlainTextString.CreateFromText(null);

      var renderedString = ExecuteWithHtmlTextWriter(
          writer =>
          {
            plainTextString.AddAttributeTo(writer, HtmlTextWriterAttribute.Class);
            writer.RenderBeginTag("a");
          });
      Assert.That(renderedString, Is.EqualTo("<a class=\"\">"));
    }

    [Test]
    public void AddAttribute_EmptyValueWithAttributeEnum_RendersEmptyAttribute ()
    {
      var plainTextString = PlainTextString.Empty;

      var renderedString = ExecuteWithHtmlTextWriter(
          writer =>
          {
            plainTextString.AddAttributeTo(writer, HtmlTextWriterAttribute.Class);
            writer.RenderBeginTag("a");
          });
      Assert.That(renderedString, Is.EqualTo("<a class=\"\">"));
    }

    [Test]
    public void AddAttribute_StringWithAttributeEnum_RendersEncodedAttributeValue ()
    {
      var stringValue = "aoe   \" & ' < > é \r \n \r\n";
      var plainTextString = PlainTextString.CreateFromText(stringValue);

      var renderedString = ExecuteWithHtmlTextWriter(
          writer =>
          {
            plainTextString.AddAttributeTo(writer, HtmlTextWriterAttribute.Class);
            writer.RenderBeginTag("a");
          });
      Assert.That(renderedString, Is.EqualTo("<a class=\"aoe   &quot; &amp; &#39; &lt; > é \r \n \r\n\">"));
    }

    [Test]
    public void AddAttribute_StringWithAttributeEnumThatDoesNotHaveEncodeDefault_RendersEncodedAttributeValue ()
    {
      var stringValue = "aoe   \" & ' < > é \r \n \r\n";
      var plainTextString = PlainTextString.CreateFromText(stringValue);

      var renderedString = ExecuteWithHtmlTextWriter(
          writer =>
          {
            plainTextString.AddAttributeTo(writer, HtmlTextWriterAttribute.Cols);
            writer.RenderBeginTag("a");
          });
      Assert.That(renderedString, Is.EqualTo("<a cols=\"aoe   &quot; &amp; &#39; &lt; > é \r \n \r\n\">"));
    }

    [Test]
    public void AddAttribute_StringWithAttributeString_RendersEncodedAttributeValue ()
    {
      var stringValue = "aoe   \" & ' < > é \r \n \r\n";
      var plainTextString = PlainTextString.CreateFromText(stringValue);

      var renderedString = ExecuteWithHtmlTextWriter(
          writer =>
          {
            plainTextString.AddAttributeTo(writer, "class");
            writer.RenderBeginTag("a");
          });
      Assert.That(renderedString, Is.EqualTo("<a class=\"aoe   &quot; &amp; &#39; &lt; > é \r \n \r\n\">"));
    }

    [Test]
    public void Equals ()
    {
      Assert.That(PlainTextString.CreateFromText("a").Equals(PlainTextString.CreateFromText("a")), Is.True);
      Assert.That(PlainTextString.CreateFromText("a").Equals(PlainTextString.CreateFromText("b")), Is.False);
    }

    [Test]
    public void ToString_EncodesOutputWithHtmlEncoding ()
    {
      Assert.That(
          PlainTextString.CreateFromText("aoe   \" & ' < > é \r \n \r\n").ToString(),
          Is.EqualTo("aoe &#160; &quot; &amp; &#39; &lt; &gt; &#233; \r \n \r\n"));
    }

    [Test]
    public void ToString_WithEncodedLineBreaksEncoding_EncodesOutputIncludingLineBreaks ()
    {
      Assert.That(
          PlainTextString.CreateFromText("aoe   \" & ' < > é \r \n \r\n").ToString(WebStringEncoding.HtmlWithTransformedLineBreaks),
          Is.EqualTo("aoe &#160; &quot; &amp; &#39; &lt; &gt; &#233; <br /> <br /> <br />"));
    }

    private string ExecuteWithHtmlTextWriter (Action<HtmlTextWriter> action)
    {
      var stringWriter = new StringWriter();
      var htmlTextWriter = new HtmlTextWriter(stringWriter);

      action(htmlTextWriter);
      htmlTextWriter.Flush();

      return stringWriter.ToString();
    }
  }
}
