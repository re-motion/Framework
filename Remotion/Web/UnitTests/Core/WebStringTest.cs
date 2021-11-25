using System;
using System.IO;
using System.Web.UI;
using NUnit.Framework;

namespace Remotion.Web.UnitTests.Core
{
  [TestFixture]
  public class WebStringTest
  {
    [Test]
    public void CreateFromHtml ()
    {
      var stringValue = "aoe   \" & ' < > é \r \n \r\n";
      var webString = WebString.CreateFromHtml(stringValue);

      Assert.That(webString.Type, Is.EqualTo(WebStringType.Encoded));
      Assert.That(webString.GetValue(), Is.EqualTo(stringValue));
    }

    [Test]
    public void CreateFromHtml_WithNullValue_IsSameAsEmptyString ()
    {
      Assert.That(WebString.CreateFromHtml(null), Is.EqualTo(WebString.CreateFromHtml(string.Empty)));
    }

    [Test]
    public void CreateFromText ()
    {
      var stringValue = "aoe   \" & ' < > é \r \n \r\n";
      var webString = WebString.CreateFromText(stringValue);

      Assert.That(webString.Type, Is.EqualTo(WebStringType.PlainText));
      Assert.That(webString.GetValue(), Is.EqualTo(stringValue));
    }

    [Test]
    public void CreateFromText_WithNullValue_IsSameAsEmptyString ()
    {
      Assert.That(WebString.CreateFromText(null), Is.EqualTo(WebString.CreateFromText(string.Empty)));
    }

    [Test]
    public void IsEmpty ()
    {
      Assert.That(new WebString().IsEmpty, Is.True);
      Assert.That(WebString.CreateFromText("").IsEmpty, Is.True);
      Assert.That(WebString.CreateFromText(" ").IsEmpty, Is.False);
      Assert.That(WebString.CreateFromText("test").IsEmpty, Is.False);
    }

