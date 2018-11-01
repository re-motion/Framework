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
using NUnit.Framework;
using Remotion.Mixins.Context.Serialization;

namespace Remotion.Mixins.UnitTests.Core.Context.Serialization
{
  [TestFixture]
  public class FlatMixinContextSerializationTest
  {
    private FlatMixinContextSerializer _serializer;

    [SetUp]
    public void SetUp()
    {
      _serializer = new FlatMixinContextSerializer();
    }

    [Test]
    public void AddMixinType ()
    {
      _serializer.AddMixinType (typeof (DateTime));
      Assert.That (_serializer.Values[0], Is.EqualTo (typeof (DateTime).AssemblyQualifiedName));

      var deserializer = new FlatMixinContextDeserializer (_serializer.Values);
      Assert.That (deserializer.GetMixinType(), Is.SameAs (typeof (DateTime)));
    }

    [Test]
    public void AddMixinKind ()
    {
      _serializer.AddMixinKind (MixinKind.Used);
      Assert.That (_serializer.Values[1], Is.EqualTo (MixinKind.Used));

      var deserializer = new FlatMixinContextDeserializer (_serializer.Values);
      Assert.That (deserializer.GetMixinKind (), Is.EqualTo (MixinKind.Used));
    }

    [Test]
    public void AddIntroducedMemberVisibility ()
    {
      _serializer.AddIntroducedMemberVisibility (MemberVisibility.Public);
      Assert.That (_serializer.Values[2], Is.EqualTo (MemberVisibility.Public));

      var deserializer = new FlatMixinContextDeserializer (_serializer.Values);
      Assert.That (deserializer.GetIntroducedMemberVisibility (), Is.EqualTo (MemberVisibility.Public));
    }

    [Test]
    public void AddExplicitDependencies ()
    {
      _serializer.AddExplicitDependencies (new[] {typeof (int), typeof (string)});
      Assert.That (_serializer.Values[3], 
          Is.EqualTo (new[] {typeof (int).AssemblyQualifiedName, typeof (string).AssemblyQualifiedName}));

      var deserializer = new FlatMixinContextDeserializer (_serializer.Values);
      Assert.That (deserializer.GetExplicitDependencies ().ToArray (), Is.EqualTo (new[] { typeof (int), typeof (string) }));
    }

    [Test]
    public void AddOrigin ()
    {
      var mixinContextOrigin = MixinContextOriginObjectMother.Create (assembly: GetType ().Assembly);
      _serializer.AddOrigin (mixinContextOrigin);

      // Check that the chain of serializers correctly sets up the FlatMixinContextOriginSerializer
      var serializedMixinOrigin = (object[]) _serializer.Values[4];
      var serializedMixinOriginAssembly = serializedMixinOrigin[1];
      Assert.That (serializedMixinOriginAssembly, Is.EqualTo (GetType ().Assembly.FullName));

      var deserializer = new FlatMixinContextDeserializer (_serializer.Values);
      Assert.That (deserializer.GetOrigin (), Is.EqualTo (mixinContextOrigin));
    }
  }
}
