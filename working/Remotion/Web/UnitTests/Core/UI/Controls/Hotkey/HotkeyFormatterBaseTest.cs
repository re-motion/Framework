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
using System.Text;
using NUnit.Framework;
using Remotion.Web.UI.Controls.Hotkey;

namespace Remotion.Web.UnitTests.Core.UI.Controls.Hotkey
{
  [TestFixture]
  public class HotkeyFormatterBaseTest
  {
    private class TestableHotkeyFormatterBase : HotkeyFormatterBase
    {
      protected override void AppendHotkeyBeginTag (StringBuilder stringBuilder, string hotkey)
      {
        stringBuilder.AppendFormat ("<x '{0}'>", hotkey);
      }

      protected override void AppendHotkeyEndTag (StringBuilder stringBuilder)
      {
        stringBuilder.AppendFormat ("</x>");
      }
    }

    [Test]
    public void FormatHotkey_NoHotkey ()
    {
      var formatter = new TestableHotkeyFormatterBase();
      var textWithHotkey = new TextWithHotkey ("text", null);

      Assert.That (formatter.FormatHotkey (textWithHotkey), Is.Null);
    }

    [Test]
    public void FormatHotkey ()
    {
      var formatter = new TestableHotkeyFormatterBase();
      var textWithHotkey = new TextWithHotkey ("text", 'A');

      Assert.That (formatter.FormatHotkey (textWithHotkey), Is.EqualTo ("A"));
    }

    [Test]
    public void FormatHotkey_LowerCaseHotkey_MakesUpperCase ()
    {
      var formatter = new TestableHotkeyFormatterBase();
      var textWithHotkey = new TextWithHotkey ("text", 'a');

      Assert.That (formatter.FormatHotkey (textWithHotkey), Is.EqualTo ("A"));
    }

    [Test]
    public void FormatText_NoEncoding_NoHotkey ()
    {
      var formatter = new TestableHotkeyFormatterBase();
      var textWithHotkey = new TextWithHotkey ("f<b>o</b>o b&ar", null);

      Assert.That (formatter.FormatText (textWithHotkey, false), Is.EqualTo ("f<b>o</b>o b&ar"));
    }

    [Test]
    public void FormatText_WithHtmlEncoding_NoHotkey ()
    {
      var formatter = new TestableHotkeyFormatterBase();
      var textWithHotkey = new TextWithHotkey ("f<b>o</b>o b&ar", null);

      Assert.That (formatter.FormatText (textWithHotkey, true), Is.EqualTo ("f&lt;b&gt;o&lt;/b&gt;o b&amp;ar"));
    }

    [Test]
    public void FormatText_NoEncoding_WithHotkey ()
    {
      var formatter = new TestableHotkeyFormatterBase();
      var textWithHotkey = new TextWithHotkey ("foo b&ar", 4);

      Assert.That (formatter.FormatText (textWithHotkey, false), Is.EqualTo ("foo <x 'b'>b</x>&ar"));
    }

    [Test]
    public void FormatText_NoEncoding_WithHotkeyAtStart ()
    {
      var formatter = new TestableHotkeyFormatterBase();
      var textWithHotkey = new TextWithHotkey ("foo bar", 0);

      Assert.That (formatter.FormatText (textWithHotkey, false), Is.EqualTo ("<x 'f'>f</x>oo bar"));
    }

    [Test]
    public void FormatText_NoEncoding_WithHotkeyAtEnd ()
    {
      var formatter = new TestableHotkeyFormatterBase();
      var textWithHotkey = new TextWithHotkey ("foo bar", 6);

      Assert.That (formatter.FormatText (textWithHotkey, false), Is.EqualTo ("foo ba<x 'r'>r</x>"));
    }

    [Test]
    public void FormatText_WithHtmlEncoding_WithHotkey ()
    {
      var formatter = new TestableHotkeyFormatterBase();
      var textWithHotkey = new TextWithHotkey ("f<b>o</b>o b&ar", 11);

      Assert.That (formatter.FormatText (textWithHotkey, true), Is.EqualTo ("f&lt;b&gt;o&lt;/b&gt;o <x 'b'>b</x>&amp;ar"));
    }

    [Test]
    public void FormatText_WithHtmlEncoding_WithEncodedHotkey ()
    {
      var formatter = new TestableHotkeyFormatterBase();
      var textWithHotkey = new TextWithHotkey ("foo öar", 4);

      Assert.That (formatter.FormatText (textWithHotkey, true), Is.EqualTo ("foo <x '&#246;'>&#246;</x>ar"));
    }
  }
}