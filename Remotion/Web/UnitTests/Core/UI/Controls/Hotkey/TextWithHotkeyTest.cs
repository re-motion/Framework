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
using NUnit.Framework;
using Remotion.Web.UI.Controls.Hotkey;

namespace Remotion.Web.UnitTests.Core.UI.Controls.Hotkey
{
  [TestFixture]
  public class TextWithHotkeyTest
  {
    [Test]
    public void Initialize_WithText_AndHotkeyIndex ()
    {
      var textWithHotkey = new TextWithHotkey ("foo bar", 4);

      Assert.That (textWithHotkey.Text, Is.EqualTo ("foo bar"));
      Assert.That (textWithHotkey.HotkeyIndex, Is.EqualTo (4));
      Assert.That (textWithHotkey.Hotkey, Is.EqualTo ('b'));
    }

    [Test]
    public void Initialize_WithText_AndHotkeyIndexNull ()
    {
      var textWithHotkey = new TextWithHotkey ("foo bar", null);

      Assert.That (textWithHotkey.Text, Is.EqualTo ("foo bar"));
      Assert.That (textWithHotkey.HotkeyIndex, Is.Null);
      Assert.That (textWithHotkey.Hotkey, Is.Null);
    }

    [Test]
    public void Initialize_WithTextEmpty_AndHotkeyIndexNull ()
    {
      var textWithHotkey = new TextWithHotkey ("", null);

      Assert.That (textWithHotkey.Text, Is.Empty);
      Assert.That (textWithHotkey.HotkeyIndex, Is.Null);
      Assert.That (textWithHotkey.Hotkey, Is.Null);
    }

    [Test]
    public void Initialize_WithTextNull_ThrowsArgumentNullException ()
    {
      // ReSharper disable AssignNullToNotNullAttribute
      Assert.That (() => new TextWithHotkey (null, null), Throws.InstanceOf<ArgumentNullException>());
      // ReSharper restore AssignNullToNotNullAttribute
    }

    [Test]
    public void Initialize_WithText_AndHotkeyIndexTooBig_ThrowsArgumentOutOfRangeException ()
    {
      Assert.That (() => new TextWithHotkey ("foo", 3), Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Initialize_WithText_AndHotkeyNegative_ThrowsArgumentOutOfRangeException ()
    {
      Assert.That (() => new TextWithHotkey ("foo", -1), Throws.InstanceOf<ArgumentOutOfRangeException>());
    }

    [Test]
    public void Initialize_WithText_AndHotkeyIndexNotInidicatingLetterOrDigit_ThrowsArgumentException ()
    {
      Assert.That (() => new TextWithHotkey ("fo.o", 2), Throws.InstanceOf<ArgumentException>());
    }

    [Test]
    public void Initialize_WithText_AndHotkey ()
    {
      var textWithHotkey = new TextWithHotkey ("foo bar", 'X');

      Assert.That (textWithHotkey.Text, Is.EqualTo ("foo bar"));
      Assert.That (textWithHotkey.HotkeyIndex, Is.Null);
      Assert.That (textWithHotkey.Hotkey, Is.EqualTo ('X'));
    }

    [Test]
    public void Initialize_WithTextEmpty_AndHotkey ()
    {
      var textWithHotkey = new TextWithHotkey ("", 'X');

      Assert.That (textWithHotkey.Text, Is.Empty);
      Assert.That (textWithHotkey.HotkeyIndex, Is.Null);
      Assert.That (textWithHotkey.Hotkey, Is.EqualTo ('X'));
    }

    [Test]
    public void Initialize_WithTextNull_AndHotkey_ThrowsArgumentNullException ()
    {
      // ReSharper disable AssignNullToNotNullAttribute
      Assert.That (() => new TextWithHotkey (null, 'X'), Throws.InstanceOf<ArgumentNullException>());
      // ReSharper restore AssignNullToNotNullAttribute
    }

    [Test]
    public void Initialize_WithText_AndHotkeyNotLetterOrDigit_ThrowsArgumentException ()
    {
      Assert.That (() => new TextWithHotkey ("foo", '/'), Throws.InstanceOf<ArgumentException>());
    }
  }
}