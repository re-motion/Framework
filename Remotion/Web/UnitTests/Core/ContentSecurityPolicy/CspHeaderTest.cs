// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Primitives;
using NUnit.Framework;
using Remotion.Development.NUnit.UnitTesting;
using Remotion.Web.ContentSecurityPolicy;

namespace Remotion.Web.UnitTests.Core.ContentSecurityPolicy;

[TestFixture]
public class CspHeaderTest
{
  [Test]
  public void AddDirectiveValue_WithNonExistentDirective ()
  {
    var header = CspHeader.Empty.AddDirectiveValue(CspDirective.ScriptSrc, "asd");

    Assert.That(header.ToString(), Is.EqualTo("script-src asd"));
  }

  [Test]
  public void AddDirectiveValue_WithExistentDirective ()
  {
    var header = CspHeader.Empty
        .SetDirective(CspDirective.ScriptSrc, "bbb")
        .AddDirectiveValue(CspDirective.ScriptSrc, "asd");

    Assert.That(header.ToString(), Is.EqualTo("script-src bbb asd"));
  }

  [Test]
  public void AddDirectiveValue_WithExistingDirectiveValue ()
  {
    var header = CspHeader.Empty
        .SetDirective(CspDirective.ScriptSrc, "a b c")
        .AddDirectiveValue(CspDirective.ScriptSrc, "a");

    Assert.That(header.ToString(), Is.EqualTo("script-src a b c"));
  }

  [Test]
  [TestCase(' ')]
  [TestCase('\t')]
  [TestCase('\n')]
  [TestCase('\r')]
  [TestCase('\f')]
  public void AddDirectiveValue_WithSpace_Throws (char whiteSpace)
  {
    Assert.That(
        () => CspHeader.Empty.AddDirectiveValue(CspDirective.ScriptSrc, $"'self'{whiteSpace}https:"),
        Throws.ArgumentException
            .With.ArgumentExceptionMessageEqualTo("Value must not contain spaces.", "value"));
  }

  [Test]
  public void SetDirectiveValue_WithNonExistentDirective ()
  {
    var header = CspHeader.Empty.SetDirective(CspDirective.ScriptSrc, "asd 123");

    Assert.That(header.ToString(), Is.EqualTo("script-src asd 123"));
  }

  [Test]
  public void SetDirectiveValue_WithExistentDirective ()
  {
    var header = CspHeader.Empty
        .SetDirective(CspDirective.ScriptSrc, "bbb")
        .SetDirective(CspDirective.ScriptSrc, "asd 123");

    Assert.That(header.ToString(), Is.EqualTo("script-src asd 123"));
  }

  [Test]
  public void RemoveDirectiveValue_WithNonExistentDirective ()
  {
    var header = CspHeader.Empty.RemoveDirective(CspDirective.ScriptSrc);

    Assert.That(header.ToString(), Is.EqualTo(""));
  }

  [Test]
  public void RemoveDirectiveValue_WithExistentDirective ()
  {
    var header = CspHeader.Empty
        .SetDirective(CspDirective.ScriptSrc, "bbb")
        .RemoveDirective(CspDirective.ScriptSrc);

    Assert.That(header.ToString(), Is.EqualTo(""));
  }

  [Test]
  public void TryGetDirectiveValues_WithNonExistentDirective ()
  {
    var header = CspHeader.Empty;

    Assert.That(
        header.TryGetDirectiveValues(CspDirective.ScriptSrc, out var values),
        Is.False);
    Assert.That(values, Is.EqualTo(StringValues.Empty));
  }

  [Test]
  public void TryGetDirectiveValues_WithSingleValue ()
  {
    var header = CspHeader.Empty
        .SetDirective(CspDirective.ScriptSrc, "asd");

    Assert.That(
        header.TryGetDirectiveValues(CspDirective.ScriptSrc, out var values),
        Is.True);
    Assert.That(
        values,
        Is.EqualTo(new StringValues(["asd"])));
  }

