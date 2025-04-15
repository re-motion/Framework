// SPDX-FileCopyrightText: (c) RUBICON IT GmbH, www.rubicon.eu
// SPDX-License-Identifier: LGPL-2.1-or-later
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Remotion.Web.UI.Globalization;

namespace Remotion.Web.UnitTests.UI.Globalization;

[TestFixture]
public class ResourceDispatcherTest
{
  private class WebStringClass
  {
    public WebString Text { get; set; }
  }

  private class PlainTextStringClass
  {
    public PlainTextString Text { get; set; }
  }

  private class StringClass
  {
    public string Text { get; set; }
  }

  private class WebStringSubClass : StringClass
  {
    public new WebString Text { get; set; }
  }

  private class PlainTextStringSubClass : StringClass
  {
    public new PlainTextString Text { get; set; }
  }

  private class AmbiguousStringSubClass : StringClass
  {
    public new StringBuilder Text { get; set; }
  }

  [Test]
  public void DispatchGeneric_WebString ()
  {
    var target = new WebStringClass();
    Dispatch(target, WebString.CreateFromText("value"));

    Assert.That(target.Text, Is.EqualTo(WebString.CreateFromText("value")));
  }

  [Test]
  public void DispatchGeneric_PlainTextString ()
  {
    var target = new PlainTextStringClass();
    Dispatch(target, WebString.CreateFromText("value"));

    Assert.That(target.Text, Is.EqualTo(PlainTextString.CreateFromText("value")));
  }

  [Test]
  public void DispatchGeneric_String ()
  {
    var target = new StringClass();
    Dispatch(target, WebString.CreateFromText("value"));

    Assert.That(target.Text, Is.EqualTo("value"));
  }

  [Test]
  public void DispatchGeneric_NewWebStringProperty ()
  {
    var target = new WebStringSubClass();
    Dispatch(target, WebString.CreateFromText("value"));

    Assert.That(target.Text, Is.EqualTo(WebString.CreateFromText("value")));
  }

  [Test]
  public void DispatchGeneric_NewPlainTextStringProperty ()
  {
    var target = new PlainTextStringSubClass();
    Dispatch(target, WebString.CreateFromText("value"));

    Assert.That(target.Text, Is.EqualTo(PlainTextString.CreateFromText("value")));
  }

  [Test]
  public void DispatchGeneric_AmbiguousProperty ()
  {
    var target = new AmbiguousStringSubClass();

    Assert.That(
        () => Dispatch(target, WebString.CreateFromText("value")),
        Throws.TypeOf<AmbiguousMatchException>());
  }

  private void Dispatch (object target, WebString value)
  {
    var values = new Dictionary<string, WebString>
                 {
                     { "Text", value }
                 };

    ResourceDispatcher.DispatchGeneric(target, values);
  }
}
