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
using System.Reflection;
using NUnit.Framework;

namespace Remotion.Reflection.UnitTests
{
  [TestFixture]
  public class PropertyInfoAdapterConverterTest
  {
    private PropertyInfo _propertyInfo;
    private PropertyInfoAdapter _propertyInfoAdapter;
    private PropertyInfoAdapterConverter _converter;

    [SetUp]
    public void SetUp ()
    {
      _propertyInfo = typeof (string).GetProperty ("Length");
      _propertyInfoAdapter = PropertyInfoAdapter.Create(_propertyInfo);

      _converter = new PropertyInfoAdapterConverter ();
    }

    [Test]
    public void CanConvertFrom_PropertyInfo_IsTrue ()
    {
      Assert.That (_converter.CanConvertFrom (null, typeof (PropertyInfo)), Is.True);
    }

    [Test]
    public void CanConvertFrom_OtherTypes_IsFalse ()
    {
      Assert.That (_converter.CanConvertFrom (null, typeof (object)), Is.False);
      Assert.That (_converter.CanConvertFrom (null, typeof (string)), Is.False);
    }

    [Test]
    public void CanConvertTo_PropertyInfo_IsTrue ()
    {
      Assert.That (_converter.CanConvertTo (null, typeof (PropertyInfo)), Is.True);
    }

    [Test]
    public void CanConvertTo_OtherTypes_IsFalse ()
    {
      Assert.That (_converter.CanConvertTo (null, typeof (object)), Is.False);
      Assert.That (_converter.CanConvertTo (null, typeof (string)), Is.False);
    }

    [Test]
    public void ConvertFrom_Null ()
    {
      var value = _converter.ConvertFrom (null, null, null);
      Assert.That (value, Is.Null);
    }

    [Test]
    public void ConvertFrom_PropertyInfo ()
    {
      var value = _converter.ConvertFrom (null, null, _propertyInfo);
      Assert.That (value, Is.EqualTo (_propertyInfoAdapter));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "Cannot convert value from type 'System.String' to type 'Remotion.Reflection.PropertyInfoAdapter'.")]
    public void ConvertFrom_InvalidType ()
    {
      _converter.ConvertFrom (null, null, "string");
    }

    [Test]
    public void ConvertTo_Null ()
    {
      var value = _converter.ConvertTo (null, null, null, typeof (PropertyInfo));
      Assert.That (value, Is.Null);
    }

    [Test]
    public void ConvertTo_PropertyInfo ()
    {
      var value = _converter.ConvertTo (null, null, _propertyInfoAdapter, typeof (PropertyInfo));
      Assert.That (value, Is.EqualTo (_propertyInfo));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "Cannot convert values to type 'System.String'. This converter only supports converting to type 'System.Reflection.PropertyInfo'.")]
    public void ConvertTo_InvalidType ()
    {
      _converter.ConvertTo (null, null, null, typeof (string));
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "Cannot convert values of type 'System.String' to type 'System.Reflection.PropertyInfo'. "
        + "This converter only supports values of type 'Remotion.Reflection.PropertyInfoAdapter'.")]
    public void ConvertTo_InvalidValue ()
    {
      _converter.ConvertTo (null, null, "string", typeof (PropertyInfo));
    }
  }
}