  [Test]
  public void TryGetDirectiveValues_WithMultipleValues ()
  {
    var header = CspHeader.Empty
        .SetDirective(CspDirective.ScriptSrc, "a b")
        .AddDirectiveValue(CspDirective.ScriptSrc, "c");

    Assert.That(
        header.TryGetDirectiveValues(CspDirective.ScriptSrc, out var values),
        Is.True);
    Assert.That(
        values.ToArray(),
        Is.EqualTo((string[])["a", "b", "c"]));
  }

  [Test]
  public void ToString_WithMultipleElements ()
  {
    var header = CspHeader.Empty
        .SetDirective(CspDirective.ScriptSrc, "'self' https:")
        .SetDirective(CspDirective.ImgSrc, "'none'")
        .AddDirectiveValue(CspDirective.FormAction, "bla")
        .AddDirectiveValue(CspDirective.ScriptSrc, "http://mycode.com")
        .RemoveDirective(CspDirective.FormAction);

    Assert.That(
        header.ToString(),
        Is.EqualTo("img-src 'none'; script-src 'self' https: http://mycode.com"));
  }

  private static object[] s_enumValues = Enum.GetValues<CspDirective>().Cast<object>().ToArray();

  [TestCaseSource(nameof(s_enumValues))]
  [Test]
  public void EnumValuesNamesTest (CspDirective directive)
  {
    var header = CspHeader.Empty.SetDirective(directive, "");

    // Converts the PascalCase name to kebab-case as that should match actual directive name
    var expectedHeaderName = new string(directive
        .ToString()
        .SelectMany((e, i) => char.IsUpper(e) && i > 0 ? (IEnumerable<char>)['-', char.ToLower(e)] : [char.ToLower(e)])
        .ToArray());

    Assert.That(
        header.ToString(),
        Is.EqualTo(expectedHeaderName));
  }

  [Test]
  public void Parse_EmptyInput_ReturnsEmptyCspHeader ()
  {
    Assert.That(CspHeader.Parse(string.Empty).ToString(), Is.EqualTo(CspHeader.Empty.ToString()));
  }

  [Test]
  public void Parse_InputWithAsciiWhitespaces_ReturnsExpectedCspHeader ()
  {
    const string input = " \t\n\r\fdefault-src 'self'\t\n\r\f oesterreich.gv.at;\t\n\r\f";

    Assert.That(CspHeader.Parse(input).ToString(), Is.EqualTo("default-src 'self' oesterreich.gv.at"));
  }

  [Test]
  public void Parse_InputWithDuplicateDirective_IgnoresDuplicatedDirective ()
  {
    const string input = "default-src 'self';default-src data:";

    var parsedHeader = CspHeader.Parse(input);
    _ = parsedHeader.TryGetDirectiveValues(CspDirective.DefaultSrc, out var values);

    Assert.That(values.Count, Is.EqualTo(1));
    Assert.That(values[0], Is.EqualTo("'self'"));
  }

  [Test]
  public void Parse_InputWithEmptyDirective_IsIgnored ()
  {
    const string input = "default-src 'self';   ;script-src 'self';";

    var parsedHeader = CspHeader.Parse(input);

    Assert.That(parsedHeader.ToString(), Is.EqualTo("default-src 'self'; script-src 'self'"));
  }

  [Test]
  public void Parse_DirectiveWithNonAsciiCharacter_IsUsedAsIs ()
  {
    const string input = "default-src österreich.gv.at";

    var parsedHeader = CspHeader.Parse(input);

    Assert.That(parsedHeader.ToString(), Is.EqualTo(input));
  }

  [Test]
  public void Parse_DirectiveNameWithDifferentCapitalization_IsLowered ()
  {
    const string input = "DeFaUlT-SrC 'self'";

    var parsedHeader = CspHeader.Parse(input);

    Assert.That(parsedHeader.ToString(), Is.EqualTo("default-src 'self'"));
  }

  [Test]
  public void Parse_DirectiveValuesContainsSpace_IsUsedAsIs ()
  {
    const string input = "DeFaUlT-SrC 'se lf'";

    var parsedHeader = CspHeader.Parse(input);

    Assert.That(parsedHeader.ToString(), Is.EqualTo("default-src 'se lf'"));
  }
}
