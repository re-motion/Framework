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
using System.Collections.Generic;
using NUnit.Framework;
using Remotion.ExtensibleEnums.UnitTests.TestDomain;

namespace Remotion.ExtensibleEnums.UnitTests
{
  [TestFixture]
  public class ExtensibleEnumConverterTest
  {
    private ExtensibleEnumConverter _converter;

    [SetUp]
    public void SetUp ()
    {
      _converter = new ExtensibleEnumConverter (typeof (Color));
    }

    [Test]
    public void CanConvertFrom_String ()
    {
      Assert.That (_converter.CanConvertFrom (null, typeof (string)), Is.True);
    }

    [Test]
    public void CanConvertFrom_OtherTypes ()
    {
      Assert.That (_converter.CanConvertFrom (null, typeof (object)), Is.False);
      Assert.That (_converter.CanConvertFrom (null, typeof (int)), Is.False);
      Assert.That (_converter.CanConvertFrom (null, typeof (Enum)), Is.False);
      Assert.That (_converter.CanConvertFrom (null, typeof (Color)), Is.False);
    }

    [Test]
    public void CanConvertTo_String ()
    {
      Assert.That (_converter.CanConvertTo (null, typeof (string)), Is.True);
    }

    [Test]
    public void CanConvertTo_OtherTypes ()
    {
      Assert.That (_converter.CanConvertTo (null, typeof (object)), Is.False);
      Assert.That (_converter.CanConvertTo (null, typeof (int)), Is.False);
      Assert.That (_converter.CanConvertTo (null, typeof (Enum)), Is.False);
      Assert.That (_converter.CanConvertTo (null, typeof (Color)), Is.False);
    }

    [Test]
    public void ConvertFrom_Null ()
    {
      var value = _converter.ConvertFrom (null, null, null);
      Assert.That (value, Is.Null);
    }

    [Test]
    public void ConvertFrom_String_Empty ()
    {
      var value = _converter.ConvertFrom (null, null, "");
      Assert.That (value, Is.Null);
    }

    [Test]
    public void ConvertFrom_String_CorrectID ()
    {
      var value = _converter.ConvertFrom (null, null, Color.Values.Red ().ID);
      Assert.That (value, Is.EqualTo (Color.Values.Red()));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException), ExpectedMessage = 
        "The extensible enum type 'Remotion.ExtensibleEnums.UnitTests.TestDomain.Color' does not define a value called '?'.")]
    public void ConvertFrom_String_WrongID ()
    {
      _converter.ConvertFrom (null, null, "?");
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "Cannot convert value from type 'System.Int32' to type 'Remotion.ExtensibleEnums.UnitTests.TestDomain.Color'.")]
    public void ConvertFrom_WrongType ()
    {
      _converter.ConvertFrom (null, null, 1);
    }

    [Test]
    public void ConvertTo_String_Null ()
    {
      var stringValue = _converter.ConvertTo (null, null, null, typeof (string));
      Assert.That (stringValue, Is.Null);
    }

    [Test]
    public void ConvertTo_String_Value ()
    {
      var stringValue = _converter.ConvertTo (null, null, Color.Values.Red(), typeof (string));
      Assert.That (stringValue, Is.EqualTo (Color.Values.Red().ID));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = 
        "Cannot convert values of type 'System.Int32' to type 'System.String'. This converter only supports values of type "
        + "'Remotion.ExtensibleEnums.UnitTests.TestDomain.Color'.")]
    public void ConvertTo_String_InvalidValue ()
    {
      _converter.ConvertTo (null, null, 12, typeof (string));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), 
        ExpectedMessage = "Cannot convert values to type 'System.Int32'. This converter only supports converting to type 'System.String'.")]
    public void ConvertTo_InvalidType ()
    {
      _converter.ConvertTo (null, null, Color.Values.Red(), typeof (int));
    }
  }
}