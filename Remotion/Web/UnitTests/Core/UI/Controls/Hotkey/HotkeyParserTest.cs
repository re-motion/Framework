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
  public class HotkeyParserTest
  {
    [Test]
    public void Parse_TextWithoutHotkey ()
    {
      var result = HotkeyParser.Parse ("No Hotkey");
      Assert.That (result.Text, Is.EqualTo ("No Hotkey"));
      Assert.That (result.Hotkey, Is.Null);
      Assert.That (result.HotkeyIndex, Is.Null);
    }

    [Test]
    public void Parse_TextIsEmpty ()
    {
      var result = HotkeyParser.Parse ("");
      Assert.That (result.Text, Is.Empty);
      Assert.That (result.Hotkey, Is.Null);
      Assert.That (result.HotkeyIndex, Is.Null);
    }

    [Test]
    public void Parse_TextIsNull ()
    {
      var result = HotkeyParser.Parse (null);
      Assert.That (result.Text, Is.Empty);
      Assert.That (result.Hotkey, Is.Null);
      Assert.That (result.HotkeyIndex, Is.Null);
    }

    [Test]
    public void Parse_TextWithHotkey ()
    {
      var result = HotkeyParser.Parse ("A &Hotkey");
      Assert.That (result.Text, Is.EqualTo ("A Hotkey"));
      Assert.That (result.Hotkey, Is.EqualTo ('H'));
      Assert.That (result.HotkeyIndex, Is.EqualTo (2));
    }

    [Test]
    public void Parse_TextWithHotkeyAtStart ()
    {
      var result = HotkeyParser.Parse ("&A Hotkey");
      Assert.That (result.Text, Is.EqualTo ("A Hotkey"));
      Assert.That (result.Hotkey, Is.EqualTo ('A'));
      Assert.That (result.HotkeyIndex, Is.EqualTo (0));
    }

    [Test]
    public void Parse_TextWithHotkeyAtEnd ()
    {
      var result = HotkeyParser.Parse ("A Hotke&y");
      Assert.That (result.Text, Is.EqualTo ("A Hotkey"));
      Assert.That (result.Hotkey, Is.EqualTo ('y'));
      Assert.That (result.HotkeyIndex, Is.EqualTo (7));
    }

    [Test]
    public void Parse_TextWithHotkeyMarkerBeforeWhitespace_IgnoreHotkeyMarker ()
    {
      var result = HotkeyParser.Parse ("No & Hotkey");
      Assert.That (result.Text, Is.EqualTo ("No & Hotkey"));
      Assert.That (result.Hotkey, Is.Null);
      Assert.That (result.HotkeyIndex, Is.Null);
    }

    [Test]
    public void Parse_TextWithHotkeyMarkerBeforePunctuation_IgnoreHotkeyMarker ()
    {
      var result = HotkeyParser.Parse ("No &. Hotkey");
      Assert.That (result.Text, Is.EqualTo ("No &. Hotkey"));
      Assert.That (result.Hotkey, Is.Null);
      Assert.That (result.HotkeyIndex, Is.Null);
    }

    [Test]
    public void Parse_TextWithHotkeyMarkerAsLastCharacter_IgnoreHotkeyMarker ()
    {
      var result = HotkeyParser.Parse ("No Hotkey&");
      Assert.That (result.Text, Is.EqualTo ("No Hotkey&"));
      Assert.That (result.Hotkey, Is.Null);
      Assert.That (result.HotkeyIndex, Is.Null);
    }

    [Test]
    public void Parse_TextWithMultipleHotkeyMarkers_IgnoreHotkeyMarkers ()
    {
      var result = HotkeyParser.Parse ("&No &Hotkey");
      Assert.That (result.Text, Is.EqualTo ("&No &Hotkey"));
      Assert.That (result.Hotkey, Is.Null);
      Assert.That (result.HotkeyIndex, Is.Null);
    }

    [Test]
    public void Parse_TextWithEscapedHotkeyMarker_IgnoreHotkeyMarker ()
    {
      var result = HotkeyParser.Parse ("No &&Hotkey");
      Assert.That (result.Text, Is.EqualTo ("No &Hotkey"));
      Assert.That (result.Hotkey, Is.Null);
      Assert.That (result.HotkeyIndex, Is.Null);
    }

    [Test]
    public void Parse_TextWithEscapedHotkeyMarker_AndFollowedByHotkey ()
    {
      var result = HotkeyParser.Parse ("A &&&Hotkey");
      Assert.That (result.Text, Is.EqualTo ("A &Hotkey"));
      Assert.That (result.Hotkey, Is.EqualTo ('H'));
      Assert.That (result.HotkeyIndex, Is.EqualTo (3));
    }

    [Test]
    public void Parse_TextWithMultipleHotkeyMarkers_AndEscapedHotkeyMarkers_IgnoreHotkeyMarker_IntegrationTest ()
    {
      var result = HotkeyParser.Parse ("&Hotkey &&Integration &Test");
      Assert.That (result.Text, Is.EqualTo ("&Hotkey &&Integration &Test"));
      Assert.That (result.Hotkey, Is.Null);
      Assert.That (result.HotkeyIndex, Is.Null);
    }

    [Test]
    public void Parse_TextWithHotkey_AndIgnoredHotkeyMarkers_AndEscapedHotkeyMarkers_IntegrationTest ()
    {
      var result = HotkeyParser.Parse ("&&Hotkey & &Integration &&Test&");
      Assert.That (result.Text, Is.EqualTo ("&Hotkey & Integration &Test&"));
      Assert.That (result.Hotkey, Is.EqualTo ('I'));
      Assert.That (result.HotkeyIndex, Is.EqualTo (10));
    }

    [Test]
    public void Escape ()
    {
      Assert.That (HotkeyParser.Escape ("Foo & Bar && Foo Bar"), Is.EqualTo ("Foo && Bar &&&& Foo Bar"));
    }
    
    [Test]
    public void Escape_Empty ()
    {
      Assert.That (HotkeyParser.Escape (string.Empty), Is.Empty);
    }

    [Test]
    public void Escape_Null ()
    {
      Assert.That (HotkeyParser.Escape (null), Is.Null);
    }
  }
}