    [Test]
    public void Write_NullPlainTextWebString_RendersEmptyOutput ()
    {
      var webString = WebString.CreateFromText(null);

      var renderedString = ExecuteWithHtmlTextWriter(webString.Write);
      Assert.That(renderedString, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Write_EmptyPlainTextWebString_RendersEmptyOutput ()
    {
      var webString = WebString.CreateFromText(string.Empty);

      var renderedString = ExecuteWithHtmlTextWriter(webString.Write);
      Assert.That(renderedString, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Write_PlainTextWebString_RendersEncodedOutputWithTransformedLineBreaks ()
    {
      var stringValue = "aoe   \" & ' < > é \r \n \r\n";
      var webString = WebString.CreateFromText(stringValue);

      var renderedString = ExecuteWithHtmlTextWriter(webString.Write);
      Assert.That(renderedString, Is.EqualTo("aoe &#160; &quot; &amp; &#39; &lt; &gt; &#233; <br /> <br /> <br />"));
    }

    [Test]
    public void Write_NullEncodedWebString_RendersEmptyOutput ()
    {
      var webString = WebString.CreateFromHtml(null);

      var renderedString = ExecuteWithHtmlTextWriter(webString.Write);
      Assert.That(renderedString, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Write_EmptyEncodedWebString_RendersEmptyOutput ()
    {
      var webString = WebString.CreateFromHtml(string.Empty);

      var renderedString = ExecuteWithHtmlTextWriter(webString.Write);
      Assert.That(renderedString, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Write_EncodedWebString_RendersUnencodedOutput ()
    {
      var stringValue = "aoe   \" & ' < > é \r \n \r\n";
      var webString = WebString.CreateFromHtml(stringValue);

      var renderedString = ExecuteWithHtmlTextWriter(webString.Write);
      Assert.That(renderedString, Is.EqualTo(stringValue));
    }

    [Test]
    public void AddAttribute_NullPlainTextWebString_RendersEmptyAttribute ()
    {
      var webString = WebString.CreateFromText(null);

      var renderedString = ExecuteWithHtmlTextWriter(
          writer =>
          {
            webString.AddAttribute(writer, HtmlTextWriterAttribute.Class);
            writer.RenderBeginTag("a");
          });
      Assert.That(renderedString, Is.EqualTo("<a class=\"\">"));
    }

    [Test]
    public void AddAttribute_EmptyPlainTextWebString_RendersEmptyAttribute ()
    {
      var webString = WebString.CreateFromText(string.Empty);

      var renderedString = ExecuteWithHtmlTextWriter(
          writer =>
          {
            webString.AddAttribute(writer, HtmlTextWriterAttribute.Class);
            writer.RenderBeginTag("a");
          });
      Assert.That(renderedString, Is.EqualTo("<a class=\"\">"));
    }

    [Test]
    public void AddAttribute_PlainTextWebStringWithAttributeEnum_RendersEncodedAttributeValue ()
    {
      var stringValue = "aoe   \" & ' < > é \r \n \r\n";
      var webString = WebString.CreateFromText(stringValue);

      var renderedString = ExecuteWithHtmlTextWriter(
          writer =>
          {
            webString.AddAttribute(writer, HtmlTextWriterAttribute.Class);
            writer.RenderBeginTag("a");
          });
      Assert.That(renderedString, Is.EqualTo("<a class=\"aoe   &quot; &amp; &#39; &lt; > é \r \n \r\n\">"));
    }

    [Test]
    public void AddAttribute_PlainTextWebStringWithAttributeEnumThatDoesNotHaveEncodeDefault_RendersEncodedAttributeValue ()
    {
      var stringValue = "aoe   \" & ' < > é \r \n \r\n";
      var webString = WebString.CreateFromText(stringValue);

      var renderedString = ExecuteWithHtmlTextWriter(
          writer =>
          {
            webString.AddAttribute(writer, HtmlTextWriterAttribute.Cols);
            writer.RenderBeginTag("a");
          });
      Assert.That(renderedString, Is.EqualTo("<a cols=\"aoe   &quot; &amp; &#39; &lt; > é \r \n \r\n\">"));
    }

    [Test]
    public void AddAttribute_PlainTextWebStringWithAttributeString_RendersEncodedAttributeValue ()
    {
      var stringValue = "aoe   \" & ' < > é \r \n \r\n";
      var webString = WebString.CreateFromText(stringValue);

      var renderedString = ExecuteWithHtmlTextWriter(
          writer =>
          {
            webString.AddAttribute(writer, "class");
            writer.RenderBeginTag("a");
          });
      Assert.That(renderedString, Is.EqualTo("<a class=\"aoe   &quot; &amp; &#39; &lt; > é \r \n \r\n\">"));
    }

    [Test]
    public void AddAttribute_EncodedWebStringWithAttributeEnumThatDoesNotHaveEncodeDefault_RendersUnencodedAttributeValue ()
    {
      var stringValue = "aoe   \" & ' < > é \r \n \r\n";
      var webString = WebString.CreateFromHtml(stringValue);

      var renderedString = ExecuteWithHtmlTextWriter(
          writer =>
          {
            webString.AddAttribute(writer, HtmlTextWriterAttribute.Cols);
            writer.RenderBeginTag("a");
          });
      Assert.That(renderedString, Is.EqualTo($"<a cols=\"{stringValue}\">"));
    }

    [Test]
    public void AddAttribute_EncodedWebStringWithAttributeEnum_RendersUnencodedAttributeValue ()
    {
      var stringValue = "aoe   \" & ' < > é \r \n \r\n";
      var webString = WebString.CreateFromHtml(stringValue);

      var renderedString = ExecuteWithHtmlTextWriter(
          writer =>
          {
            webString.AddAttribute(writer, HtmlTextWriterAttribute.Class);
            writer.RenderBeginTag("a");
          });
      Assert.That(renderedString, Is.EqualTo($"<a class=\"{stringValue}\">"));
    }

    [Test]
    public void AddAttribute_EncodedWebStringWithAttributeString_RendersUnencodedAttributeValue ()
    {
      var stringValue = "aoe   \" & ' < > é \r \n \r\n";
      var webString = WebString.CreateFromHtml(stringValue);

      var renderedString = ExecuteWithHtmlTextWriter(
          writer =>
          {
            webString.AddAttribute(writer, "class");
            writer.RenderBeginTag("a");
          });
      Assert.That(renderedString, Is.EqualTo($@"<a class=""{stringValue}"">"));
    }

    [Test]
    public void Equals ()
    {
      Assert.That(WebString.CreateFromText("a").Equals(WebString.CreateFromText("a")), Is.True);
      Assert.That(WebString.CreateFromText("a").Equals(WebString.CreateFromHtml("a")), Is.False);
    }

    [Test]
    public void ToString_WithEncodedText_DoesNotEncodeOutput ()
    {
      Assert.That(WebString.CreateFromHtml("aoe   \" & ' < > é \r \n \r\n").ToString(WebStringEncoding.Html), Is.EqualTo("aoe   \" & ' < > é \r \n \r\n"));
      Assert.That(WebString.CreateFromHtml("aoe   \" & ' < > é \r \n \r\n").ToString(WebStringEncoding.Attribute), Is.EqualTo("aoe   \" & ' < > é \r \n \r\n"));
    }

    [Test]
    public void ToString_WithPlainTextText_EncodesOutputWithHtmlEncoding ()
    {
      Assert.That(WebString.CreateFromText("aoe   \" & ' < > é \r \n \r\n").ToString(), Is.EqualTo("aoe &#160; &quot; &amp; &#39; &lt; &gt; &#233; \r \n \r\n"));
    }

    [Test]
    public void ToString_WithPlainTextTextAndAttributeEncoding_EncodesOutput ()
    {
      Assert.That(WebString.CreateFromText("aoe   \" & ' < > é \r \n \r\n").ToString(WebStringEncoding.Attribute), Is.EqualTo("aoe   &quot; &amp; &#39; &lt; > é \r \n \r\n"));
    }

    [Test]
    public void ToString_WithPlainTextTextAndHtmlWithEncodedLineBreaksEncoding_EncodesOutputIncludingLineBreaks ()
    {
      Assert.That(WebString.CreateFromText("aoe   \" & ' < > é \r \n \r\n").ToString(WebStringEncoding.HtmlWithTransformedLineBreaks), Is.EqualTo("aoe &#160; &quot; &amp; &#39; &lt; &gt; &#233; <br /> <br /> <br />"));
    }

    [Test]
    public void ToPlainTextString_WithPlainTextText_Succeeds ()
    {
      var plainTextString = WebString.CreateFromText("test").ToPlainTextString();

      Assert.That(plainTextString.GetValue(), Is.EqualTo("test"));
    }

    [Test]
    public void ToPlainTextString_WithEncodedText_ThrowsInvalidOperationException ()
    {
      Assert.That(
          () => WebString.CreateFromHtml("test").ToPlainTextString(),
          Throws.TypeOf<InvalidOperationException>()
              .And.Message.EqualTo("Cannot convert to PlainTextString as the WebString is not of type 'PlainText'."));
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