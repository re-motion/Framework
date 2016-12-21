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
  public class FlatClassContextSerializationTest
  {
    private FlatClassContextSerializer _serializer;

    [SetUp]
    public void SetUp()
    {
      _serializer = new FlatClassContextSerializer();
    }

    [Test]
    public void AddClassType ()
    {
      _serializer.AddClassType (typeof (DateTime));
      Assert.That (_serializer.Values[0], Is.EqualTo (typeof (DateTime).AssemblyQualifiedName));

      var deserializer = new FlatClassContextDeserializer (_serializer.Values);
      Assert.That (deserializer.GetClassType (), Is.SameAs (typeof (DateTime)));
    }

    [Test]
    public void AddMixins ()
    {
      var mixinContext1 = MixinContextObjectMother.Create (mixinType: typeof (DateTime));
      var mixinContext2 = MixinContextObjectMother.Create (mixinType: typeof (int));
      var mixinContexts = new[] { mixinContext1, mixinContext2 };
      _serializer.AddMixins (mixinContexts);

      // Check that the chain of serializers correctly sets up the FlatMixinContextOriginSerializer
      var serializedMixins = ((object[]) _serializer.Values[1]);
      Assert.That (serializedMixins, Has.Length.EqualTo (2));
      var serializedMixin1 = (object[]) serializedMixins[0];
      Assert.That (serializedMixin1[0], Is.EqualTo (typeof (DateTime).AssemblyQualifiedName));

      var deserializer = new FlatClassContextDeserializer (_serializer.Values);
      Assert.That (deserializer.GetMixins ().ToArray (), Is.EqualTo (mixinContexts));
    }

    [Test]
    public void AddComposedInterfaces ()
    {
      _serializer.AddComposedInterfaces (new[] {typeof (int), typeof (string)});
      Assert.That (_serializer.Values[2], Is.EqualTo (new[] {typeof (int).AssemblyQualifiedName, typeof (string).AssemblyQualifiedName}));

      var deserializer = new FlatClassContextDeserializer (_serializer.Values);
      Assert.That (deserializer.GetComposedInterfaces ().ToArray (), Is.EqualTo (new[] { typeof (int), typeof (string) }));
    }
  }
}
