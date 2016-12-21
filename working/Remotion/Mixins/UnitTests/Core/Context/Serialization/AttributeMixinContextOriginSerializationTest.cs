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
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.Context.Serialization;

namespace Remotion.Mixins.UnitTests.Core.Context.Serialization
{
  [TestFixture]
  public class AttributeMixinContextOriginSerializationTest
  {
    private AttributeMixinContextOriginSerializer _serializer;
    private AttributeMixinContextOriginDeserializer _invalidDeserializer;

    [SetUp]
    public void SetUp ()
    {
      _serializer = new AttributeMixinContextOriginSerializer ();
      _invalidDeserializer = new AttributeMixinContextOriginDeserializer (new object[] { 1, 2, 3 });
    }

    [Test]
    public void AddKind ()
    {
      _serializer.AddKind ("some kind");

      var deserializer = new AttributeMixinContextOriginDeserializer (_serializer.Values);
      Assert.That (deserializer.GetKind (), Is.EqualTo ("some kind"));
    }

    [Test]
    public void AddAssembly ()
    {
      var someAssembly = GetType().Assembly;
      _serializer.AddAssembly (someAssembly);

      var deserializer = new AttributeMixinContextOriginDeserializer (_serializer.Values);
      Assert.That (deserializer.GetAssembly (), Is.EqualTo (someAssembly));
    }

    [Test]
    public void GetLocation ()
    {
      _serializer.AddLocation ("some location");

      var deserializer = new AttributeMixinContextOriginDeserializer (_serializer.Values);
      Assert.That (deserializer.GetLocation (), Is.EqualTo ("some location"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Expected an array with 3 elements.\r\nParameter name: values")]
    public void Deserializer_InvalidArray ()
    {
      Dev.Null = new AttributeMixinContextOriginDeserializer (new[] { "x" });
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'System.String' at index 0 in the values array, but found 'System.Int32'.")]
    public void GetKind_Invalid ()
    {
      _invalidDeserializer.GetKind();
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'System.String' at index 1 in the values array, but found 'System.Int32'.")]
    public void GetAssembly_Invalid ()
    {
      _invalidDeserializer.GetAssembly();
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'System.String' at index 2 in the values array, but found 'System.Int32'.")]
    public void GetLocation_Invalid ()
    {
      _invalidDeserializer.GetLocation();
    }
  }
}