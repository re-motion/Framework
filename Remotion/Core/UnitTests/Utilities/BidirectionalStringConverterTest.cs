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
using System.Threading;
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.UnitTests.Utilities
{

[TestFixture]
public class BidirectionalStringConverterTest
{
  private BidirectionalStringConverter _converter;
  private CultureInfo _culture;

  public enum ConversionTestEnum
  {
    ValueA,
    ValueB
  }
  
  [SetUp]
  public void SetUp()
  {
    _converter = new BidirectionalStringConverter();
    _culture = Thread.CurrentThread.CurrentCulture;
    Thread.CurrentThread.CurrentCulture = new CultureInfo ("en-US");
  }

  [TearDown]
  public void TearDown()
  {
    Thread.CurrentThread.CurrentCulture = _culture;
  }

  [Test]
  public void CanConvertToByte()
  {
    Assert.That (_converter.CanConvertTo (typeof (byte)), Is.True);
  }

  [Test]
  public void CanConvertFromByte()
  {
    Assert.That (_converter.CanConvertFrom (typeof (byte)), Is.True);
  }

  [Test]
  public void CanConvertToInt16()
  {
    Assert.That (_converter.CanConvertTo (typeof (short)), Is.True);
  }

  [Test]
  public void CanConvertFromInt16()
  {
    Assert.That (_converter.CanConvertFrom (typeof (short)), Is.True);
  }

  [Test]
  public void CanConvertToInt32()
  {
    Assert.That (_converter.CanConvertTo (typeof (int)), Is.True);
  }

  [Test]
  public void CanConvertFromInt32()
  {
    Assert.That (_converter.CanConvertFrom (typeof (int)), Is.True);
  }

  [Test]
  public void CanConvertToInt64()
  {
    Assert.That (_converter.CanConvertTo (typeof (long)), Is.True);
  }

  [Test]
  public void CanConvertFromInt64()
  {
    Assert.That (_converter.CanConvertFrom (typeof (long)), Is.True);
  }

  [Test]
  public void CanConvertToSingle()
  {
    Assert.That (_converter.CanConvertTo (typeof (float)), Is.True);
  }

  [Test]
  public void CanConvertFromSingle()
  {
    Assert.That (_converter.CanConvertFrom (typeof (float)), Is.True);
  }

  [Test]
  public void CanConvertToDouble()
  {
    Assert.That (_converter.CanConvertTo (typeof (double)), Is.True);
  }

  [Test]
  public void CanConvertFromDouble()
  {
    Assert.That (_converter.CanConvertFrom (typeof (double)), Is.True);
  }

  [Test]
  public void CanConvertToDateTime()
  {
    Assert.That (_converter.CanConvertTo (typeof (DateTime)), Is.True);
  }

  [Test]
  public void CanConvertFromDateTime()
  {
    Assert.That (_converter.CanConvertFrom (typeof (DateTime)), Is.True);
  }

  [Test]
  public void CanConvertToBoolean()
  {
    Assert.That (_converter.CanConvertTo (typeof (bool)), Is.True);
  }

  [Test]
  public void CanConvertFromBoolean()
  {
    Assert.That (_converter.CanConvertFrom (typeof (bool)), Is.True);
  }

  [Test]
  public void CanConvertToDecimal()
  {
    Assert.That (_converter.CanConvertTo (typeof (decimal)), Is.True);
  }

  [Test]
  public void CanConvertFromDecimal()
  {
    Assert.That (_converter.CanConvertFrom (typeof (decimal)), Is.True);
  }

  [Test]
  public void CanConvertToGuid()
  {
    Assert.That (_converter.CanConvertTo (typeof (Guid)), Is.True);
  }

  [Test]
  public void CanConvertFromGuid()
  {
    Assert.That (_converter.CanConvertFrom (typeof (Guid)), Is.True);
  }

  [Test]
  public void CanConvertToInt32Array()
  {
    Assert.That (_converter.CanConvertTo (typeof (int[])), Is.True);
  }

  [Test]
  public void CanConvertFromInt32Array()
  {
    Assert.That (_converter.CanConvertFrom (typeof (int[])), Is.True);
  }

  [Test]
  public void CanConvertToObject()
  {
    Assert.That (_converter.CanConvertTo (typeof (object)), Is.False);
  }

  [Test]
  public void CanConvertFromObject()
  {
    Assert.That (_converter.CanConvertFrom (typeof (object)), Is.False);
  }

  [Test]
  public void CanConvertToDBNull()
  {
    Assert.That (_converter.CanConvertTo (typeof (DBNull)), Is.True);
  }

  [Test]
  public void CanConvertFromDBNull()
  {
    Assert.That (_converter.CanConvertFrom (typeof (DBNull)), Is.True);
  }


  [Test]
  public void ConvertToByte()
  {
    Type destinationType = typeof (short);

    Assert.That (_converter.ConvertTo ("0", destinationType), Is.EqualTo ((short) 0));
    Assert.That (_converter.ConvertTo ("1", destinationType), Is.EqualTo ((short) 1));
  }

  [Test]
  public void ConvertFromByte()
  {
    Assert.That (_converter.ConvertFrom ((byte) 0), Is.EqualTo ("0"));
    Assert.That (_converter.ConvertFrom ((byte) 1), Is.EqualTo ("1"));
  }

  [Test]
  public void ConvertToInt16()
  {
    Type destinationType = typeof (byte);

    Assert.That (_converter.ConvertTo ("0", destinationType), Is.EqualTo ((byte) 0));
    Assert.That (_converter.ConvertTo ("1", destinationType), Is.EqualTo ((byte) 1));
  }

  [Test]
  public void ConvertFromInt16()
  {
    Assert.That (_converter.ConvertFrom ((short) 0), Is.EqualTo ("0"));
    Assert.That (_converter.ConvertFrom ((short) 1), Is.EqualTo ("1"));
  }

  [Test]
  public void ConvertToInt32()
  {
    Type destinationType = typeof (int);

    Assert.That (_converter.ConvertTo ("0", destinationType), Is.EqualTo (0));
    Assert.That (_converter.ConvertTo ("1", destinationType), Is.EqualTo (1));
  }

  [Test]
  public void ConvertFromInt32()
  {
    Assert.That (_converter.ConvertFrom (0), Is.EqualTo ("0"));
    Assert.That (_converter.ConvertFrom (1), Is.EqualTo ("1"));
  }

  [Test]
  public void ConvertToInt64()
  {
    Type destinationType = typeof (long);

    Assert.That (_converter.ConvertTo ("0", destinationType), Is.EqualTo (0L));
    Assert.That (_converter.ConvertTo ("1", destinationType), Is.EqualTo (1L));
  }

  [Test]
  public void ConvertFromInt64()
  {
    Assert.That (_converter.ConvertFrom (0L), Is.EqualTo ("0"));
    Assert.That (_converter.ConvertFrom (1L), Is.EqualTo ("1"));
  }

  [Test]
  public void ConvertToSingle()
  {
    Type destinationType = typeof (float);

    Assert.That (_converter.ConvertTo ("0", destinationType), Is.EqualTo (0F));
    Assert.That (_converter.ConvertTo ("1.5", destinationType), Is.EqualTo (1.5F));
    Assert.That (_converter.ConvertTo (float.MinValue.ToString("R"), destinationType), Is.EqualTo (float.MinValue));
    Assert.That (_converter.ConvertTo (float.MaxValue.ToString("R"), destinationType), Is.EqualTo (float.MaxValue));
  }

  [Test]
  public void ConvertFromSingle()
  {
    Assert.That (_converter.ConvertFrom (0F), Is.EqualTo ("0"));
    Assert.That (_converter.ConvertFrom (1.5F), Is.EqualTo ("1.5"));
    Assert.That (_converter.ConvertFrom (float.MinValue), Is.EqualTo (float.MinValue.ToString("R")));
    Assert.That (_converter.ConvertFrom (float.MaxValue), Is.EqualTo (float.MaxValue.ToString("R")));
  }

  [Test]
  public void ConvertToDouble()
  {
    Type destinationType = typeof (double);

    Assert.That (_converter.ConvertTo ("0", destinationType), Is.EqualTo (0));
    Assert.That (_converter.ConvertTo ("1.5", destinationType), Is.EqualTo (1.5));
    Assert.That (_converter.ConvertTo (double.MinValue.ToString("R"), destinationType), Is.EqualTo (double.MinValue));
    Assert.That (_converter.ConvertTo (double.MaxValue.ToString("R"), destinationType), Is.EqualTo (double.MaxValue));
  }

  [Test]
  public void ConvertFromDouble()
  {
    Assert.That (_converter.ConvertFrom (0), Is.EqualTo ("0"));
    Assert.That (_converter.ConvertFrom (1.5), Is.EqualTo ("1.5"));
    Assert.That (_converter.ConvertFrom (double.MinValue), Is.EqualTo (double.MinValue.ToString("R")));
    Assert.That (_converter.ConvertFrom (double.MaxValue), Is.EqualTo (double.MaxValue.ToString("R")));
  }

  [Test]
  public void ConvertToDateTime()
  {
    Type destinationType = typeof (DateTime);
    DateTime dateTime = new DateTime (2005, 12, 24, 13, 30, 30, 0);
    string dateTimeString = dateTime.ToString();

    Assert.That (_converter.ConvertTo (dateTimeString, destinationType), Is.EqualTo (dateTime));
  }

  [Test]
  public void ConvertFromDateTime()
  {
    DateTime dateTime = new DateTime (2005, 12, 24, 13, 30, 30, 0);
    string dateTimeString = dateTime.ToString();

    Assert.That (_converter.ConvertFrom (dateTime), Is.EqualTo (dateTimeString));
  }

  [Test]
  public void ConvertToBoolean()
  {
    Type destinationType = typeof (bool);

    Assert.That (_converter.ConvertTo ("True", destinationType), Is.EqualTo (true));
    Assert.That (_converter.ConvertTo ("False", destinationType), Is.EqualTo (false));
  }

  [Test]
  public void ConvertFromBoolean()
  {
    Assert.That (_converter.ConvertFrom (true), Is.EqualTo ("True"));
    Assert.That (_converter.ConvertFrom (false), Is.EqualTo ("False"));
  }

  [Test]
  public void ConvertToDecimal()
  {
    Type destinationType = typeof (double);

    Assert.That (_converter.ConvertTo ("0", destinationType), Is.EqualTo (0m));
    Assert.That (_converter.ConvertTo ("1.5", destinationType), Is.EqualTo (1.5m));
  }

  [Test]
  public void ConvertFromDecimal()
  {
    Assert.That (_converter.ConvertFrom (0m), Is.EqualTo ("0"));
    Assert.That (_converter.ConvertFrom (1.5m), Is.EqualTo ("1.5"));
  }

  [Test]
  public void ConvertToGuid()
  {
    Type destinationType = typeof (Guid);

    Guid guid = Guid.NewGuid();
    Assert.That (_converter.ConvertTo (guid.ToString(), destinationType), Is.EqualTo (guid));
    Assert.That (_converter.ConvertTo (Guid.Empty.ToString(), destinationType), Is.EqualTo (Guid.Empty));
  }

  [Test]
  public void ConvertFromGuid()
  {
    Guid guid = Guid.NewGuid();
    Assert.That (_converter.ConvertFrom (guid), Is.EqualTo (guid.ToString()));
    Assert.That (_converter.ConvertFrom (Guid.Empty), Is.EqualTo (Guid.Empty.ToString()));
  }

  [Test]
  public void ConvertToInt32Array()
  {
    Type destinationType = typeof (int[]);
    object value = _converter.ConvertTo ("0, 1", destinationType);
    Assert.That (value, Is.Not.Null);
    Assert.That (value.GetType(), Is.EqualTo (typeof (int[])));
    int[] values = (int[])value;
    Assert.That (values.Length, Is.EqualTo (2));
    Assert.That (values[0], Is.EqualTo (0));
    Assert.That (values[1], Is.EqualTo (1));
  }

  [Test]
  public void ConvertFromInt32Array()
  {
    Assert.That (_converter.ConvertFrom (new int[] {0, 1}), Is.EqualTo ("0,1"));
  }

  [Test]
  public void ConvertToNullableInt32 ()
  {
    Type destinationType = typeof (int?);

    Assert.That (_converter.ConvertTo ("0", destinationType), Is.EqualTo (0));
    Assert.That (_converter.ConvertTo ("1", destinationType), Is.EqualTo (1));
    Assert.That (_converter.ConvertTo (string.Empty, destinationType), Is.EqualTo (null));
    Assert.That (_converter.ConvertTo (null, destinationType), Is.EqualTo (null));
  }

  [Test]
  public void ConvertFromNullableInt32 ()
  {
    Assert.That (_converter.ConvertFrom ((int?) 0), Is.EqualTo ("0"));
    Assert.That (_converter.ConvertFrom ((int?) 1), Is.EqualTo ("1"));
    Assert.That (_converter.ConvertFrom ((int?) null), Is.EqualTo (string.Empty));
  }

  [Test]
  [ExpectedException (typeof (ParseException))]
  public void ConvertToInt32WithNull()
  {
    _converter.ConvertTo (null, typeof (int));
  }

  [Test]
  public void ConvertFromNull()
  {
    Assert.That (_converter.ConvertFrom (null), Is.EqualTo (string.Empty));
  }

  [Test]
  public void ConvertToDBNull()
  {
    Assert.That (_converter.ConvertTo ("", typeof (DBNull)), Is.EqualTo (DBNull.Value));
  }

  [Test]
  public void ConvertFromDBNull()
  {
    Assert.That (_converter.ConvertFrom (DBNull.Value), Is.EqualTo (""));
  }
}            

}
