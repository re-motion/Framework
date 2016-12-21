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
using System.Linq;
using System.Runtime.Serialization;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.Context.Serialization;

namespace Remotion.Mixins.UnitTests.Core.Context.Serialization
{
  [TestFixture]
  public class AttributeClassContextSerializationTest
  {
    private AttributeClassContextSerializer _serializer;
    private AttributeClassContextDeserializer _invalidDeserializer;

    [SetUp]
    public void SetUp()
    {
      _serializer = new AttributeClassContextSerializer ();
      _invalidDeserializer = new AttributeClassContextDeserializer (new object[] { 1, 2, 3 });
    }

    [Test]
    public void AddClassType()
    {
      _serializer.AddClassType (typeof (DateTime));

      var deserializer = new AttributeClassContextDeserializer (_serializer.Values);
      Assert.That (deserializer.GetClassType (), Is.EqualTo (typeof (DateTime)));
    }

    [Test]
    public void AddMixins ()
    {
      var mixinContext1 = MixinContextObjectMother.Create (mixinType: typeof (string), origin: MixinContextOriginObjectMother.Create (assembly: GetType().Assembly));
      var mixinContext2 = MixinContextObjectMother.Create (mixinType: typeof (object));
      _serializer.AddMixins (new[] {mixinContext1, mixinContext2});

      // Check that the chain of serializers correctly sets up the AttributeMixinContextOriginSerializer
      var serializedMixinContexts = ((object[]) _serializer.Values[1]);
      var serializedMixinContext1 = (object[]) serializedMixinContexts[1];
      var serializedMixinOrigin = (object[]) serializedMixinContext1[4];
      var serializedMixinOriginAssembly = serializedMixinOrigin[1];
      Assert.That (serializedMixinOriginAssembly, Is.EqualTo (GetType ().Assembly.FullName));

      var deserializer = new AttributeClassContextDeserializer (_serializer.Values);
      Assert.That (deserializer.GetMixins ().ToArray (), Is.EqualTo (new[] { mixinContext1, mixinContext2 }));
    }

    [Test]
    public void AddComposedInterfaces ()
    {
      _serializer.AddComposedInterfaces (new[] {typeof (int), typeof (string)});

      var deserializer = new AttributeClassContextDeserializer (_serializer.Values);
      Assert.That (deserializer.GetComposedInterfaces (), Is.EqualTo (new[] { typeof (int), typeof (string) }));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Expected an array with 3 elements.\r\nParameter name: values")]
    public void Deserializer_InvalidArray()
    {
      Dev.Null = new AttributeClassContextDeserializer (new[] { "x" });
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'System.Type' at index 0 in the values array, but found 'System.Int32'.")]
    public void GetClassType_Invalid()
    {
      _invalidDeserializer.GetClassType ();
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'System.Object[]' at index 1 in the values array, but found 'System.Int32'.")]
    public void GetMixins_Invalid ()
    {
      _invalidDeserializer.GetMixins ();
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'System.Type[]' at index 2 in the values array, but found 'System.Int32'.")]
    public void GetComposedInterfaces_Invalid ()
    {
      _invalidDeserializer.GetComposedInterfaces();
    }
  }
}
