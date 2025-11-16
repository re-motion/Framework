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
    var header = CspHeader.Empty.AddDirectiveValue(CspDirectives.ScriptSrc, "asd");

    Assert.That(header.ToString(), Is.EqualTo("script-src asd"));
  }

  [Test]
  public void AddDirectiveValue_WithExistentDirective ()
  {
    var header = CspHeader.Empty
        .SetDirective(CspDirectives.ScriptSrc, "bbb")
        .AddDirectiveValue(CspDirectives.ScriptSrc, "asd");

    Assert.That(header.ToString(), Is.EqualTo("script-src bbb asd"));
  }

  [Test]
  public void AddDirectiveValue_WithExistingDirectiveValue ()
  {
    var header = CspHeader.Empty
        .SetDirective(CspDirectives.ScriptSrc, "a b c")
        .AddDirectiveValue(CspDirectives.ScriptSrc, "a");

    Assert.That(header.ToString(), Is.EqualTo("script-src a b c"));
  }

  [Test]
  public void AddDirectiveValue_WithSpace_Throws ()
  {
    Assert.That(
        () => CspHeader.Empty.AddDirectiveValue(CspDirectives.ScriptSrc, "'self' https:"),
        Throws.ArgumentException
            .With.ArgumentExceptionMessageEqualTo("Value must not contain spaces.", "value"));
  }

  [Test]
  public void SetDirectiveValue_WithNonExistentDirective ()
  {
    var header = CspHeader.Empty.SetDirective(CspDirectives.ScriptSrc, "asd 123");

    Assert.That(header.ToString(), Is.EqualTo("script-src asd 123"));
  }

  [Test]
  public void SetDirectiveValue_WithExistentDirective ()
  {
    var header = CspHeader.Empty
        .SetDirective(CspDirectives.ScriptSrc, "bbb")
        .SetDirective(CspDirectives.ScriptSrc, "asd 123");

    Assert.That(header.ToString(), Is.EqualTo("script-src asd 123"));
  }

  [Test]
  public void RemoveDirectiveValue_WithNonExistentDirective ()
  {
    var header = CspHeader.Empty.RemoveDirective(CspDirectives.ScriptSrc);

    Assert.That(header.ToString(), Is.EqualTo(""));
  }

  [Test]
  public void RemoveDirectiveValue_WithExistentDirective ()
  {
    var header = CspHeader.Empty
        .SetDirective(CspDirectives.ScriptSrc, "bbb")
        .RemoveDirective(CspDirectives.ScriptSrc);

    Assert.That(header.ToString(), Is.EqualTo(""));
  }

  [Test]
  public void TryGetDirectiveValues_WithNonExistentDirective ()
  {
    var header = CspHeader.Empty;

    Assert.That(
        header.TryGetDirectiveValues(CspDirectives.ScriptSrc, out var values),
        Is.False);
    Assert.That(values, Is.EqualTo(StringValues.Empty));
  }

  [Test]
  public void TryGetDirectiveValues_WithSingleValue ()
  {
    var header = CspHeader.Empty
        .SetDirective(CspDirectives.ScriptSrc, "asd");

    Assert.That(
        header.TryGetDirectiveValues(CspDirectives.ScriptSrc, out var values),
        Is.True);
    Assert.That(
        values,
        Is.EqualTo(new StringValues(["asd"])));
  }

  [Test]
  public void TryGetDirectiveValues_WithMultipleValues ()
  {
    var header = CspHeader.Empty
        .SetDirective(CspDirectives.ScriptSrc, "a b")
        .AddDirectiveValue(CspDirectives.ScriptSrc, "c");

    Assert.That(
        header.TryGetDirectiveValues(CspDirectives.ScriptSrc, out var values),
        Is.True);
    Assert.That(
        values.ToArray(),
        Is.EqualTo((string[])["a", "b", "c"]));
  }

  [Test]
  public void ToString_WithMultipleElements ()
  {
    var header = CspHeader.Empty
        .SetDirective(CspDirectives.ScriptSrc, "'self' https:")
        .SetDirective(CspDirectives.ImgSrc, "'none'")
        .AddDirectiveValue(CspDirectives.FormAction, "bla")
        .AddDirectiveValue(CspDirectives.ScriptSrc, "http://mycode.com")
        .RemoveDirective(CspDirectives.FormAction);

    Assert.That(
        header.ToString(),
        Is.EqualTo("img-src 'none'; script-src 'self' https: http://mycode.com"));
  }

  private static object[] s_enumValues = Enum.GetValues<CspDirectives>().Cast<object>().ToArray();

  [TestCaseSource(nameof(s_enumValues))]
  [Test]
  public void EnumValuesNamesTest (CspDirectives directive)
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
}
