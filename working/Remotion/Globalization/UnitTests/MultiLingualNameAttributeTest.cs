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
using System.Globalization;
using NUnit.Framework;

namespace Remotion.Globalization.UnitTests
{
  [TestFixture]
  public class MultiLingualNameAttributeTest
  {
    [Test]
    public void Initialize_WithLocalizedNameAndCultureSet ()
    {
      var attribute = new MultiLingualNameAttribute ("The Name", "en-US");

      Assert.That (attribute.LocalizedName, Is.EqualTo ("The Name"));
      Assert.That (attribute.Culture, Is.EqualTo (CultureInfo.GetCultureInfo ("en-US")));
    }

    [Test]
    public void Initialize_WithCultureIsEmpty_UsesInvariantCulture()
    {
      var attribute = new MultiLingualNameAttribute ("The Name", "");

      Assert.That (attribute.LocalizedName, Is.EqualTo ("The Name"));
      Assert.That (attribute.Culture, Is.EqualTo (CultureInfo.InvariantCulture));
    }

    [Test]
    public void Initialize_WithCultureIsNull_ThrowsArgumentNullException()
    {
      // ReSharper disable AssignNullToNotNullAttribute
      Assert.That (() => new MultiLingualNameAttribute ("The Name", null), Throws.TypeOf<ArgumentNullException>());
      // ReSharper restore AssignNullToNotNullAttribute
    }

    [Test]
    public void Initialize_WithDe ()
    {
      var attribute = new DeAttribute ("The Name");

      Assert.That (attribute.LocalizedName, Is.EqualTo ("The Name"));
      Assert.That (attribute.Culture, Is.EqualTo (CultureInfo.GetCultureInfo ("de")));
    }

    [Test]
    public void Initialize_WithDeAT ()
    {
      var attribute = new DeATAttribute ("The Name");

      Assert.That (attribute.LocalizedName, Is.EqualTo ("The Name"));
      Assert.That (attribute.Culture, Is.EqualTo (CultureInfo.GetCultureInfo ("de-AT")));
    }

    [Test]
    public void Initialize_WithDeCH ()
    {
      var attribute = new DeCHAttribute ("The Name");

      Assert.That (attribute.LocalizedName, Is.EqualTo ("The Name"));
      Assert.That (attribute.Culture, Is.EqualTo (CultureInfo.GetCultureInfo ("de-CH")));
    }

    [Test]
    public void Initialize_WithDeDE ()
    {
      var attribute = new DeDEAttribute ("The Name");

      Assert.That (attribute.LocalizedName, Is.EqualTo ("The Name"));
      Assert.That (attribute.Culture, Is.EqualTo (CultureInfo.GetCultureInfo ("de-DE")));
    }

    [Test]
    public void Initialize_WithEn ()
    {
      var attribute = new EnAttribute ("The Name");

      Assert.That (attribute.LocalizedName, Is.EqualTo ("The Name"));
      Assert.That (attribute.Culture, Is.EqualTo (CultureInfo.GetCultureInfo ("en")));
    }

    [Test]
    public void Initialize_WithEnGB ()
    {
      var attribute = new EnGBAttribute ("The Name");

      Assert.That (attribute.LocalizedName, Is.EqualTo ("The Name"));
      Assert.That (attribute.Culture, Is.EqualTo (CultureInfo.GetCultureInfo ("en-GB")));
    }

    [Test]
    public void Initialize_WithEnUS ()
    {
      var attribute = new EnUSAttribute ("The Name");

      Assert.That (attribute.LocalizedName, Is.EqualTo ("The Name"));
      Assert.That (attribute.Culture, Is.EqualTo (CultureInfo.GetCultureInfo ("en-US")));
    }
  }
}