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
  public class AttributeMixinContextSerializationTest
  {
    private AttributeMixinContextSerializer _serializer;
    private AttributeMixinContextDeserializer _invalidDeserializer;

    [SetUp]
    public void SetUp()
    {
      _serializer = new AttributeMixinContextSerializer ();
      _invalidDeserializer = new AttributeMixinContextDeserializer (new object[] {1, 2, 3, 4, 5});
    }

    [Test]
    public void AddMixinType()
    {
      _serializer.AddMixinType (typeof (DateTime));

      var deserializer = new AttributeMixinContextDeserializer (_serializer.Values);
      Assert.That (deserializer.GetMixinType (), Is.EqualTo (typeof (DateTime)));
    }

    [Test]
    public void AddMixinKind()
    {
      _serializer.AddMixinKind (MixinKind.Used);

      var deserializer = new AttributeMixinContextDeserializer (_serializer.Values);
      Assert.That (deserializer.GetMixinKind (), Is.EqualTo (MixinKind.Used));
    }

    [Test]
    public void AddIntroducedMemberVisibility ()
    {
      _serializer.AddIntroducedMemberVisibility (MemberVisibility.Public);

      var deserializer = new AttributeMixinContextDeserializer (_serializer.Values);
      Assert.That (deserializer.GetIntroducedMemberVisibility (), Is.EqualTo (MemberVisibility.Public));
    }

    [Test]
    public void AddExplicitDependencies ()
    {
      _serializer.AddExplicitDependencies (new[] { typeof (int), typeof (string) });

      var deserializer = new AttributeMixinContextDeserializer (_serializer.Values);
      Assert.That (deserializer.GetExplicitDependencies (), Is.EqualTo (new[] { typeof (int), typeof (string) }));
    }

    [Test]
    public void AddOrigin ()
    {
      var mixinContextOrigin = MixinContextOriginObjectMother.Create (assembly: GetType ().Assembly);
      _serializer.AddOrigin (mixinContextOrigin);

      // Check that the chain of serializers correctly sets up the AttributeMixinContextOriginSerializer
      var serializedMixinOrigin = (object[]) _serializer.Values[4];
      var serializedMixinOriginAssembly = serializedMixinOrigin[1];
      Assert.That (serializedMixinOriginAssembly, Is.EqualTo (GetType ().Assembly.FullName));

      var deserializer = new AttributeMixinContextDeserializer (_serializer.Values);
      Assert.That (deserializer.GetOrigin (), Is.EqualTo (mixinContextOrigin));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Expected an array with 5 elements.\r\nParameter name: values")]
    public void Deserializer_InvalidArray()
    {
      Dev.Null = new AttributeMixinContextDeserializer (new[] { "x" });
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'System.Type' at index 0 in the values array, but found 'System.Int32'.")]
    public void GetMixinType_Invalid()
    {
      _invalidDeserializer.GetMixinType ();
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'Remotion.Mixins.MixinKind' at index 1 in the values array, but found 'System.Int32'.")]
    public void GetMixinKind_Invalid ()
    {
      _invalidDeserializer.GetMixinKind ();
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'Remotion.Mixins.MemberVisibility' at index 2 in the values array, but found 'System.Int32'.")]
    public void GetIntroducedMemberVisibility_Invalid ()
    {
      _invalidDeserializer.GetIntroducedMemberVisibility();
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'System.Type[]' at index 3 in the values array, but found 'System.Int32'.")]
    public void GetExplicitDependencies_Invalid ()
    {
      _invalidDeserializer.GetExplicitDependencies ();
    }

    [Test]
    [ExpectedException (typeof (SerializationException),
        ExpectedMessage = "Expected value of type 'System.Object[]' at index 4 in the values array, but found 'System.Int32'.")]
    public void GetOrigin_Invalid ()
    {
      _invalidDeserializer.GetOrigin ();
    }
  }
}
