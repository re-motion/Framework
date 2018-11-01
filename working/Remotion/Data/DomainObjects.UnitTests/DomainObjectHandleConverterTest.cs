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
using Remotion.Data.DomainObjects.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class DomainObjectHandleConverterTest : StandardMappingTest
  {
    private IDomainObjectHandle<Order> _handle;
    private DomainObjectHandleConverter _converter;

    public override void SetUp ()
    {
      base.SetUp ();

      _handle = DomainObjectIDs.Order1.GetHandle<Order>();
      _converter = new DomainObjectHandleConverter();
    }

    [Test]
    public void CanConvertFrom_String_ReturnsTrue ()
    {
      Assert.That (_converter.CanConvertFrom (typeof (string)), Is.True);
    }

    [Test]
    public void CanConvertFrom_Other_ReturnsFalse ()
    {
      Assert.That (_converter.CanConvertFrom (typeof (object)), Is.False);
    }

    [Test]
    public void CanConvertTo_String_ReturnsTrue ()
    {
      Assert.That (_converter.CanConvertTo (typeof (string)), Is.True);
    }

    [Test]
    public void CanConvertTo_Other_ReturnsFalse ()
    {
      Assert.That (_converter.CanConvertTo (typeof (object)), Is.False);
    }

    [Test]
    public void ConvertFrom_String_SucceedsForObjectIDString ()
    {
      var objectIDString = DomainObjectIDs.Order1.ToString();

      var result = _converter.ConvertFrom (objectIDString);

      Assert.That (result, Is.EqualTo (DomainObjectIDs.Order1.GetHandle<Order>()));
    }

    [Test]
    public void ConvertFrom_String_FailsForOtherString ()
    {
      Assert.That (
          () => _converter.ConvertFrom ("dummy"),
          Throws.TypeOf<NotSupportedException>().With.Message.EqualTo ("The given string is not a valid ObjectID string."));
    }

    [Test]
    public void ConvertFrom_String_SucceedsForNullValue ()
    {
      var result = _converter.ConvertFrom (null);

      Assert.That (result, Is.Null);
    }

    [Test]
    public void ConvertFrom_OtherType_Fails ()
    {
      Assert.That (
          () => _converter.ConvertFrom (12),
          Throws.TypeOf<NotSupportedException>().With.Message.EqualTo ("This TypeConverter cannot convert from values of type 'System.Int32'."));
    }

    [Test]
    public void ConvertTo_String_ReturnsObjectIDString ()
    {
      var result = _converter.ConvertTo (_handle, typeof (string));

      Assert.That (result, Is.EqualTo (DomainObjectIDs.Order1.ToString()));
    }

    [Test]
    public void ConvertTo_OtherType_Fails ()
    {
      Assert.That (
          () => _converter.ConvertTo (_handle, typeof (int)),
          Throws.TypeOf<NotSupportedException>().With.Message.EqualTo ("This TypeConverter cannot convert to values of type 'System.Int32'."));
    }

    [Test]
    public void ConvertTo_Null_ReturnsNull ()
    {
      var result = _converter.ConvertTo (null, typeof (string));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void ConvertTo_WrongSourceObject_Fails ()
    {
      Assert.That (
          () => _converter.ConvertTo (12, typeof (string)),
          Throws.TypeOf<NotSupportedException>().With.Message.EqualTo (
              "This TypeConverter can only convert values of type 'Remotion.Data.DomainObjects.IDomainObjectHandle`1[Remotion.Data.DomainObjects.IDomainObject]'."));
    }

    [Test]
    public void Roundtrip ()
    {
      var result = _converter.ConvertFrom (_converter.ConvertTo (_handle, typeof (string)));

      Assert.That (result, Is.EqualTo (_handle));
    }
  }
}