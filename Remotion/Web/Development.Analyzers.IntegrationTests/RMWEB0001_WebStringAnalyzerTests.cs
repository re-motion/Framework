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
using System.Threading.Tasks;
using NUnit.Framework;
using Verifier = Remotion.Web.Development.Analyzers.IntegrationTests.CSharpAnalyzerVerifier<Remotion.Web.Development.Analyzers.RMWEB0001_WebStringAnalyzer>;

namespace Remotion.Web.Development.Analyzers.IntegrationTests
{
  [TestFixture]
  public class RMWEB0001_WebStringAnalyzerTests
  {
    [Test]
    public async Task StringBuilder_WithWebString_Diagnostic ()
    {
      const string input = @"
using System.Text;
using Remotion.Web;
public class A
{
  public void Test()
  {
    var sb = new StringBuilder();
    var webString = WebString.CreateFromText(""test"");
    sb.Append(webString);
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(10, 5, 10, 25)
          .WithMessage(
              "'System.Text.StringBuilder.Append(object?)' should not be used with a 'Remotion.Web.WebString' argument. "
              + "Call '.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks)' on the Remotion.Web.WebString instance instead.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task StringBuilder_WithPlainTextString_Diagnostic ()
    {
      const string input = @"
using System.Text;
using Remotion.Web;
public class A
{
  public void Test()
  {
    var sb = new StringBuilder();
    var plainTextString = PlainTextString.CreateFromText(""test"");
    sb.Append(plainTextString);
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(10, 5, 10, 31)
          .WithMessage(
              "'System.Text.StringBuilder.Append(object?)' should not be used with a 'Remotion.Web.PlainTextString' argument. "
              + "Call '.ToString(WebStringEncoding.HtmlWithTransformedLineBreaks)' on the Remotion.Web.PlainTextString instance instead.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task StringBuilder_WithWebStringToStringCall_Valid ()
    {
      const string input = @"
using System.Text;
using Remotion.Web;
public class A
{
  public void Test()
  {
    var sb = new StringBuilder();
    var encodedWebString = WebString.CreateFromText(""test"").ToString(WebStringEncoding.HtmlWithTransformedLineBreaks);
    sb.Append(encodedWebString);
  }
}";

      await Verifier.VerifyAnalyzerAsync(input);
    }

    [Test]
    public async Task HtmlTextWriter_WithWebString_Diagnostic ()
    {
      const string input = @"
using System.Web.UI;
using Remotion.Web;
public class A
{
  public void Test(HtmlTextWriter writer)
  {
    var webString = WebString.CreateFromText(""test"");
    writer.Write(webString);
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(9, 5, 9, 28)
          .WithMessage(
              "'System.Web.UI.HtmlTextWriter.Write(object)' should not be used with a 'Remotion.Web.WebString' argument. "
              + "Use 'Remotion.Web.WebString.WriteTo(HtmlTextWriter)' instead.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task HtmlTextWriter_WithPlainTextString_Diagnostic ()
    {
      const string input = @"
using System.Web.UI;
using Remotion.Web;
public class A
{
  public void Test(HtmlTextWriter writer)
  {
    var plainTextString = PlainTextString.CreateFromText(""test"");
    writer.Write(plainTextString);
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(9, 5, 9, 34)
          .WithMessage(
              "'System.Web.UI.HtmlTextWriter.Write(object)' should not be used with a 'Remotion.Web.PlainTextString' argument. "
              + "Use 'Remotion.Web.PlainTextString.WriteTo(HtmlTextWriter)' instead.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task HtmlTextWriter_WithWebStringWriteCalled_Valid ()
    {
      const string input = @"
using System.Web.UI;
using Remotion.Web;
public class A
{
  public void Test(HtmlTextWriter writer)
  {
    var webString = WebString.CreateFromText(""test"");
    webString.WriteTo(writer);
  }
}";

      await Verifier.VerifyAnalyzerAsync(input);
    }

    [Test]
    public async Task StringJoin_WithWebStringParams ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var webString1 = WebString.CreateFromText(""test1"");
    return string.Join("", "", webString1, WebString.CreateFromText(""test2""));
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(8, 12, 8, 76)
          .WithMessage(
              "'string.Join(string?, params object?[])' should not be used with a 'Remotion.Web.WebString' argument. "
              + "Encode the Remotion.Web.WebString instances first.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task StringJoin_WithWebStringArray ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var webStrings = new WebString[]{WebString.CreateFromText(""test1""), WebString.CreateFromText(""test2"")};
    return string.Join("", "", webStrings);
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(8, 12, 8, 41)
          .WithMessage(
              "'string.Join<T>(string?, System.Collections.Generic.IEnumerable<T>)' should not be used with a 'Remotion.Web.WebString[]' argument. "
              + "Encode the Remotion.Web.WebString[] instances first.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task StringJoin_WithPlainTextStringParams ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var plainTextString2 = PlainTextString.CreateFromText(""test2"");
    return string.Join("", "", PlainTextString.CreateFromText(""test1""), plainTextString2);
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(8, 12, 8, 88)
          .WithMessage(
              "'string.Join(string?, params object?[])' should not be used with a 'Remotion.Web.PlainTextString' argument. "
              + "Encode the Remotion.Web.PlainTextString instances first.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task StringJoin_WithPlainTextStringArray ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var plainTextStrings = new PlainTextString[]{PlainTextString.CreateFromText(""test1""), PlainTextString.CreateFromText(""test2"")};
    return string.Join("", "", plainTextStrings);
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(8, 12, 8, 47)
          .WithMessage(
              "'string.Join<T>(string?, System.Collections.Generic.IEnumerable<T>)' should not be used with a 'Remotion.Web.PlainTextString[]' argument. "
              + "Encode the Remotion.Web.PlainTextString[] instances first.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task StringFormat_WithWebString2Params ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var webString1 = WebString.CreateFromText(""test1"");
    return string.Format(""{0}, {1}"", webString1, WebString.CreateFromText(""test2""));
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(8, 12, 8, 84)
          .WithMessage(
              "'string.Format(string, object?, object?)' should not be used with a 'Remotion.Web.WebString' argument. "
              + "Encode the Remotion.Web.WebString instances first.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task StringFormat_WithWebStringParams ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var webString = WebString.CreateFromText(""test1"");
    return string.Format(""{0}, {1}, {2}, {3}, {4}"", webString, webString, webString, webString, webString);
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(8, 12, 8, 107)
          .WithMessage(
              "'string.Format(string, params object?[])' should not be used with a 'Remotion.Web.WebString' argument. "
              + "Encode the Remotion.Web.WebString instances first.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task StringFormat_WithPlainTextString2Params ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var plainTextString2 = PlainTextString.CreateFromText(""test2"");
    return string.Format(""{0}, {1}"", PlainTextString.CreateFromText(""test1""), plainTextString2);
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(8, 12, 8, 96)
          .WithMessage(
              "'string.Format(string, object?, object?)' should not be used with a 'Remotion.Web.PlainTextString' argument. "
              + "Encode the Remotion.Web.PlainTextString instances first.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task StringFormat_WithPlainTextStringParams ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var plainTextString = PlainTextString.CreateFromText(""test2"");
    return string.Format(""{0}, {1}, {2}, {3}, {4}"", plainTextString, plainTextString, plainTextString, plainTextString, plainTextString);
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(8, 12, 8, 137)
          .WithMessage(
              "'string.Format(string, params object?[])' should not be used with a 'Remotion.Web.PlainTextString' argument. "
              + "Encode the Remotion.Web.PlainTextString instances first.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task StringConcat_WithPlainTextString2Params ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var plainTextString2 = PlainTextString.CreateFromText(""test2"");
    return string.Concat(PlainTextString.CreateFromText(""test1""), plainTextString2);
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(8, 12, 8, 84)
          .WithMessage(
              "'string.Concat(object?, object?)' should not be used with a 'Remotion.Web.PlainTextString' argument. "
              + "Encode the Remotion.Web.PlainTextString instances first.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task StringConcat_WithPlainTextStringArray ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var plainTextStrings = new PlainTextString[]{PlainTextString.CreateFromText(""test1""), PlainTextString.CreateFromText(""test2"")};
    return string.Concat(plainTextStrings);
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(8, 12, 8, 43)
          .WithMessage(
              "'string.Concat<T>(System.Collections.Generic.IEnumerable<T>)' should not be used with a 'Remotion.Web.PlainTextString[]' argument. "
              + "Encode the Remotion.Web.PlainTextString[] instances first.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task StringConcat_WithWebString2Params ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var webString2 = WebString.CreateFromText(""test2"");
    return string.Concat(WebString.CreateFromText(""test1""), webString2);
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(8, 12, 8, 72)
          .WithMessage(
              "'string.Concat(object?, object?)' should not be used with a 'Remotion.Web.WebString' argument. "
              + "Encode the Remotion.Web.WebString instances first.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task StringConcat_WithWebStringArray ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var webStrings = new WebString[]{WebString.CreateFromText(""test1""), WebString.CreateFromText(""test2"")};
    return string.Concat(webStrings);
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(8, 12, 8, 37)
          .WithMessage(
              "'string.Concat<T>(System.Collections.Generic.IEnumerable<T>)' should not be used with a 'Remotion.Web.WebString[]' argument. "
              + "Encode the Remotion.Web.WebString[] instances first.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task AddConcat_WithStringAndPlainTextString ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var plainTextString = PlainTextString.CreateFromText(""test2"");
    return """" + plainTextString;
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(8, 17, 8, 32)
          .WithMessage(
              "'+' should not be used with a 'Remotion.Web.PlainTextString' argument. "
              + "Encode the Remotion.Web.PlainTextString instance first.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task AddConcat_WithPlainTextStrings ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var plainTextString2 = PlainTextString.CreateFromText(""test2"");
    return """" + PlainTextString.CreateFromText(""test1"") + plainTextString2;
  }
}";

      var diagnostic1 = Verifier.Diagnostic()
          .WithSpan(8, 17, 8, 56)
          .WithMessage(
              "'+' should not be used with a 'Remotion.Web.PlainTextString' argument. "
              + "Encode the Remotion.Web.PlainTextString instance first.");
      var diagnostic2 = Verifier.Diagnostic()
          .WithSpan(8, 59, 8, 75)
          .WithMessage(
              "'+' should not be used with a 'Remotion.Web.PlainTextString' argument. "
              + "Encode the Remotion.Web.PlainTextString instance first.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic1, diagnostic2);
    }

    [Test]
    public async Task AddConcat_WithStringAndWebString ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var webString = WebString.CreateFromText(""test2"");
    return """" + webString;
  }
}";

      var diagnostic = Verifier.Diagnostic()
          .WithSpan(8, 17, 8, 26)
          .WithMessage(
              "'+' should not be used with a 'Remotion.Web.WebString' argument. "
              + "Encode the Remotion.Web.WebString instance first.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic);
    }

    [Test]
    public async Task AddConcat_WithWebStrings ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var webString2 = WebString.CreateFromText(""test2"");
    return """" + WebString.CreateFromText(""test1"") + webString2;
  }
}";

      var diagnostic1 = Verifier.Diagnostic()
          .WithSpan(8, 17, 8, 50)
          .WithMessage(
              "'+' should not be used with a 'Remotion.Web.WebString' argument. "
              + "Encode the Remotion.Web.WebString instance first.");
      var diagnostic2 = Verifier.Diagnostic()
          .WithSpan(8, 53, 8, 63)
          .WithMessage(
              "'+' should not be used with a 'Remotion.Web.WebString' argument. "
              + "Encode the Remotion.Web.WebString instance first.");
      await Verifier.VerifyAnalyzerAsync(input, diagnostic1, diagnostic2);
    }

    [Test]
    public async Task StringInterpolation_WithWebStrings ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var webString2 = WebString.CreateFromText(""test2"");
    return $"" {WebString.CreateFromText(""test1"")}, {webString2} "";
  }
}";

      var diagnostic1 = Verifier.Diagnostic()
          .WithSpan(8, 16, 8, 49)
          .WithMessage(
              "'$\"\"' should not be used with a 'Remotion.Web.WebString' argument. "
                  + "Encode the Remotion.Web.WebString instance first.");
      var diagnostic2 = Verifier.Diagnostic()
          .WithSpan(8, 53, 8, 63)
          .WithMessage(
              "'$\"\"' should not be used with a 'Remotion.Web.WebString' argument. "
              + "Encode the Remotion.Web.WebString instance first.");

      await Verifier.VerifyAnalyzerAsync(input, diagnostic1, diagnostic2);
    }

    [Test]
    public async Task StringInterpolation_WithPlainTextStrings ()
    {
      const string input = @"
using Remotion.Web;
public class A
{
  public string Test()
  {
    var plainTextString2 = PlainTextString.CreateFromText(""test2"");
    return $"" {PlainTextString.CreateFromText(""test1"")}, {plainTextString2} "";
  }
}";

      var diagnostic1 = Verifier.Diagnostic()
          .WithSpan(8, 16, 8, 55)
          .WithMessage(
              "'$\"\"' should not be used with a 'Remotion.Web.PlainTextString' argument. "
              + "Encode the Remotion.Web.PlainTextString instance first.");
      var diagnostic2 = Verifier.Diagnostic()
          .WithSpan(8, 59, 8, 75)
          .WithMessage(
              "'$\"\"' should not be used with a 'Remotion.Web.PlainTextString' argument. "
              + "Encode the Remotion.Web.PlainTextString instance first.");

      await Verifier.VerifyAnalyzerAsync(input, diagnostic1, diagnostic2);
    }
  }
}
