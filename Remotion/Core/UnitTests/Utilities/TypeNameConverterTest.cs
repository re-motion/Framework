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
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{
  [TestFixture]
  public class TypeNameConverterTest
  {
    // types

    // static members

    // member fields

    TypeNameConverter _converter;

    // construction and disposing

    public TypeNameConverterTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _converter = new TypeNameConverter ();
    }

    [Test]
    public void CanConvertToString ()
    {
      Assert.That (_converter.CanConvertTo (typeof (string)), Is.True);
    }

    [Test]
    public void CanConvertFromString ()
    {
      Assert.That (_converter.CanConvertFrom (typeof (string)), Is.True);
    }

    [Test]
    public void ConvertToString ()
    {
      Type destinationType = typeof (string);

      Assert.That (_converter.ConvertTo (null, null, null, destinationType), Is.EqualTo (""));
      Assert.That ((string) _converter.ConvertTo (null, null, typeof (TypeNameConverterTest), destinationType), Is.EqualTo ("Remotion.UnitTests.Utilities.TypeNameConverterTest, Remotion.UnitTests"));
    }

    [Test]
    public void ConvertFromString ()
    {
      Assert.That (_converter.ConvertFrom (null, null, ""), Is.EqualTo (null));
      Assert.That (_converter.ConvertFrom (null, null, "Remotion.UnitTests.Utilities.TypeNameConverterTest, Remotion.UnitTests"), Is.EqualTo (typeof (TypeNameConverterTest)));
      Assert.That (_converter.ConvertFrom (null, null, "Remotion.UnitTests::Utilities.TypeNameConverterTest"), Is.EqualTo (typeof (TypeNameConverterTest)));
    }
  }
}
